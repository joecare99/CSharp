using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal static class SourceFlowParser
{
    public static FlowGraph Parse(string source)
    {
        var findings = new List<StatEqualFinding>();
        var root = CSharpSyntaxTree.ParseText(source).GetCompilationUnitRoot();
        var statements = root.Members.OfType<GlobalStatementSyntax>().Select(item => item.Statement).ToArray();
        if (statements.Length == 1 && statements[0] is LocalFunctionStatementSyntax localFunction && localFunction.Body is not null)
        {
            statements = [localFunction.Body];
            AddParseDiagnostics(localFunction, findings);
        }
        else if (statements.Length > 0)
        {
            AddParseDiagnostics(root, findings);
        }
        else
        {
            var bodies = root.DescendantNodes().OfType<BaseMethodDeclarationSyntax>()
                .Select(method => method.Body)
                .Where(body => body is not null)
                .Cast<BlockSyntax>()
                .ToArray();
            if (bodies.Length == 0 && SyntaxFactory.ParseMemberDeclaration(source) is BaseMethodDeclarationSyntax method && method.Body is not null)
            {
                bodies = [method.Body];
                AddParseDiagnostics(method, findings);
            }
            else if (bodies.Length > 0)
            {
                AddParseDiagnostics(root, findings);
            }

            if (bodies.Length > 0)
            {
                statements = [bodies[0]];
                if (bodies.Length > 1)
                    findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Warning, "MULTIPLE_BODIES",
                        $"The source contains {bodies.Length} executable method bodies; only the first source-order body is compared."));
            }
            else
            {
                var statement = SyntaxFactory.ParseStatement(source);
                if (!statement.ContainsDiagnostics && statement.ToFullString().Trim() == source.Trim())
                {
                    statements = [statement];
                }
                else
                {
                    AddParseDiagnostics(root, findings);
                    AddParseDiagnostics(statement, findings);
                    findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Warning, "NO_EXECUTABLE_BODY",
                        "No supported executable statement or method body could be identified."));
                    statements = [SyntaxFactory.EmptyStatement()];
                }
            }
        }

        var combined = SyntaxFactory.Block(statements);
        var graph = new FlowGraphBuilder().Build(combined);
        return new FlowGraph(graph.Nodes, graph.Entry, graph.Exit, findings.Concat(graph.Findings).ToArray());
    }

    private static void AddParseDiagnostics(SyntaxNode node, List<StatEqualFinding> findings)
    {
        foreach (var diagnostic in node.GetDiagnostics().Where(item => item.Severity == DiagnosticSeverity.Error))
        {
            var span = diagnostic.Location.IsInSource ? ToSpan(diagnostic.Location) : (StatEqualSourceSpan?)null;
            findings.Add(new StatEqualFinding(StatEqualFindingSeverity.Warning, "CSHARP_PARSE_ERROR",
                diagnostic.GetMessage(), span));
        }
    }

    private static StatEqualSourceSpan ToSpan(Location location)
    {
        var line = location.GetLineSpan().StartLinePosition;
        return new StatEqualSourceSpan(location.SourceSpan.Start, location.SourceSpan.Length, line.Line + 1, line.Character + 1);
    }
}









