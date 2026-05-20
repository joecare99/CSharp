using System.Reflection;
using RepoMigrator.Core.Diagnostics;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SvnCliProviderSyncDirectoryTests
{
    [TestMethod]
    public void SyncDirectory_PreservesAdministrativeSvnFolders()
    {
        var sRootPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"));
        var sSourcePath = Path.Combine(sRootPath, "source");
        var sDestPath = Path.Combine(sRootPath, "dest");
        Directory.CreateDirectory(sSourcePath);
        Directory.CreateDirectory(sDestPath);

        try
        {
            File.WriteAllText(Path.Combine(sSourcePath, "README.md"), "source");
            Directory.CreateDirectory(Path.Combine(sDestPath, ".svn", "tmp"));
            Directory.CreateDirectory(Path.Combine(sDestPath, ".svn", "pristine", "ab"));
            File.WriteAllText(Path.Combine(sDestPath, ".svn", "pristine", "ab", "keep.svn-base"), "metadata");
            File.WriteAllText(Path.Combine(sDestPath, "obsolete.txt"), "old");

            InvokeSyncDirectory(sSourcePath, sDestPath);

            Assert.IsTrue(Directory.Exists(Path.Combine(sDestPath, ".svn", "tmp")));
            Assert.IsTrue(File.Exists(Path.Combine(sDestPath, ".svn", "pristine", "ab", "keep.svn-base")));
            Assert.IsFalse(File.Exists(Path.Combine(sDestPath, "obsolete.txt")));
            Assert.AreEqual("source", File.ReadAllText(Path.Combine(sDestPath, "README.md")));
        }
        finally
        {
            if (Directory.Exists(sRootPath))
                Directory.Delete(sRootPath, recursive: true);
        }
    }

    [TestMethod]
    public void SyncDirectory_AppliesDifferentialPatchWithoutOverwritingUnchangedFile()
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

            File.WriteAllText(Path.Combine(sSourcePath, "changed.txt"), "new-value");
            File.WriteAllText(Path.Combine(sDestPath, "changed.txt"), "old-value");

            File.WriteAllText(Path.Combine(sSourcePath, "added.txt"), "added");
            File.WriteAllText(Path.Combine(sDestPath, "removed.txt"), "remove-me");

            Directory.CreateDirectory(Path.Combine(sDestPath, ".svn", "pristine"));
            File.WriteAllText(Path.Combine(sDestPath, ".svn", "pristine", "keep.svn-base"), "metadata");

            InvokeSyncDirectory(sSourcePath, sDestPath);

            Assert.AreEqual("same-content", File.ReadAllText(sDestSameFilePath));
            Assert.AreEqual(dtOldWriteTime.UtcDateTime, File.GetLastWriteTimeUtc(sDestSameFilePath));
            Assert.AreEqual("new-value", File.ReadAllText(Path.Combine(sDestPath, "changed.txt")));
            Assert.AreEqual("added", File.ReadAllText(Path.Combine(sDestPath, "added.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(sDestPath, "removed.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(sDestPath, ".svn", "pristine", "keep.svn-base")));

            var sOperationPath = Directory.GetDirectories(Path.Combine(sSnapshotRootPath, "SvnCli")).Single();
            Assert.AreEqual("new-value", File.ReadAllText(Path.Combine(sOperationPath, "source", "changed.txt")));
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

    private static void InvokeSyncDirectory(string sSourcePath, string sDestPath)
    {
        var method = typeof(SvnCliProvider).GetMethod("SyncDirectory", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        method.Invoke(null, new object[] { sSourcePath, sDestPath });
    }
}
