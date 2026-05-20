namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC identifier expression.
/// </summary>
public sealed class IecIdentifierExpression : IecExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecIdentifierExpression"/> class.
    /// </summary>
    /// <param name="identifier">The referenced identifier name.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecIdentifierExpression(string identifier, int sourcePos = -1)
        : base(sourcePos)
    {
        Identifier = identifier;
    }

    /// <summary>
    /// Gets the referenced identifier name.
    /// </summary>
    public string Identifier { get; }
}
