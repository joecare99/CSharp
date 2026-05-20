using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Adapts an <see cref="IContentAnalysisTool"/> to the string-based <see cref="IOllamaTool"/> contract.
/// </summary>
public sealed class ContentAnalysisToolAdapter : IOllamaTool
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IContentAnalysisTool _analysisTool;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentAnalysisToolAdapter"/> class.
    /// </summary>
    /// <param name="analysisTool">The adapted content analysis tool.</param>
    public ContentAnalysisToolAdapter(IContentAnalysisTool analysisTool)
    {
        _analysisTool = analysisTool ?? throw new ArgumentNullException(nameof(analysisTool));
    }

    /// <inheritdoc/>
    public string Name => _analysisTool.Name;

    /// <inheritdoc/>
    public string Description => _analysisTool.Description;

    /// <inheritdoc/>
    public OllamaToolSchema Schema => _analysisTool.Schema;

    /// <inheritdoc/>
    public OllamaToolValidationResult Validate(string input)
    {
        if (!TryDeserializeRequest(input, out ContentAnalysisRequest? request, out string errorMessage))
        {
            return OllamaToolValidationResult.Failure(errorMessage);
        }

        ContentAnalysisRequest analysisRequest = request ?? throw new InvalidOperationException("The content analysis tool input must deserialize to a content analysis request.");

        ContentAnalysisRequestValidationResult validationResult = _analysisTool.Validate(analysisRequest);
        if (validationResult.IsValid)
        {
            return OllamaToolValidationResult.Success();
        }

        return OllamaToolValidationResult.Failure(validationResult.Issues.Select(static issue => issue.Message).ToArray());
    }

    /// <inheritdoc/>
    public async Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        if (!TryDeserializeRequest(input, out ContentAnalysisRequest? request, out string errorMessage))
        {
            return new OllamaToolResult
            {
                Success = false,
                Output = errorMessage,
            };
        }

        ContentAnalysisRequest analysisRequest = request ?? throw new InvalidOperationException("The content analysis tool input must deserialize to a content analysis request.");

        ContentAnalysisRequestValidationResult validationResult = _analysisTool.Validate(analysisRequest);
        if (!validationResult.IsValid)
        {
            return new OllamaToolResult
            {
                Success = false,
                Output = string.Join(" ", validationResult.Issues.Select(static issue => issue.Message)),
            };
        }

        ContentAnalysisResult result = await _analysisTool.AnalyzeAsync(analysisRequest, cancellationToken);
        return new OllamaToolResult
        {
            Success = true,
            Output = JsonSerializer.Serialize(result, SerializerOptions),
        };
    }

    private static bool TryDeserializeRequest(string input, out ContentAnalysisRequest? request, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            request = null;
            errorMessage = "The content analysis tool input must not be empty.";
            return false;
        }

        try
        {
            request = JsonSerializer.Deserialize<ContentAnalysisRequest>(input, SerializerOptions);
        }
        catch (JsonException)
        {
            request = null;
            errorMessage = "The content analysis tool input must be valid JSON.";
            return false;
        }

        if (request is null)
        {
            errorMessage = "The content analysis tool input must deserialize to a content analysis request.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
