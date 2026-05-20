using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Performs lightweight type inference for the currently supported typed IEC AST subset.
/// The binder uses declaration types, identifier bindings, literals, operators, and a small
/// set of known IEC function names so later code generation can rely on stable baseline types.
/// </summary>
public static class IecTypeBinder
{
    private static readonly IReadOnlyDictionary<string, string> _knownFunctionReturnTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["ABS"] = "LREAL",
        ["LIMIT"] = "LREAL",
        ["SEL"] = "LREAL",
        ["SQRT"] = "LREAL",
        ["SIGN"] = "LREAL",
        ["rw_ABS"] = "INT",
    };

    /// <summary>
    /// Infers expression and statement types for the supplied compilation unit.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit to analyze.</param>
    /// <param name="bindingResult">The identifier binding result for the same compilation unit.</param>
    /// <returns>The type binding result.</returns>
    public static IecTypeBindingResult Bind(IecCompilationUnit compilationUnit, IecBindingResult bindingResult)
    {
        var expressionTypes = new List<KeyValuePair<IecExpression, string>>();
        var statementTypes = new List<KeyValuePair<IecStatement, string>>();
        var unresolvedExpressions = new List<IecExpression>();
        var identifierTypes = bindingResult.Bindings
            .GroupBy(binding => binding.Key, ReferenceEqualityComparer<IecIdentifierExpression>.Instance)
            .ToDictionary(group => group.Key, group => group.Last().Value.TypeName, ReferenceEqualityComparer<IecIdentifierExpression>.Instance);
        var cache = new Dictionary<IecExpression, string?>(ReferenceEqualityComparer<IecExpression>.Instance);

        foreach (var statement in compilationUnit.Statements)
        {
            BindStatementTypes(statement, identifierTypes, cache, expressionTypes, statementTypes, unresolvedExpressions);
        }

        return new IecTypeBindingResult(expressionTypes, statementTypes, unresolvedExpressions.Distinct(ReferenceEqualityComparer<IecExpression>.Instance));
    }

    private static void BindStatementTypes(
        IecStatement statement,
        IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes,
        IDictionary<IecExpression, string?> cache,
        ICollection<KeyValuePair<IecExpression, string>> expressionTypes,
        ICollection<KeyValuePair<IecStatement, string>> statementTypes,
        ICollection<IecExpression> unresolvedExpressions)
    {
        switch (statement)
        {
            case IecAssignmentStatement assignment:
                var targetType = InferExpressionType(assignment.Target, identifierTypes, cache, expressionTypes, unresolvedExpressions);
                if (targetType != null)
                {
                    statementTypes.Add(new KeyValuePair<IecStatement, string>(statement, targetType));
                }

                InferExpressionType(assignment.Value, identifierTypes, cache, expressionTypes, unresolvedExpressions);
                break;
            case IecIfStatement ifStatement:
                var conditionType = InferExpressionType(ifStatement.Condition, identifierTypes, cache, expressionTypes, unresolvedExpressions);
                if (conditionType != null)
                {
                    statementTypes.Add(new KeyValuePair<IecStatement, string>(statement, conditionType));
                }

                foreach (var thenStatement in ifStatement.ThenStatements)
                {
                    BindStatementTypes(thenStatement, identifierTypes, cache, expressionTypes, statementTypes, unresolvedExpressions);
                }

                foreach (var elseStatement in ifStatement.ElseStatements)
                {
                    BindStatementTypes(elseStatement, identifierTypes, cache, expressionTypes, statementTypes, unresolvedExpressions);
                }
                break;
            case IecReturnStatement returnStatement:
                var returnType = InferExpressionType(returnStatement.Expression, identifierTypes, cache, expressionTypes, unresolvedExpressions);
                if (returnType != null)
                {
                    statementTypes.Add(new KeyValuePair<IecStatement, string>(statement, returnType));
                }
                break;
        }
    }

    private static string? InferExpressionType(
        IecExpression expression,
        IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes,
        IDictionary<IecExpression, string?> cache,
        ICollection<KeyValuePair<IecExpression, string>> expressionTypes,
        ICollection<IecExpression> unresolvedExpressions)
    {
        if (cache.TryGetValue(expression, out var cachedType))
        {
            return cachedType;
        }

        string? inferredType = expression switch
        {
            IecIdentifierExpression identifier => InferIdentifierType(identifier, identifierTypes),
            IecLiteralExpression literal => InferLiteralType(literal.Value),
            IecUnaryExpression unary => InferUnaryType(unary, identifierTypes, cache, expressionTypes, unresolvedExpressions),
            IecBinaryExpression binary => InferBinaryType(binary, identifierTypes, cache, expressionTypes, unresolvedExpressions),
            IecFunctionCallExpression functionCall => InferFunctionCallType(functionCall, identifierTypes, cache, expressionTypes, unresolvedExpressions),
            _ => null,
        };

        cache[expression] = inferredType;
        if (inferredType != null)
        {
            expressionTypes.Add(new KeyValuePair<IecExpression, string>(expression, inferredType));
        }
        else
        {
            unresolvedExpressions.Add(expression);
        }

        return inferredType;
    }

    private static string? InferIdentifierType(IecIdentifierExpression identifier, IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes)
    {
        return identifierTypes.TryGetValue(identifier, out var typeName) ? NormalizeTypeName(typeName) : null;
    }

    private static string? InferLiteralType(object? value)
    {
        return value switch
        {
            bool => "BOOL",
            float or double or decimal => "LREAL",
            short or int or long => "INT",
            string => "STRING",
            _ => null,
        };
    }

    private static string? InferUnaryType(
        IecUnaryExpression unary,
        IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes,
        IDictionary<IecExpression, string?> cache,
        ICollection<KeyValuePair<IecExpression, string>> expressionTypes,
        ICollection<IecExpression> unresolvedExpressions)
    {
        var operandType = InferExpressionType(unary.Operand, identifierTypes, cache, expressionTypes, unresolvedExpressions);
        return unary.OperatorType switch
        {
            IecUnaryOperator.Not => "BOOL",
            IecUnaryOperator.Plus or IecUnaryOperator.Negate => operandType,
            _ => null,
        };
    }

    private static string? InferBinaryType(
        IecBinaryExpression binary,
        IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes,
        IDictionary<IecExpression, string?> cache,
        ICollection<KeyValuePair<IecExpression, string>> expressionTypes,
        ICollection<IecExpression> unresolvedExpressions)
    {
        var leftType = InferExpressionType(binary.Left, identifierTypes, cache, expressionTypes, unresolvedExpressions);
        var rightType = InferExpressionType(binary.Right, identifierTypes, cache, expressionTypes, unresolvedExpressions);

        return binary.OperatorType switch
        {
            IecBinaryOperator.Equal or IecBinaryOperator.NotEqual or IecBinaryOperator.LessThan or IecBinaryOperator.LessThanOrEqual or IecBinaryOperator.GreaterThan or IecBinaryOperator.GreaterThanOrEqual => "BOOL",
            IecBinaryOperator.And or IecBinaryOperator.Or => "BOOL",
            IecBinaryOperator.Add or IecBinaryOperator.Subtract or IecBinaryOperator.Multiply or IecBinaryOperator.Divide => MergeNumericType(leftType, rightType),
            _ => null,
        };
    }

    private static string? InferFunctionCallType(
        IecFunctionCallExpression functionCall,
        IReadOnlyDictionary<IecIdentifierExpression, string?> identifierTypes,
        IDictionary<IecExpression, string?> cache,
        ICollection<KeyValuePair<IecExpression, string>> expressionTypes,
        ICollection<IecExpression> unresolvedExpressions)
    {
        foreach (var argument in functionCall.Arguments)
        {
            _ = InferExpressionType(argument, identifierTypes, cache, expressionTypes, unresolvedExpressions);
        }

        if (_knownFunctionReturnTypes.TryGetValue(functionCall.FunctionName, out var returnType))
        {
            return returnType;
        }

        return null;
    }

    private static string? MergeNumericType(string? leftType, string? rightType)
    {
        var normalizedLeft = NormalizeTypeName(leftType);
        var normalizedRight = NormalizeTypeName(rightType);
        if (normalizedLeft == null)
        {
            return normalizedRight;
        }

        if (normalizedRight == null)
        {
            return normalizedLeft;
        }

        if (string.Equals(normalizedLeft, "LREAL", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalizedRight, "LREAL", StringComparison.OrdinalIgnoreCase))
        {
            return "LREAL";
        }

        return normalizedLeft;
    }

    private static string? NormalizeTypeName(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        return typeName.Trim().ToUpperInvariant();
    }

    private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public static ReferenceEqualityComparer<T> Instance { get; } = new();

        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }
}
