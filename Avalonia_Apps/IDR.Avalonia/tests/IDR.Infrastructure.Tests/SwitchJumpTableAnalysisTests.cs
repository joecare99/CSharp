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
    public async Task AnalysisTransfersClassRegisterContextToSwitchCases()
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb9;
        WriteUInt32(image, 1, 0x402030);
        image[5] = 0x83;
        image[6] = 0xf8;
        image[7] = 0x01;
        image[8] = 0xff;
        image[9] = 0x24;
        image[10] = 0x85;
        WriteUInt32(image, 11, 0x402020);
        image[15] = 0xc3;
        image[0x20] = 0x8b;
        image[0x21] = 0x41;
        image[0x22] = 0x08;
        image[0x23] = 0xc3;
        WriteUInt32(image, 0x60, 0x401020);

        AnalysisSession session = new(
            "switch-class-context.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem classVmt = session.GetOrAddItem(0x2030);
        classVmt.Name = "TChild";
        classVmt.SetFlags(AnalysisFlags.Vmt);
        classVmt.ClassInstanceSizeBytes = 20;
        classVmt.Fields = [new DelphiVmtField("FCount", 8, 0x2038)];
        session.GetOrAddItem(0x2038).Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(
            "FCount",
            session.Items[0x1020].MemberAccessCandidates.Single().FieldName);
    }

    [TestMethod]
    public async Task AnalysisTransfersTrackedGlobalAddressContextToSwitchCases()
    {
        byte[] image = new byte[0x80];
        image[0] = 0xbb;
        WriteUInt32(image, 1, 0x402020);
        image[5] = 0x83;
        image[6] = 0xf8;
        image[7] = 0x01;
        image[8] = 0xff;
        image[9] = 0x24;
        image[10] = 0x85;
        WriteUInt32(image, 11, 0x402024);
        image[15] = 0xc3;
        image[0x20] = 0x8b;
        image[0x21] = 0x13;
        image[0x22] = 0x8b;
        image[0x23] = 0x4a;
        image[0x24] = 0x08;
        image[0x25] = 0xc3;
        WriteUInt32(image, 0x60, 0x402030);
        WriteUInt32(image, 0x64, 0x401020);

        AnalysisSession session = new(
            "switch-global-address-context.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem classVmt = session.GetOrAddItem(0x2030);
        classVmt.Name = "TChild";
        classVmt.SetFlags(AnalysisFlags.Vmt);
        classVmt.ClassInstanceSizeBytes = 20;
        classVmt.Fields = [new DelphiVmtField("FCount", 8, 0x2038)];
        session.GetOrAddItem(0x2038).Name = "Integer";
        session.GetOrAddItem(0x2020).DataTypeCandidate = "TChild";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1022].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
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
