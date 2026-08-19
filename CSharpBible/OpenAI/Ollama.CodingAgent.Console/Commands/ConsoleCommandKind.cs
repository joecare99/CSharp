using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
namespace Ollama.CodingAgent.Console.Commands;

/// <summary>
/// Identifies a command supported by the interactive terminal client.
/// </summary>
public enum ConsoleCommandKind
{
    /// <summary>
    /// Represents an empty input line.
    /// </summary>
    Empty,

    /// <summary>
    /// Represents a normal agent prompt.
    /// </summary>
    Prompt,

    /// <summary>
    /// Displays or changes the endpoint, model, and workspace configuration.
    /// </summary>
    Config,

    /// <summary>
    /// Displays command usage.
    /// </summary>
    Help,

    /// <summary>
    /// Displays current session state.
    /// </summary>
    Status,

    /// <summary>
    /// Displays the visible session transcript.
    /// </summary>
    Transcript,

    /// <summary>
    /// Reloads the persisted session snapshot.
    /// </summary>
    Reload,

    /// <summary>
    /// Clears the persisted session transcript.
    /// </summary>
    Clear,

    /// <summary>
    /// Cancels an active agent request when possible.
    /// </summary>
    Cancel,

    /// <summary>
    /// Displays pending approval requests.
    /// </summary>
    Approvals,

    /// <summary>
    /// Approves a pending operation.
    /// </summary>
    Approve,

    /// <summary>
    /// Rejects a pending operation.
    /// </summary>
    Reject,

    /// <summary>
    /// Ends the interactive session.
    /// </summary>
    Exit,
}
