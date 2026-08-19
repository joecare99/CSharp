using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using CommandlineHelper;

namespace Ollama.CodingAgent.Console.Configuration;

[CommandDescriptor("ollama-coding-agent-console")]
internal sealed class ConsoleAgentCommandOptions
{
    [CommandOption("--endpoint")]
    public string? Endpoint { get; init; } = "http://localhost:11434/";

    [CommandOption("--model")]
    public string? Model { get; init; } = "qwen2.5-coder:7b";

    [CommandOption("--workspace-root")]
    public string? WorkspacePath { get; init; } = ".";

    [CommandOption("--session")]
    public string? SessionId { get; init; } = "default";
}
