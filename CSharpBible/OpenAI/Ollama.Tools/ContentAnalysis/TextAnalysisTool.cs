using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Provides a first local text analysis tool based on simple heuristics.
/// </summary>
public sealed class TextAnalysisTool : IContentAnalysisTool
{
    private static readonly OllamaToolSchema ToolSchema = new()
    {
        Summary = "Accepts a content analysis request for plain text and returns a structured local evaluation.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "contentKind",
                Type = "number",
                Description = "Must be Text for this analysis tool.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "sourceKind",
                Type = "number",
                Description = "Specifies whether content is inline or file-based.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "mediaType",
                Type = "string",
                Description = "Should describe a text media type such as text/plain.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "content",
                Type = "string",
                Description = "Inline text to analyze when sourceKind is Inline.",
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
    public string Name => "analyze_text";

    /// <inheritdoc/>
    public string Description => "Evaluates plain text for clarity, structure, and basic readability.";

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
        if (request!.ContentKind != OllamaContentKind.Text)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.ContentKind),
                Code = "contentKind.text.required",
                Message = "The text analysis tool only supports text requests.",
            });
        }

        if (!request.MediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.MediaType),
                Code = "mediaType.text.invalid",
                Message = "The text analysis tool requires a text media type.",
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
        int characterCount = content.Length;
        int wordCount = CountWords(content);
        int sentenceCount = CountSentences(content);
        bool hasParagraphs = content.Contains(Environment.NewLine + Environment.NewLine, StringComparison.Ordinal) || content.Contains("\n\n", StringComparison.Ordinal);
        bool hasListMarkers = content.Contains("- ", StringComparison.Ordinal) || content.Contains("* ", StringComparison.Ordinal);

        List<ContentAnalysisFinding> findings = [];
        List<ContentAnalysisSuggestion> suggestions = [];
        double score = 1.0;

        if (wordCount < 5)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Very short text",
                Message = "The text is very short, so the evaluation confidence is limited.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Words: {wordCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Add more context",
                Description = "Provide a longer text sample so the analysis can assess structure and clarity more reliably.",
                Priority = "medium",
            });
            score -= 0.25;
        }

        if (sentenceCount <= 1 && wordCount > 20)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Long unbroken sentence",
                Message = "The text contains many words but very few sentence breaks.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Sentences: {sentenceCount}, Words: {wordCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Split long passages",
                Description = "Break long passages into multiple sentences to improve readability.",
                Priority = "high",
            });
            score -= 0.2;
        }

        if (!hasParagraphs && wordCount > 80)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Missing paragraph structure",
                Message = "The text is relatively long but does not appear to use paragraph separation.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Words: {wordCount}",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Add paragraphs",
                Description = "Separate major ideas into paragraphs to improve scanning and comprehension.",
                Priority = "medium",
            });
            score -= 0.15;
        }

        if (hasListMarkers)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Structured bullet points detected",
                Message = "The text already includes list markers, which helps structure and scannability.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = "List markers detected",
            });
        }

        if (findings.Count == 0)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Balanced text structure",
                Message = "The text shows a reasonable balance of length and sentence structure for a first-pass evaluation.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Words: {wordCount}, Sentences: {sentenceCount}",
            });
        }

        ContentAnalysisResult result = new()
        {
            Summary = BuildSummary(score, wordCount, sentenceCount),
            Score = Math.Clamp(score, 0.0, 1.0),
            Confidence = CalculateConfidence(wordCount),
            Rationale = $"The local heuristic analysis evaluated text/plain content with {characterCount} characters, {wordCount} words, and {sentenceCount} sentence boundaries.",
            Findings = findings,
            Suggestions = suggestions,
        };

        return Task.FromResult(result);
    }

    private static string BuildSummary(double score, int wordCount, int sentenceCount)
    {
        if (score >= 0.85)
        {
            return $"The text looks structurally healthy with {wordCount} words across {sentenceCount} sentence units.";
        }

        if (score >= 0.6)
        {
            return $"The text is usable but shows some readability weaknesses across {wordCount} words.";
        }

        return $"The text would benefit from revisions to improve clarity and structure.";
    }

    private static double CalculateConfidence(int wordCount)
    {
        if (wordCount >= 120)
        {
            return 0.9;
        }

        if (wordCount >= 40)
        {
            return 0.75;
        }

        if (wordCount >= 10)
        {
            return 0.6;
        }

        return 0.4;
    }

    private static int CountWords(string content)
    {
        return content.Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static int CountSentences(string content)
    {
        int count = content.Count(static character => character is '.' or '!' or '?');
        return Math.Max(count, 1);
    }
}
