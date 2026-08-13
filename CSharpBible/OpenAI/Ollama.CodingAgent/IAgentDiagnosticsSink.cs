namespace Ollama.CodingAgent;

/// <summary>
/// Receives structured agent runtime diagnostics.
/// </summary>
public interface IAgentDiagnosticsSink
{
    /// <summary>
    /// Records one diagnostic event.
    /// </summary>
    void Record(AgentDiagnosticEvent diagnosticEvent);
}
