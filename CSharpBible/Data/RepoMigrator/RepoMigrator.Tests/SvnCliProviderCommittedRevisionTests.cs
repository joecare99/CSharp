using System.Reflection;
using RepoMigrator.Providers.SvnCli;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class SvnCliProviderCommittedRevisionTests
{
    [TestMethod]
    [DataRow("Adding         src/Module7/file123.txt\r\nTransmitting file data .\r\nCommitted revision 7.\r\n", "7")]
    [DataRow("Sende        2024/report.txt\nÜbertragen wurden Revision 42.\n", "42")]
    [DataRow("Sending        docs/v2/spec.md\nNo revision line here\n", null)]
    public void ExtractCommittedRevision_ReturnsRevisionFromCommitLine(string sCommitOutput, string? sExpectedRevision)
    {
        var sRevision = InvokeExtractCommittedRevision(sCommitOutput);

        Assert.AreEqual(sExpectedRevision, sRevision);
    }

    private static string? InvokeExtractCommittedRevision(string sCommitOutput)
    {
        var method = typeof(SvnCliProvider).GetMethod("ExtractCommittedRevision", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        return (string?)method.Invoke(null, new object[] { sCommitOutput });
    }
}
