using System;
using System.Buffers.Binary;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
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
