using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Text.Json;
using BaseLib.Models.Interfaces;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Diagnostics;

/// <summary>
/// Forwards runtime diagnostics to interactive hosts and the redacted session logger.
/// </summary>
public sealed class AgentDiagnosticsSink : IAgentDiagnosticsSink
{
    private readonly AgentDiagnosticsChannel _channel;
    private readonly ILog _log;

    /// <summary>
    /// Initializes a new diagnostics sink.
    /// </summary>
    public AgentDiagnosticsSink(AgentDiagnosticsChannel channel, ILog log)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <inheritdoc />
    public void Record(AgentDiagnosticEvent diagnosticEvent)
    {
        ArgumentNullException.ThrowIfNull(diagnosticEvent);
        _channel.Record(diagnosticEvent);
        _log.Log(JsonSerializer.Serialize(diagnosticEvent));
    }
}
