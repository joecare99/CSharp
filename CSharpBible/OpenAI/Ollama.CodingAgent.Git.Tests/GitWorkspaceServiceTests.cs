using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Git.Tests;

[TestClass]
public sealed class GitWorkspaceServiceTests
{
    [TestMethod]
    public void DiscoverAndStatus_ReturnRepositoryAndStagedChangedState()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.AppendAllText(Path.Combine(repository.WorkspacePath, "readme.txt"), "changed");
        File.WriteAllText(Path.Combine(repository.WorkspacePath, "new.txt"), "new");
        using (Repository nativeRepository = new(repository.WorkspacePath))
        {
            Commands.Stage(nativeRepository, "new.txt");
        }

        GitWorkspaceService service = new();
        GitRepositoryInfo discovered = service.Discover(Path.Combine(repository.WorkspacePath, "nested"));
        GitWorkspaceStatus status = service.GetStatus(repository.WorkspacePath);

        Assert.AreEqual(repository.WorkspacePath, discovered.RootPath);
        Assert.IsTrue(status.HasStagedChanges);
        Assert.IsTrue(status.HasChangedFiles);
    }

    [TestMethod]
    public void GetDiffPreview_BoundsContent()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        File.AppendAllText(Path.Combine(repository.WorkspacePath, "readme.txt"), new string('x', 500));

        GitDiffPreview preview = new GitWorkspaceService().GetDiffPreview(repository.WorkspacePath, 80);

        Assert.AreEqual(80, preview.Content.Length);
        Assert.IsTrue(preview.IsTruncated);
        Assert.IsGreaterThan(80, preview.TotalCharacterCount);
    }

    [TestMethod]
    public void Discover_RejectsNonRepositoryWorkspace()
    {
        string directory = Path.Combine(Path.GetTempPath(), "coding-agent-git-tests", $"coding-agent-git-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            Assert.IsNull(Repository.Discover(directory));
            InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => new GitWorkspaceService().Discover(directory));
            StringAssert.Contains(exception.Message, "not inside a supported Git repository");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void GetRemotes_RedactsCredentialsFromOperatorVisibleResult()
    {
        using GitTestRepository repository = GitTestRepository.Create();
        using (Repository nativeRepository = new(repository.WorkspacePath))
        {
            nativeRepository.Network.Remotes.Add("origin", "https://operator:super-secret@example.invalid/owner/repo.git?token=also-secret");
            nativeRepository.Network.Remotes.Add("scp", "operator:super-secret@example.invalid:owner/repo.git");
        }

        GitRemoteInfo[] remotes = [.. new GitWorkspaceService().GetRemotes(repository.WorkspacePath)];
        GitRemoteInfo remote = remotes[0];

        Assert.AreEqual("https://example.invalid/owner/repo.git", remote.FetchUrl);
        Assert.IsFalse(remote.FetchUrl.Contains("operator", StringComparison.Ordinal));
        Assert.IsFalse(remote.FetchUrl.Contains("super-secret", StringComparison.Ordinal));
        Assert.IsFalse(remote.FetchUrl.Contains("also-secret", StringComparison.Ordinal));
        Assert.AreEqual("example.invalid:owner/repo.git", remotes[1].FetchUrl);
    }

    [TestMethod]
    public void GetLocalBranches_ReportsTrackingInformation()
    {
        using GitTestRepository repository = GitTestRepository.CreateWithBareRemote();
        using (Repository nativeRepository = new(repository.WorkspacePath))
        {
            string branchName = nativeRepository.Head.FriendlyName;
            Commands.Fetch(
                nativeRepository,
                "origin",
                nativeRepository.Network.Remotes["origin"].FetchRefSpecs.Select(specification => specification.Specification),
                new FetchOptions(),
                null);
            nativeRepository.Branches.Update(
                nativeRepository.Head,
                updater => updater.TrackedBranch = $"refs/remotes/origin/{branchName}");
        }

        GitBranchInfo branch = new GitWorkspaceService()
            .GetLocalBranches(repository.WorkspacePath)
            .Single(item => item.IsCurrent);

        Assert.AreEqual($"origin/{branch.Name}", branch.UpstreamName);
    }

    [TestMethod]
    public void BranchProjection_HandlesMissingTargetAndUpstream()
    {
        Assert.AreEqual(string.Empty, GitWorkspaceService.GetTargetSha(null));
        Assert.IsNull(GitWorkspaceService.GetUpstreamName(null));
    }

    [TestMethod]
    public void WorkspaceInspection_CoversUnbornDetachedAndInvalidRepositories()
    {
        string unbornPath = Path.Combine(AppContext.BaseDirectory, "TestWorkspaces", $"coding-agent-git-unborn-{Guid.NewGuid():N}");
        string barePath = Path.Combine(AppContext.BaseDirectory, "TestWorkspaces", $"coding-agent-git-bare-{Guid.NewGuid():N}");
        Directory.CreateDirectory(unbornPath);
        Repository.Init(unbornPath);
        Repository.Init(barePath, isBare: true);
        try
        {
            GitWorkspaceService service = new();
            GitDiffPreview emptyPreview = service.GetDiffPreview(unbornPath, 10);
            Assert.IsNotNull(service.GetLocalBranches(unbornPath));

            Assert.AreEqual(string.Empty, emptyPreview.Content);
            Assert.IsFalse(emptyPreview.IsTruncated);
            Assert.ThrowsExactly<ArgumentException>(() => service.Discover("\0"));
            Assert.ThrowsExactly<InvalidOperationException>(() => service.Discover(barePath));

            Assert.AreEqual(string.Empty, InvokeSanitizeRemoteUrl(null));
            Assert.AreEqual(string.Empty, InvokeSanitizeRemoteUrl(" "));
            Assert.AreEqual("not a url", InvokeSanitizeRemoteUrl("not a url"));
            using (Repository repository = new(unbornPath))
            {
                File.WriteAllText(Path.Combine(unbornPath, "file.txt"), "content");
                Commands.Stage(repository, "file.txt");
                Signature signature = new("User", "user@example.invalid", DateTimeOffset.UtcNow);
                repository.Commit("commit", signature, signature);
                Commands.Checkout(repository, repository.Head.Tip);

                GitRepositoryInfo info = service.Discover(unbornPath);
                Assert.IsTrue(info.IsDetached);
                Assert.IsNull(info.CurrentBranch);
            }
        }
        finally
        {
            TryDelete(unbornPath);
            TryDelete(barePath);
        }
    }

    [TestMethod]
    [DataRow("fatal: ******example.invalid/owner/repo.git failed", "super-secret")]
    [DataRow("fatal: operator:super-secret@example.invalid:owner/repo.git failed", "super-secret")]
    [DataRow("fatal: https://example.invalid/owner/repo.git?token=super-secret failed", "super-secret")]
    public void GitDiagnosticRedactor_RedactsCredentials(string diagnostic, string secret)
    {
        string redactedDiagnostic = GitDiagnosticRedactor.Redact(diagnostic);

        Assert.IsFalse(redactedDiagnostic.Contains(secret, StringComparison.Ordinal));
        Assert.IsFalse(redactedDiagnostic.Contains("operator", StringComparison.Ordinal));
    }

    [TestMethod]
    public void GitDiagnosticRedactor_ReturnsEmptyForMissingDiagnostic()
    {
        Assert.AreEqual(string.Empty, GitDiagnosticRedactor.Redact(null));
        Assert.AreEqual(string.Empty, GitDiagnosticRedactor.Redact(string.Empty));
    }

    private static string InvokeSanitizeRemoteUrl(string? value)
    {
        MethodInfo method = typeof(GitWorkspaceService).GetMethod(
            "SanitizeRemoteUrl",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string)method.Invoke(null, [value])!;
    }

    private static void TryDelete(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
