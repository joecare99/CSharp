using System;
using Config.Service;

namespace RnzTrauer.Persistence.MySql;

/// <summary>
/// Configuration section for the MySQL persistence layer. Stored in JSON under a stable key.
/// </summary>
public sealed class DatabaseConfig
{
    /// <summary>MySQL server hostname or connection string.</summary>
    [SensitiveConfigProperty]
    public string? Server { get; set; }

    /// <summary>MySQL TCP port (default 3306).</summary>
    public int Port { get; set; } = 3306;

    /// <summary>Database user account.</summary>
    [SensitiveConfigProperty]
    public string? User { get; set; }

    /// <summary>Database password.</summary>
    [SensitiveConfigProperty]
    public string? Password { get; set; }

    /// <summary>Target database name for obituary notices and places.</summary>
    public string? DatabaseName { get; set; } = "rnz_trauer";

    /// <summary>Connection timeout in seconds (0 = no timeout).</summary>
    public int ConnectionTimeout { get; set; } = 30;

    /// <summary>Whether to pool connections for better performance.</summary>
    public bool EnablePooling { get; set; } = true;
}