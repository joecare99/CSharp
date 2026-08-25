using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ollama.CodingAgent;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        OllamaAgentCliOptions cliOptions = OllamaAgentCliOptions.Parse(args);
        if (cliOptions.ShowHelp)
        {
            PrintHelp();
            return 0;
        }

        ServiceCollection services = new();
        services.AddOllamaCodingAgent(cliOptions);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        try
        {
            if (cliOptions.PreflightOnly || cliOptions.BaselineSmoke)
            {
                OllamaBaselineService baselineService = serviceProvider.GetRequiredService<OllamaBaselineService>();
                OllamaBaselineCheckResult baselineResult = cliOptions.BaselineSmoke
                    ? await baselineService.RunSmokeAsync(cliOptions.Prompt, CancellationToken.None)
                    : await baselineService.RunPreflightAsync(CancellationToken.None);

                Console.WriteLine($"Endpoint: {cliOptions.Endpoint}");
                Console.WriteLine($"Model: {cliOptions.Model}");
                Console.WriteLine($"Preflight: {(baselineResult.Success ? "PASS" : "FAIL")}");
                Console.WriteLine($"Available models: {string.Join(", ", baselineResult.AvailableModels)}");
                if (!string.IsNullOrWhiteSpace(baselineResult.Response))
                {
                    Console.WriteLine("Baseline response:");
                    Console.WriteLine(baselineResult.Response);
                }

                if (!baselineResult.Success && !string.IsNullOrWhiteSpace(baselineResult.Error))
                {
                    Console.WriteLine($"Error: {baselineResult.Error}");
                }

                return baselineResult.Success ? 0 : 1;
            }

            AgentRunResult result;
            if (cliOptions.DelegateMode)
            {
                CodingTaskDelegationService delegationService = serviceProvider.GetRequiredService<CodingTaskDelegationService>();
                result = await delegationService.RunDelegatedAsync(cliOptions.Prompt, CancellationToken.None);
            }
            else
            {
                AgentRunner runner = serviceProvider.GetRequiredService<AgentRunner>();
                result = await runner.RunAsync(new AgentRunRequest
                {
                    Prompt = cliOptions.Prompt,
                    SystemPrompt = AgentPromptBuilder.BuildDefaultSystemPrompt(),
                });
            }

            if (cliOptions.RuntimeSettings.Verbosity != AgentVerbosity.Quiet)
            {
                Console.WriteLine($"Endpoint: {cliOptions.Endpoint}");
                Console.WriteLine($"Model: {cliOptions.Model}");
                Console.WriteLine($"Delegate mode: {cliOptions.DelegateMode}");
                Console.WriteLine($"Verbosity: {cliOptions.RuntimeSettings.Verbosity}");
                Console.WriteLine($"Iterations: {result.IterationsUsed}/{cliOptions.RuntimeSettings.MaxIterations}");
                Console.WriteLine($"Retry attempts used: {result.RetryAttemptsUsed}/{cliOptions.RuntimeSettings.RetryCount * cliOptions.RuntimeSettings.MaxIterations}");
                Console.WriteLine();
                Console.WriteLine("Agent response:");
            }

            Console.WriteLine(result.FinalResponse);
            if (cliOptions.RuntimeSettings.ShowThinking && result.Thinking.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Model thinking:");
                Console.WriteLine(string.Join(string.Empty, result.Thinking));
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Agent execution failed.");
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Ollama.CodingAgent");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project .\\Ollama.CodingAgent\\Ollama.CodingAgent.csproj -- [options] [prompt]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --endpoint <url>          Ollama endpoint (default: http://localhost:11434/)");
        Console.WriteLine("  --model <name>            Model name (default: qwen2.5-coder:7b)");
        Console.WriteLine("  --timeout-minutes <num>   Step timeout in minutes (default: 12)");
        Console.WriteLine("  --retries <num>           Retries per step (default: 3)");
        Console.WriteLine("  --max-iterations <num>    Hard iteration cap (default: 80)");
        Console.WriteLine("  --verbosity <level>       Output level: quiet, normal, or verbose (default: normal)");
        Console.WriteLine("  --show-thinking           Display model thinking fragments when available");
        Console.WriteLine("  --log-tool-calls          Emit one structured console line per delegated tool call (name, parameters, status, duration, truncated result). Suppressed at --verbosity quiet.");
        Console.WriteLine("  --preflight               Check endpoint reachability and configured model availability");
        Console.WriteLine("  --baseline-smoke          Run preflight plus one bounded chat roundtrip");
        Console.WriteLine("  --prompt <text>           Prompt text (alternative to positional prompt)");
        Console.WriteLine("  --delegate                Enable delegated coding-task mode with safe workspace tools");
        Console.WriteLine("  --workspace-root <path>   Workspace root for delegated tool access (default: current directory)");
        Console.WriteLine("  --session <id>            Session identifier for logs (default: default)");
        Console.WriteLine("  --help                    Show help");
        Console.WriteLine();
        Console.WriteLine("LLM traffic is logged by default to <workspace>\\.agent\\logs\\<session>.jsonl.");
        Console.WriteLine("Credentials are redacted before persistence. The --debug-log switch is planned, but not available yet.");
    }
}
