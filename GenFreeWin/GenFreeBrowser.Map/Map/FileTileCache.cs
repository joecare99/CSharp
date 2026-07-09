using GenFreeBrowser.Map.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GenFreeBrowser.Map;

public sealed class FileTileCache : ITileCache
{
    private readonly string _root;
    private readonly long? _maxBytes;
    private readonly TimeSpan? _maxAge;
    private DateTime _nextCleanupTime = DateTime.MinValue; // throttle cleanup frequency
    private readonly object _cleanupLock = new();
    private readonly IFileSystem _fs;

    public FileTileCache(string root, long? maxBytes = null, TimeSpan? maxAge = null, IFileSystem? fileSystem = null)
    {
        _root = root;
        _maxBytes = maxBytes;
        _maxAge = maxAge;
        _fs = fileSystem ?? new LocalFileSystem();
        _fs.EnsureDirectory(_root);
    }

    private string GetPath(TileId id, string pId)
    {
        // Layout: root/z/x/pID_y.png
        var zDir = Path.Combine(_root, id.Z.ToString());
        var xDir = Path.Combine(zDir, id.X.ToString());
        _fs.EnsureDirectory(xDir);
        return Path.Combine(xDir, $"{pId}_{id.Y}.png");
    }

    public async Task<byte[]?> TryGetAsync(TileId id, string pId)
    {
        var path = GetPath(id, pId);
        if (_fs.FileExists(path))
        {
            return await _fs.ReadAllBytesAsync(path).ConfigureAwait(false);
        }
        return null;
    }

    public async Task StoreAsync(TileId id, string pId, byte[] data)
    {
        var path = GetPath(id, pId);
        await _fs.WriteAllBytesAsync(path, data).ConfigureAwait(false);

        if (NeedsCleanupTrigger())
        {
            _ = Task.Run(Cleanup); // fire & forget
        }
    }

    private bool NeedsCleanupTrigger()
    {
        if (_maxBytes is null && _maxAge is null) return false;
        var now = DateTime.UtcNow;
        if (now >= _nextCleanupTime)
        {
            _nextCleanupTime = now.AddMinutes(1);
            return true;
        }
        return false;
    }

    private void Cleanup()
    {
        if (_maxBytes is null && _maxAge is null) return;
        try
        {
            lock (_cleanupLock)
            {
                if (!_fs.DirectoryExists(_root)) return;

                var files = _fs.EnumerateFiles(_root, "*.png", recursive: true)
                    .Select(p =>
                    {
                        try
                        {
                            var meta = _fs.GetFileMetadata(p);
                            return new { Path = p, meta.Length, LastWriteUtc = meta.LastWriteUtc };
                        }
                        catch { return null; }
                    })
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();

                if (files.Count == 0) return;

                if (_maxAge is not null)
                {
                    var threshold = DateTime.UtcNow - _maxAge.Value;
                    foreach (var f in files.Where(f => f.LastWriteUtc < threshold))
                    {
                        _fs.TryDeleteFile(f.Path);
                    }
                    files = files.Where(f => _fs.FileExists(f.Path)).ToList();
                }

                if (_maxBytes is not null)
                {
                    long total = files.Sum(f => f.Length);
                    if (total > _maxBytes.Value)
                    {
                        foreach (var f in files.OrderBy(f => f.LastWriteUtc))
                        {
                            if (total <= _maxBytes.Value) break;
                            if (_fs.TryDeleteFile(f.Path))
                            {
                                total -= f.Length;
                            }
                        }
                    }
                }

                TryPruneEmptyDirs(_root);
            }
        }
        catch { }
    }

    private void TryPruneEmptyDirs(string root)
    {
        try
        {
            foreach (var dir in _fs.EnumerateDirectories(root, recursive: true).OrderByDescending(d => d.Length))
            {
                try
                {
                    if (_fs.DirectoryExists(dir) && _fs.GetEntryCount(dir) == 0)
                    {
                        _fs.TryDeleteDirectory(dir);
                    }
                }
                catch { }
            }
        }
        catch { }
    }
}
