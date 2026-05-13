using System;
using System.IO;
using System.Threading.Tasks;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Samples.TextAnalysis;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        ContentAnalysisMode mode = ResolveMode(args);
        string[] inputArguments = FilterInputArguments(args);
        string displayName = inputArguments.Length > 0
            ? inputArguments[0]
            : mode == ContentAnalysisMode.CSharp
                ? "inline csharp input"
                : "inline input";
        string inputText = inputArguments.Length > 0 && File.Exists(inputArguments[0])
            ? await File.ReadAllTextAsync(inputArguments[0])
            : inputArguments.Length > 0
                ? string.Join(" ", inputArguments)
                : mode == ContentAnalysisMode.CSharp
                    ? "using System;\nnamespace Demo;\n\npublic sealed class Sample\n{\n    public void Run()\n    {\n        Console.WriteLine(\"hello\");\n    }\n}"
                    : "This is a first local text analysis sample. It contains multiple sentences so the heuristic tool can evaluate structure and clarity.";

        ContentAnalysisRouter router = new(new TextAnalysisTool(), new CSharpCodeAnalysisTool());
        ContentAnalysisExecutionResult executionResult = await router.AnalyzeAsync(inputText, displayName, mode);

        Console.WriteLine($"Mode: {executionResult.Decision.AnalysisLabel}");
        Console.WriteLine($"Reason: {executionResult.Decision.Reason}");
        Console.WriteLine($"Input: {displayName}");
        ContentAnalysisResult result = executionResult.Result;
        Console.WriteLine($"Summary: {result.Summary}");
        Console.WriteLine($"Score: {result.Score:0.00}");
        Console.WriteLine($"Confidence: {result.Confidence:0.00}");
        Console.WriteLine($"Rationale: {result.Rationale}");
        Console.WriteLine();
        Console.WriteLine("Findings:");
        foreach (ContentAnalysisFinding finding in result.Findings)
        {
            Console.WriteLine($"- [{finding.Severity}] {finding.Title}: {finding.Message}");
        }

        if (result.Suggestions.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Suggestions:");
            foreach (ContentAnalysisSuggestion suggestion in result.Suggestions)
            {
                Console.WriteLine($"- ({suggestion.Priority}) {suggestion.Title}: {suggestion.Description}");
            }
        }

        return 0;
    }

    private static ContentAnalysisMode ResolveMode(string[] args)
    {
        if (Array.Exists(args, static argument => string.Equals(argument, "--text", StringComparison.OrdinalIgnoreCase)))
        {
            return ContentAnalysisMode.Text;
        }

        if (Array.Exists(args, static argument => string.Equals(argument, "--csharp", StringComparison.OrdinalIgnoreCase)))
        {
            return ContentAnalysisMode.CSharp;
        }

        return ContentAnalysisMode.Auto;
    }

    private static string[] FilterInputArguments(string[] args)
    {
        return Array.FindAll(args, static argument => !string.Equals(argument, "--text", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(argument, "--csharp", StringComparison.OrdinalIgnoreCase));
    }
}
