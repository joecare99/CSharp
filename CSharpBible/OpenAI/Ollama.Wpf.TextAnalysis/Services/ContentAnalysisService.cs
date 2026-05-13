using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Adapts reusable analysis tools for the WPF sample.
/// </summary>
public sealed class ContentAnalysisService : IContentAnalysisService
{
    private readonly ContentAnalysisRouter _contentAnalysisRouter;

    public ContentAnalysisService(ContentAnalysisRouter contentAnalysisRouter)
    {
        _contentAnalysisRouter = contentAnalysisRouter ?? throw new ArgumentNullException(nameof(contentAnalysisRouter));
    }

    public Task<ContentAnalysisExecutionResult> AnalyzeAsync(string inputText, string? displayName, ContentAnalysisMode mode, CancellationToken cancellationToken = default)
    {
        return _contentAnalysisRouter.AnalyzeAsync(inputText, displayName, mode, cancellationToken);
    }
}
