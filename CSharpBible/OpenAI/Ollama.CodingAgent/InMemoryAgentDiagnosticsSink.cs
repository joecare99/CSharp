using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent;

/// <summary>
/// Captures diagnostics for tests and local evaluation runs.
/// </summary>
public sealed class InMemoryAgentDiagnosticsSink : IAgentDiagnosticsSink
{
    private readonly List<AgentDiagnosticEvent> _events = [];

    /// <summary>
    /// Gets the events recorded so far.
    /// </summary>
    public IReadOnlyList<AgentDiagnosticEvent> Events => _events;

    /// <inheritdoc />
    public void Record(AgentDiagnosticEvent diagnosticEvent)
    {
        ArgumentNullException.ThrowIfNull(diagnosticEvent);
        _events.Add(diagnosticEvent);
    }
}
