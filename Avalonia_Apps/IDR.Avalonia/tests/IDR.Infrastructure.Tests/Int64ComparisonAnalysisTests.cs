using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class Int64ComparisonAnalysisTests
{
    [TestMethod]
    public async Task AnalysisMarksDelphiSplitInt64ComparisonPattern()
    {
        byte[] code =
        [
            0x39, 0xd8,
            0x72, 0x06,
            0x39, 0xc8,
            0x77, 0x06,
            0xeb, 0x06,
            0x72, 0x04,
            0x90, 0x90, 0x90, 0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsTrue(session.Items[0x1002].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsFalse(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsTrue(session.Items[0x1006].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.AreEqual(0x1010U, session.Items[0x1000].Int64ComparisonEndAddress);
    }

    [TestMethod]
    public async Task AnalysisRejectsSplitInt64ComparisonWithoutUnconditionalJump()
    {
        byte[] code =
        [
            0x39, 0xd8,
            0x72, 0x06,
            0x39, 0xc8,
            0x77, 0x06,
            0x75, 0x06,
            0x72, 0x04,
            0x90, 0x90, 0x90, 0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsNull(session.Items[0x1000].Int64ComparisonEndAddress);
    }

    [TestMethod]
    public async Task AnalysisMarksDelphiStackBasedInt64ComparisonPattern()
    {
        byte[] code =
        [
            0x50,
            0x53,
            0x89, 0xd1,
            0x3b, 0x44, 0x24, 0x04,
            0x72, 0x03,
            0x3b, 0x04, 0x24,
            0x5f,
            0x5e,
            0x72, 0x02,
            0x90, 0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.AreEqual(0x100dU, session.Items[0x1000].Int64ComparisonEndAddress);
    }

    [TestMethod]
    public async Task AnalysisRejectsStackBasedInt64ComparisonWithWrongStackOffset()
    {
        byte[] code =
        [
            0x50,
            0x53,
            0x89, 0xd1,
            0x3b, 0x44, 0x24, 0x08,
            0x72, 0x03,
            0x3b, 0x04, 0x24,
            0x5f,
            0x5e,
            0x72, 0x02,
            0x90, 0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsNull(session.Items[0x1000].Int64ComparisonEndAddress);
    }

    [TestMethod]
    public async Task AnalysisMarksDelphiStackBasedInt64ComparisonWithSplitCleanup()
    {
        byte[] code =
        [
            0x50,
            0x53,
            0x89, 0xd1,
            0x3b, 0x44, 0x24, 0x04,
            0x72, 0x09,
            0x3b, 0x04, 0x24,
            0x5f,
            0x5e,
            0x72, 0x07,
            0xeb, 0x05,
            0x5f,
            0x5e,
            0x72, 0x01,
            0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        foreach (uint address in new uint[] { 0x1004, 0x1008, 0x100d, 0x100e, 0x100f, 0x1011 })
        {
            Assert.IsTrue(
                session.Items[address].Flags.HasFlag(AnalysisFlags.Int64Comparison),
                $"Expected Int64 comparison marker at RVA 0x{address:X8}.");
        }

        Assert.AreEqual(0x1018U, session.Items[0x1000].Int64ComparisonEndAddress);
    }

    [TestMethod]
    public async Task AnalysisRejectsStackBasedSplitCleanupWhenBranchSkipsPopLabel()
    {
        byte[] code =
        [
            0x50,
            0x53,
            0x89, 0xd1,
            0x3b, 0x44, 0x24, 0x04,
            0x72, 0x08,
            0x3b, 0x04, 0x24,
            0x5f,
            0x5e,
            0x72, 0x07,
            0xeb, 0x05,
            0x5f,
            0x5e,
            0x72, 0x01,
            0x90,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.Int64Comparison));
        Assert.IsNull(session.Items[0x1000].Int64ComparisonEndAddress);
    }

    private static AnalysisSession CreateSession(byte[] code)
    {
        byte[] image = new byte[0x40];
        code.CopyTo(image, 0);
        return new AnalysisSession(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
    }
}
