namespace Config.Service;

/// <summary>
/// Editor kind for a single configuration property. The value determines which
/// control a configuration UI binds to (text box, check box, or combo box).
/// </summary>
public enum ConfigPropertyKind
{
    /// <summary>Free-form text values such as names, paths, and file names.</summary>
    Text = 0,

    /// <summary>Numeric values such as ports, sizes, and timeouts.</summary>
    Number = 1,

    /// <summary>Boolean on/off switches.</summary>
    Boolean = 2,

    /// <summary>Closed choice sets defined by an <c>enum</c> type.</summary>
    Enum = 3,
}
