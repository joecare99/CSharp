using System;
using System.Buffers.Binary;
using System.IO;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxMemoWriterTests
{
    [TestMethod]
    public void EncodeText_PreservesTabsAndRoundTripsEncryptedMultilineMemo()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        string memoPath = Path.Combine(directory, "AWD.MB");
        const uint encryption = 0x00005A93;
        try
        {
            byte[] initialMemoFile = new byte[4096];
            BinaryPrimitives.WriteUInt16LittleEndian(initialMemoFile.AsSpan(3), 1);
            ParadoxBlockCipher.EncryptMemoBlock(initialMemoFile, encryption);
            File.WriteAllBytes(memoPath, initialMemoFile);

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
}
