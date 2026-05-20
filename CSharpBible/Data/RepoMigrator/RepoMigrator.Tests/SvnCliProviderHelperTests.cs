using System.Reflection;
using RepoMigrator.Providers.SvnCli;
using System.Xml.Linq;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SvnCliProviderHelperTests
{
    [TestMethod]
    public void ParseMissingFromStatus_ReturnsMissingAndObstructedEntries()
    {
        const string sStatusOutput = "!       src/old.cs\nM       src/active.cs\n~       docs\n?       new.txt\n";

        var arrMissing = InvokeParseMissingFromStatus(sStatusOutput).ToArray();

        CollectionAssert.AreEqual(new[] { "src/old.cs", "docs" }, arrMissing);
    }

    [TestMethod]
    public void ParseMissingFromStatusXml_ReturnsMissingAndObstructedEntries_WithUnicodePath()
    {
        var sStatusXml =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<status>\n" +
            "  <target path=\".\">\n" +
            "    <entry path=\"Multimedia\\data\\Für Elise.fmk\">\n" +
            "      <wc-status item=\"missing\" />\n" +
            "    </entry>\n" +
            "    <entry path=\"Multimedia\\data\">\n" +
            "      <wc-status item=\"obstructed\" />\n" +
            "    </entry>\n" +
            "    <entry path=\"Multimedia\\data\\keep.txt\">\n" +
            "      <wc-status item=\"modified\" />\n" +
            "    </entry>\n" +
            "  </target>\n" +
            "</status>\n";

        var arrMissing = InvokeParseMissingFromStatusXml(sStatusXml).ToArray();

        CollectionAssert.AreEqual(
            new[] { "Multimedia\\data\\Für Elise.fmk", "Multimedia\\data" },
            arrMissing);
    }

    [TestMethod]
    [DataRow("C:/wc", "C:/wc/.svn/entries", true)]
    [DataRow("C:/wc", "C:/wc/src/.svn/text-base/file.svn-base", true)]
    [DataRow("C:/wc", "C:/wc/src/file.cs", false)]
    [DataRow("C:/wc", "C:/wc", false)]
    public void IsSvnAdministrativePath_ReturnsExpectedValue(string sRootPath, string sCandidatePath, bool xExpected)
    {
        var xActual = InvokeIsSvnAdministrativePath(sRootPath, sCandidatePath);

        Assert.AreEqual(xExpected, xActual);
    }

    [TestMethod]
    public void EscapeProp_EscapesQuoteCharacters()
    {
        var sActual = InvokeEscapeProp("a\"b\"c");

        Assert.AreEqual("a\\\"b\\\"c", sActual);
    }

    [TestMethod]
    [DataRow("Multimedia\\data", "Multimedia/data")]
    [DataRow("/Multimedia/data/", "Multimedia/data")]
    [DataRow("Multimedia/data", "Multimedia/data")]
    public void NormalizeSvnPath_NormalizesSeparatorsAndTrimsOuterSlashes(string sInput, string sExpected)
    {
        var sActual = InvokeNormalizeSvnPath(sInput);

        Assert.AreEqual(sExpected, sActual);
    }

    private static IEnumerable<string> InvokeParseMissingFromStatus(string sStatusOutput)
    {
        var method = typeof(SvnCliProvider).GetMethod("ParseMissingFromStatus", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        var result = method.Invoke(null, new object[] { sStatusOutput }) as IEnumerable<string>;
        Assert.IsNotNull(result);
        return result;
    }

    private static IEnumerable<string> InvokeParseMissingFromStatusXml(string sStatusXml)
    {
        var method = typeof(SvnCliProvider).GetMethod("ParseMissingFromStatusXml", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        var result = method.Invoke(null, new object[] { sStatusXml }) as IEnumerable<string>;
        Assert.IsNotNull(result);
        return result;
    }

    private static bool InvokeIsSvnAdministrativePath(string sRootPath, string sCandidatePath)
    {
        var method = typeof(SvnCliProvider).GetMethod("IsSvnAdministrativePath", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (bool)method.Invoke(null, new object[] { sRootPath, sCandidatePath })!;
    }

    private static string InvokeEscapeProp(string sValue)
    {
        var method = typeof(SvnCliProvider).GetMethod("EscapeProp", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string)method.Invoke(null, new object[] { sValue })!;
    }

    private static string InvokeNormalizeSvnPath(string sPath)
    {
        var method = typeof(SvnCliProvider).GetMethod("NormalizeSvnPath", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string)method.Invoke(null, new object[] { sPath })!;
    }
}
