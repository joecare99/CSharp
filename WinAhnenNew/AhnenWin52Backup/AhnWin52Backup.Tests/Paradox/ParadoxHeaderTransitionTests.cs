using System;
using System.Buffers.Binary;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxHeaderTransitionTests
{
    [TestMethod]
    [DataRow(0, 2, 0, 1, 0)]
    [DataRow(1, 1, 1, 0, 1)]
    [DataRow(6, 2, 0, 1, 1)]
    [DataRow(8, 2, 0, 1, 1)]
    [DataRow(7, 1, 1, 0, 1)]
    public void OneBlock_HeaderCountersFollowNativeTransitions(
        int fileType, int recordCount, int indexChange, int headerChange, int metadataChange)
    {
        byte[] header = new byte[0x58];
        header[4] = (byte)fileType;
        header[0x2C] = 42;
        header[0x2D] = 73;
        header[0x2E] = 19;
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x49, 4), 0x0000FFFF);

        ParadoxRecordWriter.UpdateSingleBlockHeader(header, recordCount);

        Assert.AreEqual(recordCount, BinaryPrimitives.ReadInt32LittleEndian(header.AsSpan(6, 4)));
        Assert.AreEqual(42 + indexChange, header[0x2C]);
        Assert.AreEqual(73 + headerChange, header[0x2D]);
        Assert.AreEqual(19, header[0x2E]);
        Assert.AreEqual(0x0000FFFFu + (uint)metadataChange,
            BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0x49, 4)));
    }

    [TestMethod]
    public void SecondaryRevision_FollowsPrimaryRevisionIncludingWraparound()
    {
        byte[] header = new byte[0x58];
        header[0x2F] = 0xFF;

        ParadoxSecondaryIndexWriter.SynchronizePrimaryRevision(header, 0);

        Assert.AreEqual(0, header[0x2F]);
        Assert.ThrowsExactly<InvalidDataException>(() =>
            ParadoxSecondaryIndexWriter.SynchronizePrimaryRevision(header, 2));
        Assert.AreEqual(0, header[0x2F]);
    }

    [TestMethod]
    public void EncodeRecord_MatchesCrossImplementationGoldenVector()
    {
        ParadoxField[] fields =
        [
            new("Label", 0x01, 7),
            new("Small", 0x03, 2),
            new("Large", 0x04, 4),
            new("Auto", 0x16, 4)
        ];

        byte[] encoded = ParadoxRecordWriter.EncodeRecord(
            fields,
            ["Ä€漢", "32767", "91041", "91042"]);

        CollectionAssert.AreEqual(ParadoxConformanceVectors.ReadHex("record-mixed-types"), encoded);
    }

    [TestMethod]
    public void PrimaryIndexBuilder_MatchesCrossImplementationGoldenVector()
    {
        byte[] template = new byte[0x800];
        BinaryPrimitives.WriteUInt16LittleEndian(template.AsSpan(0, 2), 8);
        BinaryPrimitives.WriteUInt16LittleEndian(template.AsSpan(2, 2), 0x800);
        template[4] = 1;
        template[5] = 2;
        BinaryPrimitives.WriteUInt16LittleEndian(template.AsSpan(0x21, 2), 1);
        template[0x58] = 0x01;
        template[0x59] = 2;
        ParadoxIndexFileBuilder.IndexEntry[] entries =
        [
            new([0x80, 0x01], 1, 2),
            new([0x80, 0x02], 2, 3)
        ];

        byte[] built = ParadoxIndexFileBuilder.Build(template, 1, entries);

        Assert.AreEqual(0x1000, built.Length);
        Assert.AreEqual(2, BinaryPrimitives.ReadInt32LittleEndian(built.AsSpan(6, 4)));
        Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(built.AsSpan(0x1E, 2)));
        Assert.AreEqual(1, built[0x20]);
        byte[] firstRecord = ParadoxConformanceVectors.ReadHex("index-record-one");
        byte[] secondRecord = ParadoxConformanceVectors.ReadHex("index-record-two");
        CollectionAssert.AreEqual(firstRecord[2..], built.AsSpan(0x800 + 8, 6).ToArray());
        CollectionAssert.AreEqual(secondRecord[2..], built.AsSpan(0x800 + 16, 6).ToArray());
    }

    [TestMethod]
    public void EmptyTableWriter_MatchesCrossImplementationFileVectors()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"ParadoxWriter-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databasePath = Path.Combine(directory, "synthetic.DB");
            byte[] databaseTemplate = new byte[0x800];
            BinaryPrimitives.WriteUInt16LittleEndian(databaseTemplate.AsSpan(0, 2), 8);
            BinaryPrimitives.WriteUInt16LittleEndian(databaseTemplate.AsSpan(2, 2), 0x800);
            databaseTemplate[4] = 0;
            databaseTemplate[5] = 2;
            BinaryPrimitives.WriteUInt16LittleEndian(databaseTemplate.AsSpan(0x21, 2), 2);
            BinaryPrimitives.WriteUInt16LittleEndian(databaseTemplate.AsSpan(0x23, 2), 1);
            databaseTemplate[0x39] = 0x0C;
            databaseTemplate[0x78] = 0x04;
            databaseTemplate[0x79] = 4;
            databaseTemplate[0x7A] = 0x01;
            databaseTemplate[0x7B] = 4;
            int tableNameOffset = 0x78 + (2 * 2) + 4 + (2 * 4);
            Encoding.ASCII.GetBytes("Synthetic\0").CopyTo(databaseTemplate, tableNameOffset);
            int fieldNamesOffset = tableNameOffset + 261;
            Encoding.ASCII.GetBytes("ID\0Label\0").CopyTo(databaseTemplate, fieldNamesOffset);
            File.WriteAllBytes(databasePath, databaseTemplate);

            byte[] primaryIndexTemplate = new byte[0x800];
            BinaryPrimitives.WriteUInt16LittleEndian(primaryIndexTemplate.AsSpan(0, 2), 10);
            BinaryPrimitives.WriteUInt16LittleEndian(primaryIndexTemplate.AsSpan(2, 2), 0x800);
            primaryIndexTemplate[4] = 1;
            primaryIndexTemplate[5] = 2;
            BinaryPrimitives.WriteUInt16LittleEndian(primaryIndexTemplate.AsSpan(0x21, 2), 1);
            primaryIndexTemplate[0x58] = 0x04;
            primaryIndexTemplate[0x59] = 4;
            File.WriteAllBytes(Path.ChangeExtension(databasePath, ".PX"), primaryIndexTemplate);

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(
                databasePath,
                "ID",
                [
                    new Dictionary<string, string?> { ["ID"] = "2", ["Label"] = "B" },
                    new Dictionary<string, string?> { ["ID"] = "1", ["Label"] = "A" }
                ]);

            byte[] database = File.ReadAllBytes(databasePath);
            Assert.AreEqual(0x1000, database.Length);
            Assert.AreEqual(2, BinaryPrimitives.ReadInt32LittleEndian(database.AsSpan(6, 4)));
            Assert.AreEqual(1, BinaryPrimitives.ReadUInt16LittleEndian(database.AsSpan(0x0C, 2)));
            CollectionAssert.AreEqual(
                new byte[] { 0x80, 0, 0, 1, (byte)'A', 0, 0, 0 },
                database.AsSpan(0x800 + 6, 8).ToArray());
            CollectionAssert.AreEqual(
                new byte[] { 0x80, 0, 0, 2, (byte)'B', 0, 0, 0 },
                database.AsSpan(0x800 + 14, 8).ToArray());

            byte[] primaryIndex = File.ReadAllBytes(Path.ChangeExtension(databasePath, ".PX"));
            Assert.AreEqual(0x1000, primaryIndex.Length);
            CollectionAssert.AreEqual(
                new byte[] { 0x80, 0, 0, 1, 0x80, 1, 0x80, 2, 0x80, 0 },
                primaryIndex.AsSpan(0x800 + 6, 10).ToArray());
            ParadoxTable table = reader.Read(databasePath);
            Assert.AreEqual(2, table.Records.Count);
            Assert.AreEqual("1", table.Records[0][0]);
            Assert.AreEqual("A", table.Records[0][1]);
            Assert.AreEqual("2", table.Records[1][0]);
            Assert.AreEqual("B", table.Records[1][1]);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow(2833279u, 1, 2833280u)]
    [DataRow(91041u, 2, 91043u)]
    public void MissingAutoIncrementKeys_AreAllocatedFromLastIssuedValue(
        uint lastIssued, int count, uint expectedLastIssued)
    {
        byte[] header = new byte[0x58];
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x49, 4), lastIssued);
        ParadoxField[] fields = [new("N", 0x16, 4)];
        List<IReadOnlyDictionary<string, string?>> records = new();
        for (int index = 0; index < count; index++)
        {
            records.Add(new Dictionary<string, string?>());
        }

        IReadOnlyList<IReadOnlyDictionary<string, string?>> resolved =
            ParadoxRecordWriter.ResolveAutoIncrementRecords(fields, header, records);

        Assert.AreEqual(expectedLastIssued, BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0x49, 4)));
        for (int index = 0; index < count; index++)
        {
            string expected = (lastIssued + (uint)index + 1).ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(expected, resolved[index]["N"]);
            byte[] encoded = ParadoxRecordWriter.EncodeRecord(fields, resolved[index]);
            Assert.AreEqual(0x80, encoded[0]);
            Assert.AreEqual((int)(lastIssued + (uint)index + 1) & 0xFF, encoded[3]);
        }
    }

    [TestMethod]
    public void ExplicitAutoIncrementKey_DoesNotAdvanceHeader()
    {
        byte[] header = new byte[0x58];
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x49, 4), 2833279);
        ParadoxField[] fields = [new("Numr", 0x16, 4)];
        IReadOnlyList<IReadOnlyDictionary<string, string?>> records =
            [new Dictionary<string, string?> { ["Numr"] = "1" }];

        IReadOnlyList<IReadOnlyDictionary<string, string?>> resolved =
            ParadoxRecordWriter.ResolveAutoIncrementRecords(fields, header, records);

        Assert.AreEqual("1", resolved[0]["Numr"]);
        Assert.AreEqual(2833279u, BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0x49, 4)));
    }

    [TestMethod]
    public void ExhaustedAutoIncrementKey_IsRejectedWithoutHeaderChange()
    {
        byte[] header = new byte[0x58];
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x49, 4), int.MaxValue);
        ParadoxField[] fields = [new("N", 0x16, 4)];

        Assert.ThrowsExactly<InvalidDataException>(() => ParadoxRecordWriter.ResolveAutoIncrementRecords(
            fields, header, [new Dictionary<string, string?>()]));
        Assert.AreEqual((uint)int.MaxValue, BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(0x49, 4)));
    }
}
