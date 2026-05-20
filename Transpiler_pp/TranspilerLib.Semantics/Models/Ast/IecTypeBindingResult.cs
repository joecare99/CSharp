using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents the result of inferring IEC expression and statement types.
/// </summary>
public sealed class IecTypeBindingResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecTypeBindingResult"/> class.
    /// </summary>
    /// <param name="expressionTypes">The inferred expression types.</param>
    /// <param name="statementTypes">The inferred statement target types.</param>
    /// <param name="unresolvedExpressions">The expressions whose type could not be determined.</param>
    public IecTypeBindingResult(
        IEnumerable<KeyValuePair<IecExpression, string>>? expressionTypes = null,
        IEnumerable<KeyValuePair<IecStatement, string>>? statementTypes = null,
        IEnumerable<IecExpression>? unresolvedExpressions = null)
    {
        ExpressionTypes = expressionTypes?.ToArray() ?? [];
        StatementTypes = statementTypes?.ToArray() ?? [];
        UnresolvedExpressions = unresolvedExpressions?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets the inferred expression types.
    /// </summary>
    public IReadOnlyList<KeyValuePair<IecExpression, string>> ExpressionTypes { get; }

    /// <summary>
    /// Gets the inferred statement target types.
    /// </summary>
    public IReadOnlyList<KeyValuePair<IecStatement, string>> StatementTypes { get; }

    /// <summary>
    /// Gets the expressions whose type could not be determined.
    /// </summary>
    public IReadOnlyList<IecExpression> UnresolvedExpressions { get; }
}
