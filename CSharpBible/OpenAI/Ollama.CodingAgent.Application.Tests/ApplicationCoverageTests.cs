using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Protocol.Models;
using Ollama.Tools;
using Ollama.CodingAgent.Application.Models;

namespace Ollama.CodingAgent.Application.Tests;

[TestClass]
public sealed class ApplicationCoverageTests
{
    [TestMethod]
    public async Task ApprovalService_RejectsInvalidDuplicateAndUnknownRequests()
    {
        AgentApprovalService service = new();
        Assert.ThrowsExactly<ArgumentException>(() => service.Resolve(string.Empty, true));
        Assert.IsFalse(service.Resolve("missing", true));

        AgentApprovalRequest request = CreateRequest("request");
        Task<bool> first = service.RequestApprovalAsync(request);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.RequestApprovalAsync(request));
        Assert.IsTrue(service.Resolve("request", false));
        Assert.IsFalse(await first);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => service.RequestApprovalAsync(new AgentApprovalRequest
        {
            Id = "invalid",
            Operation = string.Empty,
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow,
        }));
    }

    [TestMethod]
    public async Task JsonStore_HandlesMissingInvalidAndPersistedSnapshots()
    {
        using TestWorkspace workspace = new();
        string filePath = workspace.FilePath("nested/session.json");
        JsonAgentSessionStore store = new(filePath);

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => store.LoadAsync());
        await store.SaveAsync(new AgentSessionSnapshot
        {
            SessionId = "session",
            WorkspacePath = workspace.RootPath,
            Conversation =
            [
                new AgentConversationTurn
                {
                    Role = AgentConversationRole.User,
                    Content = "prompt",
                    CreatedAt = DateTimeOffset.UnixEpoch,
                },
            ],
        });
        Assert.AreEqual("session", (await store.LoadAsync()).SessionId);

        await File.WriteAllTextAsync(filePath, "null");
        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());
        await File.WriteAllTextAsync(filePath, """{"SessionId":"","WorkspacePath":"","Conversation":[]}""");
        await Assert.ThrowsExactlyAsync<InvalidDataException>(() => store.LoadAsync());
        Assert.ThrowsExactly<ArgumentException>(() => new JsonAgentSessionStore(string.Empty));
    }

    [TestMethod]
    public async Task SessionViewModel_HandlesFailureCancellationClearAndReload()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        AgentApprovalService approvals = new();
        AgentSessionViewModel viewModel = new(sessionService, store, approvals, "session", ".");

        sessionService.RunAsync("fail", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<AgentRunResult>(new InvalidOperationException("failed")));
        viewModel.Prompt = "fail";
        await viewModel.SubmitCommand.ExecuteAsync(null);
        Assert.AreEqual("Agent request failed.", viewModel.Status);
        Assert.AreEqual("failed", viewModel.ErrorMessage);
        Assert.AreEqual(2, viewModel.Conversation.Count);

        sessionService.RunAsync("cancel", Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.Delay(Timeout.InfiniteTimeSpan, callInfo.ArgAt<CancellationToken>(1)));
        viewModel.Prompt = "cancel";
        Task submission = viewModel.SubmitCommand.ExecuteAsync(null);
        await Task.Delay(10);
        viewModel.CancelCommand.Execute(null);
        await submission;
        Assert.AreEqual("Cancelling agent request.", viewModel.Status);

        await viewModel.ClearCommand.ExecuteAsync(null);
        Assert.AreEqual(0, viewModel.Conversation.Count);
        Assert.AreEqual("Session cleared.", viewModel.Status);
        await store.Received().SaveAsync(Arg.Any<AgentSessionSnapshot>(), Arg.Any<CancellationToken>());

        store.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentSessionSnapshot
        {
            SessionId = "session",
            WorkspacePath = ".",
            Conversation = [new AgentConversationTurn { Role = AgentConversationRole.System, Content = "restored", CreatedAt = DateTimeOffset.UtcNow }],
        }));
        await viewModel.ReloadCommand.ExecuteAsync(null);
        Assert.AreEqual("restored", viewModel.Conversation[0].Content);

        Task<bool> pending = approvals.RequestApprovalAsync(CreateRequest("approval"));
        Assert.IsTrue(viewModel.ResolveApproval("approval", true));
        Assert.IsTrue(await pending);
        Assert.AreEqual("Operation approved.", viewModel.Status);
        Assert.IsFalse(viewModel.ResolveApproval("missing", false));
    }

    [TestMethod]
    public async Task SessionServiceAndComposition_UseRuntimeAndPersistedSessionContracts()
    {
        IAgentModelClient client = Substitute.For<IAgentModelClient>();
        client.CompleteAsync(Arg.Any<System.Collections.Generic.IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("answer"));
        AgentSessionService sessionService = new(new AgentRunner(client, new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1)));
        Assert.AreEqual("answer", (await sessionService.RunAsync("prompt")).FinalResponse);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => sessionService.RunAsync(string.Empty));
        ServiceCollection services = new();
        services.AddOllamaCodingAgent(OllamaAgentCliOptions.Parse(["--workspace-root", "."]));
        services.AddAgentApplication(".", "composition");
        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Assert.IsNotNull(provider.GetRequiredService<AgentSessionViewModel>());
        Assert.ThrowsExactly<ArgumentException>(() => services.AddAgentApplication(".", string.Empty));
    }

    [TestMethod]
    public async Task SessionService_UsesDelegatedToolDefinitionsThroughApplicationComposition()
    {
        using TestWorkspace workspace = new();
        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse(
        [
            "--endpoint", "http://localhost:11434",
            "--model", "model",
            "--workspace-root", workspace.RootPath,
        ]);
        ServiceCollection services = new();
        services.AddOllamaCodingAgent(options);
        services.AddAgentApplication(workspace.RootPath, "tool-session");
        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);

        IOllamaProtocolAdapter protocol = Substitute.For<IOllamaProtocolAdapter>();
        protocol.ChatStreamingAsync(Arg.Any<OllamaChatRequest>(), Arg.Any<CancellationToken>())
            .Returns(CreateResponseStream());

        OllamaChatClient chatClient = new(protocol, "model");
        OllamaToolChatRunnerAdapter toolRunner = new(chatClient);
        IOllamaToolRegistry registry = new CodingDelegationToolRegistryFactory(
            new WorkspacePathPolicy(workspace.RootPath)).CreateRegistry();
        CodingTaskDelegationService delegationService = new(
            Substitute.For<IAgentModelClient>(),
            toolRunner,
            registry,
            new OllamaToolOrchestrator(registry),
            new OllamaAgentRuntimeSettings(TimeSpan.FromSeconds(1), 0, 1));
        AgentSessionService service = new(
            provider.GetRequiredService<AgentRunner>(),
            delegationService);

        await service.RunAsync("Inspect the workspace.");

        protocol.Received().ChatStreamingAsync(
            Arg.Is<OllamaChatRequest>(request => request.Tools != null && request.Tools.Count > 0),
            Arg.Any<CancellationToken>());
    }

    private static async IAsyncEnumerable<OllamaChatResponseChunk> CreateResponseStream()
    {
        yield return new OllamaChatResponseChunk
        {
            Message = new OllamaChatMessage
            {
                Role = "assistant",
                Content = "{\"toolName\":\"none\",\"input\":{}}",
            },
            Done = true,
        };

        await Task.Yield();
    }

    [TestMethod]
    public async Task SessionViewModel_RejectsMaintenanceCommandsDuringActiveRun()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        TaskCompletionSource<AgentRunResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        sessionService.RunAsync("running", Arg.Any<CancellationToken>()).Returns(completion.Task);
        AgentSessionViewModel viewModel = new(sessionService, store, new AgentApprovalService(), "session", ".");

        viewModel.Prompt = "running";
        Task submission = viewModel.SubmitCommand.ExecuteAsync(null);
        await Task.Delay(10);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => viewModel.ClearCommand.ExecuteAsync(null));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => viewModel.ReloadCommand.ExecuteAsync(null));

        completion.SetResult(new AgentRunResult
        {
            FinalResponse = "done",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        });
        await submission;
        Assert.IsFalse(viewModel.SubmitCommand.CanExecute(null));
        Assert.IsFalse(viewModel.CancelCommand.CanExecute(null));
    }

    [TestMethod]
    public async Task JsonStore_RemovesTemporaryFileWhenReplacementFails()
    {
        using TestWorkspace workspace = new();
        string targetPath = workspace.FilePath("session.json");
        Directory.CreateDirectory(targetPath);
        JsonAgentSessionStore store = new(targetPath);

        await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(() => store.SaveAsync(new AgentSessionSnapshot
        {
            SessionId = "session",
            WorkspacePath = workspace.RootPath,
        }));

        Assert.AreEqual(0, Directory.GetFiles(workspace.RootPath, "session.json.*.tmp").Length);
    }

    [TestMethod]
    public async Task SessionViewModel_CancellationUpdatesItsWorkflowState()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        TaskCompletionSource<AgentRunResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        sessionService.RunAsync("cancel", Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                CancellationToken cancellationToken = callInfo.ArgAt<CancellationToken>(1);
                cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
                return completion.Task;
            });
        AgentApprovalService approvals = new();
        AgentSessionViewModel viewModel = new(sessionService, store, approvals, "session", ".");

        Assert.AreEqual(0, viewModel.PendingApprovals.Count);
        viewModel.Prompt = "cancel";
        Assert.IsTrue(viewModel.SubmitCommand.CanExecute(null));
        Task submission = viewModel.SubmitCommand.ExecuteAsync(null);
        await Task.Delay(10);
        Assert.IsTrue(viewModel.CancelCommand.CanExecute(null));
        viewModel.CancelCommand.Execute(null);
        await submission;

        Assert.AreEqual("Agent request cancelled.", viewModel.Status);
        Assert.AreEqual(1, viewModel.Conversation.Count);

        Task<bool> rejection = approvals.RequestApprovalAsync(CreateRequest("rejected"));
        Assert.IsTrue(viewModel.ResolveApproval("rejected", approved: false));
        Assert.IsFalse(await rejection);
        Assert.AreEqual("Operation rejected.", viewModel.Status);
    }

    [TestMethod]
    public async Task SessionViewModel_AwaitsCompletedAndFaultedAsynchronousSaves()
    {
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        sessionService.RunAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new AgentRunResult
        {
            FinalResponse = "answer",
            IterationsUsed = 1,
            RetryAttemptsUsed = 0,
            FinalizedWithMarker = false,
        }));

        IAgentSessionStore successfulStore = Substitute.For<IAgentSessionStore>();
        TaskCompletionSource successfulSave = new(TaskCreationOptions.RunContinuationsAsynchronously);
        successfulStore.SaveAsync(Arg.Any<AgentSessionSnapshot>(), Arg.Any<CancellationToken>()).Returns(successfulSave.Task);
        AgentSessionViewModel successfulViewModel = new(sessionService, successfulStore, new AgentApprovalService(), "session", ".");
        successfulViewModel.Prompt = "answer";
        Task successfulSubmission = successfulViewModel.SubmitCommand.ExecuteAsync(null);
        await Task.Delay(10);
        Assert.IsFalse(successfulSubmission.IsCompleted);
        successfulSave.SetResult();
        await successfulSubmission;

        IAgentSessionStore faultedStore = Substitute.For<IAgentSessionStore>();
        TaskCompletionSource faultedSave = new(TaskCreationOptions.RunContinuationsAsynchronously);
        faultedStore.SaveAsync(Arg.Any<AgentSessionSnapshot>(), Arg.Any<CancellationToken>()).Returns(faultedSave.Task);
        AgentSessionViewModel faultedViewModel = new(sessionService, faultedStore, new AgentApprovalService(), "session", ".");
        faultedViewModel.Prompt = "answer";
        Task faultedSubmission = faultedViewModel.SubmitCommand.ExecuteAsync(null);
        await Task.Delay(10);
        Assert.IsFalse(faultedSubmission.IsCompleted);
        faultedSave.SetException(new IOException("save failed"));
        await Assert.ThrowsExactlyAsync<IOException>(() => faultedSubmission);
    }

    [TestMethod]
    public void ConstructorsAndCommandPredicates_RejectNullDependenciesAndCoverStates()
    {
        ConstructorInfo sessionServiceConstructor = typeof(AgentSessionService).GetConstructor([typeof(AgentRunner)])!;
        Assert.ThrowsExactly<TargetInvocationException>(() => sessionServiceConstructor.Invoke([null]));

        ConstructorInfo viewModelConstructor = typeof(AgentSessionViewModel).GetConstructor(
        [
            typeof(IAgentSessionService),
            typeof(IAgentSessionStore),
            typeof(IAgentApprovalService),
            typeof(string),
            typeof(string),
        ])!;
        IAgentSessionService sessionService = Substitute.For<IAgentSessionService>();
        IAgentSessionStore store = Substitute.For<IAgentSessionStore>();
        IAgentApprovalService approvals = Substitute.For<IAgentApprovalService>();
        Assert.ThrowsExactly<TargetInvocationException>(() => viewModelConstructor.Invoke([null, store, approvals, "session", "."]));
        Assert.ThrowsExactly<TargetInvocationException>(() => viewModelConstructor.Invoke([sessionService, null, approvals, "session", "."]));
        Assert.ThrowsExactly<TargetInvocationException>(() => viewModelConstructor.Invoke([sessionService, store, null, "session", "."]));

        AgentSessionViewModel viewModel = new(sessionService, store, approvals, "session", ".");
        MethodInfo canSubmit = typeof(AgentSessionViewModel).GetMethod("CanSubmit", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo canCancel = typeof(AgentSessionViewModel).GetMethod("CanCancel", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.IsFalse((bool)canSubmit.Invoke(viewModel, null)!);
        Assert.IsFalse((bool)canCancel.Invoke(viewModel, null)!);
        viewModel.Prompt = "prompt";
        Assert.IsTrue((bool)canSubmit.Invoke(viewModel, null)!);
        viewModel.IsRunning = true;
        Assert.IsFalse((bool)canSubmit.Invoke(viewModel, null)!);
        Assert.IsTrue((bool)canCancel.Invoke(viewModel, null)!);
    }

    private static AgentApprovalRequest CreateRequest(string id)
        => new()
        {
            Id = id,
            Operation = "operation",
            Preview = "preview",
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
