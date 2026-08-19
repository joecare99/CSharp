using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Diagnostics;

/// <summary>
/// Publishes provider-neutral runtime diagnostics to interactive hosts.
/// </summary>
public sealed class AgentDiagnosticsChannel : IAgentDiagnosticsSink
{
    /// <summary>
    /// Raised when a diagnostic event is recorded.
    /// </summary>
    public event EventHandler<AgentDiagnosticEvent>? EventRecorded;

    /// <summary>
    /// Records and publishes one diagnostic event.
    /// </summary>
    /// <param name="diagnosticEvent">The diagnostic event.</param>
    public void Record(AgentDiagnosticEvent diagnosticEvent)
    {
        ArgumentNullException.ThrowIfNull(diagnosticEvent);
        EventRecorded?.Invoke(this, diagnosticEvent);
    }
}
