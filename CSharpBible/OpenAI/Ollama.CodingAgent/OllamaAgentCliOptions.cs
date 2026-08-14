using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents command-line options for the coding-agent host.
/// </summary>
public sealed class OllamaAgentCliOptions
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "qwen2.5-coder:7b";
    private const string DefaultPrompt = "Summarize how you would solve a medium C# coding task in three concise steps.";

    private OllamaAgentCliOptions()
    {
    }

    /// <summary>
    /// Gets the Ollama endpoint value.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets the selected model.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Gets the prompt text.
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// Gets the runtime settings.
    /// </summary>
    public required OllamaAgentRuntimeSettings RuntimeSettings { get; init; }

    /// <summary>
    /// Gets a value indicating whether delegated coding-task mode is enabled.
    /// </summary>
    public required bool DelegateMode { get; init; }

    /// <summary>
    /// Gets the workspace root used by delegated tools.
    /// </summary>
    public required string WorkspaceRoot { get; init; }

    /// <summary>
    /// Gets a value indicating whether help output should be displayed.
    /// </summary>
    public required bool ShowHelp { get; init; }

    /// <summary>
    /// Gets a value indicating whether only endpoint/model preflight should run.
    /// </summary>
    public required bool PreflightOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the baseline one-turn smoke check should run.
    /// </summary>
    public required bool BaselineSmoke { get; init; }

    /// <summary>
    /// Parses command-line arguments into validated options.
    /// </summary>
    /// <param name="args">The incoming command-line arguments.</param>
    /// <returns>The parsed options.</returns>
    public static OllamaAgentCliOptions Parse(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        string endpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;
        string model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? DefaultModel;
        string? explicitPrompt = null;
        bool showHelp = false;
        bool delegateMode = false;
        bool preflightOnly = false;
        bool baselineSmoke = false;
        string workspaceRoot = Environment.CurrentDirectory;

        double timeoutMinutes = ParseDouble(Environment.GetEnvironmentVariable("AGENT_TIMEOUT_MINUTES"), 12d);
        int retries = ParseInt(Environment.GetEnvironmentVariable("AGENT_RETRY_COUNT"), OllamaAgentRuntimeSettings.DefaultRetryCount);
        int maxIterations = ParseInt(Environment.GetEnvironmentVariable("AGENT_MAX_ITERATIONS"), OllamaAgentRuntimeSettings.DefaultMaxIterations);
        AgentVerbosity verbosity = ParseVerbosity(Environment.GetEnvironmentVariable("AGENT_VERBOSITY"), AgentVerbosity.Normal);
        bool showThinking = ParseBool(Environment.GetEnvironmentVariable("AGENT_SHOW_THINKING"));

        List<string> promptParts = [];
        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            switch (argument)
            {
                case "--help":
                case "-h":
                    showHelp = true;
                    break;
                case "--endpoint":
                    endpoint = ReadNextValue(args, ref i, "--endpoint");
                    break;
                case "--model":
                    model = ReadNextValue(args, ref i, "--model");
                    break;
                case "--timeout-minutes":
                    timeoutMinutes = ParseRequiredDouble(ReadNextValue(args, ref i, "--timeout-minutes"), "--timeout-minutes");
                    break;
                case "--retries":
                    retries = ParseRequiredInt(ReadNextValue(args, ref i, "--retries"), "--retries");
                    break;
                case "--max-iterations":
                    maxIterations = ParseRequiredInt(ReadNextValue(args, ref i, "--max-iterations"), "--max-iterations");
                    break;
                case "--verbosity":
                    verbosity = ParseVerbosity(ReadNextValue(args, ref i, "--verbosity"), "--verbosity");
                    break;
                case "--show-thinking":
                    showThinking = true;
                    break;
                case "--preflight":
                    preflightOnly = true;
                    break;
                case "--baseline-smoke":
                    baselineSmoke = true;
                    break;
                case "--prompt":
                    explicitPrompt = ReadNextValue(args, ref i, "--prompt");
                    break;
                case "--delegate":
                    delegateMode = true;
                    break;
                case "--workspace-root":
                    workspaceRoot = ReadNextValue(args, ref i, "--workspace-root");
                    break;
                default:
                    if (argument.StartsWith("-", StringComparison.Ordinal))
                    {
                        throw new ArgumentException($"Unknown option '{argument}'.", nameof(args));
                    }

                    promptParts.Add(argument);
                    break;
            }
        }

        string prompt = ResolvePrompt(explicitPrompt, promptParts);
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);

        return new OllamaAgentCliOptions
        {
            Endpoint = endpoint,
            Model = model,
            Prompt = prompt,
            RuntimeSettings = new OllamaAgentRuntimeSettings(
                TimeSpan.FromMinutes(timeoutMinutes),
                retries,
                maxIterations,
                verbosity,
                showThinking),
            DelegateMode = delegateMode,
            WorkspaceRoot = workspaceRoot,
            ShowHelp = showHelp,
            PreflightOnly = preflightOnly,
            BaselineSmoke = baselineSmoke,
        };
    }

    private static string ReadNextValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"Option '{optionName}' requires a value.", nameof(args));
        }

        index++;
        return args[index];
    }

    private static string ResolvePrompt(string? explicitPrompt, IReadOnlyList<string> promptParts)
    {
        if (!string.IsNullOrWhiteSpace(explicitPrompt))
        {
            return explicitPrompt;
        }

        return promptParts.Count > 0
            ? string.Join(" ", promptParts)
            : DefaultPrompt;
    }

    private static double ParseDouble(string? value, double fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return ParseRequiredDouble(value, nameof(value));
    }

    private static int ParseInt(string? value, int fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return ParseRequiredInt(value, nameof(value));
    }

    private static double ParseRequiredDouble(string value, string optionName)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
        {
            throw new ArgumentException($"Value '{value}' for '{optionName}' is not a valid number.");
        }

        return parsedValue;
    }

    private static int ParseRequiredInt(string value, string optionName)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
        {
            throw new ArgumentException($"Value '{value}' for '{optionName}' is not a valid integer.");
        }

        return parsedValue;
    }

    private static AgentVerbosity ParseVerbosity(string? value, AgentVerbosity fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return ParseVerbosity(value, nameof(value));
    }

    private static AgentVerbosity ParseVerbosity(string value, string optionName)
    {
        if (Enum.TryParse(value, ignoreCase: true, out AgentVerbosity parsedValue))
        {
            return parsedValue;
        }

        throw new ArgumentException($"Value '{value}' for '{optionName}' must be quiet, normal, or verbose.");
    }

    private static bool ParseBool(string? value)
        => bool.TryParse(value, out bool parsedValue) && parsedValue;
}
