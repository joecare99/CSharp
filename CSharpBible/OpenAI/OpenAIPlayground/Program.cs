using System;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace OpenAIPlayground;

internal static class Program
{
    private const string ApiKeyEnvironmentVariable = "OPENAI_API_KEY";
    private const string DefaultModel = "gpt-4o-mini";

    internal static Func<string, IOpenAIChatCompletionClient> ChatClientFactory { get; set; } = CreateChatClient;

    internal static async Task<int> Main(string[] args)
    {
        string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine($"Set the environment variable '{ApiKeyEnvironmentVariable}' before starting the sample.");
            Console.WriteLine("PowerShell example: $env:OPENAI_API_KEY = \"your-api-key\"");
            return 1;
        }

        string prompt = args.Length > 0
            ? string.Join(" ", args)
            : "Explain in simple words what the OpenAI .NET SDK does.";

        try
        {
            IOpenAIChatCompletionClient client = ChatClientFactory(apiKey);
            ChatCompletion completion = await client.CompleteChatAsync(prompt);

            Console.WriteLine($"Model: {DefaultModel}");
            Console.WriteLine();
            Console.WriteLine("Prompt:");
            Console.WriteLine(prompt);
            Console.WriteLine();
            Console.WriteLine("Answer:");

            foreach (ChatMessageContentPart contentPart in completion.Content)
            {
                if (!string.IsNullOrWhiteSpace(contentPart.Text))
                {
                    Console.Write(contentPart.Text);
                }
            }

            Console.WriteLine();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("The request failed.");
            Console.WriteLine(ex.Message);
            return 2;
        }
    }

    internal static IOpenAIChatCompletionClient CreateChatClient(string apiKey)
        => new OpenAIChatCompletionClient(new ChatClient(model: DefaultModel, apiKey: apiKey));
}

internal interface IOpenAIChatCompletionClient
{
    Task<ChatCompletion> CompleteChatAsync(string prompt);
}

internal sealed class OpenAIChatCompletionClient : IOpenAIChatCompletionClient
{
    private readonly ChatClient _chatClient;

    public OpenAIChatCompletionClient(ChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    public async Task<ChatCompletion> CompleteChatAsync(string prompt)
        => await _chatClient.CompleteChatAsync(prompt);
}
