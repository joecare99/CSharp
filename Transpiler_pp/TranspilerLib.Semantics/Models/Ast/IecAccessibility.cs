namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Defines language-independent accessibility metadata for shared semantic artifacts.
/// </summary>
public enum IecAccessibility
{
    /// <summary>
    /// Uses the default accessibility for the target mapping.
    /// </summary>
    Default,

    /// <summary>
    /// Represents public accessibility.
    /// </summary>
    Public,

    /// <summary>
    /// Represents internal accessibility.
    /// </summary>
    Internal,

    /// <summary>
    /// Represents protected accessibility.
    /// </summary>
    Protected,

    /// <summary>
    /// Represents private accessibility.
    /// </summary>
    Private,
}
