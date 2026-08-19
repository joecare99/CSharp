using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Protocol.Models;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class RuntimeCoverageTests
{
    [TestMethod]
    public void ModelsAndParsers_ValidateAllPublicInputVariants()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AgentMessage(string.Empty, "content"));
        Assert.ThrowsExactly<ArgumentException>(() => new AgentMessage("user", string.Empty));
        AgentMessage message = new("user", "content");
        Assert.AreEqual("user", message.Role);
        Assert.AreEqual("content", message.Content);

        Assert.AreEqual(string.Empty, AgentResponseNormalizer.Normalize(" ", out bool emptyFinal));
        Assert.IsFalse(emptyFinal);
        Assert.AreEqual("answer", AgentResponseNormalizer.Normalize(" answer ", out bool unmarkedFinal));
        Assert.IsFalse(unmarkedFinal);
        Assert.AreEqual("answer", AgentResponseNormalizer.Normalize("[[FINAL]] answer", out bool markedFinal));
        Assert.IsTrue(markedFinal);

        AgentRunRequest request = new() { Prompt = "prompt", SystemPrompt = "system" };
        request.Validate();
        Assert.ThrowsExactly<ArgumentException>(() => new AgentRunRequest { Prompt = string.Empty, SystemPrompt = "system" }.Validate());
        Assert.ThrowsExactly<ArgumentException>(() => new AgentRunRequest { Prompt = "prompt", SystemPrompt = string.Empty }.Validate());
        Assert.AreEqual(AgentVerbosity.Normal, new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1).Verbosity);

        Assert.ThrowsExactly<ArgumentException>(() => new GoalContract(string.Empty, []));
        Assert.ThrowsExactly<ArgumentException>(() => new PlannedSubtask(string.Empty, "title", "reason"));
        Assert.ThrowsExactly<ArgumentException>(() => new PlannedSubtask("id", string.Empty, "reason"));
        Assert.ThrowsExactly<ArgumentException>(() => new PlannedSubtask("id", "title", string.Empty));
        Assert.ThrowsExactly<ArgumentException>(() => new PlanState(new GoalContract("goal", []), []));

        OllamaToolCall objectInput = ToolCallParser.Parse("""before {"name":"tool","input":{"value":1}} after""");
        OllamaToolCall arrayInput = ToolCallParser.Parse("""{"tool":"tool","arguments":["value"]}""");
        OllamaToolCall nullInput = ToolCallParser.Parse("""{"tool_name":"tool","params":null}""");
        OllamaToolCall scalarInput = ToolCallParser.Parse("""{"toolName":"tool","input":42}""");
        Assert.AreEqual("""{"value":1}""", objectInput.Input);
        Assert.AreEqual("""["value"]""", arrayInput.Input);
        Assert.AreEqual(string.Empty, nullInput.Input);
        Assert.AreEqual("42", scalarInput.Input);
        Assert.AreEqual("{}", ToolCallParser.Parse("""{"toolName":"tool"}""").Input);
        Assert.ThrowsExactly<InvalidOperationException>(() => ToolCallParser.Parse("""{"input":"{}"}"""));
        Assert.ThrowsExactly<InvalidOperationException>(() => ToolCallParser.Parse("""{"toolName":"tool" """));

        WebKnowledgePolicy policy = new();
        Assert.IsTrue(policy.TryResolveSourceTemplate("rosettacode", out string template));
        StringAssert.Contains(template, "{query}");
        Assert.IsFalse(policy.TryResolveSourceTemplate("nope", out _));
        Assert.AreEqual(3, policy.AllowedSources.Count);
    }

    [TestMethod]
    public async Task WebLookupAndOllamaAdapters_NormalizeDeterministicResponses()
    {
        using HttpClient httpClient = new(new StaticResponseHandler(HttpStatusCode.BadGateway, new string('x', 4_100)));
        WebLookupTool lookup = new(httpClient, new WebKnowledgePolicy());
        Assert.IsFalse(lookup.Validate(string.Empty).IsValid);
        Assert.IsFalse(lookup.Validate("""{"source":"","query":"query"}""").IsValid);
        Assert.IsTrue(lookup.Validate("""{"source":"mslearn","query":"dependency injection"}""").IsValid);
        OllamaToolResult unavailable = await lookup.ExecuteAsync("""{"source":"nope","query":"query"}""");
        Assert.IsFalse(unavailable.Success);
        OllamaToolResult response = await lookup.ExecuteAsync("""{"source":"mslearn","query":"dependency injection"}""");
        Assert.IsFalse(response.Success);
        using JsonDocument document = JsonDocument.Parse(response.Output);
        Assert.AreEqual(4_000, document.RootElement.GetProperty("ContentPreview").GetString()!.Length);
        Assert.AreEqual("web_lookup", lookup.Name);
        Assert.IsTrue(lookup.Schema.Parameters.Count > 0);

        IOllamaProtocolAdapter adapter = Substitute.For<IOllamaProtocolAdapter>();
        adapter.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = "answer" },
                    Thinking = "reasoning",
                    Done = true,
                },
            ]));
        OllamaChatClient chatClient = new(adapter, "model");
        OllamaChatModelClient modelClient = new(chatClient);
        AgentCompletion completion = await modelClient.CompleteDetailedAsync([new AgentMessage("user", "question")]);
        Assert.AreEqual("answer", completion.Content);
        CollectionAssert.AreEqual(new[] { "reasoning" }, (System.Collections.ICollection)completion.Thinking);
        Assert.AreEqual("answer", await modelClient.CompleteAsync([new AgentMessage("user", "question")]));
        Assert.AreEqual("ollama", modelClient.Capabilities.ProviderName);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => modelClient.CompleteDetailedAsync([]));
        OllamaToolChatRunnerAdapter toolAdapter = new(chatClient);
        Assert.AreEqual("answer", (await toolAdapter.CompleteChatAsync(new ChatCompletionOptions
        {
            Messages = [new OllamaClientChatMessage { Role = "user", Content = "question" }],
        })).Content);
    }

    [TestMethod]
    public void RegistryAndServiceRegistration_CreateAllRuntimeServices()
    {
        using TestWorkspace workspace = new();
        WorkspacePathPolicy pathPolicy = new(workspace.RootPath);
        CodingDelegationToolRegistryFactory factory = new(pathPolicy);
        IOllamaToolRegistry registry = factory.CreateRegistry();
        Assert.AreEqual(8, registry.GetDescriptors().Count);
        Assert.IsTrue(registry.TryGetTool("local_wiki_search", out _));

        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse(
        [
            "--endpoint", "http://localhost:11434",
            "--model", "model",
            "--workspace-root", workspace.RootPath,
        ]);
        ServiceCollection services = new();
        services.AddOllamaCodingAgent(options);
        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Assert.IsNotNull(provider.GetRequiredService<AgentRunner>());
        Assert.IsNotNull(provider.GetRequiredService<OllamaBaselineService>());
        Assert.IsNotNull(provider.GetRequiredService<CodingTaskDelegationService>());
    }

    [TestMethod]
    public async Task ToolChatRunner_LogsDelegatedToolDefinitions()
    {
        IOllamaProtocolAdapter protocol = Substitute.For<IOllamaProtocolAdapter>();
        protocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = "answer" },
                    Done = true,
                },
            ]));
        ILlmTrafficLogger logger = Substitute.For<ILlmTrafficLogger>();
        OllamaToolChatRunnerAdapter adapter = new(
            new OllamaChatClient(protocol, "model"),
            null,
            logger,
            new Uri("http://localhost:11434/api/chat"));

        await adapter.CompleteChatAsync(new ChatCompletionOptions
        {
            Messages = [new OllamaClientChatMessage { Role = "user", Content = "question" }],
            Tools =
            [
                new OllamaChatTool
                {
                    Name = "list_workspace_files",
                },
            ],
        });

        logger.Received(1).LogRequest(
            "ollama",
            Arg.Any<Uri>(),
            "chat.completions",
            Arg.Is<string>(payload => payload.Contains("list_workspace_files", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task BaselineService_ConvertsTransportAndSmokeOutcomes()
    {
        IOllamaBaselineClient client = Substitute.For<IOllamaBaselineClient>();
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<string>>(["model"]));
        client.CompleteChatAsync("prompt", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new OllamaChatCompletion { Content = "answer" }));
        OllamaBaselineService service = new(client, "model");
        Assert.IsTrue((await service.RunSmokeAsync("prompt")).Success);

        client.CompleteChatAsync("empty", Arg.Any<CancellationToken>()).Returns(Task.FromResult(new OllamaChatCompletion { Content = string.Empty }));
        Assert.IsFalse((await service.RunSmokeAsync("empty")).Success);
        client.CompleteChatAsync("failure", Arg.Any<CancellationToken>()).Returns(Task.FromException<OllamaChatCompletion>(new HttpRequestException("offline")));
        StringAssert.Contains((await service.RunSmokeAsync("failure")).Error, "offline");
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException<IReadOnlyList<string>>(new HttpRequestException("offline")));
        StringAssert.Contains((await service.RunPreflightAsync()).Error, "offline");
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => service.RunSmokeAsync(string.Empty));
        Assert.ThrowsExactly<ArgumentException>(() => new OllamaBaselineService(client, string.Empty));
    }

    [TestMethod]
    public async Task DelegationService_ExecutesModelAndFallbackPaths()
    {
        using TestWorkspace workspace = new();
        await File.WriteAllTextAsync(workspace.GetPath("source.cs"), "class Source { }");
        IOllamaToolRegistry registry = new CodingDelegationToolRegistryFactory(new WorkspacePathPolicy(workspace.RootPath)).CreateRegistry();
        OllamaToolOrchestrator orchestrator = new(registry);
        IAgentModelClient summaryClient = Substitute.For<IAgentModelClient>();
        summaryClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("[[FINAL]] summary"));
        OllamaAgentRuntimeSettings verboseSettings = new(TimeSpan.FromSeconds(1), 0, 3, AgentVerbosity.Verbose);

        IOllamaProtocolAdapter selectingProtocol = Substitute.For<IOllamaProtocolAdapter>();
        selectingProtocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = """{"toolName":"list_workspace_files","input":"{}"}""" },
                    Done = true,
                },
            ]));
        InMemoryAgentDiagnosticsSink diagnostics = new();
        CodingTaskDelegationService service = new(
            summaryClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(selectingProtocol, "model")),
            registry,
            orchestrator,
            verboseSettings,
            diagnostics);

        AgentRunResult selected = await service.RunDelegatedAsync("Inspect source files.");
        StringAssert.Contains(selected.FinalResponse, "Step 1: list_workspace_files | success=True");
        StringAssert.Contains(selected.FinalResponse, "Agent summary:");
        StringAssert.EndsWith(selected.FinalResponse, "summary");
        Assert.IsTrue(selected.FinalizedWithMarker);
        selectingProtocol.Received().ChatStreamingAsync(
            Arg.Is<OllamaChatRequest>(request => request.Tools != null && request.Tools.Count > 0
                && request.Tools.Any(tool => tool.Function.Name == "list_workspace_files")),
            Arg.Any<CancellationToken>());
        string[] eventNames = diagnostics.Events.Select(static diagnosticEvent => diagnosticEvent.EventName).ToArray();
        CollectionAssert.Contains(eventNames, "delegation.started");
        CollectionAssert.Contains(eventNames, "tool.selection.requested");
        CollectionAssert.Contains(eventNames, "tool.selection.completed");
        CollectionAssert.Contains(eventNames, "tool.call.requested");
        CollectionAssert.Contains(eventNames, "tool.call.completed");

        IOllamaProtocolAdapter failingProtocol = Substitute.For<IOllamaProtocolAdapter>();
        failingProtocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new HttpRequestException("selection unavailable"));
        summaryClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<string>(new InvalidOperationException("summary unavailable")));
        CodingTaskDelegationService fallbackService = new(
            summaryClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(failingProtocol, "model")),
            registry,
            orchestrator,
            new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 3, AgentVerbosity.Quiet));

        AgentRunResult fallback = await fallbackService.RunDelegatedAsync("Inspect source files.");
        StringAssert.Contains(fallback.FinalResponse, "continue from tool list_workspace_files");
        Assert.AreEqual(0, fallback.IterationsUsed);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => fallbackService.RunDelegatedAsync(string.Empty));
    }

    [TestMethod]
    public async Task OllamaClientBaselineAdapter_UsesTagsAndChatClients()
    {
        using HttpClient httpClient = new(new OllamaResponseHandler());
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri("https://ollama.test")));
        OllamaClientBaselineAdapter adapter = new(client, "model");

        IReadOnlyList<string> models = await adapter.GetAvailableModelsAsync();
        CollectionAssert.AreEqual(new[] { "model", "ignored" }, (System.Collections.ICollection)models);
        Assert.AreEqual("answer", (await adapter.CompleteChatAsync("prompt")).Content);
        Assert.ThrowsExactly<ArgumentException>(() => new OllamaClientBaselineAdapter(client, string.Empty));
    }

    private sealed class StaticResponseHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public StaticResponseHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(_statusCode) { Content = new StringContent(_content) });
    }

    private sealed class OllamaResponseHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string content = request.RequestUri!.AbsolutePath switch
            {
                "/api/tags" => """{"models":[{"name":"model"},{"model":"ignored"}]}""",
                "/api/chat" => """{"message":{"role":"assistant","content":"answer"},"done":true}""",
                _ => throw new InvalidOperationException($"Unexpected endpoint '{request.RequestUri}'."),
            };
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });
        }
    }
}
