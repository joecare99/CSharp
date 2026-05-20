using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests.TestDoubles;

internal sealed class TestContentAnalysisTool : IContentAnalysisTool
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public OllamaToolSchema Schema { get; init; } = new()
    {
        Summary = "Accepts a content analysis request as JSON.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "contentKind",
                Description = "The declared content kind.",
                Required = true,
            },
        ],
    };

    public ContentAnalysisRequestValidationResult ValidationResult { get; init; } = new()
    {
        IsValid = true,
    };

    public ContentAnalysisResult Result { get; init; } = new()
    {
        Summary = "ok",
    };

    public ContentAnalysisRequest? LastValidatedRequest { get; private set; }

    public ContentAnalysisRequest? LastAnalyzedRequest { get; private set; }

    public ContentAnalysisRequestValidationResult Validate(ContentAnalysisRequest? request)
    {
        LastValidatedRequest = request;
        return ValidationResult;
    }

    public Task<ContentAnalysisResult> AnalyzeAsync(ContentAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        LastAnalyzedRequest = request;
        return Task.FromResult(Result);
    }
}
