using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Binary;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class SwitchJumpTableAnalysisTests
{
    [TestMethod]
    public async Task AnalysisRecognizesIndexedJumpTableAndFollowsCases()
    {
        byte[] image = CreateSwitchImage(0x401020);
        WriteUInt32(image, 0x20, 0x401030);
        WriteUInt32(image, 0x24, 0x401032);
        image[0x30] = 0xc3;
        image[0x32] = 0xc3;
        AnalysisSession session = CreateSession(image);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Switch));
        Assert.IsTrue(session.Items[0x1003].Flags.HasFlag(AnalysisFlags.Switch));
        Assert.IsTrue(session.Items[0x1020].Flags.HasFlag(AnalysisFlags.Data | AnalysisFlags.SwitchTable));
        Assert.IsTrue(session.Items[0x1024].Flags.HasFlag(AnalysisFlags.Data | AnalysisFlags.SwitchTable));
        Assert.IsTrue(session.Items[0x1030].Flags.HasFlag(AnalysisFlags.Instruction));
        Assert.IsTrue(session.Items[0x1032].Flags.HasFlag(AnalysisFlags.Instruction));
        CollectionAssert.AreEquivalent(
            new uint[] { 0x1030, 0x1032 },
            session.Items[0x1003].CrossReferences.Select(reference => reference.TargetAddress).ToArray());
    }

    [TestMethod]
    public async Task AnalysisDoesNotClassifyIndexedJumpWithUnmappedTable()
    {
        byte[] image = CreateSwitchImage(0x402000);
        AnalysisSession session = CreateSession(image);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Switch));
        Assert.IsFalse(session.Items[0x1003].Flags.HasFlag(AnalysisFlags.Switch));
        Assert.AreEqual(0, session.Items[0x1003].CrossReferences.Count);
    }

    private static byte[] CreateSwitchImage(uint tableAddress)
    {
        byte[] image = new byte[0x40];
        image[0] = 0x83;
        image[1] = 0xf8;
        image[2] = 0x01;
        image[3] = 0xff;
        image[4] = 0x24;
        image[5] = 0x8d;
        WriteUInt32(image, 6, tableAddress);
        return image;
    }

    private static AnalysisSession CreateSession(byte[] image) => new(
        "switch.exe",
        image,
        [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
        0x400000)
    {
        EntryPointRva = 0x1000
    };

    private static void WriteUInt32(byte[] image, int offset, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(offset, sizeof(uint)), value);
    }
}
