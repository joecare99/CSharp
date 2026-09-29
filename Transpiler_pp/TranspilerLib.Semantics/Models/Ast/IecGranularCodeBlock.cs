namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Provides a shared base for granular semantic code blocks.
/// </summary>
public abstract class IecGranularCodeBlock : IecAstNode, IIecGranularCodeBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecGranularCodeBlock"/> class.
    /// </summary>
    /// <param name="kind">The semantic kind of the block.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecGranularCodeBlock(IecGranularBlockKind kind, int sourcePos = -1)
        : base(sourcePos)
    {
        Kind = kind;
    }

    /// <summary>
    /// Gets the semantic kind of the block.
    /// </summary>
    public virtual IecGranularBlockKind Kind { get; }
}
