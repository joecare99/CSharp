using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxEmptyTemplateInsertionTests
{
    private const string TemplateEnvironmentVariable = "AHWB_EMPTY_TEMPLATE_DIRECTORY";
    private const string TrialEnvironmentVariable = "AHWB_EMPTY_TRIAL_DIRECTORY";
    private const string TrialDirectoryName = "AHNENWIN_Empty2_pxlib_trial12";

    [TestMethod]
    public void InsertSyntheticRecords_IntoASeparateTemplateCopy()
    {
        string? templateDirectory = Environment.GetEnvironmentVariable(TemplateEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(templateDirectory))
        {
            Assert.Inconclusive(
                $"Set {TemplateEnvironmentVariable} to run the AhnWin template insertion experiment.");
            return;
        }

        string templatePath = Path.GetFullPath(templateDirectory);
        Assert.AreEqual("AHNENWIN_Empty2", Path.GetFileName(templatePath), ignoreCase: true);
        string parentDirectory = Path.GetDirectoryName(templatePath)
            ?? throw new InvalidOperationException("The template must have a parent directory.");
        string trialDirectory = Path.GetFullPath(
            Environment.GetEnvironmentVariable(TrialEnvironmentVariable) is string configuredTrial &&
            !string.IsNullOrWhiteSpace(configuredTrial)
                ? configuredTrial
                : Path.Combine(parentDirectory, TrialDirectoryName));
        Assert.AreEqual(parentDirectory, Path.GetDirectoryName(trialDirectory), ignoreCase: true);
        Assert.IsFalse(Directory.Exists(trialDirectory), $"Trial directory already exists: {trialDirectory}");

        CopyTemplate(templatePath, trialDirectory);
        Dictionary<string, byte> primaryIndexMutationBytes = Directory
            .EnumerateFiles(trialDirectory, "*.PX", SearchOption.TopDirectoryOnly)
            .ToDictionary(
                static path => Path.GetFileNameWithoutExtension(path),
                static path => File.ReadAllBytes(path)[0x2C],
                StringComparer.OrdinalIgnoreCase);
        string[] indexedTableNames = ["AWD", "MRG", "adp", "LOC", "sour2"];
        string[] secondaryDataPaths = Directory
            .EnumerateFiles(trialDirectory, "*.XG*", SearchOption.TopDirectoryOnly)
            .Where(path => indexedTableNames.Any(tableName => IsIndexSidecar(path, tableName, "XG")))
            .ToArray();
        string[] secondaryIndexPaths = Directory
            .EnumerateFiles(trialDirectory, "*.YG*", SearchOption.TopDirectoryOnly)
            .Where(path => indexedTableNames.Any(tableName => IsIndexSidecar(path, tableName, "YG")))
            .ToArray();
        Assert.AreEqual(17, secondaryDataPaths.Length);
        Assert.AreEqual(17, secondaryIndexPaths.Length);
        IParadoxTableReader reader = new ParadoxTableReader();
        ParadoxRecordWriter writer = new(reader);

        writer.AppendRecords(
            Path.Combine(trialDirectory, "AWD.DB"),
            "Nummer",
            [
                Record(
                    ("Nummer", "1"),
                    ("Vater", "0"),
                    ("Mutter", "0"),
                    ("Name", "Testmann"),
                    ("Vornamen", "Ada"),
                    ("Geschlecht", "W"),
                    ("Gebort", "Pxlib-Testort"),
                    ("Quelleg", "ProbeSrc")),
                Record(
                    ("Nummer", "2"),
                    ("Vater", "0"),
                    ("Mutter", "0"),
                    ("Name", "Testpartner"),
                    ("Vornamen", "Ben"),
                    ("Geschlecht", "M"))
            ]);
        writer.AppendRecords(
            Path.Combine(trialDirectory, "MRG.DB"),
            "Numr",
            [
                Record(
                    ("Numr", "1"),
                    ("Nummer", "1"),
                    ("Epnum", "2"),
                    ("Htag", "1"),
                    ("Hmonat", "1"),
                    ("Hjahr", "1900"),
                    ("Hort", "Pxlib-Testort"),
                    ("Hqu", "ProbeSrc"))
            ]);
        writer.AppendRecords(
            Path.Combine(trialDirectory, "adp.DB"),
            "Nummer",
            [Record(("Nummer", "1"), ("Av", "1"), ("Am", "2"))]);
        writer.AppendRecords(
            Path.Combine(trialDirectory, "LOC.db"),
            "Ort",
            [Record(("Ort", "Pxlib-Testort"), ("Land", "Testland"))]);
        writer.AppendRecords(
            Path.Combine(trialDirectory, "sour2.DB"),
            "N",
            [Record(("N", "1"), ("Titel", "Probe source"), ("Abk", "ProbeSrc"))]);

        ParadoxSecondaryIndexWriter secondaryIndexWriter = new(reader);
        foreach (string databaseFileName in new[] { "AWD.DB", "MRG.DB", "adp.DB", "LOC.db", "sour2.DB" })
        {
            secondaryIndexWriter.RebuildForTable(Path.Combine(trialDirectory, databaseFileName));
        }

        ParadoxTable individuals = reader.Read(Path.Combine(trialDirectory, "AWD.DB"));
        Assert.AreEqual(2, individuals.Records.Count);
        Assert.AreEqual("Testmann", individuals.Records[0][3]);
        Assert.AreEqual("Ada", individuals.Records[0][4]);
        Assert.AreEqual("0", individuals.Records[0][1]);
        Assert.AreEqual("0", individuals.Records[0][2]);
        Assert.AreEqual("Pxlib-Testort", individuals.Records[0][11]);
        Assert.AreEqual("ProbeSrc", individuals.Records[0][27]);
        Assert.AreEqual("Testpartner", individuals.Records[1][3]);
        Assert.AreEqual("2", individuals.Records[1][0]);

        byte[] individualBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "AWD.DB"));
        int nameFieldIndex = Enumerable.Range(0, individuals.Fields.Count)
            .Single(index => string.Equals(individuals.Fields[index].Name, "Name", StringComparison.OrdinalIgnoreCase));
        int nameOffset = 6 + individuals.Fields.Take(nameFieldIndex).Sum(static field => field.Length);
        byte[] nameBytes = individualBlock.AsSpan(nameOffset, individuals.Fields[nameFieldIndex].Length).ToArray();
        byte[] expectedNameBytes = System.Text.Encoding.ASCII.GetBytes("Testmann")
            .Concat(new byte[nameBytes.Length - "Testmann".Length])
            .ToArray();
        CollectionAssert.AreEqual(expectedNameBytes, nameBytes);

        int emptyAlphaFieldIndex = Enumerable.Range(0, individuals.Fields.Count)
            .Single(index => string.Equals(individuals.Fields[index].Name, "Religion", StringComparison.OrdinalIgnoreCase));
        int emptyAlphaOffset = 6 + individuals.Fields.Take(emptyAlphaFieldIndex).Sum(static field => field.Length);
        CollectionAssert.AreEqual(
            new byte[individuals.Fields[emptyAlphaFieldIndex].Length],
            individualBlock.AsSpan(emptyAlphaOffset, individuals.Fields[emptyAlphaFieldIndex].Length).ToArray());

        byte[] primaryIndexFile = File.ReadAllBytes(Path.Combine(trialDirectory, "AWD.PX"));
        int primaryHeaderSize = BinaryPrimitives.ReadUInt16LittleEndian(primaryIndexFile.AsSpan(2, sizeof(ushort)));
        int primaryBlockSize = primaryIndexFile[5] * 1024;
        byte[] primaryBlock = primaryIndexFile.AsSpan(primaryHeaderSize, primaryBlockSize).ToArray();
        uint primaryEncryption = BinaryPrimitives.ReadUInt32LittleEndian(primaryIndexFile.AsSpan(0x25, sizeof(uint)));
        if (primaryEncryption == 0xFF00FF00)
        {
            primaryEncryption = BinaryPrimitives.ReadUInt32LittleEndian(primaryIndexFile.AsSpan(0x5C, sizeof(uint)));
        }

        ParadoxBlockCipher.DecryptDatabaseBlock(primaryBlock, primaryEncryption, 1);
        int primaryKeyLength = individuals.Fields
            .Single(static field => string.Equals(field.Name, "Nummer", StringComparison.OrdinalIgnoreCase))
            .Length;
        CollectionAssert.AreEqual(
            new byte[] { 0x80, 0x01, 0x80, 0x02, 0x80, 0x00 },
            primaryBlock.AsSpan(6 + primaryKeyLength, 6).ToArray());

        ParadoxTable marriages = reader.Read(Path.Combine(trialDirectory, "MRG.DB"));
        Assert.AreEqual(1, marriages.Records.Count);
        Assert.AreEqual("2", marriages.Records[0][2]);
        Assert.AreEqual("Pxlib-Testort", marriages.Records[0][6]);
        Assert.AreEqual("ProbeSrc", marriages.Records[0][18]);
        Assert.AreEqual(1, reader.Read(Path.Combine(trialDirectory, "adp.DB")).Records.Count);
        Assert.AreEqual("Pxlib-Testort", reader.Read(Path.Combine(trialDirectory, "LOC.db")).Records[0][0]);
        Assert.AreEqual(
            "Probe source",
            reader.Read(Path.Combine(trialDirectory, "sour2.DB")).Records[0][1]);

        foreach ((string tableName, int recordCount) in new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                 {
                     ["AWD"] = 2,
                     ["MRG"] = 1,
                     ["adp"] = 1,
                     ["LOC"] = 1,
                     ["sour2"] = 1
                 })
        {
            byte[] primaryIndex = File.ReadAllBytes(Path.Combine(trialDirectory, $"{tableName}.PX"));
            Assert.AreEqual(
                unchecked((byte)(primaryIndexMutationBytes[tableName] + recordCount)),
                primaryIndex[0x2C],
                $"The primary-index change marker for {tableName} was not advanced.");
        }

        foreach (string secondaryDataPath in secondaryDataPaths)
        {
            string fileName = Path.GetFileName(secondaryDataPath);
            string tableName = fileName[..fileName.LastIndexOf(".XG", StringComparison.OrdinalIgnoreCase)];
            int expectedRecordCount = tableName.ToUpperInvariant() switch
            {
                "AWD" => 2,
                "MRG" or "ADP" or "LOC" or "SOUR2" => 1,
                _ => throw new AssertFailedException($"Unexpected secondary-index table {tableName}.")
            };
            byte[] secondaryData = File.ReadAllBytes(secondaryDataPath);
            Assert.AreEqual(8, secondaryData[4], fileName);
            Assert.AreEqual(
                expectedRecordCount,
                BinaryPrimitives.ReadInt32LittleEndian(secondaryData.AsSpan(6)),
                fileName);
            Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(secondaryData.AsSpan(0x0C)), fileName);

            string suffix = fileName[(fileName.LastIndexOf(".XG", StringComparison.OrdinalIgnoreCase) + 3)..];
            string secondaryIndexPath = Path.Combine(trialDirectory, $"{tableName}.YG{suffix}");
            byte[] secondaryIndex = File.ReadAllBytes(secondaryIndexPath);
            Assert.AreEqual(7, secondaryIndex[4], Path.GetFileName(secondaryIndexPath));
            Assert.AreEqual(1, BinaryPrimitives.ReadInt32LittleEndian(secondaryIndex.AsSpan(6)));
            Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(secondaryIndex.AsSpan(0x0C)));
        }

        byte[] firstSecondaryDataBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "AWD.XG0"));
        CollectionAssert.AreEqual(
            new byte[] { 0x80, 0x00, 0x00, 0x01 },
            firstSecondaryDataBlock.AsSpan(6 + 20, 4).ToArray());
        CollectionAssert.AreEqual(
            new byte[] { 0x80, 0x00, 0x00, 0x02 },
            firstSecondaryDataBlock.AsSpan(6 + 26 + 20, 4).ToArray());

        byte[] firstSecondaryIndexBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "AWD.YG0"));
        CollectionAssert.AreEqual(
            firstSecondaryDataBlock.AsSpan(6, 24).ToArray(),
            firstSecondaryIndexBlock.AsSpan(6, 24).ToArray());
        CollectionAssert.AreEqual(
            new byte[] { 0x80, 0x01, 0x80, 0x02, 0x80, 0x00 },
            firstSecondaryIndexBlock.AsSpan(6 + 24, 6).ToArray());

        byte[] nameIndexBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "AWD.XG2"));
        CollectionAssert.AreEqual(
            System.Text.Encoding.ASCII.GetBytes("TESTMANN"),
            nameIndexBlock.AsSpan(6, "TESTMANN".Length).ToArray());

        byte[] sourceIndexBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "sour2.XG0"));
        CollectionAssert.AreEqual(
            System.Text.Encoding.ASCII.GetBytes("PROBE SOURCE"),
            sourceIndexBlock.AsSpan(6, "PROBE SOURCE".Length).ToArray());

        byte[] placeIndexBlock = ReadDecryptedBlock(Path.Combine(trialDirectory, "LOC.XG0"));
        CollectionAssert.AreEqual(
            System.Text.Encoding.ASCII.GetBytes("PXLIB-TESTORT"),
            placeIndexBlock.AsSpan(6, "PXLIB-TESTORT".Length).ToArray());
        CollectionAssert.AreEqual(
            System.Text.Encoding.ASCII.GetBytes("Pxlib-Testort"),
            placeIndexBlock.AsSpan(6 + 40, "Pxlib-Testort".Length).ToArray());

        Console.WriteLine($"AhnWin trial database created at: {trialDirectory}");
        Console.WriteLine("All XG secondary records and their YG primary indexes were written.");
    }

    private static byte[] ReadDecryptedBlock(string path)
    {
        byte[] file = File.ReadAllBytes(path);
        int headerSize = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(2, sizeof(ushort)));
        int blockSize = file[5] * 1024;
        byte[] block = file.AsSpan(headerSize, blockSize).ToArray();
        uint encryption = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x25, sizeof(uint)));
        if (encryption == 0xFF00FF00)
        {
            encryption = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x5C, sizeof(uint)));
        }

        if (encryption != 0)
        {
            ParadoxBlockCipher.DecryptDatabaseBlock(block, encryption, 1);
        }

        return block;
    }

    private static bool IsIndexSidecar(string path, string tableName, string indexExtension)
    {
        string fileName = Path.GetFileName(path);
        string prefix = $"{tableName}.{indexExtension}";
        if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string suffix = fileName[prefix.Length..];
        return suffix.Length > 0 && suffix.All(char.IsAsciiDigit);
    }

    private static IReadOnlyDictionary<string, string?> Record(params (string Field, string Value)[] values) =>
        values.ToDictionary(static item => item.Field, static item => (string?)item.Value, StringComparer.OrdinalIgnoreCase);

    private static void CopyTemplate(string sourceDirectory, string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);
        foreach (string sourceSubdirectory in Directory.EnumerateDirectories(
                     sourceDirectory,
                     "*",
                     SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDirectory, sourceSubdirectory);
            Directory.CreateDirectory(Path.Combine(destinationDirectory, relativePath));
        }

        foreach (string sourceFile in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceDirectory, sourceFile);
            string destinationFile = Path.Combine(destinationDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)
                ?? throw new InvalidOperationException("A copied file has no parent directory."));
            File.Copy(sourceFile, destinationFile, overwrite: false);
        }

        string[] originalFiles = Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(sourceDirectory, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
        string[] copiedFiles = Directory.EnumerateFiles(destinationDirectory, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(destinationDirectory, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
        CollectionAssert.AreEqual(originalFiles, copiedFiles);
        foreach (string relativePath in originalFiles)
        {
            string sourcePath = Path.Combine(sourceDirectory, relativePath);
            string destinationPath = Path.Combine(destinationDirectory, relativePath);
            CollectionAssert.AreEqual(File.ReadAllBytes(sourcePath), File.ReadAllBytes(destinationPath), relativePath);
        }
    }
}
