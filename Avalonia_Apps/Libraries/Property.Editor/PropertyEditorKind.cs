namespace Property.Editor;

/// <summary>
/// Identifies the neutral editing behavior required for a property value.
/// </summary>
public enum PropertyEditorKind
{
    /// <summary>A scalar value with no specialized editor requirement.</summary>
    Scalar = 0,

    /// <summary>A Boolean value.</summary>
    Boolean = 1,

    /// <summary>An enum value with optional supplied choices.</summary>
    Enum = 2,
}
