namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC assignment statement.
/// </summary>
public sealed class IecAssignmentStatement : IecStatement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecAssignmentStatement"/> class.
    /// </summary>
    /// <param name="target">The assignment target.</param>
    /// <param name="value">The assigned expression value.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecAssignmentStatement(IecIdentifierExpression target, IecExpression value, int sourcePos = -1)
        : base(sourcePos)
    {
        Target = target;
        Value = value;
    }

    /// <summary>
    /// Gets the assignment target expression.
    /// </summary>
    public IecIdentifierExpression Target { get; }

    /// <summary>
    /// Gets the expression assigned to the target.
    /// </summary>
    public IecExpression Value { get; }
}
