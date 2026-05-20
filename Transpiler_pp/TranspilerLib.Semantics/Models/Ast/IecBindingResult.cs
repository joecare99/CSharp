using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents the result of binding identifier expressions against a symbol table.
/// </summary>
public sealed class IecBindingResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecBindingResult"/> class.
    /// </summary>
    /// <param name="bindings">The successful identifier bindings.</param>
    /// <param name="unresolvedIdentifiers">The identifiers that could not be resolved.</param>
    public IecBindingResult(IEnumerable<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>>? bindings = null, IEnumerable<IecIdentifierExpression>? unresolvedIdentifiers = null)
    {
        Bindings = bindings?.ToArray() ?? [];
        UnresolvedIdentifiers = unresolvedIdentifiers?.ToArray() ?? [];
    }

    /// <summary>
    /// Gets the successful bindings between identifier expressions and declarations.
    /// </summary>
    public IReadOnlyList<KeyValuePair<IecIdentifierExpression, IecVariableDeclaration>> Bindings { get; }

    /// <summary>
    /// Gets the identifier expressions that could not be resolved.
    /// </summary>
    public IReadOnlyList<IecIdentifierExpression> UnresolvedIdentifiers { get; }
}
