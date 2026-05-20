using LibGit2Sharp;
using NSubstitute;
using RepoMigrator.Core;
using RepoMigrator.Providers.Git;
using System.Reflection;

namespace RepoMigrator.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GitProviderRemoteTests
{
    private static readonly object s_gitProviderPushSeamLock = new();

    [TestMethod]
    public async Task GetSelectionDataAsync_ForRemote_ParsesHeadBranchesAndTags()
    {
        var calls = new List<string>();
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Git,
            UrlOrPath = "https://example.org/repo.git"
        };

        await WithGitCommandRunnerAsync(async (arguments, _workingDir, _operation, _ct) =>
        {
            calls.Add(arguments);

            if (arguments.Contains("--symref", StringComparison.Ordinal))
                return "ref: refs/heads/main\tHEAD\nabc123\tHEAD\n";

            if (arguments.Contains("--heads", StringComparison.Ordinal))
                return "a1\trefs/heads/main\n" +
                       "b2\trefs/heads/feature/demo\n";

            if (arguments.Contains("--tags", StringComparison.Ordinal))
                return "c3\trefs/tags/v1.0.0\n";

            return string.Empty;
        }, async () =>
        {
            await using var provider = new GitProvider();

            var selectionData = await provider.GetSelectionDataAsync(endpoint, CancellationToken.None);

            Assert.AreEqual("main", selectionData.DefaultBranch);
            Assert.AreEqual(2, selectionData.Branches.Count);
            Assert.AreEqual("feature/demo", selectionData.Branches[0].Name);
            Assert.AreEqual("main", selectionData.Branches[1].Name);
            Assert.AreEqual(1, selectionData.Tags.Count);
            Assert.AreEqual("v1.0.0", selectionData.Tags[0].Name);
        });

        Assert.IsTrue(calls.Any(call => call.Contains("--symref", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.Contains("--heads", StringComparison.Ordinal)));
        Assert.IsTrue(calls.Any(call => call.Contains("--tags", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task ProbeAsync_ForRemote_WithReferences_ReturnsSuccessAndReferenceDetails()
    {
        var endpoint = new RepositoryEndpoint
        {
            Type = RepoType.Git,
            UrlOrPath = "https://example.org/repo.git",
            BranchOrTrunk = "main"
        };

        await WithGitCommandRunnerAsync((_arguments, _workingDir, _operation, _ct) =>
                Task.FromResult("ref: refs/heads/main\tHEAD\nabc123\tHEAD\n"),
            async () =>
            {
                await using var provider = new GitProvider();

                var probeResult = await provider.ProbeAsync(endpoint, RepositoryAccessMode.Write, CancellationToken.None);

                Assert.IsTrue(probeResult.Success);
                Assert.IsTrue(probeResult.Details.Any(detail => detail.Contains("Remote:", StringComparison.OrdinalIgnoreCase)));
                Assert.IsTrue(probeResult.Details.Any(detail => detail.Contains("Ref:", StringComparison.OrdinalIgnoreCase)));
                Assert.IsTrue(probeResult.Details.Any(detail => detail.Contains("Schreibrechte", StringComparison.OrdinalIgnoreCase)));
            });
    }

    [TestMethod]
    public async Task TransferAsync_WithRemoteTarget_PushesSelectedBranchAndTag()
    {
        var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        var sSourcePath = Path.Combine(sRootPath, "source");
        Directory.CreateDirectory(sSourcePath);

        var calls = new List<string>();
        var progress = NSubstitute.Substitute.For<IMigrationProgress>();

        Repository.Init(sSourcePath);
        using (var sourceRepository = new Repository(sSourcePath))
        {
            File.WriteAllText(Path.Combine(sSourcePath, "README.md"), "seed");
            Commands.Stage(sourceRepository, "*");
            var signature = new Signature("alice", "alice@example.org", DateTimeOffset.UtcNow);
            var commit = sourceRepository.Commit("init", signature, signature);

            var gitMainBranch = sourceRepository.Branches["main"] ?? sourceRepository.CreateBranch("main", commit);
            Commands.Checkout(sourceRepository, gitMainBranch);
            sourceRepository.ApplyTag("v1.0.0", commit.Sha, signature, "release");
        }

        var sourceEndpoint = new RepositoryEndpoint
        {
            Type = RepoType.Git,
            UrlOrPath = sSourcePath,
            BranchOrTrunk = "main"
        };
        var targetEndpoint = new RepositoryEndpoint
        {
            Type = RepoType.Git,
            UrlOrPath = "https://example.org/target.git"
        };
        var options = new MigrationOptions
        {
            TransferMode = RepositoryTransferMode.NativeHistory,
            TransferBranches = false,
            TransferTags = true,
            SelectedTags = ["v1.0.0"]
        };

        try
        {
            await WithGitCommandRunnerAsync((arguments, _workingDir, _operation, _ct) =>
            {
                calls.Add(arguments);
                if (arguments.Contains("ls-remote --heads", StringComparison.Ordinal)
                    || arguments.Contains("ls-remote --tags", StringComparison.Ordinal))
                {
                    return Task.FromResult(string.Empty);
                }

                return Task.FromResult("ok");
            }, async () =>
            {
                await using var provider = new GitProvider();
                await provider.TransferAsync(sourceEndpoint, targetEndpoint, options, progress, CancellationToken.None);
            });

            Assert.IsTrue(calls.Any(call => call.StartsWith("ls-remote --heads", StringComparison.Ordinal)));
            Assert.IsTrue(calls.Any(call => call.StartsWith("ls-remote --tags", StringComparison.Ordinal)));
            Assert.IsTrue(calls.Any(call => call.Contains("push \"https://example.org/target.git\" \"refs/heads/main:refs/heads/main\"", StringComparison.Ordinal)));
            Assert.IsTrue(calls.Any(call => call.Contains("push \"https://example.org/target.git\" \"refs/tags/v1.0.0:refs/tags/v1.0.0\"", StringComparison.Ordinal)));

            progress.Received().Report(MigrationReportSeverity.Information, MigrationReportMessage.GitBranchTransferStarting, Arg.Any<object?[]>());
            progress.Received().Report(MigrationReportSeverity.Information, MigrationReportMessage.GitTagTransferStarting, Arg.Any<object?[]>());
        }
        finally
        {
            TryDeleteDirectoryWithRetry(sRootPath);
        }
    }

    [TestMethod]
    public async Task CommitSnapshotAsync_WithRemoteTarget_SchedulesPushAfterEachRevision()
    {
        var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        var sWorkPath = Path.Combine(sRootPath, "work");
        Directory.CreateDirectory(sWorkPath);

        var targetEndpoint = new RepositoryEndpoint
        {
            Type = RepoType.Git,
            UrlOrPath = "https://example.org/target.git",
            BranchOrTrunk = "main"
        };

        var pushCalls = new List<string>();

        try
        {
            await WithGitCommandRunnerAndPushAsync(
                runner: (_arguments, _workingDir, _operation, _ct) => Task.FromResult("ok"),
                pushBranchAsync: (_provider, branchName, _ct) =>
                {
                    lock (pushCalls)
                    {
                        pushCalls.Add(branchName);
                    }

                    return Task.CompletedTask;
                },
                testAction: async () =>
                {
                    await using var provider = new GitProvider();
                    await provider.InitializeTargetAsync(targetEndpoint, emptyInit: true, CancellationToken.None);

                    File.WriteAllText(Path.Combine(sWorkPath, "a.txt"), "one");
                    await provider.CommitSnapshotAsync(sWorkPath, new CommitMetadata
                    {
                        Message = "rev-1",
                        AuthorName = "alice",
                        AuthorEmail = "alice@example.org",
                        Timestamp = DateTimeOffset.UtcNow,
                        TargetBranch = "main"
                    }, CancellationToken.None);

                    File.WriteAllText(Path.Combine(sWorkPath, "a.txt"), "two");
                    await provider.CommitSnapshotAsync(sWorkPath, new CommitMetadata
                    {
                        Message = "rev-2",
                        AuthorName = "alice",
                        AuthorEmail = "alice@example.org",
                        Timestamp = DateTimeOffset.UtcNow,
                        TargetBranch = "main"
                    }, CancellationToken.None);

                    for (var iAttempt = 0; iAttempt < 20; iAttempt++)
                    {
                        lock (pushCalls)
                        {
                            if (pushCalls.Count >= 2)
                                break;
                        }

                        await Task.Delay(25);
                    }

                    lock (pushCalls)
                    {
                        Assert.AreEqual(2, pushCalls.Count);
                        Assert.IsTrue(pushCalls.All(branch => string.Equals(branch, "main", StringComparison.Ordinal)));
                    }

                    await provider.FlushAsync(CancellationToken.None);

                    lock (pushCalls)
                    {
                        Assert.AreEqual(2, pushCalls.Count);
                    }
                });
        }
        finally
        {
            TryDeleteDirectoryWithRetry(sRootPath);
        }
    }

    private static async Task WithGitCommandRunnerAsync(
        Func<string, string?, string, CancellationToken, Task<string>> runner,
        Func<Task> testAction)
    {
        await WithGitCommandRunnerAndPushAsync(runner, null, testAction);
    }

    private static async Task WithGitCommandRunnerAndPushAsync(
        Func<string, string?, string, CancellationToken, Task<string>> runner,
        Func<GitProvider, string, CancellationToken, Task>? pushBranchAsync,
        Func<Task> testAction)
    {
        var runField = typeof(GitProvider).GetField("s_runGitCommandAsync", BindingFlags.NonPublic | BindingFlags.Static);
        var pushField = typeof(GitProvider).GetField("s_pushBranchAsync", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(runField);
        Assert.IsNotNull(pushField);

        Func<string, string?, string, CancellationToken, Task<string>> previousRunner;
        Func<GitProvider, string, CancellationToken, Task> previousPush;

        lock (s_gitProviderPushSeamLock)
        {
            previousRunner = (Func<string, string?, string, CancellationToken, Task<string>>)runField.GetValue(null)!;
            previousPush = (Func<GitProvider, string, CancellationToken, Task>)pushField.GetValue(null)!;

            runField.SetValue(null, runner);
            if (pushBranchAsync is not null)
                pushField.SetValue(null, pushBranchAsync);
        }

        try
        {
            await testAction();
        }
        finally
        {
            lock (s_gitProviderPushSeamLock)
            {
                runField.SetValue(null, previousRunner);
                pushField.SetValue(null, previousPush);
            }
        }
    }

    private static void TryDeleteDirectoryWithRetry(string sDirectory)
    {
        if (!Directory.Exists(sDirectory))
            return;

        for (var iAttempt = 0; iAttempt < 5; iAttempt++)
        {
            try
            {
                Directory.Delete(sDirectory, recursive: true);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Thread.Sleep(50);
            }
            catch (IOException)
            {
                Thread.Sleep(50);
            }
        }
    }
}
