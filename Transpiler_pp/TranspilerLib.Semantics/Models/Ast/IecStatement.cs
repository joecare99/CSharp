namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents the base type for typed IEC statements.
/// </summary>
public abstract class IecStatement : IecAstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecStatement"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecStatement(int sourcePos)
        : base(sourcePos)
    {
    }
}
