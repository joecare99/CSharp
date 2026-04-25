using System.Text;
using BaseLib.Models.Interfaces;
using FBParser.Models.Interfaces;

namespace FBParserTests.Models;

internal sealed class FakeFileSystem : IFile, IDirectory, IPath
{
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);
    private int _tempFileCounter;

    public bool Exists(string sPath)
        => _files.ContainsKey(sPath) || _directories.Contains(sPath);

    public Stream OpenRead(string sPath)
        => new MemoryStream(ReadAllBytes(sPath), writable: false);

    public Stream OpenWrite(string sPath)
        => throw new NotSupportedException();

    public Stream Create(string sPath)
        => throw new NotSupportedException();

    public string ReadAllText(string sPath)
        => ReadAllText(sPath, Encoding.UTF8);

    public string ReadAllText(string sPath, Encoding encoding)
        => encoding.GetString(ReadAllBytes(sPath));

    public void WriteAllText(string sPath, string sContents)
        => WriteAllText(sPath, sContents, Encoding.UTF8);

    public void WriteAllText(string sPath, string sContents, Encoding encoding)
        => _files[sPath] = encoding.GetBytes(sContents);

    public byte[] ReadAllBytes(string sPath)
        => _files.TryGetValue(sPath, out var bytes) ? bytes : throw new FileNotFoundException(sPath);

    public void WriteAllBytes(string sPath, byte[] rgBytes)
        => _files[sPath] = rgBytes;

    public void Delete(string sPath)
    {
        _files.Remove(sPath);
        _directories.Remove(sPath);
    }

    public void Copy(string sSourceFileName, string sDestFileName, bool xOverwrite)
    {
        if (!xOverwrite && _files.ContainsKey(sDestFileName))
        {
            throw new IOException("Destination exists.");
        }

        _files[sDestFileName] = ReadAllBytes(sSourceFileName).ToArray();
    }

    public void Move(string sSourceFileName, string sDestFileName)
    {
        _files[sDestFileName] = ReadAllBytes(sSourceFileName).ToArray();
        _files.Remove(sSourceFileName);
    }

    public void CreateDirectory(string sPath)
        => _directories.Add(sPath);

    public string GetFullPath(string sPath)
        => sPath.StartsWith("ROOT:", StringComparison.OrdinalIgnoreCase) ? sPath : "ROOT:" + sPath;

    public string? GetDirectoryName(string sPath)
    {
        var normalized = sPath.Replace('/', '\\');
        var index = normalized.LastIndexOf('\\');
        return index < 0 ? null : normalized[..index];
    }

    public string GetTempFileName()
    {
        _tempFileCounter++;
        var path = $"ROOT_temp{_tempFileCounter}.tmp";
        _files[path] = [];
        return path;
    }

    public StreamWriter CreateStreamWriter(string sPath)
        => new(CreateWriteStream(sPath), new UTF8Encoding(false), leaveOpen: false);

    private Stream CreateWriteStream(string sPath)
        => new FakeWriteBackStream(this, sPath);

    private sealed class FakeWriteBackStream : MemoryStream
    {
        private readonly FakeFileSystem _fileSystem;
        private readonly string _path;

        public FakeWriteBackStream(FakeFileSystem fileSystem, string path)
        {
            _fileSystem = fileSystem;
            _path = path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileSystem._files[_path] = ToArray();
            }

            base.Dispose(disposing);
        }
    }
}
