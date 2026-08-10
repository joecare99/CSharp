using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public interface IPlaceCoordinateStore
{
    Task<PlaceCoordinate?> GetAsync(
        string place,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        PlaceCoordinate coordinate,
        CancellationToken cancellationToken = default);
}
