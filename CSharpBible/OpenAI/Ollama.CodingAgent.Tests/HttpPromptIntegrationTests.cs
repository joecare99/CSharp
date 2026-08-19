using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Client.Models;
using Ollama.Protocol.Models;
using Ollama.Protocol.Services;
using Ollama.Protocol.Models;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class HttpPromptIntegrationTests
{
    [TestMethod]
    public async Task DelegatedPrompt_SendsExpectedPayloadToTestHttpInterface()
    {
        using TestWorkspace workspace = new();
        using CapturingOllamaHttpHandler handler = new();
        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("http://test-ollama.local/"),
        };
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri("http://test-ollama.local/")));
        OllamaChatClient chatClient = client.GetChatClient("test-model");
        FileLlmTrafficLogger logger = new(new FileLlmTrafficLogOptions(
            Path.Combine(Path.GetTempPath(), "coding-agent-integration-logs", Guid.NewGuid().ToString("N"))),
            "integration-session");
        IOllamaToolRegistry registry = new CodingDelegationToolRegistryFactory(new WorkspacePathPolicy(workspace.RootPath)).CreateRegistry();
        OllamaToolOrchestrator orchestrator = new(registry);
        OllamaAgentRuntimeSettings settings = new(TimeSpan.FromSeconds(10), 0, 3, AgentVerbosity.Normal);
        CodingTaskDelegationService service = new(
            new OllamaChatModelClient(chatClient, new Uri("http://test-ollama.local/api/chat"), logger),
            new OllamaToolChatRunnerAdapter(chatClient, null, logger, new Uri("http://test-ollama.local/api/chat")),
            registry,
            orchestrator,
            settings);

        AgentRunResult result = await service.RunDelegatedAsync("Inspect the workspace and report what you find.");

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.FinalResponse));
        Assert.IsTrue(handler.Requests.Count >= 2);
        CapturedChatRequest toolRequest = handler.Requests.First(request => request.Tools.Count > 0);
        Assert.AreEqual("test-model", toolRequest.Model);
        CapturedMessage userMessage = toolRequest.Messages.First(message => message.Role == "user");
        StringAssert.Contains(userMessage.Content, "Inspect the workspace");
        CollectionAssert.Contains(toolRequest.Tools.Select(tool => tool.Name).ToArray(), "list_workspace_files");
        CapturedChatRequest summaryRequest = handler.Requests.Last(request => request.Tools.Count == 0);
        Assert.IsTrue(summaryRequest.Messages.Any(message => message.Content.Contains("Original coding task", StringComparison.Ordinal)));
    }

    private sealed class CapturingOllamaHttpHandler : HttpMessageHandler
    {
        public ConcurrentQueue<CapturedChatRequest> Requests { get; } = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.AreEqual("/api/chat", request.RequestUri?.AbsolutePath);
            string payload = await request.Content!.ReadAsStringAsync(cancellationToken);
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement root = document.RootElement;
            List<CapturedMessage> messages = root.GetProperty("messages")
                .EnumerateArray()
                .Select(message => new CapturedMessage(
                    message.GetProperty("role").GetString() ?? string.Empty,
                    message.GetProperty("content").GetString() ?? string.Empty))
                .ToList();
            List<CapturedTool> tools = root.TryGetProperty("tools", out JsonElement toolsElement)
                ? toolsElement.EnumerateArray()
                    .Select(tool => new CapturedTool(tool.GetProperty("function").GetProperty("name").GetString() ?? string.Empty))
                    .ToList()
                : [];
            CapturedChatRequest captured = new(root.GetProperty("model").GetString() ?? string.Empty, messages, tools);
            Requests.Enqueue(captured);

            string content = tools.Count > 0
                ? "{\"toolName\":\"list_workspace_files\",\"input\":{}}"
                : "[[FINAL]] HTTP test summary";
            string response = JsonSerializer.Serialize(new OllamaChatResponseChunk
            {
                Message = new OllamaChatMessage { Role = "assistant", Content = content },
                Done = true,
            }) + "\n";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/x-ndjson"),
            };
        }
    }

    private sealed record CapturedChatRequest(string Model, IReadOnlyList<CapturedMessage> Messages, IReadOnlyList<CapturedTool> Tools);

    private sealed record CapturedMessage(string Role, string Content);

    private sealed record CapturedTool(string Name);
}
