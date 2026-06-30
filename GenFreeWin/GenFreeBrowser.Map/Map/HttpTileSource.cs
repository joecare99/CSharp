using GenFreeBrowser.Map.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace GenFreeBrowser.Map;

public sealed class HttpTileSource : ITileSource
{
    private readonly IMapProvider _provider;
    private readonly ITileCache _cache;
    private readonly HttpClient _client;

    public HttpTileSource(IMapProvider provider, ITileCache cache, HttpClient? client = null)
    {
        _provider = provider;
        _cache = cache;
        _client = client ?? new HttpClient();
    }

    public async Task<byte[]?> GetTileAsync(TileId id)
    {
        var cached = await _cache.TryGetAsync(id,_provider.Id).ConfigureAwait(false);
        if (cached != null) return cached;

        var url = _provider.GetTileUrl(id);
        if (url == null) return null;

        var data = await _client.GetByteArrayAsync(url).ConfigureAwait(false);
        await _cache.StoreAsync(id, _provider.Id, data).ConfigureAwait(false);
        return data;
    }
}
