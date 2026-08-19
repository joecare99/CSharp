using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents one persisted LLM traffic record.
/// </summary>
public sealed class LlmTrafficLogEntry
{
    /// <summary>Gets the UTC timestamp.</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the session identifier.</summary>
    public required string SessionId { get; init; }

    /// <summary>Gets the provider name.</summary>
    public required string Provider { get; init; }

    /// <summary>Gets the traffic direction.</summary>
    public required string Direction { get; init; }

    /// <summary>Gets the operation name.</summary>
    public required string Operation { get; init; }

    /// <summary>Gets the redacted endpoint.</summary>
    public required string Endpoint { get; init; }

    /// <summary>Gets the optional HTTP status code.</summary>
    public int? StatusCode { get; init; }

    /// <summary>Gets the redacted request or response payload.</summary>
    public string? Payload { get; init; }

    /// <summary>Gets the redacted exception type.</summary>
    public string? ExceptionType { get; init; }

    /// <summary>Gets the redacted exception message.</summary>
    public string? ExceptionMessage { get; init; }

    /// <summary>Gets the redacted request headers.</summary>
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}
