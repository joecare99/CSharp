using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Models;
using OpenAI.CodingAgent;

namespace PeripheralProduction.Tests;

[TestClass]
public sealed class OpenAICompatibleChatModelClientTests
{
    [TestMethod]
    public void Options_ValidateArgumentsAndExposeValues()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new OpenAICompatibleClientOptions(null!, "model"));
        Assert.ThrowsExactly<ArgumentException>(() => new OpenAICompatibleClientOptions(new Uri("https://example.test/"), " "));

        OpenAICompatibleClientOptions options = new(new Uri("https://example.test/v1/"), "model", "token");

        Assert.AreEqual("https://example.test/v1/", options.Endpoint.AbsoluteUri);
        Assert.AreEqual("model", options.Model);
        Assert.AreEqual("token", options.ApiKey);
    }

    [TestMethod]
    public void ConstructorAndCapabilities_ValidateDependenciesAndExposeContract()
    {
        OpenAICompatibleClientOptions options = new(new Uri("https://example.test/"), "model");
        using HttpClient httpClient = new(new DeterministicHttpMessageHandler((_, _) => throw new InvalidOperationException()));

        Assert.ThrowsExactly<ArgumentNullException>(() => new OpenAICompatibleChatModelClient(null!, options));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OpenAICompatibleChatModelClient(httpClient, null!));

        AgentProviderCapabilities capabilities = new OpenAICompatibleChatModelClient(httpClient, options).Capabilities;
        Assert.AreEqual("openai-compatible", capabilities.ProviderName);
        Assert.AreEqual("model", capabilities.Model);
        Assert.IsFalse(capabilities.SupportsStreaming);
        Assert.IsFalse(capabilities.SupportsToolCalls);
        Assert.IsFalse(capabilities.SupportsThinking);
    }

    [TestMethod]
    public async Task CompleteAsync_UsesVersionedEndpointAndBearerToken()
    {
        HttpRequestMessage? capturedRequest = null;
        using HttpClient httpClient = new(new DeterministicHttpMessageHandler(async (request, cancellationToken) =>
        {
            capturedRequest = request;
            string body = await request.Content!.ReadAsStringAsync(cancellationToken);
            StringAssert.Contains(body, "\"model\":\"model\"");
            StringAssert.Contains(body, "\"role\":\"user\"");
            StringAssert.Contains(body, "\"content\":\"hello\"");
            return JsonResponse("{\"choices\":[{\"message\":{\"content\":\"answer\"}}]}");
        }));
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test/v1/"), "model", "token"));

        string result = await client.CompleteAsync([new AgentMessage("user", "hello")]);

        Assert.AreEqual("answer", result);
        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual("/v1/chat/completions", capturedRequest.RequestUri?.AbsolutePath);
        Assert.AreEqual("Bearer", capturedRequest.Headers.Authorization?.Scheme);
        Assert.AreEqual("token", capturedRequest.Headers.Authorization?.Parameter);
    }

    [TestMethod]
    public async Task CompleteDetailedAsync_UsesUnversionedBaseAndSupportsMissingContent()
    {
        using HttpClient httpClient = new(new DeterministicHttpMessageHandler((request, _) =>
        {
            Assert.AreEqual("/v1/chat/completions", request.RequestUri?.AbsolutePath);
            Assert.IsNull(request.Headers.Authorization);
            return Task.FromResult(JsonResponse("{\"choices\":[{\"message\":{}}]}"));
        }));
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test/api"), "model"));

        AgentCompletion completion = await client.CompleteDetailedAsync([new AgentMessage("user", "hello")]);

        Assert.AreEqual(string.Empty, completion.Content);
    }

    [TestMethod]
    public async Task CompleteDetailedAsync_NormalizesNullContentToAnEmptyString()
    {
        using HttpClient httpClient = new(new DeterministicHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("{\"choices\":[{\"message\":{\"content\":null}}]}"))));
        OpenAICompatibleChatModelClient client = new(httpClient, new OpenAICompatibleClientOptions(new Uri("https://example.test/"), "model"));

        AgentCompletion completion = await client.CompleteDetailedAsync([new AgentMessage("user", "hello")]);

        Assert.AreEqual(string.Empty, completion.Content);
    }

    [TestMethod]
    public async Task CompleteDetailedAsync_ValidatesMessagesAndResponseShape()
    {
        using HttpClient httpClient = new(new DeterministicHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("{}"))));
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test/"), "model"));

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => client.CompleteDetailedAsync(null!));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => client.CompleteDetailedAsync(Array.Empty<AgentMessage>()));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.CompleteDetailedAsync([new AgentMessage("user", "hello")]));
    }

    [TestMethod]
    public async Task CompleteDetailedAsync_RejectsNonArrayEmptyAndFailedResponses()
    {
        foreach (string response in new[] { "{\"choices\":{}}", "{\"choices\":[]}" })
        {
            using HttpClient httpClient = new(new DeterministicHttpMessageHandler((_, _) => Task.FromResult(JsonResponse(response))));
            OpenAICompatibleChatModelClient client = new(httpClient, new OpenAICompatibleClientOptions(new Uri("https://example.test/"), "model"));

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.CompleteDetailedAsync([new AgentMessage("user", "hello")]));
        }

        using HttpClient failedHttpClient = new(new DeterministicHttpMessageHandler((_, _) => Task.FromResult(JsonResponse("failed", HttpStatusCode.BadGateway))));
        OpenAICompatibleChatModelClient failedClient = new(failedHttpClient, new OpenAICompatibleClientOptions(new Uri("https://example.test/"), "model"));
        await Assert.ThrowsExactlyAsync<HttpRequestException>(() => failedClient.CompleteDetailedAsync([new AgentMessage("user", "hello")]));
    }

    private static HttpResponseMessage JsonResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json"),
        };
}
