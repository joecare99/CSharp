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
/// Holds one parsed command or prompt, independent from console I/O.
/// </summary>
public sealed class ConsoleCommand
{
    /// <summary>
    /// Gets the parsed command type.
    /// </summary>
    public required ConsoleCommandKind Kind { get; init; }

    /// <summary>
    /// Gets the prompt text or command argument.
    /// </summary>
    public string? Argument { get; init; }
}
