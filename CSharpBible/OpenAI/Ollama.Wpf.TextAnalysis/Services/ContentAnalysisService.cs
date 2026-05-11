using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Adapts reusable analysis tools for the WPF sample.
/// </summary>
public sealed class ContentAnalysisService : IContentAnalysisService
{
    private readonly TextAnalysisTool _textAnalysisTool;
    private readonly CSharpCodeAnalysisTool _cSharpCodeAnalysisTool;

    public ContentAnalysisService(TextAnalysisTool textAnalysisTool, CSharpCodeAnalysisTool cSharpCodeAnalysisTool)
    {
        _textAnalysisTool = textAnalysisTool ?? throw new ArgumentNullException(nameof(textAnalysisTool));
        _cSharpCodeAnalysisTool = cSharpCodeAnalysisTool ?? throw new ArgumentNullException(nameof(cSharpCodeAnalysisTool));
    }

    public async Task<ContentAnalysisResult> AnalyzeAsync(string inputText, bool analyzeCSharp, CancellationToken cancellationToken = default)
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = analyzeCSharp ? OllamaContentKind.SourceCode : OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            DisplayName = analyzeCSharp ? "WPF C# input" : "WPF text input",
            MediaType = analyzeCSharp ? "text/x-csharp" : "text/plain",
            Language = analyzeCSharp ? "csharp" : string.Empty,
            Content = inputText ?? string.Empty,
        };

        IContentAnalysisTool tool = analyzeCSharp ? _cSharpCodeAnalysisTool : _textAnalysisTool;
        ContentAnalysisRequestValidationResult validationResult = tool.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(string.Join(" ", validationResult.Issues.Select(static issue => issue.Message)));
        }

        return await tool.AnalyzeAsync(request, cancellationToken);
    }
}
