using RepoMigrator.Tools.GitBranchSplitter;

namespace RepoMigrator.Tools.GitBranchSplitter.Tests;

[TestClass]
public sealed class GitBranchSplitOptionsTests
{
    [TestMethod]
    public void Parse_ReturnsExpectedOptions()
    {
        var args = new[]
        {
            "--repo", @"C:\Repos\Demo",
            "--source", "main",
            "--prefix", "split/output",
            "--overwrite",
            "--author-name", "RepoMigrator Tool",
            "--author-email", "tool@example.local"
        };

        var options = GitBranchSplitOptions.Parse(args);

        Assert.IsNotNull(options);
        Assert.AreEqual(@"C:\Repos\Demo", options.RepositoryPath);
        Assert.AreEqual("main", options.SourceBranch);
        Assert.AreEqual("split/output", options.BranchPrefix);
        Assert.IsTrue(options.OverwriteExistingBranches);
        Assert.AreEqual("RepoMigrator Tool", options.AuthorName);
        Assert.AreEqual("tool@example.local", options.AuthorEmail);
    }

    [TestMethod]
    [DataRow(new[] { "--source", "main" }, "--repo")]
    [DataRow(new[] { "--repo", @"C:\Repos\Demo" }, "--source")]
    [DataRow(new[] { "--repo" }, "Missing value")]
    public void Parse_ThrowsForInvalidArguments(string[] args, string sExpectedMessage)
    {
        try
        {
            _ = GitBranchSplitOptions.Parse(args);
            Assert.Fail("Expected an ArgumentException to be thrown.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, sExpectedMessage);
        }
    }

    [TestMethod]
    public void Parse_ReturnsNullForHelp()
    {
        var options = GitBranchSplitOptions.Parse(new[] { "--help" });

        Assert.IsNull(options);
    }
}
