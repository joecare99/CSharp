using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        Assert.AreEqual(0x37A75081u, manifest.EncryptionKey);
        Assert.AreEqual(113, manifest.Assets.Count);
        Assert.AreEqual(18, manifest.Tables.Count);
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "par.cfg"));
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "FOKO.DBF"));
        Assert.IsFalse(manifest.Assets.Any(static asset =>
            asset.FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
            asset.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)));
        string[] resourceNames = typeof(StructureTemplateMaterializer).Assembly.GetManifestResourceNames();
        Assert.IsTrue(resourceNames.Contains("AhnWin52Backup.Core.StructureTemplate.assets.zip"));
        Assert.IsFalse(resourceNames.Any(static name =>
            name.StartsWith("AhnWin52Backup.Core.StructureTemplate.Files.", StringComparison.Ordinal)));

        StructureTemplateTable namesdays = manifest.Tables.Single(static table => table.FileName == "chnt.DB");
        Assert.AreEqual(168, namesdays.ExpectedRecords);
        Assert.AreEqual("AhnWin52Backup.Core.namesdays.json", namesdays.BaselineData?.ResourceName);
        Assert.AreEqual(168, namesdays.BaselineData?.ExpectedRecords);

        StructureTemplateTable individuals = manifest.Tables.Single(static table => table.FileName == "AWD.DB");
        Assert.AreEqual(8, individuals.SecondaryIndexes.Count);
        Assert.IsTrue(individuals.SecondaryIndexes.Any(static index =>
            index.Label.Equals("gebo", StringComparison.OrdinalIgnoreCase) &&
            index.KeyFields.Contains("Gebort", StringComparer.OrdinalIgnoreCase)));

        StructureTemplateTable over = manifest.Tables.Single(static table => table.FileName == "over.DB");
        Assert.IsFalse(over.GeneratedFromSchema);
        Assert.AreEqual(17, over.Fields.Count);
        Assert.AreEqual(2, over.SecondaryIndexes.Count);
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "over.db"));

        StructureTemplateTable notes = manifest.Tables.Single(static table => table.FileName == "nol.db");
        Assert.IsFalse(notes.GeneratedFromSchema);
        Assert.AreEqual("nol.MB", notes.MemoFile);
        Assert.AreEqual(3, notes.Fields.Count);
        Assert.AreEqual(1, notes.SecondaryIndexes.Count);
        Assert.IsTrue(manifest.Assets.Any(static asset => asset.FileName == "nol.db"));
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
            Assert.IsTrue(File.Exists(Path.Combine(destination, "over.db")));
            Assert.IsTrue(File.Exists(Path.Combine(destination, "over.PX")));
            Assert.IsTrue(File.Exists(Path.Combine(destination, "nol.db")));
            Assert.IsTrue(File.Exists(Path.Combine(destination, "nol.MB")));
            Assert.AreEqual(
                0,
                tables.Single(static table =>
                    table.FileName.Equals("over.DB", StringComparison.OrdinalIgnoreCase))
                    .Schema.DeclaredRecordCount);
            Assert.AreEqual(
                0,
                tables.Single(static table =>
                    table.FileName.Equals("nol.db", StringComparison.OrdinalIgnoreCase))
                    .Schema.DeclaredRecordCount);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void MaterializeGenerated_CreatesParadoxFamiliesFromSchemasAndKeepsBaselineRows()
    {
        string root = NewDirectory();
        try
        {
            string reference = Path.Combine(root, "reference-database");
            string destination = Path.Combine(root, "generated-database");

            StructureTemplateManifest referenceManifest = _materializer.Materialize(reference);
            StructureTemplateManifest manifest = _materializer.MaterializeGenerated(destination);

            Assert.AreEqual(referenceManifest.IndexMaximumTableSize, manifest.IndexMaximumTableSize);
            Assert.IsTrue(Directory.Exists(destination));
            Assert.AreEqual(manifest.Assets.Count, Directory.EnumerateFiles(destination).Count());
            foreach (StructureTemplateAsset asset in manifest.Assets.Where(static asset =>
                         asset.FileName.StartsWith("over.", StringComparison.OrdinalIgnoreCase) ||
                         asset.FileName.StartsWith("nol.", StringComparison.OrdinalIgnoreCase)))
            {
                CollectionAssert.AreEqual(
                    File.ReadAllBytes(Path.Combine(reference, asset.FileName)),
                    File.ReadAllBytes(Path.Combine(destination, asset.FileName)),
                    asset.FileName);
            }
            IReadOnlyList<ParadoxTableInventory> inventory = new ParadoxDirectoryInventory().Read(destination);
            Assert.AreEqual(manifest.Tables.Count, inventory.Count);
            foreach (StructureTemplateTable expected in manifest.Tables)
            {
                ParadoxTableSchema actual = inventory.Single(table =>
                    table.FileName.Equals(expected.FileName, StringComparison.OrdinalIgnoreCase)).Schema;
                Assert.AreEqual(expected.Version, actual.Version, expected.FileName);
                Assert.AreEqual(expected.RecordSize, actual.Fields.Sum(static field => field.Length), expected.FileName);
                Assert.AreEqual(expected.Encrypted, actual.Encrypted, expected.FileName);
                byte[] database = File.ReadAllBytes(Path.Combine(destination, expected.FileName));
                byte[] referenceDatabase = File.ReadAllBytes(Path.Combine(reference, expected.FileName));
                CollectionAssert.AreEqual(
                    expected.DatabaseFieldNumbers.ToArray(),
                    ReadDatabaseFieldNumbers(database),
                    expected.FileName);
                Assert.AreEqual(expected.DatabaseSortOrder, ReadDatabaseSortOrder(database), expected.FileName);
                CollectionAssert.AreEqual(
                    ReadDatabaseFieldNumbers(referenceDatabase),
                    ReadDatabaseFieldNumbers(database),
                    $"{expected.FileName} reference field numbers");
                Assert.AreEqual(
                    ReadDatabaseSortOrder(referenceDatabase),
                    ReadDatabaseSortOrder(database),
                    $"{expected.FileName} reference sort order");
                Assert.AreEqual(expected.MaximumTableSize, (int)database[5], expected.FileName);
                Assert.AreEqual(referenceDatabase[5], database[5], expected.FileName);
                AssertHeaderMetadata(referenceDatabase, database, expected.FileName);
                Assert.AreEqual(
                    expected.Encrypted ? manifest.EncryptionKey : 0,
                    ParadoxRecordWriter.ReadEncryption(database),
                    expected.FileName);
            }

            ParadoxTable individuals = new ParadoxTableReader().Read(Path.Combine(destination, "AWD.DB"));
            Assert.AreEqual(0, individuals.Records.Count);
            Assert.AreEqual(1, individuals.Fields.Count(field => field.TypeCode == 0x10));

            ParadoxTable namesdays = new ParadoxTableReader().Read(Path.Combine(destination, "chnt.DB"));
            Assert.AreEqual(168, namesdays.Records.Count);
            Assert.IsTrue(namesdays.Records.All(static record => record.Count == 4));

            foreach (StructureTemplateTable table in manifest.Tables)
            {
                string primaryPath = Path.Combine(destination, table.PrimaryIndexFile);
                Assert.IsTrue(File.Exists(primaryPath));
                byte[] primary = File.ReadAllBytes(primaryPath);
                byte[] referencePrimary = File.ReadAllBytes(Path.Combine(reference, table.PrimaryIndexFile));
                Assert.AreEqual(manifest.IndexMaximumTableSize, (int)primary[5], table.PrimaryIndexFile);
                Assert.AreEqual(referencePrimary[5], primary[5], table.PrimaryIndexFile);
                AssertHeaderMetadata(referencePrimary, primary, table.PrimaryIndexFile);
                Assert.AreEqual(
                    table.Encrypted ? manifest.EncryptionKey : 0,
                    ParadoxRecordWriter.ReadEncryption(primary),
                    table.PrimaryIndexFile);
                ParadoxRecordWriter.TableHeader primaryHeader =
                    ParadoxRecordWriter.ReadHeader(primary, 1, primaryPath);
                Assert.AreEqual(1, primaryHeader.FieldCount, table.PrimaryIndexFile);
                Assert.AreEqual(0, primaryHeader.PrimaryKeyFieldCount, table.PrimaryIndexFile);
                Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(
                    primary.AsSpan(0x1E)), table.PrimaryIndexFile);
                Assert.AreEqual(
                    BinaryPrimitives.ReadUInt16LittleEndian(referencePrimary.AsSpan(0x0C)),
                    primaryHeader.BlockCount,
                    table.PrimaryIndexFile);
                Assert.AreEqual(table.SecondaryIndexes.Count,
                    inventory.Single(item => item.FileName.Equals(table.FileName, StringComparison.OrdinalIgnoreCase))
                        .SecondaryIndexes.Count);
                if (table.ExpectedRecords == 0)
                {
                    Assert.AreEqual(table.ExpectedPrimaryIndexBlocks, primaryHeader.BlockCount, table.PrimaryIndexFile);
                    Assert.AreEqual(0, primary[0x20], table.PrimaryIndexFile);
                    AssertEncryptedEmptyBlocks(primary, primaryHeader, table.PrimaryIndexFile);
                    if (primaryHeader.BlockCount > 0)
                    {
                        AssertInitialIndexBlocksMatch(
                            primary,
                            referencePrimary,
                            primaryHeader,
                            table.PrimaryIndexFile);
                    }
                }

                foreach (StructureTemplateIndex index in table.SecondaryIndexes)
                {
                    string xgPath = Path.Combine(destination, index.XgFile);
                    string ygPath = Path.Combine(destination, index.YgFile);
                    Assert.IsTrue(File.Exists(xgPath));
                    Assert.IsTrue(File.Exists(ygPath));
                    byte[] xg = File.ReadAllBytes(xgPath);
                    byte[] yg = File.ReadAllBytes(ygPath);
                    byte[] referenceXg = File.ReadAllBytes(Path.Combine(reference, index.XgFile));
                    byte[] referenceYg = File.ReadAllBytes(Path.Combine(reference, index.YgFile));
                    Assert.AreEqual(table.MaximumTableSize, (int)xg[5], index.XgFile);
                    Assert.AreEqual(manifest.IndexMaximumTableSize, (int)yg[5], index.YgFile);
                    Assert.AreEqual(referenceXg[5], xg[5], index.XgFile);
                    Assert.AreEqual(referenceYg[5], yg[5], index.YgFile);
                    AssertHeaderMetadata(referenceXg, xg, index.XgFile);
                    AssertHeaderMetadata(referenceYg, yg, index.YgFile);
                    ParadoxRecordWriter.TableHeader xgHeader =
                        ParadoxRecordWriter.ReadHeader(xg, 8, xgPath);
                    ParadoxRecordWriter.TableHeader ygHeader =
                        ParadoxRecordWriter.ReadHeader(yg, 7, ygPath);
                    ParadoxRecordWriter.TableHeader referenceXgHeader =
                        ParadoxRecordWriter.ReadHeader(referenceXg, 8, index.XgFile);
                    ParadoxRecordWriter.TableHeader referenceYgHeader =
                        ParadoxRecordWriter.ReadHeader(referenceYg, 7, index.YgFile);
                    Assert.AreEqual(index.Fields.Count, xgHeader.FieldCount, index.XgFile);
                    Assert.AreEqual(index.KeyFields.Count, xgHeader.PrimaryKeyFieldCount, index.XgFile);
                    Assert.AreEqual(index.ExpectedXgRecords, xgHeader.RecordCount, index.XgFile);
                    Assert.AreEqual(index.ExpectedYgRecords, ygHeader.RecordCount, index.YgFile);
                    Assert.AreEqual(0, ygHeader.PrimaryKeyFieldCount, index.YgFile);
                    Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(yg.AsSpan(0x1E)), index.YgFile);
                    Assert.AreEqual(referenceXgHeader.BlockCount, xgHeader.BlockCount, index.XgFile);
                    Assert.AreEqual(referenceYgHeader.BlockCount, ygHeader.BlockCount, index.YgFile);
                    Assert.AreEqual(
                        table.Encrypted ? manifest.EncryptionKey : 0,
                        ParadoxRecordWriter.ReadEncryption(xg),
                        index.XgFile);
                    Assert.AreEqual(
                        table.Encrypted ? manifest.EncryptionKey : 0,
                        ParadoxRecordWriter.ReadEncryption(yg),
                        index.YgFile);
                    int[] expectedFieldMap = index.KeyFields
                        .Select((fieldName, fieldIndex) =>
                            fieldIndex == index.KeyFields.Count - 1
                                ? index.KeyFields.Count
                                : table.Fields.ToList().FindIndex(field =>
                                    field.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase)) + 1)
                        .Append(index.Fields.Count)
                        .ToArray();
                    CollectionAssert.AreEqual(
                        expectedFieldMap,
                        ReadSecondaryFieldMap(xg, index.SortOrder, index.Label),
                        index.XgFile);
                    Assert.AreEqual(0, xg[0x14], $"{index.XgFile} modified-flags byte");
                    Assert.AreEqual(expectedFieldMap[0], (int)xg[0x15], $"{index.XgFile} leading key field ordinal");
                    if (table.ExpectedRecords == 0)
                    {
                        Assert.AreEqual(index.ExpectedXgBlocks, xgHeader.BlockCount, index.XgFile);
                        Assert.AreEqual(index.ExpectedYgBlocks, ygHeader.BlockCount, index.YgFile);
                        AssertEncryptedEmptyBlocks(xg, xgHeader, index.XgFile);
                        AssertEncryptedEmptyBlocks(yg, ygHeader, index.YgFile);
                        if (xgHeader.BlockCount > 0)
                        {
                            AssertInitialIndexBlocksMatch(xg, referenceXg, xgHeader, index.XgFile);
                        }

                        if (ygHeader.BlockCount > 0)
                        {
                            AssertInitialIndexBlocksMatch(yg, referenceYg, ygHeader, index.YgFile);
                        }
                    }
                }

                if (table.MemoFile is not null)
                {
                    byte[] memo = File.ReadAllBytes(Path.Combine(destination, table.MemoFile));
                    Assert.AreEqual(4096, memo.Length, table.MemoFile);
                    if (table.Encrypted)
                    {
                        ParadoxBlockCipher.DecryptMemoBlock(memo, manifest.EncryptionKey);
                    }

                    Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(memo.AsSpan(1)), table.MemoFile);
                    Assert.AreEqual(0x1000,
                        BinaryPrimitives.ReadUInt16LittleEndian(memo.AsSpan(0x0B)), table.MemoFile);
                }
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static void AssertEncryptedEmptyBlocks(
        byte[] file,
        ParadoxRecordWriter.TableHeader header,
        string fileName)
    {
        int blockSize = header.BlockSize;
        uint encryption = ParadoxRecordWriter.ReadEncryption(file);
        for (int blockNumber = 1; blockNumber <= header.BlockCount; blockNumber++)
        {
            byte[] block = file.AsSpan(header.HeaderSize + (blockNumber - 1) * blockSize, blockSize).ToArray();
            if (encryption != 0)
            {
                ParadoxBlockCipher.DecryptDatabaseBlock(block, encryption, blockNumber);
            }

            Assert.AreEqual(
                blockNumber == header.BlockCount ? 0 : blockNumber + 1,
                (int)BinaryPrimitives.ReadUInt16LittleEndian(block),
                $"{fileName} block {blockNumber} next link");
            Assert.AreEqual(
                blockNumber - 1,
                (int)BinaryPrimitives.ReadUInt16LittleEndian(block.AsSpan(2)),
                $"{fileName} block {blockNumber} previous link");
            Assert.AreEqual(
                -header.RecordSize,
                (int)BinaryPrimitives.ReadInt16LittleEndian(block.AsSpan(4)),
                $"{fileName} block {blockNumber} empty data size");
        }
    }

    private static void AssertHeaderMetadata(byte[] reference, byte[] generated, string fileName)
    {
        Assert.AreEqual(
            BinaryPrimitives.ReadUInt16LittleEndian(reference.AsSpan(0x12)),
            BinaryPrimitives.ReadUInt16LittleEndian(generated.AsSpan(0x12)),
            $"{fileName} header word 0x12");
        Assert.AreEqual(reference[0x29], generated[0x29], $"{fileName} sort-order code");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x2C, 4).ToArray(),
            generated.AsSpan(0x2C, 4).ToArray(),
            $"{fileName} revision bytes");
        Assert.AreEqual(
            BinaryPrimitives.ReadUInt32LittleEndian(reference.AsSpan(0x49)),
            BinaryPrimitives.ReadUInt32LittleEndian(generated.AsSpan(0x49)),
            $"{fileName} autoincrement metadata");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x38, 8).ToArray(),
            generated.AsSpan(0x38, 8).ToArray(),
            $"{fileName} extended file flags");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x16, 8).ToArray(),
            generated.AsSpan(0x16, 8).ToArray(),
            $"{fileName} index workspace metadata");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x30, 0x19).ToArray(),
            generated.AsSpan(0x30, 0x19).ToArray(),
            $"{fileName} pointer and common header metadata");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x4D, 0x0B).ToArray(),
            generated.AsSpan(0x4D, 0x0B).ToArray(),
            $"{fileName} trailing file-header metadata");
        CollectionAssert.AreEqual(
            reference.AsSpan(0x58, 0x20).ToArray(),
            generated.AsSpan(0x58, 0x20).ToArray(),
            $"{fileName} extended/reserved header metadata");
    }

    private static void AssertInitialIndexBlocksMatch(
        byte[] generatedFile,
        byte[] referenceFile,
        ParadoxRecordWriter.TableHeader generatedHeader,
        string fileName)
    {
        ParadoxRecordWriter.TableHeader referenceHeader =
            ParadoxRecordWriter.ReadHeader(referenceFile, generatedFile[4], fileName);
        Assert.AreEqual(referenceHeader.BlockCount, generatedHeader.BlockCount, fileName);
        Assert.AreEqual(referenceHeader.BlockSize, generatedHeader.BlockSize, fileName);

        uint generatedEncryption = ParadoxRecordWriter.ReadEncryption(generatedFile);
        uint referenceEncryption = ParadoxRecordWriter.ReadEncryption(referenceFile);
        for (int blockNumber = 1; blockNumber <= generatedHeader.BlockCount; blockNumber++)
        {
            byte[] generatedBlock = generatedFile.AsSpan(
                generatedHeader.HeaderSize + (blockNumber - 1) * generatedHeader.BlockSize,
                generatedHeader.BlockSize).ToArray();
            byte[] referenceBlock = referenceFile.AsSpan(
                referenceHeader.HeaderSize + (blockNumber - 1) * referenceHeader.BlockSize,
                referenceHeader.BlockSize).ToArray();
            if (generatedEncryption != 0)
            {
                ParadoxBlockCipher.DecryptDatabaseBlock(generatedBlock, generatedEncryption, blockNumber);
            }

            if (referenceEncryption != 0)
            {
                ParadoxBlockCipher.DecryptDatabaseBlock(referenceBlock, referenceEncryption, blockNumber);
            }

            CollectionAssert.AreEqual(referenceBlock, generatedBlock, $"{fileName} block {blockNumber}");
        }
    }

    private static int[] ReadSecondaryFieldMap(byte[] file, string expectedSortOrder, string expectedIndexLabel)
    {
        int fieldCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x21));
        int tableNameLength = file[0x39] == 0x0C ? 261 : 79;
        int offset = checked(0x78 + fieldCount * 2 + sizeof(uint) + fieldCount * sizeof(uint) + tableNameLength);
        for (int index = 0; index < fieldCount; index++)
        {
            while (offset < file.Length && file[offset] != 0)
            {
                offset++;
            }

            if (offset >= file.Length)
            {
                throw new InvalidDataException("An XG field name is not terminated within its header.");
            }

            offset++;
        }

        int[] fieldMap = new int[fieldCount];
        for (int index = 0; index < fieldCount; index++)
        {
            fieldMap[index] = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(offset));
            offset += sizeof(ushort);
        }

        string sortOrder = ReadNullTerminatedAscii(file, ref offset);
        string indexLabel = ReadNullTerminatedAscii(file, ref offset);
        Assert.AreEqual(expectedSortOrder, sortOrder, "XG sort order");
        Assert.AreEqual(expectedIndexLabel, indexLabel, "XG index label");
        return fieldMap;
    }

    private static string ReadNullTerminatedAscii(byte[] file, ref int offset)
    {
        int start = offset;
        while (offset < file.Length && file[offset] != 0)
        {
            offset++;
        }

        if (offset >= file.Length)
        {
            throw new InvalidDataException("XG index metadata is not terminated within its header.");
        }

        string value = Encoding.ASCII.GetString(file, start, offset - start);
        offset++;
        return value;
    }

    private static int[] ReadDatabaseFieldNumbers(byte[] file)
    {
        int fieldCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x21));
        int tableNameLength = file[0x39] == 0x0C ? 261 : 79;
        int offset = checked(0x78 + fieldCount * 2 + sizeof(uint) + fieldCount * sizeof(uint) + tableNameLength);
        for (int index = 0; index < fieldCount; index++)
        {
            _ = ReadNullTerminatedAscii(file, ref offset);
        }

        int[] fieldNumbers = new int[fieldCount];
        for (int index = 0; index < fieldCount; index++)
        {
            fieldNumbers[index] = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(offset));
            offset += sizeof(ushort);
        }

        return fieldNumbers;
    }

    private static string ReadDatabaseSortOrder(byte[] file)
    {
        int fieldCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x21));
        int tableNameLength = file[0x39] == 0x0C ? 261 : 79;
        int offset = checked(0x78 + fieldCount * 2 + sizeof(uint) + fieldCount * sizeof(uint) + tableNameLength);
        for (int index = 0; index < fieldCount; index++)
        {
            _ = ReadNullTerminatedAscii(file, ref offset);
        }

        offset = checked(offset + fieldCount * sizeof(ushort));
        return ReadNullTerminatedAscii(file, ref offset);
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
