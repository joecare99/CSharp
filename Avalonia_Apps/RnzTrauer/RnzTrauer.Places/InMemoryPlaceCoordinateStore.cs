using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public sealed class InMemoryPlaceCoordinateStore : IPlaceCoordinateStore
{
    private readonly Dictionary<string, PlaceCoordinate> _entries =
        new(StringComparer.OrdinalIgnoreCase);

    public Task<PlaceCoordinate?> GetAsync(
        string place,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(place);
        cancellationToken.ThrowIfCancellationRequested();
        _entries.TryGetValue(PlaceNormalizer.Normalize(place) ?? string.Empty, out var result);
        return Task.FromResult(result);
    }

    public Task SaveAsync(
        PlaceCoordinate coordinate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(coordinate);
        cancellationToken.ThrowIfCancellationRequested();
        var key = PlaceNormalizer.Normalize(coordinate.Place);
        if (key is null)
            throw new ArgumentException("Place must not be empty.", nameof(coordinate));
        if (coordinate.Latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(coordinate), "Latitude must be between -90 and 90.");
        if (coordinate.Longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(coordinate), "Longitude must be between -180 and 180.");

        _entries[key] = coordinate with { Place = key };
        return Task.CompletedTask;
    }
}
