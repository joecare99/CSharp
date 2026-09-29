namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Provides a shared base for granular statement blocks.
/// </summary>
public abstract class IecStatementBlock : IecGranularCodeBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecStatementBlock"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecStatementBlock(int sourcePos = -1)
        : base(IecGranularBlockKind.Statement, sourcePos)
    {
    }
}
