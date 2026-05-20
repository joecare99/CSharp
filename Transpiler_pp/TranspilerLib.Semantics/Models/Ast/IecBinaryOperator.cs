namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Defines the supported binary operators for the first typed IEC AST slice.
/// </summary>
public enum IecBinaryOperator
{
    /// <summary>
    /// Adds the right operand to the left operand.
    /// </summary>
    Add,

    /// <summary>
    /// Subtracts the right operand from the left operand.
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplies the two operands.
    /// </summary>
    Multiply,

    /// <summary>
    /// Divides the left operand by the right operand.
    /// </summary>
    Divide,

    /// <summary>
    /// Compares the operands for equality.
    /// </summary>
    Equal,

    /// <summary>
    /// Compares the operands for inequality.
    /// </summary>
    NotEqual,

    /// <summary>
    /// Checks whether the left operand is less than the right operand.
    /// </summary>
    LessThan,

    /// <summary>
    /// Checks whether the left operand is less than or equal to the right operand.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Checks whether the left operand is greater than the right operand.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Checks whether the left operand is greater than or equal to the right operand.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Applies logical conjunction.
    /// </summary>
    And,

    /// <summary>
    /// Applies logical disjunction.
    /// </summary>
    Or,
}
