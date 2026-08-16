using System;
using Config.Service;

namespace RnzTrauer.Acquisition;

/// <summary>
/// Configuration section for web scraping and data acquisition. Stored in JSON under a stable key.
/// </summary>
public sealed class AcquisitionConfig
{
    /// <summary>Maximum number of obituaries to import per run.</summary>
    public int MaxItemsPerRun { get; set; } = 100;

    /// <summary>Delay in milliseconds between requests (anti-bot delay).</summary>
    public int RequestDelay { get; set; } = 500;

    /// <summary>Timeout in seconds for HTTP requests.</summary>
    public int HttpTimeout { get; set; } = 30;

    /// <summary>Whether to enable retries for failed requests.</summary>
    public bool EnableRetries { get; set; } = true;

    /// <summary>Number of retry attempts (max 3).</summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>User agent string for HTTP requests.</summary>
    public string? UserAgent { get; set; } = 
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
}
