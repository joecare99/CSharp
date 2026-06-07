using System.Reflection;
using RepoMigrator.Tools.ArchiveSmokeTest;

namespace RepoMigrator.Tools.ArchiveSmokeTest.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ArchiveSmokeTestProgramTests
{
    [TestMethod]
    public void Main_ReturnsOne_ForInvalidArguments()
    {
        var args = new[] { "--workspace", @"C:\workspace" };

        var exitCode = InvokeMain(args).GetAwaiter().GetResult();

        Assert.AreEqual(1, exitCode);
    }

    [TestMethod]
    [DataRow("--help")]
    [DataRow("-h")]
    public void Main_ReturnsZero_ForHelpArgument(string sHelpArg)
    {
        var args = new[] { sHelpArg };

        var exitCode = InvokeMain(args).GetAwaiter().GetResult();

        Assert.AreEqual(0, exitCode);
    }

    [TestMethod]
    public void Main_ReturnsTwo_ForRuntimeFailure()
    {
        var missingSource = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tools.ArchiveSmokeTest.Tests", Guid.NewGuid().ToString("N"), "missing-archives");
        var workspace = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tools.ArchiveSmokeTest.Tests", Guid.NewGuid().ToString("N"), "workspace");
        var args = new[] { "--source", missingSource, "--workspace", workspace };

        var exitCode = InvokeMain(args).GetAwaiter().GetResult();

        Assert.AreEqual(2, exitCode);
    }

    private static Task<int> InvokeMain(string[] args)
    {
        var method = typeof(ArchiveSmokeTestService).Assembly
            .GetType("RepoMigrator.Tools.ArchiveSmokeTest.Program")?
            .GetMethod("Main", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(method);
        var task = method.Invoke(null, new object[] { args }) as Task<int>;
        Assert.IsNotNull(task);
        return task;
    }
}
