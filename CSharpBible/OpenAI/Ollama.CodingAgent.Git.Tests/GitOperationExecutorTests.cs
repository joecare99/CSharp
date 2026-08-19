using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using NSubstitute;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;
using Ollama.CodingAgent.Application.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Git.Tests;

[TestClass]
public sealed class GitOperationExecutorTests
{
    [TestMethod]
    public async Task ExecuteAsync_WhenDenied_DoesNotStageFile()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.WriteAllText(Path.Combine(repository.WorkspacePath, "new.txt"), "new");
        IAgentApprovalService approvalService = Substitute.For<IAgentApprovalService>();
        approvalService.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(false);

        GitOperationResult result = await new GitOperationExecutor(approvalService)
            .ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(["new.txt"]));

        Assert.IsFalse(result.WasApproved);
        Assert.IsFalse(result.WasApplied);
        Assert.IsNull(result.ErrorMessage);
        using Repository nativeRepository = new(repository.WorkspacePath);
        Assert.IsFalse(nativeRepository.RetrieveStatus()["new.txt"].State.HasFlag(FileStatus.NewInIndex));
        await approvalService.Received(1).RequestApprovalAsync(Arg.Is<AgentApprovalRequest>(request => request.Preview == result.Preview.Render()), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenApproved_StagesFile()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.WriteAllText(Path.Combine(repository.WorkspacePath, "new.txt"), "new");
        IAgentApprovalService approvalService = Substitute.For<IAgentApprovalService>();
        approvalService.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);

        GitOperationResult result = await new GitOperationExecutor(approvalService)
            .ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(["new.txt"]));

        Assert.IsTrue(result.WasApproved);
        Assert.IsTrue(result.WasApplied);
        using Repository nativeRepository = new(repository.WorkspacePath);
        Assert.IsTrue(nativeRepository.RetrieveStatus()["new.txt"].State.HasFlag(FileStatus.NewInIndex));
    }

    [TestMethod]
    [DataRow("bad..branch")]
    [DataRow("bad branch")]
    [DataRow("refs/heads/main")]
    [DataRow("/branch")]
    [DataRow("branch/")]
    [DataRow("branch.")]
    [DataRow("a@{b}")]
    [DataRow("a\\b")]
    [DataRow("a~b")]
    [DataRow("a^b")]
    [DataRow("a:b")]
    [DataRow("a?b")]
    [DataRow("a*b")]
    [DataRow("a[b")]
    [DataRow("a//b")]
    [DataRow("a.lock")]
    [DataRow("\u0001")]
    [DataRow("\t")]
    public async Task ExecuteAsync_RejectsUnsafeBranchBeforeApproval(string branchName)
    {
        using GitTestRepository repository = GitTestRepository.Create();
        IAgentApprovalService approvalService = Substitute.For<IAgentApprovalService>();

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => new GitOperationExecutor(approvalService)
            .ExecuteAsync(repository.WorkspacePath, new CreateGitBranchOperation(branchName)));

        await approvalService.DidNotReceive().RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public void ReferenceNameValidation_ExaminesAsciiCharacterRules()
    {
        MethodInfo validator = typeof(GitOperationExecutor).GetMethod(
            "IsSafeReferenceName",
            BindingFlags.NonPublic | BindingFlags.Static)!;

        for (int character = 0; character <= 0x7F; character++)
        {
            _ = validator.Invoke(null, [$"a{(char)character}b"]);
        }
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenCancelled_DoesNotMutate()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.WriteAllText(Path.Combine(repository.WorkspacePath, "new.txt"), "new");
        IAgentApprovalService approvalService = Substitute.For<IAgentApprovalService>();
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => new GitOperationExecutor(approvalService)
            .ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(["new.txt"]), cancellation.Token));

        using Repository nativeRepository = new(repository.WorkspacePath);
        Assert.IsTrue(nativeRepository.RetrieveStatus()["new.txt"].State.HasFlag(FileStatus.NewInWorkdir));
        await approvalService.DidNotReceive().RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenCancelledWhileApprovalIsPending_DoesNotMutate()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.WriteAllText(Path.Combine(repository.WorkspacePath, "new.txt"), "new");
        AgentApprovalService approvalService = new();
        using CancellationTokenSource cancellation = new();

        Task<GitOperationResult> execution = new GitOperationExecutor(approvalService)
            .ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(["new.txt"]), cancellation.Token);

        Assert.AreEqual(1, approvalService.PendingRequests.Count);
        cancellation.Cancel();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => execution);
        Assert.AreEqual(0, approvalService.PendingRequests.Count);
        using Repository nativeRepository = new(repository.WorkspacePath);
        Assert.IsTrue(nativeRepository.RetrieveStatus()["new.txt"].State.HasFlag(FileStatus.NewInWorkdir));
        Assert.IsFalse(nativeRepository.RetrieveStatus()["new.txt"].State.HasFlag(FileStatus.NewInIndex));
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenPushIsNonFastForward_ReturnsRedactedFailureWithoutChangingRemote()
    {
        using GitTestRepository repository = GitTestRepository.CreateWithBareRemote();
        repository.CreateRemoteDivergence();
        using (Repository localRepository = new(repository.WorkspacePath))
        {
            File.WriteAllText(Path.Combine(repository.WorkspacePath, "local.txt"), "local change");
            Commands.Stage(localRepository, "local.txt");
            Signature signature = new("Local User", "local@example.invalid", DateTimeOffset.UtcNow);
            localRepository.Commit("Local commit", signature, signature);
        }

        IAgentApprovalService approvalService = Substitute.For<IAgentApprovalService>();
        approvalService.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);

        GitOperationResult result = await new GitOperationExecutor(approvalService).ExecuteAsync(
            repository.WorkspacePath,
            new PushGitOperation("origin", "master"));

        Assert.IsTrue(result.WasApproved);
        Assert.IsFalse(result.WasApplied);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
        Assert.IsFalse((result.ErrorMessage ?? string.Empty).Contains("remote@example.invalid", StringComparison.Ordinal));
        using Repository verificationRepository = new(repository.WorkspacePath);
        using Repository bareRemote = new(repository.BareRemotePath);
        Assert.AreNotEqual(verificationRepository.Head.Tip.Sha, bareRemote.Head.Tip.Sha);
    }
}
