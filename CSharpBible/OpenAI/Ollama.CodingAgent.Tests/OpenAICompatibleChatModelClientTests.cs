using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent;
using OpenAI.CodingAgent;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class OpenAICompatibleChatModelClientTests
{
    [TestMethod]
    public async Task CompleteDetailedAsync_NormalizesResponseAndRequest()
    {
        RecordingHandler handler = new("""{"choices":[{"message":{"role":"assistant","content":"hello"}}]}""");
        using HttpClient httpClient = new(handler);
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test/v1/"), "test-model", "secret"));

        AgentCompletion completion = await client.CompleteDetailedAsync(
            [new AgentMessage("user", "say hello")]);

        Assert.AreEqual("hello", completion.Content);
        Assert.AreEqual(HttpMethod.Post, handler.Method);
        Assert.AreEqual("https://example.test/v1/chat/completions", handler.RequestUri?.ToString());
        StringAssert.Contains(handler.RequestBody, "\"role\":\"user\"");
        StringAssert.Contains(handler.RequestBody, "\"content\":\"say hello\"");
        Assert.AreEqual("Bearer secret", handler.Authorization);
        Assert.AreEqual("openai-compatible", client.Capabilities.ProviderName);
        Assert.IsFalse(client.Capabilities.SupportsThinking);
    }

    [TestMethod]
    public async Task CompleteDetailedAsync_RejectsResponseWithoutChoices()
    {
        RecordingHandler handler = new("""{"choices":[]}""");
        using HttpClient httpClient = new(handler);
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test"), "test-model"));

        try
        {
            await client.CompleteDetailedAsync([new AgentMessage("user", "hello")]);
            Assert.Fail("An invalid response must be rejected.");
        }
        catch (InvalidOperationException)
        {
            // Expected normalization failure.
        }
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _response;

        public RecordingHandler(string response)
        {
            _response = response;
        }

        public HttpMethod? Method { get; private set; }
        public Uri? RequestUri { get; private set; }
        public string RequestBody { get; private set; } = string.Empty;
        public string? Authorization { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            RequestUri = request.RequestUri;
            RequestBody = await request.Content!.ReadAsStringAsync(cancellationToken);
            Authorization = request.Headers.Authorization?.ToString();
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_response),
            };
        }
    }
}
