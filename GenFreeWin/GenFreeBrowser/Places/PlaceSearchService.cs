using GenInterfaces.Data;
using GenInterfaces.Interfaces.Authorities;
using GenFreeBrowser.Places.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GenFreeBrowser.Places;

public sealed class PlaceSearchService : IPlaceSearchService
{
    private readonly IEnumerable<IGenPlaceAuthority> _authorities;

    public PlaceSearchService(IEnumerable<IGenPlaceAuthority> authorities)
    {
        _authorities = authorities;
    }

    public async Task<IReadOnlyList<GenPlaceMatch>> SearchAllAsync(GenPlaceQuery query, CancellationToken ct = default)
    {
        var tasks = _authorities.Select(a => a.SearchPlacesAsync(query, ct));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r).ToList();
    }
}
