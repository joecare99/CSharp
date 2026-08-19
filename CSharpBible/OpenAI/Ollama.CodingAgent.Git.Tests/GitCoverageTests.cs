using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Models;

namespace Ollama.CodingAgent.Git.Tests;

[TestClass]
public sealed class GitCoverageTests
{
    [TestMethod]
    public async Task Executor_AppliesBranchCommitUnstageAndRemoteOperations()
    {
        using GitTestRepository repository = GitTestRepository.CreateWithBareRemote();
        IAgentApprovalService approvals = Substitute.For<IAgentApprovalService>();
        approvals.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);
        GitOperationExecutor executor = new(approvals);

        GitOperationResult branch = await executor.ExecuteAsync(repository.WorkspacePath, new CreateGitBranchOperation("feature"));
        Assert.IsTrue(branch.WasApplied);
        Assert.IsTrue((await executor.ExecuteAsync(repository.WorkspacePath, new SwitchGitBranchOperation("feature"))).WasApplied);

        File.WriteAllText(Path.Combine(repository.WorkspacePath, "commit.txt"), "commit");
        using (Repository nativeRepository = new(repository.WorkspacePath))
        {
            Commands.Stage(nativeRepository, "commit.txt");
        }

        GitOperationResult commit = await executor.ExecuteAsync(
            repository.WorkspacePath,
            new CommitGitOperation("Add commit file", new GitIdentity("User", "user@example.invalid")));
        Assert.IsTrue(commit.WasApplied);
        Assert.IsFalse(string.IsNullOrWhiteSpace(commit.CommitSha));

        File.WriteAllText(Path.Combine(repository.WorkspacePath, "unstage.txt"), "unstage");
        using (Repository nativeRepository = new(repository.WorkspacePath))
        {
            Commands.Stage(nativeRepository, "unstage.txt");
        }

        Assert.IsTrue((await executor.ExecuteAsync(repository.WorkspacePath, new UnstageGitPathsOperation(["unstage.txt"]))).WasApplied);
        Assert.IsTrue((await executor.ExecuteAsync(repository.WorkspacePath, new FetchGitOperation("origin"))).WasApplied);
        Assert.IsTrue((await executor.ExecuteAsync(repository.WorkspacePath, new PushGitOperation("origin", "feature"))).WasApplied);
        GitOperationResult pull = await executor.ExecuteAsync(
            repository.WorkspacePath,
            new PullGitOperation(new GitIdentity("User", "user@example.invalid")));
        Assert.IsTrue(pull.WasApproved);
    }

    [TestMethod]
    public async Task Executor_PullsFastForwardAndRejectsConflictedRepositories()
    {
        using (GitTestRepository pullRepository = GitTestRepository.CreateWithBareRemote())
        {
            pullRepository.CreateRemoteDivergence();
            using (Repository nativeRepository = new(pullRepository.WorkspacePath))
            {
                Commands.Fetch(
                    nativeRepository,
                    "origin",
                    nativeRepository.Network.Remotes["origin"].FetchRefSpecs.Select(specification => specification.Specification),
                    null,
                    null);
                nativeRepository.Branches.Update(
                    nativeRepository.Head,
                    updater => updater.TrackedBranch = "refs/remotes/origin/master");
            }

            IAgentApprovalService approvals = Substitute.For<IAgentApprovalService>();
            approvals.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);
            GitOperationResult result = await new GitOperationExecutor(approvals).ExecuteAsync(
                pullRepository.WorkspacePath,
                new PullGitOperation(new GitIdentity("User", "user@example.invalid")));

            Assert.IsTrue(result.WasApplied);
        }

        using GitTestRepository conflictRepository = GitTestRepository.Create();
        string baseBranch;
        using (Repository nativeRepository = new(conflictRepository.WorkspacePath))
        {
            baseBranch = nativeRepository.Head.FriendlyName;
            Branch conflictBranch = nativeRepository.CreateBranch("conflict");
            Commands.Checkout(nativeRepository, conflictBranch);
            File.WriteAllText(Path.Combine(conflictRepository.WorkspacePath, "readme.txt"), "feature");
            Commands.Stage(nativeRepository, "readme.txt");
            Signature signature = new("User", "user@example.invalid", DateTimeOffset.UtcNow);
            nativeRepository.Commit("feature", signature, signature);
            Commands.Checkout(nativeRepository, baseBranch);
            File.WriteAllText(Path.Combine(conflictRepository.WorkspacePath, "readme.txt"), "base");
            Commands.Stage(nativeRepository, "readme.txt");
            nativeRepository.Commit("base", signature, signature);
            nativeRepository.Merge(conflictBranch, signature);
        }

        IAgentApprovalService conflictApprovals = Substitute.For<IAgentApprovalService>();
        GitOperationExecutor conflictExecutor = new(conflictApprovals);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => conflictExecutor.ExecuteAsync(
            conflictRepository.WorkspacePath,
            new CreateGitBranchOperation("blocked")));
    }

    [TestMethod]
    public async Task Executor_RejectsUnsafePathsReferencesAndIdentity()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        IAgentApprovalService approvals = Substitute.For<IAgentApprovalService>();
        GitOperationExecutor executor = new(approvals);

        Assert.ThrowsExactly<ArgumentNullException>(() => new GitOperationExecutor(null!));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation([])));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(null!)));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation([""])));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation(["../outside.txt"])));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation([".git"])));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new StageGitPathsOperation([".git/config"])));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new CommitGitOperation(string.Empty, new GitIdentity("User", "user@example.invalid"))));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new CommitGitOperation("message", new GitIdentity("User", "invalid"))));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new FetchGitOperation("bad name")));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new FetchGitOperation(" ")));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new CreateGitBranchOperation(null!)));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => executor.ExecuteAsync(repository.WorkspacePath, new CommitGitOperation("message", null!)));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => executor.ExecuteAsync(repository.WorkspacePath, new FetchGitOperation("missing")));
        approvals.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);
        Assert.IsFalse((await executor.ExecuteAsync(repository.WorkspacePath, new SwitchGitBranchOperation("missing"))).WasApplied);
    }

    [TestMethod]
    public async Task Executor_CoversUnsupportedAndCancellationApplyPaths()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        IAgentApprovalService approvals = Substitute.For<IAgentApprovalService>();
        approvals.RequestApprovalAsync(Arg.Any<AgentApprovalRequest>(), Arg.Any<CancellationToken>()).Returns(true);
        GitOperationExecutor executor = new(approvals);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => executor.ExecuteAsync(
            repository.WorkspacePath,
            new UnsupportedGitOperation()));

        FieldInfo applyOperation = typeof(GitOperationExecutor).GetField(
            "ApplyOperation",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate original = (Delegate)applyOperation.GetValue(null)!;
        try
        {
            applyOperation.SetValue(null, (Func<Repository, GitWorkspaceOperation, string?>)((_, _) =>
                throw new OperationCanceledException()));

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => executor.ExecuteAsync(
                repository.WorkspacePath,
                new CreateGitBranchOperation("cancelled")));
        }
        finally
        {
            applyOperation.SetValue(null, original);
        }

        MethodInfo apply = typeof(GitOperationExecutor).GetMethod(
            "Apply",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        TargetInvocationException exception = Assert.ThrowsExactly<TargetInvocationException>(
            () => apply.Invoke(null, [new Repository(repository.WorkspacePath), new UnsupportedGitOperation()]));
        Assert.IsInstanceOfType(exception.InnerException, typeof(NotSupportedException));
    }

    [TestMethod]
    public void WorkspaceServiceAndModels_CoverEmptyDiffMetadataAndRegistrations()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        GitWorkspaceService service = new();
        GitDiffPreview preview = service.GetDiffPreview(repository.WorkspacePath, 10);
        Assert.AreEqual(string.Empty, preview.Content);
        Assert.IsFalse(preview.IsTruncated);
        Assert.AreEqual(1, service.GetLocalBranches(repository.WorkspacePath).Count);
        Assert.AreEqual(0, service.GetRemotes(repository.WorkspacePath).Count);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => service.GetDiffPreview(repository.WorkspacePath, 0));
        Assert.ThrowsExactly<ArgumentException>(() => service.Discover(string.Empty));
        Assert.ThrowsExactly<DirectoryNotFoundException>(() => service.Discover(Path.Combine(repository.WorkspacePath, "missing")));

        GitFileChange staged = new("file", FileStatus.NewInIndex);
        GitFileChange changed = new("file", FileStatus.ModifiedInWorkdir);
        GitFileChange conflict = new("file", FileStatus.Conflicted);
        Assert.IsTrue(staged.IsStaged);
        Assert.IsTrue(changed.IsChanged);
        Assert.IsTrue(conflict.IsConflicted);
        GitRepositoryInfo info = new(repository.WorkspacePath, ".git", "master", false, false, false);
        Assert.IsTrue(new GitWorkspaceStatus(info, [staged, changed]).HasStagedChanges);
        Assert.IsTrue(new GitWorkspaceStatus(info, [staged, changed]).HasChangedFiles);
        Assert.IsTrue(new GitWorkspaceStatus(info, [conflict]).HasConflicts);
        Assert.IsTrue(new GitWorkspaceStatus(info with { HasConflicts = true }, []).HasConflicts);

        GitOperationPreview operationPreview = new("Stage", "workspace", new Dictionary<string, string> { ["B"] = "2", ["A"] = "1" });
        StringAssert.Contains(operationPreview.Render(), $"A: 1{Environment.NewLine}B: 2");
        ServiceCollection services = new();
        services.AddSingleton(Substitute.For<IAgentApprovalService>());
        services.AddCodingAgentGit();
        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Assert.IsNotNull(provider.GetRequiredService<IGitWorkspaceService>());
        Assert.IsNotNull(provider.GetRequiredService<IGitOperationExecutor>());
    }

    private sealed record UnsupportedGitOperation : GitWorkspaceOperation;
}
