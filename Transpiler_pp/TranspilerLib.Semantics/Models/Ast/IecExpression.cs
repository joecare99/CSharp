namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents the base type for typed IEC expressions.
/// </summary>
public abstract class IecExpression : IecAstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecExpression"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecExpression(int sourcePos)
        : base(sourcePos)
    {
    }
}
