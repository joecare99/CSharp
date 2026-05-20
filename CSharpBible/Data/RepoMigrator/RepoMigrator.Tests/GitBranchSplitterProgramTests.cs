using System.Reflection;
using RepoMigrator.Tools.GitBranchSplitter;

namespace RepoMigrator.Tests;

[TestClass]
[DoNotParallelize]
public sealed class GitBranchSplitterProgramTests
{
    [TestMethod]
    public void Main_ReturnsOne_ForInvalidArguments()
    {
        var args = new[] { "--repo", "C:/repo" };

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
        var sMissingRepository = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tests", Guid.NewGuid().ToString("N"), "missing-git-repo");
        var args = new[] { "--repo", sMissingRepository, "--source", "main" };

        var exitCode = InvokeMain(args).GetAwaiter().GetResult();

        Assert.AreEqual(2, exitCode);
    }

    private static Task<int> InvokeMain(string[] args)
    {
        var method = typeof(GitBranchSplitService).Assembly
            .GetType("RepoMigrator.Tools.GitBranchSplitter.Program")?
            .GetMethod("Main", BindingFlags.Static | BindingFlags.Public);

        Assert.IsNotNull(method);
        var task = method.Invoke(null, new object[] { args }) as Task<int>;
        Assert.IsNotNull(task);
        return task;
    }
}
