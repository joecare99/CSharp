using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Protocol.Models;
using Ollama.Protocol.Tests.TestDoubles;

namespace Ollama.Protocol.Tests;

[TestClass]
public sealed class OllamaProtocolClientTests
{
    [TestMethod]
    public async Task GetTagsAsync_ReturnsModelsFromTagsEndpoint()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            Assert.AreEqual(HttpMethod.Get, request.Method);
            Assert.AreEqual("http://localhost:11434/api/tags", request.RequestUri?.ToString());
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"models\":[{\"name\":\"qwen3.5:4b\",\"model\":\"qwen3.5:4b\"}]}", Encoding.UTF8, "application/json"),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        OllamaTagsResponse response = await client.GetTagsAsync();

        Assert.AreEqual(1, response.Models.Count);
        Assert.AreEqual("qwen3.5:4b", response.Models[0].Name);
    }

    [TestMethod]
    public async Task GenerateStreamingAsync_ReturnsStreamedChunks()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual("http://localhost:11434/api/generate", request.RequestUri?.ToString());
            string body = await request.Content!.ReadAsStringAsync(cancellationToken);
            StringAssert.Contains(body, "\"model\":\"qwen3.5:4b\"");
            StringAssert.Contains(body, "\"prompt\":\"Explain AI.\"");

            string streamContent = "{\"response\":\"Hello\",\"thinking\":\"Think\",\"done\":false}\n"
                + "{\"response\":\" world\",\"thinking\":\" more\",\"done\":true}\n";
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(streamContent))),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));
        OllamaGenerateRequest request = new()
        {
            Model = "qwen3.5:4b",
            Prompt = "Explain AI.",
            Stream = true,
        };
        List<OllamaGenerateResponseChunk> chunks = [];

        await foreach (OllamaGenerateResponseChunk chunk in client.GenerateStreamingAsync(request))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(2, chunks.Count);
        Assert.AreEqual("Hello", chunks[0].Response);
        Assert.AreEqual(" world", chunks[1].Response);
        Assert.IsTrue(chunks[1].Done);
    }

    [TestMethod]
    public async Task ChatStreamingAsync_ReturnsStreamedChatChunks()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual("http://localhost:11434/api/chat", request.RequestUri?.ToString());
            string body = await request.Content!.ReadAsStringAsync(cancellationToken);
            StringAssert.Contains(body, "\"model\":\"qwen3.5:4b\"");
            StringAssert.Contains(body, "\"role\":\"user\"");
            StringAssert.Contains(body, "\"content\":\"Explain AI.\"");

            string streamContent = "{\"message\":{\"role\":\"assistant\",\"content\":\"Hello\"},\"thinking\":\"Think\",\"done\":false}\n"
                + "{\"message\":{\"role\":\"assistant\",\"content\":\" world\"},\"thinking\":\" more\",\"done\":true}\n";
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(streamContent))),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));
        OllamaChatRequest request = new()
        {
            Model = "qwen3.5:4b",
            Messages =
            [
                new OllamaChatMessage
                {
                    Role = "user",
                    Content = "Explain AI.",
                },
            ],
            Stream = true,
        };
        List<OllamaChatResponseChunk> chunks = [];

        await foreach (OllamaChatResponseChunk chunk in client.ChatStreamingAsync(request))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(2, chunks.Count);
        Assert.AreEqual("assistant", chunks[0].Message?.Role);
        Assert.AreEqual("Hello", chunks[0].Message?.Content);
        Assert.AreEqual(" world", chunks[1].Message?.Content);
        Assert.IsTrue(chunks[1].Done);
    }

    [TestMethod]
    public async Task EmbedAsync_ReturnsEmbeddingVectors()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            Assert.AreEqual(HttpMethod.Post, request.Method);
            Assert.AreEqual("http://localhost:11434/api/embed", request.RequestUri?.ToString());
            string body = await request.Content!.ReadAsStringAsync(cancellationToken);
            StringAssert.Contains(body, "\"model\":\"qwen-embed\"");
            StringAssert.Contains(body, "\"input\":[\"Hello\",\"World\"]");
            await Task.CompletedTask;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"embeddings\":[[0.1,0.2],[0.3,0.4]]}", Encoding.UTF8, "application/json"),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));
        OllamaEmbedRequest request = new()
        {
            Model = "qwen-embed",
            Input = ["Hello", "World"],
        };

        OllamaEmbedResponse response = await client.EmbedAsync(request);

        Assert.AreEqual(2, response.Embeddings.Count);
        Assert.AreEqual(2, response.Embeddings[0].Count);
        Assert.AreEqual(0.1d, response.Embeddings[0][0]);
        Assert.AreEqual(0.4d, response.Embeddings[1][1]);
    }

    [TestMethod]
    public void OllamaProtocolClient_Constructor_ThrowsForNullHttpClient()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaProtocolClient(null!, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/"))));

    [TestMethod]
    public void OllamaProtocolClient_Constructor_ThrowsForNullOptions()
    {
        using HttpClient httpClient = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaProtocolClient(httpClient, null!));
    }

    [TestMethod]
    public async Task GetTagsAsync_ReturnsEmptyResponseWhenPayloadIsNull()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            await Task.CompletedTask;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json"),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        OllamaTagsResponse response = await client.GetTagsAsync();

        Assert.AreEqual(0, response.Models.Count);
    }

    [TestMethod]
    public async Task GenerateStreamingAsync_ThrowsForNullRequest()
    {
        using HttpClient httpClient = new(new TestHttpMessageHandler((request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            await foreach (OllamaGenerateResponseChunk _ in client.GenerateStreamingAsync(null!))
            {
            }
        });
    }

    [TestMethod]
    public async Task ChatStreamingAsync_ThrowsForNullRequest()
    {
        using HttpClient httpClient = new(new TestHttpMessageHandler((request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            await foreach (OllamaChatResponseChunk _ in client.ChatStreamingAsync(null!))
            {
            }
        });
    }

    [TestMethod]
    public async Task EmbedAsync_ThrowsForNullRequest()
    {
        using HttpClient httpClient = new(new TestHttpMessageHandler((request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.EmbedAsync(null!));
    }

    [TestMethod]
    public async Task EmbedAsync_ReturnsEmptyResponseWhenPayloadIsNull()
    {
        TestHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            await Task.CompletedTask;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json"),
            };
        });
        using HttpClient httpClient = new(handler);
        OllamaProtocolClient client = new(httpClient, new OllamaProtocolClientOptions(new Uri("http://localhost:11434/")));

        OllamaEmbedResponse response = await client.EmbedAsync(new OllamaEmbedRequest
        {
            Model = "qwen-embed",
            Input = ["Hello"],
        });

        Assert.AreEqual(0, response.Embeddings.Count);
    }
}
