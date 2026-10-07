using System;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Workflows;

[TestClass]
public sealed class StructureTemplateMaterializerTests
{
    private readonly StructureTemplateMaterializer _materializer = new();

    [TestMethod]
    public void ReadManifest_DescribesAllTablesAssetsAndNamesdayBaseline()
    {
        StructureTemplateManifest manifest = _materializer.ReadManifest();

        Assert.AreEqual(1, manifest.SchemaVersion);
        Assert.AreEqual("ahnwin52-empty2-structure-v1", manifest.TemplateId);
        Assert.AreEqual(102, manifest.Assets.Count);
        Assert.AreEqual(16, manifest.Tables.Count);
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "par.cfg"));
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "FOKO.DBF"));
        Assert.IsFalse(manifest.Assets.Any(static asset =>
            asset.FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
            asset.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)));

        StructureTemplateTable namesdays = manifest.Tables.Single(static table => table.FileName == "chnt.DB");
        Assert.AreEqual(168, namesdays.ExpectedRecords);
        Assert.AreEqual("AhnWin52Backup.Core.namesdays.json", namesdays.BaselineData?.ResourceName);
        Assert.AreEqual(168, namesdays.BaselineData?.ExpectedRecords);

        StructureTemplateTable individuals = manifest.Tables.Single(static table => table.FileName == "AWD.DB");
        Assert.AreEqual(8, individuals.SecondaryIndexes.Count);
        Assert.IsTrue(individuals.SecondaryIndexes.Any(static index =>
            index.Label.Equals("gebo", StringComparison.OrdinalIgnoreCase) &&
            index.KeyFields.Contains("Gebort", StringComparer.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Materialize_CreatesAndValidatesCompleteStructureInNewDirectory()
    {
        string root = NewDirectory();
        try
        {
            string destination = Path.Combine(root, "database");

            StructureTemplateManifest manifest = _materializer.Materialize(destination);

            Assert.IsTrue(Directory.Exists(destination));
            Assert.AreEqual(manifest.Assets.Count, Directory.EnumerateFiles(destination).Count());
            _materializer.ValidateDirectory(destination);

            ParadoxDirectoryInventory inventory = new();
            var tables = inventory.Read(destination);
            Assert.AreEqual(manifest.Tables.Count, tables.Count);
            Assert.AreEqual(
                168,
                new ParadoxTableReader().Read(Path.Combine(destination, "chnt.DB")).Records.Count);
            Assert.AreEqual(0, tables.Single(static table => table.FileName == "AWD.DB").Schema.DeclaredRecordCount);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Materialize_RefusesExistingDestinationWithoutChangingIt()
    {
        string root = NewDirectory();
        try
        {
            string destination = Path.Combine(root, "existing");
            Directory.CreateDirectory(destination);
            string sentinel = Path.Combine(destination, "keep.txt");
            File.WriteAllText(sentinel, "preserve");

            Assert.ThrowsExactly<IOException>(() => _materializer.Materialize(destination));

            Assert.AreEqual("preserve", File.ReadAllText(sentinel));
            Assert.AreEqual(1, Directory.EnumerateFiles(destination).Count());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Materialize_RequiresAnExistingParentDirectory()
    {
        string root = NewDirectory();
        try
        {
            string destination = Path.Combine(root, "missing-parent", "database");

            Assert.ThrowsExactly<DirectoryNotFoundException>(() => _materializer.Materialize(destination));

            Assert.IsFalse(Directory.Exists(Path.GetDirectoryName(destination)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string NewDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), $"ahwb-template-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }
}
