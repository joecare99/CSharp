using System;

namespace Ollama.Protocol;

/// <summary>
/// Represents a protocol-level failure reported by the Ollama service.
/// </summary>
public sealed class OllamaProtocolException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProtocolException"/> class.
    /// </summary>
    /// <param name="message">The error message reported by the Ollama service.</param>
    public OllamaProtocolException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaProtocolException"/> class.
    /// </summary>
    /// <param name="message">The error message reported by the Ollama service.</param>
    /// <param name="innerException">The inner exception, if any.</param>
    public OllamaProtocolException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}