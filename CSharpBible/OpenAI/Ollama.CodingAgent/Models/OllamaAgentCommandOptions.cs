using CommandlineHelper;

namespace Ollama.CodingAgent.Models;

[CommandDescriptor("ollama-coding-agent")]
internal sealed class OllamaAgentCommandOptions
{
    [CommandOption("--endpoint", ShortName = "-e")]
    public string? Endpoint { get; init; } = "http://localhost:11434/";

    [CommandOption("--model", ShortName = "-m")]
    public string? Model { get; init; } = "qwen2.5-coder:7b";

    [CommandOption("--timeout-minutes")]
    public string? TimeoutMinutes { get; init; } = "12";

    [CommandOption("--retries")]
    public string? Retries { get; init; } = "3";

    [CommandOption("--max-iterations")]
    public string? MaxIterations { get; init; } = "80";

    [CommandOption("--verbosity")]
    public string? Verbosity { get; init; } = "normal";

    [CommandFlag("--show-thinking")]
    public bool ShowThinking { get; init; }

    [CommandFlag("--log-tool-calls")]
    public bool LogToolCalls { get; init; }

    [CommandFlag("--preflight")]
    public bool PreflightOnly { get; init; }

    [CommandFlag("--baseline-smoke")]
    public bool BaselineSmoke { get; init; }

    [CommandOption("--prompt", ShortName = "-p")]
    public string? ExplicitPrompt { get; init; }

    [CommandFlag("--delegate")]
    public bool DelegateMode { get; init; }

    [CommandOption("--workspace-root", ShortName = "-w")]
    public string? WorkspaceRoot { get; init; } = ".";

    [CommandOption("--session")]
    public string? SessionId { get; init; } = "default";

    [CommandArgument(0)]
    public string? PositionalPrompt { get; init; }

}
