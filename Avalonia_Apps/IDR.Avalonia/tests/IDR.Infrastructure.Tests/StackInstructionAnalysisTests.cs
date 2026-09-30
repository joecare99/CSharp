using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class StackInstructionAnalysisTests
{
    [TestMethod]
    public async Task AnalysisMarksFrameAndStackOperationsWithTheirEspDeltas()
    {
        byte[] code =
        [
            0x55,
            0x8b, 0xec,
            0x53,
            0x5b,
            0x83, 0xec, 0x10,
            0x83, 0xc4, 0x10,
            0x8b, 0xe5,
            0x5d,
            0xc3
        ];
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.FrameInstruction));
        Assert.IsTrue(session.Items[0x1001].Flags.HasFlag(AnalysisFlags.FrameInstruction));
        Assert.IsTrue(session.Items[0x100b].Flags.HasFlag(AnalysisFlags.FrameInstruction));
        Assert.IsTrue(session.Items[0x1003].Flags.HasFlag(AnalysisFlags.StackPush));
        Assert.IsTrue(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.StackPop));
        Assert.IsTrue(session.Items[0x1005].Flags.HasFlag(AnalysisFlags.StackAdjustment));
        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.StackAdjustment));
        Assert.AreEqual(-4L, session.Items[0x1003].StackPointerDeltaBytes);
        Assert.AreEqual(4L, session.Items[0x1004].StackPointerDeltaBytes);
        Assert.AreEqual(-16L, session.Items[0x1005].StackPointerDeltaBytes);
        Assert.AreEqual(16L, session.Items[0x1008].StackPointerDeltaBytes);
        Assert.IsTrue(session.Items[0x1000].UsesFramePointer);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatNonEspArithmeticAsStackAdjustment()
    {
        AnalysisSession session = CreateSession([0x83, 0xe8, 0x10, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.StackAdjustment));
        Assert.IsNull(session.Items[0x1000].StackPointerDeltaBytes);
        Assert.IsFalse(session.Items[0x1000].UsesFramePointer);
    }

    [TestMethod]
    public async Task AnalysisRecognizesEnterFramePrologueAndLocalAllocation()
    {
        AnalysisSession session = CreateSession(
        [
            0xc8, 0x10, 0x00, 0x00,
            0x8b, 0x45, 0x08,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem procedure = session.Items[0x1000];
        Assert.IsTrue(procedure.UsesFramePointer);
        Assert.IsTrue(procedure.Flags.HasFlag(AnalysisFlags.FrameInstruction));
        Assert.IsTrue(procedure.Flags.HasFlag(AnalysisFlags.StackAdjustment));
        Assert.AreEqual(-20L, procedure.StackPointerDeltaBytes);
        Assert.AreEqual(8u, procedure.StackArguments.Single().Offset);
    }

    [TestMethod]
    public async Task AnalysisRecognizesNestedEnterAndLeaveFrameInstructions()
    {
        AnalysisSession session = CreateSession(
        [
            0xc8, 0x00, 0x00, 0x01,
            0xc9,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].UsesFramePointer);
        Assert.AreEqual(-8L, session.Items[0x1000].StackPointerDeltaBytes);
        Assert.IsTrue(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.FrameInstruction));
        Assert.IsNull(session.Items[0x1004].StackPointerDeltaBytes);
    }

    [TestMethod]
    public async Task AnalysisKeepsCallPopPcIdiomsInlineAndPreservesOtherRegisterArguments()
    {
        AnalysisSession session = CreateSession(
        [
            0xe8, 0x00, 0x00, 0x00, 0x00,
            0x5b,
            0x83, 0xf8, 0x00,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1005].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatCallToNextInstructionAsInlineWithoutPop()
    {
        AnalysisSession session = CreateSession(
        [
            0xe8, 0x00, 0x00, 0x00, 0x00,
            0x90,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1005].Flags.HasFlag(AnalysisFlags.ProcedureStart));
    }

    [TestMethod]
    public async Task AnalysisPreservesSignExtendedStackAdjustmentSemantics()
    {
        AnalysisSession session = CreateSession(
        [
            0x83, 0xec, 0xf0,
            0x83, 0xc4, 0xf0,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(16L, session.Items[0x1000].StackPointerDeltaBytes);
        Assert.AreEqual(-16L, session.Items[0x1003].StackPointerDeltaBytes);
    }

    [TestMethod]
    public async Task AnalysisInfersPositiveEbpStackArgumentOffsets()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0x8b, 0x4d, 0x0c,
            0x8b, 0x55, 0xfc,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new uint[] { 8, 12 },
            session.Items[0x1000].StackArguments.Select(argument => argument.Offset).ToArray());
        CollectionAssert.AreEqual(
            new[] { 4, 4 },
            session.Items[0x1000].StackArguments.Select(argument => argument.SizeBytes).ToArray());
    }

    [TestMethod]
    public async Task AnalysisFlagsStackArgumentsLargerThanRetCleanup()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0x8b, 0x4d, 0x0c,
            0xc2, 0x04, 0x00
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.StackArgumentSizeMismatch));
    }

    [TestMethod]
    public async Task AnalysisDoesNotFlagStackArgumentsMatchingRetCleanup()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0x8b, 0x4d, 0x0c,
            0xc2, 0x08, 0x00
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.StackArgumentSizeMismatch));
    }

    [TestMethod]
    public async Task AnalysisRoundsSmallStackArgumentAccessesToFourByteSlots()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x0f, 0xb6, 0x45, 0x08,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].StackArguments.Count);
        Assert.AreEqual(4, session.Items[0x1000].StackArguments[0].SizeBytes);
    }

    [TestMethod]
    public async Task AnalysisInfersPointerSizedArgumentFromEbpRelativeLea()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x8d, 0x45, 0x08,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].StackArguments.Count);
        Assert.AreEqual(8u, session.Items[0x1000].StackArguments[0].Offset);
        Assert.AreEqual(4, session.Items[0x1000].StackArguments[0].SizeBytes);
    }

    [TestMethod]
    public async Task AnalysisLabelsTenByteStackArgumentAsExtended()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0xdb, 0x6d, 0x08,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].StackArguments.Count);
        Assert.AreEqual(12, session.Items[0x1000].StackArguments[0].SizeBytes);
        Assert.AreEqual("Extended", session.Items[0x1000].StackArguments[0].TypeName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferEbpArgumentsWithoutAFramePrologue()
    {
        AnalysisSession session = CreateSession(
        [
            0x8b, 0x45, 0x08,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].StackArguments.Count);
    }

    [TestMethod]
    public async Task AnalysisFollowsUnclassifiedAbsoluteCodePointersInImmediateOperands()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x3d;
        code[1] = 0x0a;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xc3;
        code[0x0a] = 0x55;
        code[0x0b] = 0x8b;
        code[0x0c] = 0xec;
        code[0x0d] = 0x5d;
        code[0x0e] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x100a].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.Items[0x100e].Flags.HasFlag(AnalysisFlags.ProcedureEnd));
        CrossReference reference = session.GetIncomingCrossReferences(0x100a).Single();
        Assert.AreEqual(0x1000u, reference.SourceAddress);
        Assert.AreEqual(CrossReferenceKind.Constant, reference.Kind);
    }

    [TestMethod]
    public async Task AnalysisRejectsBareReturnAsAnUnclassifiedCodePointer()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x3d;
        code[1] = 0x0a;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xc3;
        code[0x0a] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x100a, out AnalysisItem? target)
            && target.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x100a).Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatPreviouslyClassifiedDataAsCodePointer()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x3d;
        code[1] = 0x0a;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xc3;
        code[0x0a] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x100a).SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x100a].Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsFalse(session.Items[0x100a].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x100a).Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatImmediateTargetsInsideCurrentProcedureAsCodePointers()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x3d;
        code[1] = 0x04;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x1004, out AnalysisItem? target)
            && target.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x1004).Count);
    }

    [TestMethod]
    public async Task AnalysisIgnoresImmediateOnlyFirstOperandsAsCodePointers()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x68;
        code[1] = 0x0a;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xc3;
        code[0x0a] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x100a, out AnalysisItem? target)
            && target.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x100a).Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotFollowImmediateTargetsInNonExecutableSections()
    {
        byte[] image = new byte[0x20];
        image[0] = 0x3d;
        image[1] = 0x06;
        image[2] = 0x10;
        image[3] = 0x40;
        image[4] = 0x00;
        image[5] = 0xc3;
        image[6] = 0xc3;
        AnalysisSession session = new(
            "pointer.exe",
            image,
            [
                new PeSection(".text", 0x1000, 6, 0, 6, true),
                new PeSection(".data", 0x1006, 0x1a, 6, 0x1a, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x1006, out AnalysisItem? target)
            && target.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x1006).Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatRvaImmediateAsAnAbsoluteVaCodePointer()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x3d;
        code[1] = 0x0a;
        code[2] = 0x10;
        code[3] = 0x00;
        code[4] = 0x00;
        code[5] = 0xc3;
        code[0x0a] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x100a, out AnalysisItem? target)
            && target.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x100a).Count);
    }

    [TestMethod]
    public async Task AnalysisInfersDelphiRegisterArgumentCandidatesInRegisterOrder()
    {
        AnalysisSession session = CreateSession(
        [
            0x8b, 0xd8,
            0x83, 0xfa, 0x00,
            0x8b, 0x01,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersEaxArgumentReadImplicitlyBySahf()
    {
        AnalysisSession session = CreateSession([0x9e, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitCwdRegisterReadsAndWrites()
    {
        AnalysisSession session = CreateSession([0x66, 0x99, 0x52, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisInfersEaxArgumentReadImplicitlyByCwde()
    {
        AnalysisSession session = CreateSession([0x98, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisInfersEaxArgumentReadImplicitlyByCbw()
    {
        AnalysisSession session = CreateSession([0x66, 0x98, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisTracksCpuidImplicitRegisterReadsAndWrites()
    {
        AnalysisSession session = CreateSession([0x0f, 0xa2, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksRdpmcImplicitRegisterReadsAndWrites()
    {
        AnalysisSession session = CreateSession([0x0f, 0x33, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksRdtscpAsImplicitRegisterWritesOnly()
    {
        AnalysisSession session = CreateSession([0x0f, 0x01, 0xf9, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Rdtscp", session.Items[0x1000].Name);
        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitMsrAndExtendedControlRegisterEffects()
    {
        byte[][] codeSamples =
        [
            [0x0f, 0x32, 0xc3],
            [0x0f, 0x30, 0xc3],
            [0x0f, 0x01, 0xd0, 0xc3],
            [0x0f, 0x01, 0xd1, 0xc3]
        ];
        string[] expectedMnemonics = ["Rdmsr", "Wrmsr", "Xgetbv", "Xsetbv"];

        foreach ((byte[] code, int index) in codeSamples.Select((code, index) => (code, index)))
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(expectedMnemonics[index], session.Items[0x1000].Name);
            CollectionAssert.AreEqual(
                new[] { "EAX", "EDX", "ECX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitCmpxchgAccumulatorReadAndWrite()
    {
        AnalysisSession session = CreateSession([0x0f, 0xb1, 0xd1, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitCmpxchg8bRegisterEffects()
    {
        AnalysisSession session = CreateSession([0x0f, 0xc7, 0x08, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksBothXaddRegisterInputs()
    {
        AnalysisSession session = CreateSession([0x0f, 0xc1, 0xd0, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksXchgRegisterInputsAndOverwrites()
    {
        AnalysisSession session = CreateSession([0x92, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Xchg", session.Items[0x1000].Name);
        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersRegisterArgumentsReadByOutInstruction()
    {
        AnalysisSession session = CreateSession([0xef, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsDxAsInputAndEaxAsOutputForInInstruction()
    {
        AnalysisSession session = CreateSession([0xed, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferRegisterArgumentsFromImmediatePortIn()
    {
        AnalysisSession session = CreateSession([0xe5, 0x80, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossInInstruction()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xed;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferEaxArgumentAfterRdtscClobbersIt()
    {
        AnalysisSession session = CreateSession([0x0f, 0x31, 0x50, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisInfersRegisterArgumentsReadByPushad()
    {
        AnalysisSession session = CreateSession([0x60, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersRegisterArgumentsReadByPusha()
    {
        AnalysisSession session = CreateSession([0x66, 0x60, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Pusha", session.Items[0x1000].Name);
        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersEaxArgumentReadAndWrittenByXlat()
    {
        AnalysisSession session = CreateSession([0xd7, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferRegisterArgumentWhenItIsOverwrittenBeforeUse()
    {
        AnalysisSession session = CreateSession(
        [
            0xb8, 0x01, 0x00, 0x00, 0x00,
            0x31, 0xd2,
            0x31, 0xc9,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisCountsRegisterUsedByPushAsAnArgumentCandidate()
    {
        AnalysisSession session = CreateSession([0x50, 0x5b, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsImplicitEaxInputToCdqAsAnArgumentCandidate()
    {
        AnalysisSession session = CreateSession([0x99, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitAccumulatorEffectsOfBcdInstructions()
    {
        byte[][] codeSamples =
        [
            [0x37, 0xc3],
            [0x3f, 0xc3],
            [0xd4, 0x0a, 0xc3],
            [0xd5, 0x0a, 0xc3],
            [0x27, 0xc3],
            [0x2f, 0xc3]
        ];

        foreach (byte[] code in codeSamples)
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { "EAX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferEaxArgumentAfterFstswWritesAx()
    {
        AnalysisSession session = CreateSession([0xdf, 0xe0, 0x50, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Fnstsw", session.Items[0x1000].Name);
        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateEaxReturnAcrossFstswToAx()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xdf;
        code[6] = 0xe0;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPreservesEaxReturnAcrossFstswToMemory()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xdd;
        code[6] = 0x38;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Fnstsw", session.Items[0x1005].Name);
        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTreatsImplicitMultiplyOutputsAsOverwritingEaxAndEdx()
    {
        AnalysisSession session = CreateSession(
        [
            0xf7, 0xe3,
            0x8b, 0xca,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsImplicitEdxDividendInputToDivAsAnArgumentCandidate()
    {
        AnalysisSession session = CreateSession([0xf7, 0xf3, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsReadModifyWriteAndClRegisterOperandsAsArguments()
    {
        AnalysisSession session = CreateSession([0xd3, 0xe0, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsBitTestAndSetReadModifyWriteOperandsAsArguments()
    {
        AnalysisSession session = CreateSession([0x0f, 0xab, 0xd0, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsReadOnlyRegisterOperandsOfBoundAndSelectorChecks()
    {
        byte[][] codeSamples =
        [
            [0x62, 0x08, 0xc3],
            [0x0f, 0x00, 0xe1, 0xc3],
            [0x0f, 0x00, 0xe9, 0xc3]
        ];

        string[] expectedMnemonics = ["Bound", "Verr", "Verw"];
        foreach ((byte[] code, int index) in codeSamples.Select((code, index) => (code, index)))
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(expectedMnemonics[index], session.Items[0x1000].Name);
            CollectionAssert.AreEqual(
                new[] { "EAX", "EDX", "ECX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotCountOverwrittenSelectorCheckOperandAsAnInput()
    {
        AnalysisSession session = CreateSession(
        [
            0xb9, 0x00, 0x00, 0x00, 0x00,
            0x0f, 0x00, 0xe1,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisCountsBswapInputAsAnEaxArgument()
    {
        AnalysisSession session = CreateSession([0x0f, 0xc8, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsAdcxAndAdoxReadModifyWriteOperandsAsArguments()
    {
        byte[][] codeSamples =
        [
            [0x66, 0x0f, 0x38, 0xf6, 0xc2, 0xc3],
            [0xf3, 0x0f, 0x38, 0xf6, 0xc2, 0xc3]
        ];

        foreach (byte[] code in codeSamples)
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { "EAX", "EDX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisCountsOldDestinationReadByConditionalMove()
    {
        AnalysisSession session = CreateSession([0x0f, 0x44, 0xc3, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Cmove", session.Items[0x1000].Name);
        CollectionAssert.AreEqual(
            new[] { "EAX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsOldDestinationReadByArpl()
    {
        AnalysisSession session = CreateSession([0x63, 0xc1, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Arpl", session.Items[0x1000].Name);
        CollectionAssert.AreEqual(
            new[] { "EAX", "EDX", "ECX" },
            session.Items[0x1000].RegisterArgumentCandidates.ToArray());
    }

    [TestMethod]
    public async Task AnalysisCountsRegistersUsedByIndirectCallTargets()
    {
        byte[][] codeSamples =
        [
            [0xff, 0xd0, 0xc3],
            [0xff, 0x14, 0x11, 0xc3]
        ];

        string[][] expectedArguments =
        [
            ["EAX"],
            ["EAX", "EDX", "ECX"]
        ];

        foreach ((byte[] code, int index) in codeSamples.Select((code, index) => (code, index)))
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                expectedArguments[index],
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotCountArplDestinationOverwrittenBeforeRead()
    {
        AnalysisSession session = CreateSession(
        [
            0xb9, 0x00, 0x00, 0x00, 0x00,
            0x63, 0xd9,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisMarksCalleeWhenItsReturnIsUsedAsConditionalMoveDestination()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x44;
        code[7] = 0xc3;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitAccumulatorEffectsOfStringInstructions()
    {
        byte[][] accumulatorReadSamples =
        [
            [0xab, 0xc3],
            [0xaf, 0xc3]
        ];

        foreach (byte[] code in accumulatorReadSamples)
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { "EAX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }

        AnalysisSession accumulatorWriteSession = CreateSession([0xad, 0x50, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(accumulatorWriteSession, null, CancellationToken.None);

        Assert.AreEqual(0, accumulatorWriteSession.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitPortRegisterEffectsOfStringInstructions()
    {
        byte[][] codeSamples =
        [
            [0x6d, 0xc3],
            [0x6f, 0xc3]
        ];

        foreach (byte[] code in codeSamples)
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { "EAX", "EDX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisTracksImplicitEcxEffectsOfRepeatAndLoopInstructions()
    {
        byte[][] codeSamples =
        [
            [0xf3, 0xa4, 0xc3],
            [0xe2, 0x00, 0xc3],
            [0xe3, 0x00, 0xc3]
        ];

        foreach (byte[] code in codeSamples)
        {
            AnalysisSession session = CreateSession(code);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { "EAX", "EDX", "ECX" },
                session.Items[0x1000].RegisterArgumentCandidates.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisMarksProcedureCallingClassCreateAsConstructorCandidate()
    {
        AnalysisSession session = CreateSession(
        [
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ]);
        AnalysisItem classCreate = session.GetOrAddItem(0x1010);
        classCreate.Name = "@ClassCreate";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ConstructorCandidate));
        Assert.IsFalse(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.DestructorCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksProcedureCallingClassDestroyAsDestructorCandidate()
    {
        AnalysisSession session = CreateSession(
        [
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ]);
        AnalysisItem classDestroy = session.GetOrAddItem(0x1010);
        classDestroy.Name = "@ClassDestroy";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.DestructorCandidate));
        Assert.IsFalse(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ConstructorCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotClassifyRegularCallsAsConstructorsOrDestructors()
    {
        AnalysisSession session = CreateSession(
        [
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ]);
        AnalysisItem helper = session.GetOrAddItem(0x1010);
        helper.Name = "Helper";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ConstructorCandidate));
        Assert.IsFalse(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.DestructorCandidate));
    }

    [TestMethod]
    public async Task AnalysisInfersBooleanReturnCandidateFromSetccWritingAl()
    {
        AnalysisSession session = CreateSession([0x0f, 0x95, 0xc0, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisClearsBooleanReturnCandidateWhenEaxIsOverwrittenBeforeReturn()
    {
        AnalysisSession session = CreateSession(
        [
            0x0f, 0x95, 0xc0,
            0xb0, 0x00,
            0xc3
        ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesBooleanReturnCandidateFromImmediatelyReturnedCall()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisStopsEntryPointAtHalt0Call()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x95;
        code[7] = 0xc0;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@Halt0";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x1005));
        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisContinuesAfterOrdinaryDirectCall()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x95;
        code[7] = 0xc0;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "Helper";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.IsTrue(session.Items.ContainsKey(0x1005));
    }

    [TestMethod]
    public async Task AnalysisStopsProcedureAtHalt0Call()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x03;
        code[5] = 0xc3;
        code[8] = 0xe8;
        code[9] = 0x0b;
        code[0x0d] = 0x0f;
        code[0x0e] = 0x95;
        code[0x0f] = 0xc0;
        code[0x10] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1018).Name = "@Halt0";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x100d));
        Assert.IsNull(session.Items[0x1008].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesBooleanReturnCandidateAcrossStackCleanup()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x83;
        code[6] = 0xc4;
        code[7] = 0x04;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesReturnCandidateAcrossBitTest()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xa3;
        code[7] = 0xd0;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossBswap()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xc8;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossLodsd()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xad;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossAaa()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x37;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossLahf()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x9f;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossCwde()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x98;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossCmpxchg()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xb1;
        code[7] = 0xd1;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossCmpxchg8b()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xc7;
        code[7] = 0x08;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateWhenXaddWritesEaxAsSecondOperand()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xc1;
        code[7] = 0xc2;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossRdtsc()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x31;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossRdtscp()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x01;
        code[7] = 0xf9;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Rdtscp", session.Items[0x1005].Name);
        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersBooleanReturnWhenReturningIsClassCallResult()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@IsClass";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferBooleanReturnFromUnidentifiedCallResult()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersTlsReturnMarkerFromGetTlsCall()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@GetTls";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("#TLS", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersAsClassReturnTypeFromKnownVmtRegister()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xba;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersClassCreateReturnTypeFromKnownEaxVmt()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@ClassCreate";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferClassCreateTypeFromUnknownEaxValue()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@ClassCreate";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPreservesKnownClassTypeAcrossBeforeDestruction()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@BeforeDestruction";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPreserveUnknownClassTypeAcrossBeforeDestruction()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@BeforeDestruction";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPreservesClassTypeAcrossRegisterPreservingRuntimeCall()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x10;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x0e;
        code[10] = 0xe8;
        code[11] = 0x0d;
        code[15] = 0xc3;
        code[0x18] = 0xc3;
        code[0x1c] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1018).Name = "@IntOver";
        session.GetOrAddItem(0x101c).Name = "@BeforeDestruction";
        AnalysisItem vmt = session.GetOrAddItem(0x1010);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalInterfaceClearedByRuntimeHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@IntfClear", 0x402004);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("IInterface", session.Items[0x2004].DataTypeCandidate);
        Assert.IsTrue(session.Items[0x2004].Flags.HasFlag(AnalysisFlags.Data));
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalVariantClearedByRuntimeHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@VarClr", 0x402004);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Variant", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalRecordFinalizedWithKnownRtti()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeRecord",
            0x402004,
            typeInfoAddress: 0x402008);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TRecord";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TRecord", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferRecordTypeFromUnknownRtti()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeRecord",
            0x402004,
            typeInfoAddress: 0x402008);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatNonRttiItemsAsTypeInfo()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeRecord",
            0x402004,
            typeInfoAddress: 0x402008);
        AnalysisItem unrelated = session.GetOrAddItem(0x2008);
        unrelated.Name = "TUnrelated";
        unrelated.TypeKind = DelphiTypeKind.Record;

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalArrayFinalizedWithKnownRttiAndCount()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeArray",
            0x402004,
            typeInfoAddress: 0x402008,
            ecxValue: 3);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("array[3] of TItem", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferFinalizedArrayFromUnknownRtti()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeArray",
            0x402004,
            typeInfoAddress: 0x402008,
            ecxValue: 3);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferFinalizedArrayWithUnrepresentableCount()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeArray",
            0x402004,
            typeInfoAddress: 0x402008,
            ecxValue: 0x80000000);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalDynamicArrayFromSetLengthHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@DynArraySetLength",
            0x402004,
            typeInfoAddress: 0x402008);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TItem", session.Items[0x2004].DataTypeCandidate);
        Assert.IsTrue(session.Items[0x2004].Flags.HasFlag(AnalysisFlags.DynamicArrayCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksGlobalDynamicArrayPassedToAddRefHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@DynArrayAddRef", 0x402004);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x2004].Flags.HasFlag(AnalysisFlags.DynamicArrayCandidate));
        Assert.IsNull(session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesLocalAnsiStringArrayCleanedByRuntimeHelper()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession("@LStrArrayClr", 2);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[]
            {
                new StackLocalVariable(-8, 4, "AnsiString"),
                new StackLocalVariable(-4, 4, "AnsiString")
            },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesLocalWideStringArrayCleanedByRuntimeHelper()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession("@WStrArrayClr", 1);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "WideString") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesLocalUnicodeStringArrayCleanedByRuntimeHelper()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession("@UStrArrayClr", 1);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "UString") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesLocalRecordFinalizedWithKnownRtti()
    {
        AnalysisSession session = CreateLocalRecordFinalizeSession();
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TRecord";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "TRecord") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesLocalDynamicArrayFromSetLengthRtti()
    {
        AnalysisSession session = CreateLocalRecordFinalizeSession("@DynArraySetLength");
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "TItem") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisPreservesKnownLocalTypeWhenFinalizingDynamicArray()
    {
        AnalysisSession session = CreateLocalStringThenDynamicArraySession();
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "AnsiString") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisMarksLocalDynamicArrayPassedToAddRefHelper()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession("@DynArrayAddRef", 2);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "array of ?") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesLocalDynamicArrayCopySourceAndDestination()
    {
        AnalysisSession session = CreateLocalDynamicArrayCopySession();
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[]
            {
                new StackLocalVariable(-12, 4, "TItem"),
                new StackLocalVariable(-8, 4, "TItem")
            },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferTooManyLocalArrayVariables()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession("@LStrArrayClr", 257);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].StackLocalVariables.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferLocalArrayAfterEaxIsOverwritten()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession(
            "@LStrArrayClr",
            2,
            overwriteEax: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].StackLocalVariables.Count);
    }

    [TestMethod]
    public async Task AnalysisMarksTryFinallyExitSequenceWithoutCrossingConditionalBranch()
    {
        AnalysisSession session = CreateTryFinallyExitSession("@TryFinallyExit");

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.FinallyExit));
        Assert.IsFalse(session.Items[0x1003].Flags.HasFlag(AnalysisFlags.FinallyExit));
        Assert.IsTrue(session.Items[0x1005].Flags.HasFlag(AnalysisFlags.FinallyExit));
        Assert.IsTrue(session.Items[0x1006].Flags.HasFlag(AnalysisFlags.FinallyExit));
        Assert.IsTrue(session.Items[0x100b].Flags.HasFlag(AnalysisFlags.FinallyExit));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkOrdinaryRuntimeCallAsFinallyExit()
    {
        AnalysisSession session = CreateTryFinallyExitSession("@OtherRuntimeHelper");

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.Values.Any(item => item.Flags.HasFlag(AnalysisFlags.FinallyExit)));
    }

    [TestMethod]
    public async Task AnalysisResolvesDelphi5DynamicMethodFromBxAndParentVmt()
    {
        AnalysisSession session = CreateDynamicMethodSession(DelphiVersion.Delphi5, overwriteMethodId: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1012].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x1030).Count);
    }

    [TestMethod]
    public async Task AnalysisResolvesModernDynamicMethodFromSi()
    {
        AnalysisSession session = CreateDynamicMethodSession(DelphiVersion.Delphi2009, overwriteMethodId: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1012].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1034));
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveCallDynamicMethodWhenDelphiVersionIsUnknown()
    {
        AnalysisSession session = CreateDynamicMethodSession(DelphiVersion.Unknown, overwriteMethodId: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1012].CrossReferences.Any(reference =>
            reference.TargetAddress is 0x1030 or 0x1034));
    }

    [TestMethod]
    public async Task AnalysisResolvesFindDynamicMethodFromDxWithoutVersionGuessing()
    {
        AnalysisSession session = CreateDynamicMethodSession(
            DelphiVersion.Unknown,
            overwriteMethodId: false,
            findDynamicMethod: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1012].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveDynamicMethodAfterIdRegisterOverwrite()
    {
        AnalysisSession session = CreateDynamicMethodSession(DelphiVersion.Delphi5, overwriteMethodId: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1015].CrossReferences.Any(reference =>
            reference.TargetAddress is 0x1030 or 0x1034));
    }

    [TestMethod]
    public async Task AnalysisResolvesIndirectVirtualCallFromKnownClassVmtSlot()
    {
        AnalysisSession session = CreateVirtualMethodSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x02, 0xff, 0x50, 0x04, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x100e].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
        Assert.IsTrue(session.Items[0x100e].Flags.HasFlag(AnalysisFlags.Call));
        Assert.IsTrue(session.Items[0x1030].Flags.HasFlag(AnalysisFlags.ProcedureStart));
    }

    [TestMethod]
    public async Task AnalysisResolvesInheritedIndirectVirtualCallFromParentVmt()
    {
        AnalysisSession session = CreateVirtualMethodSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x02, 0xff, 0x50, 0x04, 0xc3]);
        AnalysisItem childVmt = session.Items[0x2000];
        childVmt.VirtualMethods = [];
        childVmt.ParentAddress = 0x2010;
        AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
        parentVmt.Name = "TBase";
        parentVmt.SetFlags(AnalysisFlags.Vmt);
        parentVmt.VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1030)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x100e].CrossReferences.Any(reference =>
            reference.TargetAddress == 0x1030));
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveIndirectVirtualCallAfterBaseRegisterClobber()
    {
        AnalysisSession session = CreateVirtualMethodSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x31, 0xd2, 0x8b, 0x02, 0xff, 0x50, 0x04, 0xc3]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1010].CrossReferences.Any(reference =>
            reference.TargetAddress == 0x1030));
    }

    [TestMethod]
    public async Task AnalysisResolvesKnownClassFieldAccessFromExactVmtOffset()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x4a, 0x08, 0xc3],
            useParentField: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual(8, access.Offset);
        Assert.AreEqual("FCount", access.FieldName);
        Assert.AreEqual("Integer", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferClassFieldAfterObjectRegisterClobber()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x31, 0xd2, 0x8b, 0x4a, 0x08, 0xc3],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x100e].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeThroughTypedClassField()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x42, 0x08,
                0x8b, 0x48, 0x0c,
                0xc3
            ],
            useParentField: false);
        AnalysisItem nestedTypeInfo = session.Items[0x2018];
        nestedTypeInfo.TypeKind = DelphiTypeKind.Class;
        nestedTypeInfo.ClassVmtAddress = 0x2020;
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);
        nestedVmt.Fields = [new DelphiVmtField("FValue", 12, 0x2030)];
        session.GetOrAddItem(0x2030).Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x100c].MemberAccessCandidates.Single().OwnerTypeName);
        MemberAccessCandidate nestedAccess = session.Items[0x100f].MemberAccessCandidates.Single();
        Assert.AreEqual("TInner", nestedAccess.OwnerTypeName);
        Assert.AreEqual("FValue", nestedAccess.FieldName);
    }

    [TestMethod]
    public async Task AnalysisResolvesRecordFieldAccessThroughClassField()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x42, 0x08,
                0x8b, 0x40, 0x04,
                0x8b, 0x48, 0x04,
                0xc3
            ],
            useParentField: false);
        AnalysisItem recordTypeInfo = session.Items[0x2018];
        recordTypeInfo.TypeKind = DelphiTypeKind.Record;
        recordTypeInfo.Name = "TPoint";
        recordTypeInfo.RecordFields = [new DelphiRttiRecordField("f4", 4, 0x2020)];
        AnalysisItem nestedRecordTypeInfo = session.GetOrAddItem(0x2020);
        nestedRecordTypeInfo.TypeKind = DelphiTypeKind.Record;
        nestedRecordTypeInfo.Name = "TSize";
        nestedRecordTypeInfo.RecordFields = [new DelphiRttiRecordField("f4", 4, 0x2030)];
        AnalysisItem integerTypeInfo = session.GetOrAddItem(0x2030);
        integerTypeInfo.TypeKind = DelphiTypeKind.Integer;
        integerTypeInfo.Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TPoint", session.Items[0x100f].MemberAccessCandidates.Single().OwnerTypeName);
        Assert.AreEqual("f4", session.Items[0x100f].MemberAccessCandidates.Single().FieldName);
        Assert.AreEqual("TSize", session.Items[0x1012].MemberAccessCandidates.Single().OwnerTypeName);
        Assert.AreEqual("f4", session.Items[0x1012].MemberAccessCandidates.Single().FieldName);
    }

    [TestMethod]
    public async Task AnalysisResolvesNestedRecordFieldFromCombinedObjectOffset()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x42, 0x0c, 0xc3],
            useParentField: false);
        AnalysisItem recordTypeInfo = session.Items[0x2018];
        recordTypeInfo.TypeKind = DelphiTypeKind.Record;
        recordTypeInfo.Name = "TPoint";
        recordTypeInfo.RecordSizeBytes = 8;
        recordTypeInfo.RecordFields = [new DelphiRttiRecordField("f4", 4, 0x2020)];
        AnalysisItem integerTypeInfo = session.GetOrAddItem(0x2020);
        integerTypeInfo.TypeKind = DelphiTypeKind.Integer;
        integerTypeInfo.Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount.f4", access.FieldName);
        Assert.AreEqual("Integer", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisPreservesExistingGlobalTypeForDynamicArraySetLength()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@DynArraySetLength",
            0x402004,
            typeInfoAddress: 0x402008);
        AnalysisItem data = session.GetOrAddItem(0x2004);
        data.DataTypeCandidate = "TExisting";
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExisting", data.DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesBothGlobalDynamicArraysFromCopyHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@DynArrayCopy",
            0x402004,
            typeInfoAddress: 0x402008,
            ecxValue: 0x40200c);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TItem", session.Items[0x2004].DataTypeCandidate);
        Assert.AreEqual("TItem", session.Items[0x200c].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotApplyGlobalTypeToCodeAddress()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@VarClr", 0x401000);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x1000, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisDoesNotApplyGlobalTypeForUnknownRuntimeHelper()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@UnknownHelper", 0x402004);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisDoesNotApplyGlobalTypeAfterEaxIsOverwritten()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession("@VarClr", 0x402004, overwriteEax: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2004, out AnalysisItem? item)
            && item.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisPropagatesKnownClassTypeBetweenRegisterArguments()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0x8b;
        code[6] = 0xd0;
        code[7] = 0xe8;
        code[8] = 0x04;
        code[12] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesKnownClassTypeAcrossRegisterXchg()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xb8;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0x92;
        code[6] = 0xe8;
        code[7] = 0x05;
        code[11] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisInfersAsClassReturnTypeFromAbsoluteVmtLea()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x8d;
        code[1] = 0x15;
        code[2] = 0x18;
        code[3] = 0x10;
        code[4] = 0x40;
        code[5] = 0x00;
        code[6] = 0xe8;
        code[7] = 0x05;
        code[11] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesAsClassReturnTypeAcrossDirectCallers()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xba;
        code[0x11] = 0x08;
        code[0x12] = 0x10;
        code[0x13] = 0x40;
        code[0x14] = 0x00;
        code[0x15] = 0xe8;
        code[0x16] = 0x02;
        code[0x1a] = 0xc3;
        code[0x1c] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x101c).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1008);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TExample", session.Items[0x1010].ReturnTypeCandidate);
        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferAsClassReturnTypeFromUnknownEdxValue()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xba;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferAsClassReturnTypeFromBasedLea()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x8d;
        code[1] = 0x90;
        code[2] = 0x18;
        code[3] = 0x10;
        code[4] = 0x40;
        code[5] = 0x00;
        code[6] = 0xe8;
        code[7] = 0x05;
        code[11] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotCarryAsClassTypeAcrossConditionalBranch()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xba;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0x74;
        code[6] = 0x00;
        code[7] = 0xe8;
        code[8] = 0x04;
        code[12] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisClearsAsClassTypeCandidateWhenEaxIsOverwritten()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xba;
        code[1] = 0x18;
        code[2] = 0x10;
        code[3] = 0x40;
        code[4] = 0x00;
        code[5] = 0xe8;
        code[6] = 0x06;
        code[10] = 0xb8;
        code[11] = 0x01;
        code[15] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).Name = "@AsClass";
        AnalysisItem vmt = session.GetOrAddItem(0x1018);
        vmt.Name = "TExample";
        vmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossRdmsr()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0x32;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesReturnCandidateAcrossCwd()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x66;
        code[6] = 0x99;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossPopad()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x61;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossPopa()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x66;
        code[6] = 0x61;
        code[7] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Popa", session.Items[0x1005].Name);
        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossXlat()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xd7;
        code[6] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);
        DecodedInstruction xlat = new IcedInstructionDecoder().Decode([0xd7], 0x1005, 32);

        Assert.AreEqual("Xlatb", xlat.Mnemonic);
        Assert.AreEqual(InstructionFlowControl.Next, xlat.FlowControl);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Xlatb", session.Items[0x1005].Name);
        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateReturnCandidateAcrossBitSet()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x0f;
        code[6] = 0xab;
        code[7] = 0xd0;
        code[8] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisMarksDirectCalleeAsFunctionWhenEaxResultIsRead()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x83;
        code[6] = 0xf8;
        code[7] = 0x00;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksDirectCalleeAsFunctionWhenEaxIsExchanged()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x92;
        code[6] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksDirectCalleeAsFunctionWhenEaxResultIsReturned()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksCalleeAsFunctionWhenX87ReturnIsStored()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xd9;
        code[6] = 0x1d;
        code[7] = 0x00;
        code[8] = 0x20;
        code[9] = 0x00;
        code[10] = 0x00;
        code[11] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Fstp", session.Items[0x1005].Name);
        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkCallAsReturningX87ValueWhenStoreIsNotImmediate()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x17;
        code[5] = 0x90;
        code[6] = 0xd9;
        code[7] = 0x1d;
        code[8] = 0x00;
        code[9] = 0x20;
        code[10] = 0x00;
        code[11] = 0x00;
        code[12] = 0xb8;
        code[13] = 0x01;
        code[17] = 0xc3;
        code[0x1c] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x101c].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkCalleeAsFunctionWhenEaxIsOverwritten()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x13;
        code[5] = 0xb8;
        code[10] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkImportedCalleeAsFunctionCandidate()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x83;
        code[6] = 0xf8;
        code[7] = 0x00;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).SetFlags(AnalysisFlags.Import);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotReclassifyConstructorCandidateAsFunction()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0x83;
        code[6] = 0xf8;
        code[7] = 0x00;
        code[8] = 0xc3;
        code[0x10] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1010).SetFlags(AnalysisFlags.ConstructorCandidate);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.ConstructorCandidate));
        Assert.IsFalse(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisPropagatesReturnCandidateAcrossNestedDirectCalls()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x03;
        code[5] = 0xc3;
        code[0x08] = 0xe8;
        code[0x09] = 0x03;
        code[0x0d] = 0xc3;
        code[0x10] = 0x0f;
        code[0x11] = 0x95;
        code[0x12] = 0xc0;
        code[0x13] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Boolean", session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1008].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1010].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPreservesRegisterArgumentCandidatesAcrossDelphiRuntimeChecks()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x03;
        code[5] = 0x89;
        code[6] = 0xc2;
        code[7] = 0xc3;
        code[8] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1008).Name = "@IntOver";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items[0x1000].RegisterArgumentCandidates.Count);
        Assert.AreEqual("EAX", session.Items[0x1000].RegisterArgumentCandidates[0]);
    }

    [TestMethod]
    public async Task AnalysisStillClobbersRegisterArgumentCandidatesAcrossUnknownCalls()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x03;
        code[5] = 0x89;
        code[6] = 0xc2;
        code[7] = 0xc3;
        code[8] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].RegisterArgumentCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateCallReturnCandidateWhenEaxIsWrittenAfterCall()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x13;
        code[5] = 0xb8;
        code[10] = 0xc3;
        code[0x18] = 0x0f;
        code[0x19] = 0x95;
        code[0x1a] = 0xc0;
        code[0x1b] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
        Assert.AreEqual("Boolean", session.Items[0x1018].ReturnTypeCandidate);
    }

    private static AnalysisSession CreateSession(byte[] code)
    {
        byte[] image = new byte[0x20];
        code.CopyTo(image, 0);
        return new AnalysisSession(
            "stack.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
    }

    private static AnalysisSession CreateRuntimeVariableTypeSession(
        string helperName,
        uint variableAddress,
        bool overwriteEax = false,
        uint? typeInfoAddress = null,
        uint? ecxValue = null)
    {
        byte[] image = new byte[0x40];
        image[0] = 0xb8;
        image[1] = (byte)variableAddress;
        image[2] = (byte)(variableAddress >> 8);
        image[3] = (byte)(variableAddress >> 16);
        image[4] = (byte)(variableAddress >> 24);
        int callOffset = 5;
        int callReturnOffset = 10;
        if (overwriteEax)
        {
            image[5] = 0x31;
            image[6] = 0xc0;
            callOffset = 7;
            callReturnOffset = 12;
        }
        else if (typeInfoAddress is uint typeInfoValue)
        {
            image[5] = 0xba;
            image[6] = (byte)typeInfoValue;
            image[7] = (byte)(typeInfoValue >> 8);
            image[8] = (byte)(typeInfoValue >> 16);
            image[9] = (byte)(typeInfoValue >> 24);
            callOffset = 10;
            callReturnOffset = 15;
        }

        if (ecxValue is uint count)
        {
            image[10] = 0xb9;
            image[11] = (byte)count;
            image[12] = (byte)(count >> 8);
            image[13] = (byte)(count >> 16);
            image[14] = (byte)(count >> 24);
            callOffset = 15;
            callReturnOffset = 20;
        }

        image[callOffset] = 0xe8;
        image[callOffset + 1] = (byte)(0x18 - callOffset - 5);
        image[callReturnOffset] = 0xc3;
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "runtime-type.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateLocalStringArrayCleanupSession(
        string helperName,
        uint count,
        bool overwriteEax = false)
    {
        byte[] image = new byte[0x40];
        image[0] = 0x55;
        image[1] = 0x8b;
        image[2] = 0xec;
        image[3] = 0x8d;
        image[4] = 0x45;
        image[5] = 0xf8;
        int callOffset = 11;
        int returnOffset = 16;
        if (overwriteEax)
        {
            image[6] = 0xb8;
            image[7] = 0x04;
            image[8] = 0x20;
            image[9] = 0x40;
            image[10] = 0x00;
            callOffset = 16;
            returnOffset = 21;
        }

        image[callOffset - 5] = 0xba;
        image[callOffset - 4] = (byte)count;
        image[callOffset - 3] = (byte)(count >> 8);
        image[callOffset - 2] = (byte)(count >> 16);
        image[callOffset - 1] = (byte)(count >> 24);
        image[callOffset] = 0xe8;
        image[callOffset + 1] = (byte)(0x18 - (callOffset + 5));
        image[returnOffset] = 0xc3;
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "local-string-array.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateLocalRecordFinalizeSession(string helperName = "@FinalizeRecord")
    {
        byte[] image = new byte[0x40];
        image[0] = 0x55;
        image[1] = 0x8b;
        image[2] = 0xec;
        image[3] = 0x8d;
        image[4] = 0x45;
        image[5] = 0xf8;
        image[6] = 0xba;
        image[7] = 0x08;
        image[8] = 0x20;
        image[9] = 0x40;
        image[10] = 0x00;
        image[11] = 0xe8;
        image[12] = 0x08;
        image[16] = 0xc3;
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "local-record-finalize.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateLocalDynamicArrayCopySession()
    {
        byte[] image = new byte[0x50];
        image[0] = 0x55;
        image[1] = 0x8b;
        image[2] = 0xec;
        image[3] = 0x8d;
        image[4] = 0x45;
        image[5] = 0xf8;
        image[6] = 0x8d;
        image[7] = 0x4d;
        image[8] = 0xf4;
        image[9] = 0xba;
        image[10] = 0x08;
        image[11] = 0x20;
        image[12] = 0x40;
        image[13] = 0x00;
        image[14] = 0xe8;
        image[15] = 0x0d;
        image[19] = 0xc3;
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "local-dynamic-array-copy.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x28, 0, 0x28, true),
                new PeSection(".data", 0x2000, 0x28, 0x28, 0x28, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@DynArrayCopy";
        return session;
    }

    private static AnalysisSession CreateTryFinallyExitSession(string helperName)
    {
        byte[] image = new byte[0x30];
        image[0] = 0x83;
        image[1] = 0xf8;
        image[2] = 0x00;
        image[3] = 0x74;
        image[4] = 0x00;
        image[5] = 0x90;
        image[6] = 0xe8;
        image[7] = 0x0d;
        image[11] = 0xeb;
        image[12] = 0x00;
        image[13] = 0xc3;
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "finally-exit.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateDynamicMethodSession(
        DelphiVersion version,
        bool overwriteMethodId,
        bool findDynamicMethod = false)
    {
        byte[] image = new byte[0x70];
        image[0] = 0xba;
        image[1] = 0x00;
        image[2] = 0x20;
        image[3] = 0x40;
        image[4] = 0x00;
        image[5] = 0xe8;
        image[6] = 0x16;
        image[10] = 0x66;
        image[11] = findDynamicMethod ? (byte)0xba : (byte)0xbb;
        image[12] = 0x11;
        image[13] = 0x11;
        image[14] = 0x66;
        image[15] = 0xbe;
        image[16] = 0x22;
        image[17] = 0x22;
        int dynamicCallOffset = 18;
        if (overwriteMethodId)
        {
            image[18] = 0x83;
            image[19] = 0xc3;
            image[20] = 0x01;
            dynamicCallOffset = 21;
        }

        image[dynamicCallOffset] = 0xe8;
        image[dynamicCallOffset + 1] = (byte)(0x28 - dynamicCallOffset - 5);
        image[dynamicCallOffset + 5] = 0xc3;
        image[0x20] = 0xc3;
        image[0x28] = 0xc3;
        image[0x30] = 0xc3;
        image[0x34] = 0xc3;
        AnalysisSession session = new(
            "dynamic-method.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x30, 0x40, 0x30, false)
            ],
            0x400000,
            version)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@AsClass";
        session.GetOrAddItem(0x1028).Name = findDynamicMethod ? "@FindDynaInst" : "@CallDynaInst";
        AnalysisItem childVmt = session.GetOrAddItem(0x2000);
        childVmt.Name = "TChild";
        childVmt.ParentAddress = 0x2010;
        childVmt.SetFlags(AnalysisFlags.Vmt);
        AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
        parentVmt.Name = "TBase";
        parentVmt.SetFlags(AnalysisFlags.Vmt);
        parentVmt.DynamicMethods =
        [
            new DelphiVmtDynamicMethod(0x1111, 0x1030),
            new DelphiVmtDynamicMethod(0x2222, 0x1034)
        ];
        return session;
    }

    private static AnalysisSession CreateVirtualMethodSession(byte[] code)
    {
        byte[] image = new byte[0x60];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "virtual-method.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000,
            DelphiVersion.Unknown)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@ClassCreate";
        AnalysisItem vmt = session.GetOrAddItem(0x2000);
        vmt.Name = "TChild";
        vmt.SetFlags(AnalysisFlags.Vmt);
        vmt.VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1030)];
        return session;
    }

    private static AnalysisSession CreateClassFieldAccessSession(byte[] code, bool useParentField)
    {
        byte[] image = new byte[0x80];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "class-field-access.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            DelphiVersion.Unknown)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@ClassCreate";
        AnalysisItem childVmt = session.GetOrAddItem(0x2000);
        childVmt.Name = "TChild";
        childVmt.SetFlags(AnalysisFlags.Vmt);
        if (useParentField)
        {
            childVmt.ParentAddress = 0x2010;
            AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
            parentVmt.Name = "TBase";
            parentVmt.SetFlags(AnalysisFlags.Vmt);
            parentVmt.Fields = [new DelphiVmtField("FCount", 8, 0x2018)];
            session.GetOrAddItem(0x2018).Name = "Integer";
        }
        else
        {
            childVmt.Fields = [new DelphiVmtField("FCount", 8, 0x2018)];
            session.GetOrAddItem(0x2018).Name = "Integer";
        }

        return session;
    }

    private static AnalysisSession CreateLocalStringThenDynamicArraySession()
    {
        byte[] image = new byte[0x60];
        image[0] = 0x55;
        image[1] = 0x8b;
        image[2] = 0xec;
        image[3] = 0x8d;
        image[4] = 0x45;
        image[5] = 0xf8;
        image[6] = 0xba;
        image[7] = 0x01;
        image[11] = 0xe8;
        image[12] = 0x20;
        image[16] = 0x8d;
        image[17] = 0x45;
        image[18] = 0xf8;
        image[19] = 0xba;
        image[20] = 0x08;
        image[21] = 0x20;
        image[22] = 0x40;
        image[24] = 0xe8;
        image[25] = 0x1b;
        image[29] = 0xc3;
        image[0x30] = 0xc3;
        image[0x38] = 0xc3;
        AnalysisSession session = new(
            "local-string-dynamic-array.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1030).Name = "@LStrArrayClr";
        session.GetOrAddItem(0x1038).Name = "@DynArraySetLength";
        return session;
    }
}
