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
    /// <param name="analyzeCSharp">A value indicating whether the content should be analyzed as C# source code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The structured analysis result.</returns>
    Task<ContentAnalysisResult> AnalyzeAsync(string inputText, bool analyzeCSharp, CancellationToken cancellationToken = default);
}
