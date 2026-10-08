using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxMultiBlockWriterTests
{
    [TestMethod]
    public void BuildIndexFile_SupportsThreeLevels()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string templatePath = Path.Combine(databaseDirectory, "AWD.YG2");
            byte[] template = File.ReadAllBytes(templatePath);
            ParadoxRecordWriter.TableHeader templateHeader =
                ParadoxRecordWriter.ReadHeader(template, 7, templatePath);
            int entriesPerNode = (templateHeader.BlockSize - 6) / templateHeader.RecordSize;
            int entryCount = checked(entriesPerNode * entriesPerNode + 1);
            int keyLength = templateHeader.RecordSize - 6;
            ParadoxIndexFileBuilder.IndexEntry[] entries = Enumerable
                .Range(1, entryCount)
                .Select(index =>
                {
                    byte[] key = new byte[keyLength];
                    BinaryPrimitives.WriteInt32BigEndian(key, index);
                    return new ParadoxIndexFileBuilder.IndexEntry(key, index, 1);
                })
                .ToArray();

            byte[] rebuilt = ParadoxIndexFileBuilder.Build(template, 7, entries);

            ParadoxRecordWriter.TableHeader rebuiltHeader =
                ParadoxRecordWriter.ReadHeader(rebuilt, 7, "rebuilt AWD.YG2");
            int leafCount = (entryCount + entriesPerNode - 1) / entriesPerNode;
            int parentCount = (leafCount + entriesPerNode - 1) / entriesPerNode;
            int rootBlock = BinaryPrimitives.ReadUInt16LittleEndian(rebuilt.AsSpan(0x1E, sizeof(ushort)));
            Assert.AreEqual(3, rebuilt[0x20]);
            Assert.AreEqual(leafCount + parentCount + 1, rebuiltHeader.BlockCount);
            Assert.AreEqual(rootBlock, rebuiltHeader.BlockCount);
            Assert.AreEqual(entryCount + leafCount + parentCount, rebuiltHeader.RecordCount);

            byte[] root = rebuilt
                .AsSpan(rebuiltHeader.HeaderSize + (rootBlock - 1) * rebuiltHeader.BlockSize, rebuiltHeader.BlockSize)
                .ToArray();
            ParadoxBlockCipher.DecryptDatabaseBlock(
                root,
                ParadoxRecordWriter.ReadEncryption(rebuilt),
                rootBlock);
            Assert.AreEqual((short)templateHeader.RecordSize, BinaryPrimitives.ReadInt16LittleEndian(root.AsSpan(4, 2)));
            int childLinkOffset = 6 + keyLength;
            Assert.AreEqual(
                leafCount + 1,
                ReadParadoxShort(root.AsSpan(childLinkOffset, sizeof(short))));
            Assert.AreEqual(
                leafCount + 2,
                ReadParadoxShort(root.AsSpan(childLinkOffset + templateHeader.RecordSize, sizeof(short))));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow(2, "AWD.XG2")]
    [DataRow(3, "AWD.XG3")]
    [DataRow(4, "AWD.XG4")]
    public void RebuildSecondaryIndexes_PreservesNameKeySpellingAndSortsWithoutCase(
        int indexNumber,
        string secondaryFileName)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string databasePath = Path.Combine(databaseDirectory, "AWD.DB");
            IReadOnlyDictionary<string, string?>[] records =
            [
                new Dictionary<string, string?> { ["Nummer"] = "1", ["Name"] = "apple" },
                new Dictionary<string, string?> { ["Nummer"] = "2", ["Name"] = "Banana" }
            ];

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "Nummer", records);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

            StructureTemplateTable individuals = new StructureTemplateMaterializer()
                .ReadManifest()
                .Tables
                .Single(static table => table.FileName.Equals("AWD.DB", StringComparison.OrdinalIgnoreCase));
            StructureTemplateIndex index = individuals.SecondaryIndexes
                .Single(candidate => candidate.Number == indexNumber);
            int nameOffset = index.Fields
                .TakeWhile(static field => !field.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
                .Sum(static field => field.Length);
            int nameLength = index.Fields
                .Single(static field => field.Name.Equals("Name", StringComparison.OrdinalIgnoreCase))
                .Length;
            string secondaryPath = Path.Combine(databaseDirectory, secondaryFileName);
            byte[] secondaryFile = File.ReadAllBytes(secondaryPath);
            ParadoxRecordWriter.TableHeader header =
                ParadoxRecordWriter.ReadHeader(secondaryFile, 8, secondaryPath);
            Assert.AreEqual(2, header.RecordCount);
            byte[] block = secondaryFile
                .AsSpan(header.HeaderSize, header.BlockSize)
                .ToArray();
            ParadoxBlockCipher.DecryptDatabaseBlock(
                block,
                ParadoxRecordWriter.ReadEncryption(secondaryFile),
                1);

            int firstNameOffset = 6 + nameOffset;
            string firstName = Encoding.ASCII.GetString(block, firstNameOffset, nameLength).TrimEnd('\0', ' ');
            string secondName = Encoding.ASCII.GetString(
                block,
                firstNameOffset + header.RecordSize,
                nameLength).TrimEnd('\0', ' ');

            Assert.AreEqual("apple", firstName);
            Assert.AreEqual("Banana", secondName);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void RebuildSecondaryIndexes_StoresSourceDatabaseBlockInGebnamHint()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string databasePath = Path.Combine(databaseDirectory, "AWD.DB");
            byte[] emptyDatabase = File.ReadAllBytes(databasePath);
            ParadoxRecordWriter.TableHeader databaseHeader =
                ParadoxRecordWriter.ReadHeader(emptyDatabase, 0, databasePath);
            int recordsPerDatabaseBlock = ParadoxRecordWriter.GetRecordsPerDataBlock(
                databaseHeader.BlockSize,
                databaseHeader.RecordSize);
            IReadOnlyDictionary<string, string?>[] records = Enumerable
                .Range(1, recordsPerDatabaseBlock + 1)
                .Select(static key => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?>
                {
                    ["Nummer"] = key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["Name"] = "Baker"
                })
                .ToArray();

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "Nummer", records);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

            string secondaryPath = Path.Combine(databaseDirectory, "AWD.XG4");
            byte[] secondaryFile = File.ReadAllBytes(secondaryPath);
            ParadoxRecordWriter.TableHeader secondaryHeader =
                ParadoxRecordWriter.ReadHeader(secondaryFile, 8, secondaryPath);
            byte[] block = secondaryFile
                .AsSpan(secondaryHeader.HeaderSize, secondaryHeader.BlockSize)
                .ToArray();
            ParadoxBlockCipher.DecryptDatabaseBlock(
                block,
                ParadoxRecordWriter.ReadEncryption(secondaryFile),
                1);

            int keyLength = 4 + 2 + 2 + 45 + 45 + 4;
            int lastRecordOffset = 6 + recordsPerDatabaseBlock * secondaryHeader.RecordSize;
            CollectionAssert.AreEqual(
                new byte[] { 0x80, 0x02 },
                block.AsSpan(lastRecordOffset + keyLength, 2).ToArray());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void RebuildSecondaryIndexes_UsesObservedSparseXgBlockOccupancy()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string databasePath = Path.Combine(databaseDirectory, "AWD.DB");
            string secondaryPath = Path.Combine(databaseDirectory, "AWD.XG2");
            byte[] emptySecondary = File.ReadAllBytes(secondaryPath);
            ParadoxRecordWriter.TableHeader secondaryHeader =
                ParadoxRecordWriter.ReadHeader(emptySecondary, 8, secondaryPath);
            int physicalCapacity = (secondaryHeader.BlockSize - 6) / secondaryHeader.RecordSize;
            int sparseCapacity = physicalCapacity * 65 / 100;
            int requestedRecords = sparseCapacity + 1;
            IReadOnlyDictionary<string, string?>[] records = Enumerable
                .Range(1, requestedRecords)
                .Select(static key => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?>
                {
                    ["Nummer"] = key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["Name"] = "Baker"
                })
                .ToArray();

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "Nummer", records);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

            byte[] rebuiltSecondary = File.ReadAllBytes(secondaryPath);
            ParadoxRecordWriter.TableHeader rebuiltXgHeader =
                ParadoxRecordWriter.ReadHeader(rebuiltSecondary, 8, secondaryPath);
            Assert.AreEqual(requestedRecords, rebuiltXgHeader.RecordCount);
            Assert.AreEqual(2, rebuiltXgHeader.BlockCount);

            string indexPath = Path.Combine(databaseDirectory, "AWD.YG2");
            byte[] rebuiltIndex = File.ReadAllBytes(indexPath);
            ParadoxRecordWriter.TableHeader rebuiltYgHeader =
                ParadoxRecordWriter.ReadHeader(rebuiltIndex, 7, indexPath);
            Assert.AreEqual(2, rebuiltYgHeader.RecordCount);
            Assert.AreEqual(1, rebuiltYgHeader.BlockCount);
            Assert.AreEqual(1, rebuiltIndex[0x20]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void AppendAndRebuildSecondaryIndexes_SupportsRecordsAcrossBlocks()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string databasePath = Path.Combine(databaseDirectory, "adp.DB");
            byte[] header = File.ReadAllBytes(databasePath);
            ParadoxRecordWriter.TableHeader tableHeader =
                ParadoxRecordWriter.ReadHeader(header, 0, databasePath);
            int recordsPerBlock = (tableHeader.BlockSize - 6) / tableHeader.RecordSize;
            int requestedRecords = recordsPerBlock + 3;
            IReadOnlyDictionary<string, string?>[] records = Enumerable
                .Range(1, requestedRecords)
                .Select(static key => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?>
                {
                    ["Nummer"] = key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["Av"] = "1",
                    ["Am"] = "2"
                })
                .ToArray();

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "Nummer", records);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

            ParadoxTable restored = reader.Read(databasePath);
            Assert.AreEqual(requestedRecords, restored.Records.Count);
            Assert.AreEqual("1", restored.Records[0][0]);
            Assert.AreEqual(requestedRecords.ToString(System.Globalization.CultureInfo.InvariantCulture),
                restored.Records[^1][0]);

            byte[] outputHeader = File.ReadAllBytes(databasePath);
            ParadoxRecordWriter.TableHeader outputTableHeader =
                ParadoxRecordWriter.ReadHeader(outputHeader, 0, databasePath);
            Assert.AreEqual(2, outputTableHeader.BlockCount);
            Assert.AreEqual(requestedRecords, outputTableHeader.RecordCount);

            string primaryPath = Path.ChangeExtension(databasePath, ".PX");
            ParadoxRecordWriter.TableHeader primaryHeader = ParadoxRecordWriter.ReadHeader(
                File.ReadAllBytes(primaryPath),
                1,
                primaryPath);
            Assert.AreEqual(2, primaryHeader.RecordCount);

            foreach (string xgPath in Directory.EnumerateFiles(databaseDirectory, "adp.XG*", SearchOption.TopDirectoryOnly))
            {
                string xgFileName = Path.GetFileName(xgPath);
                string suffix = Path.GetExtension(xgPath);
                string ygPath = Path.Combine(
                    databaseDirectory,
                    xgFileName.Replace(".XG", ".YG", StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(File.Exists(ygPath), $"Missing paired index for {xgPath}.");
                ParadoxRecordWriter.TableHeader xgHeader = ParadoxRecordWriter.ReadHeader(
                    File.ReadAllBytes(xgPath),
                    8,
                    xgPath);
                Assert.AreEqual(requestedRecords, xgHeader.RecordCount, suffix);
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static int ReadParadoxShort(ReadOnlySpan<byte> value) =>
        ((value[0] & 0x7F) << 8) | value[1];
}
