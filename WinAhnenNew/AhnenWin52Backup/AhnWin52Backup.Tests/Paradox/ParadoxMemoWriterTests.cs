using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxMemoWriterTests
{
    [TestMethod]
    public void EncodeText_PacksSmallMemosIntoSuballocatedBlocks()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string memoPath = Path.Combine(directory, "AWD.MB");
        const uint encryption = 0x00005A93;
        try
        {
            WriteEmptyMemoFile(memoPath, encryption);
            ParadoxMemoWriter writer = new(memoPath, encryption);
            List<byte[]> references = new();
            List<string> values = Enumerable.Range(0, 200)
                .Select(static index => $"memo-value-{index:D3}")
                .ToList();

            foreach (string value in values)
            {
                references.Add(writer.EncodeText(value, 20, "Kommentar"));
            }

            writer.Save();

            Assert.AreEqual(5 * 4096, new FileInfo(memoPath).Length);
            byte[] memo = File.ReadAllBytes(memoPath);
            ParadoxBlockCipher.DecryptMemoBlock(memo, encryption);
            Assert.AreEqual(3, memo[4096]);
            Assert.AreEqual(3, memo[2 * 4096]);
            Assert.AreEqual(3, memo[3 * 4096]);
            Assert.AreEqual(3, memo[4 * 4096]);

            ParadoxMemoReader reader = new(memoPath, encryption);
            for (int index = 0; index < references.Count; index++)
            {
                byte[]? actual = reader.Read(references[index], graphic: false);
                Assert.IsNotNull(actual);
                Assert.AreEqual(values[index], Windows1252.Decode(actual));
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void EncodeText_UsesStandaloneBlocksForMemosLargerThanSuballocationLimit()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string memoPath = Path.Combine(directory, "AWD.MB");
        const uint encryption = 0x00005A93;
        try
        {
            WriteEmptyMemoFile(memoPath, encryption);
            string value = new('x', 2049);
            ParadoxMemoWriter writer = new(memoPath, encryption);
            byte[] reference = writer.EncodeText(value, 20, "Kommentar");
            writer.Save();

            byte[]? actual = new ParadoxMemoReader(memoPath, encryption).Read(reference, graphic: false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(value, Windows1252.Decode(actual));
            Assert.AreEqual(2 * 4096, new FileInfo(memoPath).Length);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void EncodeText_PreservesTabsAndRoundTripsEncryptedMultilineMemo()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string memoPath = Path.Combine(directory, "AWD.MB");
        const uint encryption = 0x00005A93;
        try
        {
            WriteEmptyMemoFile(memoPath, encryption);

            ParadoxMemoWriter writer = new(memoPath, encryption);
            byte[] reference = writer.EncodeText("first\tsecond\nthird", 10, "Kommentar");
            writer.Save();

            byte[]? memoBytes = new ParadoxMemoReader(memoPath, encryption).Read(reference, graphic: false);

            Assert.IsNotNull(memoBytes);
            Assert.AreEqual("first\tsecond\r\nthird", Windows1252.Decode(memoBytes));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static void WriteEmptyMemoFile(string memoPath, uint encryption)
    {
        byte[] initialMemoFile = new byte[4096];
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(1), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(3), 1);
        initialMemoFile[5] = 0x82;
        initialMemoFile[6] = 0x73;
        initialMemoFile[7] = 0x02;
        initialMemoFile[9] = 0x29;
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(0x0B), 0x1000);
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(0x0D), 0x1000);
        initialMemoFile[0x10] = 0x10;
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(0x11), 0x0040);
        BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(0x13), 0x0800);
        if (encryption != 0)
        {
            ParadoxBlockCipher.EncryptMemoBlock(initialMemoFile, encryption);
        }

        File.WriteAllBytes(memoPath, initialMemoFile);
    }
}
