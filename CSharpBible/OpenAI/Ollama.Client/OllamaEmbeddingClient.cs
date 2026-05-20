using System;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;
using Ollama.Protocol.Models;

namespace Ollama.Client;

/// <summary>
/// Provides model-scoped embedding operations.
/// </summary>
public sealed class OllamaEmbeddingClient
{
    private readonly string _model;
    private readonly IOllamaProtocolAdapter _protocolAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaEmbeddingClient"/> class.
    /// </summary>
    /// <param name="protocolClient">The underlying protocol client.</param>
    /// <param name="model">The bound model name.</param>
    public OllamaEmbeddingClient(IOllamaProtocolAdapter protocolAdapter, string model)
    {
        _protocolAdapter = protocolAdapter ?? throw new ArgumentNullException(nameof(protocolAdapter));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
    }

    /// <summary>
    /// Requests embeddings for the provided input value.
    /// </summary>
    /// <param name="input">The input text.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The embedding result.</returns>
    public async Task<OllamaEmbeddingResult> GenerateEmbeddingAsync(string input, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        return await GenerateEmbeddingAsync(
            new EmbeddingOptions
            {
                Input = [input],
            },
            cancellationToken);
    }

    /// <summary>
    /// Requests embeddings for the provided options.
    /// </summary>
    /// <param name="options">The embedding options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The embedding result.</returns>
    public async Task<OllamaEmbeddingResult> GenerateEmbeddingAsync(EmbeddingOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Input);
        if (options.Input.Count == 0)
        {
            throw new ArgumentException("At least one embedding input is required.", nameof(options));
        }

        foreach (string input in options.Input)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(input);
        }

        OllamaEmbedResponse response = await _protocolAdapter.EmbedAsync(
            new OllamaEmbedRequest
            {
                Model = _model,
                Input = options.Input,
            },
            cancellationToken);

        return new OllamaEmbeddingResult
        {
            Embeddings = response.Embeddings,
        };
    }
}
