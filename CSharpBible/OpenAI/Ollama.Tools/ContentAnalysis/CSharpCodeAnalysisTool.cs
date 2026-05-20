using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Provides a first local C# source code analysis tool based on simple heuristics.
/// </summary>
public sealed class CSharpCodeAnalysisTool : IContentAnalysisTool
{
    private static readonly Regex TypeDeclarationRegex = new(@"\b(class|record|struct|interface)\s+[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);
    private static readonly Regex MethodDeclarationRegex = new(@"\b(public|private|internal|protected)\s+(static\s+)?[A-Za-z_][A-Za-z0-9_<>\[\],?]*\s+[A-Za-z_][A-Za-z0-9_]*\s*\(", RegexOptions.Compiled);

    private static readonly OllamaToolSchema ToolSchema = new()
    {
        Summary = "Accepts a content analysis request for C# source code and returns a structured local evaluation.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "contentKind",
                Type = "number",
                Description = "Must be SourceCode for this analysis tool.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "language",
                Type = "string",
                Description = "Must identify C# source code, for example csharp or c#.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "mediaType",
                Type = "string",
                Description = "Should describe a text-based C# media type such as text/x-csharp.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "content",
                Type = "string",
                Description = "Inline C# source to analyze when sourceKind is Inline.",
                Required = false,
            },
            new OllamaToolParameter
            {
                Name = "filePath",
                Type = "string",
                Description = "File path to analyze when sourceKind is FilePath.",
                Required = false,
            },
        ],
    };

    /// <inheritdoc/>
    public string Name => "analyze_csharp";

    /// <inheritdoc/>
    public string Description => "Evaluates C# source code for structure, maintainability hints, and simple code smells.";

    /// <inheritdoc/>
    public OllamaToolSchema Schema => ToolSchema;

    /// <inheritdoc/>
    public ContentAnalysisRequestValidationResult Validate(ContentAnalysisRequest? request)
    {
        ContentAnalysisRequestValidationResult baseResult = ContentAnalysisRequestValidator.Validate(request);
        if (!baseResult.IsValid)
        {
            return baseResult;
        }

        List<ContentAnalysisValidationIssue> issues = [.. baseResult.Issues];
        if (request!.ContentKind != OllamaContentKind.SourceCode)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.ContentKind),
                Code = "contentKind.sourceCode.required",
                Message = "The C# analysis tool only supports source code requests.",
            });
        }

        if (!IsCSharpLanguage(request.Language))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.Language),
                Code = "language.csharp.required",
                Message = "The C# analysis tool requires the language to be declared as C#.",
            });
        }

        if (!request.MediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.MediaType),
                Code = "mediaType.csharp.invalid",
                Message = "The C# analysis tool requires a text-based media type.",
            });
        }

        return new ContentAnalysisRequestValidationResult
        {
            IsValid = issues.Count == 0,
            Issues = issues,
        };
    }

    /// <inheritdoc/>
    public Task<ContentAnalysisResult> AnalyzeAsync(ContentAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        string content = request.Content;
        string normalizedContent = content.Replace("\r\n", "\n", StringComparison.Ordinal);
        string[] lines = normalizedContent.Split('\n');
        int nonEmptyLineCount = lines.Count(static line => !string.IsNullOrWhiteSpace(line));
        int usingDirectiveCount = lines.Count(static line => line.TrimStart().StartsWith("using ", StringComparison.Ordinal));
        int longLineCount = lines.Count(static line => line.Length > 120);
        int todoCount = lines.Count(static line => line.Contains("TODO", StringComparison.OrdinalIgnoreCase));
        int consoleWriteLineCount = lines.Count(static line => line.Contains("Console.WriteLine(", StringComparison.Ordinal));
        int typeDeclarationCount = TypeDeclarationRegex.Matches(content).Count;
        int methodDeclarationCount = MethodDeclarationRegex.Matches(content).Count;
        bool hasNamespace = lines.Any(static line => line.TrimStart().StartsWith("namespace ", StringComparison.Ordinal));

        List<ContentAnalysisFinding> findings = [];
        List<ContentAnalysisSuggestion> suggestions = [];
        double score = 1.0;

        if (typeDeclarationCount > 0 && methodDeclarationCount > 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Structured C# source detected",
                Message = "The source contains both type and method declarations, which indicates a recognizable code structure.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Types: {typeDeclarationCount}, Methods: {methodDeclarationCount}",
            });
        }

        if (!hasNamespace)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Missing namespace declaration",
                Message = "The file does not appear to declare a namespace.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = "No namespace declaration detected",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Declare a namespace",
                Description = "Add an explicit namespace to keep the source file aligned with the surrounding code organization.",
                Priority = "medium",
            });
            score -= 0.1;
        }

        if (consoleWriteLineCount > 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Direct console output detected",
                Message = "The code writes directly to the console, which may complicate testing or reuse outside console applications.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Console.WriteLine calls: {consoleWriteLineCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Abstract console output",
                Description = "Consider isolating direct console output behind a service or adapter when the logic needs to be reused or tested.",
                Priority = "medium",
            });
            score -= 0.15;
        }

        if (todoCount > 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "TODO markers detected",
                Message = "The source still contains TODO markers that may indicate unfinished work.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"TODO markers: {todoCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Review pending TODO items",
                Description = "Confirm whether each TODO is still relevant and convert important ones into tracked backlog work.",
                Priority = "low",
            });
            score -= 0.1;
        }

        if (longLineCount > 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Long lines detected",
                Message = "Some lines exceed 120 characters, which can reduce readability in reviews and editors.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Long lines: {longLineCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Wrap long lines",
                Description = "Split or reformat long statements to improve readability and reduce horizontal scrolling.",
                Priority = "medium",
            });
            score -= 0.1;
        }

        if (methodDeclarationCount == 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "No methods detected",
                Message = "The source does not appear to contain method declarations, so the structural evaluation is limited.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = "No method signatures detected",
            });
            score -= 0.15;
        }

        if (usingDirectiveCount == 0 && nonEmptyLineCount > 10)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "No using directives detected",
                Message = "The file does not contain using directives, which may be valid but can also indicate a truncated sample.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Non-empty lines: {nonEmptyLineCount}",
            });
        }

        if (findings.Count == 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Balanced C# source structure",
                Message = "The C# source looks structurally consistent for a first-pass heuristic evaluation.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Types: {typeDeclarationCount}, Methods: {methodDeclarationCount}, Using directives: {usingDirectiveCount}",
            });
        }

        ContentAnalysisResult result = new()
        {
            Summary = BuildSummary(score, typeDeclarationCount, methodDeclarationCount),
            Score = Math.Clamp(score, 0.0, 1.0),
            Confidence = CalculateConfidence(nonEmptyLineCount),
            Rationale = $"The local heuristic analysis evaluated {nonEmptyLineCount} non-empty lines, {typeDeclarationCount} type declarations, {methodDeclarationCount} method signatures, {todoCount} TODO markers, and {consoleWriteLineCount} direct console output calls.",
            Findings = findings,
            Suggestions = suggestions,
        };

        return Task.FromResult(result);
    }

    private static bool IsCSharpLanguage(string language)
    {
        return string.Equals(language, "c#", StringComparison.OrdinalIgnoreCase)
            || string.Equals(language, "csharp", StringComparison.OrdinalIgnoreCase)
            || string.Equals(language, "cs", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildSummary(double score, int typeDeclarationCount, int methodDeclarationCount)
    {
        if (score >= 0.85)
        {
            return $"The C# source looks structurally healthy with {typeDeclarationCount} type declarations and {methodDeclarationCount} detected methods.";
        }

        if (score >= 0.6)
        {
            return $"The C# source is usable but shows several maintainability signals that deserve attention.";
        }

        return "The C# source would benefit from refactoring to improve structure and maintainability.";
    }

    private static double CalculateConfidence(int nonEmptyLineCount)
    {
        if (nonEmptyLineCount >= 80)
        {
            return 0.9;
        }

        if (nonEmptyLineCount >= 30)
        {
            return 0.75;
        }

        if (nonEmptyLineCount >= 10)
        {
            return 0.6;
        }

        return 0.45;
    }
}
