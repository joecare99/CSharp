using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Client.Models;

namespace Ollama.CodingAgent;

/// <summary>
/// Adapts the public Ollama client to the baseline-check contract.
/// </summary>
public sealed class OllamaClientBaselineAdapter : IOllamaBaselineClient
{
    private readonly OllamaClient _ollamaClient;
    private readonly string _model;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaClientBaselineAdapter"/> class.
    /// </summary>
    public OllamaClientBaselineAdapter(OllamaClient ollamaClient, string model)
    {
        _ollamaClient = ollamaClient ?? throw new ArgumentNullException(nameof(ollamaClient));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _ollamaClient.GetTagsAsync(cancellationToken);
        return tags.Models
            .Select(static model => model.Name ?? model.Model ?? string.Empty)
            .Where(static name => !string.IsNullOrWhiteSpace(name))
            .ToArray();
    }

    /// <inheritdoc />
    public Task<OllamaChatCompletion> CompleteChatAsync(string prompt, CancellationToken cancellationToken = default)
        => _ollamaClient.GetChatClient(_model).CompleteChatAsync(prompt, cancellationToken);
}
