using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class FinalCoreCoverageTests
{
    [TestMethod]
    public void ConstructorsAndContracts_RejectMissingDependenciesAndCoverEmptySignals()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        OllamaAgentRuntimeSettings settings = new(TimeSpan.FromSeconds(1), 0, 1);
        AssertConstructorRejectsNull(typeof(AgentRunner), [null, settings, null]);
        AssertConstructorRejectsNull(typeof(AgentRunner), [modelClient, null, null]);
        AssertConstructorRejectsNull(typeof(CodingDelegationToolRegistryFactory), [null]);
        AssertConstructorRejectsNull(typeof(LocalWikiSearchTool), [null]);
        AssertConstructorRejectsNull(typeof(LocalWikiWriteTool), [null, null]);
        AssertConstructorRejectsNull(typeof(OllamaBaselineService), [null, "model"]);
        AssertConstructorRejectsNull(typeof(OllamaChatModelClient), [null]);
        AssertConstructorRejectsNull(typeof(ListWorkspaceFilesTool), [null]);
        AssertConstructorRejectsNull(typeof(ReadWorkspaceFileTool), [null]);
        AssertConstructorRejectsNull(typeof(WriteWorkspaceFileTool), [null]);
        AssertConstructorRejectsNull(typeof(RunDotnetBuildTool), [null]);
        AssertConstructorRejectsNull(typeof(RunDotnetTestTool), [null]);
        AssertConstructorRejectsNull(typeof(WebLookupTool), [null, new WebKnowledgePolicy(), null]);
        using HttpClient constructorClient = new();
        AssertConstructorRejectsNull(typeof(WebLookupTool), [constructorClient, null, null]);
        Assert.ThrowsExactly<ArgumentException>(() => new PlanStateStore(string.Empty));
        AssertConstructorRejectsNull(typeof(GoalContract), ["goal", null]);
        AssertConstructorRejectsNull(typeof(AgentEvaluationScenario), ["id", "description", null]);

        GoalContract goal = new("Implement a reliable build pipeline", []);
        PlannedSubtask subtask = new("id", "title", "rationale");
        Assert.IsTrue(GoalDriftAnalyzer.IsDriftDetected(goal, subtask, string.Empty));
        AssertStaticMethodRejectsNull(typeof(GoalDriftAnalyzer), "IsDriftDetected", [null, subtask, "output"]);
        AssertStaticMethodRejectsNull(typeof(GoalDriftAnalyzer), "IsDriftDetected", [goal, null, "output"]);
        MethodInfo? driftMethod = typeof(GoalDriftAnalyzer).GetMethod("IsDriftDetected");
        if (driftMethod is null)
        {
            throw new InvalidOperationException("Goal-drift method was not found.");
        }

        Assert.IsTrue(driftMethod.Invoke(null, [goal, subtask, null]) is true);
        Assert.IsTrue(GoalDriftAnalyzer.IsDriftDetected(goal, subtask, "failed"));
        Assert.IsFalse(GoalDriftAnalyzer.IsDriftDetected(goal, subtask, "completed successfully"));

        LocalWikiWritePolicy policy = new();
        Assert.IsFalse(policy.TryValidate(new LocalKnowledgeEntry
        {
            Id = "id",
            Title = "title",
            Summary = "summary",
            CitationUrl = "not a uri",
        }, out _));
    }

    [TestMethod]
    public async Task AgentRunner_CoversCancellationRetryThinkingAndFinalFailure()
    {
        IAgentModelClient retryingClient = Substitute.For<IAgentModelClient>();
        using CancellationTokenSource timeoutCancellation = new();
        timeoutCancellation.Cancel();
        retryingClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromCanceled<string>(timeoutCancellation.Token),
                Task.FromResult("answer"));
        AgentRunResult retryResult = await new AgentRunner(retryingClient, new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 1, 1, retryBackoff: TimeSpan.Zero))
            .RunAsync(new AgentRunRequest { Prompt = "prompt", SystemPrompt = "system" });
        Assert.AreEqual(1, retryResult.RetryAttemptsUsed);

        IThinkingAgentModelClient thinkingClient = Substitute.For<IThinkingAgentModelClient>();
        thinkingClient.CompleteDetailedAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AgentCompletion { Content = "answer", Thinking = ["", " ", "kept"] }));
        AgentRunResult thinkingResult = await new AgentRunner(thinkingClient, new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1))
            .RunAsync(new AgentRunRequest { Prompt = "prompt", SystemPrompt = "system" });
        CollectionAssert.AreEqual(new[] { "kept" }, (System.Collections.ICollection)thinkingResult.Thinking);

        IAgentModelClient failingClient = Substitute.For<IAgentModelClient>();
        failingClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<string>(new InvalidOperationException("failed")));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            new AgentRunner(failingClient, new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1))
                .RunAsync(new AgentRunRequest { Prompt = "prompt", SystemPrompt = "system" }));
    }

    [TestMethod]
    public async Task KnowledgeBaseAndWikiTools_CoverParsingAndPersistenceEdges()
    {
        using TestWorkspace workspace = new();
        string databasePath = workspace.GetPath("knowledge.json");
        LocalKnowledgeBaseStore store = new(databasePath, new LocalWikiWritePolicy());
        await File.WriteAllTextAsync(databasePath, "null");
        Assert.AreEqual(0, (await store.LoadAllAsync()).Count);

        LocalWikiSearchTool search = new(store);
        Assert.IsFalse(search.Validate("""{"query":"","maxResults":1}""").IsValid);
        Assert.IsFalse(search.Validate("""{"query":"term","maxResults":0}""").IsValid);
        Assert.IsFalse(search.Validate("invalid json").IsValid);

        LocalWikiWriteTool write = new(store, new LocalWikiWritePolicy());
        Assert.IsFalse(write.Validate("""{"id":"","title":"title","summary":"summary"}""").IsValid);
        Assert.IsFalse(write.Validate("invalid json").IsValid);
        Assert.IsFalse(write.Validate("""{"relativePath":"sample.txt","content":"new","overwrite":false}""").IsValid);
        OllamaToolResult written = await write.ExecuteAsync(
            """{"id":" entry ","title":" Entry ","summary":" Summary ","source":" source ","citationUrl":"https://learn.microsoft.com/en-us/dotnet","tags":[" tag ","TAG",""]}""");
        Assert.IsTrue(written.Success);
        LocalKnowledgeEntry entry = (await store.LoadAllAsync())[0];
        Assert.AreEqual("source", entry.Source);
        Assert.AreEqual("https://learn.microsoft.com/en-us/dotnet", entry.CitationUrl);
        CollectionAssert.AreEqual(new[] { "tag" }, (System.Collections.ICollection)entry.Tags);

        string vault = workspace.GetPath("vault");
        Directory.CreateDirectory(Path.Combine(vault, ".hidden"));
        await File.WriteAllTextAsync(
            Path.Combine(vault, ".hidden", "long.md"),
            "---\ntitle: \"\"Quoted title\"\"\ntags: [ one , 'two' ]\n---\n" + new string('x', 8_100));
        LocalWikiImportResult imported = await new LocalWikiMarkdownImporter().ImportAsync(vault, store);
        Assert.AreEqual(1, imported.ImportedCount);
        LocalKnowledgeEntry? importedEntry = (await store.LoadAllAsync()).FirstOrDefault(item => item.Id == ".hidden/long.md");
        if (importedEntry is null)
        {
            throw new AssertFailedException("Expected the imported wiki entry.");
        }

        Assert.AreEqual("Quoted title", importedEntry.Title);
        Assert.AreEqual(8_000, importedEntry.Summary.Length);
        CollectionAssert.Contains((System.Collections.ICollection)importedEntry.Tags, "one");
    }

    [TestMethod]
    public async Task WorkspaceTools_CoverMalformedAndBoundaryInputs()
    {
        using TestWorkspace workspace = new();
        await File.WriteAllLinesAsync(workspace.GetPath("sample.txt"), ["one", "two"]);
        WorkspacePathPolicy policy = new(workspace.RootPath);
        ListWorkspaceFilesTool list = new(policy);
        ReadWorkspaceFileTool read = new(policy);
        WriteWorkspaceFileTool write = new(policy);
        RunDotnetBuildTool build = new(policy);
        RunDotnetTestTool test = new(policy);

        Assert.IsFalse(list.Validate("invalid json").IsValid);
        Assert.IsFalse(read.Validate("""{"relativePath":""}""").IsValid);
        Assert.IsFalse(read.Validate("invalid json").IsValid);
        Assert.IsFalse(write.Validate("""{"relativePath":"file.txt","content":null}""").IsValid);
        Assert.IsFalse(write.Validate("invalid json").IsValid);
        Assert.IsFalse(build.Validate("""{"relativePath":"file.txt"}""").IsValid);
        Assert.IsFalse(build.Validate("invalid json").IsValid);
        Assert.IsFalse(test.Validate("""{"relativePath":"file.txt"}""").IsValid);
        Assert.IsFalse(test.Validate("invalid json").IsValid);

        Assert.IsFalse((await write.ExecuteAsync("""{"relativePath":"sample.txt","content":"new","overwrite":false}""")).Success);
        Assert.IsFalse((await read.ExecuteAsync("""{"relativePath":"missing.txt"}""")).Success);
        Assert.AreEqual(string.Empty, (await read.ExecuteAsync("""{"relativePath":"sample.txt","startLine":3,"lineCount":1}""")).Output);
        Assert.IsFalse((await list.ExecuteAsync("""{"relativePath":"missing"}""")).Success);
        Assert.IsFalse((await build.ExecuteAsync("""{"relativePath":"missing.csproj"}""")).Success);
        Assert.IsFalse((await test.ExecuteAsync("""{"relativePath":"missing.csproj"}""")).Success);

        Assert.AreEqual($"output{Environment.NewLine}error", InvokePrivate<string>(typeof(RunDotnetBuildTool), "CombineOutput", " output ", " error "));
        Assert.AreEqual(string.Empty, InvokePrivate<string>(typeof(RunDotnetTestTool), "CombineOutput", string.Empty, string.Empty));
    }

    [TestMethod]
    public void ParsersAndDelegationFormatting_CoverAlternativeResults()
    {
        Assert.ThrowsExactly<ArgumentException>(() => ToolCallParser.Parse(string.Empty));
        Assert.ThrowsExactly<InvalidOperationException>(() => ToolCallParser.Parse("[1,2]"));
        Assert.ThrowsExactly<InvalidOperationException>(() => ToolCallParser.Parse("""{"toolName":""}"""));
        Assert.AreEqual("value", ToolCallParser.Parse("""{"name":"tool","params":"value"}""").Input);
        Assert.AreEqual("False", ToolCallParser.Parse("""{"name":"tool","input":false}""").Input);
        Assert.AreEqual("{}", ToolCallParser.Parse("""{"name":"tool"}""").Input);

        DelegatedToolStep successful = new()
        {
            StepIndex = 2,
            ToolName = "read_workspace_file",
            Success = true,
            Input = new string('i', 1_201),
            Output = new string('o', 4_001),
            Duration = TimeSpan.FromMilliseconds(5),
        };
        DelegatedToolStep failed = new()
        {
            StepIndex = 1,
            ToolName = "none",
            Success = false,
            Output = "output",
        };
        string emptyReport = InvokePrivate<string>(typeof(CodingTaskDelegationService), "BuildDelegationReport", Array.Empty<DelegatedToolStep>(), false);
        string detailedReport = InvokePrivate<string>(typeof(CodingTaskDelegationService), "BuildDelegationReport", new[] { successful, failed }, true);
        Assert.AreEqual("No delegated tool steps were executed.", emptyReport);
        StringAssert.Contains(detailedReport, "Duration:");
        StringAssert.Contains(detailedReport, "...");
        Assert.AreEqual("abc...", InvokePrivate<string>(typeof(CodingTaskDelegationService), "Truncate", "abcdef", 3));
        Assert.AreEqual("value", InvokePrivate<string>(typeof(CodingTaskDelegationService), "Truncate", "value", 5));

        OllamaToolInvocationResult successfulResult = new() { ToolName = "tool", Success = true, Output = "output", Error = " " };
        OllamaToolInvocationResult bothFailureValues = new() { ToolName = "tool", Success = false, Output = "output", Error = "error" };
        OllamaToolInvocationResult onlyError = new() { ToolName = "tool", Success = false, Output = string.Empty, Error = "error" };
        OllamaToolInvocationResult onlyOutput = new() { ToolName = "tool", Success = false, Output = "output", Error = string.Empty };
        Assert.AreEqual("output", InvokePrivate<string>(typeof(CodingTaskDelegationService), "NormalizeToolOutput", successfulResult));
        Assert.AreEqual("error\noutput", InvokePrivate<string>(typeof(CodingTaskDelegationService), "NormalizeToolOutput", bothFailureValues));
        Assert.AreEqual("error", InvokePrivate<string>(typeof(CodingTaskDelegationService), "NormalizeToolOutput", onlyError));
        Assert.AreEqual("output", InvokePrivate<string>(typeof(CodingTaskDelegationService), "NormalizeToolOutput", onlyOutput));
    }

    [TestMethod]
    public async Task BaselineAndWebLookup_CoverEarlyAndErrorOutcomes()
    {
        IOllamaBaselineClient unavailableClient = Substitute.For<IOllamaBaselineClient>();
        unavailableClient.GetAvailableModelsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<string>>(["other"]));
        OllamaBaselineCheckResult unavailable = await new OllamaBaselineService(unavailableClient, "model").RunSmokeAsync("prompt");
        Assert.IsFalse(unavailable.Success);
        await unavailableClient.DidNotReceive().CompleteChatAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        using HttpClient client = new(new ThrowingHandler());
        WebLookupTool lookup = new(client, new WebKnowledgePolicy());
        Assert.IsFalse(lookup.Validate(string.Empty).IsValid);
        Assert.IsFalse(lookup.Validate("""{"source":"wikipedia","query":""}""").IsValid);
        Assert.IsFalse(lookup.Validate("invalid json").IsValid);

        using HttpClient responseClient = new(new StaticResponseHandler());
        WebLookupTool rejectedLookup = new(responseClient, new WebKnowledgePolicy(), static _ => false);
        Assert.IsFalse((await rejectedLookup.ExecuteAsync("""{"source":"wikipedia","query":"query"}""")).Success);

        IOllamaBaselineClient cancelledClient = Substitute.For<IOllamaBaselineClient>();
        cancelledClient.GetAvailableModelsAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<string>>(["model"]));
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        cancelledClient.CompleteChatAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromCanceled<OllamaChatCompletion>(cancellation.Token));
        await Assert.ThrowsExactlyAsync<TaskCanceledException>(() =>
            new OllamaBaselineService(cancelledClient, "model").RunSmokeAsync("prompt"));
    }

    [TestMethod]
    public async Task DelegationService_CoversNoReadyAndNoFurtherToolBranches()
    {
        using TestWorkspace workspace = new();
        IOllamaToolRegistry registry = new CodingDelegationToolRegistryFactory(new WorkspacePathPolicy(workspace.RootPath)).CreateRegistry();
        OllamaToolOrchestrator orchestrator = new(registry);
        IAgentModelClient summaryClient = Substitute.For<IAgentModelClient>();
        summaryClient.CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("summary"));
        IOllamaProtocolAdapter protocol = Substitute.For<IOllamaProtocolAdapter>();
        protocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(AsyncSequence.From(
            [
                new OllamaChatResponseChunk
                {
                    Message = new OllamaChatMessage { Role = "assistant", Content = """{"toolName":"none","input":""}""" },
                    Done = true,
                },
            ]));
        CodingTaskDelegationService service = new(
            summaryClient,
            new OllamaToolChatRunnerAdapter(new OllamaChatClient(protocol, "model")),
            registry,
            orchestrator,
            new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1));

        AgentRunResult result = await service.RunDelegatedAsync("Inspect the workspace.");
        StringAssert.Contains(result.FinalResponse, "none");

        PlanState completedPlan = SubtaskPlanner.CreateInitialPlan("Inspect the workspace.");
        foreach (PlannedSubtask subtask in completedPlan.Subtasks)
        {
            subtask.Status = PlannedSubtaskStatus.Done;
        }

        MethodInfo? runSteps = typeof(CodingTaskDelegationService).GetMethod("RunDelegatedStepsAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        if (runSteps is null)
        {
            throw new InvalidOperationException("Delegated-step method was not found.");
        }

        object? invocationResult = runSteps.Invoke(service, [completedPlan, CancellationToken.None]);
        if (invocationResult is not Task<List<DelegatedToolStep>> steps)
        {
            throw new InvalidOperationException("Delegated-step invocation did not return the expected task.");
        }

        Assert.AreEqual(0, (await steps).Count);
    }

    [TestMethod]
    public async Task PlanStateStoreAndImporter_CoverInvalidPersistedInputs()
    {
        using TestWorkspace workspace = new();
        string planPath = workspace.GetPath("plan.json");
        PlanStateStore store = new(planPath);
        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => store.LoadAsync());
        await File.WriteAllTextAsync(planPath, "null");
        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());
        await File.WriteAllTextAsync(planPath, """{"Objective":"","Subtasks":[]}""");
        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());

        string vault = workspace.GetPath("vault");
        Directory.CreateDirectory(vault);
        await File.WriteAllTextAsync(Path.Combine(vault, "page.md"), "# Page\nbody");
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() =>
            new LocalWikiMarkdownImporter().ImportAsync(vault, new LocalKnowledgeBaseStore(workspace.GetPath("knowledge.json")), cancellation.Token));
    }

    private static T InvokePrivate<T>(Type type, string methodName, params object?[] arguments)
    {
        MethodInfo? method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method is null)
        {
            throw new InvalidOperationException($"Method '{methodName}' was not found.");
        }

        object? result = method.Invoke(null, arguments);
        return result is T typedResult
            ? typedResult
            : throw new InvalidOperationException($"Method '{methodName}' returned an unexpected result.");
    }

    private static void AssertConstructorRejectsNull(Type type, object?[] arguments)
        => Assert.ThrowsExactly<TargetInvocationException>(() => type.GetConstructors()
            .Single(constructor => constructor.GetParameters().Length == arguments.Length)
            .Invoke(arguments));

    private static void AssertStaticMethodRejectsNull(Type type, string methodName, object?[] arguments)
    {
        MethodInfo? method = type.GetMethod(methodName);
        if (method is null)
        {
            throw new InvalidOperationException($"Method '{methodName}' was not found.");
        }

        Assert.ThrowsExactly<TargetInvocationException>(() => method.Invoke(null, arguments));
    }

    private sealed class ThrowingHandler : System.Net.Http.HttpMessageHandler
    {
        protected override Task<System.Net.Http.HttpResponseMessage> SendAsync(
            System.Net.Http.HttpRequestMessage request,
            CancellationToken cancellationToken)
            => throw new InvalidOperationException("unexpected request");
    }

    private sealed class StaticResponseHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("response"),
            });
    }
}
