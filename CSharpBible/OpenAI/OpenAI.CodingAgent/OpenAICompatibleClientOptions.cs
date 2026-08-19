using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace OpenAI.CodingAgent;

/// <summary>
/// Configures an OpenAI-compatible chat-completions endpoint.
/// </summary>
public sealed class OpenAICompatibleClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleClientOptions"/> class.
    /// </summary>
    public OpenAICompatibleClientOptions(Uri endpoint, string model, string? apiKey = null)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        Model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("The model name must not be empty.", nameof(model))
            : model;
        ApiKey = apiKey;
    }

    /// <summary>
    /// Gets the compatible API base endpoint.
    /// </summary>
    public Uri Endpoint { get; }

    /// <summary>
    /// Gets the model identifier.
    /// </summary>
    public string Model { get; }

    /// <summary>
    /// Gets the optional bearer token.
    /// </summary>
    public string? ApiKey { get; }
}
