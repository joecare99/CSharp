using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC function call expression.
/// </summary>
public sealed class IecFunctionCallExpression : IecExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecFunctionCallExpression"/> class.
    /// </summary>
    /// <param name="functionName">The called IEC function name.</param>
    /// <param name="arguments">The function call arguments.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecFunctionCallExpression(string functionName, IEnumerable<IecExpression>? arguments = null, int sourcePos = -1)
        : base(sourcePos)
    {
        FunctionName = functionName;
        Arguments = arguments?.ToArray() ?? Array.Empty<IecExpression>();
    }

    /// <summary>
    /// Gets the called IEC function name.
    /// </summary>
    public string FunctionName { get; }

    /// <summary>
    /// Gets the typed function call arguments.
    /// </summary>
    public IReadOnlyList<IecExpression> Arguments { get; }
}
