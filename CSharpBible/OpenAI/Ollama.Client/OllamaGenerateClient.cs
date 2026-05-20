using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;
using Ollama.Protocol.Models;

namespace Ollama.Client;

/// <summary>
/// Provides model-scoped text generation operations.
/// </summary>
public sealed class OllamaGenerateClient
{
    private readonly string _model;
    private readonly IOllamaProtocolAdapter _protocolAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaGenerateClient"/> class.
    /// </summary>
    /// <param name="protocolClient">The underlying protocol client.</param>
    /// <param name="model">The bound model name.</param>
    public OllamaGenerateClient(IOllamaProtocolAdapter protocolAdapter, string model)
    {
        _protocolAdapter = protocolAdapter ?? throw new ArgumentNullException(nameof(protocolAdapter));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
    }

    /// <summary>
    /// Gets a buffered generate result for the provided prompt.
    /// </summary>
    /// <param name="prompt">The input prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The buffered generate result.</returns>
    public async Task<OllamaGeneratedText> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        return await GenerateAsync(
            new GenerateOptions
            {
                Prompt = prompt,
            },
            cancellationToken);
    }

    /// <summary>
    /// Gets a buffered generate result for the provided options.
    /// </summary>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The buffered generate result.</returns>
    public async Task<OllamaGeneratedText> GenerateAsync(GenerateOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Prompt);

        StringBuilder contentBuilder = new();
        List<string> thinking = [];

        await foreach (OllamaStreamingGenerateUpdate update in GenerateStreamingAsync(options, cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(update.Content))
            {
                contentBuilder.Append(update.Content);
            }

            if (!string.IsNullOrWhiteSpace(update.Thinking))
            {
                thinking.Add(update.Thinking);
            }
        }

        return new OllamaGeneratedText
        {
            Content = contentBuilder.ToString(),
            Thinking = thinking,
        };
    }

    /// <summary>
    /// Streams generated text for the provided prompt.
    /// </summary>
    /// <param name="prompt">The input prompt.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of generate updates.</returns>
    public async IAsyncEnumerable<OllamaStreamingGenerateUpdate> GenerateStreamingAsync(
        string prompt,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        await foreach (OllamaStreamingGenerateUpdate update in GenerateStreamingAsync(
            new GenerateOptions
            {
                Prompt = prompt,
            },
            cancellationToken))
        {
            yield return update;
        }
    }

    /// <summary>
    /// Streams generated text for the provided options.
    /// </summary>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of generate updates.</returns>
    public async IAsyncEnumerable<OllamaStreamingGenerateUpdate> GenerateStreamingAsync(
        GenerateOptions options,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Prompt);

        OllamaGenerateRequest request = new()
        {
            Model = _model,
            Prompt = options.Prompt,
            Stream = true,
        };

        await foreach (OllamaGenerateResponseChunk chunk in _protocolAdapter.GenerateStreamingAsync(request, cancellationToken))
        {
            yield return new OllamaStreamingGenerateUpdate
            {
                Content = chunk.Response,
                Thinking = chunk.Thinking,
                Done = chunk.Done,
            };
        }
    }
}
