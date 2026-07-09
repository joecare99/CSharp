#if NET10_0
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies deterministic result mapping for the Git-backed version control service.
/// </summary>
[TestClass]
public class GitVersionControlServiceTests
{
    /// <summary>
    /// Verifies that a discovered branch repository is mapped to the shared versioning contract.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_MapsRepositoryBranchAndChanges()
    {
        IGitCommandRunner gitCommandRunner = Substitute.For<IGitCommandRunner>();
        string repositoryContextPath = Path.Combine("C:", "src", "AA98");
        string repositoryRootPath = Path.Combine("C:", "src", "AA98");

        gitCommandRunner
            .RunAsync(repositoryContextPath, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = repositoryRootPath,
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "branch --show-current", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "main",
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "status --porcelain --ignored", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "M  src/Program.cs\n M README.md\nR  old.txt -> new.txt\n!! bin/Debug/temp.bin",
            }));

        GitVersionControlService service = new(gitCommandRunner);
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = repositoryContextPath,
        }).ConfigureAwait(false);

        Assert.AreEqual(repositoryRootPath, status.RepositoryRootPath);
        Assert.AreEqual("AA98", status.RepositoryName);
        Assert.IsTrue(status.IsRepositoryRootDiscovered);
        Assert.AreEqual("main", status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Branch, status.ActiveReferenceKind);
        Assert.IsFalse(status.IsDetached);
        Assert.IsTrue(status.HasLocalChanges);
        CollectionAssert.AreEquivalent(
            new[]
            {
                VersionControlCapability.InspectStatus,
                VersionControlCapability.InspectReferences,
                VersionControlCapability.EnumerateChanges,
                VersionControlCapability.DistinguishStagedChanges,
                VersionControlCapability.EvaluateIgnoreRules,
            },
            (System.Collections.ICollection)status.Capabilities);
        Assert.AreEqual(4, status.Changes.Count);
        Assert.AreEqual("src/Program.cs", status.Changes[0].Path);
        Assert.AreEqual(VersionControlChangeKind.Modified, status.Changes[0].ChangeKind);
        Assert.IsTrue(status.Changes[0].IsStaged);
        Assert.AreEqual("README.md", status.Changes[1].Path);
        Assert.IsFalse(status.Changes[1].IsStaged);
        Assert.AreEqual(VersionControlChangeKind.Modified, status.Changes[1].ChangeKind);
        Assert.AreEqual("old.txt", status.Changes[2].PreviousPath);
        Assert.AreEqual("new.txt", status.Changes[2].Path);
        Assert.AreEqual(VersionControlChangeKind.Renamed, status.Changes[2].ChangeKind);
        Assert.IsTrue(status.Changes[3].IsIgnored);
        Assert.IsFalse(status.Changes[3].IsStaged);
    }

    /// <summary>
    /// Verifies that a detached repository is mapped without branch information.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_MapsDetachedHeadWhenBranchIsUnavailable()
    {
        IGitCommandRunner gitCommandRunner = Substitute.For<IGitCommandRunner>();
        string repositoryRootPath = Path.Combine("C:", "src", "AA98");

        gitCommandRunner
            .RunAsync(repositoryRootPath, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = repositoryRootPath,
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "branch --show-current", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = string.Empty,
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "rev-parse --short HEAD", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "abc1234",
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "status --porcelain --ignored", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = string.Empty,
            }));

        GitVersionControlService service = new(gitCommandRunner);
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryRootPath = repositoryRootPath,
        }).ConfigureAwait(false);

        Assert.AreEqual("abc1234", status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Detached, status.ActiveReferenceKind);
        Assert.IsTrue(status.IsDetached);
        Assert.IsFalse(status.HasLocalChanges);
        Assert.AreEqual(0, status.Changes.Count);
    }

    /// <summary>
    /// Verifies that Git command failures return a conservative neutral status.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_ReturnsConservativeStatusWhenRepositoryDiscoveryFails()
    {
        IGitCommandRunner gitCommandRunner = Substitute.For<IGitCommandRunner>();
        string repositoryContextPath = Path.Combine("C:", "src", "NoRepo");

        gitCommandRunner
            .RunAsync(repositoryContextPath, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 128,
                StandardError = "fatal: not a git repository",
            }));

        GitVersionControlService service = new(gitCommandRunner);
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = repositoryContextPath,
        }).ConfigureAwait(false);

        Assert.AreEqual(repositoryContextPath, status.RepositoryRootPath);
        Assert.AreEqual("NoRepo", status.RepositoryName);
        Assert.IsFalse(status.IsRepositoryRootDiscovered);
        Assert.IsNull(status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Unknown, status.ActiveReferenceKind);
        Assert.IsFalse(status.HasLocalChanges);
        Assert.AreEqual(0, status.Changes.Count);
        Assert.AreEqual(0, status.Capabilities.Count);
    }

    /// <summary>
    /// Verifies that capability population can be suppressed independently from repository inspection.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_DoesNotPopulateCapabilitiesWhenDisabled()
    {
        IGitCommandRunner gitCommandRunner = Substitute.For<IGitCommandRunner>();
        string repositoryRootPath = Path.Combine("C:", "src", "AA98");

        gitCommandRunner
            .RunAsync(repositoryRootPath, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = repositoryRootPath,
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "branch --show-current", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "main",
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "status --porcelain --ignored", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "",
            }));

        GitVersionControlService service = new(gitCommandRunner);
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryRootPath = repositoryRootPath,
            IncludeCapabilities = false,
        }).ConfigureAwait(false);

        Assert.AreEqual(0, status.Capabilities.Count);
        Assert.AreEqual("main", status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Branch, status.ActiveReferenceKind);
    }

    /// <summary>
    /// Verifies that detailed change enumeration can be skipped while repository discovery still succeeds.
    /// </summary>
    [TestMethod]
    public async Task GetStatusAsync_SkipsChangeEnumerationWhenDisabled()
    {
        IGitCommandRunner gitCommandRunner = Substitute.For<IGitCommandRunner>();
        string repositoryRootPath = Path.Combine("C:", "src", "AA98");

        gitCommandRunner
            .RunAsync(repositoryRootPath, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = repositoryRootPath,
            }));
        gitCommandRunner
            .RunAsync(repositoryRootPath, "branch --show-current", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new GitCommandResult
            {
                ExitCode = 0,
                StandardOutput = "main",
            }));

        GitVersionControlService service = new(gitCommandRunner);
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryRootPath = repositoryRootPath,
            IncludeChanges = false,
        }).ConfigureAwait(false);

        Assert.IsFalse(status.HasLocalChanges);
        Assert.AreEqual(0, status.Changes.Count);
        await gitCommandRunner.DidNotReceive()
            .RunAsync(repositoryRootPath, "status --porcelain --ignored", Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies porcelain parsing for representative local Git states.
    /// </summary>
    [TestMethod]
    public void ParseChanges_MapsRepresentativePorcelainStates()
    {
        IReadOnlyList<VersionControlChangeSummary> changes = GitVersionControlService.ParseChanges("A  src/NewFile.cs\n D src/OldFile.cs\n?? notes.txt\n!! obj/cache.bin");

        Assert.AreEqual(4, changes.Count);
        Assert.AreEqual(VersionControlChangeKind.Added, changes[0].ChangeKind);
        Assert.IsTrue(changes[0].IsStaged);
        Assert.AreEqual(VersionControlChangeKind.Deleted, changes[1].ChangeKind);
        Assert.IsFalse(changes[1].IsStaged);
        Assert.AreEqual(VersionControlChangeKind.Added, changes[2].ChangeKind);
        Assert.IsFalse(changes[2].IsIgnored);
        Assert.IsTrue(changes[3].IsIgnored);
        Assert.AreEqual(VersionControlChangeKind.Unknown, changes[3].ChangeKind);
    }
}
#endif
