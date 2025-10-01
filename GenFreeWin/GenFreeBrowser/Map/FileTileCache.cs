using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenFreeBrowser.Map;

public sealed class FileTileCache : ITileCache
{
    private readonly string _root;

    public FileTileCache(string root)
    {
        _root = root;
        Directory.CreateDirectory(_root);
    }

    private string GetPath(TileId id,int pId)
    {
        // Layout: root/z/x/pID_y.png
        var zDir = Path.Combine(_root, id.Z.ToString());
        var xDir = Path.Combine(zDir, id.X.ToString());
        Directory.CreateDirectory(xDir);
        return Path.Combine(xDir, $"{pId:X}_{id.Y}.png");
    }

    public async Task<byte[]?> TryGetAsync(TileId id, int pId)
    {
        var path = GetPath(id,pId);
        if (File.Exists(path))
            return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        return null;
    }

    public async Task StoreAsync(TileId id, int pId, byte[] data)
    {
        var path = GetPath(id,pId);
        await File.WriteAllBytesAsync(path, data).ConfigureAwait(false);
    }
}
