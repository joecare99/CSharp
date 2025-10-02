using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenFreeBrowser.Map;

namespace GenFreeBrowser.Places.Interface;

public sealed record PlaceQuery(string Text, string? Country = null, int MaxResults = 20);

public sealed record PlaceResult(
    string Id,
    string Name,
    GeoPoint Location,
    IReadOnlyList<string> Hierarchy,
    string Source
);

public interface IPlaceAuthority
{
    string Name { get; }
    Task<IReadOnlyList<PlaceResult>> SearchAsync(PlaceQuery query, CancellationToken ct = default);
}

public interface IPlaceSearchService
{
    Task<IReadOnlyList<PlaceResult>> SearchAllAsync(PlaceQuery query, CancellationToken ct = default);
}
