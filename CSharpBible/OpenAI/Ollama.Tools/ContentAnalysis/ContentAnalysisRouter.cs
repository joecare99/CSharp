using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Detects suitable content analysis modes and routes requests to the matching analysis tool.
/// </summary>
public sealed class ContentAnalysisRouter
{
    private readonly TextAnalysisTool _textAnalysisTool;
    private readonly CSharpCodeAnalysisTool _cSharpCodeAnalysisTool;
    private readonly IPdfTextExtractor _pdfTextExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentAnalysisRouter"/> class.
    /// </summary>
    /// <param name="textAnalysisTool">The plain text analysis tool.</param>
    /// <param name="cSharpCodeAnalysisTool">The C# source analysis tool.</param>
    public ContentAnalysisRouter(TextAnalysisTool textAnalysisTool, CSharpCodeAnalysisTool cSharpCodeAnalysisTool, IPdfTextExtractor? pdfTextExtractor = null)
    {
        _textAnalysisTool = textAnalysisTool ?? throw new ArgumentNullException(nameof(textAnalysisTool));
        _cSharpCodeAnalysisTool = cSharpCodeAnalysisTool ?? throw new ArgumentNullException(nameof(cSharpCodeAnalysisTool));
        _pdfTextExtractor = pdfTextExtractor ?? new PdfPigTextExtractor();
    }

    /// <summary>
    /// Routes the specified content to the most suitable analysis tool.
    /// </summary>
    /// <param name="inputText">The input content.</param>
    /// <param name="displayName">The display name or file name of the content.</param>
    /// <param name="mode">The requested routing mode.</param>
    /// <returns>The routed analysis context.</returns>
    public ContentAnalysisRoutingContext Route(string inputText, string? displayName, ContentAnalysisMode mode = ContentAnalysisMode.Auto)
    {
        string normalizedInputText = inputText ?? string.Empty;
        string normalizedDisplayName = string.IsNullOrWhiteSpace(displayName) ? "inline input" : displayName;
        ContentAnalysisRoutingDecision decision = Detect(normalizedInputText, normalizedDisplayName, mode);
        IContentAnalysisTool tool = decision.ContentKind == OllamaContentKind.SourceCode
            ? _cSharpCodeAnalysisTool
            : _textAnalysisTool;

        return new ContentAnalysisRoutingContext
        {
            Decision = decision,
            Tool = tool,
            Request = new ContentAnalysisRequest
            {
                ContentKind = decision.ContentKind,
                SourceKind = OllamaContentSourceKind.Inline,
                DisplayName = normalizedDisplayName,
                MediaType = decision.MediaType,
                Language = decision.Language,
                Content = normalizedInputText,
            },
        };
    }

    /// <summary>
    /// Routes and analyzes the specified content.
    /// </summary>
    /// <param name="inputText">The input content.</param>
    /// <param name="displayName">The display name or file name of the content.</param>
    /// <param name="mode">The requested routing mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The routed analysis result.</returns>
    public async Task<ContentAnalysisExecutionResult> AnalyzeAsync(string inputText, string? displayName, ContentAnalysisMode mode = ContentAnalysisMode.Auto, CancellationToken cancellationToken = default)
    {
        ContentAnalysisRoutingContext context = Route(inputText, displayName, mode);
        ContentAnalysisRequestValidationResult validationResult = context.Tool.Validate(context.Request);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(string.Join(" ", validationResult.Issues.Select(static issue => issue.Message)));
        }

        ContentAnalysisResult result = await context.Tool.AnalyzeAsync(context.Request, cancellationToken);
        return new ContentAnalysisExecutionResult
        {
            Decision = context.Decision,
            Result = result,
        };
    }

    /// <summary>
    /// Extracts text from a PDF file and analyzes the extracted text using the normal text analysis flow.
    /// </summary>
    /// <param name="filePath">The PDF file path.</param>
    /// <param name="mode">The requested routing mode for the extracted text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The routed analysis result.</returns>
    public async Task<ContentAnalysisExecutionResult> AnalyzePdfAsync(string filePath, ContentAnalysisMode mode = ContentAnalysisMode.Auto, CancellationToken cancellationToken = default)
    {
        PdfExtractionResult pdfResult = await _pdfTextExtractor.ExtractAsync(new PdfExtractionRequest
        {
            FilePath = filePath,
        }, cancellationToken);

        if (!pdfResult.IsSuccessful)
        {
            throw new InvalidOperationException(pdfResult.ErrorMessage ?? "PDF extraction failed.");
        }

        string displayName = pdfResult.FileMetadata?.FileName ?? filePath;
        ContentAnalysisExecutionResult analysisResult = await AnalyzeAsync(pdfResult.ExtractedText, displayName, mode, cancellationToken);

        return new ContentAnalysisExecutionResult
        {
            Decision = analysisResult.Decision,
            Result = analysisResult.Result,
        };
    }

    private static ContentAnalysisRoutingDecision Detect(string inputText, string displayName, ContentAnalysisMode mode)
    {
        return mode switch
        {
            ContentAnalysisMode.Text => CreateTextDecision("Text analysis was selected explicitly."),
            ContentAnalysisMode.CSharp => CreateCSharpDecision("C# source analysis was selected explicitly."),
            _ => DetectAutomatically(inputText, displayName),
        };
    }

    private static ContentAnalysisRoutingDecision DetectAutomatically(string inputText, string displayName)
    {
        string extension = Path.GetExtension(displayName);
        if (string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCSharpDecision("The .cs file extension indicates C# source code.");
        }

        if (LooksLikeCSharp(inputText))
        {
            return CreateCSharpDecision("C# language patterns were detected in the input content.");
        }

        return CreateTextDecision("No strong C# indicators were found, so the input was treated as plain text.");
    }

    private static bool LooksLikeCSharp(string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return false;
        }

        bool hasUsingDirective = inputText.Contains("using ", StringComparison.Ordinal);
        bool hasNamespace = inputText.Contains("namespace ", StringComparison.Ordinal);
        bool hasTypeDeclaration = inputText.Contains(" class ", StringComparison.Ordinal)
            || inputText.Contains(" record ", StringComparison.Ordinal)
            || inputText.Contains(" interface ", StringComparison.Ordinal)
            || inputText.Contains(" struct ", StringComparison.Ordinal);
        bool hasSemicolons = inputText.Count(static character => character == ';') >= 2;
        bool hasAccessModifier = inputText.Contains("public ", StringComparison.Ordinal)
            || inputText.Contains("private ", StringComparison.Ordinal)
            || inputText.Contains("internal ", StringComparison.Ordinal)
            || inputText.Contains("protected ", StringComparison.Ordinal);

        return (hasUsingDirective && hasSemicolons)
            || (hasNamespace && hasTypeDeclaration)
            || (hasTypeDeclaration && hasAccessModifier);
    }

    private static ContentAnalysisRoutingDecision CreateTextDecision(string reason)
    {
        return new ContentAnalysisRoutingDecision
        {
            AnalysisLabel = "Text analysis",
            ContentKind = OllamaContentKind.Text,
            MediaType = "text/plain",
            Language = string.Empty,
            Reason = reason,
        };
    }

    private static ContentAnalysisRoutingDecision CreateCSharpDecision(string reason)
    {
        return new ContentAnalysisRoutingDecision
        {
            AnalysisLabel = "C# source analysis",
            ContentKind = OllamaContentKind.SourceCode,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Reason = reason,
        };
    }
}
