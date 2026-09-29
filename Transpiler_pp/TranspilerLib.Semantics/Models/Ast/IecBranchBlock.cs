namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Provides a shared base for granular branch or conditional blocks.
/// </summary>
public abstract class IecBranchBlock : IecStatementBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecBranchBlock"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecBranchBlock(int sourcePos = -1)
        : base(sourcePos)
    {
    }

    /// <summary>
    /// Gets the semantic kind of the block.
    /// </summary>
    public sealed override IecGranularBlockKind Kind => IecGranularBlockKind.Branch;
}
