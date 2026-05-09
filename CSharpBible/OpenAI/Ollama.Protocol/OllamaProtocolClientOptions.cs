using System;

namespace Ollama.Protocol;

/// <summary>
/// Provides configuration for the <see cref="OllamaProtocolClient"/>.
/// </summary>
public sealed class OllamaProtocolClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProtocolClientOptions"/> class.
    /// </summary>
    /// <param name="endpoint">The base Ollama server endpoint.</param>
    public OllamaProtocolClientOptions(Uri endpoint)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    /// <summary>
    /// Gets the base Ollama server endpoint.
    /// </summary>
    public Uri Endpoint { get; }
}
