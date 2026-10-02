using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class FloatingPointMemoryAnalysisTests
{
    [TestMethod]
    public async Task AnalysisInfersGlobalTypesFromX87MemoryPrecision()
    {
        (byte[] Instruction, string ExpectedType)[] cases =
        [
            ([0xd9, 0x05, 0x20, 0x20, 0x40, 0x00], "Single"),
            ([0xdd, 0x05, 0x20, 0x20, 0x40, 0x00], "Double"),
            ([0xdb, 0x2d, 0x20, 0x20, 0x40, 0x00], "Extended")
        ];

        foreach ((byte[] instruction, string expectedType) in cases)
        {
            AnalysisSession session = CreateSession([.. instruction, 0xc3]);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            AnalysisItem data = session.Items[0x2020];
            Assert.AreEqual(expectedType, data.DataTypeCandidate);
            Assert.IsTrue(data.Flags.HasFlag(AnalysisFlags.Data));
        }
    }

    [TestMethod]
    public async Task AnalysisInfersX87PrecisionForFrameBasedStackLocals()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0xd9, 0x5d, 0xfc,
            0xdd, 0x5d, 0xf8,
            0xdb, 0x7d, 0xf6,
            0xc9,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[]
            {
                new StackLocalVariable(-10, 10, "Extended"),
                new StackLocalVariable(-8, 8, "Double"),
                new StackLocalVariable(-4, 4, "Single")
            },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersX87PrecisionThroughDirectlyTrackedRegisterAddress()
    {
        AnalysisSession session = CreateSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0xd9, 0x00,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Single", session.Items[0x2020].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersX87PrecisionThroughGlobalAddressHeldInEbx()
    {
        AnalysisSession session = CreateSession(
        [
            0xbb, 0x20, 0x20, 0x40, 0x00,
            0xd9, 0x03,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Single", session.Items[0x2020].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferX87PrecisionForOffsetOrUnknownRegisterAddress()
    {
        AnalysisSession offsetSession = CreateSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0xd9, 0x40, 0x04,
            0xc3
        ]);
        AnalysisSession unknownSession = CreateSession([0xd9, 0x00, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(offsetSession, null, CancellationToken.None);
        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(unknownSession, null, CancellationToken.None);

        Assert.IsFalse(offsetSession.Items.ContainsKey(0x2020));
        Assert.IsFalse(unknownSession.Items.ContainsKey(0x2020));
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferFloatingPointTypesFromIntegerMemoryAccesses()
    {
        AnalysisSession session = CreateSession([0xa1, 0x20, 0x20, 0x40, 0x00, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x2020));
    }

    private static AnalysisSession CreateSession(byte[] code)
    {
        byte[] image = new byte[0x80];
        code.CopyTo(image, 0);
        return new AnalysisSession(
            "floating-point.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
    }
}
