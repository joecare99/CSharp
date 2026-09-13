namespace Config.UI;

/// <summary>
/// Represents the visible lifecycle state of a configuration section.
/// </summary>
public enum ConfigUiSectionState
{
    /// <summary>The section has not been loaded.</summary>
    Unavailable,

    /// <summary>The section is loading persisted values.</summary>
    Loading,

    /// <summary>The section is ready for display or editing.</summary>
    Ready,

    /// <summary>The section is saving values.</summary>
    Saving,

    /// <summary>The section is resetting persisted values.</summary>
    Resetting,

    /// <summary>The last operation failed and exposes an error message.</summary>
    Failed,
}
