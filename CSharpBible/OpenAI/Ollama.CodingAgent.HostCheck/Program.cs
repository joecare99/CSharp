using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent;
using Ollama.Client;
using Ollama.Tools;

namespace Ollama.CodingAgent.HostCheck;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "qwen2.5-coder:7b";

    private static readonly IReadOnlyList<string> DefaultScenarios =
    [
        "Read this task and summarize the intended C# change in 3 concise bullet points: add retry-aware agent runtime defaults.",
        "Provide a precise one-file refactoring strategy for a C# class with duplicated validation logic.",
        "Suggest a minimal build-and-test command sequence for a .NET solution with one changed project.",
    ];

    private static async Task<int> Main(string[] args)
    {
        string endpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;
        string model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? DefaultModel;
        bool singlePromptMode = false;
        string? singlePrompt = null;
        bool delegateMode = false;
        string workspaceRoot = Environment.CurrentDirectory;

        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i];
            switch (argument)
            {
                case "--endpoint":
                    endpoint = ReadNextValue(args, ref i, "--endpoint");
                    break;
                case "--model":
                    model = ReadNextValue(args, ref i, "--model");
                    break;
                case "--prompt":
                    singlePromptMode = true;
                    singlePrompt = ReadNextValue(args, ref i, "--prompt");
                    break;
                case "--delegate":
                    delegateMode = true;
                    break;
                case "--workspace-root":
                    workspaceRoot = ReadNextValue(args, ref i, "--workspace-root");
                    break;
                default:
                    throw new ArgumentException($"Unknown option '{argument}'.");
            }
        }

        IReadOnlyList<string> scenarios = singlePromptMode && !string.IsNullOrWhiteSpace(singlePrompt)
            ? [singlePrompt]
            : DefaultScenarios;

        using HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OllamaClient ollamaClient = new(httpClient, new OllamaClientOptions(new Uri(endpoint)));
        OllamaChatModelClient modelClient = new(ollamaClient.GetChatClient(model));
        AgentRunner runner = new(modelClient, new OllamaAgentRuntimeSettings(TimeSpan.FromMinutes(12), retryCount: 3, maxIterations: 80));
        WorkspacePathPolicy pathPolicy = new(workspaceRoot);
        CodingDelegationToolRegistryFactory registryFactory = new(pathPolicy);
        IOllamaToolRegistry toolRegistry = registryFactory.CreateRegistry();
        OllamaToolOrchestrator toolOrchestrator = new(toolRegistry);
        OllamaToolChatRunnerAdapter toolChatRunnerAdapter = new(ollamaClient.GetChatClient(model));
        CodingTaskDelegationService delegationService = new(modelClient, toolChatRunnerAdapter, toolRegistry, toolOrchestrator, new OllamaAgentRuntimeSettings(TimeSpan.FromMinutes(12), 3, 80));

        Console.WriteLine($"Endpoint: {endpoint}");
        Console.WriteLine($"Model: {model}");
        Console.WriteLine($"Delegate mode: {delegateMode}");
        Console.WriteLine($"Scenarios: {scenarios.Count}");
        Console.WriteLine();

        int scenarioNumber = 0;
        foreach (string scenario in scenarios)
        {
            scenarioNumber++;
            Console.WriteLine($"=== Scenario {scenarioNumber} ===");
            Console.WriteLine(scenario);
            Console.WriteLine();

            AgentRunResult result = delegateMode
                ? await delegationService.RunDelegatedAsync(scenario, CancellationToken.None)
                : await runner.RunAsync(new AgentRunRequest
                {
                    Prompt = scenario,
                    SystemPrompt = AgentPromptBuilder.BuildDefaultSystemPrompt(),
                }, CancellationToken.None);

            Console.WriteLine($"Iterations: {result.IterationsUsed} | Retries used: {result.RetryAttemptsUsed}");
            Console.WriteLine(result.FinalResponse);
            Console.WriteLine();
        }

        return 0;
    }

    private static string ReadNextValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"Option '{optionName}' requires a value.");
        }

        index++;
        return args[index];
    }
}
