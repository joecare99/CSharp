using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BaseLib.Models.Interfaces;

namespace Ollama.CodingAgent;

/// <summary>
/// Persists redacted LLM traffic as session-scoped JSON Lines.
/// </summary>
public sealed class FileLlmTrafficLogger : ILlmTrafficLogger, ILog
{
    private readonly object _syncRoot = new();
    private readonly string _logFilePath;
    private readonly string _sessionId;

    /// <summary>
    /// Initializes a logger in the central application-data location.
    /// </summary>
    public FileLlmTrafficLogger(string sessionId)
        : this(FileLlmTrafficLogOptions.CreateDefault(), sessionId)
    {
    }

    /// <summary>
    /// Initializes a logger with explicit location and application naming options.
    /// </summary>
    /// <param name="options">The log location and naming options.</param>
    /// <param name="sessionId">The session identifier.</param>
    public FileLlmTrafficLogger(FileLlmTrafficLogOptions options, string sessionId)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        if (sessionId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || sessionId.Contains(Path.DirectorySeparatorChar)
            || sessionId.Contains(Path.AltDirectorySeparatorChar)
            || sessionId is "." or "..")
        {
            throw new ArgumentException("The session identifier must be a safe file name.", nameof(sessionId));
        }

        string logDirectory = Path.Combine(options.BaseDirectory, options.VendorName, options.ApplicationName, "Logs");
        Directory.CreateDirectory(logDirectory);
        _sessionId = sessionId;
        string timestamp = options.SessionStartTimestamp.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'", System.Globalization.CultureInfo.InvariantCulture);
        _logFilePath = Path.Combine(logDirectory, $"{timestamp}-{sessionId}.jsonl");
    }

    /// <summary>
    /// Gets the persisted log file path.
    /// </summary>
    public string LogFilePath => _logFilePath;

    /// <inheritdoc />
    public void LogRequest(
        string provider,
        Uri endpoint,
        string operation,
        string payload,
        IReadOnlyDictionary<string, string>? headers = null)
        => Write(new LlmTrafficLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _sessionId,
            Provider = LlmTrafficRedactor.Redact(provider),
            Direction = "request",
            Operation = LlmTrafficRedactor.Redact(operation),
            Endpoint = LlmTrafficRedactor.Redact(endpoint.ToString()),
            Payload = LlmTrafficRedactor.Redact(payload),
            Headers = headers is null ? null : RedactHeaders(headers),
        });

    /// <inheritdoc />
    public void LogResponse(
        string provider,
        Uri endpoint,
        string operation,
        int? statusCode,
        string payload)
        => Write(new LlmTrafficLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _sessionId,
            Provider = LlmTrafficRedactor.Redact(provider),
            Direction = "response",
            Operation = LlmTrafficRedactor.Redact(operation),
            Endpoint = LlmTrafficRedactor.Redact(endpoint.ToString()),
            StatusCode = statusCode,
            Payload = LlmTrafficRedactor.Redact(payload),
        });

    /// <inheritdoc />
    public void LogFailure(
        string provider,
        Uri endpoint,
        string operation,
        Exception exception,
        string? payload = null)
    {
        ArgumentNullException.ThrowIfNull(exception);
        Write(new LlmTrafficLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _sessionId,
            Provider = LlmTrafficRedactor.Redact(provider),
            Direction = "failure",
            Operation = LlmTrafficRedactor.Redact(operation),
            Endpoint = LlmTrafficRedactor.Redact(endpoint.ToString()),
            Payload = payload is null ? null : LlmTrafficRedactor.Redact(payload),
            ExceptionType = LlmTrafficRedactor.Redact(exception.GetType().FullName ?? exception.GetType().Name),
            ExceptionMessage = LlmTrafficRedactor.Redact(exception.Message),
        });
    }

    /// <inheritdoc />
    public void Log(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Write(new LlmTrafficLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _sessionId,
            Provider = "agent",
            Direction = "diagnostic",
            Operation = "log",
            Endpoint = string.Empty,
            Payload = LlmTrafficRedactor.Redact(message),
        });
    }

    /// <inheritdoc />
    public void Log(string message, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(exception);
        Write(new LlmTrafficLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = _sessionId,
            Provider = "agent",
            Direction = "diagnostic",
            Operation = "log",
            Endpoint = string.Empty,
            Payload = LlmTrafficRedactor.Redact(message),
            ExceptionType = LlmTrafficRedactor.Redact(exception.GetType().FullName ?? exception.GetType().Name),
            ExceptionMessage = LlmTrafficRedactor.Redact(exception.Message),
        });
    }

    private static IReadOnlyDictionary<string, string> RedactHeaders(IReadOnlyDictionary<string, string> headers)
    {
        Dictionary<string, string> redacted = new(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> header in headers)
        {
            redacted[header.Key] = LlmTrafficRedactor.Redact(header.Value);
        }

        return redacted;
    }

    private void Write(LlmTrafficLogEntry entry)
    {
        try
        {
            string line = JsonSerializer.Serialize(entry);
            lock (_syncRoot)
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
