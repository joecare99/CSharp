namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Describes a typed granular IEC code block that exposes a semantic classification.
/// </summary>
public interface IIecGranularCodeBlock
{
    /// <summary>
    /// Gets the semantic kind of the block.
    /// </summary>
    IecGranularBlockKind Kind { get; }
}
