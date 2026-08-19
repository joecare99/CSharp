using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandlineHelper;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Console.Configuration;

/// <summary>
/// Defines the terminal-specific configuration needed to open a persistent agent session.
/// </summary>
public sealed class ConsoleAgentCliOptions
{
    private ConsoleAgentCliOptions()
    {
    }

    public required string Endpoint { get; init; }
    public required string Model { get; init; }
    public required string WorkspacePath { get; init; }
    public required string SessionId { get; init; }
    public required bool ShowHelp { get; init; }

    public static ConsoleAgentCliOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        bool showHelp = args.Any(static argument =>
            string.Equals(argument, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(argument, "-h", StringComparison.OrdinalIgnoreCase));
        CommandParseResult<ConsoleAgentCommandOptions> result =
            ConsoleAgentCommandOptionsCommand.Parse(NormalizeArguments(args).ToArray());

        if (result.RequestHelp)
        {
            return Create(new ConsoleAgentCommandOptions(), true);
        }

        if (!result.Success || result.Options is null)
        {
            throw new ArgumentException(result.ErrorMessage ?? "The command-line arguments are invalid.", nameof(args));
        }

        return Create(result.Options, showHelp);
    }

    public OllamaAgentCliOptions ToRuntimeOptions()
        => OllamaAgentCliOptions.Parse(
        [
            "--endpoint", Endpoint,
            "--model", Model,
            "--workspace-root", WorkspacePath,
            "--session", SessionId,
        ]);

    private static ConsoleAgentCliOptions Create(ConsoleAgentCommandOptions options, bool showHelp)
    {
        string endpoint = options.Endpoint ?? Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434/";
        string model = options.Model ?? Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen2.5-coder:7b";
        string workspace = options.WorkspacePath
            ?? Environment.GetEnvironmentVariable("AGENT_WORKSPACE")
            ?? Environment.CurrentDirectory;
        string sessionId = options.SessionId
            ?? Environment.GetEnvironmentVariable("AGENT_SESSION")
            ?? "default";

        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? endpointUri)
            || (endpointUri.Scheme != Uri.UriSchemeHttp && endpointUri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("The endpoint must be an absolute HTTP or HTTPS URL.", nameof(endpoint));
        }

        ValidateSessionId(sessionId);
        return new ConsoleAgentCliOptions
        {
            Endpoint = endpointUri.AbsoluteUri,
            Model = model,
            WorkspacePath = Path.GetFullPath(workspace),
            SessionId = sessionId,
            ShowHelp = showHelp,
        };
    }

    private static IReadOnlyList<string> NormalizeArguments(IReadOnlyList<string> args)
    {
        List<string> normalized = [];
        for (int index = 0; index < args.Count; index++)
        {
            string argument = args[index] switch
            {
                "--workspace" => "--workspace-root",
                _ => args[index],
            };
            normalized.Add(argument);
        }

        AddEnvironmentDefault(normalized, args, "OLLAMA_ENDPOINT", "--endpoint");
        AddEnvironmentDefault(normalized, args, "OLLAMA_MODEL", "--model");
        AddEnvironmentDefault(normalized, args, "AGENT_WORKSPACE", "--workspace-root", "--workspace");
        AddEnvironmentDefault(normalized, args, "AGENT_SESSION", "--session");
        return normalized;
    }

    private static void AddEnvironmentDefault(
        ICollection<string> target,
        IReadOnlyList<string> args,
        string variableName,
        string optionName,
        params string[] aliases)
    {
        if (args.Any(argument => string.Equals(argument, optionName, StringComparison.OrdinalIgnoreCase)
            || aliases.Any(alias => string.Equals(argument, alias, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }

        string? value = Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrWhiteSpace(value))
        {
            target.Add(optionName);
            target.Add(value);
        }
    }

    private static void ValidateSessionId(string sessionId)
    {
        if (sessionId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
            || sessionId.Contains(Path.DirectorySeparatorChar)
            || sessionId.Contains(Path.AltDirectorySeparatorChar)
            || sessionId is "." or "..")
        {
            throw new ArgumentException("The session identifier must be a safe file name.", nameof(sessionId));
        }
    }
}
