using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class LoopAnalysisTests
{
    [TestMethod]
    public async Task AnalysisMarksBackwardBranchTargetAsLoop()
    {
        AnalysisSession session = CreateSession([0x75, 0xfe, 0xeb, 0xfc, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Loop));
        Assert.IsFalse(session.Items[0x1002].Flags.HasFlag(AnalysisFlags.Loop));
        Assert.IsTrue(session.Items[0x1000].CrossReferences.Count > 0);
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkForwardBranchAsLoop()
    {
        AnalysisSession session = CreateSession([0xeb, 0x01, 0x90, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1003].Flags.HasFlag(AnalysisFlags.Loop));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkFinallyHandlerBackwardBranchAsLoop()
    {
        AnalysisSession session = CreateSession([0x90, 0x90, 0xeb, 0xfc]);
        session.GetOrAddItem(0x1002).SetFlags(AnalysisFlags.FinallyHandler);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Loop));
    }

    private static AnalysisSession CreateSession(byte[] code)
    {
        byte[] image = new byte[0x20];
        code.CopyTo(image, 0);
        return new AnalysisSession(
            "loop.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
    }
}
