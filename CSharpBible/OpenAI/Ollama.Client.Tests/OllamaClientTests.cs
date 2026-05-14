using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Models;
using Ollama.Protocol;
using Ollama.Client.Tests.TestDoubles;
using Ollama.Protocol.Models;

namespace Ollama.Client.Tests;

[TestClass]
public sealed class OllamaClientTests
{
    [TestMethod]
    public async Task ChatClient_CompleteChatAsync_AggregatesContentAndThinking()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetChatChunksAsync(request),
        };
        OllamaClient client = new(adapter);

        OllamaChatCompletion completion = await client.GetChatClient("qwen3.5:4b").CompleteChatAsync("Explain AI.");

        Assert.AreEqual("Hello world", completion.Content);
        CollectionAssert.AreEqual(new[] { "Think", " more" }, (System.Collections.ICollection)completion.Thinking);
    }

    [TestMethod]
    public async Task ChatClient_CompleteChatAsync_WithOptions_MapsMessages()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) => GetOptionBasedChatChunksAsync(request),
        };
        OllamaClient client = new(adapter);

        OllamaChatCompletion completion = await client.GetChatClient("qwen3.5:4b").CompleteChatAsync(new ChatCompletionOptions
        {
            Messages =
            [
                new OllamaClientChatMessage
                {
                    Role = "system",
                    Content = "Be concise.",
                },
                new OllamaClientChatMessage
                {
                    Role = "user",
                    Content = "Explain AI.",
                },
            ],
        });

        Assert.AreEqual("Option result", completion.Content);
        CollectionAssert.AreEqual(new[] { "Plan" }, (System.Collections.ICollection)completion.Thinking);
    }

    [TestMethod]
    public async Task ChatClient_CompleteChatAsync_WithOptions_MapsImages()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            ChatStreamingAsyncHandler = (request, cancellationToken) =>
            {
                Assert.AreEqual("qwen3.5:4b", request.Model);
                Assert.AreEqual(1, request.Messages.Count);
                Assert.AreEqual("user", request.Messages[0].Role);
                Assert.AreEqual("Summarize", request.Messages[0].Content);
                CollectionAssert.AreEqual(new[] { "image1", "image2" }, (System.Collections.ICollection)request.Messages[0].Images!);

                return GetSingleChatChunkAsync(request);
            },
        };
        OllamaClient client = new(adapter);

        OllamaChatCompletion completion = await client.GetChatClient("qwen3.5:4b").CompleteChatAsync(new ChatCompletionOptions
        {
            Messages =
            [
                new OllamaClientChatMessage
                {
                    Role = "user",
                    Content = "Summarize",
                    Images = ["image1", "image2"],
                },
            ],
        });

        Assert.AreEqual("Hello", completion.Content);
    }

    [TestMethod]
    public async Task GenerateClient_GenerateAsync_AggregatesContentAndThinking()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetGenerateChunksAsync(request),
        };
        OllamaClient client = new(adapter);

        OllamaGeneratedText result = await client.GetGenerateClient("qwen3.5:4b").GenerateAsync("Explain AI.");

        Assert.AreEqual("Hello world", result.Content);
        CollectionAssert.AreEqual(new[] { "Think", " more" }, (System.Collections.ICollection)result.Thinking);
    }

    [TestMethod]
    public async Task GenerateClient_GenerateAsync_WithOptions_MapsPrompt()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            GenerateStreamingAsyncHandler = (request, cancellationToken) => GetGenerateOptionChunksAsync(request),
        };
        OllamaClient client = new(adapter);

        OllamaGeneratedText result = await client.GetGenerateClient("qwen3.5:4b").GenerateAsync(new GenerateOptions
        {
            Prompt = "Summarize AI.",
        });

        Assert.AreEqual("Prompt result", result.Content);
        CollectionAssert.AreEqual(new[] { "Plan" }, (System.Collections.ICollection)result.Thinking);
    }

    [TestMethod]
    public async Task EmbeddingClient_GenerateEmbeddingAsync_ReturnsEmbeddings()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            EmbedAsyncHandler = (request, cancellationToken) => Task.FromResult(new OllamaEmbedResponse
            {
                Embeddings = new List<IReadOnlyList<double>>
                {
                    new List<double> { 0.1d, 0.2d },
                },
            }),
        };
        OllamaClient client = new(adapter);

        OllamaEmbeddingResult result = await client.GetEmbeddingClient("nomic-embed-text").GenerateEmbeddingAsync("hello");

        Assert.AreEqual(1, result.Embeddings.Count);
        Assert.AreEqual(2, result.Embeddings[0].Count);
        Assert.AreEqual(0.1d, result.Embeddings[0][0]);
    }

    [TestMethod]
    public async Task EmbeddingClient_GenerateEmbeddingAsync_WithOptions_MapsInputs()
    {
        TestOllamaProtocolAdapter adapter = new()
        {
            EmbedAsyncHandler = (request, cancellationToken) =>
            {
                CollectionAssert.AreEqual(new[] { "hello", "world" }, (System.Collections.ICollection)request.Input);

                return Task.FromResult(new OllamaEmbedResponse
                {
                    Embeddings = new List<IReadOnlyList<double>>
                    {
                        new List<double> { 0.1d },
                        new List<double> { 0.2d },
                    },
                });
            },
        };
        OllamaClient client = new(adapter);

        OllamaEmbeddingResult result = await client.GetEmbeddingClient("nomic-embed-text").GenerateEmbeddingAsync(new EmbeddingOptions
        {
            Input = ["hello", "world"],
        });

        Assert.AreEqual(2, result.Embeddings.Count);
        Assert.AreEqual(0.2d, result.Embeddings[1][0]);
    }

    [TestMethod]
    public async Task OllamaProtocolAdapter_ForwardsAllCalls()
    {
        StubHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            string path = request.RequestUri?.AbsolutePath ?? string.Empty;

            if (path == "/api/tags")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"models\":[{\"name\":\"qwen3.5:4b\"}]}", System.Text.Encoding.UTF8, "application/json"),
                };
            }

            if (path == "/api/generate")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("{\"response\":\"gen\",\"done\":true}\n"))),
                };
            }

            if (path == "/api/chat")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("{\"message\":{\"role\":\"assistant\",\"content\":\"chat\"},\"done\":true}\n"))),
                };
            }

            if (path == "/api/embed")
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"embeddings\":[[1.0,2.0]]}", System.Text.Encoding.UTF8, "application/json"),
                };
            }

            await Task.CompletedTask;
            throw new AssertFailedException($"Unexpected request path: {path}");
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient protocolClient = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));
        OllamaProtocolAdapter adapter = new(protocolClient);

        OllamaTagsResponse tags = await adapter.GetTagsAsync();
        Assert.AreEqual(1, tags.Models.Count);

        List<OllamaGenerateResponseChunk> generated = [];
        await foreach (OllamaGenerateResponseChunk chunk in adapter.GenerateStreamingAsync(new OllamaGenerateRequest
        {
            Model = "qwen3.5:4b",
            Prompt = "hello",
            Stream = true,
        }))
        {
            generated.Add(chunk);
        }

        List<OllamaChatResponseChunk> chatted = [];
        await foreach (OllamaChatResponseChunk chunk in adapter.ChatStreamingAsync(new OllamaChatRequest
        {
            Model = "qwen3.5:4b",
            Messages =
            [
                new OllamaChatMessage
                {
                    Role = "user",
                    Content = "hello",
                },
            ],
            Stream = true,
        }))
        {
            chatted.Add(chunk);
        }

        OllamaEmbedResponse embeddings = await adapter.EmbedAsync(new OllamaEmbedRequest
        {
            Model = "nomic-embed-text",
            Input = ["hello"],
        });

        Assert.AreEqual("gen", generated[0].Response);
        Assert.AreEqual("chat", chatted[0].Message?.Content);
        Assert.AreEqual(2, embeddings.Embeddings[0].Count);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly global::System.Func<HttpRequestMessage, global::System.Threading.CancellationToken, Task<HttpResponseMessage>> _handler;

        public StubHttpMessageHandler(global::System.Func<HttpRequestMessage, global::System.Threading.CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            => _handler(request, cancellationToken);
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> GetChatChunksAsync(OllamaChatRequest request)
    {
        Assert.AreEqual("qwen3.5:4b", request.Model);
        Assert.AreEqual(1, request.Messages.Count);
        Assert.AreEqual("user", request.Messages[0].Role);
        Assert.AreEqual("Explain AI.", request.Messages[0].Content);

        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = "Hello",
            },
            Thinking = "Think",
            Done = false,
        };

        await Task.Yield();

        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = " world",
            },
            Thinking = " more",
            Done = true,
        };
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> GetOptionBasedChatChunksAsync(OllamaChatRequest request)
    {
        Assert.AreEqual("qwen3.5:4b", request.Model);
        Assert.AreEqual(2, request.Messages.Count);
        Assert.AreEqual("system", request.Messages[0].Role);
        Assert.AreEqual("Be concise.", request.Messages[0].Content);
        Assert.AreEqual("user", request.Messages[1].Role);
        Assert.AreEqual("Explain AI.", request.Messages[1].Content);

        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = "Option result",
            },
            Thinking = "Plan",
            Done = true,
        };

        await Task.Yield();
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> GetSingleChatChunkAsync(OllamaChatRequest request)
    {
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

    private static async IAsyncEnumerable<OllamaGenerateResponseChunk> GetGenerateChunksAsync(OllamaGenerateRequest request)
    {
        Assert.AreEqual("qwen3.5:4b", request.Model);
        Assert.AreEqual("Explain AI.", request.Prompt);
        Assert.IsTrue(request.Stream);

        yield return new OllamaGenerateResponseChunk
        {
            Response = "Hello",
            Thinking = "Think",
            Done = false,
        };

        await Task.Yield();

        yield return new OllamaGenerateResponseChunk
        {
            Response = " world",
            Thinking = " more",
            Done = true,
        };
    }

    private static async IAsyncEnumerable<OllamaGenerateResponseChunk> GetGenerateOptionChunksAsync(OllamaGenerateRequest request)
    {
        Assert.AreEqual("qwen3.5:4b", request.Model);
        Assert.AreEqual("Summarize AI.", request.Prompt);
        Assert.IsTrue(request.Stream);

        yield return new OllamaGenerateResponseChunk
        {
            Response = "Prompt result",
            Thinking = "Plan",
            Done = true,
        };

        await Task.Yield();
    }
}
