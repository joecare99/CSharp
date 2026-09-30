using IDR.Core.Services;
using IDR.Infrastructure.Pe;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class PeImageLoaderTests
{
    [TestMethod]
    public async Task LoadAsyncReadsRuntimePeImage()
    {
        string sourcePath = Environment.ProcessPath
            ?? throw new InvalidOperationException("The test process path is unavailable.");
        PeImageLoader loader = new();

        var image = await loader.LoadAsync(sourcePath, CancellationToken.None);

        Assert.AreEqual(sourcePath, image.SourcePath);
        Assert.IsTrue(image.Image.Length > 0);
        Assert.IsTrue(image.Sections.Count > 0);
        Assert.IsTrue(image.Sections.Any(section => section.ContainsCode));
        Assert.AreNotEqual(0u, image.EntryPointRva);
        Assert.IsTrue(image.Imports.Count > 0);
        Assert.IsTrue(image.Imports[0].Symbols.Count > 0);
    }

    [TestMethod]
    public async Task LoadAsyncRejectsInvalidImage()
    {
        string sourcePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bin");
        await File.WriteAllBytesAsync(sourcePath, [0, 1, 2, 3]);

        try
        {
            PeImageLoader loader = new();
            await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => loader.LoadAsync(sourcePath, CancellationToken.None));
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [TestMethod]
    public async Task LoadAsyncReadsImportsAndNamedAndForwardedExports()
    {
        string sourcePath = await WriteFixtureAsync(CreatePeFixture());

        try
        {
            PeImage image = await new PeImageLoader().LoadAsync(sourcePath, CancellationToken.None);

            Assert.AreEqual(0x400000UL, image.ImageBase);
            Assert.AreEqual(1, image.Imports.Count);
            Assert.AreEqual("KERNEL32.dll", image.Imports[0].Name);
            Assert.AreEqual(2, image.Imports[0].Symbols.Count);
            Assert.AreEqual("CreateFileW", image.Imports[0].Symbols[0].Name);
            Assert.AreEqual((ushort?)123, image.Imports[0].Symbols[1].Ordinal);
            Assert.AreEqual(2, image.Exports.Count);
            Assert.AreEqual("ExportedFunction", image.Exports[0].Name);
            Assert.AreEqual(1u, image.Exports[0].Ordinal);
            Assert.IsNull(image.Exports[0].Forwarder);
            Assert.AreEqual("OTHER.Function", image.Exports[1].Forwarder);
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [TestMethod]
    public async Task LoadAsyncRejectsImportDirectoryOutsideRawSections()
    {
        byte[] fixture = CreatePeFixture();
        BitConverter.GetBytes(0x5000u).CopyTo(fixture, 0x100);
        string sourcePath = await WriteFixtureAsync(fixture);

        try
        {
            await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => new PeImageLoader().LoadAsync(sourcePath, CancellationToken.None));
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    private static async Task<string> WriteFixtureAsync(byte[] data)
    {
        string sourcePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.exe");
        await File.WriteAllBytesAsync(sourcePath, data);
        return sourcePath;
    }

    private static byte[] CreatePeFixture()
    {
        byte[] data = new byte[0x600];
        data[0] = (byte)'M';
        data[1] = (byte)'Z';
        BitConverter.GetBytes(0x80).CopyTo(data, 0x3c);
        Encoding.ASCII.GetBytes("PE\0\0").CopyTo(data, 0x80);
        int coffOffset = 0x84;
        BitConverter.GetBytes((ushort)0x14c).CopyTo(data, coffOffset);
        BitConverter.GetBytes((ushort)1).CopyTo(data, coffOffset + 2);
        BitConverter.GetBytes((ushort)224).CopyTo(data, coffOffset + 16);
        int optionalOffset = coffOffset + 20;
        BitConverter.GetBytes((ushort)0x10b).CopyTo(data, optionalOffset);
        BitConverter.GetBytes(0x1000u).CopyTo(data, optionalOffset + 16);
        BitConverter.GetBytes(0x400000u).CopyTo(data, optionalOffset + 28);
        BitConverter.GetBytes(0x1000u).CopyTo(data, optionalOffset + 32);
        BitConverter.GetBytes(0x200u).CopyTo(data, optionalOffset + 36);
        BitConverter.GetBytes(0x2000u).CopyTo(data, optionalOffset + 56);
        BitConverter.GetBytes(0x200u).CopyTo(data, optionalOffset + 60);
        BitConverter.GetBytes(16u).CopyTo(data, optionalOffset + 92);
        int directoryOffset = optionalOffset + 96;
        BitConverter.GetBytes(0x1000u).CopyTo(data, directoryOffset);
        BitConverter.GetBytes(0x100u).CopyTo(data, directoryOffset + 4);
        BitConverter.GetBytes(0x1100u).CopyTo(data, directoryOffset + 8);
        BitConverter.GetBytes(40u).CopyTo(data, directoryOffset + 12);

        int sectionOffset = optionalOffset + 224;
        Encoding.ASCII.GetBytes(".rdata").CopyTo(data, sectionOffset);
        BitConverter.GetBytes(0x400u).CopyTo(data, sectionOffset + 8);
        BitConverter.GetBytes(0x1000u).CopyTo(data, sectionOffset + 12);
        BitConverter.GetBytes(0x400u).CopyTo(data, sectionOffset + 16);
        BitConverter.GetBytes(0x200u).CopyTo(data, sectionOffset + 20);
        BitConverter.GetBytes(0x40000040u).CopyTo(data, sectionOffset + 36);

        WriteUInt32(data, 0x200 + 12, 0x1040);
        WriteUInt32(data, 0x200 + 16, 1);
        WriteUInt32(data, 0x200 + 20, 2);
        WriteUInt32(data, 0x200 + 24, 1);
        WriteUInt32(data, 0x200 + 28, 0x1050);
        WriteUInt32(data, 0x200 + 32, 0x1058);
        WriteUInt32(data, 0x200 + 36, 0x105c);
        WriteAscii(data, 0x240, "fixture.dll");
        WriteUInt32(data, 0x250, 0x1200);
        WriteUInt32(data, 0x254, 0x1090);
        WriteUInt32(data, 0x258, 0x1060);
        BitConverter.GetBytes((ushort)0).CopyTo(data, 0x25c);
        WriteAscii(data, 0x260, "ExportedFunction");
        WriteAscii(data, 0x290, "OTHER.Function");

        WriteUInt32(data, 0x300, 0x1140);
        WriteUInt32(data, 0x30c, 0x1130);
        WriteUInt32(data, 0x310, 0x1160);
        WriteAscii(data, 0x330, "KERNEL32.dll");
        WriteUInt32(data, 0x340, 0x1170);
        WriteUInt32(data, 0x344, 0x8000007b);
        WriteUInt32(data, 0x348, 0);
        BitConverter.GetBytes((ushort)0).CopyTo(data, 0x370);
        WriteAscii(data, 0x372, "CreateFileW");
        return data;
    }

    private static void WriteUInt32(byte[] data, int offset, uint value) =>
        BitConverter.GetBytes(value).CopyTo(data, offset);

    private static void WriteAscii(byte[] data, int offset, string value) =>
        Encoding.ASCII.GetBytes(value).CopyTo(data, offset);
}
