using System;

namespace Ollama.Client;

/// <summary>
/// Provides options for the public Ollama client layer.
/// </summary>
public sealed class OllamaClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaClientOptions"/> class.
    /// </summary>
    /// <param name="endpoint">The base Ollama endpoint.</param>
    public OllamaClientOptions(Uri endpoint)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    /// <summary>
    /// Gets the base Ollama endpoint.
    /// </summary>
    public Uri Endpoint { get; }
}
