using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Providers.Git;
using System.Reflection;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitProviderTests
{
    [TestMethod]
    public async Task GetCapabilitiesAsync_ForLocalBareRepositoryPath_TreatsPathAsLocal()
    {
        var sRepositoryPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"), "CSharp.git");
        Directory.CreateDirectory(Path.GetDirectoryName(sRepositoryPath)!);
        Repository.Init(sRepositoryPath, isBare: true);

        try
        {
            await using var provider = new GitProvider();

            var capabilities = await provider.GetCapabilitiesAsync(new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = sRepositoryPath }, CancellationToken.None);

            Assert.IsTrue(capabilities.SupportsNativeHistoryTransfer);
            Assert.IsTrue(capabilities.SupportsBranchSelection);
            Assert.IsTrue(capabilities.SupportsTagSelection);
        }
        finally
        {
            if (Directory.Exists(Path.GetDirectoryName(sRepositoryPath)!))
                Directory.Delete(Path.GetDirectoryName(sRepositoryPath)!, recursive: true);
        }
    }

    [TestMethod]
    public async Task CommitSnapshotAsync_WhenChangedPathCountExceedsExpected_ThrowsBeforeCommit()
    {
        var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        var sTargetPath = Path.Combine(sRootPath, "target");
        var sSnapshotPath = Path.Combine(sRootPath, "snapshot");
        Directory.CreateDirectory(sTargetPath);
        Directory.CreateDirectory(sSnapshotPath);

        try
        {
            Repository.Init(sTargetPath);
            using (var gitRepository = new Repository(sTargetPath))
            {
                File.WriteAllText(Path.Combine(sTargetPath, "one.txt"), "one");
                Commands.Stage(gitRepository, "*");
                var signature = new Signature("test", "test@example.org", DateTimeOffset.UtcNow);
                gitRepository.Commit("initial", signature, signature);
            }

            File.WriteAllText(Path.Combine(sSnapshotPath, "one.txt"), "changed");
            File.WriteAllText(Path.Combine(sSnapshotPath, "two.txt"), "added");

            InvalidOperationException? ex = null;
            await using (var provider = new GitProvider())
            {
                await provider.InitializeTargetAsync(new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = sTargetPath, BranchOrTrunk = "master" }, true, CancellationToken.None);

                try
                {
                    await provider.CommitSnapshotAsync(sSnapshotPath, new CommitMetadata
                    {
                        Message = "import",
                        AuthorName = "alice",
                        Timestamp = DateTimeOffset.UtcNow,
                        ExpectedChangedPathCount = 1,
                        ExpectedChangedFilePathCount = 1,
                        VerifyChangedPathCount = true
                    }, CancellationToken.None);
                }
                catch (InvalidOperationException caughtEx)
                {
                    ex = caughtEx;
                }
            }

            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, "Git target has 2 changed file paths");
        }
        finally
        {
            if (Directory.Exists(sRootPath))
                TryDeleteDirectory(sRootPath);
        }
    }

    private static void TryDeleteDirectory(string sDirectory)
    {
        try
        {
            foreach (var sFilePath in Directory.EnumerateFiles(sDirectory, "*", SearchOption.AllDirectories))
                File.SetAttributes(sFilePath, FileAttributes.Normal);

            Directory.Delete(sDirectory, recursive: true);
        }
        catch
        {
        }
    }

    [TestMethod]
    public async Task GetCapabilitiesAsync_ForGitProtocolUrl_TreatsPathAsRemote()
    {
        await using var provider = new GitProvider();

        var capabilities = await provider.GetCapabilitiesAsync(new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = "git://192.168.0.32/repo.git" }, CancellationToken.None);

        Assert.IsFalse(capabilities.SupportsNativeHistoryTransfer);
        Assert.IsFalse(capabilities.SupportsBranchSelection);
        Assert.IsFalse(capabilities.SupportsTagSelection);
    }

    [TestMethod]
    public async Task InitializeTargetAsync_ForNonEmptyDirectoryWithoutRepository_ThrowsAndPreservesContents()
    {
        var sTargetPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sTargetPath);
        var sExistingFilePath = Path.Combine(sTargetPath, "keep.txt");
        File.WriteAllText(sExistingFilePath, "existing");

        try
        {
            await using var provider = new GitProvider();

            InvalidOperationException? ex = null;
            try
            {
                await provider.InitializeTargetAsync(
                    new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = sTargetPath },
                    emptyInit: true,
                    CancellationToken.None);
            }
            catch (InvalidOperationException caughtEx)
            {
                ex = caughtEx;
            }

            Assert.IsNotNull(ex);
            StringAssert.Contains(ex.Message, "must be empty or already contain a Git repository");
            Assert.IsTrue(File.Exists(sExistingFilePath));
            Assert.AreEqual("existing", File.ReadAllText(sExistingFilePath));
        }
        finally
        {
            if (Directory.Exists(sTargetPath))
                Directory.Delete(sTargetPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task InitializeTargetAsync_ForEmptyPath_ThrowsClearMessage()
    {
        await using var provider = new GitProvider();

        InvalidOperationException? ex = null;
        try
        {
            await provider.InitializeTargetAsync(
                new RepositoryEndpoint { Type = RepoType.Git, UrlOrPath = "   " },
                emptyInit: true,
                CancellationToken.None);
        }
        catch (InvalidOperationException caughtEx)
        {
            ex = caughtEx;
        }

        Assert.IsNotNull(ex);
        StringAssert.Contains(ex.Message, "Git target path must not be empty.");
    }

    [TestMethod]
    public async Task FlushAsync_WhenNoPushTargetConfigured_CompletesWithoutError()
    {
        await using var provider = new GitProvider();

        await provider.FlushAsync(CancellationToken.None);
    }

    [TestMethod]
    public async Task RunGitAsync_WhenCommandFails_ThrowsInvalidOperationException()
    {
        var method = typeof(GitProvider).GetMethod("RunGitAsync", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);

        var task = (Task<string>)method.Invoke(null, new object?[]
        {
            "__copilot_invalid_git_command__",
            null,
            "Git-Test-Operation",
            CancellationToken.None
        })!;

        try
        {
            await task;
            Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(ex.Message));
        }
    }
}
