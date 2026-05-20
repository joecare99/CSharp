namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC literal expression.
/// </summary>
public sealed class IecLiteralExpression : IecExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecLiteralExpression"/> class.
    /// </summary>
    /// <param name="value">The literal value.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecLiteralExpression(object? value, int sourcePos = -1)
        : base(sourcePos)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the literal value.
    /// </summary>
    public object? Value { get; }
}
