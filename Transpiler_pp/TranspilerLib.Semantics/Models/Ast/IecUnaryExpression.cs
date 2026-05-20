namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC unary expression.
/// </summary>
public sealed class IecUnaryExpression : IecExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecUnaryExpression"/> class.
    /// </summary>
    /// <param name="operatorType">The unary operator.</param>
    /// <param name="operand">The operand expression.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecUnaryExpression(IecUnaryOperator operatorType, IecExpression operand, int sourcePos = -1)
        : base(sourcePos)
    {
        OperatorType = operatorType;
        Operand = operand;
    }

    /// <summary>
    /// Gets the unary operator.
    /// </summary>
    public IecUnaryOperator OperatorType { get; }

    /// <summary>
    /// Gets the operand expression.
    /// </summary>
    public IecExpression Operand { get; }
}
