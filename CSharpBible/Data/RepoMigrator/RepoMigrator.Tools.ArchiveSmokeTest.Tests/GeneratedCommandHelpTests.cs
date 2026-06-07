using System.Reflection;
using RepoMigrator.Tools.ArchiveSmokeTest;

namespace RepoMigrator.Tools.ArchiveSmokeTest.Tests;

[TestClass]
public sealed class GeneratedCommandHelpTests
{
    [TestMethod]
    public void ArchiveSmokeTest_WriteHelp_ContainsResourceText_AndRepeatableOption()
    {
        var helpText = InvokeWriteHelp(
            typeof(ArchiveSmokeTestService).Assembly,
            "RepoMigrator.Tools.ArchiveSmokeTest.ArchiveSmokeTestOptionsCommand");

        StringAssert.Contains(helpText, "Scans archive snapshots and prepares an archive-import plan preview.");
        StringAssert.Contains(helpText, "Allowed archive extension. Repeat the option to allow multiple extensions.");
        StringAssert.Contains(helpText, "--extension <value> repeated");
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
