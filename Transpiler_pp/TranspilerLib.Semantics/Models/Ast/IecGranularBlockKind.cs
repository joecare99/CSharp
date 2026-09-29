namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Describes the coarse-grained semantic category of a granular IEC code block.
/// </summary>
public enum IecGranularBlockKind
{
    /// <summary>
    /// An unspecified or not yet classified semantic block.
    /// </summary>
    Unknown,

    /// <summary>
    /// A generic statement block.
    /// </summary>
    Statement,

    /// <summary>
    /// An assignment statement block.
    /// </summary>
    Assignment,

    /// <summary>
    /// A branch or conditional statement block.
    /// </summary>
    Branch,

    /// <summary>
    /// A loop statement block.
    /// </summary>
    Loop
}
