using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC conditional statement.
/// </summary>
public sealed class IecIfStatement : IecStatement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecIfStatement"/> class.
    /// </summary>
    /// <param name="condition">The conditional expression.</param>
    /// <param name="thenStatements">The statements emitted when the condition is true.</param>
    /// <param name="elseStatements">The statements emitted when the condition is false.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecIfStatement(IecExpression condition, IEnumerable<IecStatement>? thenStatements = null, IEnumerable<IecStatement>? elseStatements = null, int sourcePos = -1)
        : base(sourcePos)
    {
        Condition = condition;
        ThenStatements = thenStatements?.ToArray() ?? Array.Empty<IecStatement>();
        ElseStatements = elseStatements?.ToArray() ?? Array.Empty<IecStatement>();
    }

    /// <summary>
    /// Gets the conditional expression.
    /// </summary>
    public IecExpression Condition { get; }

    /// <summary>
    /// Gets the statements emitted when the condition is true.
    /// </summary>
    public IReadOnlyList<IecStatement> ThenStatements { get; }

    /// <summary>
    /// Gets the statements emitted when the condition is false.
    /// </summary>
    public IReadOnlyList<IecStatement> ElseStatements { get; }
}
