using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Models;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Tools;

namespace Ollama.CodingAgent.HostCheck;

internal static class Program
{
    private static readonly IReadOnlyList<string> DefaultScenarios =
    [
        "Read this task and summarize the intended C# change in 3 concise bullet points: add retry-aware agent runtime defaults.",
        "Provide a precise one-file refactoring strategy for a C# class with duplicated validation logic.",
        "Suggest a minimal build-and-test command sequence for a .NET solution with one changed project.",
    ];

    private static async Task<int> Main(string[] args)
    {
        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse(args);
        bool singlePromptMode = HasOption(args, "--prompt", "-p");
        string endpoint = options.Endpoint;
        string model = options.Model;
        string? singlePrompt = options.Prompt;
        bool delegateMode = options.DelegateMode;
        string workspaceRoot = options.WorkspaceRoot;

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

    private static bool HasOption(IReadOnlyList<string> args, params string[] optionNames)
    {
        foreach (string argument in args)
        {
            foreach (string optionName in optionNames)
            {
                if (string.Equals(argument, optionName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
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
