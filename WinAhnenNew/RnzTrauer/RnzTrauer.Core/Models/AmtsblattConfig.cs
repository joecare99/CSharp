namespace RnzTrauer.Core;

/// <summary>
/// Describes the Amtsblatt loader configuration loaded from JSON.
/// </summary>
public sealed class AmtsblattConfig : DatabaseSettings
{
    private readonly IConfigLoader? _xConfigLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmtsblattConfig"/> class.
    /// </summary>
    public AmtsblattConfig()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AmtsblattConfig"/> class with a configuration loader dependency.
    /// </summary>
    public AmtsblattConfig(IConfigLoader xConfigLoader)
    {
        _xConfigLoader = xConfigLoader ?? throw new ArgumentNullException(nameof(xConfigLoader));
    }

    /// <summary>
    /// Gets or sets the page URL prefix.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expected page title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the local storage root.
    /// </summary>
    public string LocalPath { get; set; } = string.Empty;

    /// <summary>
    /// Loads the configuration from a JSON file.
    /// </summary>
    public AmtsblattConfig Load(string sFilePath)
    {
        return (_xConfigLoader ?? throw new InvalidOperationException("No configuration loader has been provided.")).Load<AmtsblattConfig>(sFilePath);
    }
}
