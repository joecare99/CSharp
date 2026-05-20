using RepoMigrator.Core;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class PipelinedMigrationOptionsTests
{
    [TestMethod]
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
    [DataRow("--prefetch", "abc")]
    [DataRow("--export-workers", "abc")]
    [DataRow("--max-count", "abc")]
    public void Parse_ThrowsForNonNumericValues(string sOptionName, string sOptionValue)
    {
        var arrArgs = new[] { "--source", "svn://example/source", "--target", "C:/git-target", sOptionName, sOptionValue };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "is not a valid integer");
        }
    }

    [TestMethod]
    public void Parse_ThrowsForUnexpectedArgument()
    {
        var arrArgs = new[] { "--source", "svn://example/source", "--target", "C:/git-target", "invalid" };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Unexpected argument");
        }
    }

    [TestMethod]
    public void Parse_ThrowsForMissingValue()
    {
        var arrArgs = new[] { "--source", "svn://example/source", "--target", "C:/git-target", "--from" };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Missing value for '--from'");
        }
    }

    [TestMethod]
    public void Parse_ThrowsForMissingSourceValue()
    {
        var arrArgs = new[] { "--target", "C:/git-target" };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Missing required '--source' value");
        }
    }

    [TestMethod]
    public void Parse_ThrowsForMissingTargetValue()
    {
        var arrArgs = new[] { "--source", "svn://example/source" };

        try
        {
            PipelinedMigrationOptions.Parse(arrArgs);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "Missing required '--target' value");
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

    [TestMethod]
    public void CreateSourceEndpoint_ReturnsExpectedValues()
    {
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = "svn://example/source",
            SourceBranchOrTrunk = "trunk",
            SourceUser = "alice",
            SourcePassword = "secret"
        };

        var endpoint = options.CreateSourceEndpoint();

        Assert.AreEqual(RepoType.Svn, endpoint.Type);
        Assert.AreEqual("svn://example/source", endpoint.UrlOrPath);
        Assert.AreEqual("trunk", endpoint.BranchOrTrunk);
        Assert.IsNotNull(endpoint.Credentials);
        Assert.AreEqual("alice", endpoint.Credentials.Username);
        Assert.AreEqual("secret", endpoint.Credentials.Password);
    }

    [TestMethod]
    public void CreateTargetEndpoint_ReturnsExpectedValues()
    {
        var options = new PipelinedMigrationOptions
        {
            TargetUrl = "C:/git-target",
            TargetBranch = "main",
            TargetUser = "bob",
            TargetPassword = "pwd"
        };

        var endpoint = options.CreateTargetEndpoint();

        Assert.AreEqual(RepoType.Git, endpoint.Type);
        Assert.AreEqual("C:/git-target", endpoint.UrlOrPath);
        Assert.AreEqual("main", endpoint.BranchOrTrunk);
        Assert.IsNotNull(endpoint.Credentials);
        Assert.AreEqual("bob", endpoint.Credentials.Username);
        Assert.AreEqual("pwd", endpoint.Credentials.Password);
    }

    [TestMethod]
    public void CreateQuery_ReturnsExpectedValues()
    {
        var options = new PipelinedMigrationOptions
        {
            FromId = "100",
            ToId = "120",
            MaxCount = 25,
            OldestFirst = false
        };

        var query = options.CreateQuery();

        Assert.AreEqual("100", query.FromExclusiveId);
        Assert.AreEqual("120", query.ToInclusiveId);
        Assert.AreEqual(25, query.MaxCount);
        Assert.IsFalse(query.OldestFirst);
    }

    [TestMethod]
    public void Validate_ThrowsForMissingSourceUrl()
    {
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = " ",
            TargetUrl = "C:/git-target"
        };

        try
        {
            options.Validate();
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "source URL or path");
        }
    }

    [TestMethod]
    public void Validate_ThrowsForMissingTargetUrl()
    {
        var options = new PipelinedMigrationOptions
        {
            SourceUrl = "svn://example/source",
            TargetUrl = " "
        };

        try
        {
            options.Validate();
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "target URL or path");
        }
    }
}
