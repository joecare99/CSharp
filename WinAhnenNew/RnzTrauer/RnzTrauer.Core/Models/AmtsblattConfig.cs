namespace RnzTrauer.Core;

/// <summary>
/// Describes the Amtsblatt loader configuration loaded from JSON.
/// </summary>
public sealed class AmtsblattConfig : DatabaseSettings
{
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
    public static AmtsblattConfig Load(string sFilePath)
    {
        return ConfigLoader.Load<AmtsblattConfig>(sFilePath);
    }
}
