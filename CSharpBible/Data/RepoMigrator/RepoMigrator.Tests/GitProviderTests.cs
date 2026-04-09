using LibGit2Sharp;
using RepoMigrator.Core;
using RepoMigrator.Providers.Git;

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
}
