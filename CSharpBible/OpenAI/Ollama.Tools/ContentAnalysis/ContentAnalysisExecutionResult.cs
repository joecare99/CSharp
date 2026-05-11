namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Represents the final analysis result together with the routing decision.
/// </summary>
public sealed class ContentAnalysisExecutionResult
{
    /// <summary>
    /// Gets the routing decision used for the analysis.
    /// </summary>
    public required ContentAnalysisRoutingDecision Decision { get; init; }

    /// <summary>
    /// Gets the structured analysis result.
    /// </summary>
    public required ContentAnalysisResult Result { get; init; }
}
