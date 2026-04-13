namespace RnzTrauer.Core;

/// <summary>
/// Describes the RNZ scraper configuration loaded from JSON.
/// </summary>
public sealed class RnzConfig : DatabaseSettings
{
    private readonly IConfigLoader? _xConfigLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="RnzConfig"/> class.
    /// </summary>
    public RnzConfig()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RnzConfig"/> class with a configuration loader dependency.
    /// </summary>
    public RnzConfig(IConfigLoader xConfigLoader)
    {
        _xConfigLoader = xConfigLoader ?? throw new ArgumentNullException(nameof(xConfigLoader));
    }

    /// <summary>
    /// Gets or sets the login URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected page title after navigation.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the login user name.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the login password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the local storage root.
    /// </summary>
    public string LocalPath { get; set; } = string.Empty;

    /// <summary>
    /// Loads the configuration from a JSON file.
    /// </summary>
    public RnzConfig Load(string sFilePath)
    {
        return (_xConfigLoader ?? throw new InvalidOperationException("No configuration loader has been provided.")).Load<RnzConfig>(sFilePath);
    }
}
