using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using CommandlineHelper;

namespace Ollama.CodingAgent.Desktop.Host;

[CommandDescriptor("ollama-coding-agent-desktop")]
internal sealed class DesktopCommandOptions
{
    [CommandOption("--endpoint")]
    public string? Endpoint { get; init; } = "http://localhost:11434/";

    [CommandOption("--model")]
    public string? Model { get; init; } = "gemma4:e4b";

    [CommandOption("--workspace-root")]
    public string? WorkspacePath { get; init; } = ".";

    [CommandOption("--session")]
    public string? SessionId { get; init; } = "default";

    [CommandOption("--code-wiki-vault")]
    public string? CodeWikiVaultPath { get; init; }
}
