using Ollama.CodingAgent.Models;
using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Interfaces;

/// <summary>
/// Records redacted traffic exchanged with an LLM provider.
/// </summary>
public interface ILlmTrafficLogger
{
    /// <summary>
    /// Records an outgoing provider request.
    /// </summary>
    void LogRequest(
        string provider,
        Uri endpoint,
        string operation,
        string payload,
        IReadOnlyDictionary<string, string>? headers = null);

    /// <summary>
    /// Records an incoming provider response.
    /// </summary>
    void LogResponse(
        string provider,
        Uri endpoint,
        string operation,
        int? statusCode,
        string payload);

    /// <summary>
    /// Records a provider failure.
    /// </summary>
    void LogFailure(
        string provider,
        Uri endpoint,
        string operation,
        Exception exception,
        string? payload = null);
}
