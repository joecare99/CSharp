using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Places;

public sealed class GeocodingRateLimitException : Exception
{
    public GeocodingRateLimitException(TimeSpan retryAfter)
        : base($"Geocoding rate limit active. Retry after {retryAfter}.")
    {
        RetryAfter = retryAfter;
    }

    public TimeSpan RetryAfter { get; }
}

public sealed record GeocodingPolicyDiagnostics(
    int CacheHits,
    int CacheMisses,
    int RemoteRequests,
    int RateLimitRejections,
    TimeSpan? LastRetryAfter);

public sealed class CachingGeocodingAdapter : IGeocodingAdapter
{
    private readonly IGeocodingAdapter _inner;
    private readonly TimeSpan _cacheDuration;
    private readonly TimeSpan _minimumInterval;
    private readonly TimeProvider _clock;
    private readonly Dictionary<string, CacheEntry> _cache = new(StringComparer.OrdinalIgnoreCase);
    private DateTimeOffset? _lastRemoteRequest;
    private int _cacheHits;
    private int _cacheMisses;
    private int _remoteRequests;
    private int _rateLimitRejections;
    private TimeSpan? _lastRetryAfter;

    public CachingGeocodingAdapter(
        IGeocodingAdapter inner,
        TimeSpan cacheDuration,
        TimeSpan minimumInterval,
        TimeProvider? clock = null)
    {
        ArgumentNullException.ThrowIfNull(inner);
        if (cacheDuration < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(cacheDuration));
        if (minimumInterval < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(minimumInterval));

        _inner = inner;
        _cacheDuration = cacheDuration;
        _minimumInterval = minimumInterval;
        _clock = clock ?? TimeProvider.System;
    }

    public async Task<GeocodingResult?> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        cancellationToken.ThrowIfCancellationRequested();
        var key = PlaceNormalizer.Normalize(query) ?? string.Empty;
        var now = _clock.GetUtcNow();

        if (_cache.TryGetValue(key, out var cached)
            && now - cached.StoredAt <= _cacheDuration)
        {
            _cacheHits++;
            return cached.Result;
        }

        _cacheMisses++;
        if (_lastRemoteRequest is not null)
        {
            var elapsed = now - _lastRemoteRequest.Value;
            if (elapsed < _minimumInterval)
            {
                var retryAfter = _minimumInterval - elapsed;
                _rateLimitRejections++;
                _lastRetryAfter = retryAfter;
                throw new GeocodingRateLimitException(retryAfter);
            }
        }

        _lastRemoteRequest = now;
        _remoteRequests++;
        var result = await _inner.ResolveAsync(query, cancellationToken).ConfigureAwait(false);
        if (result is not null)
            _cache[key] = new CacheEntry(result, now);
        return result;
    }

    public GeocodingPolicyDiagnostics GetDiagnostics() =>
        new(
            _cacheHits,
            _cacheMisses,
            _remoteRequests,
            _rateLimitRejections,
            _lastRetryAfter);

    private sealed record CacheEntry(GeocodingResult Result, DateTimeOffset StoredAt);
}
