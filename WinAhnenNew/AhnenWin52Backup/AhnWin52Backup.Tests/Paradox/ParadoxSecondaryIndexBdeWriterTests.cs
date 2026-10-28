using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxSecondaryIndexBdeWriterTests
{
    [TestMethod]
    [DoNotParallelize]
    public void BuildSecondaryIndexes_FromActiveRowsAfterBdeDeletion()
    {
        string? templateDirectory =
            Environment.GetEnvironmentVariable(
                "DBPARADOX_SECONDARY_BDE_DELETION_TEMPLATE_DIR");
        if (string.IsNullOrWhiteSpace(templateDirectory))
        {
            Assert.Inconclusive(
                "Set DBPARADOX_SECONDARY_BDE_DELETION_TEMPLATE_DIR to the disposable BDE deletion template directory.");
        }

        string outputDirectory = Path.Combine(
            Path.GetTempPath(), $"AhnWin52-SecondaryIndex-CSharp-Delete-{Guid.NewGuid():N}");
        Directory.CreateDirectory(outputDirectory);
        foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
        {
            File.Copy(
                sourcePath,
                Path.Combine(outputDirectory, Path.GetFileName(sourcePath)),
                overwrite: true);
        }

        string databasePath = Path.Combine(outputDirectory, "Synthetic.DB");
        IReadOnlyDictionary<string, string?>[] records =
        [
            Row("1", "Alpha", "B"),
            Row("2", "Alpha", "A"),
            Row("3", "Alpha", "A"),
            Row("4", "alpha", "A")
        ];
        ParadoxTableReader reader = new();
        new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);
        new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

        ParadoxTable table = reader.Read(databasePath);
        Assert.AreEqual(4, table.Records.Count);
        Assert.IsFalse(
            table.Records.Any(record => record[0] == "5"),
            "The BDE-deleted key must not be regenerated from the active-row input.");
    }

    [TestMethod]
    [DoNotParallelize]
    public void BuildSecondaryIndexes_WideKeysAcrossIndexTreeLevels()
    {
        string? templateDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR");
        if (string.IsNullOrWhiteSpace(templateDirectory))
        {
            Assert.Inconclusive(
                "Set DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR to the disposable BDE capacity template directory.");
        }

        string outputDirectory = Path.Combine(
            Path.GetTempPath(), $"AhnWin52-SecondaryIndex-CSharp-Capacity-{Guid.NewGuid():N}");
        Directory.CreateDirectory(outputDirectory);
        foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
        {
            File.Copy(
                sourcePath,
                Path.Combine(outputDirectory, Path.GetFileName(sourcePath)),
                overwrite: true);
        }

        string databasePath = Path.Combine(outputDirectory, "Synthetic.DB");
        IReadOnlyDictionary<string, string?>[] records =
            Enumerable.Range(1, 770)
                .Select(id => new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ID"] = id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["GroupKey"] = $"Group{(id - 1) % 17:D2}",
                    ["Suffix"] = $"Suffix{(id - 1) % 11:D2}"
                })
                .ToArray();

        ParadoxTableReader reader = new();
        new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);
        new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

        ParadoxTable table = reader.Read(databasePath);
        Assert.AreEqual(770, table.Records.Count);
        Assert.AreEqual("1", table.Records[0][0]);
        Assert.AreEqual("770", table.Records[^1][0]);
        foreach (string extension in new[] { ".XG0", ".YG0", ".XG1", ".YG1" })
        {
            string path = Path.ChangeExtension(databasePath, extension);
            byte expectedType = extension.StartsWith(".XG", StringComparison.OrdinalIgnoreCase)
                ? (byte)8
                : (byte)7;
            ParadoxRecordWriter.TableHeader header =
                ParadoxRecordWriter.ReadHeader(File.ReadAllBytes(path), expectedType, path);
            Assert.IsTrue(header.BlockCount > 1, $"{extension} must cross a block boundary.");
            if (extension.StartsWith(".YG", StringComparison.OrdinalIgnoreCase))
            {
                Assert.AreEqual(3, File.ReadAllBytes(path)[0x20]);
            }
        }
    }

    [TestMethod]
    [DoNotParallelize]
    public void BuildSecondaryIndexes_FromDisposableBdeTemplate()
    {
        string? templateDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR");
        string? outputDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_OUTPUT_DIR");
        if (string.IsNullOrWhiteSpace(templateDirectory))
        {
            Assert.Inconclusive(
                "Set DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR to the disposable BDE template directory.");
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            Assert.Inconclusive(
                "Set DBPARADOX_SECONDARY_BDE_OUTPUT_DIR to a disposable output directory.");
        }

        outputDirectory = Path.Combine(outputDirectory, $"run-{Guid.NewGuid():N}");
        Directory.CreateDirectory(outputDirectory);
        foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
        {
            File.Copy(
                sourcePath,
                Path.Combine(outputDirectory, Path.GetFileName(sourcePath)),
                overwrite: true);
        }

        string databasePath = Path.Combine(outputDirectory, "Synthetic.DB");
        IReadOnlyDictionary<string, string?>[] records =
        [
            Row("1", "Alpha", "B"),
            Row("2", "Alpha", "A"),
            Row("3", "Alpha", "A"),
            Row("4", "alpha", "A"),
            Row("5", "Beta", "A")
        ];
        ParadoxTableReader reader = new();
        new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);
        new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

        ParadoxTable table = reader.Read(databasePath);
        Assert.AreEqual(5, table.Records.Count);
        Assert.AreEqual("1", table.Records[0][0]);
        Assert.AreEqual("5", table.Records[^1][0]);
        foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
        {
            string extension = Path.GetExtension(sourcePath);
            Assert.IsTrue(
                File.Exists(Path.ChangeExtension(databasePath, extension)),
                $"Expected BDE-template family member {extension}.");
        }

        string[] secondaryIndexExtensions = [".XG0", ".XG1"];
        string caseInsensitiveExtension = secondaryIndexExtensions.Single(extension =>
            (File.ReadAllBytes(Path.ChangeExtension(databasePath, extension))[0x55] & 0x01) != 0);
        string caseSensitiveExtension = secondaryIndexExtensions.Single(
            extension => extension != caseInsensitiveExtension);
        byte[] caseInsensitiveIndex =
            File.ReadAllBytes(Path.ChangeExtension(databasePath, caseInsensitiveExtension));
        byte[] caseSensitiveIndex =
            File.ReadAllBytes(Path.ChangeExtension(databasePath, caseSensitiveExtension));
        ParadoxRecordWriter.TableHeader caseInsensitiveHeader =
            ParadoxRecordWriter.ReadHeader(
                caseInsensitiveIndex, 8, databasePath + caseInsensitiveExtension);
        ParadoxRecordWriter.TableHeader caseSensitiveHeader =
            ParadoxRecordWriter.ReadHeader(
                caseSensitiveIndex, 8, databasePath + caseSensitiveExtension);
        Assert.AreEqual(
            "ALPHA",
            Encoding.ASCII.GetString(
                caseInsensitiveIndex,
                caseInsensitiveHeader.HeaderSize + 6,
                5));
        Assert.AreEqual(
            "Alpha",
            Encoding.ASCII.GetString(
                caseSensitiveIndex,
                caseSensitiveHeader.HeaderSize + 6,
                5));

    }

    [TestMethod]
    [DoNotParallelize]
    public void BuildSecondaryIndexes_ReferencesRecordsAcrossTableDataBlocks()
    {
        string? templateDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR");
        string? outputDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_OUTPUT_DIR");
        if (string.IsNullOrWhiteSpace(templateDirectory) ||
            string.IsNullOrWhiteSpace(outputDirectory))
        {
            Assert.Inconclusive(
                "Set the disposable BDE template and output directories to run this test.");
        }

        outputDirectory = Path.Combine(outputDirectory, $"multiblock-{Guid.NewGuid():N}");
        Directory.CreateDirectory(outputDirectory);
        foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
        {
            File.Copy(
                sourcePath,
                Path.Combine(outputDirectory, Path.GetFileName(sourcePath)),
                overwrite: true);
        }

        string databasePath = Path.Combine(outputDirectory, "Synthetic.DB");
        IReadOnlyDictionary<string, string?>[] records =
            Enumerable.Range(1, 75)
                .Select(id => new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ID"] = id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["GroupKey"] = $"Group{(id - 1) % 5:D2}",
                    ["Suffix"] = ((char)('A' + (id - 1) % 3)).ToString()
                })
                .ToArray();

        ParadoxTableReader reader = new();
        new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);
        new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

        ParadoxTable table = reader.Read(databasePath);
        Assert.AreEqual(75, table.Records.Count);
        Assert.AreEqual("1", table.Records[0][0]);
        Assert.AreEqual("75", table.Records[^1][0]);
        ParadoxRecordWriter.TableHeader databaseHeader =
            ParadoxRecordWriter.ReadHeader(
                File.ReadAllBytes(databasePath), 0, databasePath);
        Assert.AreEqual(2, databaseHeader.BlockCount);
        foreach (string extension in new[] { ".PX", ".XG0", ".YG0", ".XG1", ".YG1" })
        {
            Assert.IsTrue(
                File.Exists(Path.ChangeExtension(databasePath, extension)),
                $"Expected BDE-template family member {extension}.");
        }

        byte[] secondaryData = File.ReadAllBytes(Path.ChangeExtension(databasePath, ".XG0"));
        ParadoxRecordWriter.TableHeader secondaryHeader =
            ParadoxRecordWriter.ReadHeader(
                secondaryData, 8, Path.ChangeExtension(databasePath, ".XG0"));
        Assert.AreEqual(2, secondaryHeader.BlockCount);
    }

    [TestMethod]
    [DoNotParallelize]
    public void BuildSecondaryIndexes_RejectsUnverifiedNonAsciiCaseFolding()
    {
        string? templateDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_TEMPLATE_DIR");
        string? outputDirectory =
            Environment.GetEnvironmentVariable("DBPARADOX_SECONDARY_BDE_OUTPUT_DIR");
        if (string.IsNullOrWhiteSpace(templateDirectory) ||
            string.IsNullOrWhiteSpace(outputDirectory))
        {
            Assert.Inconclusive(
                "Set the disposable BDE template and output directories to run this test.");
        }

        string runDirectory = Path.Combine(outputDirectory, $"unsupported-{Guid.NewGuid():N}");
        Directory.CreateDirectory(runDirectory);
        try
        {
            foreach (string sourcePath in Directory.EnumerateFiles(templateDirectory, "Synthetic.*"))
            {
                File.Copy(
                    sourcePath,
                    Path.Combine(runDirectory, Path.GetFileName(sourcePath)),
                    overwrite: true);
            }

            string databasePath = Path.Combine(runDirectory, "Synthetic.DB");
            IReadOnlyDictionary<string, string?>[] records =
            [
                new Dictionary<string, string?> { ["ID"] = "1", ["GroupKey"] = "Älpha", ["Suffix"] = "A" },
                new Dictionary<string, string?> { ["ID"] = "2", ["GroupKey"] = "Alpha", ["Suffix"] = "A" }
            ];

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);
            NotSupportedException error = Assert.Throws<NotSupportedException>(
                () => new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath));
            StringAssert.Contains(error.Message, "Non-ASCII case folding");
        }
        finally
        {
            Directory.Delete(runDirectory, recursive: true);
        }
    }

    private static IReadOnlyDictionary<string, string?> Row(
        string id,
        string groupKey,
        string suffix) =>
        new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["ID"] = id,
            ["GroupKey"] = groupKey,
            ["Suffix"] = suffix
        };
}
