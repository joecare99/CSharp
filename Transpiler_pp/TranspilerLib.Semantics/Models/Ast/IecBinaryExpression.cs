namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC binary expression.
/// </summary>
public sealed class IecBinaryExpression : IecExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecBinaryExpression"/> class.
    /// </summary>
    /// <param name="left">The left operand expression.</param>
    /// <param name="operatorType">The binary operator.</param>
    /// <param name="right">The right operand expression.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecBinaryExpression(IecExpression left, IecBinaryOperator operatorType, IecExpression right, int sourcePos = -1)
        : base(sourcePos)
    {
        Left = left;
        OperatorType = operatorType;
        Right = right;
    }

    /// <summary>
    /// Gets the left operand expression.
    /// </summary>
    public IecExpression Left { get; }

    /// <summary>
    /// Gets the binary operator.
    /// </summary>
    public IecBinaryOperator OperatorType { get; }

    /// <summary>
    /// Gets the right operand expression.
    /// </summary>
    public IecExpression Right { get; }
}
