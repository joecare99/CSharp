using RepoMigrator.Tools.ArchiveSmokeTest;

namespace RepoMigrator.Tools.ArchiveSmokeTest.Tests;

[TestClass]
public sealed class ArchiveSmokeTestOptionsTests
{
    [TestMethod]
    public void Parse_ReturnsExpectedOptions_ForRepeatableExtensions()
    {
        var tempWorkspace = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tools.ArchiveSmokeTest.Tests", Guid.NewGuid().ToString("N"), "workspace");
        var tempSource = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tools.ArchiveSmokeTest.Tests", Guid.NewGuid().ToString("N"), "archives");

        var args = new[]
        {
            "--source", tempSource,
            "--workspace", tempWorkspace,
            "--recursive",
            "--extension", ".zip",
            "--extension", ".tar.gz"
        };

        var options = ArchiveSmokeTestOptions.Parse(args);

        Assert.IsNotNull(options);
        Assert.AreEqual(Path.GetFullPath(tempSource), options.SourceDirectoryPath);
        Assert.AreEqual(Path.GetFullPath(tempWorkspace), options.WorkspaceRootPath);
        Assert.IsTrue(options.Recursive);
        CollectionAssert.AreEqual(new[] { ".zip", ".tar.gz" }, options.AllowedExtensions.ToArray());
    }

    [TestMethod]
    public void Parse_DefaultsWorkspaceToCurrentDirectory_WhenNotProvided()
    {
        var tempSource = Path.Combine(Path.GetTempPath(), "RepoMigrator.Tools.ArchiveSmokeTest.Tests", Guid.NewGuid().ToString("N"), "archives");

        var options = ArchiveSmokeTestOptions.Parse(new[] { "--source", tempSource });

        Assert.IsNotNull(options);
        Assert.AreEqual(Directory.GetCurrentDirectory(), options.WorkspaceRootPath);
    }

    [TestMethod]
    public void Parse_ThrowsForMissingSource()
    {
        try
        {
            _ = ArchiveSmokeTestOptions.Parse(new[] { "--workspace", @"C:\workspace" });
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException ex)
        {
            StringAssert.Contains(ex.Message, "--source");
        }
    }

    [TestMethod]
    public void Parse_ReturnsNullForHelp()
    {
        var options = ArchiveSmokeTestOptions.Parse(new[] { "--help" });

        Assert.IsNull(options);
    }
}
