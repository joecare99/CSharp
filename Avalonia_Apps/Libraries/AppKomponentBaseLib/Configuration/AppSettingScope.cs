namespace AppKomponentBaseLib.Configuration;

/// <summary>
/// Identifies the persistence scope of a setting value.
/// </summary>
public enum AppSettingScope
{
    /// <summary>
    /// The setting belongs to the application-wide configuration.
    /// </summary>
    Application,

    /// <summary>
    /// The setting belongs to the current user profile or preferences.
    /// </summary>
    User,

    /// <summary>
    /// The setting belongs to a workspace-like context.
    /// </summary>
    Workspace
}
