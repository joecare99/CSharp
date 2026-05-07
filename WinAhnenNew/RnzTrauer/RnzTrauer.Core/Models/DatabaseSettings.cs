using System.Security;

namespace RnzTrauer.Core;

/// <summary>
/// Provides database connection settings shared by the ported applications.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the database user name.
    /// </summary>
    public string DBuser { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database password.
    /// </summary>
    public SecureString? DBpass { get; set; } = default;

    /// <summary>
    /// Gets or sets the database host name.
    /// </summary>
    public string DBhost { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string DB { get; set; } = string.Empty;
}
