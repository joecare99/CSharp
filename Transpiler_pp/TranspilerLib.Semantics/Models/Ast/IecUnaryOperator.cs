namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Defines the supported unary operators for the first typed IEC AST slice.
/// </summary>
public enum IecUnaryOperator
{
    /// <summary>
    /// Leaves the operand unchanged.
    /// </summary>
    Plus,

    /// <summary>
    /// Negates the numeric operand.
    /// </summary>
    Negate,

    /// <summary>
    /// Applies logical negation to the operand.
    /// </summary>
    Not,
}
