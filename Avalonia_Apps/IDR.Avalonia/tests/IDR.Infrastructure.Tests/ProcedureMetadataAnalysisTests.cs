using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class ProcedureMetadataAnalysisTests
{
    [TestMethod]
    public async Task AnalysisRecordsRetImmediateAndProcedureSizeAtStart()
    {
        AnalysisSession session = CreateSession([0x90, 0xc2, 0x08, 0x00]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem procedure = session.Items[0x1000];
        Assert.AreEqual(4U, procedure.ProcedureSizeBytes);
        Assert.AreEqual((ushort)8, procedure.ReturnStackBytes);
        Assert.IsTrue(session.Items[0x1001].Flags.HasFlag(AnalysisFlags.ProcedureEnd));
    }

    [TestMethod]
    public async Task AnalysisRecordsZeroStackCleanupForPlainRet()
    {
        AnalysisSession session = CreateSession([0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1U, session.Items[0x1000].ProcedureSizeBytes);
        Assert.AreEqual((ushort)0, session.Items[0x1000].ReturnStackBytes);
    }

    private static AnalysisSession CreateSession(byte[] code)
    {
        byte[] image = new byte[0x20];
        code.CopyTo(image, 0);
        return new AnalysisSession(
            "procedure.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
    }
}
