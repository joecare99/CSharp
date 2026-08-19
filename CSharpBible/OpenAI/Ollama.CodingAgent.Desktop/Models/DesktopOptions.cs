using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandlineHelper;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Desktop.Host;

namespace Ollama.CodingAgent.Desktop.Models;

/// <summary>
/// Defines the desktop host configuration used to compose one persistent session.
/// </summary>
public sealed class DesktopOptions
{
    public DesktopOptions()
    {
    }

    public required string Endpoint { get; init; }
    public required string Model { get; init; }
    public required string WorkspacePath { get; init; }
    public required string SessionId { get; init; }
    public required string CodeWikiVaultPath { get; init; }

    public static DesktopOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);
        CommandParseResult<DesktopCommandOptions> result =
            DesktopCommandOptionsCommand.Parse(NormalizeArguments(args).ToArray());
        bool showHelp = args.Any(static argument =>
            string.Equals(argument, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(argument, "-h", StringComparison.OrdinalIgnoreCase));

        if (result.RequestHelp)
        {
            return Create(new DesktopCommandOptions(), true);
        }

        if (!result.Success || result.Options is null)
        {
            throw new ArgumentException(result.ErrorMessage ?? "The command-line arguments are invalid.", nameof(args));
        }

        return Create(result.Options, showHelp);
    }

    public OllamaAgentCliOptions ToRuntimeOptions()
        => ToRuntimeOptions(new DesktopConfiguration
        {
            Endpoint = Endpoint,
            Model = Model,
            WorkspacePath = WorkspacePath,
        });

    public OllamaAgentCliOptions ToRuntimeOptions(DesktopConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        DesktopConfiguration normalized = configuration.Normalize();
        return OllamaAgentCliOptions.Parse(
        [
            "--endpoint", normalized.Endpoint,
            "--model", normalized.Model,
            "--workspace-root", normalized.WorkspacePath,
            "--session", SessionId,
        ]);
    }

    private static DesktopOptions Create(DesktopCommandOptions options, bool _)
    {
        string endpoint = options.Endpoint ?? Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434/";
        string model = options.Model ?? Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen2.5-coder:7b";
        string workspace = options.WorkspacePath
            ?? Environment.GetEnvironmentVariable("AGENT_WORKSPACE")
            ?? Environment.CurrentDirectory;
        string sessionId = options.SessionId
            ?? Environment.GetEnvironmentVariable("AGENT_SESSION")
            ?? "default";
        string wikiVault = options.CodeWikiVaultPath
            ?? Environment.GetEnvironmentVariable("CODE_WIKI_VAULT")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CodeWikiVault");

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
        return new DesktopOptions
        {
            Endpoint = endpointUri.AbsoluteUri,
            Model = model,
            WorkspacePath = Path.GetFullPath(workspace),
            SessionId = sessionId,
            CodeWikiVaultPath = Path.GetFullPath(wikiVault),
        };
    }

    private static IReadOnlyList<string> NormalizeArguments(IReadOnlyList<string> args)
    {
        List<string> normalized = [];
        for (int index = 0; index < args.Count; index++)
        {
            normalized.Add(args[index] == "--workspace" ? "--workspace-root" : args[index]);
        }

        AddEnvironmentDefault(normalized, args, "OLLAMA_ENDPOINT", "--endpoint");
        AddEnvironmentDefault(normalized, args, "OLLAMA_MODEL", "--model");
        AddEnvironmentDefault(normalized, args, "AGENT_WORKSPACE", "--workspace-root", "--workspace");
        AddEnvironmentDefault(normalized, args, "AGENT_SESSION", "--session");
        AddEnvironmentDefault(normalized, args, "CODE_WIKI_VAULT", "--code-wiki-vault");
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
