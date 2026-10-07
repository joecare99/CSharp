using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Workflows;

[TestClass]
public sealed class HejDatabaseRestoreServiceTests
{
    [TestMethod]
    public void Restore_FromScratchWritesAndVerifiesFiveSectionsAndMemoText()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            HejDocument source = CreateDocument();
            string inputPath = WriteHejFile(root, source);
            string destination = Path.Combine(root, "restored");
            HejDatabaseRestoreResult result = CreateService().Restore(
                inputPath,
                destination,
                HejDatabaseRestoreMode.FromScratch,
                force: false);

            Assert.IsTrue(Directory.Exists(destination));
            Assert.AreEqual(1, result.RecordCounts["Individuals"]);
            Assert.AreEqual(1, result.RecordCounts["Marriages"]);
            Assert.AreEqual(1, result.RecordCounts["Adoptions"]);
            Assert.AreEqual(1, result.RecordCounts["Places"]);
            Assert.AreEqual(1, result.RecordCounts["Sources"]);

            byte[] individualsDatabase = File.ReadAllBytes(Path.Combine(destination, "AWD.DB"));
            StringAssert.Contains(Encoding.ASCII.GetString(individualsDatabase), "intl850");

            HejDocument restored = new HejDatabaseExportService(new ParadoxTableReader()).Export(destination);
            Assert.AreEqual(source.Individuals[0].Fields[31], Normalize(restored.Individuals[0].Fields[31]));
            Assert.AreEqual(source.Individuals[0].Fields[3], restored.Individuals[0].Fields[3]);
            ParadoxTable surnameList = new ParadoxTableReader().Read(Path.Combine(destination, "NM.db"));
            Assert.AreEqual(1, surnameList.Records.Count);
            Assert.AreEqual("Restore", surnameList.Records[0][0]);
            Assert.IsTrue(File.Exists(Path.Combine(destination, "over.db")));
            Assert.IsTrue(File.Exists(Path.Combine(destination, "over.YG1")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Restore_OverridePreservesUnrelatedDirectoryAssets()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string destination = Path.Combine(root, "existing");
            new StructureTemplateMaterializer().Materialize(destination);
            string sentinel = Path.Combine(destination, "user.keep");
            File.WriteAllText(sentinel, "preserve");
            byte[] overBefore = File.ReadAllBytes(Path.Combine(destination, "over.db"));
            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(
                Path.Combine(destination, "NM.db"),
                "Name",
                [new Dictionary<string, string?> { ["Name"] = "OldSurname" }]);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(Path.Combine(destination, "NM.db"));
            string inputPath = WriteHejFile(root, CreateDocument());

            HejDatabaseRestoreResult result = CreateService().Restore(
                inputPath,
                destination,
                HejDatabaseRestoreMode.Override,
                force: false);

            Assert.AreEqual("preserve", File.ReadAllText(Path.Combine(destination, "user.keep")));
            Assert.AreEqual(1, result.RecordCounts["Individuals"]);
            Assert.AreEqual(1, new ParadoxTableReader().Read(Path.Combine(destination, "AWD.DB")).Records.Count);
            Assert.AreEqual(
                "Restore",
                new ParadoxTableReader().Read(Path.Combine(destination, "NM.db")).Records.Single()[0]);
            Assert.IsTrue(File.Exists(Path.Combine(destination, "over.db")));
            CollectionAssert.AreEqual(overBefore, File.ReadAllBytes(Path.Combine(destination, "over.db")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Restore_ExistingTemplatePublishesValidatedRestore()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string destination = Path.Combine(root, "template");
            new StructureTemplateMaterializer().Materialize(destination);
            string inputPath = WriteHejFile(root, CreateDocument());

            HejDatabaseRestoreResult result = CreateService().Restore(
                inputPath,
                destination,
                HejDatabaseRestoreMode.ExistingTemplate,
                force: false);

            Assert.AreEqual(1, result.RecordCounts["Individuals"]);
            Assert.AreEqual(1, new ParadoxTableReader().Read(Path.Combine(destination, "AWD.DB")).Records.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void Restore_RequiresForceToAcceptCompatibilityWarnings()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string inputPath = WriteHejFile(root, CreateDocument());
            byte[] bytes = File.ReadAllBytes(inputPath);
            int nameOffset = Encoding.ASCII.GetByteCount("1\u000F0\u000F0\u000F");
            bytes[nameOffset] = 0x01;
            File.WriteAllBytes(inputPath, bytes);
            string destination = Path.Combine(root, "must-not-exist");

            InvalidDataException exception = Assert.Throws<InvalidDataException>(() =>
                CreateService().Restore(
                    inputPath,
                    destination,
                    HejDatabaseRestoreMode.FromScratch,
                    force: false));

            StringAssert.Contains(exception.Message, "pass --force");
            Assert.IsFalse(Directory.Exists(destination));

            HejDatabaseRestoreResult result = CreateService().Restore(
                inputPath,
                destination,
                HejDatabaseRestoreMode.FromScratch,
                force: true);
            Assert.IsTrue(Directory.Exists(destination));
            Assert.AreEqual(1, result.Warnings.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static HejDatabaseRestoreService CreateService()
    {
        ParadoxTableReader reader = new();
        HejCodec codec = new();
        return new HejDatabaseRestoreService(
            codec,
            new HejDatabaseExportService(reader),
            reader,
            new StructureTemplateMaterializer());
    }

    private static string WriteHejFile(string directory, HejDocument document)
    {
        string path = Path.Combine(directory, "input.hej");
        using FileStream destination = new(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        new HejCodec().Write(document, destination);
        return path;
    }

    private static HejDocument CreateDocument()
    {
        string[] individuals = EmptyFields(HejSection.Individuals);
        individuals[0] = "1";
        individuals[1] = "0";
        individuals[2] = "0";
        individuals[3] = "Restore";
        individuals[4] = "Probe";
        individuals[31] = "memo\tline one\nline two";

        string[] marriages = EmptyFields(HejSection.Marriages);
        marriages[0] = "1";
        marriages[1] = "2";

        string[] adoptions = EmptyFields(HejSection.Adoptions);
        adoptions[0] = "1";
        adoptions[1] = "1";
        adoptions[2] = "2";

        string[] places = EmptyFields(HejSection.Places);
        places[0] = "Restore Place";

        string[] sources = EmptyFields(HejSection.Sources);
        sources[0] = "Restore Source";
        sources[1] = "RS";

        return new HejDocument(
            [new HejRecord(individuals)],
            [new HejRecord(marriages)],
            [new HejRecord(adoptions)],
            [new HejRecord(places)],
            [new HejRecord(sources)]);
    }

    private static string[] EmptyFields(HejSection section) =>
        Enumerable.Repeat(string.Empty, HejSchema.GetFieldCount(section)).ToArray();

    private static string Normalize(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
}
