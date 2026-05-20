using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Abstractions;

/// <summary>
/// Defines a reusable content analysis tool.
/// </summary>
public interface IContentAnalysisTool
{
    /// <summary>
    /// Gets the unique tool name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the human-readable tool description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the declared input schema for the tool.
    /// </summary>
    OllamaToolSchema Schema { get; }

    /// <summary>
    /// Validates the specified content analysis request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>The structured validation result.</returns>
    ContentAnalysisRequestValidationResult Validate(ContentAnalysisRequest? request);

    /// <summary>
    /// Analyzes the specified content.
    /// </summary>
    /// <param name="request">The request to analyze.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The structured analysis result.</returns>
    Task<ContentAnalysisResult> AnalyzeAsync(ContentAnalysisRequest request, CancellationToken cancellationToken = default);
}
