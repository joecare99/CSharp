using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal sealed class FlowGraphBuilder
{
    private readonly List<FlowNode> _nodes = [];
    private readonly List<StatEqualFinding> _findings = [];
    private readonly Dictionary<string, int> _labels = new(StringComparer.Ordinal);
    private int _exit;
    public FlowGraph Build(StatementSyntax statement)
    {
        ArgumentNullException.ThrowIfNull(statement);
        foreach (var label in statement.DescendantNodesAndSelf().OfType<LabeledStatementSyntax>())
            _labels[label.Identifier.ValueText] = AddNode(label);
        _exit = AddNode(null);
        var entry = BuildStatement(statement, _exit, new FlowContext(null, null));
        return new FlowGraph(_nodes, entry, _exit, _findings);
    }

    private int BuildStatement(StatementSyntax statement, int next, FlowContext context)
    {
        switch (statement)
        {
            case BlockSyntax block:
                return BuildSequence(block.Statements, next, context);
            case IfStatementSyntax conditional:
            {
                var elseEntry = conditional.Else is null
                    ? next
                    : BuildStatement(conditional.Else.Statement, next, context);
                var thenEntry = BuildStatement(conditional.Statement, next, context);
                return ConditionParser.Build(conditional.Condition, thenEntry, elseEntry, AddNode, AddEdge, _findings);
            }
            case WhileStatementSyntax loop:
            {
                var header = AddNode(loop);
                var loopContext = new FlowContext(next, header);
                var body = BuildStatement(loop.Statement, header, loopContext);
                var condition = ConditionParser.Build(loop.Condition, body, next, AddNode, AddEdge, _findings);
                AddEpsilon(header, condition);
                return header;
            }
            case SwitchStatementSyntax selection:
                return BuildSwitch(selection, next, context);
            case LabeledStatementSyntax labeled:
            {
                var labelEntry = _labels[labeled.Identifier.ValueText];
                var body = BuildStatement(labeled.Statement, next, context);
                AddEpsilon(labelEntry, body);
                return labelEntry;
            }
            case GotoStatementSyntax jump when jump.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.GotoStatement:
            {
                var name = jump.Expression is IdentifierNameSyntax identifier ? identifier.Identifier.ValueText : null;
                if (name is not null && _labels.TryGetValue(name, out var destination))
                {
                    var node = AddNode(jump);
                    AddEpsilon(node, destination);
                    return node;
                }
                AddUnmodeled(jump, "goto", "Only goto-label routing is modeled; goto case/default is unsupported.");
                return AddObservable(jump, "unsupported:goto", next);
            }
            case BreakStatementSyntax when context.BreakTarget is int breakTarget:
            {
                var node = AddNode(statement);
                AddEpsilon(node, breakTarget);
                return node;
            }
            case BreakStatementSyntax:
                AddUnmodeled(statement, "break", "A break outside a modeled loop or switch cannot be interpreted safely.");
                return AddObservable(statement, "unsupported:break", next);
            case ContinueStatementSyntax when context.ContinueTarget is int continueTarget:
            {
                var node = AddNode(statement);
                AddEpsilon(node, continueTarget);
                return node;
            }
            case ContinueStatementSyntax:
                AddUnmodeled(statement, "continue", "A continue outside a modeled loop cannot be interpreted safely.");
                return AddObservable(statement, "unsupported:continue", next);
            case ReturnStatementSyntax or ThrowStatementSyntax:
                return AddObservable(statement, "action:" + Normalize(statement), _exit);
            case ExpressionStatementSyntax or LocalDeclarationStatementSyntax:
                return AddObservable(statement, "action:" + Normalize(statement), next);
            case EmptyStatementSyntax:
                return AddNodeWithEpsilon(statement, next);
            default:
                AddUnmodeled(statement, "statement", $"Statement kind '{statement.Kind()}' is outside the modeled subset.");
                return AddObservable(statement, "unsupported:" + Normalize(statement), next);
        }
    }

private int BuildSwitch(SwitchStatementSyntax selection, int next, FlowContext context)
    {
        var selector = (ExpressionSyntax)StripParentheses(selection.Expression);
        if (selector is not IdentifierNameSyntax and not LiteralExpressionSyntax)
        {
            AddUnmodeled(selection.Expression, "switch-selector",
                "Only identifier and literal switch selectors are modeled; complex selectors may have observable evaluation effects.");
            return AddObservable(selection, "unsupported-switch:" + Normalize(selection), next);
        }

        var switchContext = new FlowContext(next, context.ContinueTarget);
        var sectionEntries = new Dictionary<SwitchSectionSyntax, int>();
        foreach (var section in selection.Sections)
            sectionEntries[section] = BuildSequence(section.Statements, next, switchContext);

        var defaultEntry = next;
        foreach (var section in selection.Sections)
        {
            if (section.Labels.Any(label => label is DefaultSwitchLabelSyntax))
                defaultEntry = sectionEntries[section];
        }

        var fallThrough = defaultEntry;
        for (var sectionIndex = selection.Sections.Count - 1; sectionIndex >= 0; sectionIndex--)
        {
            var section = selection.Sections[sectionIndex];
            foreach (var label in section.Labels.Reverse())
            {
                if (label is CaseSwitchLabelSyntax caseLabel)
                {
                    var comparison = Microsoft.CodeAnalysis.CSharp.SyntaxFactory.BinaryExpression(
                        Microsoft.CodeAnalysis.CSharp.SyntaxKind.EqualsExpression,
                        selector,
                        caseLabel.Value);
                    fallThrough = ConditionParser.Build(comparison, sectionEntries[section], fallThrough, AddNode, AddEdge, _findings);
                }
                else if (label is not DefaultSwitchLabelSyntax)
                {
                    AddUnmodeled(label, "switch-label", "Only constant case labels and default are modeled.");
                }
            }
        }

        return fallThrough;
    }

    private int BuildSequence(SyntaxList<StatementSyntax> statements, int next, FlowContext context)
    {
        var current = next;
        for (var index = statements.Count - 1; index >= 0; index--)
            current = BuildStatement(statements[index], current, context);
        return current;
    }

    private int AddObservable(SyntaxNode source, string label, int next)
    {
        var node = AddNode(source);
        _nodes[node].Edges.Add(new FlowEdge(next, label));
        return node;
    }

    private int AddNodeWithEpsilon(SyntaxNode source, int next)
    {
        var node = AddNode(source);
        AddEpsilon(node, next);
        return node;
    }

    private int AddNode(SyntaxNode? source)
    {
        StatEqualSourceSpan? span = source is null ? null : ToSpan(source);
        var node = new FlowNode(_nodes.Count, span);
        _nodes.Add(node);
        return node.Id;
    }

    private void AddEpsilon(int source, int target) => _nodes[source].Edges.Add(new FlowEdge(target, null));

    private void AddEdge(int source, FlowEdge edge) => _nodes[source].Edges.Add(edge);

    private void AddUnmodeled(SyntaxNode source, string code, string message) =>
        _findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Warning, "UNMODELED_" + code.ToUpperInvariant(),
            message, ToSpan(source)));

    internal static string Normalize(SyntaxNode node) => node.WithoutTrivia().NormalizeWhitespace().ToFullString();

    internal static SyntaxNode StripParentheses(SyntaxNode node)
    {
        while (node is ParenthesizedExpressionSyntax parenthesized)
            node = parenthesized.Expression;
        return node;
    }

    internal static StatEqualSourceSpan ToSpan(SyntaxNode node)
    {
        var line = node.GetLocation().GetLineSpan().StartLinePosition;
        return new StatEqualSourceSpan(node.SpanStart, node.Span.Length, line.Line + 1, line.Character + 1);
    }

    private sealed record FlowContext(int? BreakTarget, int? ContinueTarget);
}

















