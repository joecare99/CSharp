using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Runs repeatable endpoint, model-readiness, and one-turn smoke checks.
/// </summary>
public sealed class OllamaBaselineService
{
    private readonly IOllamaBaselineClient _client;
    private readonly string _model;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBaselineService"/> class.
    /// </summary>
    public OllamaBaselineService(IOllamaBaselineClient client, string model)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
    }

    /// <summary>
    /// Verifies endpoint reachability and model availability.
    /// </summary>
    public async Task<OllamaBaselineCheckResult> RunPreflightAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var availableModels = await _client.GetAvailableModelsAsync(cancellationToken);
            bool modelAvailable = availableModels.Any(model => string.Equals(model, _model, StringComparison.OrdinalIgnoreCase));
            return new OllamaBaselineCheckResult
            {
                Success = modelAvailable,
                ModelAvailable = modelAvailable,
                AvailableModels = availableModels,
                Error = modelAvailable ? null : $"Model '{_model}' is not available at the configured endpoint.",
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new OllamaBaselineCheckResult
            {
                Success = false,
                Error = $"Ollama preflight failed: {ex.Message}",
            };
        }
    }

    /// <summary>
    /// Runs preflight and one non-empty chat completion.
    /// </summary>
    public async Task<OllamaBaselineCheckResult> RunSmokeAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        OllamaBaselineCheckResult preflight = await RunPreflightAsync(cancellationToken);
        if (!preflight.Success)
        {
            return preflight;
        }

        try
        {
            var completion = await _client.CompleteChatAsync(prompt, cancellationToken);
            if (string.IsNullOrWhiteSpace(completion.Content))
            {
                return new OllamaBaselineCheckResult
                {
                    Success = false,
                    ModelAvailable = true,
                    AvailableModels = preflight.AvailableModels,
                    Error = "Ollama returned an empty baseline response.",
                };
            }

            return new OllamaBaselineCheckResult
            {
                Success = true,
                ModelAvailable = true,
                AvailableModels = preflight.AvailableModels,
                Response = completion.Content,
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new OllamaBaselineCheckResult
            {
                Success = false,
                ModelAvailable = true,
                AvailableModels = preflight.AvailableModels,
                Error = $"Ollama baseline smoke request failed: {ex.Message}",
            };
        }
    }
}
