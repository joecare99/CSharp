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
    public async Task LoadAsyncReadsStringResources()
    {
        string sourcePath = await WriteFixtureAsync(CreatePeFixtureWithStringResource());

        try
        {
            PeImage image = await new PeImageLoader().LoadAsync(sourcePath, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { new PeResourceString(1, 0x0409, "Hello") },
                image.ResourceStrings.ToArray());
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [TestMethod]
    public async Task LoadAsyncRejectsTruncatedStringResourceDirectory()
    {
        byte[] image = CreatePeFixtureWithStringResource();
        BitConverter.GetBytes(15u).CopyTo(image, 0x10c);
        string sourcePath = await WriteFixtureAsync(image);

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

    [TestMethod]
    public async Task LoadAsyncReadsNamedDelphiFormResources()
    {
        string sourcePath = await WriteFixtureAsync(CreatePeFixtureWithFormResource());

        try
        {
            PeImage image = await new PeImageLoader().LoadAsync(sourcePath, CancellationToken.None);

            DelphiForm form = image.Forms.Single();
            Assert.AreEqual("Form1", form.ResourceName);
            Assert.AreEqual("TForm", form.Root.ClassName);
            Assert.AreEqual("Form1", form.Root.Name);
            Assert.AreEqual("Example", form.Root.Properties.Single(property =>
                property.Name == "Caption").Value);
            DelphiFormComponent button = form.Root.Children.Single();
            Assert.AreEqual("TButton", button.ClassName);
            Assert.AreEqual("Button1", button.Name);
            Assert.AreEqual("Run", button.Properties.Single(property =>
                property.Name == "Caption").Value);
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [TestMethod]
    public async Task LoadAsyncRejectsTruncatedDfmComponentData()
    {
        byte[] image = CreatePeFixtureWithFormResource();
        BitConverter.GetBytes(4u).CopyTo(image, 0x404);
        string sourcePath = await WriteFixtureAsync(image);

        try
        {
            PeImage loadedImage = await new PeImageLoader().LoadAsync(sourcePath, CancellationToken.None);

            Assert.AreEqual(0, loadedImage.Forms.Count);
            StringAssert.Contains(string.Join(Environment.NewLine, loadedImage.FormDiagnostics), "truncated");
        }
        finally
        {
            File.Delete(sourcePath);
        }
    }

    [TestMethod]
    public async Task LoadAsyncReadsConfiguredRealDelphiForms()
    {
        string? sourcePath = Environment.GetEnvironmentVariable("IDR_DFM_INTEGRATION_PE");
        if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
        {
            Assert.Inconclusive("Set IDR_DFM_INTEGRATION_PE to a Delphi PE containing DFM resources.");
        }

        PeImage image = await new PeImageLoader().LoadAsync(sourcePath!, CancellationToken.None);

        Assert.IsTrue(image.Forms.Count > 0, "The configured PE contains no supported TPF0 form resources.");
        Assert.IsTrue(image.Forms.Any(form => form.Root.Children.Count > 0));
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

    private static byte[] CreatePeFixtureWithStringResource()
    {
        byte[] data = CreatePeFixture();
        BitConverter.GetBytes(0x1180u).CopyTo(data, 0x108);
        BitConverter.GetBytes(0x80u).CopyTo(data, 0x10c);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x38e);
        WriteUInt32(data, 0x390, 6);
        WriteUInt32(data, 0x394, 0x80000020);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x3ae);
        WriteUInt32(data, 0x3b0, 1);
        WriteUInt32(data, 0x3b4, 0x80000040);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x3ce);
        WriteUInt32(data, 0x3d0, 0x0409);
        WriteUInt32(data, 0x3d4, 0x60);

        WriteUInt32(data, 0x3e0, 0x1200);
        WriteUInt32(data, 0x3e4, 42);
        BitConverter.GetBytes((ushort)5).CopyTo(data, 0x402);
        Encoding.Unicode.GetBytes("Hello").CopyTo(data, 0x404);
        return data;
    }

    private static byte[] CreatePeFixtureWithFormResource()
    {
        byte[] data = CreatePeFixture();
        byte[] formBytes = CreateBinaryFormFixture();
        BitConverter.GetBytes(0x1180u).CopyTo(data, 0x108);
        BitConverter.GetBytes(0xc0u).CopyTo(data, 0x10c);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x38e);
        WriteUInt32(data, 0x390, 10);
        WriteUInt32(data, 0x394, 0x80000020);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x3ac);
        WriteUInt32(data, 0x3b0, 0x80000060);
        WriteUInt32(data, 0x3b4, 0x80000040);

        BitConverter.GetBytes((ushort)1).CopyTo(data, 0x3ce);
        WriteUInt32(data, 0x3d0, 0);
        WriteUInt32(data, 0x3d4, 0x80);

        BitConverter.GetBytes((ushort)5).CopyTo(data, 0x3e0);
        Encoding.Unicode.GetBytes("Form1").CopyTo(data, 0x3e2);
        WriteUInt32(data, 0x400, 0x1240);
        WriteUInt32(data, 0x404, checked((uint)formBytes.Length));
        formBytes.CopyTo(data, 0x440);
        return data;
    }

    private static byte[] CreateBinaryFormFixture()
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);
        writer.Write(Encoding.ASCII.GetBytes("TPF0"));
        WriteShortString(writer, "TForm");
        WriteShortString(writer, "Form1");
        WriteShortString(writer, "Caption");
        writer.Write((byte)6);
        WriteShortString(writer, "Example");
        WriteShortString(writer, "Width");
        writer.Write((byte)3);
        writer.Write((short)320);
        WriteShortString(writer, "Height");
        writer.Write((byte)3);
        writer.Write((short)200);
        writer.Write((byte)0);

        WriteShortString(writer, "TButton");
        WriteShortString(writer, "Button1");
        WriteShortString(writer, "Caption");
        writer.Write((byte)6);
        WriteShortString(writer, "Run");
        WriteShortString(writer, "Left");
        writer.Write((byte)3);
        writer.Write((short)16);
        WriteShortString(writer, "Top");
        writer.Write((byte)3);
        writer.Write((short)24);
        WriteShortString(writer, "Width");
        writer.Write((byte)3);
        writer.Write((short)80);
        WriteShortString(writer, "Height");
        writer.Write((byte)3);
        writer.Write((short)25);
        writer.Write((byte)0);
        writer.Write((byte)0);
        writer.Write((byte)0);
        return stream.ToArray();
    }

    private static void WriteShortString(BinaryWriter writer, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length >= byte.MaxValue)
        {
            throw new InvalidOperationException("The test short string is too long.");
        }

        writer.Write((byte)bytes.Length);
        writer.Write(bytes);
    }

    private static void WriteUInt32(byte[] data, int offset, uint value) =>
        BitConverter.GetBytes(value).CopyTo(data, offset);

    private static void WriteAscii(byte[] data, int offset, string value) =>
        Encoding.ASCII.GetBytes(value).CopyTo(data, offset);
}
