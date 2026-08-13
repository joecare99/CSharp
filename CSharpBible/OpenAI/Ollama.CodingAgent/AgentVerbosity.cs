namespace Ollama.CodingAgent;

/// <summary>
/// Controls how much runtime information is included in the command-line output.
/// </summary>
public enum AgentVerbosity
{
    /// <summary>
    /// Prints only the final response.
    /// </summary>
    Quiet,

    /// <summary>
    /// Prints the final response and basic run metadata.
    /// </summary>
    Normal,

    /// <summary>
    /// Prints detailed tool, retry, and model diagnostics.
    /// </summary>
    Verbose,
}
