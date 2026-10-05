using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxDirectoryInventoryTests
{
    [TestMethod]
    public void Read_UsesOnlyHeaderAndPairsSidecarsCaseInsensitively()
    {
        string directory = NewDirectory();
        try
        {
            WriteTable(directory, "Test.db", recordCount: 1);
            File.WriteAllBytes(Path.Combine(directory, "TEST.PX"), []);
            File.WriteAllBytes(Path.Combine(directory, "test.XG0"), []);
            File.WriteAllBytes(Path.Combine(directory, "test.YG0"), []);
            File.WriteAllBytes(Path.Combine(directory, "Test.MB"), []);

            var table = new ParadoxDirectoryInventory().Read(directory);

            Assert.HasCount(1, table);
            Assert.AreEqual("Test.db", table[0].FileName);
            Assert.AreEqual(1, table[0].Schema.DeclaredRecordCount);
            Assert.AreEqual("Key", table[0].Schema.Fields[0].Name);
            Assert.AreEqual(0x7F, table[0].Schema.Fields[0].TypeCode);
            Assert.IsTrue(table[0].HasMemoFile);
            Assert.HasCount(1, table[0].SecondaryIndexes);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void Read_RejectsMissingSecondaryPartner()
    {
        string directory = NewDirectory();
        try
        {
            WriteTable(directory, "Test.DB", recordCount: 0);
            File.WriteAllBytes(Path.Combine(directory, "Test.PX"), []);
            File.WriteAllBytes(Path.Combine(directory, "Test.XG0"), []);

            Assert.ThrowsExactly<InvalidDataException>(() => new ParadoxDirectoryInventory().Read(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void Read_RejectsOrphanAndMissingPrimaryIndex()
    {
        string directory = NewDirectory();
        try
        {
            WriteTable(directory, "Test.DB", recordCount: 0);
            Assert.ThrowsExactly<InvalidDataException>(() => new ParadoxDirectoryInventory().Read(directory));

            File.WriteAllBytes(Path.Combine(directory, "Test.PX"), []);
            File.WriteAllBytes(Path.Combine(directory, "Orphan.MB"), []);
            Assert.ThrowsExactly<InvalidDataException>(() => new ParadoxDirectoryInventory().Read(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string NewDirectory()
    {
        string path = Path.Combine(Path.GetTempPath(), $"ahwb-inventory-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    private static void WriteTable(string directory, string name, int recordCount)
    {
        byte[] header = new byte[2048];
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0), 2);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(2), 2048);
        header[5] = 1;
        BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(6), recordCount);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x0C), recordCount == 0 ? (ushort)0 : (ushort)1);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x0E), recordCount == 0 ? (ushort)0 : (ushort)1);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x21), 1);
        header[0x39] = 0x0B;
        header[0x78] = 0x7F; // A record type unsupported by the full record reader.
        header[0x79] = 2;
        Encoding.ASCII.GetBytes("Test").CopyTo(header, 0x84);
        Encoding.ASCII.GetBytes("Key").CopyTo(header, 0xD1);
        int length = recordCount == 0 ? 2048 : 3072;
        byte[] file = new byte[length];
        header.CopyTo(file, 0);
        File.WriteAllBytes(Path.Combine(directory, name), file);
    }
}
