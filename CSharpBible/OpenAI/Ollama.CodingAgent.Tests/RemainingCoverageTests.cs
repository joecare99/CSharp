using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Client.Models;
using Ollama.Protocol.Models;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class RemainingCoverageTests
{
    [TestMethod]
    public async Task AgentRunner_RetriesOperationCancellationAndContinuesAfterEmptyNormalizedResponse()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => Task.FromException<string>(new OperationCanceledException("step timeout")),
                _ => Task.FromResult("[[FINAL]]"),
                _ => Task.FromResult("Recovered"));

        AgentRunner runner = new(
            modelClient,
                new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), retryCount: 1, maxIterations: 2, retryBackoff: TimeSpan.Zero));

        AgentRunResult result = await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "retry cancellation",
            SystemPrompt = "system",
        });

        Assert.AreEqual("Recovered", result.FinalResponse);
        Assert.AreEqual(1, result.RetryAttemptsUsed);
        Assert.AreEqual(2, result.IterationsUsed);
    }

    [TestMethod]
    public async Task AgentRunner_HandlesCancellationOnFinalAttempt()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<string>(new OperationCanceledException("final cancellation")));

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() =>
            new AgentRunner(
                modelClient,
                new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), retryCount: 0, maxIterations: 1, retryBackoff: TimeSpan.Zero))
            .RunAsync(new AgentRunRequest { Prompt = "prompt", SystemPrompt = "system" }));
    }

    [TestMethod]
    public async Task AgentRunner_HandlesCancellationAfterTheModelCancelsTheCaller()
    {
        using CancellationTokenSource cancellation = new();
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                cancellation.Cancel();
                return Task.FromException<string>(new OperationCanceledException("cancelled by model"));
            });

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() =>
            new AgentRunner(
                modelClient,
                new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), retryCount: 1, maxIterations: 1, retryBackoff: TimeSpan.Zero))
            .RunAsync(new AgentRunRequest { Prompt = "prompt", SystemPrompt = "system" }, cancellation.Token));
    }

    [TestMethod]
    public void LocalWikiWrite_ValidatesAllowListedCitation()
    {
        using TestWorkspace workspace = new();
        LocalWikiWriteTool tool = new(new LocalKnowledgeBaseStore(workspace.GetPath("wiki.json")));

        var validation = tool.Validate(
            """{"id":"entry","title":"Title","summary":"Summary","citationUrl":"https://learn.microsoft.com/en-us/dotnet"}""");

        Assert.IsTrue(validation.IsValid);
    }

    [TestMethod]
    public async Task PlannerAndFilesystemTools_CoverNullAndFallbackInputs()
    {
        Assert.AreEqual("run_dotnet_test", DelegationFallbackToolPlanner.CreateFallbackToolCall("run integration test").ToolName);
        Assert.AreEqual("run_dotnet_build", DelegationFallbackToolPlanner.CreateFallbackToolCall("compile the project").ToolName);
        Assert.AreEqual("list_workspace_files", DelegationFallbackToolPlanner.CreateFallbackToolCall("inspect files").ToolName);
        MethodInfo fallbackMethod = typeof(DelegationFallbackToolPlanner)
            .GetMethod(nameof(DelegationFallbackToolPlanner.CreateFallbackToolCall))
            ?? throw new InvalidOperationException("Fallback method was not found.");
        OllamaToolCall nullPromptFallback = (OllamaToolCall)fallbackMethod.Invoke(null, [null])!;
        Assert.AreEqual("list_workspace_files", nullPromptFallback.ToolName);

        GoalContract goal = new("Build a reliable service", []);
        PlannedSubtask subtask = new("id", "title", "reason");
        MethodInfo driftMethod = typeof(GoalDriftAnalyzer).GetMethod(nameof(GoalDriftAnalyzer.IsDriftDetected))
            ?? throw new InvalidOperationException("Goal drift method was not found.");
        Assert.IsTrue((bool)driftMethod.Invoke(null, [goal, subtask, null])!);
        FieldInfo objectiveField = typeof(GoalContract).GetField("<Objective>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Goal objective backing field was not found.");
        objectiveField.SetValue(goal, null);
        Assert.IsTrue(GoalDriftAnalyzer.IsDriftDetected(goal, subtask, "failed"));

        using TestWorkspace workspace = new();
        WorkspacePathPolicy policy = new(workspace.RootPath);
        ListWorkspaceFilesTool listTool = new(policy);
        Assert.IsTrue(listTool.Validate("null").IsValid);
        Assert.IsTrue((await listTool.ExecuteAsync(string.Empty)).Success);
        Assert.IsTrue((await listTool.ExecuteAsync("null")).Success);

        Assert.ThrowsExactly<ArgumentException>(() => new LocalKnowledgeBaseStore(string.Empty));
        LocalKnowledgeBaseStore store = new(workspace.GetPath("wiki.json"));
        LocalWikiSearchTool searchTool = new(store);
        Assert.IsFalse(searchTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => searchTool.ExecuteAsync("null"));

        LocalWikiWriteTool writeTool = new(store);
        Assert.IsFalse(writeTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writeTool.ExecuteAsync("null"));
        await writeTool.ExecuteAsync(
            """{"id":"null-tag","title":"Title","summary":"Summary","tags":[null]}""");

        FieldInfo databasePathField = typeof(LocalKnowledgeBaseStore)
            .GetField("_databaseFilePath", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Knowledge database field was not found.");
        databasePathField.SetValue(store, "C:\\");
        MethodInfo saveMethod = typeof(LocalKnowledgeBaseStore)
            .GetMethod("SaveEntriesAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Save method was not found.");
        Task saveTask = (Task)saveMethod.Invoke(store, [Array.Empty<LocalKnowledgeEntry>(), CancellationToken.None])!;
        await Assert.ThrowsAsync<Exception>(() => saveTask);

        WebLookupTool webTool = new(new HttpClient(), new WebKnowledgePolicy());
        Assert.IsFalse(webTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => webTool.ExecuteAsync("null"));
    }

    [TestMethod]
    public async Task Importer_HandlesSingleQuotedFrontmatterTitle()
    {
        using TestWorkspace workspace = new();
        string vault = workspace.GetPath("vault");
        Directory.CreateDirectory(vault);
        await File.WriteAllTextAsync(
            workspace.GetPath("vault", "single.md"),
            "---\ntitle: 'Single quoted title'\n---\n\nBody");

        LocalKnowledgeBaseStore store = new(workspace.GetPath("wiki.json"));
        await new LocalWikiMarkdownImporter().ImportAsync(vault, store);

        Assert.AreEqual("Single quoted title", (await store.LoadAllAsync())[0].Title);

        await File.WriteAllTextAsync(
            workspace.GetPath("vault", "short-title.md"),
            "---\ntitle: \"\n---\n\n# Fallback title\n\nBody");
        await new LocalWikiMarkdownImporter().ImportAsync(vault, store);
        Assert.IsTrue((await store.LoadAllAsync()).Count >= 2);

        await File.WriteAllTextAsync(
            workspace.GetPath("vault", "same-title.md"),
            "---\ntitle: aa\n---\n\nBody");
        await new LocalWikiMarkdownImporter().ImportAsync(vault, store);
        Assert.IsTrue((await store.LoadAllAsync()).Any(entry => entry.Title == "aa"));
    }

    [TestMethod]
    public async Task OllamaAdapters_HandleMissingOptionalValues()
    {
        using HttpClient httpClient = new(new TagsWithEmptyModelHandler());
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri("https://ollama.test")));
        Assert.ThrowsExactly<TargetInvocationException>(() =>
            typeof(OllamaClientBaselineAdapter).GetConstructors()[0].Invoke([null, "model"]));

        OllamaClientBaselineAdapter adapter = new(client, "model");
        CollectionAssert.AreEqual(
            new[] { "named", "modeled" },
            (System.Collections.ICollection)await adapter.GetAvailableModelsAsync());

        Assert.ThrowsExactly<TargetInvocationException>(() =>
            typeof(OllamaToolChatRunnerAdapter).GetConstructors()
                .Single(candidate => candidate.GetParameters().Length == 1)
                .Invoke([null]));

        Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>> completionRunner =
            Substitute.For<Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>>>();
        OllamaChatCompletion nullContentCompletion = new() { Content = "placeholder" };
        (typeof(OllamaChatCompletion).GetProperty(nameof(OllamaChatCompletion.Content))
            ?? throw new InvalidOperationException("Completion content property was not found."))
            .SetValue(nullContentCompletion, null);
        completionRunner.Invoke(Arg.Any<ChatCompletionOptions>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(nullContentCompletion));
        OllamaChatModelClient modelWithNullContent = new(
            client.GetChatClient("model"),
            completionRunner);
        Assert.AreEqual(string.Empty, (await modelWithNullContent.CompleteDetailedAsync(
            [new AgentMessage("user", "question")])).Content);

        PlanState plan = new(new GoalContract("goal", []), [new PlannedSubtask("id", "title", "reason")]);
        Assert.ThrowsExactly<TargetInvocationException>(() =>
            typeof(PlanState).GetConstructors()[0].Invoke([null, plan.Subtasks]));
        Assert.ThrowsExactly<TargetInvocationException>(() =>
            typeof(PlanState).GetConstructors()[0].Invoke([plan.GoalContract, null]));
    }

    [TestMethod]
    public async Task DelegationService_CoversConstructorAndFallbackOutcomes()
    {
        using TestWorkspace workspace = new();
        WorkspacePathPolicy policy = new(workspace.RootPath);
        IOllamaToolRegistry registry = new CodingDelegationToolRegistryFactory(policy).CreateRegistry();
        OllamaToolOrchestrator orchestrator = new(registry);
        IOllamaProtocolAdapter protocol = Substitute.For<IOllamaProtocolAdapter>();
        OllamaToolChatRunnerAdapter adapter = new(new OllamaChatClient(protocol, "model"));
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        OllamaAgentRuntimeSettings settings = new(TimeSpan.FromSeconds(1), 0, 2);

        AssertConstructorRejectsNull(typeof(CodingTaskDelegationService), [null, adapter, registry, orchestrator, settings]);
        AssertConstructorRejectsNull(typeof(CodingTaskDelegationService), [modelClient, null, registry, orchestrator, settings]);
        AssertConstructorRejectsNull(typeof(CodingTaskDelegationService), [modelClient, adapter, null, orchestrator, settings]);
        AssertConstructorRejectsNull(typeof(CodingTaskDelegationService), [modelClient, adapter, registry, null, settings]);
        AssertConstructorRejectsNull(typeof(CodingTaskDelegationService), [modelClient, adapter, registry, orchestrator, null]);

        modelClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("summary"));
        protocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = "not json" },
                    Done = true,
                },
            ]));

        AgentRunResult parseFailure = await new CodingTaskDelegationService(
            modelClient, adapter, registry, orchestrator, settings)
            .RunDelegatedAsync("Inspect source files.");
        StringAssert.Contains(parseFailure.FinalResponse, "summary");

        Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>> nullCompletionRunner =
            Substitute.For<Func<ChatCompletionOptions, CancellationToken, Task<OllamaChatCompletion>>>();
        OllamaChatCompletion nullCompletion = new() { Content = "placeholder" };
        (typeof(OllamaChatCompletion).GetProperty(nameof(OllamaChatCompletion.Content))
            ?? throw new InvalidOperationException("Completion content property was not found."))
            .SetValue(nullCompletion, null);
        nullCompletionRunner.Invoke(Arg.Any<ChatCompletionOptions>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(nullCompletion));
        AgentRunResult nullContentFailure = await new CodingTaskDelegationService(
            modelClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(protocol, "model"), nullCompletionRunner),
            registry,
            orchestrator,
            settings)
            .RunDelegatedAsync("Inspect source files.");
        StringAssert.Contains(nullContentFailure.FinalResponse, "summary");

        IOllamaToolRegistry emptyRegistry = new OllamaToolRegistry([]);
        OllamaToolOrchestrator emptyOrchestrator = new(emptyRegistry);
        IOllamaProtocolAdapter failingProtocol = Substitute.For<IOllamaProtocolAdapter>();
        failingProtocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new InvalidOperationException("selection failed"));
        AgentRunResult fallbackFailure = await new CodingTaskDelegationService(
            modelClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(failingProtocol, "model")),
            emptyRegistry,
            emptyOrchestrator,
            settings)
            .RunDelegatedAsync("Run tests.");
        StringAssert.Contains(fallbackFailure.FinalResponse, "selection failed");

        IOllamaProtocolAdapter noneProtocol = Substitute.For<IOllamaProtocolAdapter>();
        noneProtocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = """{"toolName":"none","input":""}""" },
                    Done = true,
                },
            ]));
        modelClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<string>(new InvalidOperationException("summary failed")));
        AgentRunResult noFurtherTool = await new CodingTaskDelegationService(
            modelClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(noneProtocol, "model")),
            registry,
            orchestrator,
            settings)
            .RunDelegatedAsync("Inspect files.");
        StringAssert.Contains(noFurtherTool.FinalResponse, "without a successful tool step");
    }

    [TestMethod]
    public async Task WorkspaceValidation_CoversNullJsonAndRootDirectoryFallback()
    {
        using TestWorkspace workspace = new();
        await File.WriteAllTextAsync(workspace.GetPath("sample.txt"), "content");
        WorkspacePathPolicy policy = new(workspace.RootPath);

        ReadWorkspaceFileTool readTool = new(policy);
        Assert.IsFalse(readTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => readTool.ExecuteAsync("null"));

        RunDotnetBuildTool buildTool = new(policy);
        Assert.IsFalse(buildTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => buildTool.ExecuteAsync("null"));
        await File.WriteAllTextAsync(workspace.GetPath("build.csproj"), "<Project />");
        Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>> buildRunner =
            Substitute.For<Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>>>();
        buildRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((0, string.Empty, string.Empty)));
        await new RunDotnetBuildTool(policy, buildRunner).ExecuteAsync(
            """{"relativePath":"build.csproj","configuration":""}""");

        RunDotnetTestTool testTool = new(policy);
        Assert.IsFalse(testTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => testTool.ExecuteAsync("null"));
        await File.WriteAllTextAsync(workspace.GetPath("test.csproj"), "<Project />");
        Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>> testRunner =
            Substitute.For<Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>>>();
        testRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((0, string.Empty, string.Empty)));
        await new RunDotnetTestTool(policy, testRunner).ExecuteAsync(
            """{"relativePath":"test.csproj","configuration":""}""");

        WriteWorkspaceFileTool rootTool = new(new WorkspacePathPolicy("C:\\"));
        await Assert.ThrowsAsync<Exception>(() => rootTool.ExecuteAsync(
            """{"relativePath":"","content":"content"}"""));
        Assert.IsFalse(rootTool.Validate("null").IsValid);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => rootTool.ExecuteAsync("null"));
    }

    [TestMethod]
    public void Parser_HandlesAllInputKinds()
    {
        Assert.AreEqual(string.Empty, ToolCallParser.Parse("""{"toolName":"tool","input":""}""").Input);
        Assert.AreEqual(string.Empty, ToolCallParser.Parse("""{"toolName":"tool","input":null}""").Input);
        Assert.AreEqual("False", ToolCallParser.Parse("""{"toolName":"tool","input":false}""").Input);
        Assert.AreEqual("42", ToolCallParser.Parse("""{"toolName":42,"input":"value"}""").ToolName);
        MethodInfo normalizeInput = typeof(ToolCallParser).GetMethod(
            "NormalizeInput",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("NormalizeInput method was not found.");
        Assert.AreEqual(string.Empty, normalizeInput.Invoke(null, [default(JsonElement)]));
    }

    private sealed class TagsWithEmptyModelHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string content = request.RequestUri?.AbsolutePath == "/api/tags"
                ? """{"models":[{"name":"named"},{"model":"modeled"},{"name":null,"model":null}]}"""
                : """{"message":{"role":"assistant","content":"answer"},"done":true}""";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content),
            });
        }

    }

    [TestMethod]
    public async Task RunDotnetBuild_UsesInjectedResultAndTruncatesOutput()
    {
        using TestWorkspace workspace = new();
        await File.WriteAllTextAsync(workspace.GetPath("build.csproj"), "<Project />");
        Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>> processRunner =
            Substitute.For<Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>>>();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((1, " build output ", " build error ")));

        RunDotnetBuildTool tool = new(new WorkspacePathPolicy(workspace.RootPath), processRunner);
        OllamaToolResult result = await tool.ExecuteAsync(
            """{"relativePath":"build.csproj","configuration":"Release"}""");

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Output, "build output");
        StringAssert.Contains(result.Output, "build error");

        processRunner.ClearReceivedCalls();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((0, new string('x', 7_001), string.Empty)));
        result = await tool.ExecuteAsync("""{"relativePath":"build.csproj"}""");

        Assert.IsTrue(result.Success);
        Assert.AreEqual(7_000, result.Output.Length);
        await processRunner.Received(1).Invoke(
            Arg.Is<ProcessStartInfo>(info => info.ArgumentList.Contains("Debug")),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task RunDotnetBuild_PropagatesInjectedCancellation()
    {
        using TestWorkspace workspace = new();
        await File.WriteAllTextAsync(workspace.GetPath("build.csproj"), "<Project />");
        Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>> processRunner =
            Substitute.For<Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>>>();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<(int, string, string)>(new OperationCanceledException("cancelled")));

        RunDotnetBuildTool tool = new(new WorkspacePathPolicy(workspace.RootPath), processRunner);

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() =>
            tool.ExecuteAsync("""{"relativePath":"build.csproj"}"""));
    }

    [TestMethod]
    public async Task RunDotnetTest_ValidatesMissingFileAndUsesInjectedProcessOutcomes()
    {
        using TestWorkspace workspace = new();
        RunDotnetTestTool validationTool = new(new WorkspacePathPolicy(workspace.RootPath));
        var validation = validationTool.Validate("""{"relativePath":"missing.csproj"}""");
        Assert.IsFalse(validation.IsValid);

        await File.WriteAllTextAsync(workspace.GetPath("tests.csproj"), "<Project />");
        Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>> processRunner =
            Substitute.For<Func<ProcessStartInfo, CancellationToken, Task<(int ExitCode, string Output, string Error)>>>();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((1, " test output ", " test error ")));

        RunDotnetTestTool tool = new(new WorkspacePathPolicy(workspace.RootPath), processRunner);
        OllamaToolResult result = await tool.ExecuteAsync(
            """{"relativePath":"tests.csproj","configuration":"Release","framework":"net8.0"}""");

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Output, "test output");
        StringAssert.Contains(result.Output, "test error");
        await processRunner.Received(1).Invoke(
            Arg.Is<ProcessStartInfo>(info =>
                info.ArgumentList.Contains("Release")
                && info.ArgumentList.Contains("-f")
                && info.ArgumentList.Contains("net8.0")),
            Arg.Any<CancellationToken>());

        processRunner.ClearReceivedCalls();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((0, new string('x', 7_001), string.Empty)));
        result = await tool.ExecuteAsync("""{"relativePath":"tests.csproj"}""");
        Assert.IsTrue(result.Success);
        Assert.AreEqual(7_000, result.Output.Length);

        processRunner.ClearReceivedCalls();
        processRunner.Invoke(Arg.Any<ProcessStartInfo>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<(int, string, string)>(new OperationCanceledException("cancelled")));
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() =>
            tool.ExecuteAsync("""{"relativePath":"tests.csproj"}"""));
    }

    [TestMethod]
    public async Task WriteWorkspaceFile_ValidationRejectsExistingFileWithoutOverwrite()
    {
        using TestWorkspace workspace = new();
        string path = workspace.GetPath("existing.txt");
        await File.WriteAllTextAsync(path, "existing");
        WriteWorkspaceFileTool tool = new(new WorkspacePathPolicy(workspace.RootPath));

        var validation = tool.Validate("""{"relativePath":"existing.txt","content":"replacement"}""");

        Assert.IsFalse(validation.IsValid);
        StringAssert.Contains(string.Join(" ", validation.Errors), "overwrite=false");
    }

    private static void AssertConstructorRejectsNull(Type type, object?[] arguments)
    {
        ConstructorInfo constructor = type.GetConstructors()
            .Single(candidate => candidate.GetParameters().Length == arguments.Length);
        Assert.ThrowsExactly<TargetInvocationException>(() => constructor.Invoke(arguments));
    }
}
