using System.Reflection;
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

    private static void InvokeSyncDirectory(string sSourcePath, string sDestPath)
    {
        var method = typeof(SvnCliProvider).GetMethod("SyncDirectory", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        method.Invoke(null, new object[] { sSourcePath, sDestPath });
    }
}
