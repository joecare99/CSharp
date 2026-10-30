using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using IDR.Infrastructure.KnowledgeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public async Task AnalysisClearsStaleStackArgumentMismatchWhenRecomputingProcedureSummary()
    {
        AnalysisSession session = CreateSession(
        [
            0x55,
            0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0xc2, 0x04, 0x00
        ]);
        session.GetOrAddItem(0x1000).SetFlags(AnalysisFlags.StackArgumentSizeMismatch);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items[0x1000].Flags.HasFlag(AnalysisFlags.StackArgumentSizeMismatch));
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
    public async Task AnalysisRecordsConstantCrossReferenceToAbsoluteDataAddress()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x24, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CrossReference reference = session.GetIncomingCrossReferences(0x2024).Single();
        Assert.AreEqual(0x1000u, reference.SourceAddress);
        Assert.AreEqual(CrossReferenceKind.Constant, reference.Kind);
    }

    [TestMethod]
    public async Task AnalysisResolvesVarCharacterPointerPrototypeFromFileBackedPointerSlot()
    {
        foreach ((string typeName, byte[] literal, string expectedName) in new[]
        {
            ("PAnsiChar", Encoding.Latin1.GetBytes("var ansi\0"), "var ansi"),
            ("PWideChar", Encoding.Unicode.GetBytes("var wide\0"), "var wide")
        })
        {
            AnalysisSession session = CreateVarStringPointerPrototypeSession(
                typeName,
                literal,
                terminateString: true);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(typeName, session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual(expectedName, session.Items[0x2020].Name);
            Assert.IsTrue(session.Items[0x2020].Flags.HasFlag(AnalysisFlags.String));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesPCharUsingDelphiVersionEncoding()
    {
        foreach ((DelphiVersion version, byte[] payload, string text) in new[]
        {
            (
                DelphiVersion.Delphi2007,
                Encoding.Latin1.GetBytes("Legacy PChar"),
                "Legacy PChar"),
            (
                DelphiVersion.Delphi2009,
                [.. Encoding.Unicode.GetBytes("Unicode PChar"), 0, 0],
                "Unicode PChar")
        })
        {
            AnalysisSession session = CreatePrototypeStringSession(
                0,
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", "PChar"),
                payload,
                delphiVersion: version);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsTrue(
                session.Items.TryGetValue(0x2010, out AnalysisItem? stringItem),
                $"No string item for {version} PChar; analysis contained {session.Items.Count} items.");
            Assert.AreEqual(text, stringItem!.Name, version.ToString());
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesSingleStackStringArgumentFromImmediatePush()
    {
        foreach (byte callingConvention in new byte[] { 1, 3 })
        {
            AnalysisSession session = CreateStackStringPrototypeSession(
                callingConvention,
                Encoding.Latin1.GetBytes("Stack argument"));

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual("Stack argument", session.Items[0x2020].Name);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesStackStringArgumentAcrossStraightLineInstruction()
    {
        AnalysisSession session = CreateStackStringPrototypeSession(
            3,
            Encoding.Latin1.GetBytes("Stack argument"),
            insertNop: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Stack argument", session.Items[0x2020].Name);
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2020)
                .Count(reference => reference.SourceAddress == 0x1006
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisMapsMultipleStackStringArgumentsInCallingOrder()
    {
        AnalysisSession session = CreateMultiStackStringPrototypeSession();

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("First argument", session.Items[0x2020].Name);
        Assert.AreEqual("Second argument", session.Items[0x2030].Name);
        foreach (uint stringRva in new uint[] { 0x2020, 0x2030 })
        {
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(stringRva)
                    .Count(reference => reference.SourceAddress == 0x100a
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesStackStringArgumentBesideUnanalyzedConstArgument()
    {
        AnalysisSession session = CreateMixedStackStringPrototypeSession();

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Mixed stack", session.Items[0x2030].Name);
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2030)
                .Count(reference => reference.SourceAddress == 0x100a
                    && reference.Kind == CrossReferenceKind.Constant));
        Assert.IsFalse(session.Items.TryGetValue(0x2020, out AnalysisItem? ignoredArgument)
            && ignoredArgument.DataTypeCandidate is not null);
        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x2020)
                .Count(reference => reference.SourceAddress == 0x100a
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisResolvesStackGuidArgumentsAndRejectsInvalidTargets()
    {
        Guid expectedGuid = Guid.Parse("73a2019b-0ea4-417d-a58d-25e396dc8f31");
        AnalysisSession validSession = CreateStackStringPrototypeSession(
            3,
            expectedGuid.ToByteArray(),
            argumentType: "TGUID");

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(validSession, null, CancellationToken.None);

        Assert.AreEqual(expectedGuid.ToString("D"), validSession.Items[0x2020].Name);
        Assert.AreEqual("TGUID", validSession.Items[0x2020].DataTypeCandidate);

        foreach (uint targetRva in new uint[] { 0x1010, 0x203f })
        {
            AnalysisSession invalidSession = CreateStackStringPrototypeSession(
                3,
                expectedGuid.ToByteArray(),
                argumentType: "TGUID",
                targetRva: targetRva);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(invalidSession, null, CancellationToken.None);

            Assert.IsFalse(
                invalidSession.Items.TryGetValue(targetRva, out AnalysisItem? item)
                && item.DataTypeCandidate == "TGUID");
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesStackVarCharacterPointerArguments()
    {
        foreach ((DelphiVersion version, string typeName, byte[] payload, string expectedText) in new[]
        {
            (
                DelphiVersion.Delphi2007,
                "PAnsiChar",
                Encoding.Latin1.GetBytes("Stack var ANSI"),
                "Stack var ANSI"),
            (
                DelphiVersion.Delphi2009,
                "PChar",
                Encoding.Unicode.GetBytes("Wide stack"),
                "Wide stack")
        })
        {
            AnalysisSession session = CreateStackStringPrototypeSession(
                3,
                payload,
                argumentType: typeName,
                argumentTag: 0x22,
                targetRva: 0x2010,
                delphiVersion: version);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(typeName, session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual(expectedText, session.Items[0x2020].Name);
            foreach (uint targetRva in new uint[] { 0x2010, 0x2020 })
            {
                Assert.AreEqual(
                    1,
                    session.GetIncomingCrossReferences(targetRva)
                        .Count(reference => reference.SourceAddress == 0x1005
                            && reference.Kind == CrossReferenceKind.Constant));
            }
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesStackShortStringAndWideStringValueAndVarArguments()
    {
        byte[] shortString = [11, .. Encoding.Latin1.GetBytes("Short stack")];
        foreach (bool isVarArgument in new[] { false, true })
        {
            AnalysisSession shortSession = CreateStackStringPrototypeSession(
                3,
                shortString,
                argumentType: "ShortString",
                targetRva: isVarArgument ? 0x2010u : 0x2020u,
                argumentTag: isVarArgument ? (byte)0x22 : (byte)0x21);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(shortSession, null, CancellationToken.None);

            Assert.AreEqual("Short stack", shortSession.Items[0x2020].Name);
            Assert.AreEqual("ShortString", shortSession.Items[0x2020].DataTypeCandidate);

            byte[] widePayload = [.. Encoding.Unicode.GetBytes("Wide stack"), 0, 0];
            AnalysisSession wideSession = CreateStackWideStringPrototypeSession(isVarArgument, widePayload);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(wideSession, null, CancellationToken.None);

            Assert.AreEqual("Wide stack", wideSession.Items[0x2020].Name);
            Assert.AreEqual("WideString", wideSession.Items[0x2020].DataTypeCandidate);
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesStackManagedStringAndResourceRecordArguments()
    {
        AnalysisSession managedSession = CreateStackStringPrototypeSession(
            3,
            Encoding.Unicode.GetBytes("Managed"),
            argumentType: "UnicodeString",
            delphiVersion: DelphiVersion.Delphi2009);
        AnalysisItem managedString = managedSession.GetOrAddItem(0x2020);
        managedString.SetFlags(AnalysisFlags.Data | AnalysisFlags.String);
        managedString.TypeKind = DelphiTypeKind.UnicodeString;
        managedString.Name = "Managed";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(managedSession, null, CancellationToken.None);

        Assert.AreEqual("UnicodeString", managedSession.Items[0x2020].DataTypeCandidate);

        AnalysisSession resourceSession = CreateStackStringPrototypeSession(
            3,
            [0, 0, 0, 0, 42, 0, 0, 0],
            argumentType: "PResStringRec");
        resourceSession.LoadPeDirectories(
            [],
            [],
            [new PeResourceString(42, 0, "Stack resource")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(resourceSession, null, CancellationToken.None);

        Assert.AreEqual("Stack resource", resourceSession.Items[0x1005].ResourceStringCandidate);
        Assert.AreEqual("PResStringRec", resourceSession.Items[0x2020].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisRemovesStackWideStringTypeKindAfterLateCallReprocessing()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xb8, 0x30, 0x20, 0x40, 0x00,
            0x68, 0x20, 0x20, 0x40, 0x00,
            0xe8, 0x11, 0x00, 0x00, 0x00,
            0xeb, 0xf9
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        byte[] widePayload = [.. Encoding.Unicode.GetBytes("Wide stack"), 0, 0];
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(0x5c, sizeof(int)),
            widePayload.Length - sizeof(ushort));
        widePayload.CopyTo(image, 0x60);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadStackWideText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = 3,
            Arguments =
            [
                new KnowledgeBaseArgument(
                    0x21,
                    false,
                    8,
                    sizeof(uint),
                    "Text",
                    "WideString")
            ]
        };
        AnalysisSession session = new(
            "stack-wide-string-late-reprocessing.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = procedure.Name;
        AnalysisItem wideStringItem = session.GetOrAddItem(0x2020);
        wideStringItem.SetFlags(AnalysisFlags.Data);
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(wideStringItem.TypeKind);
        Assert.IsNull(wideStringItem.Name);
        Assert.IsTrue(wideStringItem.Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsFalse(wideStringItem.Flags.HasFlag(AnalysisFlags.String));
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveUnsupportedOrNonImmediateStackStringArguments()
    {
        foreach ((byte callingConvention, bool pushImmediate, uint callRva) in new[]
        {
            ((byte)2, true, 0x1005u),
            ((byte)4, true, 0x1005u),
            ((byte)3, false, 0x1005u)
        })
        {
            AnalysisSession session = CreateStackStringPrototypeSession(
                callingConvention,
                Encoding.Latin1.GetBytes("Stack argument"),
                pushImmediate);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == callRva
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTypeVarCharacterPointerWithInvalidTargetOrTerminator()
    {
        foreach ((uint targetRva, bool terminateString) in new[]
        {
            (0x1020u, true),
            (0x2020u, false)
        })
        {
            AnalysisSession session = CreateVarStringPointerPrototypeSession(
                "PAnsiChar",
                Encoding.Latin1.GetBytes("invalid target"),
                terminateString,
                targetRva);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == "PAnsiChar");
            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesVarPCharUsingDelphiVersionEncoding()
    {
        foreach ((DelphiVersion version, byte[] payload, string text) in new[]
        {
            (
                DelphiVersion.Delphi2007,
                Encoding.Latin1.GetBytes("Legacy var PChar"),
                "Legacy var PChar"),
            (
                DelphiVersion.Delphi2009,
                [.. Encoding.Unicode.GetBytes("Wide PChar"), 0, 0],
                "Wide PChar")
        })
        {
            AnalysisSession session = CreateVarStringPointerPrototypeSession(
                "PChar",
                payload,
                terminateString: true,
                delphiVersion: version);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual("PChar", session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual(text, session.Items[0x2020].Name);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesVarShortStringPrototypeFromFileBackedPointerSlot()
    {
        foreach (string typeName in new[] { "ShortString", "PShortString" })
        {
            AnalysisSession session = CreateVarShortStringPrototypeSession(
                typeName,
                [5, .. Encoding.Latin1.GetBytes("Short")]);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(typeName, session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual("ShortString", session.Items[0x2020].DataTypeCandidate);
            Assert.AreEqual("Short", session.Items[0x2020].Name);
            Assert.IsTrue(session.Items[0x2020].Flags.HasFlag(AnalysisFlags.String));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2020)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveVarShortStringWhenLengthExceedsTarget()
    {
        AnalysisSession session = CreateVarShortStringPrototypeSession(
            "PShortString",
            [12, .. Encoding.Latin1.GetBytes("truncated")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
            && slot.DataTypeCandidate == "PShortString");
        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x2020)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisResolvesVarGuidPrototypeFromFileBackedPointerSlot()
    {
        AnalysisSession session = CreateVarGuidPrototypeSession(
            0x2020,
            [
                0x33, 0x22, 0x11, 0x00,
                0x55, 0x44,
                0x77, 0x66,
                0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff
            ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TGUID", session.Items[0x2010].DataTypeCandidate);
        Assert.AreEqual("TGUID", session.Items[0x2020].DataTypeCandidate);
        Assert.AreEqual("00112233-4455-6677-8899-aabbccddeeff", session.Items[0x2020].Name);
        foreach (uint address in new[] { 0x2010u, 0x2020u })
        {
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(address)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveVarGuidPrototypeForCodeOrTruncatedTarget()
    {
        foreach ((uint targetRva, bool truncated) in new[]
        {
            (0x1020u, false),
            (0x2038u, true)
        })
        {
            AnalysisSession session = CreateVarGuidPrototypeSession(
                targetRva,
                new byte[16],
                truncated);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == "TGUID");
            Assert.IsFalse(session.Items.TryGetValue(targetRva, out AnalysisItem? target)
                && target.DataTypeCandidate == "TGUID");
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesVarWideStringPrototypeFromFileBackedPointerSlot()
    {
        const string value = "Wide var text";
        AnalysisSession session = CreateVarWideStringPrototypeSession(
            value.Length * sizeof(ushort),
            [.. Encoding.Unicode.GetBytes(value), 0, 0]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("WideString", session.Items[0x2010].DataTypeCandidate);
        Assert.AreEqual("WideString", session.Items[0x2024].DataTypeCandidate);
        Assert.AreEqual(DelphiTypeKind.WideString, session.Items[0x2024].TypeKind);
        Assert.AreEqual(value, session.Items[0x2024].Name);
        foreach (uint address in new[] { 0x2010u, 0x2024u })
        {
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(address)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveVarWideStringWithInvalidLengthOrTerminator()
    {
        foreach ((int byteLength, byte[] payload) in new[]
        {
            (5, new byte[] { 0x41, 0, 0x42, 0, 0, 0 }),
            (4, new byte[] { 0x41, 0, 0x42, 0, 1, 0 })
        })
        {
            AnalysisSession session = CreateVarWideStringPrototypeSession(byteLength, payload);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == "WideString");
            Assert.IsFalse(session.Items.TryGetValue(0x2024, out AnalysisItem? target)
                && target.DataTypeCandidate == "WideString");
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesVarManagedStringPrototypeOnlyForMatchingParsedLiteral()
    {
        foreach ((string argumentType, ushort elementSize) in new[]
        {
            ("AnsiString", (ushort)1),
            ("UnicodeString", (ushort)2),
            ("String", (ushort)2)
        })
        {
            AnalysisSession session = CreateVarManagedStringPrototypeSession(
                argumentType,
                elementSize);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            string expectedType = elementSize == 1 ? "AnsiString" : "UnicodeString";
            Assert.AreEqual(expectedType, session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual(expectedType, session.Items[0x2024].DataTypeCandidate);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2024)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveVarManagedStringWithMismatchedLiteralType()
    {
        AnalysisSession session = CreateVarManagedStringPrototypeSession("AnsiString", 2);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
            && slot.DataTypeCandidate == "AnsiString");
        Assert.IsFalse(session.Items.TryGetValue(0x2024, out AnalysisItem? target)
            && target.DataTypeCandidate == "AnsiString");
        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x2010)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisResolvesLegacyVarAnsiStringPrototypeFromValidatedHeader()
    {
        foreach (string argumentType in new[] { "AnsiString", "String" })
        {
            AnalysisSession session = CreateVarLegacyAnsiStringPrototypeSession(argumentType);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual("AnsiString", session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual("AnsiString", session.Items[0x2028].DataTypeCandidate);
            Assert.AreEqual(DelphiTypeKind.AnsiString, session.Items[0x2028].TypeKind);
            Assert.AreEqual("Legacy var text", session.Items[0x2028].Name);
            foreach (uint address in new[] { 0x2010u, 0x2028u })
            {
                Assert.AreEqual(
                    1,
                    session.GetIncomingCrossReferences(address)
                        .Count(reference => reference.SourceAddress == 0x1005
                            && reference.Kind == CrossReferenceKind.Constant));
            }
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveLegacyVarAnsiStringWithInvalidHeaderOrTruncatedPayload()
    {
        foreach ((int referenceCount, int declaredLength, bool terminator, uint dataRawSize) in new[]
        {
            (0, 15, true, 0x40u),
            (-1, 15, false, 0x40u),
            (-1, 0x10001, true, 0x40u),
            (-1, 15, true, 0x28u)
        })
        {
            AnalysisSession session = CreateVarLegacyAnsiStringPrototypeSession(
                "String",
                referenceCount,
                declaredLength,
                terminator,
                dataRawSize);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == "AnsiString",
                $"Pointer slot was typed for refcount={referenceCount}, length={declaredLength}, terminator={terminator}, rawSize={dataRawSize}.");
            Assert.IsFalse(session.Items.TryGetValue(0x2028, out AnalysisItem? target)
                && target.DataTypeCandidate == "AnsiString",
                $"String target was typed for refcount={referenceCount}, length={declaredLength}, terminator={terminator}, rawSize={dataRawSize}.");
        }
    }

    [TestMethod]
    public async Task AnalysisUsesUniqueKnowledgeBasePrototypeToMarkAnsiAndWideStringArguments()
    {
        foreach ((int registerIndex, string typeName, byte[] literal, string expectedText) in new[]
        {
            (
                0,
                "PAnsiChar",
                Encoding.Latin1.GetBytes("ANSI argument\0"),
                "ANSI argument"),
            (
                1,
                "PWideChar",
                Encoding.Unicode.GetBytes("Wide argument\0"),
                "Wide argument")
        })
        {
            AnalysisSession session = CreatePrototypeStringSession(
                registerIndex,
                new KnowledgeBaseArgument(0x21, true, registerIndex, sizeof(uint), "Text", typeName),
                literal);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(expectedText, session.Items[0x2010].Name);
            Assert.IsTrue(session.Items[0x2010].Flags.HasFlag(AnalysisFlags.String));
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisUsesUniqueKnowledgeBasePrototypeToMarkGuidArgument()
    {
        byte[] guidBytes =
        [
            0x33, 0x22, 0x11, 0x00,
            0x55, 0x44,
            0x77, 0x66,
            0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff
        ];
        AnalysisSession session = CreatePrototypeStringSession(
            0,
            new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Id", "TGUID"),
            guidBytes,
            terminateString: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem guidItem = session.Items[0x2010];
        Assert.IsTrue(guidItem.Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsFalse(guidItem.Flags.HasFlag(AnalysisFlags.String));
        Assert.AreEqual("TGUID", guidItem.DataTypeCandidate);
        Assert.AreEqual("00112233-4455-6677-8899-aabbccddeeff", guidItem.Name);
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2010)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkGuidArgumentForCodeOrTruncatedData()
    {
        foreach (uint address in new[] { 0x1000u, 0x2038u })
        {
            AnalysisSession session = CreatePrototypeStringSession(
                0,
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Id", "TGUID"),
                new byte[16],
                terminateString: false,
                stringRva: address);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(address, out AnalysisItem? item)
                && item.DataTypeCandidate == "TGUID");
            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(address)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisUsesLegacyPrototypeForLongAnsiStringArguments()
    {
        foreach (string argumentType in new[] { "AnsiString", "String" })
        {
            AnalysisSession session = CreateLegacyManagedStringPrototypeSession(argumentType);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            AnalysisItem stringItem = session.Items[0x2008];
            Assert.AreEqual("Legacy text", stringItem.Name, argumentType);
            Assert.AreEqual(DelphiTypeKind.AnsiString, stringItem.TypeKind, argumentType);
            Assert.AreEqual("AnsiString", stringItem.DataTypeCandidate, argumentType);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2008)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant),
                argumentType);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatInvalidLegacyStringHeaderAsLongString()
    {
        AnalysisSession session = CreateLegacyManagedStringPrototypeSession(
            "String",
            referenceCount: 0);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2008, out AnalysisItem? item)
            && item.DataTypeCandidate == "AnsiString");
        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x2008)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisUsesUniqueKnowledgeBasePrototypeForWideStringArguments()
    {
        string value = "Wide value";
        byte[] payload = [.. Encoding.Unicode.GetBytes(value), 0, 0];
        AnalysisSession session = CreateWideStringPrototypeSession(
            value.Length * sizeof(ushort),
            payload);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem item = session.Items[0x2004];
        Assert.IsTrue(item.Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsTrue(item.Flags.HasFlag(AnalysisFlags.String));
        Assert.AreEqual(DelphiTypeKind.WideString, item.TypeKind);
        Assert.AreEqual("WideString", item.DataTypeCandidate);
        Assert.AreEqual(value, item.Name);
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2004)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkWideStringWithOddOrTruncatedLength()
    {
        foreach (int byteLength in new[] { 5, 12 })
        {
            AnalysisSession session = CreateWideStringPrototypeSession(
                byteLength,
                [],
                contentRva: 0x203c);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x203c, out AnalysisItem? item)
                && item.DataTypeCandidate == "WideString");
            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(0x203c)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesPrototypeResourceStringRecord()
    {
        AnalysisSession session = CreatePrototypeResourceStringSession(
            0x2010,
            42,
            [new PeResourceString(42, 0x0409, "Prototype resource")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Prototype resource", session.Items[0x1005].ResourceStringCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveTruncatedOrAmbiguousPrototypeResourceStringRecord()
    {
        foreach ((uint recordRva, PeResourceString[] resources) in new[]
        {
            (
                0x203cu,
                new[] { new PeResourceString(42, 0x0409, "Prototype resource") }),
            (
                0x2010u,
                new[]
                {
                    new PeResourceString(42, 0x0409, "Prototype resource"),
                    new PeResourceString(42, 0x040c, "Autre ressource")
                })
        })
        {
            AnalysisSession session = CreatePrototypeResourceStringSession(recordRva, 42, resources);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsNull(session.Items[0x1005].ResourceStringCandidate);
        }
    }

    [TestMethod]
    public async Task AnalysisResolvesVarPrototypeResourceStringRecordThroughPointerSlot()
    {
        AnalysisSession session = CreateVarPrototypeResourceStringSession(
            0x2020,
            [new PeResourceString(42, 0x0409, "Var prototype resource")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Var prototype resource", session.Items[0x1005].ResourceStringCandidate);
        Assert.AreEqual("PResStringRec", session.Items[0x2010].DataTypeCandidate);
        Assert.AreEqual("PResStringRec", session.Items[0x2020].DataTypeCandidate);
        foreach (uint address in new[] { 0x2010u, 0x2020u })
        {
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(address)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotResolveInvalidVarPrototypeResourceStringRecord()
    {
        foreach ((uint recordRva, PeResourceString[] resources) in new[]
        {
            (
                0x203cu,
                new[] { new PeResourceString(42, 0x0409, "Var resource") }),
            (
                0x2020u,
                new[]
                {
                    new PeResourceString(42, 0x0409, "Resource A"),
                    new PeResourceString(42, 0x040c, "Resource B")
                }),
            (
                0x1020u,
                new[] { new PeResourceString(42, 0x0409, "Code target") })
        })
        {
            AnalysisSession session = CreateVarPrototypeResourceStringSession(recordRva, resources);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsNull(session.Items[0x1005].ResourceStringCandidate);
            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == "PResStringRec");
            Assert.IsFalse(session.Items.TryGetValue(recordRva, out AnalysisItem? record)
                && record.DataTypeCandidate == "PResStringRec");
        }
    }

    [TestMethod]
    public async Task AnalysisTypesVarVariantAndInterfaceOnlyForCompleteNonCodeTargets()
    {
        foreach (string typeName in new[]
        {
            "Variant",
            "IInterface",
            "IUnknown",
            "IDispatch",
            "Boolean",
            "ByteBool",
            "Byte",
            "SmallInt",
            "WordBool",
            "WideChar",
            "Integer",
            "LongWord",
            "NativeInt",
            "NativeUInt",
            "PtrInt",
            "PtrUInt",
            "THandle",
            "Single",
            "Int64",
            "Double",
            "Currency",
            "TDate",
            "TTime",
            "Comp",
            "Extended"
        })
        {
            AnalysisSession session = CreateVarFixedSizeTypePrototypeSession(typeName, 0x2024, 0x40);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            string expectedType = typeName switch
            {
                "LongWord" => "LongWord",
                "WideChar" => "WideChar",
                _ => typeName
            };
            Assert.AreEqual(expectedType, session.Items[0x2010].DataTypeCandidate);
            Assert.AreEqual(expectedType, session.Items[0x2024].DataTypeCandidate);
            Assert.IsTrue(session.Items[0x2024].Flags.HasFlag(AnalysisFlags.Data));
            Assert.IsFalse(session.Items[0x2024].Flags.HasFlag(AnalysisFlags.Code));
            foreach (uint address in new[] { 0x2010u, 0x2024u })
            {
                Assert.AreEqual(
                    1,
                    session.GetIncomingCrossReferences(address)
                        .Count(reference => reference.SourceAddress == 0x1005
                            && reference.Kind == CrossReferenceKind.Constant));
            }

        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTypeFixedSizeVarForCodeTruncatedOrUnknownTargets()
    {
        foreach ((string typeName, uint targetRva, uint dataRawSize) in new[]
        {
            ("Variant", 0x1020u, 0x40u),
            ("IInterface", 0x1020u, 0x40u),
            ("IUnknown", 0x1020u, 0x40u),
            ("IDispatch", 0x1020u, 0x40u),
            ("Variant", 0x2024u, 0x30u),
            ("Double", 0x203cu, 0x40u),
            ("Extended", 0x203cu, 0x40u),
            ("TDate", 0x203cu, 0x40u),
            ("Comp", 0x203cu, 0x40u),
            ("Integer", 0x203cu, 0x3fu),
            ("TUnknownType", 0x2024u, 0x40u)
        })
        {
            AnalysisSession session = CreateVarFixedSizeTypePrototypeSession(
                typeName,
                targetRva,
                dataRawSize);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? slot)
                && slot.DataTypeCandidate == typeName);
            Assert.IsFalse(session.Items.TryGetValue(targetRva, out AnalysisItem? target)
                && target.DataTypeCandidate == typeName);
        }
    }

    [TestMethod]
    public async Task AnalysisTypesVarSingleAtEndOfMappedDataButRejectsLargerValues()
    {
        AnalysisSession session = CreateVarFixedSizeTypePrototypeSession("Single", 0x203c, 0x40);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Single", session.Items[0x2010].DataTypeCandidate);
        Assert.AreEqual("Single", session.Items[0x203c].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisUsesDelphiVersionToSizeVarChar()
    {
        AnalysisSession legacyChar = CreateVarFixedSizeTypePrototypeSession(
            "Char",
            0x203f,
            0x40,
            DelphiVersion.Delphi2007);
        AnalysisSession unicodeCharTooShort = CreateVarFixedSizeTypePrototypeSession(
            "Char",
            0x203f,
            0x40,
            DelphiVersion.Delphi2009);
        AnalysisSession unicodeCharComplete = CreateVarFixedSizeTypePrototypeSession(
            "Char",
            0x203e,
            0x40,
            DelphiVersion.Delphi2009);

        foreach (AnalysisSession session in new[] { legacyChar, unicodeCharTooShort, unicodeCharComplete })
        {
            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);
        }

        Assert.AreEqual("Char", legacyChar.Items[0x203f].DataTypeCandidate);
        Assert.IsFalse(unicodeCharTooShort.Items.TryGetValue(0x203f, out AnalysisItem? tooShort)
            && tooShort.DataTypeCandidate == "Char");
        Assert.AreEqual("Char", unicodeCharComplete.Items[0x203e].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisUsesUniqueKnowledgeBasePrototypeToMarkShortStringArguments()
    {
        foreach (string typeName in new[] { "ShortString", "PShortString" })
        {
            byte[] shortString = [5, .. Encoding.Latin1.GetBytes("Short")];
            AnalysisSession session = CreatePrototypeStringSession(
                0,
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", typeName),
                shortString,
                terminateString: false);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            AnalysisItem item = session.Items[0x2010];
            Assert.IsTrue(item.Flags.HasFlag(AnalysisFlags.Data));
            Assert.IsTrue(item.Flags.HasFlag(AnalysisFlags.String));
            Assert.AreEqual("ShortString", item.DataTypeCandidate);
            Assert.AreEqual("Short", item.Name);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x2010)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkShortStringWhenLengthExceedsFileBackedData()
    {
        AnalysisSession session = CreatePrototypeStringSession(
            0,
            new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", "ShortString"),
            [8, .. Encoding.Latin1.GetBytes("text")],
            terminateString: false,
            stringRva: 0x203c);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x203c, out AnalysisItem? item)
            && item.DataTypeCandidate == "ShortString");
        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x203c)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisUsesUniqueKnowledgeBasePrototypeForManagedStringArguments()
    {
        foreach ((string argumentType, ushort codePage, ushort elementSize, DelphiTypeKind expectedKind, string expectedName) in new[]
        {
            ("AnsiString", (ushort)1252, (ushort)1, DelphiTypeKind.AnsiString, "AnsiString"),
            ("UnicodeString", (ushort)1200, (ushort)2, DelphiTypeKind.UnicodeString, "UnicodeString"),
            ("String", (ushort)1200, (ushort)2, DelphiTypeKind.UnicodeString, "UnicodeString"),
            ("UString", (ushort)1200, (ushort)2, DelphiTypeKind.UnicodeString, "UnicodeString")
        })
        {
            AnalysisSession session = CreateManagedStringPrototypeSession(
                argumentType,
                codePage,
                elementSize,
                DelphiVersion.Delphi2009);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            AnalysisItem stringItem = session.Items[0x200c];
            Assert.AreEqual(expectedKind, stringItem.TypeKind, argumentType);
            Assert.AreEqual(expectedName, stringItem.DataTypeCandidate, argumentType);
            Assert.IsTrue(stringItem.Flags.HasFlag(AnalysisFlags.String), argumentType);
            Assert.AreEqual(
                1,
                session.GetIncomingCrossReferences(0x200c)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant),
                argumentType);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotUseManagedStringPrototypeForMismatchedOrLegacyLiteral()
    {
        foreach (DelphiVersion version in new[] { DelphiVersion.Delphi7, DelphiVersion.Delphi2009 })
        {
            AnalysisSession session = CreateManagedStringPrototypeSession(
                version == DelphiVersion.Delphi7 ? "UnicodeString" : "UnicodeString",
                codePage: 1252,
                elementSize: 1,
                version);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items.TryGetValue(0x200c, out AnalysisItem? stringItem)
                && stringItem.DataTypeCandidate == "UnicodeString");
            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(0x200c)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotUseAmbiguousKnowledgeBasePrototypeForStringArguments()
    {
        AnalysisSession session = CreatePrototypeStringSession(
            0,
            new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", "PAnsiChar"),
            Encoding.Latin1.GetBytes("ANSI argument\0"),
            duplicatePrototype: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(
            0,
            session.GetIncomingCrossReferences(0x2010)
                .Count(reference => reference.SourceAddress == 0x1005
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkPrototypeStringWithoutUniquePrototypeOrTerminator()
    {
        foreach ((bool includePrototype, bool duplicatePrototype, bool terminateString, uint stringRva) in new[]
        {
            (false, false, true, 0x2010u),
            (true, false, false, 0x2010u),
            (true, false, false, 0x203fu)
        })
        {
            AnalysisSession session = CreatePrototypeStringSession(
                0,
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", "PAnsiChar"),
                Encoding.Latin1.GetBytes("bounded string"),
                duplicatePrototype,
                includePrototype,
                terminateString,
                stringRva);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(
                0,
                session.GetIncomingCrossReferences(stringRva)
                    .Count(reference => reference.SourceAddress == 0x1005
                        && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisTypesDirectReferencesToKnownManagedStringLiterals()
    {
        foreach ((byte[] instruction, DelphiTypeKind typeKind, string expectedType) in new[]
        {
            (new byte[] { 0xb8, 0x24, 0x20, 0x40, 0x00 }, DelphiTypeKind.AnsiString, "AnsiString"),
            (new byte[] { 0xb8, 0x24, 0x20, 0x40, 0x00 }, DelphiTypeKind.UnicodeString, "UnicodeString"),
            (new byte[] { 0x8d, 0x05, 0x24, 0x20, 0x40, 0x00 }, DelphiTypeKind.AnsiString, "AnsiString"),
            (new byte[] { 0x8d, 0x05, 0x24, 0x20, 0x40, 0x00 }, DelphiTypeKind.UnicodeString, "UnicodeString")
        })
        {
            AnalysisSession session = CreateClassFieldAccessSession(
            [.. instruction, 0xc3],
                useParentField: false);
            AnalysisItem stringItem = session.GetOrAddItem(0x2024);
            stringItem.SetFlags(AnalysisFlags.String);
            stringItem.TypeKind = typeKind;

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(
                expectedType,
                session.Items[0x1000].DataTypeCandidate,
                Convert.ToHexString(instruction));
            Assert.AreEqual(
                CrossReferenceKind.Constant,
                session.GetIncomingCrossReferences(0x2024).Single().Kind,
                Convert.ToHexString(instruction));
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotGuessTypeForUntypedStringReferences()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x24, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);
        session.GetOrAddItem(0x2024).SetFlags(AnalysisFlags.String);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesKnownManagedStringTypeToGlobalPointerStore()
    {
        foreach ((DelphiTypeKind typeKind, string expectedType) in new[]
        {
            (DelphiTypeKind.AnsiString, "AnsiString"),
            (DelphiTypeKind.UnicodeString, "UnicodeString")
        })
        {
            AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x24, 0x20, 0x40, 0x00,
                0xa3, 0x30, 0x20, 0x40, 0x00,
                0xc3
            ],
                useParentField: false);
            AnalysisItem stringItem = session.GetOrAddItem(0x2024);
            stringItem.SetFlags(AnalysisFlags.String);
            stringItem.TypeKind = typeKind;

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(expectedType, session.Items[0x2030].DataTypeCandidate);
            Assert.AreEqual(
                CrossReferenceKind.Constant,
                session.GetIncomingCrossReferences(0x2030).Single().Kind);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTypeGlobalPointerStoreFromGenericString()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x24, 0x20, 0x40, 0x00,
            0xa3, 0x30, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);
        session.GetOrAddItem(0x2024).SetFlags(AnalysisFlags.String);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
            session.Items.TryGetValue(0x2030, out AnalysisItem? destinationItem)
                && destinationItem?.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisPropagatesManagedStringTypeToFrameLocalPointerStore()
    {
        foreach ((DelphiTypeKind typeKind, string expectedType) in new[]
        {
            (DelphiTypeKind.AnsiString, "AnsiString"),
            (DelphiTypeKind.UnicodeString, "UnicodeString")
        })
        {
            AnalysisSession session = CreateClassFieldAccessSession(
            [
                0x55,
                0x8b, 0xec,
                0xb8, 0x24, 0x20, 0x40, 0x00,
                0x89, 0x45, 0xfc,
                0xc9,
                0xc3
            ],
                useParentField: false);
            AnalysisItem stringItem = session.GetOrAddItem(0x2024);
            stringItem.SetFlags(AnalysisFlags.String);
            stringItem.TypeKind = typeKind;

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { new StackLocalVariable(-4, 4, expectedType) },
                session.Items[0x1000].StackLocalVariables.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTypeFrameLocalPointerStoreFromGenericString()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x24, 0x20, 0x40, 0x00,
            0x89, 0x45, 0xfc,
            0xc9,
            0xc3
        ],
            useParentField: false);
        session.GetOrAddItem(0x2024).SetFlags(AnalysisFlags.String);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].StackLocalVariables.Count);
    }

    [TestMethod]
    public async Task AnalysisPropagatesManagedStringTypeThroughTrackedGlobalBase()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
                0xbb, 0x2c, 0x20, 0x40, 0x00,
                0xb8, 0x24, 0x20, 0x40, 0x00,
                0x89, 0x43, 0x04,
                0xc3
        ],
                useParentField: false);
        AnalysisItem stringItem = session.GetOrAddItem(0x2024);
        stringItem.SetFlags(AnalysisFlags.String);
        stringItem.TypeKind = DelphiTypeKind.UnicodeString;

        await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("UnicodeString", session.Items[0x2030].DataTypeCandidate);
        Assert.AreEqual(
                CrossReferenceKind.Constant,
                session.GetIncomingCrossReferences(0x2030).Single().Kind);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateManagedStringTypeThroughIndexedStore()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
                0xbb, 0x2c, 0x20, 0x40, 0x00,
                0xb8, 0x24, 0x20, 0x40, 0x00,
                0x89, 0x44, 0x8b, 0x04,
                0xc3
        ],
                useParentField: false);
        AnalysisItem stringItem = session.GetOrAddItem(0x2024);
        stringItem.SetFlags(AnalysisFlags.String);
        stringItem.TypeKind = DelphiTypeKind.UnicodeString;

        await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(
                session.Items.TryGetValue(0x2030, out AnalysisItem? destinationItem)
                    && destinationItem?.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisRecordsConstantCrossReferencesForAbsoluteMemoryOperands()
    {
        byte[][] instructions =
        [
            [0xa1, 0x24, 0x20, 0x40, 0x00],
            [0x8d, 0x05, 0x24, 0x20, 0x40, 0x00]
        ];

        foreach (byte[] instruction in instructions)
        {
            AnalysisSession session = CreateClassFieldAccessSession(
                [.. instruction, 0xc3],
                useParentField: false);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CrossReference[] references = session.GetIncomingCrossReferences(0x2024).ToArray();
            Assert.AreEqual(1, references.Length, Convert.ToHexString(instruction));
            CrossReference reference = references[0];
            Assert.AreEqual(0x1000u, reference.SourceAddress);
            Assert.AreEqual(CrossReferenceKind.Constant, reference.Kind);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotTreatSegmentRelativeMemoryAsAbsoluteImageAddress()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x64, 0xa1, 0x24, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x2024).Count);
    }

    [TestMethod]
    public async Task AnalysisRecordsAbsoluteDataCrossReferenceToZeroInitializedSection()
    {
        byte[] image = new byte[0x20];
        byte[] code = [0xb8, 0x04, 0x20, 0x40, 0x00, 0xc3];
        code.CopyTo(image, 0);
        AnalysisSession session = new(
            "bss-reference.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".bss", 0x2000, 0x20, 0x20, 0, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CrossReference reference = session.GetIncomingCrossReferences(0x2004).Single();
        Assert.AreEqual(0x1000u, reference.SourceAddress);
        Assert.AreEqual(CrossReferenceKind.Constant, reference.Kind);
    }

    [TestMethod]
    public async Task AnalysisRecordsDataCrossReferenceThroughTrackedBaseRegister()
    {
        DecodedOperand operand = new IcedInstructionDecoder()
            .Decode([0x8b, 0x10], 0x1005, 32)
            .Operands[1];
        Assert.AreEqual("EAX", operand.BaseRegister);
        Assert.AreEqual(0UL, operand.Displacement);
        Assert.AreEqual("DS", operand.SegmentRegister);
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x24, 0x20, 0x40, 0x00,
            0x8b, 0x10,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(
            session.GetIncomingCrossReferences(0x2024).Any(reference =>
                reference.SourceAddress == 0x1005u
                && reference.Kind == CrossReferenceKind.Constant),
            string.Join(
                "; ",
                session.Items.Values.SelectMany(item => item.CrossReferences)
                    .Select(reference => $"{reference.SourceAddress:X8}->{reference.TargetAddress:X8}")));
    }

    [TestMethod]
    public async Task AnalysisDoesNotCrossReferenceUnknownBaseRegisterAddress()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x8b, 0x10,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.GetIncomingCrossReferences(0x2024).Count);
    }

    [TestMethod]
    public async Task AnalysisIncludesPositiveDisplacementForTrackedBaseRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x50, 0x04,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.GetIncomingCrossReferences(0x2024).Any(reference =>
            reference.SourceAddress == 0x1005u
            && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferDataAddressForIndexedMemoryOperand()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x14, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.GetIncomingCrossReferences(0x2020).Any(reference =>
            reference.SourceAddress == 0x1005u));
    }

    [TestMethod]
    public async Task AnalysisIgnoresAbsoluteImmediateOutsideMappedImageData()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x50, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].CrossReferences.Count);
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
    public async Task AnalysisMarksNestedProcedureCallingClassCreateAsConstructorCandidate()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xe8;
        code[0x11] = 0x03;
        code[0x15] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1018).Name = "@ClassCreate";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.ConstructorCandidate));
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
    public async Task AnalysisMarksNestedProcedureCallingClassDestroyAsDestructorCandidate()
    {
        byte[] code = new byte[0x20];
        code[0] = 0xe8;
        code[1] = 0x0b;
        code[5] = 0xc3;
        code[0x10] = 0xe8;
        code[0x11] = 0x03;
        code[0x15] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1018).Name = "@ClassDestroy";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1010].Flags.HasFlag(AnalysisFlags.DestructorCandidate));
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
    public async Task AnalysisRecognizesImmediateEaxBasedThreadVariableAccessAfterGetTls()
    {
        byte[] code = new byte[0x20];
        byte[] instructions =
        [
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x8b, 0x48, 0x24,
            0xc3
        ];
        instructions.CopyTo(code, 0);
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);
        session.GetOrAddItem(0x1018).Name = "@GetTls";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("threadvar_36", session.Items[0x1005].ThreadVariableCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferThreadVariableWithoutImmediateEaxBasedAccess()
    {
        foreach (byte[] followingInstructions in new[]
        {
            new byte[] { 0x90, 0x8b, 0x48, 0x24, 0xc3 },
            new byte[] { 0x8b, 0x4b, 0x24, 0xc3 }
        })
        {
            byte[] code = new byte[0x20];
            byte[] prefix = [0xe8, 0x13, 0x00, 0x00, 0x00];
            prefix.CopyTo(code, 0);
            followingInstructions.CopyTo(code, prefix.Length);
            code[0x18] = 0xc3;
            AnalysisSession session = CreateSession(code);
            session.GetOrAddItem(0x1018).Name = "@GetTls";

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsTrue(session.Items.Values.All(item => item.ThreadVariableCandidate is null));
        }
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
    public async Task AnalysisInfersReturnTypeFromKnownEaxGlobalDataType()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xa1, 0x20, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);
        AnalysisItem global = session.GetOrAddItem(0x2020);
        global.DataTypeCandidate = "TChild";
        global.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferClassReturnTypeForConstructorCandidate()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xa1, 0x20, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);
        AnalysisItem global = session.GetOrAddItem(0x2020);
        global.DataTypeCandidate = "TChild";
        global.SetFlags(AnalysisFlags.Data);
        session.GetOrAddItem(0x1000).SetFlags(AnalysisFlags.ConstructorCandidate);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].ReturnTypeCandidate);
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
    public async Task AnalysisTypesTracksGlobalAddressAfterRegisterImmediateAddition()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x83, 0xc0, 0x04,
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "runtime-type-register-add.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@IntfClear";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("IInterface", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksGlobalAddressAfterSignedImmediateSubtraction()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x83, 0xe8, 0xfc,
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "runtime-type-register-sub.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@IntfClear";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("IInterface", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksGlobalAddressThroughLeaWithPositiveDisplacement()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x8d, 0x40, 0x04,
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "runtime-type-register-lea-positive.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@IntfClear";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("IInterface", session.Items[0x2004].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksGlobalAddressThroughLeaWithSignedNegativeDisplacement()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8d, 0x40, 0xfc,
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "runtime-type-register-lea-negative.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                new PeSection(".data", 0x2000, 0x20, 0x20, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@IntfClear";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("IInterface", session.Items[0x201c].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksPointerLoadedFromAbsoluteGlobal()
    {
        AnalysisSession session = CreatePointerLoadedGlobalHelperSession(
            [0xa1, 0x00, 0x20, 0x40, 0x00]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Variant", session.Items[0x2010].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksPointerLoadedThroughTrackedBaseRegister()
    {
        AnalysisSession session = CreatePointerLoadedGlobalHelperSession(
            [
                0xbb, 0x00, 0x20, 0x40, 0x00,
                0x8b, 0x03
            ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Variant", session.Items[0x2010].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesTracksPointerLoadedThroughNegativeBaseDisplacement()
    {
        AnalysisSession session = CreatePointerLoadedGlobalHelperSession(
            [
                0xbb, 0x04, 0x20, 0x40, 0x00,
                0x8b, 0x43, 0xfc
            ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Variant", session.Items[0x2010].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesDoesNotTrackPointerLoadedFromCodeBytes()
    {
        AnalysisSession session = CreatePointerLoadedGlobalHelperSession(
            [0xa1, 0x30, 0x10, 0x40, 0x00],
            storePointerInCode: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? variable)
            && variable.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisTypesDoesNotReadPointerAcrossDataSectionBoundary()
    {
        AnalysisSession session = CreatePointerLoadedGlobalHelperSession(
            [0xa1, 0x1e, 0x20, 0x40, 0x00],
            dataRawSize: 0x20,
            pointerRawOffset: 0x5e);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x2010, out AnalysisItem? variable)
            && variable.DataTypeCandidate is not null);
    }

    [TestMethod]
    public async Task AnalysisTypesGlobalFormVariableFromCreateFormCall()
    {
        byte[] image = new byte[0xc0];
        byte[] code =
        [
            0x8d, 0x15, 0x20, 0x20, 0x40, 0x00,
            0xb9, 0x34, 0x20, 0x40, 0x00,
            0xe8, 0x10, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "create-form.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x1fc0, 0x80, 0x40, 0x80, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem vmt = session.GetOrAddItem(0x2020);
        vmt.Name = "TMainForm";
        vmt.SetFlags(AnalysisFlags.Vmt);
        session.GetOrAddItem(0x1020).Name = "TApplication.CreateForm";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TMainForm", session.Items[0x2034].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesLocalFormVariableFromCreateFormCall()
    {
        byte[] image = new byte[0xc0];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8d, 0x15, 0x20, 0x20, 0x40, 0x00,
            0x8d, 0x4d, 0xfc,
            0xe8, 0x0f, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "create-form-local.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x1fc0, 0x80, 0x40, 0x80, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem vmt = session.GetOrAddItem(0x2020);
        vmt.Name = "TMainForm";
        vmt.SetFlags(AnalysisFlags.Vmt);
        session.GetOrAddItem(0x1020).Name = "TApplication.CreateForm";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-4, 4, "TMainForm") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesFormStackArgumentFromCreateFormCall()
    {
        byte[] image = new byte[0xc0];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8d, 0x15, 0x20, 0x20, 0x40, 0x00,
            0x8d, 0x4d, 0x08,
            0xe8, 0x0f, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "create-form-stack-argument.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x1fc0, 0x80, 0x40, 0x80, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem vmt = session.GetOrAddItem(0x2020);
        vmt.Name = "TMainForm";
        vmt.SetFlags(AnalysisFlags.Vmt);
        session.GetOrAddItem(0x1020).Name = "TApplication.CreateForm";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        StackArgument argument = session.Items[0x1000].StackArguments.Single();
        Assert.AreEqual(8u, argument.Offset);
        Assert.AreEqual("TMainForm", argument.TypeName);
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
    public async Task AnalysisAnnotatesCallsWithUniqueResourceStrings()
    {
        foreach (string helperName in new[] { "@LoadStr", "FmtLoadStr", "@LoadResString" })
        {
            AnalysisSession session = CreateResourceStringHelperSession(
                helperName,
                [new PeResourceString(1, 0x0409, "Hello")]);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual("Hello", session.Items[0x1005].ResourceStringCandidate);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotChooseBetweenConflictingResourceStringLanguages()
    {
        AnalysisSession session = CreateResourceStringHelperSession(
            "@LoadStr",
            [
                new PeResourceString(1, 0x0409, "Hello"),
                new PeResourceString(1, 0x040c, "Bonjour")
            ]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1005].ResourceStringCandidate);
    }

    [TestMethod]
    public async Task AnalysisTypesUninitializedGlobalRecordFinalizedWithKnownRtti()
    {
        AnalysisSession session = CreateRuntimeVariableTypeSession(
            "@FinalizeRecord",
            0x402004,
            typeInfoAddress: 0x402008,
            zeroFillDataSection: true);
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TRecord";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TRecord", session.Items[0x2004].DataTypeCandidate);
        Assert.IsTrue(session.Items[0x2004].Flags.HasFlag(AnalysisFlags.Data));
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
    public async Task AnalysisRestoresLocalAddressProvenanceThroughRegisterPushAndPop()
    {
        AnalysisSession session = CreateLocalStringArrayCleanupSession(
            "@LStrArrayClr",
            1,
            preserveLocalAddressThroughStack: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "AnsiString") },
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
    public async Task AnalysisTypesLocalVariableClearedByRuntimeHelpers()
    {
        foreach ((string helperName, string expectedType) in new[]
        {
            ("@IntfClear", "IInterface"),
            ("@VarClr", "Variant")
        })
        {
            AnalysisSession session = CreateLocalRuntimeClearSession(helperName);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            CollectionAssert.AreEqual(
                new[] { new StackLocalVariable(-8, 4, expectedType) },
                session.Items[0x1000].StackLocalVariables.ToArray());
        }
    }

    [TestMethod]
    public async Task AnalysisTypesStackArgumentClearedByRuntimeHelpers()
    {
        foreach ((string helperName, string expectedType) in new[]
        {
            ("@IntfClear", "IInterface"),
            ("@VarClr", "Variant")
        })
        {
            AnalysisSession session = CreateStackRuntimeClearSession(helperName);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            StackArgument argument = session.Items[0x1000].StackArguments.Single();
            Assert.AreEqual(8u, argument.Offset);
            Assert.AreEqual(expectedType, argument.TypeName);
        }
    }

    [TestMethod]
    public async Task AnalysisTypesLocalFixedArrayFinalizedWithKnownRttiAndCount()
    {
        byte[] image = new byte[0x50];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8d, 0x45, 0xf8,
            0xba, 0x08, 0x20, 0x40, 0x00,
            0xb9, 0x01, 0x01, 0x00, 0x00,
            0xe8, 0x0b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "local-fixed-array-finalize.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x28, 0, 0x28, true),
                new PeSection(".data", 0x2000, 0x20, 0x28, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@FinalizeArray";
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-8, 4, "array[257] of TItem") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisTypesStackArgumentFinalizedWithKnownRtti()
    {
        byte[] image = new byte[0x60];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0xba, 0x08, 0x20, 0x40, 0x00,
            0xe8, 0x20, 0x00, 0x00, 0x00,
            0x8b, 0x45, 0x08,
            0x8b, 0x48, 0x04,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "stack-record-finalize.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1030).Name = "@FinalizeRecord";
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TRecord";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);
        typeInfo.RecordSizeBytes = 8;
        typeInfo.RecordFields = [new DelphiRttiRecordField("FValue", 4, 0x2010)];
        AnalysisItem integerType = session.GetOrAddItem(0x2010);
        integerType.Name = "Integer";
        integerType.TypeKind = DelphiTypeKind.Integer;

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        StackArgument argument = session.Items[0x1000].StackArguments.Single();
        Assert.AreEqual(8u, argument.Offset);
        Assert.AreEqual("TRecord", argument.TypeName);
        MemberAccessCandidate access = session.Items[0x1013].MemberAccessCandidates.Single();
        Assert.AreEqual("TRecord", access.OwnerTypeName);
        Assert.AreEqual("FValue", access.FieldName);
        Assert.AreEqual("Integer", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisTypesStackArrayArgumentFinalizedWithKnownRttiAndCount()
    {
        byte[] image = new byte[0x60];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0xba, 0x08, 0x20, 0x40, 0x00,
            0xb9, 0x01, 0x01, 0x00, 0x00,
            0xe8, 0x1b, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "stack-fixed-array-finalize.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1030).Name = "@FinalizeArray";
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TItem";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        StackArgument argument = session.Items[0x1000].StackArguments.Single();
        Assert.AreEqual(8u, argument.Offset);
        Assert.AreEqual("array[257] of TItem", argument.TypeName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotTypeStackArgumentAfterItsRegisterIsClobbered()
    {
        byte[] image = new byte[0x60];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0x31, 0xc0,
            0xba, 0x08, 0x20, 0x40, 0x00,
            0xe8, 0x06, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "stack-record-finalize-clobbered.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@FinalizeRecord";
        AnalysisItem typeInfo = session.GetOrAddItem(0x2008);
        typeInfo.Name = "TRecord";
        typeInfo.TypeKind = DelphiTypeKind.Record;
        typeInfo.SetFlags(AnalysisFlags.Rtti);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        StackArgument argument = session.Items[0x1000].StackArguments.Single();
        Assert.AreEqual(8u, argument.Offset);
        Assert.IsNull(argument.TypeName);
    }

    [TestMethod]
    public async Task AnalysisMarksStackArgumentPassedToDynamicArrayAddRef()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0xe8, 0x0d, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "stack-dynarray-addref.exe",
            image,
            [new PeSection(".text", 0x1000, 0x20, 0, 0x20, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@DynArrayAddRef";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("array of ?", session.Items[0x1000].StackArguments.Single().TypeName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesLocalRecordTypeThroughFrameRelativeLea()
    {
        byte[] image = new byte[0x48];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8d, 0x45, 0xf8,
            0xba, 0x08, 0x20, 0x40, 0x00,
            0xe8, 0x08, 0x00, 0x00, 0x00,
            0x8d, 0x4d, 0xf8,
            0x8b, 0x41, 0x04, 0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "local-record-field-access.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x28, 0, 0x28, true),
                new PeSection(".data", 0x2000, 0x20, 0x28, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@FinalizeRecord";
        AnalysisItem recordType = session.GetOrAddItem(0x2008);
        recordType.Name = "TRecord";
        recordType.TypeKind = DelphiTypeKind.Record;
        recordType.SetFlags(AnalysisFlags.Rtti);
        recordType.RecordSizeBytes = 8;
        recordType.RecordFields = [new DelphiRttiRecordField("FValue", 4, 0x2010)];
        AnalysisItem integerType = session.GetOrAddItem(0x2010);
        integerType.Name = "Integer";
        integerType.TypeKind = DelphiTypeKind.Integer;

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1013].MemberAccessCandidates.Single();
        Assert.AreEqual("TRecord", access.OwnerTypeName);
        Assert.AreEqual("FValue", access.FieldName);
        Assert.AreEqual("Integer", access.TypeName);
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
    public async Task AnalysisPreservesLocalAddressAcrossConditionalBranchToHelperCall()
    {
        byte[] image = new byte[0x60];
        image[0] = 0x55;
        image[1] = 0x8b;
        image[2] = 0xec;
        image[3] = 0x8d;
        image[4] = 0x45;
        image[5] = 0xf8;
        image[6] = 0x74;
        image[7] = 0x00;
        image[8] = 0xe8;
        image[9] = 0x23;
        image[13] = 0xc3;
        image[0x30] = 0xc3;

        AnalysisSession session = new(
            "local-address-branch.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1030).Name = "@DynArrayAddRef";

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
    public async Task AnalysisResolvesDynamicMethodIdTransferredAcrossForwardBranch()
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
        image[11] = 0xbb;
        image[12] = 0x11;
        image[13] = 0x11;
        image[14] = 0x74;
        image[15] = 0x01;
        image[16] = 0xc3;
        image[17] = 0xe8;
        image[18] = 0x12;
        image[22] = 0xc3;
        image[0x20] = 0xc3;
        image[0x28] = 0xc3;
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "dynamic-method-branch-context.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x30, 0x40, 0x30, false)
            ],
            0x400000,
            DelphiVersion.Delphi5)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@AsClass";
        session.GetOrAddItem(0x1028).Name = "@CallDynaInst";
        session.GetOrAddItem(0x1011).SetFlags(AnalysisFlags.ProcedureStart);
        AnalysisItem childVmt = session.GetOrAddItem(0x2000);
        childVmt.Name = "TChild";
        childVmt.ParentAddress = 0x2010;
        childVmt.SetFlags(AnalysisFlags.Vmt);
        AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
        parentVmt.Name = "TBase";
        parentVmt.SetFlags(AnalysisFlags.Vmt);
        parentVmt.DynamicMethods = [new DelphiVmtDynamicMethod(0x1111, 0x1030)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1011].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
    }

    [TestMethod]
    public async Task AnalysisRestoresDynamicMethodIdFromRegisterStackAtBranchTarget()
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
        image[11] = 0xbb;
        image[12] = 0x11;
        image[13] = 0x11;
        image[14] = 0x53;
        image[15] = 0x31;
        image[16] = 0xdb;
        image[17] = 0x74;
        image[18] = 0x02;
        image[19] = 0xc3;
        image[20] = 0x90;
        image[21] = 0x5b;
        image[22] = 0xe8;
        image[23] = 0x0d;
        image[27] = 0xc3;
        image[0x20] = 0xc3;
        image[0x28] = 0xc3;
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "dynamic-method-stack-branch-context.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x30, 0x40, 0x30, false)
            ],
            0x400000,
            DelphiVersion.Delphi5)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@AsClass";
        session.GetOrAddItem(0x1028).Name = "@CallDynaInst";
        session.GetOrAddItem(0x1015).SetFlags(AnalysisFlags.ProcedureStart);
        AnalysisItem childVmt = session.GetOrAddItem(0x2000);
        childVmt.Name = "TChild";
        childVmt.ParentAddress = 0x2010;
        childVmt.SetFlags(AnalysisFlags.Vmt);
        AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
        parentVmt.Name = "TBase";
        parentVmt.SetFlags(AnalysisFlags.Vmt);
        parentVmt.DynamicMethods = [new DelphiVmtDynamicMethod(0x1111, 0x1030)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1016].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
    }

    [TestMethod]
    public async Task AnalysisTransfersDynamicMethodIdBetweenRegistersBeforeBranch()
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
        image[11] = 0xbb;
        image[12] = 0x11;
        image[13] = 0x11;
        image[14] = 0x66;
        image[15] = 0x8b;
        image[16] = 0xf3;
        image[17] = 0x74;
        image[18] = 0x01;
        image[19] = 0xc3;
        image[20] = 0xe8;
        image[21] = 0x0f;
        image[25] = 0xc3;
        image[0x20] = 0xc3;
        image[0x28] = 0xc3;
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "dynamic-method-register-move-branch.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x30, 0x40, 0x30, false)
            ],
            0x400000,
            DelphiVersion.Delphi2009)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@AsClass";
        session.GetOrAddItem(0x1028).Name = "@CallDynaInst";
        session.GetOrAddItem(0x1014).SetFlags(AnalysisFlags.ProcedureStart);
        AnalysisItem childVmt = session.GetOrAddItem(0x2000);
        childVmt.Name = "TChild";
        childVmt.ParentAddress = 0x2010;
        childVmt.SetFlags(AnalysisFlags.Vmt);
        AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
        parentVmt.Name = "TBase";
        parentVmt.SetFlags(AnalysisFlags.Vmt);
        parentVmt.DynamicMethods = [new DelphiVmtDynamicMethod(0x1111, 0x1030)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1014].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
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
    public async Task AnalysisTypesFormVariableFromResolvedVirtualCreateFormCall()
    {
        AnalysisSession session = CreateVirtualMethodSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xba, 0x10, 0x20, 0x40, 0x00,
                0xb9, 0x18, 0x20, 0x40, 0x00,
                0xff, 0x50, 0x04,
                0xc3
            ]);
        AnalysisItem applicationVmt = session.Items[0x2000];
        applicationVmt.Name = "TApplication";
        applicationVmt.VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1030)];
        AnalysisItem formVmt = session.GetOrAddItem(0x2010);
        formVmt.Name = "TForm";
        formVmt.SetFlags(AnalysisFlags.Vmt);
        session.GetOrAddItem(0x1030).Name = "TApplication.CreateForm";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x100f].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1030));
        Assert.AreEqual("TForm", session.Items[0x2018].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPreservesKnownReturnTypeFromResolvedIndirectCall()
    {
        AnalysisSession session = CreateVirtualMethodSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xff, 0x50, 0x04,
                0xff, 0x50, 0x08,
                0xc3
            ]);
        AnalysisItem classVmt = session.Items[0x2000];
        classVmt.Name = "TChild";
        classVmt.VirtualMethods =
        [
            new DelphiVmtVirtualMethod(4, 0x1030),
            new DelphiVmtVirtualMethod(8, 0x1034)
        ];
        session.GetOrAddItem(0x1030).Name = "@ClassCreate";
        session.GetOrAddItem(0x1034).Name = "TChild.GetValue";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1008].CrossReferences.Any(reference =>
            reference.Kind == CrossReferenceKind.Call && reference.TargetAddress == 0x1034));
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
    public async Task AnalysisReportsUnknownInRangeClassFieldAsNonPersistentCandidate()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x4a, 0x0c,
                0xc3
            ],
            useParentField: false);
        AnalysisItem classVmt = session.Items[0x2000];
        classVmt.Fields = [];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual(12, access.Offset);
        Assert.AreEqual("f12", access.FieldName);
        Assert.IsNull(access.TypeInfoAddress);
        Assert.IsNull(access.TypeName);
        Assert.IsTrue(access.IsInferred);
        Assert.AreEqual(0, classVmt.Fields.Count);
    }

    [TestMethod]
    public async Task AnalysisCarriesKnownStoredTypeOnUnknownClassFieldCandidate()
    {
        foreach (byte[] storeInstruction in new[]
        {
            new byte[] { 0x89, 0x4a, 0x0c },
            new byte[] { 0x87, 0x4a, 0x0c }
        })
        {
            byte[] code =
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0xb9, 0x20, 0x20, 0x40, 0x00,
                .. storeInstruction,
                0xc3
            ];
            AnalysisSession session = CreateClassFieldAccessSession(
                code,
                useParentField: false);
            session.Items[0x2000].Fields = [];
            AnalysisItem sourceVmt = session.GetOrAddItem(0x2020);
            sourceVmt.Name = "TInner";
            sourceVmt.SetFlags(AnalysisFlags.Vmt);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            uint accessAddress = (uint)(0x1000 + 5 + 5 + 2 + 5);
            MemberAccessCandidate access = session.Items[accessAddress]
                .MemberAccessCandidates.Single();
            Assert.AreEqual("TChild", access.OwnerTypeName);
            Assert.AreEqual(12, access.Offset);
            Assert.AreEqual("f12", access.FieldName);
            Assert.IsNull(access.TypeInfoAddress);
            Assert.AreEqual("TInner", access.TypeName);
            Assert.IsTrue(access.IsInferred);
            Assert.AreEqual(0, session.Items[0x2000].Fields.Count);
        }
    }

    [TestMethod]
    public async Task AnalysisInfersX87PrecisionForUnknownClassFieldAccess()
    {
        (byte[] Instruction, string ExpectedType)[] cases =
        [
            ([0xd9, 0x40, 0x08], "Single"),
            ([0xdd, 0x40, 0x08], "Double"),
            ([0xdb, 0x68, 0x08], "Extended")
        ];

        foreach ((byte[] instruction, string expectedType) in cases)
        {
            AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                .. instruction,
                0xc3
            ],
                useParentField: false);
            session.Items[0x2000].Fields = [];

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            MemberAccessCandidate access = session.Items[0x1005].MemberAccessCandidates.Single();
            Assert.AreEqual("TChild", access.OwnerTypeName);
            Assert.AreEqual("f8", access.FieldName);
            Assert.AreEqual(expectedType, access.TypeName);
            Assert.IsTrue(access.IsInferred);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferUnknownClassFieldForIndexedOrOutOfRangeAccess()
    {
        byte[][] codeSamples =
        [
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x4c, 0x02, 0x0c,
                0xc3
            ],
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x4a, 0x14,
                0xc3
            ],
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x4a, 0x12,
                0xc3
            ]
        ];

        foreach (byte[] code in codeSamples)
        {
            AnalysisSession session = CreateClassFieldAccessSession(
                code,
                useParentField: false);
            session.Items[0x2000].Fields = [];

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(0, session.Items[0x100c].MemberAccessCandidates.Count);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferUnknownClassFieldForExplicitSegmentPrefix()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x3e, 0x8b, 0x4a, 0x0c,
                0xc3
            ],
            useParentField: false);
        session.Items[0x2000].Fields = [];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x100c].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferUnknownClassFieldWithoutKnownInstanceSize()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x8b, 0x4a, 0x0c,
                0xc3
            ],
            useParentField: false);
        session.Items[0x2000].Fields = [];
        session.Items[0x2000].ClassInstanceSizeBytes = null;

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x100c].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisResolvesClassFieldWhenAddressFormedByAddImmediate()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x83, 0xc0, 0x08,
                0xc3
            ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual(8, access.Offset);
        Assert.AreEqual("FCount", access.FieldName);
        Assert.AreEqual("Integer", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesNestedClassTypeWhenAddressFormedByAddImmediate()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x89, 0xc2,
                0x83, 0xc0, 0x08,
                0x8b, 0x48, 0x0c,
                0xc3
            ],
            useParentField: false);
        AnalysisItem fieldTypeInfo = session.Items[0x2018];
        fieldTypeInfo.TypeKind = DelphiTypeKind.Class;
        fieldTypeInfo.ClassVmtAddress = 0x2020;
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);
        nestedVmt.Fields = [new DelphiVmtField("FValue", 12, 0x2030)];
        session.GetOrAddItem(0x2030).Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate addressAccess = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("FCount", addressAccess.FieldName);
        MemberAccessCandidate nestedAccess = session.Items[0x100f].MemberAccessCandidates.Single();
        Assert.AreEqual("TInner", nestedAccess.OwnerTypeName);
        Assert.AreEqual("FValue", nestedAccess.FieldName);
    }

    [TestMethod]
    public async Task AnalysisInfersUnknownClassFieldTypeFromStoredClassRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0x89, 0xc2,
                0xb9, 0x20, 0x20, 0x40, 0x00,
                0x89, 0x4a, 0x08,
                0xc3
            ],
            useParentField: false);
        session.Items[0x2000].Fields = [new DelphiVmtField("FChild", 8, null)];
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FChild", access.FieldName);
        Assert.AreEqual("TInner", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisInfersUnknownClassFieldTypeFromExchangedClassRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0x89, 0xc2,
                0xb9, 0x20, 0x20, 0x40, 0x00,
                0x87, 0x4a, 0x08,
                0xc3
            ],
            useParentField: false);
        session.Items[0x2000].Fields = [new DelphiVmtField("FChild", 8, null)];
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FChild", access.FieldName);
        Assert.AreEqual("TInner", access.TypeName);
    }

    [TestMethod]
    public async Task AnalysisLoadsClassFieldTypeIntoRegisterAfterExchange()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0x89, 0xc2,
                0xb9, 0x30, 0x20, 0x40, 0x00,
                0x87, 0x4a, 0x08,
                0x8b, 0x41, 0x0c,
                0xc3
            ],
            useParentField: false);
        session.Items[0x2000].Fields = [new DelphiVmtField("FChild", 8, 0x2018)];
        AnalysisItem nestedTypeInfo = session.GetOrAddItem(0x2018);
        nestedTypeInfo.Name = "TInner";
        nestedTypeInfo.TypeKind = DelphiTypeKind.Class;
        nestedTypeInfo.ClassVmtAddress = 0x2020;
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);
        nestedVmt.ClassInstanceSizeBytes = 20;
        nestedVmt.Fields = [new DelphiVmtField("FValue", 12, 0x2038)];
        session.GetOrAddItem(0x2030).Name = "TSource";
        session.GetOrAddItem(0x2030).SetFlags(AnalysisFlags.Vmt);
        session.GetOrAddItem(0x2038).Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate exchangedField = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TInner", exchangedField.TypeName);
        MemberAccessCandidate nestedField = session.Items[0x100f].MemberAccessCandidates.Single();
        Assert.AreEqual("TInner", nestedField.OwnerTypeName);
        Assert.AreEqual("FValue", nestedField.FieldName);
    }

    [TestMethod]
    public async Task AnalysisInfersVirtualMethodOwnerForImplicitSelfRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0x8b, 0x48, 0x08, 0xc3],
            useParentField: false);
        session.Items[0x2000].VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1000)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1000].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisInfersDynamicMethodOwnerForImplicitSelfRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0x8b, 0x48, 0x08, 0xc3],
            useParentField: false);
        session.Items[0x2000].DynamicMethods = [new DelphiVmtDynamicMethod(1, 0x1000)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1000].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisInfersCommonAncestorAsInheritedMethodOwner()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0x8b, 0x48, 0x08, 0xc3],
            useParentField: true);
        session.Items[0x2000].VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1000)];
        session.Items[0x2010].VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1000)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1000].MemberAccessCandidates.Single();
        Assert.AreEqual("TBase", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferOwnerForMethodSharedByUnrelatedClasses()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0x8b, 0x48, 0x08, 0xc3],
            useParentField: false);
        session.Items[0x2000].VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1000)];
        AnalysisItem unrelatedVmt = session.GetOrAddItem(0x2010);
        unrelatedVmt.Name = "TOther";
        unrelatedVmt.SetFlags(AnalysisFlags.Vmt);
        unrelatedVmt.VirtualMethods = [new DelphiVmtVirtualMethod(4, 0x1000)];

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1000].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisResolvesClassFieldAccessInsideFieldRange()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x4a, 0x0a, 0xc3],
            useParentField: true);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100c].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual(10, access.Offset);
        Assert.AreEqual("FCount", access.FieldName);
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
    public async Task AnalysisInfersGlobalClassPointerTypeWhenStoringKnownClassRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0xa3, 0x20, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x2020].DataTypeCandidate);
        Assert.IsTrue(session.Items[0x2020].Flags.HasFlag(AnalysisFlags.Data));
        CrossReference reference = session.GetIncomingCrossReferences(0x2020).Single();
        Assert.AreEqual(0x100au, reference.SourceAddress);
        Assert.AreEqual(CrossReferenceKind.Constant, reference.Kind);
    }

    [TestMethod]
    public async Task AnalysisInfersClassTypeForGlobalStoreThroughTrackedRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x89, 0x10,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x2020].DataTypeCandidate);
        Assert.IsTrue(session.GetIncomingCrossReferences(0x2020).Any(reference =>
            reference.SourceAddress == 0x1011u
            && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisInfersClassTypeForGlobalStoreThroughEbx()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0xbb, 0x20, 0x20, 0x40, 0x00,
            0x89, 0x13,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x2020].DataTypeCandidate);
        Assert.IsTrue(session.GetIncomingCrossReferences(0x2020).Any(reference =>
            reference.SourceAddress == 0x1011u
            && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisInfersClassTypeForGlobalStoreThroughTrackedRegisterOffset()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0xb8, 0x1c, 0x20, 0x40, 0x00,
            0x89, 0x50, 0x04,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x2020].DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferClassTypeForIndexedGlobalStoreThroughTrackedRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x31, 0xc9,
            0x89, 0x14, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x2020));
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferGlobalTypeForUnknownStoreBase()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0x31, 0xc0,
            0x89, 0x10,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x2020));
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferGlobalTypeForSegmentRelativeStore()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x89, 0xc2,
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x64, 0x89, 0x10,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x2020));
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferGlobalClassPointerTypeAfterRegisterClobber()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0xa3, 0x20, 0x20, 0x40, 0x00,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x2020));
    }

    [TestMethod]
    public async Task AnalysisInfersClassTypeForStackLocalStoresFromKnownRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0xfc,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        CollectionAssert.AreEqual(
            new[] { new StackLocalVariable(-4, 4, "TChild") },
            session.Items[0x1000].StackLocalVariables.ToArray());
    }

    [TestMethod]
    public async Task AnalysisInfersClassTypeForStackArgumentStoresFromKnownRegister()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(
            new StackArgument(8, 4, "TChild"),
            session.Items[0x1000].StackArguments.Single());
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferStackArgumentTypeAfterRegisterClobber()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0x89, 0x45, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(8u, session.Items[0x1000].StackArguments.Single().Offset);
        Assert.IsNull(session.Items[0x1000].StackArguments.Single().TypeName);
    }

    [TestMethod]
    public async Task AnalysisResolvesFieldAccessAfterTypedStackArgumentStoreAndLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0x08,
            0x8b, 0x4d, 0x08,
            0x89, 0xca,
            0x8b, 0x42, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].StackArguments.Single().TypeName);
        Assert.AreEqual("FCount", session.Items[0x1015].MemberAccessCandidates.Single().FieldName);
    }

    [TestMethod]
    public async Task AnalysisResolvesFieldAccessAfterTypedStackArgumentExchange()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0x08,
            0x87, 0x4d, 0x08,
            0x89, 0xca,
            0x8b, 0x42, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].StackArguments.Single().TypeName);
        Assert.AreEqual("FCount", session.Items[0x1015].MemberAccessCandidates.Single().FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesTypedStackArgumentThroughLea()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0x08,
            0x8d, 0x4d, 0x08,
            0x89, 0xca,
            0x8b, 0x42, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].StackArguments.Single().TypeName);
        Assert.AreEqual("FCount", session.Items[0x1015].MemberAccessCandidates.Single().FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeAcrossNonArgumentRegisters()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xbb, 0x00, 0x20, 0x40, 0x00,
            0x89, 0xde,
            0x89, 0xf7,
            0x8b, 0x47, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1009].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeThroughRegisterPushAndPop()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x50,
            0x31, 0xc0,
            0x59,
            0x8b, 0x41, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1009].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesTrackedGlobalAddressThroughRegisterPushAndPop()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xbb, 0x20, 0x20, 0x40, 0x00,
            0x53,
            0x31, 0xdb,
            0x5e,
            0x8b, 0x06,
            0x8b, 0x48, 0x08,
            0xc3
        ],
            useParentField: false,
            globalPointerValue: 0x402000);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items.TryGetValue(0x100B, out AnalysisItem? fieldLoad));
        MemberAccessCandidate access = fieldLoad!.MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
        Assert.IsTrue(session.Items[0x1009].CrossReferences.Any(reference =>
            reference.TargetAddress == 0x2020
            && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateClassTypeThroughUnsupportedStackPush()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x50,
            0x6a, 0x01,
            0x59,
            0x5b,
            0x8b, 0x43, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x100A].MemberAccessCandidates.Any());
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateClassTypeAcrossCallOnTheStack()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x50,
            0xe8, 0x10, 0x00, 0x00, 0x00,
            0x59,
            0x8b, 0x41, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x100C].MemberAccessCandidates.Any());
    }

    [TestMethod]
    public async Task AnalysisLoadsVmtPointerThroughGlobalAddressHeldInEbx()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xbb, 0x20, 0x20, 0x40, 0x00,
                0x8b, 0x03,
                0x8b, 0x48, 0x08,
                0xc3
            ],
            useParentField: false,
            globalPointerValue: 0x402000);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1007].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
        Assert.IsTrue(session.Items[0x1005].CrossReferences.Any(reference =>
            reference.TargetAddress == 0x2020
            && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisInvalidatesGlobalAddressHeldInEbxAfterFullOrPartialOverwrite()
    {
        (byte[] Code, uint LoadRva)[] cases =
        [
            (
                [
                    0xbb, 0x20, 0x20, 0x40, 0x00,
                    0x31, 0xdb,
                    0x8b, 0x03,
                    0xc3
                ],
                0x1007),
            (
                [
                    0xbb, 0x20, 0x20, 0x40, 0x00,
                    0x66, 0xbb, 0x00, 0x00,
                    0x8b, 0x03,
                    0xc3
                ],
                0x1009)
        ];

        foreach ((byte[] code, uint loadRva) in cases)
        {
            AnalysisSession session = CreateClassFieldAccessSession(
                code,
                useParentField: false,
                globalPointerValue: 0x402000);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsFalse(session.Items[loadRva].CrossReferences.Any(reference =>
                reference.TargetAddress == 0x2020
                && reference.Kind == CrossReferenceKind.Constant));
        }
    }

    [TestMethod]
    public async Task AnalysisInvalidatesNonArgumentClassTypeAfterFullOrPartialOverwrite()
    {
        (byte[] Code, uint AccessRva)[] cases =
        [
            (
                [
                    0xbb, 0x00, 0x20, 0x40, 0x00,
                    0x31, 0xdb,
                    0x8b, 0x43, 0x08,
                    0xc3
                ],
                0x1007),
            (
                [
                    0xbb, 0x00, 0x20, 0x40, 0x00,
                    0x66, 0xbb, 0x00, 0x00,
                    0x8b, 0x43, 0x08,
                    0xc3
                ],
                0x1009)
        ];

        foreach ((byte[] code, uint accessRva) in cases)
        {
            AnalysisSession session = CreateClassFieldAccessSession(code, useParentField: false);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(0, session.Items[accessRva].MemberAccessCandidates.Count);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotInferUnknownStackArgumentTypeFromExchange()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0xc1,
            0x87, 0x4d, 0x08,
            0x89, 0xca,
            0x8b, 0x42, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1000].StackArguments.Single().TypeName);
        Assert.AreEqual(0, session.Items[0x1014].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisLoadsTypedStackLocalThroughExchange()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0xf8,
            0x87, 0x4d, 0xf8,
            0x89, 0xca,
            0x8b, 0x42, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].StackLocalVariables.Single().TypeName);
        Assert.AreEqual("FCount", session.Items[0x1015].MemberAccessCandidates.Single().FieldName);
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
    public async Task AnalysisPropagatesGlobalDataTypeThroughAbsolutePointerLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xa1, 0x20, 0x20, 0x40, 0x00,
            0x89, 0xc2,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem pointerGlobal = session.GetOrAddItem(0x2020);
        pointerGlobal.DataTypeCandidate = "TChild";
        pointerGlobal.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1007].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeFromImmediateGlobalAddress()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x48, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem global = session.GetOrAddItem(0x2020);
        global.DataTypeCandidate = "TChild";
        global.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1005].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeFromAbsoluteGlobalLea()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x8d, 0x05, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x48, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem global = session.GetOrAddItem(0x2020);
        global.DataTypeCandidate = "TChild";
        global.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1006].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateTypeFromAddressInsideCodeSection()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x10, 0x40, 0x00,
            0x8b, 0x48, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem codeItem = session.GetOrAddItem(0x1000);
        codeItem.DataTypeCandidate = "TChild";
        codeItem.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1005].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeFromGlobalVmtPointerLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xa1, 0x20, 0x20, 0x40, 0x00,
            0x89, 0xc2,
            0xe8, 0x14, 0x00, 0x00, 0x00,
            0xc3
        ],
            useParentField: false,
            globalPointerValue: 0x00402000);
        session.GetOrAddItem(0x1020).Name = "@AsClass";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisPropagatesClassTypeFromTrackedOffsetVmtPointerLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x1c, 0x20, 0x40, 0x00,
            0x8b, 0x50, 0x04,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0xc3
        ],
            useParentField: false,
            globalPointerValue: 0x00402000);
        session.GetOrAddItem(0x1020).Name = "@AsClass";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("TChild", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateUntypedAbsolutePointerLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xa1, 0x20, 0x20, 0x40, 0x00,
            0x89, 0xc2,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1007].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisPropagatesGlobalTypeThroughTrackedRegisterPointerLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x10,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem pointerGlobal = session.GetOrAddItem(0x2020);
        pointerGlobal.DataTypeCandidate = "TChild";
        pointerGlobal.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1007].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisPropagatesGlobalTypeThroughTrackedRegisterPointerLoadWithOffset()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x50, 0x04,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem pointerGlobal = session.GetOrAddItem(0x2024);
        pointerGlobal.DataTypeCandidate = "TChild";
        pointerGlobal.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1008].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateUntypedOffsetGlobalThroughTrackedRegisterLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x8b, 0x50, 0x04,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1008].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateGlobalTypeThroughSegmentOverriddenRegisterLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x64, 0x8b, 0x50, 0x04,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem pointerGlobal = session.GetOrAddItem(0x2024);
        pointerGlobal.DataTypeCandidate = "TChild";
        pointerGlobal.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1009].MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisPropagatesKnownTypeThroughFrameLocalLoad()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xe8, 0x13, 0x00, 0x00, 0x00,
            0x89, 0x45, 0xfc,
            0x8b, 0x45, 0xfc,
            0x89, 0xc2,
            0x8b, 0x4a, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x1015].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisDoesNotPropagateUnknownFrameLocalType()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0x55,
            0x8b, 0xec,
            0x8b, 0x45, 0xfc,
            0x89, 0xc2,
            0x8b, 0x4a, 0x08,
            0xc9,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(0, session.Items[0x1008].MemberAccessCandidates.Count);
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
    public async Task AnalysisPropagatesInlineRecordTypeThroughFieldLea()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [
                0xb8, 0x00, 0x20, 0x40, 0x00,
                0xe8, 0x16, 0x00, 0x00, 0x00,
                0x8d, 0x50, 0x08,
                0x8b, 0x4a, 0x04,
                0xc3
            ],
            useParentField: false);
        AnalysisItem recordType = session.Items[0x2018];
        recordType.Name = "TInline";
        recordType.TypeKind = DelphiTypeKind.Record;
        recordType.RecordSizeBytes = 8;
        recordType.RecordFields = [new DelphiRttiRecordField("FValue", 4, 0x2020)];
        AnalysisItem integerType = session.GetOrAddItem(0x2020);
        integerType.Name = "Integer";
        integerType.TypeKind = DelphiTypeKind.Integer;

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate addressAccess = session.Items[0x100a].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", addressAccess.OwnerTypeName);
        Assert.AreEqual("FCount", addressAccess.FieldName);
        MemberAccessCandidate valueAccess = session.Items[0x100d].MemberAccessCandidates.Single();
        Assert.AreEqual("TInline", valueAccess.OwnerTypeName);
        Assert.AreEqual("FValue", valueAccess.FieldName);
        Assert.AreEqual("Integer", valueAccess.TypeName);
    }

    [TestMethod]
    public async Task AnalysisResolvesNestedRecordFieldFromCombinedObjectOffset()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
            [0xb8, 0x00, 0x20, 0x40, 0x00, 0xe8, 0x16, 0x00, 0x00, 0x00, 0x89, 0xc2, 0x8b, 0x42, 0x0e, 0xc3],
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
    public async Task AnalysisCarriesAsClassTypeAcrossMatchingConditionalBranch()
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

        Assert.AreEqual("TExample", session.Items[0x1000].ReturnTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisCarriesClassRegisterTypeToForwardBranchTarget()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb9, 0x00, 0x20, 0x40, 0x00,
            0x74, 0x04,
            0x31, 0xc9,
            0xc3,
            0x90,
            0x8b, 0x41, 0x08,
            0x8b, 0x50, 0x0c,
            0xc3
        ],
            useParentField: false);
        session.Items[0x2000].Fields = [new DelphiVmtField("FChild", 8, 0x2018)];
        AnalysisItem nestedTypeInfo = session.GetOrAddItem(0x2018);
        nestedTypeInfo.Name = "TInner";
        nestedTypeInfo.TypeKind = DelphiTypeKind.Class;
        nestedTypeInfo.ClassVmtAddress = 0x2020;
        AnalysisItem nestedVmt = session.GetOrAddItem(0x2020);
        nestedVmt.Name = "TInner";
        nestedVmt.SetFlags(AnalysisFlags.Vmt);
        nestedVmt.Fields = [new DelphiVmtField("FValue", 12, 0x2038)];
        session.GetOrAddItem(0x2038).Name = "Integer";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100b].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FChild", access.FieldName);
        MemberAccessCandidate nestedAccess = session.Items[0x100e].MemberAccessCandidates.Single();
        Assert.AreEqual("TInner", nestedAccess.OwnerTypeName);
        Assert.AreEqual("FValue", nestedAccess.FieldName);
    }

    [TestMethod]
    public async Task AnalysisCarriesTrackedGlobalAddressToForwardBranchTarget()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x74, 0x02,
            0xc3,
            0x90,
            0x8b, 0x10,
            0x8b, 0x4a, 0x08,
            0xc3
        ],
            useParentField: false,
            globalPointerValue: 0x402000);
        session.GetOrAddItem(0x2020).DataTypeCandidate = "TChild";

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100b].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisCarriesRegisterPushContextToForwardBranchTarget()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x50,
            0x74, 0x02,
            0x58,
            0xc3,
            0x59,
            0x8b, 0x51, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100b].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
    }

    [TestMethod]
    public async Task AnalysisDropsConflictingRegisterPushContextsAtBranchMerge()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x50,
            0x74, 0x09,
            0x58,
            0x52,
            0xeb, 0x05,
            0x90, 0x90, 0x90, 0x90, 0x90,
            0x59,
            0x8b, 0x51, 0x08,
            0xc3
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x1012, out AnalysisItem? fieldAccess)
            && fieldAccess.MemberAccessCandidates.Count != 0);
    }

    [TestMethod]
    public async Task AnalysisDropsConflictingClassRegisterTypesAtBranchMerge()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb9, 0x00, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xb9, 0x20, 0x20, 0x40, 0x00,
            0xeb, 0x01,
            0xc3,
            0x8b, 0x41, 0x08,
            0xc3
        ],
            useParentField: false);
        AnalysisItem otherVmt = session.GetOrAddItem(0x2020);
        otherVmt.Name = "TOther";
        otherVmt.SetFlags(AnalysisFlags.Vmt);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.TryGetValue(0x100f, out AnalysisItem? fieldAccess)
            && fieldAccess.MemberAccessCandidates.Count != 0);
    }

    [TestMethod]
    public async Task AnalysisCarriesClassTypeAcrossBackwardBranchToPreviouslyScannedBlock()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0xeb, 0x04,
            0x90, 0x90, 0x90, 0x90,
            0x74, 0x06,
            0xc3,
            0x8b, 0x50, 0x08,
            0xc3,
            0x90,
            0xeb, 0xf9
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        MemberAccessCandidate access = session.Items[0x100e].MemberAccessCandidates.Single();
        Assert.AreEqual("TChild", access.OwnerTypeName);
        Assert.AreEqual("FCount", access.FieldName);
        Assert.AreEqual(1, session.DisassemblyLines.Count(line => line.Address == 0x100e));
    }

    [TestMethod]
    public async Task AnalysisRemovesFieldCandidateAfterLateConflictingPredecessor()
    {
        AnalysisSession session = CreateClassFieldAccessSession(
        [
            0xb8, 0x00, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0x8b, 0x50, 0x08,
            0xc3,
            0x90,
            0x31, 0xc0,
            0xeb, 0xf7
        ],
            useParentField: false);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items.TryGetValue(0x100f, out AnalysisItem? fieldAccess));
        Assert.AreEqual(0, fieldAccess.MemberAccessCandidates.Count);
    }

    [TestMethod]
    public async Task AnalysisRemovesCallDataTypeCandidateAfterLateConflictingPredecessor()
    {
        byte[] image = new byte[0x60];
        byte[] code =
        [
            0xb8, 0x04, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0xe8, 0x0c, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0xeb, 0xf7
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "runtime-type-late-conflict.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x20, 0x40, 0x20, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@VarClr";
        AnalysisItem variable = session.GetOrAddItem(0x2004);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(variable.Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsNull(variable.DataTypeCandidate);
    }

    [TestMethod]
    public async Task AnalysisRemovesPrototypeArgumentTypesAfterLateConflictingPredecessor()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xb8, 0x04, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0xe8, 0x0c, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0xeb, 0xf7
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "prototype-type-late-conflict.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = DelphiVersion.Delphi2009
        };
        session.GetOrAddItem(0x1020).Name = "TExample.ReadValue";
        AnalysisItem guid = session.GetOrAddItem(0x2004);
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [
                new KnowledgeBaseProcedure(
                    7,
                    "TExample.ReadValue",
                    (byte)'C',
                    ReadOnlyMemory<byte>.Empty,
                    ReadOnlyMemory<byte>.Empty)
                {
                    Arguments =
                    [
                        new KnowledgeBaseArgument(
                            0x21,
                            true,
                            0,
                            sizeof(uint),
                            "Value",
                            "TGUID")
                    ]
                }
            ]));

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items.TryGetValue(0x100f, out AnalysisItem? callItem));
        Assert.IsTrue(callItem!.Flags.HasFlag(AnalysisFlags.Call));
        Assert.IsNull(guid.DataTypeCandidate);
        Assert.IsNull(guid.Name);
        Assert.IsFalse(guid.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2004)
                .Count(reference => reference.SourceAddress == 0x100f
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisRemovesPrototypeResourceStringCandidateAfterLateConflictingPredecessor()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xb8, 0x04, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0xe8, 0x0c, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0xeb, 0xf7
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x48, sizeof(uint)), 1);
        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.LoadResource",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(
                    0x21,
                    true,
                    0,
                    sizeof(uint),
                    "Resource",
                    "PResStringRec")
            ]
        };
        AnalysisSession session = new(
            "prototype-resource-string-late-conflict.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem call = session.GetOrAddItem(0x1020);
        call.Name = procedure.Name;
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        session.LoadPeDirectories([], [], [new PeResourceString(1, 0x0409, "Greeting")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items.TryGetValue(0x100f, out AnalysisItem? callItem));
        Assert.IsTrue(callItem!.Flags.HasFlag(AnalysisFlags.Call));
        Assert.IsNull(callItem.ResourceStringCandidate);
    }

    [TestMethod]
    public async Task AnalysisRemovesManagedStringStoreTypeAfterLateConflictingPredecessor()
    {
        AnalysisSession positiveSession = CreateManagedStringStoreSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x89, 0x05, 0x30, 0x20, 0x40, 0x00,
            0xc3
        ]);
        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(positiveSession, null, CancellationToken.None);
        Assert.AreEqual("UnicodeString", positiveSession.Items[0x2030].DataTypeCandidate);

        AnalysisSession session = CreateManagedStringStoreSession(
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0x89, 0x05, 0x30, 0x20, 0x40, 0x00,
            0x31, 0xc0,
            0xeb, 0xf6
        ]);
        AnalysisItem variable = session.Items[0x2030];
        variable.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(variable.DataTypeCandidate);
        Assert.IsTrue(variable.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2030)
                .Count(reference => reference.SourceAddress == 0x100f));
    }

    [TestMethod]
    public async Task AnalysisRemovesClassStoreTypeAfterLateConflictingPredecessor()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xb8, 0x20, 0x20, 0x40, 0x00,
            0x74, 0x08,
            0xeb, 0x0b,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0x89, 0x05, 0x30, 0x20, 0x40, 0x00,
            0x31, 0xc0,
            0xeb, 0xf6
        ];
        code.CopyTo(image, 0);
        AnalysisSession session = new(
            "class-store-late-conflict.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem classVmt = session.GetOrAddItem(0x2020);
        classVmt.Name = "TChild";
        classVmt.SetFlags(AnalysisFlags.Vmt);
        AnalysisItem variable = session.GetOrAddItem(0x2030);
        variable.SetFlags(AnalysisFlags.Data);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(variable.DataTypeCandidate);
        Assert.IsTrue(variable.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2030)
                .Count(reference => reference.SourceAddress == 0x100f));
    }

    [TestMethod]
    public async Task AnalysisAppliesRuntimeCallCandidatesInBranchFlowTargets()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xeb, 0x06,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0xb8, 0x30, 0x20, 0x40, 0x00,
            0xe8, 0x0e, 0x00, 0x00, 0x00,
            0xb8, 0x01, 0x00, 0x00, 0x00,
            0xe8, 0x08, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        image[0x24] = 0xc3;
        AnalysisSession session = new(
            "branch-runtime-candidates.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@VarClr";
        session.GetOrAddItem(0x1024).Name = "@LoadStr";
        AnalysisItem variable = session.GetOrAddItem(0x2030);
        session.LoadPeDirectories([], [], [new PeResourceString(1, 0x0409, "Greeting")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Variant", variable.DataTypeCandidate);
        Assert.IsTrue(variable.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual("Greeting", session.Items[0x1017].ResourceStringCandidate);
        Assert.IsTrue(session.Items.TryGetValue(0x1008, out AnalysisItem? flowTarget));
        Assert.IsFalse(flowTarget!.Flags.HasFlag(AnalysisFlags.ProcedureStart));
    }

    [TestMethod]
    public async Task AnalysisAppliesClassAndManagedStringStoresInBranchFlowTargets()
    {
        foreach (bool managedString in new[] { false, true })
        {
            byte[] image = new byte[0x80];
            byte[] code =
            [
                0xb8, 0x20, 0x20, 0x40, 0x00,
                0xeb, 0x08,
                0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
                0x89, 0x05, 0x30, 0x20, 0x40, 0x00,
                0xc3
            ];
            code.CopyTo(image, 0);
            AnalysisSession session = new(
                managedString ? "branch-managed-string-store.exe" : "branch-class-store.exe",
                image,
                [
                    new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                    new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
                ],
                0x400000)
            {
                EntryPointRva = 0x1000
            };
            AnalysisItem source = session.GetOrAddItem(0x2020);
            source.Name = managedString ? "ManagedText" : "TStoredClass";
            source.SetFlags(managedString
                ? AnalysisFlags.Data | AnalysisFlags.String
                : AnalysisFlags.Vmt);
            if (managedString)
            {
                source.TypeKind = DelphiTypeKind.UnicodeString;
            }

            AnalysisItem destination = session.GetOrAddItem(0x2030);

            await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.AreEqual(
                managedString ? "UnicodeString" : "TStoredClass",
                destination.DataTypeCandidate);
            Assert.IsTrue(destination.Flags.HasFlag(AnalysisFlags.Data));
            Assert.IsTrue(session.Items.TryGetValue(0x100f, out AnalysisItem? flowTarget));
            Assert.IsFalse(flowTarget!.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        }
    }

    [TestMethod]
    public async Task AnalysisAppliesPrototypeStringArgumentsInBranchFlowTargets()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xeb, 0x06,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0xb8, 0x10, 0x20, 0x40, 0x00,
            0xe8, 0x06, 0x00, 0x00, 0x00,
            0xc3,
            0x90, 0x90, 0x90, 0x90, 0x90,
            0xc3
        ];
        code.CopyTo(image, 0);
        Encoding.Latin1.GetBytes("Branch argument\0").CopyTo(image, 0x50);
        AnalysisSession session = new(
            "branch-prototype-argument.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            DelphiVersion.Delphi2007)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "TExample.ReadText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [
                new KnowledgeBaseProcedure(
                    7,
                    "TExample.ReadText",
                    (byte)'C',
                    ReadOnlyMemory<byte>.Empty,
                    ReadOnlyMemory<byte>.Empty)
                {
                    Arguments =
                    [
                        new KnowledgeBaseArgument(
                            0x21,
                            true,
                            0,
                            sizeof(uint),
                            "Text",
                            "PAnsiChar")
                    ]
                }
            ]));

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Branch argument", session.Items[0x2010].Name);
        Assert.IsTrue(session.Items.TryGetValue(0x1008, out AnalysisItem? flowTarget));
        Assert.IsFalse(flowTarget!.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2010)
                .Count(reference => reference.SourceAddress == 0x100d
                    && reference.Kind == CrossReferenceKind.Constant));
    }

    [TestMethod]
    public async Task AnalysisAppliesPrototypeStackArgumentsInBranchFlowTargets()
    {
        byte[] image = new byte[0x80];
        byte[] code =
        [
            0xeb, 0x06,
            0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
            0x68, 0x30, 0x20, 0x40, 0x00,
            0xe8, 0x06, 0x00, 0x00, 0x00,
            0xc3,
            0x90, 0x90, 0x90, 0x90, 0x90,
            0xc3
        ];
        code.CopyTo(image, 0);
        Encoding.Latin1.GetBytes("Branch stack\0").CopyTo(image, 0x70);
        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadStackText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = 3,
            Arguments =
            [
                    new KnowledgeBaseArgument(
                        0x21,
                        false,
                        8,
                        sizeof(uint),
                        "Text",
                        "PAnsiChar")
            ]
        };
        AnalysisSession session = new(
            "branch-stack-prototype-argument.exe",
            image,
            [
                    new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                    new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            DelphiVersion.Delphi2007)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = procedure.Name;
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Branch stack", session.Items[0x2030].Name);
        Assert.IsTrue(session.Items[0x2030].Flags.HasFlag(AnalysisFlags.String));
        Assert.IsTrue(session.Items.TryGetValue(0x1008, out AnalysisItem? flowTarget));
        Assert.IsFalse(flowTarget!.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(
            1,
            session.GetIncomingCrossReferences(0x2030)
                    .Count(reference => reference.SourceAddress == 0x100d
                        && reference.Kind == CrossReferenceKind.Constant));
    }

    private static AnalysisSession CreateManagedStringStoreSession(byte[] code)
    {
        byte[] image = new byte[0x80];
        code.CopyTo(image, 0);
        AnalysisSession session = new(
            "managed-string-store-late-conflict.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem managedString = session.GetOrAddItem(0x2020);
        managedString.SetFlags(AnalysisFlags.Data | AnalysisFlags.String);
        managedString.TypeKind = DelphiTypeKind.UnicodeString;
        session.GetOrAddItem(0x2030);
        return session;
    }

    [TestMethod]
    public async Task AnalysisRemovesCallResourceStringCandidateAfterRegisterValueIsClobbered()
    {
        byte[] image = new byte[0x40];
        byte[] code =
        [
            0xb8, 0x01, 0x00, 0x00, 0x00,
            0xe8, 0x16, 0x00, 0x00, 0x00,
            0x31, 0xc0,
            0xeb, 0xf7
        ];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        AnalysisSession session = new(
            "resource-string-late-conflict.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@LoadStr";
        session.LoadPeDirectories(
            [],
            [],
            [new PeResourceString(1, 0x0409, "Greeting")]);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.Items[0x1005].ResourceStringCandidate);
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
    public async Task AnalysisMarksCalleeAsFunctionWhenBranchOnlyFlowReadsEaxResult()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x74;
        code[1] = 0x02;
        code[2] = 0xc3;
        code[4] = 0xe8;
        code[5] = 0x0f;
        code[9] = 0x83;
        code[10] = 0xf8;
        code[11] = 0x00;
        code[12] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkBranchOnlyCalleeWhenEaxIsOverwritten()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x74;
        code[1] = 0x02;
        code[2] = 0xc3;
        code[4] = 0xe8;
        code[5] = 0x0f;
        code[9] = 0xb8;
        code[10] = 0x01;
        code[14] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisMarksCalleeAsFunctionWhenBranchOnlyFlowStoresX87Result()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x74;
        code[1] = 0x02;
        code[2] = 0xc3;
        code[4] = 0xe8;
        code[5] = 0x0f;
        code[9] = 0xd9;
        code[10] = 0x1d;
        code[11] = 0x00;
        code[12] = 0x20;
        code[13] = 0x00;
        code[14] = 0x00;
        code[15] = 0xc3;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1004].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual("Fstp", session.Items[0x1009].Name);
        Assert.IsTrue(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
    }

    [TestMethod]
    public async Task AnalysisDoesNotMarkBranchOnlyX87CalleeWhenStoreIsNotImmediate()
    {
        byte[] code = new byte[0x20];
        code[0] = 0x74;
        code[1] = 0x02;
        code[2] = 0xc3;
        code[4] = 0xe8;
        code[5] = 0x0f;
        code[9] = 0xb8;
        code[10] = 0x01;
        code[14] = 0xd9;
        code[15] = 0x1d;
        code[16] = 0x00;
        code[17] = 0x20;
        code[18] = 0x00;
        code[19] = 0x00;
        code[0x18] = 0xc3;
        AnalysisSession session = CreateSession(code);

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.FunctionCandidate));
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

    private static AnalysisSession CreatePrototypeStringSession(
        int knowledgeBaseRegisterIndex,
        KnowledgeBaseArgument argument,
        byte[] literal,
        bool duplicatePrototype = false,
        bool includePrototype = true,
        bool terminateString = true,
        uint stringRva = 0x2010,
        DelphiVersion delphiVersion = DelphiVersion.Delphi2007)
    {
        byte[] image = new byte[0x80];
        image.AsSpan(0x40, 0x40).Fill(0x41);
        int registerOpcode = knowledgeBaseRegisterIndex switch
        {
            0 => 0xb8,
            1 => 0xb9,
            2 => 0xba,
            _ => throw new ArgumentOutOfRangeException(nameof(knowledgeBaseRegisterIndex))
        };
        image[0] = (byte)registerOpcode;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(1, sizeof(uint)),
            checked(0x400000u + stringRva));
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        if (stringRva >= 0x2000 && stringRva < 0x2040)
        {
            int stringRawOffset = checked(0x40 + (int)(stringRva - 0x2000));
            int copyLength = Math.Min(literal.Length, image.Length - stringRawOffset);
            literal.AsSpan(0, copyLength).CopyTo(image.AsSpan(stringRawOffset, copyLength));
            if (terminateString && stringRawOffset + literal.Length < image.Length)
            {
                image[stringRawOffset + literal.Length] = 0;
                if (literal.Contains((byte)0))
                {
                    image[stringRawOffset + literal.Length - 1] = 0;
                }
            }
        }

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments = [argument]
        };
        KnowledgeBaseProcedure[] procedures = !includePrototype
            ? []
            : duplicatePrototype
                ? [procedure, procedure with { Arguments = [] }]
                : [procedure];
        AnalysisSession session = new(
            "prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = delphiVersion
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            procedures));
        return session;
    }

    private static AnalysisSession CreateStackStringPrototypeSession(
        byte callingConvention,
        byte[] literal,
        bool pushImmediate = true,
        bool insertNop = false,
        string argumentType = "PAnsiChar",
        uint targetRva = 0x2020,
        byte argumentTag = 0x21,
        DelphiVersion delphiVersion = DelphiVersion.Delphi2007)
    {
        byte[] image = new byte[0x80];
        image[0] = pushImmediate ? (byte)0x68 : (byte)0x50;
        if (pushImmediate)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(
                image.AsSpan(1, sizeof(uint)),
                checked(0x400000u + targetRva));
        }

        int callOffset = insertNop ? 6 : 5;
        if (insertNop)
        {
            image[5] = 0x90;
        }

        image[callOffset] = 0xe8;
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(callOffset + 1, sizeof(int)),
            0x1010 - (0x1000 + callOffset + 5));
        image[callOffset + 5] = 0xc3;
        image[0x10] = 0xc3;
        if (argumentTag == 0x22)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402020);
        }

        literal.CopyTo(image, 0x60);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadStackText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = callingConvention,
            Arguments =
            [
                new KnowledgeBaseArgument(argumentTag, false, 8, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "stack-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = delphiVersion
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadStackText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateStackWideStringPrototypeSession(
        bool isVarArgument,
        byte[] widePayload)
    {
        byte[] image = new byte[0x80];
        image[0] = 0x68;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(1, sizeof(uint)),
            isVarArgument ? 0x402010u : 0x402020u);
        image[5] = 0xe8;
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(6, sizeof(int)), 0x1010 - 0x100a);
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        if (isVarArgument)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402020);
        }

        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(0x5c, sizeof(int)),
            widePayload.Length - sizeof(ushort));
        widePayload.CopyTo(image, 0x60);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadStackWideText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = 3,
            Arguments =
            [
                new KnowledgeBaseArgument(
                    isVarArgument ? (byte)0x22 : (byte)0x21,
                    false,
                    8,
                    sizeof(uint),
                    "Text",
                    "WideString")
            ]
        };
        AnalysisSession session = new(
            "stack-prototype-wide-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadStackWideText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateMultiStackStringPrototypeSession()
    {
        byte[] image = new byte[0x80];
        image[0] = 0x68;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402030);
        image[5] = 0x68;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(6, sizeof(uint)), 0x402020);
        image[10] = 0xe8;
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(11, sizeof(int)), 0x1018 - 0x100f);
        image[15] = 0xc3;
        image[0x18] = 0xc3;
        Encoding.Latin1.GetBytes("First argument").CopyTo(image, 0x60);
        Encoding.Latin1.GetBytes("Second argument").CopyTo(image, 0x70);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadStackTexts",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = 3,
            Arguments =
            [
                new KnowledgeBaseArgument(0x21, false, 8, sizeof(uint), "First", "PAnsiChar"),
                new KnowledgeBaseArgument(0x21, false, 12, sizeof(uint), "Second", "PAnsiChar")
            ]
        };
        AnalysisSession session = new(
            "multi-stack-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = DelphiVersion.Delphi2007
        };
        session.GetOrAddItem(0x1018).Name = "TExample.ReadStackTexts";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateMixedStackStringPrototypeSession()
    {
        byte[] image = new byte[0x80];
        image[0] = 0x68;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402030);
        image[5] = 0x68;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(6, sizeof(uint)), 0x402020);
        image[10] = 0xe8;
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(11, sizeof(int)), 0x1018 - 0x100f);
        image[15] = 0xc3;
        image[0x18] = 0xc3;
        Encoding.Latin1.GetBytes("Ignored const").CopyTo(image, 0x60);
        Encoding.Latin1.GetBytes("Mixed stack").CopyTo(image, 0x70);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadMixedStackText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            CallingConvention = 3,
            Arguments =
            [
                new KnowledgeBaseArgument(0x23, false, 8, sizeof(uint), "Ignored", "PAnsiChar"),
                new KnowledgeBaseArgument(0x21, false, 12, sizeof(uint), "Text", "PAnsiChar")
            ]
        };
        AnalysisSession session = new(
            "mixed-stack-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = DelphiVersion.Delphi2007
        };
        session.GetOrAddItem(0x1018).Name = "TExample.ReadMixedStackText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarStringPointerPrototypeSession(
        string argumentType,
        byte[] literal,
        bool terminateString,
        uint targetRva = 0x2020,
        DelphiVersion delphiVersion = DelphiVersion.Delphi2007)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(0x50, sizeof(uint)),
            checked(0x400000u + targetRva));
        if (!terminateString)
        {
            image.AsSpan(0x60).Fill(0x41);
        }

        int copyLength = Math.Min(literal.Length, image.Length - 0x60);
        literal.AsSpan(0, copyLength).CopyTo(image.AsSpan(0x60, copyLength));
        if (terminateString && 0x60 + literal.Length < image.Length)
        {
            image[0x60 + literal.Length] = 0;
            if (literal.Contains((byte)0))
            {
                image[0x60 + literal.Length - 1] = 0;
            }
        }

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "prototype-var-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = delphiVersion
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarShortStringPrototypeSession(
        string argumentType,
        byte[] shortString)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402020);
        int copyLength = Math.Min(shortString.Length, image.Length - 0x60);
        shortString.AsSpan(0, copyLength).CopyTo(image.AsSpan(0x60, copyLength));

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarShortText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "prototype-var-shortstring.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarShortText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarGuidPrototypeSession(
        uint targetRva,
        byte[] guidBytes,
        bool truncated = false)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(0x50, sizeof(uint)),
            checked(0x400000u + targetRva));
        if (targetRva >= 0x2000 && targetRva < 0x2040)
        {
            int rawOffset = checked(0x40 + (int)(targetRva - 0x2000));
            int copyLength = Math.Min(guidBytes.Length, image.Length - rawOffset);
            guidBytes.AsSpan(0, copyLength).CopyTo(image.AsSpan(rawOffset, copyLength));
        }

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarGuid",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Id", "TGUID")
            ]
        };
        AnalysisSession session = new(
            "prototype-var-guid.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, truncated ? 0x3cu : 0x40u, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarGuid";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarWideStringPrototypeSession(
        int byteLength,
        byte[] payload)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402024);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x60, sizeof(int)), byteLength);
        int copyLength = Math.Min(payload.Length, image.Length - 0x64);
        payload.AsSpan(0, copyLength).CopyTo(image.AsSpan(0x64, copyLength));

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarWideText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Text", "WideString")
            ]
        };
        AnalysisSession session = new(
            "prototype-var-widestring.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarWideText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarManagedStringPrototypeSession(
        string argumentType,
        ushort elementSize)
    {
        const string text = "Managed var";
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402024);
        BinaryPrimitives.WriteUInt16LittleEndian(
            image.AsSpan(0x58, sizeof(ushort)),
            elementSize == 1 ? (ushort)1252 : (ushort)1200);
        BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(0x5a, sizeof(ushort)), elementSize);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x5c, sizeof(int)), -1);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x60, sizeof(int)), text.Length);
        byte[] payload = elementSize == 1
            ? Encoding.Latin1.GetBytes(text)
            : Encoding.Unicode.GetBytes(text);
        payload.CopyTo(image, 0x64);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "prototype-var-managed-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = DelphiVersion.Delphi2009
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarLegacyAnsiStringPrototypeSession(
        string argumentType,
        int referenceCount = -1,
        int? declaredLength = null,
        bool terminator = true,
        uint dataRawSize = 0x40)
    {
        const string text = "Legacy var text";
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x50, sizeof(uint)), 0x402028);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x60, sizeof(int)), referenceCount);
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(0x64, sizeof(int)),
            declaredLength ?? text.Length);
        Encoding.Latin1.GetBytes(text).CopyTo(image, 0x68);
        image[0x68 + text.Length] = terminator ? (byte)0 : (byte)1;

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadLegacyVarText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "prototype-var-legacy-ansistring.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, dataRawSize, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = DelphiVersion.Delphi2007
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadLegacyVarText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateLegacyManagedStringPrototypeSession(
        string argumentType,
        int referenceCount = -1)
    {
        const string text = "Legacy text";
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402008);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(0x40, sizeof(int)),
            referenceCount);
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(0x44, sizeof(int)),
            text.Length);
        Encoding.Latin1.GetBytes(text).CopyTo(image, 0x48);

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "legacy-managed-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            DelphiVersion.Delphi7)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateWideStringPrototypeSession(
        int byteLength,
        byte[] payload,
        uint contentRva = 0x2004)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(1, sizeof(uint)),
            checked(0x400000u + contentRva));
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;

        int prefixRawOffset = checked(0x40 + (int)(contentRva - sizeof(uint) - 0x2000));
        BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(prefixRawOffset, sizeof(int)),
            byteLength);
        int contentRawOffset = checked(prefixRawOffset + sizeof(int));
        int copyLength = Math.Min(payload.Length, image.Length - contentRawOffset);
        payload.AsSpan(0, copyLength).CopyTo(image.AsSpan(contentRawOffset, copyLength));

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", "WideString")
            ]
        };
        AnalysisSession session = new(
            "wide-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            DelphiVersion.Delphi7)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreatePrototypeResourceStringSession(
        uint recordRva,
        uint resourceId,
        PeResourceString[] resources)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(1, sizeof(uint)),
            checked(0x400000u + recordRva));
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        if (recordRva >= 0x2000 && recordRva < 0x2040)
        {
            int rawOffset = checked(0x40 + (int)(recordRva - 0x2000));
            if (rawOffset + 2 * sizeof(uint) <= image.Length)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(
                    image.AsSpan(rawOffset + sizeof(uint), sizeof(uint)),
                    resourceId);
            }
        }

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadResource",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Resource", "PResStringRec")
            ]
        };
        AnalysisSession session = new(
            "prototype-resource-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadResource";
        session.LoadPeDirectories([], [], resources);
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarPrototypeResourceStringSession(
        uint recordRva,
        PeResourceString[] resources)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(0x50, sizeof(uint)),
            checked(0x400000u + recordRva));
        if (recordRva >= 0x2000 && recordRva < 0x2040)
        {
            int rawOffset = checked(0x40 + (int)(recordRva - 0x2000));
            if (rawOffset + 2 * sizeof(uint) <= image.Length)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(
                    image.AsSpan(rawOffset + sizeof(uint), sizeof(uint)),
                    42);
            }
        }

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarResource",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Resource", "PResStringRec")
            ]
        };
        AnalysisSession session = new(
            "prototype-var-resource-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarResource";
        session.LoadPeDirectories([], [], resources);
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateVarFixedSizeTypePrototypeSession(
        string typeName,
        uint targetRva,
        uint dataRawSize,
        DelphiVersion delphiVersion = DelphiVersion.Delphi2007)
    {
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x402010);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(0x50, sizeof(uint)),
            checked(0x400000u + targetRva));

        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadVarValue",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x22, true, 0, sizeof(uint), "Value", typeName)
            ]
        };
        AnalysisSession session = new(
            "prototype-var-fixed-size.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, dataRawSize, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000,
            SelectedDelphiVersion = delphiVersion
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadVarValue";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateManagedStringPrototypeSession(
        string argumentType,
        ushort codePage,
        ushort elementSize,
        DelphiVersion version)
    {
        const string text = "Managed text";
        byte[] textBytes = elementSize == 1
            ? Encoding.Latin1.GetBytes(text)
            : Encoding.Unicode.GetBytes(text);
        byte[] image = new byte[0x80];
        image[0] = 0xb8;
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(1, sizeof(uint)), 0x40200c);
        image[5] = 0xe8;
        image[6] = 0x06;
        image[10] = 0xc3;
        image[0x10] = 0xc3;
        BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(0x40, sizeof(ushort)), codePage);
        BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(0x42, sizeof(ushort)), elementSize);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x44, sizeof(int)), -1);
        BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(0x48, sizeof(int)), text.Length);
        textBytes.CopyTo(image, 0x4c);
        KnowledgeBaseProcedure procedure = new(
            7,
            "TExample.ReadText",
            (byte)'C',
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            Arguments =
            [
                new KnowledgeBaseArgument(0x21, true, 0, sizeof(uint), "Text", argumentType)
            ]
        };
        AnalysisSession session = new(
            "managed-prototype-string.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, 0x40, false)
            ],
            0x400000,
            version)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1010).Name = "TExample.ReadText";
        session.LoadKnowledgeBase(new KnowledgeBaseData(
            2,
            [new KnowledgeBaseModule(7, "System")],
            [procedure]));
        return session;
    }

    private static AnalysisSession CreateRuntimeVariableTypeSession(
        string helperName,
        uint variableAddress,
        bool overwriteEax = false,
        uint? typeInfoAddress = null,
        uint? ecxValue = null,
        bool zeroFillDataSection = false)
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
            zeroFillDataSection
                ?
                [
                    new PeSection(".text", 0x1000, 0x20, 0, 0x20, true),
                    new PeSection(".bss", 0x2000, 0x20, 0x20, 0, false)
                ]
                :
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

    private static AnalysisSession CreatePointerLoadedGlobalHelperSession(
        byte[] loadInstructions,
        bool storePointerInCode = false,
        uint dataRawSize = 0x40,
        int pointerRawOffset = 0x40)
    {
        byte[] image = new byte[0x80];
        byte[] code = new byte[loadInstructions.Length + 6];
        loadInstructions.CopyTo(code, 0);
        int callOffset = loadInstructions.Length;
        code[callOffset] = 0xe8;
        int callNextOffset = callOffset + 5;
        int callDisplacement = 0x20 - callNextOffset;
        BitConverter.GetBytes(callDisplacement).CopyTo(code, callOffset + 1);
        code[^1] = 0xc3;
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        BitConverter.GetBytes(0x00402010u).CopyTo(image, pointerRawOffset);
        if (storePointerInCode)
        {
            BitConverter.GetBytes(0x00402010u).CopyTo(image, 0x30);
        }

        AnalysisSession session = new(
            "runtime-type-pointer-load.exe",
            image,
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".data", 0x2000, 0x40, 0x40, dataRawSize, false)
            ],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1020).Name = "@VarClr";
        return session;
    }

    private static AnalysisSession CreateLocalRuntimeClearSession(string helperName)
    {
        byte[] image = new byte[0x30];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8d, 0x45, 0xf8,
            0xe8, 0x0d, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "local-runtime-clear.exe",
            image,
            [new PeSection(".text", 0x1000, 0x20, 0, 0x20, true)])
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateResourceStringHelperSession(
        string helperName,
        PeResourceString[] resourceStrings)
    {
        byte[] image = new byte[0x20];
        byte[] code =
        [
            0xb8, 0x01, 0x00, 0x00, 0x00,
            0xe8, 0x0e, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "resource-string.exe",
            image,
            [new PeSection(".text", 0x1000, 0x20, 0, 0x20, true)])
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        session.LoadPeDirectories([], [], resourceStrings);
        return session;
    }

    private static AnalysisSession CreateStackRuntimeClearSession(string helperName)
    {
        byte[] image = new byte[0x30];
        byte[] code =
        [
            0x55, 0x8b, 0xec,
            0x8b, 0x45, 0x08,
            0xe8, 0x0d, 0x00, 0x00, 0x00,
            0xc3
        ];
        code.CopyTo(image, 0);
        image[0x18] = 0xc3;
        AnalysisSession session = new(
            "stack-runtime-clear.exe",
            image,
            [new PeSection(".text", 0x1000, 0x20, 0, 0x20, true)])
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = helperName;
        return session;
    }

    private static AnalysisSession CreateLocalStringArrayCleanupSession(
        string helperName,
        uint count,
        bool overwriteEax = false,
        bool preserveLocalAddressThroughStack = false)
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
        if (preserveLocalAddressThroughStack)
        {
            image[6] = 0x50;
            image[7] = 0x31;
            image[8] = 0xc0;
            image[9] = 0x58;
            callOffset = 15;
            returnOffset = 20;
        }
        else if (overwriteEax)
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

    private static AnalysisSession CreateClassFieldAccessSession(
        byte[] code,
        bool useParentField,
        uint? globalPointerValue = null)
    {
        byte[] image = new byte[0x80];
        code.CopyTo(image, 0);
        image[0x20] = 0xc3;
        if (globalPointerValue is uint pointerValue)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(0x60, sizeof(uint)), pointerValue);
        }

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
        childVmt.ClassInstanceSizeBytes = 20;
        if (useParentField)
        {
            childVmt.ParentAddress = 0x2010;
            AnalysisItem parentVmt = session.GetOrAddItem(0x2010);
            parentVmt.Name = "TBase";
            parentVmt.SetFlags(AnalysisFlags.Vmt);
            parentVmt.ClassInstanceSizeBytes = 12;
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
