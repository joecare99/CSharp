using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Provides content analysis operations for the WPF sample.
/// </summary>
public interface IContentAnalysisService
{
    /// <summary>
    /// Analyzes the specified input using the selected mode.
    /// </summary>
    /// <param name="inputText">The input text or source code.</param>
    /// <param name="displayName">The display name or file name associated with the input.</param>
    /// <param name="mode">The selected or requested analysis mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The routed structured analysis result.</returns>
    Task<ContentAnalysisExecutionResult> AnalyzeAsync(string inputText, string? displayName, ContentAnalysisMode mode, CancellationToken cancellationToken = default);
}
