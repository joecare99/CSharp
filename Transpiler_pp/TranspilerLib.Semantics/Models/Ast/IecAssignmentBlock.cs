namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Provides a shared base for granular assignment blocks.
/// </summary>
public abstract class IecAssignmentBlock : IecGranularCodeBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecAssignmentBlock"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecAssignmentBlock(int sourcePos = -1)
        : base(IecGranularBlockKind.Assignment, sourcePos)
    {
    }
}
