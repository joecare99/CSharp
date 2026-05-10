using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GenInterfaces.Data;

namespace GenFreeBrowser.Places.Interface;

public interface IPlaceSearchService
{
    Task<IReadOnlyList<GenPlaceMatch>> SearchAllAsync(GenPlaceQuery query, CancellationToken ct = default);
}
