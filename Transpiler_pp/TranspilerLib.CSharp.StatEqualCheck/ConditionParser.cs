using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal static class ConditionParser
{
    public static int Build(ExpressionSyntax expression, int whenTrue, int whenFalse,
        Func<SyntaxNode?, int> addNode, Action<int, FlowEdge> addEdge, List<StatEqualFinding> findings)
    {
        expression = (ExpressionSyntax)FlowGraphBuilder.StripParentheses(expression);
        if (expression is PrefixUnaryExpressionSyntax unary && unary.IsKind(SyntaxKind.LogicalNotExpression))
            return Build(unary.Operand, whenFalse, whenTrue, addNode, addEdge, findings);

        if (expression is BinaryExpressionSyntax binary)
        {
            if (binary.IsKind(SyntaxKind.LogicalAndExpression))
            {
                var right = Build(binary.Right, whenTrue, whenFalse, addNode, addEdge, findings);
                return Build(binary.Left, right, whenFalse, addNode, addEdge, findings);
            }
            if (binary.IsKind(SyntaxKind.LogicalOrExpression))
            {
                var right = Build(binary.Right, whenTrue, whenFalse, addNode, addEdge, findings);
                return Build(binary.Left, whenTrue, right, addNode, addEdge, findings);
            }
            if (binary.IsKind(SyntaxKind.BitwiseAndExpression))
            {
                var onLeftTrue = Build(binary.Right, whenTrue, whenFalse, addNode, addEdge, findings);
                var onLeftFalse = Build(binary.Right, whenFalse, whenFalse, addNode, addEdge, findings);
                return Build(binary.Left, onLeftTrue, onLeftFalse, addNode, addEdge, findings);
            }
            if (binary.IsKind(SyntaxKind.BitwiseOrExpression))
            {
                var onLeftTrue = Build(binary.Right, whenTrue, whenTrue, addNode, addEdge, findings);
                var onLeftFalse = Build(binary.Right, whenTrue, whenFalse, addNode, addEdge, findings);
                return Build(binary.Left, onLeftTrue, onLeftFalse, addNode, addEdge, findings);
            }
        }

        if (!IsModeledAtom(expression))
        {
            var line = expression.GetLocation().GetLineSpan().StartLinePosition;
            var span = new StatEqualSourceSpan(expression.SpanStart, expression.Span.Length, line.Line + 1, line.Character + 1);
            findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Warning, "UNMODELED_CONDITION",
                $"Condition expression kind '{expression.Kind()}' is treated as an opaque atom and cannot support a proof.", span));
        }

        var decision = addNode(expression);
        var atom = addNode(expression);
        var text = FlowGraphBuilder.Normalize(expression);
        addEdge(decision, new FlowEdge(whenTrue, "true"));
        addEdge(decision, new FlowEdge(whenFalse, "false"));
        addEdge(atom, new FlowEdge(decision, "atom:" + text));
        return atom;
    }

    private static bool IsModeledAtom(ExpressionSyntax expression)
    {
        if (expression is IdentifierNameSyntax or GenericNameSyntax or LiteralExpressionSyntax or
            MemberAccessExpressionSyntax or ElementAccessExpressionSyntax or InvocationExpressionSyntax)
            return true;
        return expression is BinaryExpressionSyntax binary && (binary.IsKind(SyntaxKind.EqualsExpression) ||
            binary.IsKind(SyntaxKind.NotEqualsExpression) || binary.IsKind(SyntaxKind.LessThanExpression) ||
            binary.IsKind(SyntaxKind.LessThanOrEqualExpression) || binary.IsKind(SyntaxKind.GreaterThanExpression) ||
            binary.IsKind(SyntaxKind.GreaterThanOrEqualExpression));
    }
}
