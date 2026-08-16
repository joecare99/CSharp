using System;
using System.IO;
using Config.Service;

namespace RnzTrauer.Places;

/// <summary>
/// Configuration section for geocoding and place data storage. Stored in JSON under a stable key.
/// </summary>
public sealed class PlaceConfig
{
    /// <summary>Geocoding service provider (Google Maps, Bing, etc.).</summary>
    [SensitiveConfigProperty]
    public string? GeocodingApiKey { get; set; }

    /// <summary>Cache directory for geocoding results.</summary>
    public string? CacheDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "RnzTrauer", "cache", "geocoding");

    /// <summary>Number of concurrent geocoding requests (default 5).</summary>
    public int MaxConcurrentRequests { get; set; } = 5;

    /// <summary>Timeout in seconds for geocoding API calls.</summary>
    public int RequestTimeout { get; set; } = 10;

    /// <summary>Whether to use cached geocoding results (true by default).</summary>
    public bool EnableCaching { get; set; } = true;
}
