using GenInterfaces.Data;

namespace GenFreeBrowser.Places.Interface;

public interface IPlaceSearchService
{
    Task<IReadOnlyList<GenPlaceMatch>> SearchAllAsync(GenPlaceQuery query, CancellationToken ct = default);
}
