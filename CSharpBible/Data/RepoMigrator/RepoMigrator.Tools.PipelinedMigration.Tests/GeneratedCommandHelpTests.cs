using System.Reflection;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tools.PipelinedMigration.Tests;

[TestClass]
public sealed class GeneratedCommandHelpTests
{
    [TestMethod]
    public void PipelinedMigration_WriteHelp_ContainsResourceText()
    {
        var helpText = InvokeWriteHelp(
            typeof(PipelinedMigrationService).Assembly,
            "RepoMigrator.Tools.PipelinedMigration.PipelinedMigrationOptionsCommand");

        StringAssert.Contains(helpText, "Runs a pipelined SVN-to-Git migration prototype.");
        StringAssert.Contains(helpText, "SVN source URL or working copy path.");
        StringAssert.Contains(helpText, "--newest-first");
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
