namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Defines the language-independent artifact kind for a shared semantic compilation unit.
/// </summary>
public enum IecArtifactKind
{
    /// <summary>
    /// Represents a function-style artifact.
    /// </summary>
    Function,

    /// <summary>
    /// Represents a function-block-style artifact.
    /// </summary>
    FunctionBlock,

    /// <summary>
    /// Represents a program-style artifact.
    /// </summary>
    Program,
}
