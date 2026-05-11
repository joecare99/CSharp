using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.Samples.ToolUse;

internal static class Program
{
    private const string DefaultEndpoint = "http://localhost:11434/";
    private const string DefaultModel = "qwen3.5:4b";

    private static async Task<int> Main(string[] args)
    {
        string endpointValue = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? DefaultEndpoint;
        string model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? DefaultModel;
        string prompt = args.Length > 0
            ? string.Join(" ", args)
            : "Use the clock tool to tell me the current host time.";

        using HttpClient httpClient = new()
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri(endpointValue)));
        OllamaChatClient chatClient = client.GetChatClient(model);
        IOllamaTool[] tools =
        [
            new ClockTool(),
        ];
        OllamaToolRegistry registry = new(tools);
        OllamaToolOrchestrator orchestrator = new(registry);
        OllamaToolLoopRunner runner = new(new OllamaToolChatRunner(chatClient), registry, orchestrator);

        try
        {
            Console.WriteLine($"Endpoint: {endpointValue}");
            Console.WriteLine($"Model: {model}");
            Console.WriteLine("Registered tools:");
            foreach (OllamaToolDescriptor descriptor in registry.GetDescriptors())
            {
                Console.WriteLine($"- {descriptor.Name}: {descriptor.Description}");
            }

            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();

            OllamaToolInvocationResult result = await runner.RunAsync(prompt);
            Console.WriteLine($"Tool: {result.ToolName}");
            Console.WriteLine($"Success: {result.Success}");
            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                Console.WriteLine($"Error: {result.Error}");
            }
            Console.WriteLine($"Output: {result.Output}");
            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Tool sample failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }
}

internal sealed class ClockTool : IOllamaTool
{
    public string Name => "clock";

    public string Description => "Returns the current host date and time.";

    public OllamaToolSchema Schema => new()
    {
        Summary = "Accepts an optional plain text hint such as 'now' or 'utc'.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "input",
                Type = "string",
                Description = "Optional time hint.",
                Required = false,
            },
        ],
    };

    public OllamaToolValidationResult Validate(string input) 
        => OllamaToolValidationResult.Success();

    public Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default) 
        => Task.FromResult(new OllamaToolResult
    {
        Output = DateTimeOffset.Now.ToString("O"),
        Success = true,
    });
}
