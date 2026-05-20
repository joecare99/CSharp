namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC return statement.
/// </summary>
public sealed class IecReturnStatement : IecStatement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecReturnStatement"/> class.
    /// </summary>
    /// <param name="expression">The returned expression.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecReturnStatement(IecExpression expression, int sourcePos = -1)
        : base(sourcePos)
    {
        Expression = expression;
    }

    /// <summary>
    /// Gets the returned expression.
    /// </summary>
    public IecExpression Expression { get; }
}
