using GenFreeBrowser.Places.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GenFreeBrowser.Places;

public sealed class PlaceSearchService : IPlaceSearchService
{
    private readonly IEnumerable<IPlaceAuthority> _authorities;

    public PlaceSearchService(IEnumerable<IPlaceAuthority> authorities)
    {
        _authorities = authorities;
    }

    public async Task<IReadOnlyList<PlaceResult>> SearchAllAsync(PlaceQuery query, CancellationToken ct = default)
    {
        var tasks = _authorities.Select(a => a.SearchAsync(query, ct));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToList();
    }
}
