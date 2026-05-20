using System.Reflection;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
[DoNotParallelize]
public sealed class PipelinedMigrationProgramTests
{
    [TestMethod]
    public void Main_ReturnsOne_ForInvalidArguments()
    {
        var args = new[] { "--source", "svn://example/source" };

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
        var sMissingWorkingCopy = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"), "missing-svn-wc");
        var sTargetPath = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"), "git-target");

        var args = new[] { "--source", sMissingWorkingCopy, "--target", sTargetPath, "--max-count", "1" };

        var exitCode = InvokeMain(args).GetAwaiter().GetResult();

        Assert.AreEqual(2, exitCode);
    }

    private static Task<int> InvokeMain(string[] args)
    {
        var method = typeof(PipelinedMigrationService).Assembly
            .GetType("RepoMigrator.Tools.PipelinedMigration.Program")?
            .GetMethod("Main", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(method);
        var task = method.Invoke(null, new object[] { args }) as Task<int>;
        Assert.IsNotNull(task);
        return task;
    }
}
