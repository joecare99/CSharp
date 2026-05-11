using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Models;
using Ollama.Client.Tests.TestDoubles;
using Ollama.Protocol.Models;

namespace Ollama.Client.Tests;

[TestClass]
public sealed class OllamaClientGuardTests
{
    [TestMethod]
    public void OllamaClientOptions_StoresEndpoint()
    {
        Uri endpoint = new("http://localhost:11434/");

        OllamaClientOptions options = new(endpoint);

        Assert.AreEqual(endpoint, options.Endpoint);
    }

    [TestMethod]
    public void OllamaClient_ThrowsForNullHttpClient() => Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaClient(null!, new OllamaClientOptions(new Uri("http://localhost:11434/"))));

    [TestMethod]
    public void OllamaClient_ThrowsForNullOptions()
    {
        using HttpClient httpClient = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaClient(httpClient, null!));
    }

    [TestMethod]
    public void OllamaClient_PublicConstructor_CreatesFeatureClients()
    {
        using HttpClient httpClient = new();
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri("http://localhost:11434/")));

        Assert.IsNotNull(client.GetChatClient("qwen3.5:4b"));
        Assert.IsNotNull(client.GetGenerateClient("qwen3.5:4b"));
        Assert.IsNotNull(client.GetEmbeddingClient("nomic-embed-text"));
    }

    [TestMethod]
    public void GetChatClient_ThrowsForEmptyModel()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaClient client = new(adapter);

        Assert.ThrowsExactly<ArgumentException>(() => client.GetChatClient(string.Empty));
    }

    [TestMethod]
    public void GetGenerateClient_ThrowsForEmptyModel()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetEmptyGenerateChunksAsync(),
        };
        OllamaClient client = new(adapter);

        Assert.ThrowsExactly<ArgumentException>(() => client.GetGenerateClient(string.Empty));
    }

    [TestMethod]
    public void GetEmbeddingClient_ThrowsForEmptyModel()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            EmbedAsyncHandler = (request, cancellationToken) => Task.FromResult(new OllamaEmbedResponse()),
        };
        OllamaClient client = new(adapter);

        Assert.ThrowsExactly<ArgumentException>(() => client.GetEmbeddingClient(string.Empty));
    }

    [TestMethod]
    public async Task CompleteChatAsync_ThrowsForEmptyMessages()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.CompleteChatAsync(new ChatCompletionOptions()));
    }

    [TestMethod]
    public async Task CompleteChatAsync_ThrowsForMissingRole()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.CompleteChatAsync(new ChatCompletionOptions
        {
            Messages =
            [
                new OllamaClientChatMessage
                {
                    Role = string.Empty,
                    Content = "Hello",
                },
            ],
        }));
    }

    [TestMethod]
    public async Task CompleteChatAsync_ThrowsForMissingContent()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.CompleteChatAsync(new ChatCompletionOptions
        {
            Messages =
            [
                new OllamaClientChatMessage
                {
                    Role = "user",
                    Content = string.Empty,
                },
            ],
        }));
    }

    [TestMethod]
    public async Task CompleteChatStreamingAsync_ThrowsForEmptyMessages()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await foreach (OllamaStreamingChatUpdate _ in client.CompleteChatStreamingAsync(new ChatCompletionOptions()))
            {
            }
        });
    }

    [TestMethod]
    public async Task CompleteChatStreamingAsync_ThrowsForMissingRole()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await foreach (OllamaStreamingChatUpdate _ in client.CompleteChatStreamingAsync(new ChatCompletionOptions
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = string.Empty,
                        Content = "Hello",
                    },
                ],
            }))
            {
            }
        });
    }

    [TestMethod]
    public async Task CompleteChatStreamingAsync_ThrowsForMissingContent()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetEmptyChatChunksAsync(),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await foreach (OllamaStreamingChatUpdate _ in client.CompleteChatStreamingAsync(new ChatCompletionOptions
            {
                Messages =
                [
                    new OllamaClientChatMessage
                    {
                        Role = "user",
                        Content = string.Empty,
                    },
                ],
            }))
            {
            }
        });
    }

    [TestMethod]
    public async Task CompleteChatStreamingAsync_StringOverload_ForwardsUpdates()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetSingleChatChunkAsync(request),
        };
        OllamaChatClient client = new(adapter, "qwen3.5:4b");
        List<OllamaStreamingChatUpdate> updates = [];

        await foreach (OllamaStreamingChatUpdate update in client.CompleteChatStreamingAsync("Hello"))
        {
            updates.Add(update);
        }

        Assert.AreEqual(1, updates.Count);
        Assert.AreEqual("Hello", updates[0].Content);
    }

    [TestMethod]
    public async Task GenerateAsync_ThrowsForEmptyPromptInOptions()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetEmptyGenerateChunksAsync(),
        };
        OllamaGenerateClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GenerateAsync(new GenerateOptions()));
    }

    [TestMethod]
    public async Task GenerateStreamingAsync_ThrowsForEmptyPromptInOptions()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetEmptyGenerateChunksAsync(),
        };
        OllamaGenerateClient client = new(adapter, "qwen3.5:4b");

        await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
        {
            await foreach (OllamaStreamingGenerateUpdate _ in client.GenerateStreamingAsync(new GenerateOptions()))
            {
            }
        });
    }

    [TestMethod]
    public async Task GenerateStreamingAsync_StringOverload_ForwardsUpdates()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetSingleGenerateChunkAsync(request),
        };
        OllamaGenerateClient client = new(adapter, "qwen3.5:4b");
        List<OllamaStreamingGenerateUpdate> updates = [];

        await foreach (OllamaStreamingGenerateUpdate update in client.GenerateStreamingAsync("Hello"))
        {
            updates.Add(update);
        }

        Assert.AreEqual(1, updates.Count);
        Assert.AreEqual("Hello", updates[0].Content);
    }

    [TestMethod]
    public async Task GenerateEmbeddingAsync_ThrowsForEmptyInputs()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            EmbedAsyncHandler = (request, cancellationToken) => Task.FromResult(new OllamaEmbedResponse()),
        };
        OllamaEmbeddingClient client = new(adapter, "nomic-embed-text");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GenerateEmbeddingAsync(new EmbeddingOptions()));
    }

    [TestMethod]
    public async Task GenerateEmbeddingAsync_ThrowsForWhitespaceInput()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            EmbedAsyncHandler = (request, cancellationToken) => Task.FromResult(new OllamaEmbedResponse()),
        };
        OllamaEmbeddingClient client = new(adapter, "nomic-embed-text");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.GenerateEmbeddingAsync(new EmbeddingOptions
        {
            Input = [" "],
        }));
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Chat client rejects null adapter")]
    [DataRow("", DisplayName = "Chat client rejects empty model")]
    [DataRow(" ", DisplayName = "Chat client rejects whitespace model")]
    public void OllamaChatClient_Constructor_ValidatesArguments(string? model)
    {
        if (model is null)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaChatClient(protocolAdapter: null!, "qwen3.5:4b"));
            return;
        }

        TestOllamaProtocolAdapter adapter = new();
        Assert.ThrowsExactly<ArgumentException>(() => new OllamaChatClient(adapter, model));
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Generate client rejects null adapter")]
    [DataRow("", DisplayName = "Generate client rejects empty model")]
    [DataRow(" ", DisplayName = "Generate client rejects whitespace model")]
    public void OllamaGenerateClient_Constructor_ValidatesArguments(string? model)
    {
        if (model is null)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaGenerateClient(protocolAdapter: null!, "qwen3.5:4b"));
            return;
        }

        TestOllamaProtocolAdapter adapter = new();
        Assert.ThrowsExactly<ArgumentException>(() => new OllamaGenerateClient(adapter, model));
    }

    [TestMethod]
    [DataRow(null, DisplayName = "Embedding client rejects null adapter")]
    [DataRow("", DisplayName = "Embedding client rejects empty model")]
    [DataRow(" ", DisplayName = "Embedding client rejects whitespace model")]
    public void OllamaEmbeddingClient_Constructor_ValidatesArguments(string? model)
    {
        if (model is null)
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaEmbeddingClient(protocolAdapter: null!, "nomic-embed-text"));
            return;
        }

        TestOllamaProtocolAdapter adapter = new();
        Assert.ThrowsExactly<ArgumentException>(() => new OllamaEmbeddingClient(adapter, model));
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> GetEmptyChatChunksAsync()
    {
        await Task.Yield();
        yield break;
    }

    private static async IAsyncEnumerable<OllamaGenerateResponseChunk> GetEmptyGenerateChunksAsync()
    {
        await Task.Yield();
        yield break;
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> GetSingleChatChunkAsync(OllamaChatRequest request)
    {
        Assert.AreEqual("Hello", request.Messages[0].Content);

        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = "Hello",
            },
            Done = true,
        };

        await Task.Yield();
    }

    private static async IAsyncEnumerable<OllamaGenerateResponseChunk> GetSingleGenerateChunkAsync(OllamaGenerateRequest request)
    {
        Assert.AreEqual("Hello", request.Prompt);

        yield return new OllamaGenerateResponseChunk
        {
            Response = "Hello",
            Done = true,
        };

        await Task.Yield();
    }
}
