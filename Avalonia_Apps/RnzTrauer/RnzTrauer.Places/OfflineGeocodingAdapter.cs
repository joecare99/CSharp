using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public sealed class OfflineGeocodingAdapter : IGeocodingAdapter
{
    private readonly IReadOnlyDictionary<string, GeocodingResult> _entries;

    public OfflineGeocodingAdapter(IReadOnlyDictionary<string, GeocodingResult> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        _entries = new Dictionary<string, GeocodingResult>(entries, StringComparer.OrdinalIgnoreCase);
    }

    public Task<GeocodingResult?> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        cancellationToken.ThrowIfCancellationRequested();
        _entries.TryGetValue(PlaceNormalizer.Normalize(query) ?? string.Empty, out var result);
        return Task.FromResult(result);
    }
}
