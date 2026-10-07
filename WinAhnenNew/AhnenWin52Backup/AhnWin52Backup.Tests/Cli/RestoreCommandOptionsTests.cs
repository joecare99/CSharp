using System;
using AhnWin52Backup.Cli.Commands;

namespace AhnWin52Backup.Tests.Cli;

[TestClass]
public sealed class RestoreCommandOptionsTests
{
    [TestMethod]
    public void TryParse_DefaultsToExistingTemplateMode()
    {
        bool parsed = RestoreCommandOptions.TryParse(
            ["restore", "backup.hej", "database"],
            out RestoreCommandOptions? options,
            out string error);

        Assert.IsTrue(parsed, error);
        Assert.IsNotNull(options);
        Assert.AreEqual(RestoreMode.ExistingTemplate, options.Mode);
        Assert.IsFalse(options.Force);
        Assert.AreEqual("backup.hej", System.IO.Path.GetFileName(options.InputPath));
        Assert.AreEqual("database", System.IO.Path.GetFileName(options.DestinationPath));
    }

    [TestMethod]
    public void TryParse_AcceptsFromScratchAndForceInEitherPosition()
    {
        bool parsed = RestoreCommandOptions.TryParse(
            ["restore", "--fromscratch", "--force", "backup.hej", "new-database"],
            out RestoreCommandOptions? options,
            out string error);

        Assert.IsTrue(parsed, error);
        Assert.IsNotNull(options);
        Assert.AreEqual(RestoreMode.FromScratch, options.Mode);
        Assert.IsTrue(options.Force);
    }

    [TestMethod]
    public void TryParse_RejectsConflictingModes()
    {
        bool parsed = RestoreCommandOptions.TryParse(
            ["restore", "--fromscratch", "--override", "backup.hej", "database"],
            out RestoreCommandOptions? options,
            out string error);

        Assert.IsFalse(parsed);
        Assert.IsNull(options);
        StringAssert.Contains(error, "only one");
    }

    [TestMethod]
    public void TryParse_RejectsDuplicateForce()
    {
        bool parsed = RestoreCommandOptions.TryParse(
            ["restore", "--force", "--force", "backup.hej", "database"],
            out RestoreCommandOptions? options,
            out string error);

        Assert.IsFalse(parsed);
        Assert.IsNull(options);
        StringAssert.Contains(error, "only once");
    }

    [TestMethod]
    public void TryParse_RejectsExistingFromScratchDestination()
    {
        string destination = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"ahwb-restore-existing-{Guid.NewGuid():N}");
        System.IO.Directory.CreateDirectory(destination);
        try
        {
            bool parsed = RestoreCommandOptions.TryParse(
                ["restore", "--fromscratch", "backup.hej", destination],
                out RestoreCommandOptions? options,
                out string error);

            Assert.IsFalse(parsed);
            Assert.IsNull(options);
            StringAssert.Contains(error, "already exists");
        }
        finally
        {
            System.IO.Directory.Delete(destination);
        }
    }

    [TestMethod]
    public void TryParse_RejectsNonHejInput()
    {
        bool parsed = RestoreCommandOptions.TryParse(
            ["restore", "backup.txt", "database"],
            out RestoreCommandOptions? options,
            out string error);

        Assert.IsFalse(parsed);
        Assert.IsNull(options);
        StringAssert.Contains(error, ".hej");
    }
}
