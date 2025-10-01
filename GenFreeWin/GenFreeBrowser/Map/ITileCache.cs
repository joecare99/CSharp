using System.Threading.Tasks;

namespace GenFreeBrowser.Map;

public interface ITileCache
{
    Task<byte[]?> TryGetAsync(TileId id, int pId);
    Task StoreAsync(TileId id, int pId, byte[] data);
}
