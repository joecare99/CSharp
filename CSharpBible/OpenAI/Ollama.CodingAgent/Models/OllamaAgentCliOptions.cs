using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents validated command-line options for the coding-agent host.
/// </summary>
public sealed class OllamaAgentCliOptions
{
    private const string DefaultPrompt = "Summarize how you would solve a medium C# coding task in three concise steps.";

    private OllamaAgentCliOptions()
    {
    }

    public required string Endpoint { get; init; }

    public required string Model { get; init; }

    public required string Prompt { get; init; }

    public required OllamaAgentRuntimeSettings RuntimeSettings { get; init; }

    public required bool DelegateMode { get; init; }

    public required string WorkspaceRoot { get; init; }

    public required string SessionId { get; init; }

    public required bool ShowHelp { get; init; }

    public required bool PreflightOnly { get; init; }

    public required bool BaselineSmoke { get; init; }

    /// <summary>
    /// Parses command-line arguments through the shared generated command parser.
    /// </summary>
    public static OllamaAgentCliOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        bool showHelp = args.Any(static argument =>
            string.Equals(argument, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(argument, "-h", StringComparison.OrdinalIgnoreCase));
        string[] parseArguments = NormalizePositionalPrompt(args)
            .Where(static argument =>
                !string.Equals(argument, "--help", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(argument, "-h", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        CommandlineHelper.CommandParseResult<OllamaAgentCommandOptions> result =
            OllamaAgentCommandOptionsCommand.Parse(AddEnvironmentDefaults(parseArguments));
        if (result.RequestHelp)
        {
            return CreateFrom(new OllamaAgentCommandOptions(), showHelp: true);
        }

        if (!result.Success || result.Options is null)
        {
            throw new ArgumentException(result.ErrorMessage ?? "The command-line arguments are invalid.", nameof(args));
        }

        return CreateFrom(result.Options, showHelp);
    }

    private static OllamaAgentCliOptions CreateFrom(OllamaAgentCommandOptions options, bool showHelp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Model);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.TimeoutMinutes);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Retries);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.MaxIterations);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Verbosity);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.WorkspaceRoot);

        string prompt = !string.IsNullOrWhiteSpace(options.ExplicitPrompt)
            ? options.ExplicitPrompt
            : !string.IsNullOrWhiteSpace(options.PositionalPrompt)
                ? options.PositionalPrompt
                : DefaultPrompt;

        return new OllamaAgentCliOptions
        {
            Endpoint = options.Endpoint,
            Model = options.Model,
            Prompt = prompt,
            RuntimeSettings = new OllamaAgentRuntimeSettings(
                TimeSpan.FromMinutes(ParseDouble(options.TimeoutMinutes, "--timeout-minutes")),
                ParseInt(options.Retries, "--retries"),
                ParseInt(options.MaxIterations, "--max-iterations"),
                ParseVerbosity(options.Verbosity),
                options.ShowThinking,
                logToolCalls: options.LogToolCalls),
            DelegateMode = options.DelegateMode,
            WorkspaceRoot = options.WorkspaceRoot,
            SessionId = options.SessionId ?? Environment.GetEnvironmentVariable("AGENT_SESSION") ?? "default",
            ShowHelp = showHelp,
            PreflightOnly = options.PreflightOnly,
            BaselineSmoke = options.BaselineSmoke,
        };
    }

    private static string[] AddEnvironmentDefaults(IReadOnlyList<string> args)
    {
        List<string> effectiveArgs = new(args);
        AddEnvironmentOption(effectiveArgs, args, "OLLAMA_ENDPOINT", "--endpoint");
        AddEnvironmentOption(effectiveArgs, args, "OLLAMA_MODEL", "--model");
        AddEnvironmentOption(effectiveArgs, args, "AGENT_TIMEOUT_MINUTES", "--timeout-minutes");
        AddEnvironmentOption(effectiveArgs, args, "AGENT_RETRY_COUNT", "--retries");
        AddEnvironmentOption(effectiveArgs, args, "AGENT_MAX_ITERATIONS", "--max-iterations");
        AddEnvironmentOption(effectiveArgs, args, "AGENT_VERBOSITY", "--verbosity");
        AddEnvironmentOption(effectiveArgs, args, "AGENT_WORKSPACE_ROOT", "--workspace-root");

        if (!ContainsOption(args, "--show-thinking")
            && bool.TryParse(Environment.GetEnvironmentVariable("AGENT_SHOW_THINKING"), out bool showThinking)
            && showThinking)
        {
            effectiveArgs.Add("--show-thinking");
        }

        if (!ContainsOption(args, "--log-tool-calls")
            && bool.TryParse(Environment.GetEnvironmentVariable("AGENT_LOG_TOOL_CALLS"), out bool logToolCalls)
            && logToolCalls)
        {
            effectiveArgs.Add("--log-tool-calls");
        }

        return effectiveArgs.ToArray();
    }

    private static IReadOnlyList<string> NormalizePositionalPrompt(IReadOnlyList<string> args)
    {
        HashSet<string> valueOptions =
        [
            "--endpoint", "-e", "--model", "-m", "--timeout-minutes", "--retries",
            "--max-iterations", "--verbosity", "--prompt", "-p", "--workspace-root", "-w",
            "--session",
        ];
        HashSet<string> flagOptions =
        [
            "--help", "-h", "--show-thinking", "--log-tool-calls", "--preflight", "--baseline-smoke", "--delegate",
        ];
        List<string> normalized = [];
        List<string> positional = [];
        bool hasExplicitPrompt = false;

        for (int index = 0; index < args.Count; index++)
        {
            string argument = args[index];
            if (argument.StartsWith("-", StringComparison.Ordinal)
                && !valueOptions.Contains(argument)
                && !flagOptions.Contains(argument))
            {
                throw new ArgumentException($"Unknown option '{argument}'.", nameof(args));
            }

            normalized.Add(argument);
            if (valueOptions.Contains(argument))
            {
                if (string.Equals(argument, "--prompt", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(argument, "-p", StringComparison.OrdinalIgnoreCase))
                {
                    hasExplicitPrompt = true;
                }

                if (index + 1 < args.Count)
                {
                    normalized.Add(args[++index]);
                }

                continue;
            }

            if (!argument.StartsWith("-", StringComparison.Ordinal))
            {
                normalized.RemoveAt(normalized.Count - 1);
                positional.Add(argument);
            }
        }

        if (!hasExplicitPrompt && positional.Count > 0)
        {
            normalized.Add("--prompt");
            normalized.Add(string.Join(" ", positional));
        }

        return normalized;
    }

    private static void AddEnvironmentOption(
        ICollection<string> target,
        IReadOnlyList<string> args,
        string variableName,
        string optionName)
    {
        if (ContainsOption(args, optionName))
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

    private static bool ContainsOption(IReadOnlyList<string> args, string optionName)
    {
        foreach (string arg in args)
        {
            if (string.Equals(arg, optionName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static double ParseDouble(string value, string optionName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
        {
            throw new ArgumentException($"Value '{value}' for '{optionName}' is not a valid number.");
        }

        return parsedValue;
    }

    private static int ParseInt(string value, string optionName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
        {
            throw new ArgumentException($"Value '{value}' for '{optionName}' is not a valid integer.");
        }

        return parsedValue;
    }

    private static AgentVerbosity ParseVerbosity(string value)
    {
        if (Enum.TryParse(value, ignoreCase: true, out AgentVerbosity parsedValue))
        {
            return parsedValue;
        }

        throw new ArgumentException($"Value '{value}' for '--verbosity' must be quiet, normal, or verbose.");
    }
}
