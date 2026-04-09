using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class PipelinedMigrationOptionsTests
{
    [DataTestMethod]
    [DataRow("--prefetch", "0", "prefetch count")]
    [DataRow("--export-workers", "0", "worker count")]
    [DataRow("--max-count", "0", "max count")]
    public void Parse_ThrowsForInvalidNumericValues(string sOptionName, string sOptionValue, string sExpectedMessagePart)
    {
        var arrArgs = new[] { "--source", "svn://example/source", "--target", "C:/git-target", sOptionName, sOptionValue };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, sExpectedMessagePart);
        }
    }

    [TestMethod]
    public void Parse_ReturnsValidatedOptions()
    {
        var arrArgs = new[]
        {
            "--source", "svn://example/source",
            "--target", "C:/git-target",
            "--source-branch", "trunk",
            "--target-branch", "main",
            "--prefetch", "4",
            "--export-workers", "3",
            "--newest-first"
        };

        var options = PipelinedMigrationOptions.Parse(arrArgs);

        Assert.IsNotNull(options);
        Assert.AreEqual("svn://example/source", options.SourceUrl);
        Assert.AreEqual("C:/git-target", options.TargetUrl);
        Assert.AreEqual("trunk", options.SourceBranchOrTrunk);
        Assert.AreEqual("main", options.TargetBranch);
        Assert.AreEqual(4, options.PrefetchCount);
        Assert.AreEqual(3, options.MaxExportWorkers);
        Assert.IsFalse(options.OldestFirst);
    }
}
