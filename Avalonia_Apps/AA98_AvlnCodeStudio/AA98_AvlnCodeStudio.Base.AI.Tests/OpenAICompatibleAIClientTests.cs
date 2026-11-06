using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Tests;

[TestClass]
public sealed class OpenAICompatibleAIClientTests
{
    [TestMethod]
    public async Task CompleteAsync_MapsMessagesToolsAndToolCalls()
    {
        string? requestBody = null;
        string? authorization = null;
        using HttpClient httpClient = new(new StubHandler(async request =>
        {
            requestBody = await request.Content!.ReadAsStringAsync();
            authorization = request.Headers.Authorization?.ToString();
            return JsonResponse(
                """
                {"choices":[{"message":{"content":null,"tool_calls":[{"id":"call-1","type":"function","function":{"name":"workspace_inspect","arguments":"{\"workspacePath\":\"sample.sln\"}"}}]}}]}
                """);
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "https://api.example.test/v1/chat/completions",
                ModelId = "test-model",
                ApiKey = "secret",
            },
            httpClient);
        AIChatToolDescriptor tool = new(
            "workspace.inspect",
            "Inspect workspace",
            """{"type":"object","properties":{"workspacePath":{"type":"string"}},"required":["workspacePath"]}""");

        AICompletionResponse result = await client.CompleteAsync(new AICompletionRequest(
            [new AIMessage(AIMessageRole.User, "Inspect this workspace.")],
            tools: [tool]));

        Assert.AreEqual("Bearer secret", authorization);
        using JsonDocument body = JsonDocument.Parse(requestBody!);
        Assert.AreEqual("test-model", body.RootElement.GetProperty("model").GetString());
        Assert.AreEqual("auto", body.RootElement.GetProperty("tool_choice").GetString());
        Assert.AreEqual("workspace_inspect", body.RootElement.GetProperty("tools")[0]
            .GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual(1, result.ToolCalls.Count);
        Assert.AreEqual("call-1", result.ToolCalls[0].CallId);
        Assert.AreEqual("workspace.inspect", result.ToolCalls[0].Name);
        Assert.AreEqual("sample.sln", result.ToolCalls[0].Arguments.GetProperty("workspacePath").GetString());
    }

    [TestMethod]
    public async Task CompleteAsync_RequestModelOverridesConfiguredModelWithoutChangingSettings()
    {
        string? requestBody = null;
        using HttpClient httpClient = new(new StubHandler(async request =>
        {
            requestBody = await request.Content!.ReadAsStringAsync();
            return JsonResponse("""{"choices":[{"message":{"content":"Ready"}}]}""");
        }));
        OpenAICompatibleAISettings settings = new()
        {
            Endpoint = "https://api.example.test/v1/chat/completions",
            ModelId = "configured",
        };
        OpenAICompatibleAIClient client = new(settings, httpClient);

        await client.CompleteAsync(new AICompletionRequest(
            [new AIMessage(AIMessageRole.User, "Hello")],
            new AICompletionOptions { ModelId = "session-model" }));

        using JsonDocument body = JsonDocument.Parse(requestBody!);
        Assert.AreEqual("session-model", body.RootElement.GetProperty("model").GetString());
        Assert.AreEqual("configured", settings.ModelId);
    }

    [TestMethod]
    public async Task CompleteAsync_UsesUniqueProviderSafeToolNamesAndMapsCallsBack()
    {
        string? requestBody = null;
        using HttpClient httpClient = new(new StubHandler(async request =>
        {
            requestBody = await request.Content!.ReadAsStringAsync();
            return JsonResponse(
                """
                {"choices":[{"message":{"content":null,"tool_calls":[{"id":"call-1","type":"function","function":{"name":"avln_tool_1","arguments":"{}"}}]}}]}
                """);
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "https://api.example.test/v1/chat/completions",
                ModelId = "test-model",
            },
            httpClient);
        AIChatToolDescriptor[] tools =
        [
            new("workspace.inspect", "Inspect workspace", """{"type":"object"}"""),
            new("workspace_inspect", "Another tool", """{"type":"object"}"""),
            new("avln_tool_0", "Reserve fallback alias", """{"type":"object"}"""),
        ];

        AICompletionResponse result = await client.CompleteAsync(new AICompletionRequest(
            [new AIMessage(AIMessageRole.User, "Inspect this workspace.")],
            tools: tools));

        using JsonDocument body = JsonDocument.Parse(requestBody!);
        JsonElement providerTools = body.RootElement.GetProperty("tools");
        Assert.AreEqual("avln_tool_1", providerTools[0].GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual("workspace_inspect", providerTools[1].GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual("avln_tool_0", providerTools[2].GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual("workspace.inspect", result.ToolCalls[0].Name);
    }

    [TestMethod]
    public async Task CompleteAsync_UsesProviderSafeToolNameInFollowUpRequest()
    {
        string? followUpRequestBody = null;
        int requestCount = 0;
        using HttpClient httpClient = new(new StubHandler(async request =>
        {
            if (requestCount++ == 0)
            {
                return JsonResponse(
                    """
                    {"choices":[{"message":{"content":null,"tool_calls":[{"id":"call-1","type":"function","function":{"name":"workspace_inspect","arguments":"{}"}}]}}]}
                    """);
            }

            followUpRequestBody = await request.Content!.ReadAsStringAsync();
            return JsonResponse("""{"choices":[{"message":{"content":"Found the workspace."}}]}""");
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "https://api.example.test/v1/chat/completions",
                ModelId = "test-model",
            },
            httpClient);
        AIChatToolDescriptor tool = new(
            "workspace.inspect",
            "Inspect workspace",
            """{"type":"object","properties":{},"additionalProperties":false}""");

        AICompletionResponse firstResponse = await client.CompleteAsync(new AICompletionRequest(
            [new AIMessage(AIMessageRole.User, "Inspect this workspace.")],
            tools: [tool]));
        AICompletionResponse followUpResponse = await client.CompleteAsync(new AICompletionRequest(
            [
                new AIMessage(AIMessageRole.User, "Inspect this workspace."),
                firstResponse.Message,
                new AIMessage(
                    AIMessageRole.Tool,
                    """{"projects":[]}""",
                    toolCallId: firstResponse.ToolCalls[0].CallId,
                    toolName: firstResponse.ToolCalls[0].Name),
            ],
            tools: [tool]));

        using JsonDocument body = JsonDocument.Parse(followUpRequestBody!);
        Assert.AreEqual(
            "workspace_inspect",
            body.RootElement.GetProperty("messages")[1]
                .GetProperty("tool_calls")[0].GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual("Found the workspace.", followUpResponse.Message.Content);
    }

    [TestMethod]
    public async Task CompleteAsync_MapsHistoricalToolsWhenCurrentModeExcludesThem()
    {
        string? requestBody = null;
        using HttpClient httpClient = new(new StubHandler(async request =>
        {
            requestBody = await request.Content!.ReadAsStringAsync();
            return JsonResponse("""{"choices":[{"message":{"content":"Ready."}}]}""");
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings { Endpoint = "https://api.example.test/v1/chat/completions", ModelId = "test" },
            httpClient);
        using JsonDocument arguments = JsonDocument.Parse("{}");
        await client.CompleteAsync(new AICompletionRequest(
            [
                new AIMessage(AIMessageRole.Assistant, "", toolCalls: [new AIChatToolCall("call-1", "workspace.build", arguments.RootElement)]),
                new AIMessage(AIMessageRole.Tool, "Done", toolCallId: "call-1", toolName: "workspace.build"),
                new AIMessage(AIMessageRole.User, "Plan the next change."),
            ],
            tools: [new AIChatToolDescriptor("workspace.inspect", "Inspect", """{"type":"object"}""")]));

        using JsonDocument body = JsonDocument.Parse(requestBody!);
        Assert.AreEqual("workspace_build", body.RootElement.GetProperty("messages")[0]
            .GetProperty("tool_calls")[0].GetProperty("function").GetProperty("name").GetString());
        Assert.AreEqual(1, body.RootElement.GetProperty("tools").GetArrayLength());
    }

    [TestMethod]
    public async Task CompleteAsync_RejectsNonLocalHttpEndpointBeforeSending()
    {
        bool requestSent = false;
        using HttpClient httpClient = new(new StubHandler(_ =>
        {
            requestSent = true;
            return Task.FromResult(JsonResponse("""{"choices":[{"message":{"content":"unexpected"}}]}"""));
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "http://api.example.test/v1/chat/completions",
                ModelId = "test-model",
            },
            httpClient);

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")])));

        StringAssert.Contains(exception.Message, "HTTPS");
        Assert.IsFalse(requestSent);
    }

    [DataTestMethod]
    [DataRow("https://user:password@api.example.test/v1/chat/completions")]
    [DataRow("https://api.example.test/v1/chat/completions#fragment")]
    public async Task CompleteAsync_RejectsCredentialsAndFragmentsInEndpoint(string endpoint)
    {
        bool requestSent = false;
        using HttpClient httpClient = new(new StubHandler(_ =>
        {
            requestSent = true;
            return Task.FromResult(JsonResponse("""{"choices":[{"message":{"content":"unexpected"}}]}"""));
        }));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings { Endpoint = endpoint, ModelId = "test" }, httpClient);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")]))); 
        Assert.IsFalse(requestSent);
    }

    [TestMethod]
    public async Task CompleteAsync_ReportsProviderHttpErrors()
    {
        using HttpClient httpClient = new(new StubHandler(_ =>
            Task.FromResult(JsonResponse("""{"error":{"message":"invalid key"}}""", HttpStatusCode.Unauthorized))));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "http://localhost:11434/v1/chat/completions",
                ModelId = "test-model",
            },
            httpClient);

        HttpRequestException exception = await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")])));

        Assert.AreEqual(HttpStatusCode.Unauthorized, exception.StatusCode);
        StringAssert.Contains(exception.Message, "invalid key");
    }

    [TestMethod]
    public async Task CompleteAsync_ResolvesApiKeyFromEnvironmentReference()
    {
        string variableName = $"AVLN_AI_TEST_KEY_{Guid.NewGuid():N}";
        string apiKey = Guid.NewGuid().ToString("N");
        Environment.SetEnvironmentVariable(variableName, apiKey);
        string? sentApiKey = null;
        try
        {
            using HttpClient httpClient = new(new StubHandler(request =>
            {
                sentApiKey = request.Headers.Authorization?.Parameter;
                return Task.FromResult(JsonResponse("""{"choices":[{"message":{"content":"ok"}}]}"""));
            }));
            OpenAICompatibleAIClient client = new(
                new OpenAICompatibleAISettings
                {
                    Endpoint = "https://api.example.test/v1/chat/completions",
                    ModelId = "test-model",
                    ApiKey = $"env:{variableName}",
                },
                httpClient);

            await client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")]));

            Assert.AreEqual(apiKey, sentApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }

    [TestMethod]
    public async Task CompleteAsync_ReportsMissingApiKeyEnvironmentVariable()
    {
        string variableName = $"AVLN_AI_MISSING_KEY_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(variableName, null);
        using HttpClient httpClient = new(new StubHandler(_ =>
            Task.FromResult(JsonResponse("""{"choices":[{"message":{"content":"unexpected"}}]}"""))));
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "https://api.example.test/v1/chat/completions",
                ModelId = "test-model",
                ApiKey = $"env:{variableName}",
            },
            httpClient);

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")])));

        StringAssert.Contains(exception.Message, variableName);
    }

    [TestMethod]
    public async Task CompleteAsync_AppliesTimeoutWhileReadingResponseBody()
    {
        using HttpClient httpClient = new(new StubHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new DelayedContent(),
            })))
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };
        OpenAICompatibleAIClient client = new(
            new OpenAICompatibleAISettings
            {
                Endpoint = "http://localhost:11434/v1/chat/completions",
                ModelId = "test-model",
            },
            httpClient,
            TimeSpan.FromMilliseconds(50));

        TimeoutException exception = await Assert.ThrowsExactlyAsync<TimeoutException>(() =>
            client.CompleteAsync(new AICompletionRequest([new AIMessage(AIMessageRole.User, "Hi.")])));

        StringAssert.Contains(exception.Message, "0.05-second timeout");
    }

    private static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
        };
    }

    private sealed class StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return respond(request);
        }
    }

    private sealed class DelayedContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return Task.Delay(Timeout.InfiniteTimeSpan);
        }

        protected override Task SerializeToStreamAsync(
            Stream stream,
            TransportContext? context,
            CancellationToken cancellationToken)
        {
            return Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
