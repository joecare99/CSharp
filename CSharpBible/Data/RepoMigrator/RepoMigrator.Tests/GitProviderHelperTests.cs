using System.Reflection;
using RepoMigrator.Core;
using RepoMigrator.Core.Diagnostics;
using RepoMigrator.Providers.Git;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class GitProviderHelperTests
{
    [TestMethod]
    [DataRow("https://example.org/repo.git", true)]
    [DataRow("http://example.org/repo.git", true)]
    [DataRow("git://example.org/repo.git", true)]
    [DataRow("ssh://example.org/repo.git", true)]
    [DataRow("git@example.org:repo.git", true)]
    [DataRow("C:/Repos/Demo", false)]
    [DataRow("\\\\server\\share\\repo", false)]
    [DataRow("file:///C:/Repos/Demo", true)]
    [DataRow("", false)]
    public void LooksLikeRemote_ReturnsExpectedValue(string sUrlOrPath, bool xExpected)
    {
        var xActual = InvokeLooksLikeRemote(sUrlOrPath);

        Assert.AreEqual(xExpected, xActual);
    }

    [TestMethod]
    public void GetRequiredLocalPath_ThrowsForWhitespaceInput()
    {
        try
        {
            InvokeGetRequiredLocalPath("   ", "Git repository");
            Assert.Fail("Expected InvalidOperationException.");
        }
        catch (TargetInvocationException ex) when (ex.InnerException is InvalidOperationException inner)
        {
            StringAssert.Contains(inner.Message, "Git repository must not be empty.");
        }
    }

    [TestMethod]
    public void GetRequiredLocalPath_ReturnsFullPath()
    {
        var sRelativePath = ".\\RepoMigrator";

        var sActual = InvokeGetRequiredLocalPath(sRelativePath, "Git repository");

        Assert.AreEqual(Path.GetFullPath(sRelativePath), sActual);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", null)]
    [DataRow("   ", null)]
    [DataRow(" main ", "main")]
    public void NormalizeBranchName_ReturnsExpectedValue(string? sInput, string? sExpected)
    {
        var sActual = InvokeNormalizeBranchName(sInput);

        Assert.AreEqual(sExpected, sActual);
    }

    [TestMethod]
    public void EnsureDirectoryCanBeInitializedAsRepository_ThrowsForNonEmptyDirectory()
    {
        var sDirectory = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sDirectory);
        File.WriteAllText(Path.Combine(sDirectory, "existing.txt"), "content");

        try
        {
            try
            {
                InvokeEnsureDirectoryCanBeInitializedAsRepository(sDirectory);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (TargetInvocationException ex) when (ex.InnerException is InvalidOperationException inner)
            {
                StringAssert.Contains(inner.Message, "must be empty or already contain a Git repository");
            }
        }
        finally
        {
            if (Directory.Exists(sDirectory))
                Directory.Delete(sDirectory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow(".git", true)]
    [DataRow(".git\\config", true)]
    [DataRow("src\\.gitignore", false)]
    [DataRow("README.md", false)]
    public void IsGitAdministrativePath_ReturnsExpectedValue(string sRelativePath, bool xExpected)
    {
        var xActual = InvokeIsGitAdministrativePath(sRelativePath);

        Assert.AreEqual(xExpected, xActual);
    }

    [TestMethod]
    public void SyncDirectoryContents_AppliesDifferentialPatchWithoutOverwritingUnchangedFile()
    {
        var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        var sSourcePath = Path.Combine(sRootPath, "source");
        var sDestPath = Path.Combine(sRootPath, "dest");
        var sSnapshotRootPath = Path.Combine(sRootPath, "snapshots");
        Directory.CreateDirectory(sSourcePath);
        Directory.CreateDirectory(sDestPath);
        DifferentialSnapshotStore.SnapshotRootOverride = sSnapshotRootPath;

        try
        {
            var sSourceSameFilePath = Path.Combine(sSourcePath, "same.txt");
            var sDestSameFilePath = Path.Combine(sDestPath, "same.txt");
            File.WriteAllText(sSourceSameFilePath, "same-content");
            File.WriteAllText(sDestSameFilePath, "same-content");

            var dtOldWriteTime = DateTimeOffset.UtcNow.AddDays(-2);
            File.SetLastWriteTimeUtc(sDestSameFilePath, dtOldWriteTime.UtcDateTime);

            File.WriteAllText(Path.Combine(sSourcePath, "changed.txt"), "newer-value");
            File.WriteAllText(Path.Combine(sDestPath, "changed.txt"), "old-value");

            File.WriteAllText(Path.Combine(sSourcePath, "added.txt"), "added");
            File.WriteAllText(Path.Combine(sDestPath, "removed.txt"), "remove-me");

            Directory.CreateDirectory(Path.Combine(sDestPath, ".git"));
            File.WriteAllText(Path.Combine(sDestPath, ".git", "config"), "metadata");

            InvokeSyncDirectoryContents(sSourcePath, sDestPath);

            Assert.AreEqual("same-content", File.ReadAllText(sDestSameFilePath));
            Assert.AreEqual(dtOldWriteTime.UtcDateTime, File.GetLastWriteTimeUtc(sDestSameFilePath));
            Assert.AreEqual("newer-value", File.ReadAllText(Path.Combine(sDestPath, "changed.txt")));
            Assert.AreEqual("added", File.ReadAllText(Path.Combine(sDestPath, "added.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(sDestPath, "removed.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(sDestPath, ".git", "config")));

            var sOperationPath = Directory.GetDirectories(Path.Combine(sSnapshotRootPath, "Git")).Single();
            Assert.AreEqual("newer-value", File.ReadAllText(Path.Combine(sOperationPath, "source", "changed.txt")));
            Assert.AreEqual("old-value", File.ReadAllText(Path.Combine(sOperationPath, "destination", "changed.txt")));
            Assert.AreEqual("added", File.ReadAllText(Path.Combine(sOperationPath, "added", "added.txt")));
            Assert.AreEqual("remove-me", File.ReadAllText(Path.Combine(sOperationPath, "removed", "removed.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(sOperationPath, "source", "same.txt")));
        }
        finally
        {
            DifferentialSnapshotStore.SnapshotRootOverride = null;
            if (Directory.Exists(sRootPath))
                Directory.Delete(sRootPath, recursive: true);
        }
    }

    private static bool InvokeLooksLikeRemote(string sUrlOrPath)
    {
        var method = typeof(GitProvider).GetMethod("LooksLikeRemote", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (bool)method.Invoke(null, new object[] { sUrlOrPath })!;
    }

    private static string InvokeGetRequiredLocalPath(string sUrlOrPath, string sDescription)
    {
        var method = typeof(GitProvider).GetMethod("GetRequiredLocalPath", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string)method.Invoke(null, new object[] { sUrlOrPath, sDescription })!;
    }

    private static string? InvokeNormalizeBranchName(string? sBranchName)
    {
        var method = typeof(GitProvider).GetMethod("NormalizeBranchName", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string?)method.Invoke(null, new object?[] { sBranchName });
    }

    private static void InvokeEnsureDirectoryCanBeInitializedAsRepository(string sLocalPath)
    {
        var method = typeof(GitProvider).GetMethod("EnsureDirectoryCanBeInitializedAsRepository", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        method.Invoke(null, new object[] { sLocalPath });
    }

    private static bool InvokeIsGitAdministrativePath(string sRelativePath)
    {
        var method = typeof(GitProvider).GetMethod("IsGitAdministrativePath", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (bool)method.Invoke(null, new object[] { sRelativePath })!;
    }

    private static void InvokeSyncDirectoryContents(string sSourceDir, string sDestDir)
    {
        var method = typeof(GitProvider).GetMethod("SyncDirectoryContents", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        method.Invoke(null, new object[] { sSourceDir, sDestDir });
    }
}
