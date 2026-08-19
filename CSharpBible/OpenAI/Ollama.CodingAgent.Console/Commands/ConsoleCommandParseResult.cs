using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Ollama.CodingAgent.Console.Commands;

/// <summary>
/// Describes a successful command parse or a safe input error.
/// </summary>
public sealed class ConsoleCommandParseResult
{
    /// <summary>
    /// Gets the parsed command when parsing succeeded.
    /// </summary>
    public ConsoleCommand? Command { get; init; }

    /// <summary>
    /// Gets an input error that can be shown to the operator.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets a value indicating whether parsing succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Command))]
    public bool Success => Command is not null;
}
