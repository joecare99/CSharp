using System.Threading.Tasks;

namespace GenFreeBrowser.Map.Interfaces;

public interface ITileCache
{
    Task<byte[]?> TryGetAsync(TileId id, string pId);
    Task StoreAsync(TileId id, string pId, byte[] data);
}
