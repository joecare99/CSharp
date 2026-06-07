using System.Reflection;
using RepoMigrator.Tools.GitBranchSplitter;

namespace RepoMigrator.Tools.GitBranchSplitter.Tests;

[TestClass]
public sealed class GeneratedCommandHelpTests
{
    [TestMethod]
    public void GitBranchSplitter_WriteHelp_ContainsResourceText()
    {
        var helpText = InvokeWriteHelp(
            typeof(GitBranchSplitService).Assembly,
            "RepoMigrator.Tools.GitBranchSplitter.GitBranchSplitOptionsCommand");

        StringAssert.Contains(helpText, "Splits a Git branch into path-based branches.");
        StringAssert.Contains(helpText, "Path to the Git repository working directory.");
        StringAssert.Contains(helpText, "--repo <value>");
    }

    private static string InvokeWriteHelp(Assembly assembly, string sTypeName)
    {
        var type = assembly.GetType(sTypeName);
        Assert.IsNotNull(type);

        var method = type.GetMethod("WriteHelp", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(method);

        using var writer = new StringWriter();
        method.Invoke(null, new object[] { writer });
        return writer.ToString();
    }
}
