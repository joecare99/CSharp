using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ollama.Client;
using Ollama.Tools;

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
        services.AddSingleton(cliOptions.RuntimeSettings);
        services.AddSingleton(cliOptions);
        services.AddSingleton(new OllamaClientOptions(new Uri(cliOptions.Endpoint)));
        services.AddSingleton(provider =>
        {
            HttpClient httpClient = new()
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };
            return httpClient;
        });
        services.AddSingleton(provider =>
        {
            HttpClient httpClient = provider.GetRequiredService<HttpClient>();
            OllamaClientOptions options = provider.GetRequiredService<OllamaClientOptions>();
            return new OllamaClient(httpClient, options);
        });
        services.AddSingleton(provider =>
        {
            OllamaClient client = provider.GetRequiredService<OllamaClient>();
            return client.GetChatClient(cliOptions.Model);
        });
        services.AddSingleton<IAgentModelClient, OllamaChatModelClient>();
        services.AddSingleton<AgentRunner>();
        services.AddSingleton<WorkspacePathPolicy>(provider => new WorkspacePathPolicy(cliOptions.WorkspaceRoot));
        services.AddSingleton<CodingDelegationToolRegistryFactory>();
        services.AddSingleton<IOllamaToolRegistry>(provider =>
            provider.GetRequiredService<CodingDelegationToolRegistryFactory>().CreateRegistry());
        services.AddSingleton<OllamaToolOrchestrator>();
        services.AddSingleton<OllamaToolChatRunnerAdapter>();
        services.AddSingleton<CodingTaskDelegationService>();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        try
        {
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

            Console.WriteLine($"Endpoint: {cliOptions.Endpoint}");
            Console.WriteLine($"Model: {cliOptions.Model}");
            Console.WriteLine($"Delegate mode: {cliOptions.DelegateMode}");
            Console.WriteLine($"Iterations: {result.IterationsUsed}/{cliOptions.RuntimeSettings.MaxIterations}");
            Console.WriteLine($"Retry attempts used: {result.RetryAttemptsUsed}/{cliOptions.RuntimeSettings.RetryCount * cliOptions.RuntimeSettings.MaxIterations}");
            Console.WriteLine();
            Console.WriteLine("Agent response:");
            Console.WriteLine(result.FinalResponse);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Agent execution failed.");
            Console.WriteLine(ex.Message);
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
        Console.WriteLine("  --prompt <text>           Prompt text (alternative to positional prompt)");
        Console.WriteLine("  --delegate                Enable delegated coding-task mode with safe workspace tools");
        Console.WriteLine("  --workspace-root <path>   Workspace root for delegated tool access (default: current directory)");
        Console.WriteLine("  --help                    Show help");
    }
}
