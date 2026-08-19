using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Protocol.Models;
using Ollama.Protocol.Services;

namespace Ollama.Client.Tests;

[TestClass]
public sealed class OllamaClientCoverageCompletionTests
{
    [TestMethod]
    public async Task GetTagsAsync_ForwardsTheResponseAndCancellationToken()
    {
        IOllamaProtocolAdapter adapter = Substitute.For<IOllamaProtocolAdapter>();
        OllamaTagsResponse expected = new();
        adapter.GetTagsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(expected));
        using CancellationTokenSource cancellationTokenSource = new();
        OllamaClient client = new(adapter);

        OllamaTagsResponse actual = await client.GetTagsAsync(cancellationTokenSource.Token);

        Assert.AreSame(expected, actual);
        _ = adapter.Received(1).GetTagsAsync(cancellationTokenSource.Token);
    }

    [TestMethod]
    public void Constructors_RejectNullDependencies()
    {
        using System.Net.Http.HttpClient httpClient = new();
        OllamaProtocolClient protocolClient = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaClient((IOllamaProtocolAdapter)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaClientOptions(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaProtocolAdapter(null!));
        Assert.IsNotNull(new OllamaProtocolAdapter(protocolClient));
    }

    [TestMethod]
    public async Task StreamingClients_SupportDistinctMethodAndEnumeratorCancellationTokens()
    {
        IOllamaProtocolAdapter adapter = Substitute.For<IOllamaProtocolAdapter>();
        adapter.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(ChatChunksAsync());
        adapter.GenerateStreamingAsync(Arg.Any<OllamaGenerateRequest>(), Arg.Any<CancellationToken>())
            .Returns(GenerateChunksAsync());
        OllamaClient client = new(adapter);
        using CancellationTokenSource methodCancellationTokenSource = new();
        using CancellationTokenSource enumeratorCancellationTokenSource = new();

        OllamaChatClient chatClient = client.GetChatClient("chat-model");
        List<OllamaStreamingChatUpdate> chatUpdates = [];
        await foreach (OllamaStreamingChatUpdate update in chatClient.CompleteChatStreamingAsync(
            new ChatCompletionOptions
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = "hello",
                    },
                ],
            },
            methodCancellationTokenSource.Token).WithCancellation(enumeratorCancellationTokenSource.Token))
        {
            chatUpdates.Add(update);
        }

        await foreach (OllamaStreamingChatUpdate _ in chatClient.CompleteChatStreamingAsync(
            "hello",
            methodCancellationTokenSource.Token).WithCancellation(enumeratorCancellationTokenSource.Token))
        {
        }

        OllamaGenerateClient generateClient = client.GetGenerateClient("generate-model");
        await foreach (OllamaStreamingGenerateUpdate _ in generateClient.GenerateStreamingAsync(
            new GenerateOptions { Prompt = "hello" },
            methodCancellationTokenSource.Token).WithCancellation(enumeratorCancellationTokenSource.Token))
        {
        }

        await foreach (OllamaStreamingGenerateUpdate _ in generateClient.GenerateStreamingAsync(
            "hello",
            methodCancellationTokenSource.Token).WithCancellation(enumeratorCancellationTokenSource.Token))
        {
        }

        Assert.AreEqual(1, chatUpdates.Count);
        Assert.AreEqual("response", chatUpdates[0].Content);
    }

    [TestMethod]
    public async Task ChatClient_ModelAndNullImages_ArePreservedWhenStreaming()
    {
        IOllamaProtocolAdapter adapter = Substitute.For<IOllamaProtocolAdapter>();
        adapter.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(ChatChunksAsync());
        OllamaChatClient client = new(adapter, "chat-model");

        await foreach (OllamaStreamingChatUpdate _ in client.CompleteChatStreamingAsync(new ChatCompletionOptions
        {
            Messages =
            [
                new OllamaClientChatMessage
                {
                    Role = "user",
                    Content = "hello",
                },
            ],
        }))
        {
        }

        Assert.AreEqual("chat-model", client.Model);
        OllamaChatRequest request = adapter.ReceivedCalls()
            .Select(call => call.GetArguments()[0])
            .OfType<OllamaChatRequest>()
            .Single();
        Assert.IsNull(request.Messages[0].Images);
    }

    [TestMethod]
    public async Task ChatClient_StreamsChunksWithoutMessages()
    {
        IOllamaProtocolAdapter adapter = Substitute.For<IOllamaProtocolAdapter>();
        adapter.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(ChunksWithoutMessagesAsync());
        OllamaChatClient client = new(adapter, "chat-model");

        await foreach (OllamaStreamingChatUpdate update in client.CompleteChatStreamingAsync("hello"))
        {
            Assert.IsNull(update.Content);
        }
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> ChatChunksAsync()
    {
        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = "response",
            },
            Done = true,
        };

        await Task.Yield();
    }

    private static async IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateChunksAsync()
    {
        yield return new OllamaGenerateResponseChunk
        {
            Response = "response",
            Done = true,
        };

        await Task.Yield();
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> ChunksWithoutMessagesAsync()
    {
        yield return new OllamaChatResponseChunk
        {
            Done = true,
        };

        await Task.Yield();
    }
}
