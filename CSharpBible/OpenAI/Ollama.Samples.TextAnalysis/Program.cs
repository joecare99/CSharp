using System;
using System.IO;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Samples.TextAnalysis;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        bool analyzeCSharp = args.Length > 0 && string.Equals(args[0], "--csharp", StringComparison.OrdinalIgnoreCase);
        int inputArgumentStartIndex = analyzeCSharp ? 1 : 0;
        string[] inputArguments = args.Length > inputArgumentStartIndex
            ? args[inputArgumentStartIndex..]
            : [];
        string inputText = args.Length > 0 && File.Exists(args[0])
            ? await File.ReadAllTextAsync(args[0])
            : inputArguments.Length > 0 && File.Exists(inputArguments[0])
                ? await File.ReadAllTextAsync(inputArguments[0])
                : inputArguments.Length > 0
                    ? string.Join(" ", inputArguments)
                    : analyzeCSharp
                        ? "using System;\nnamespace Demo;\n\npublic sealed class Sample\n{\n    public void Run()\n    {\n        Console.WriteLine(\"hello\");\n    }\n}"
                        : "This is a first local text analysis sample. It contains multiple sentences so the heuristic tool can evaluate structure and clarity.";

        IContentAnalysisTool tool = analyzeCSharp ? new CSharpCodeAnalysisTool() : new TextAnalysisTool();
        ContentAnalysisRequest request = new()
        {
            ContentKind = analyzeCSharp ? OllamaContentKind.SourceCode : OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            DisplayName = inputArguments.Length > 0 ? inputArguments[0] : analyzeCSharp ? "inline csharp input" : "inline text input",
            MediaType = analyzeCSharp ? "text/x-csharp" : "text/plain",
            Language = analyzeCSharp ? "csharp" : string.Empty,
            Content = inputText,
        };

        ContentAnalysisRequestValidationResult validationResult = tool.Validate(request);
        if (!validationResult.IsValid)
        {
            Console.WriteLine("Validation failed:");
            foreach (ContentAnalysisValidationIssue issue in validationResult.Issues)
            {
                Console.WriteLine($"- {issue.Field}: {issue.Message}");
            }

            return 1;
        }

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);
        Console.WriteLine($"Mode: {(analyzeCSharp ? "C# source analysis" : "Text analysis")}");
        Console.WriteLine($"Input: {request.DisplayName}");
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
}
