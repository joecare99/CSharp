using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public sealed record GeocodingResult(
    string Query,
    double? Latitude,
    double? Longitude,
    string? DisplayName,
    bool IsApproximate);

public interface IGeocodingAdapter
{
    Task<GeocodingResult?> ResolveAsync(string query, CancellationToken cancellationToken = default);
}
