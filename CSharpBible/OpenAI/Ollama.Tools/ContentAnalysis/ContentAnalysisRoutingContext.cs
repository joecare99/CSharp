using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the routed analysis request together with the selected tool.
/// </summary>
public sealed class ContentAnalysisRoutingContext
{
    /// <summary>
    /// Gets the routing decision.
    /// </summary>
    public required ContentAnalysisRoutingDecision Decision { get; init; }

    /// <summary>
    /// Gets the generated analysis request.
    /// </summary>
    public required ContentAnalysisRequest Request { get; init; }

    /// <summary>
    /// Gets the selected analysis tool.
    /// </summary>
    public required IContentAnalysisTool Tool { get; init; }
}
