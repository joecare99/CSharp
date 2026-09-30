using IDR.Core;
using IDR.Core.Models;
using IDR.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Core.Tests;

[TestClass]
public sealed class ArchitectureTests
{
    [TestMethod]
    public void CoreAssemblyLoads()
    {
        Assert.IsNotNull(new ArchitectureMarker());
    }

    private sealed class BlockingDecoder(
        ManualResetEventSlim decodeStarted,
        ManualResetEventSlim releaseDecoder) : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            decodeStarted.Set();
            releaseDecoder.Wait();
            return new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return);
        }
    }

    private sealed class ReturnDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness) =>
            new(address, 1, "Ret", null, InstructionFlowControl.Return);
    }

    private sealed class InvalidInstructionDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness) =>
            throw new InstructionDecodeException(address);
    }

    private sealed class CancelOnVmtProgress(CancellationTokenSource cancellation)
        : IProgress<AnalysisProgress>
    {
        public void Report(AnalysisProgress value)
        {
            if (value.Phase == "VmtScan")
            {
                cancellation.Cancel();
            }
        }
    }

    private sealed class CancelOnRuntimeSymbolsProgress(CancellationTokenSource cancellation)
        : IProgress<AnalysisProgress>
    {
        public void Report(AnalysisProgress value)
        {
            if (value.Phase == "RuntimeSymbols")
            {
                cancellation.Cancel();
            }
        }
    }

    private sealed class ManyCallTargetsDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            if (((address - 0x1000) & 1) == 0)
            {
                return new DecodedInstruction(
                    address,
                    1,
                    "Call",
                    address + 2,
                    InstructionFlowControl.Call);
            }

            return new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return);
        }
    }

    private sealed class AdjacentBranchTargetDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            if (((address - 0x1000) % 3) == 0)
            {
                return new DecodedInstruction(
                    address,
                    1,
                    "Jne",
                    address + 1,
                    InstructionFlowControl.ConditionalBranch);
            }

            return new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return);
        }
    }

    private sealed class DirectProcedureTargetDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return address switch
            {
                0x1000 => new DecodedInstruction(address, 5, "Call", 0x1008, InstructionFlowControl.Call),
                0x1008 => new DecodedInstruction(address, 1, "Nop", null),
                _ => new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return)
            };
        }
    }

    private sealed class ConditionalBranchTargetDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return address switch
            {
                0x1000 => new DecodedInstruction(
                    address,
                    2,
                    "Jz",
                    0x1008,
                    InstructionFlowControl.ConditionalBranch),
                _ => new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return)
            };
        }
    }

    private sealed class UnconditionalBranchTargetDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return address switch
            {
                0x1000 => new DecodedInstruction(
                    address,
                    2,
                    "Jmp",
                    0x1008,
                    InstructionFlowControl.UnconditionalBranch),
                _ => new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return)
            };
        }
    }

    private sealed class ExceptionHandlerBranchDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return address == 0x1000
                ? new DecodedInstruction(
                    address,
                    5,
                    "Jmp",
                    0x1018,
                    InstructionFlowControl.UnconditionalBranch)
                : new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return);
        }
    }

    private sealed class ImportAndExportTargetDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return address switch
            {
                0x1000 => new DecodedInstruction(address, 5, "Call", 0x1010, InstructionFlowControl.Call),
                0x1005 => new DecodedInstruction(address, 5, "Call", 0x2000, InstructionFlowControl.Call),
                _ => new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return)
            };
        }
    }

    [TestMethod]
    public void AddressMapConvertsOnlyMappedRawBytes()
    {
        AddressMap map = new(
        [
            new PeSection(".text", 0x1000, 0x200, 0x400, 0x180, true)
        ]);

        Assert.IsTrue(map.TryRvaToRaw(0x1010, out int rawOffset));
        Assert.AreEqual(0x410, rawOffset);
        Assert.IsFalse(map.TryRvaToRaw(0x1180, out _));
        Assert.IsTrue(map.TryRawToRva(0x410, out uint rva));
        Assert.AreEqual(0x1010u, rva);
    }

    [TestMethod]
    public void AnalysisItemMaintainsFlagsAndReferences()
    {
        AnalysisItem item = new(0x1234);
        item.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
        item.AddCrossReference(new CrossReference(0x1000, 0x1234, CrossReferenceKind.Call));
        item.ClearFlags(AnalysisFlags.Code);

        Assert.IsFalse(item.Flags.HasFlag(AnalysisFlags.Code));
        Assert.IsTrue(item.Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(1, item.CrossReferences.Count);
    }

    [TestMethod]
    public void SessionRejectsMissingSourcePath()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => new AnalysisSession(" ", ReadOnlyMemory<byte>.Empty));
    }

    [TestMethod]
    public async Task BasicAnalysisDecodesMappedEntryPoint()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[0x200],
            [new PeSection(".text", 0x1000, 0x100, 0, 0x100, true)])
        {
            EntryPointRva = 0x1000
        };
        FakeDecoder decoder = new();
        RecordingProgress progress = new();
        BasicAnalysisService service = new(decoder);

        AnalysisResult result = await service.AnalyzeAsync(
            session,
            progress,
            CancellationToken.None);

        Assert.IsTrue(result.Diagnostics.Count == 0);
        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Instruction));
        Assert.AreEqual("Imports", progress.Values[0].Phase);
        Assert.AreEqual("Exports", progress.Values[1].Phase);
        Assert.AreEqual("Strings", progress.Values[2].Phase);
        Assert.AreEqual("Strings", progress.Values[3].Phase);
        Assert.IsTrue(progress.Values.Skip(4).All(value =>
            value.Phase is "VmtScan" or "CodeScan"));
        Assert.IsTrue(progress.Values.Any(value => value.Phase == "VmtScan"));
        Assert.AreEqual("CodeScan", progress.Values[^1].Phase);
        Assert.AreEqual(1, progress.Values[^1].Completed);
        Assert.AreEqual(16384, progress.Values[^1].Total);
    }

    [TestMethod]
    public async Task BasicAnalysisBindsSystemModuleFromKnowledgeBase()
    {
        AnalysisSession session = CreateAnalysisSession();
        session.LoadKnowledgeBase(new TestKnowledgeBase(
            [new KnowledgeBaseModule(7, "System"), new KnowledgeBaseModule(8, "SysUtils")]));
        BasicAnalysisService service = new(new FakeDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual((ushort?)7, session.SystemModuleId);
        Assert.AreEqual(2, session.KnowledgeBaseModules.Count);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReportsMissingSystemModule()
    {
        AnalysisSession session = CreateAnalysisSession();
        session.LoadKnowledgeBase(new TestKnowledgeBase([new KnowledgeBaseModule(8, "SysUtils")]));
        BasicAnalysisService service = new(new FakeDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsNull(session.SystemModuleId);
        CollectionAssert.Contains(
            result.Diagnostics.ToArray(),
            "The loaded Knowledge Base does not contain the System module.");
    }

    [TestMethod]
    public async Task BasicAnalysisIndexesImportsAndExports()
    {
        AnalysisSession session = CreateAnalysisSession();
        session.LoadPeDirectories(
            [
                new PeImportModule("KERNEL32.dll",
                [
                    new PeImportSymbol("CreateFileW", null, 0x3000),
                    new PeImportSymbol(null, 123, 0x3004)
                ])
            ],
            [new PeExport("Run", 1, 0x1000, null)]);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("KERNEL32.dll!CreateFileW", session.Items[0x3000].Name);
        Assert.IsTrue(session.Items[0x3000].Flags.HasFlag(AnalysisFlags.Import));
        Assert.IsTrue(session.Items[0x3000].Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual("KERNEL32.dll!ordinal_123", session.Items[0x3004].Name);
        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Export));
        Assert.AreEqual("Run", session.Items[0x1000].Name);
    }

    [TestMethod]
    public async Task BasicAnalysisResolvesCallTargetsToExportsAndImports()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[16],
            [new PeSection(".text", 0x1000, 16, 0, 16, true)])
        {
            EntryPointRva = 0x1000
        };
        session.LoadPeDirectories(
            [new PeImportModule("KERNEL32.dll", [new PeImportSymbol("CreateFileW", null, 0x2000)])],
            [new PeExport("ExportedRoutine", 1, 0x1010, null)]);
        BasicAnalysisService service = new(new ImportAndExportTargetDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        CrossReference exportReference = session.Items[0x1000].CrossReferences[0];
        Assert.AreEqual((uint)0x1010, exportReference.TargetAddress);
        Assert.AreEqual("ExportedRoutine", exportReference.TargetName);
        CrossReference importReference = session.Items[0x1005].CrossReferences[0];
        Assert.AreEqual((uint)0x2000, importReference.TargetAddress);
        Assert.AreEqual("KERNEL32.dll!CreateFileW", importReference.TargetName);
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x1010).Count);
        Assert.AreEqual(0x1000u, session.GetIncomingCrossReferences(0x1010)[0].SourceAddress);
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x2000).Count);
        Assert.AreEqual(0x1005u, session.GetIncomingCrossReferences(0x2000)[0].SourceAddress);
    }

    [TestMethod]
    public async Task BasicAnalysisFindsPrintableStringsInMappedSections()
    {
        byte[] image = new byte[32];
        Encoding.ASCII.GetBytes("Short\0Hi!\0A&B C\0").CopyTo(image, 0);
        image[31] = 0x90;
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", 0x2000, 32, 0, 32, false)])
        {
            EntryPointRva = 0x201f
        };
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual("Short", session.Items[0x2000].Name);
        Assert.IsTrue(session.Items[0x2000].Flags.HasFlag(AnalysisFlags.String));
        Assert.IsTrue(session.Items[0x2000].Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual("A&B C", session.Items[0x200a].Name);
        Assert.IsFalse(session.Items.ContainsKey(0x2006));
    }

    [TestMethod]
    public async Task BasicAnalysisFindsDelphi2009ManagedStringLiteralsWithoutAsciiDuplicates()
    {
        byte[] image = new byte[0x100];
        WriteUInt16(image, 0, 1200);
        WriteUInt16(image, 2, 2);
        WriteInt32(image, 4, -1);
        WriteInt32(image, 8, 4);
        Encoding.Unicode.GetBytes("Wide").CopyTo(image, 12);

        WriteUInt16(image, 24, 1252);
        WriteUInt16(image, 26, 1);
        WriteInt32(image, 28, -1);
        WriteInt32(image, 32, 4);
        Encoding.Latin1.GetBytes("Ansi").CopyTo(image, 36);
        Encoding.ASCII.GetBytes("Visible").CopyTo(image, 64);
        image[71] = 0;

        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", 0x2000, 0x100, 0, 0x100, false)],
            selectedDelphiVersion: DelphiVersion.Delphi2010);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem wideString = session.Items[0x200c];
        Assert.AreEqual("Wide", wideString.Name);
        Assert.AreEqual(DelphiTypeKind.UnicodeString, wideString.TypeKind);
        Assert.AreEqual((ushort)1200, wideString.StringCodePage);
        Assert.AreEqual((ushort)2, wideString.StringElementSize);
        Assert.IsTrue(wideString.Flags.HasFlag(AnalysisFlags.String));
        Assert.IsFalse(session.Items.ContainsKey(0x200e));

        AnalysisItem ansiString = session.Items[0x2024];
        Assert.AreEqual("Ansi", ansiString.Name);
        Assert.AreEqual(DelphiTypeKind.AnsiString, ansiString.TypeKind);
        Assert.AreEqual((ushort)1252, ansiString.StringCodePage);
        Assert.AreEqual((ushort)1, ansiString.StringElementSize);
        Assert.AreEqual("Visible", session.Items[0x2040].Name);

        AnalysisSession olderVersionSession = new(
            "older.exe",
            image,
            [new PeSection(".rdata", 0x2000, 0x100, 0, 0x100, false)],
            selectedDelphiVersion: DelphiVersion.Delphi2007);
        await service.AnalyzeAsync(olderVersionSession, null, CancellationToken.None);
        Assert.IsFalse(olderVersionSession.Items.ContainsKey(0x200c));
        Assert.IsNull(olderVersionSession.Items[0x2024].TypeKind);

        byte[] malformedImage = (byte[])image.Clone();
        WriteInt32(malformedImage, 28, 0);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [new PeSection(".rdata", 0x2000, 0x100, 0, 0x100, false)],
            selectedDelphiVersion: DelphiVersion.Delphi2010);
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsNull(malformedSession.Items[0x2024].TypeKind);
        Assert.AreEqual("Ansi", malformedSession.Items[0x2024].Name);
    }

    [TestMethod]
    public async Task BasicAnalysisReportsInvalidInstructionAndContinuesWithOtherCodeStarts()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[0x100],
            [
                new PeSection(".text", 0x1000, 0x40, 0, 0x40, true),
                new PeSection(".rdata", 0x2000, 0x40, 0x40, 0x40, false)
            ])
        {
            EntryPointRva = 0x1000
        };
        AnalysisItem alternateStart = session.GetOrAddItem(0x1010);
        alternateStart.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
        BasicAnalysisService service = new(new InvalidInstructionDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        StringAssert.Contains(
            string.Join(Environment.NewLine, result.Diagnostics),
            "Stopped decoding code at RVA 0x00001000");
        Assert.IsTrue(session.DisassemblyLines.All(line => line.Address != 0x1000));
        Assert.IsTrue(result.Session.Items[0x1010].Flags.HasFlag(AnalysisFlags.ProcedureStart));
    }

    [TestMethod]
    [DataRow(0x34, 0x18, 0x1c, 0x20, 0x08, 0x04, 0x0c, 0x14)]
    [DataRow(0x40, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x1c)]
    [DataRow(0x4c, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x1c)]
    [DataRow(0x58, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x1c)]
    public async Task BasicAnalysisFindsVmtAndAssociatedRtti(
        int selfPointerDisplacement,
        int classNameDisplacement,
        int instanceSizeDisplacement,
        int parentDisplacement,
        int typeInfoDisplacement,
        int initializationTableDisplacement,
        int fieldTableDisplacement,
        int dynamicMethodTableDisplacement)
    {
        const uint imageBase = 0x400000;
        const int selfPointerRawOffset = 0x10;
        const int typeInfoRawOffset = 0x170;
        const int classNameRawOffset = 0x180;
        const int fieldTableRawOffset = 0x140;
        const int initializationTableRawOffset = 0x110;
        const int dynamicTableRawOffset = 0x120;
        const int typesTableRawOffset = 0x160;
        const uint sectionRva = 0x2000;
        byte[] image = new byte[0x200];
        uint classVmtRva = sectionRva + selfPointerRawOffset + (uint)selfPointerDisplacement;
        uint typeInfoRva = sectionRva + typeInfoRawOffset;
        uint classNameRva = sectionRva + classNameRawOffset;
        uint parentVmtRva = sectionRva + 0x80;
        WriteUInt32(image, selfPointerRawOffset, imageBase + classVmtRva);
        WriteUInt32(
            image,
            selfPointerRawOffset + classNameDisplacement,
            imageBase + classNameRva);
        WriteUInt32(image, selfPointerRawOffset + instanceSizeDisplacement, 0x20);
        WriteUInt32(
            image,
            selfPointerRawOffset + parentDisplacement,
            imageBase + parentVmtRva);
        WriteUInt32(
            image,
            selfPointerRawOffset + typeInfoDisplacement,
            imageBase + typeInfoRva);
        WriteUInt32(
            image,
            selfPointerRawOffset + fieldTableDisplacement,
            imageBase + sectionRva + (uint)fieldTableRawOffset);
        WriteUInt32(
            image,
            selfPointerRawOffset + initializationTableDisplacement,
            imageBase + sectionRva + (uint)initializationTableRawOffset);
        WriteUInt32(
            image,
            selfPointerRawOffset + dynamicMethodTableDisplacement,
            imageBase + sectionRva + (uint)dynamicTableRawOffset);
        image[typeInfoRawOffset] = (byte)DelphiTypeKind.Integer;
        image[typeInfoRawOffset + 1] = 8;
        Encoding.ASCII.GetBytes("TMyInt32").CopyTo(image, typeInfoRawOffset + 2);
        image[classNameRawOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameRawOffset + 1);
        WriteUInt16(image, fieldTableRawOffset, 1);
        WriteUInt32(image, fieldTableRawOffset + 2, imageBase + sectionRva + (uint)typesTableRawOffset);
        WriteInt32(image, fieldTableRawOffset + 6, -4);
        WriteUInt16(image, fieldTableRawOffset + 10, 0);
        image[fieldTableRawOffset + 12] = 6;
        Encoding.ASCII.GetBytes("FValue").CopyTo(image, fieldTableRawOffset + 13);
        WriteUInt16(image, typesTableRawOffset, 1);
        WriteUInt32(image, typesTableRawOffset + 2, imageBase + typeInfoRva);
        WriteUInt16(image, dynamicTableRawOffset, 0);
        WriteUInt32(image, initializationTableRawOffset + 6, 0);

        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", sectionRva, 0x200, 0, 0x200, false)],
            imageBase);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual("TTestClass", vmt.Name);
        Assert.IsTrue(vmt.Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.IsTrue(vmt.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(0x20U, vmt.ClassInstanceSizeBytes);
        Assert.AreEqual(parentVmtRva, vmt.ParentAddress);
        Assert.IsTrue(session.Items[classNameRva].Flags.HasFlag(AnalysisFlags.Data));
        Assert.IsTrue(session.Items[typeInfoRva].Flags.HasFlag(AnalysisFlags.Rtti));
        Assert.IsTrue(session.Items[typeInfoRva].Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(DelphiTypeKind.Integer, session.Items[typeInfoRva].TypeKind);
        Assert.AreEqual("TMyInt32", session.Items[typeInfoRva].Name);
        Assert.AreEqual(sectionRva + (uint)fieldTableRawOffset, vmt.FieldTableAddress);
        Assert.AreEqual(sectionRva + (uint)initializationTableRawOffset, vmt.InitializationTableAddress);
        Assert.AreEqual(0, vmt.InitializationFields.Count);
        Assert.AreEqual(1, vmt.Fields.Count);
        Assert.AreEqual("FValue", vmt.Fields[0].Name);
        Assert.AreEqual(-4, vmt.Fields[0].Offset);
        Assert.AreEqual(typeInfoRva, vmt.Fields[0].TypeInfoAddress);
        Assert.AreEqual(sectionRva + (uint)dynamicTableRawOffset, vmt.DynamicMethodTableAddress);
        Assert.AreEqual(0, vmt.DynamicMethods.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsExtendedVmtFieldsForDelphi2010AndRejectsMalformedCounts()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int fieldTableOffset = 0x130;
        const int typesTableOffset = 0x160;
        const int typeInfoOffset = 0x170;
        const int classNameOffset = 0x180;
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x4c;
        uint typeInfoRva = dataRva + typeInfoOffset;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 0x14, imageBase + dataRva + (uint)fieldTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        WriteUInt32(image, selfPointerOffset + 0x10, imageBase + typeInfoRva);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        image[typeInfoOffset] = (byte)DelphiTypeKind.Integer;
        image[typeInfoOffset + 1] = 8;
        Encoding.ASCII.GetBytes("TMyInt32").CopyTo(image, typeInfoOffset + 2);

        WriteUInt16(image, fieldTableOffset, 1);
        WriteUInt32(image, fieldTableOffset + 2, imageBase + dataRva + (uint)typesTableOffset);
        WriteInt32(image, fieldTableOffset + 6, -4);
        WriteUInt16(image, fieldTableOffset + 10, 0);
        image[fieldTableOffset + 12] = 6;
        Encoding.ASCII.GetBytes("FCount").CopyTo(image, fieldTableOffset + 13);
        int extendedCountOffset = fieldTableOffset + 19;
        WriteUInt16(image, extendedCountOffset, 1);
        int extendedFieldOffset = extendedCountOffset + 2;
        image[extendedFieldOffset] = 1;
        WriteUInt32(image, extendedFieldOffset + 1, imageBase + typeInfoRva);
        WriteInt32(image, extendedFieldOffset + 5, 8);
        image[extendedFieldOffset + 9] = 6;
        Encoding.ASCII.GetBytes("FValue").CopyTo(image, extendedFieldOffset + 10);
        WriteUInt16(image, extendedFieldOffset + 16, 2);
        WriteUInt16(image, typesTableOffset, 1);
        WriteUInt32(image, typesTableOffset + 2, imageBase + typeInfoRva);

        PeSection[] sections =
        [
            new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
            new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
        ];
        BasicAnalysisService service = new(new ReturnDecoder());
        AnalysisSession session = new(
            "sample.exe",
            image,
            sections,
            imageBase)
        {
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(2, vmt.Fields.Count);
        Assert.AreEqual("FCount", vmt.Fields[0].Name);
        Assert.AreEqual(-4, vmt.Fields[0].Offset);
        Assert.IsFalse(vmt.Fields[0].IsExtended);
        Assert.AreEqual("FValue", vmt.Fields[1].Name);
        Assert.AreEqual(8, vmt.Fields[1].Offset);
        Assert.AreEqual(typeInfoRva, vmt.Fields[1].TypeInfoAddress);
        Assert.IsTrue(vmt.Fields[1].IsExtended);
        Assert.AreEqual((byte)1, vmt.Fields[1].Flags);

        AnalysisSession olderVersionSession = new(
            "older.exe",
            image,
            sections,
            imageBase,
            DelphiVersion.Delphi2009);
        await service.AnalyzeAsync(olderVersionSession, null, CancellationToken.None);
        Assert.AreEqual(1, olderVersionSession.Items[classVmtRva].Fields.Count);

        byte[] malformedImage = (byte[])image.Clone();
        WriteUInt16(malformedImage, fieldTableOffset, ushort.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            sections,
            imageBase)
        {
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsTrue(malformedSession.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedSession.Items[classVmtRva].Fields.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsVmtInterfacesAndDelphiXe2TypeInfoReferences()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int interfaceTableOffset = 0x130;
        const int typeInfoOffset = 0x170;
        const int classNameOffset = 0x180;
        const int vTableOffset = 0x190;
        const int typeInfoReferenceOffset = 0x1a0;
        Guid interfaceId = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x58;
        uint typeInfoRva = dataRva + typeInfoOffset;
        uint vTableRva = dataRva + vTableOffset;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 4, imageBase + dataRva + (uint)interfaceTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x10, imageBase + typeInfoRva);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        image[typeInfoOffset] = (byte)DelphiTypeKind.Interface;
        image[typeInfoOffset + 1] = 10;
        Encoding.ASCII.GetBytes("IMyService").CopyTo(image, typeInfoOffset + 2);
        WriteInt32(image, interfaceTableOffset, 1);
        interfaceId.ToByteArray().CopyTo(image, interfaceTableOffset + 4);
        WriteUInt32(image, interfaceTableOffset + 20, imageBase + vTableRva);
        WriteInt32(image, interfaceTableOffset + 24, -12);
        WriteInt32(image, interfaceTableOffset + 28, 0x1234);
        WriteUInt32(
            image,
            interfaceTableOffset + 32,
            imageBase + dataRva + (uint)typeInfoReferenceOffset);
        WriteUInt32(image, typeInfoReferenceOffset, imageBase + typeInfoRva);

        PeSection[] sections =
        [
            new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
            new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
        ];
        BasicAnalysisService service = new(new ReturnDecoder());
        AnalysisSession session = new(
            "sample.exe",
            image,
            sections,
            imageBase)
        {
            SelectedDelphiVersion = DelphiVersion.DelphiXE2
        };

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(dataRva + interfaceTableOffset, vmt.InterfaceTableAddress);
        Assert.AreEqual(1, vmt.Interfaces.Count);
        Assert.AreEqual(interfaceId, vmt.Interfaces[0].Id);
        Assert.AreEqual(vTableRva, vmt.Interfaces[0].VTableAddress);
        Assert.AreEqual(-12, vmt.Interfaces[0].Offset);
        Assert.AreEqual(0x1234, vmt.Interfaces[0].ImplementationGetter);
        Assert.AreEqual(typeInfoRva, vmt.Interfaces[0].TypeInfoAddress);
        Assert.AreEqual(DelphiTypeKind.Interface, session.Items[typeInfoRva].TypeKind);
        Assert.AreEqual("IMyService", session.Items[typeInfoRva].Name);

        AnalysisSession olderVersionSession = new(
            "older.exe",
            image,
            sections,
            imageBase,
            DelphiVersion.Delphi2010);
        await service.AnalyzeAsync(olderVersionSession, null, CancellationToken.None);
        Assert.IsNull(olderVersionSession.Items[classVmtRva].Interfaces[0].TypeInfoAddress);

        byte[] malformedImage = (byte[])image.Clone();
        WriteInt32(malformedImage, interfaceTableOffset, int.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            sections,
            imageBase)
        {
            SelectedDelphiVersion = DelphiVersion.DelphiXE2
        };
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsTrue(malformedSession.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedSession.Items[classVmtRva].Interfaces.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsDelphi3InterfaceEntriesWithoutImplementationGetter()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int interfaceTableOffset = 0x130;
        const int classNameOffset = 0x180;
        Guid interfaceId = Guid.Parse("ffeeddcc-bbaa-9988-7766-554433221100");
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x40;
        uint vTableRva = dataRva + 0x190;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 4, imageBase + dataRva + (uint)interfaceTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        WriteInt32(image, interfaceTableOffset, 1);
        interfaceId.ToByteArray().CopyTo(image, interfaceTableOffset + 4);
        WriteUInt32(image, interfaceTableOffset + 20, imageBase + vTableRva);
        WriteInt32(image, interfaceTableOffset + 24, 16);

        AnalysisSession session = new(
            "delphi3.exe",
            image,
            [new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false)],
            imageBase,
            DelphiVersion.Delphi3);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        DelphiVmtInterface intf = session.Items[classVmtRva].Interfaces.Single();
        Assert.AreEqual(interfaceId, intf.Id);
        Assert.AreEqual(vTableRva, intf.VTableAddress);
        Assert.AreEqual(16, intf.Offset);
        Assert.IsNull(intf.ImplementationGetter);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsVmtDynamicMethodsAndScansTheirTargets()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int dynamicTableOffset = 0x130;
        const int classNameOffset = 0x180;
        const uint firstMethodRva = 0x3008;
        const uint secondMethodRva = 0x3010;
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x58;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 0x1c, imageBase + dataRva + (uint)dynamicTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        WriteUInt16(image, dynamicTableOffset, 2);
        WriteUInt16(image, dynamicTableOffset + 2, 0x46);
        WriteUInt16(image, dynamicTableOffset + 4, 0x55);
        WriteUInt32(image, dynamicTableOffset + 6, imageBase + firstMethodRva);
        WriteUInt32(image, dynamicTableOffset + 10, imageBase + secondMethodRva);

        AnalysisSession session = new(
            "sample.exe",
            image,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        BasicAnalysisService service = new(new ReturnDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(2, vmt.DynamicMethods.Count);
        Assert.AreEqual((ushort)0x46, vmt.DynamicMethods[0].MessageId);
        Assert.AreEqual(firstMethodRva, vmt.DynamicMethods[0].CodeAddress);
        Assert.AreEqual((ushort)0x55, vmt.DynamicMethods[1].MessageId);
        Assert.AreEqual(secondMethodRva, vmt.DynamicMethods[1].CodeAddress);
        Assert.AreEqual("TTestClass.$Dynamic_0046", session.Items[firstMethodRva].Name);
        Assert.IsTrue(session.Items[firstMethodRva].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == secondMethodRva));

        byte[] malformedImage = (byte[])image.Clone();
        WriteUInt16(malformedImage, dynamicTableOffset, ushort.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsTrue(malformedSession.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedSession.Items[classVmtRva].DynamicMethods.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsVmtAutomatedMethodsAndScansStaticAndVirtualTargets()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int autoTableOffset = 0x130;
        const int classNameOffset = 0x180;
        const int firstMethodNameOffset = 0x1a0;
        const int secondMethodNameOffset = 0x1a8;
        const int parameterListOffset = 0x1b0;
        const int virtualSlotOffset = 0x170;
        const uint staticMethodRva = 0x3008;
        const uint virtualMethodRva = 0x3010;
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x58;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 8, imageBase + dataRva + (uint)autoTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);

        WriteInt32(image, autoTableOffset, 2);
        WriteInt32(image, autoTableOffset + 4, 0x100);
        WriteUInt32(image, autoTableOffset + 8, imageBase + dataRva + (uint)firstMethodNameOffset);
        WriteInt32(image, autoTableOffset + 12, 3);
        WriteUInt32(image, autoTableOffset + 16, imageBase + dataRva + (uint)parameterListOffset);
        WriteUInt32(image, autoTableOffset + 20, imageBase + staticMethodRva);
        WriteInt32(image, autoTableOffset + 24, 0x200);
        WriteUInt32(image, autoTableOffset + 28, imageBase + dataRva + (uint)secondMethodNameOffset);
        WriteInt32(image, autoTableOffset + 32, 8);
        WriteUInt32(image, autoTableOffset + 36, 0);
        WriteUInt32(image, autoTableOffset + 40, 0x108);
        image[firstMethodNameOffset] = 4;
        Encoding.ASCII.GetBytes("Read").CopyTo(image, firstMethodNameOffset + 1);
        image[secondMethodNameOffset] = 7;
        Encoding.ASCII.GetBytes("Execute").CopyTo(image, secondMethodNameOffset + 1);
        image[parameterListOffset] = 0x21;
        image[parameterListOffset + 1] = 2;
        image[parameterListOffset + 2] = 0x10;
        image[parameterListOffset + 3] = 0x20;
        WriteUInt32(image, virtualSlotOffset, imageBase + virtualMethodRva);

        AnalysisSession session = new(
            "sample.exe",
            image,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        BasicAnalysisService service = new(new ReturnDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(dataRva + autoTableOffset, vmt.AutoMethodTableAddress);
        Assert.AreEqual(2, vmt.AutoMethods.Count);
        Assert.AreEqual(0x100, vmt.AutoMethods[0].DispatchId);
        Assert.AreEqual("TTestClass.GetRead", vmt.AutoMethods[0].Name);
        Assert.AreEqual(staticMethodRva, vmt.AutoMethods[0].CodeAddress);
        Assert.IsTrue(vmt.AutoMethods[0].IsMethod);
        Assert.IsFalse(vmt.AutoMethods[0].IsVirtual);
        Assert.AreEqual((byte)0x21, vmt.AutoMethods[0].ReturnType);
        CollectionAssert.AreEqual(new byte[] { 0x10, 0x20 }, vmt.AutoMethods[0].ParameterTypes.ToArray());
        Assert.AreEqual("TTestClass.GetRead", session.Items[staticMethodRva].Name);
        Assert.AreEqual(virtualMethodRva, vmt.AutoMethods[1].CodeAddress);
        Assert.IsTrue(vmt.AutoMethods[1].IsVirtual);
        Assert.IsTrue(session.Items[virtualMethodRva].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == staticMethodRva));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == virtualMethodRva));

        byte[] malformedImage = (byte[])image.Clone();
        WriteInt32(malformedImage, autoTableOffset, int.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsTrue(malformedSession.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedSession.Items[classVmtRva].AutoMethods.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsVmtInitializationFieldReferencesAndOffsets()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int initializationTableOffset = 0x130;
        const int typeInfoOffset = 0x170;
        const int classNameOffset = 0x180;
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x58;
        uint typeInfoRva = dataRva + typeInfoOffset;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 0x0c, imageBase + dataRva + (uint)initializationTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x10, imageBase + typeInfoRva);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        image[typeInfoOffset] = (byte)DelphiTypeKind.Integer;
        image[typeInfoOffset + 1] = 8;
        Encoding.ASCII.GetBytes("TMyInt32").CopyTo(image, typeInfoOffset + 2);

        image[initializationTableOffset] = (byte)DelphiTypeKind.Record;
        image[initializationTableOffset + 1] = 0;
        WriteUInt32(image, initializationTableOffset + 2, 0x20);
        WriteUInt32(image, initializationTableOffset + 6, 1);
        WriteUInt32(image, initializationTableOffset + 10, imageBase + typeInfoRva);
        WriteInt32(image, initializationTableOffset + 14, 0x0c);

        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false)],
            imageBase,
            DelphiVersion.Delphi2010);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(dataRva + initializationTableOffset, vmt.InitializationTableAddress);
        Assert.AreEqual(1, vmt.InitializationFields.Count);
        Assert.AreEqual(typeInfoRva, vmt.InitializationFields[0].TypeInfoAddress);
        Assert.AreEqual(0x0c, vmt.InitializationFields[0].Offset);
        Assert.AreEqual("TMyInt32", session.Items[typeInfoRva].Name);

        byte[] malformedImage = (byte[])image.Clone();
        WriteUInt32(malformedImage, initializationTableOffset + 6, uint.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false)],
            imageBase,
            DelphiVersion.Delphi2010);
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.IsTrue(malformedSession.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedSession.Items[classVmtRva].InitializationFields.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsBoundedVmtVirtualSlotsAndScansTargets()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int classNameOffset = 0x180;
        const int firstSlotOffset = selfPointerOffset + 0x28 + sizeof(uint);
        const int secondSlotOffset = selfPointerOffset + 0x58 + sizeof(uint);
        const uint firstMethodRva = 0x3008;
        const uint secondMethodRva = 0x3010;
        byte[] image = new byte[0x220];
        uint classVmtRva = dataRva + selfPointerOffset + 0x58;
        WriteUInt32(image, selfPointerOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + (uint)classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        WriteUInt32(image, firstSlotOffset, imageBase + firstMethodRva);
        WriteUInt32(image, secondSlotOffset, imageBase + secondMethodRva);

        AnalysisSession session = new(
            "sample.exe",
            image,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000
        };
        BasicAnalysisService service = new(new ReturnDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[classVmtRva];
        Assert.AreEqual(2, vmt.VirtualMethods.Count);
        Assert.AreEqual(-0x2c, vmt.VirtualMethods[0].SlotOffset);
        Assert.AreEqual(firstMethodRva, vmt.VirtualMethods[0].CodeAddress);
        Assert.AreEqual(4, vmt.VirtualMethods[1].SlotOffset);
        Assert.AreEqual(secondMethodRva, vmt.VirtualMethods[1].CodeAddress);
        Assert.AreEqual("TTestClass.$Virtual_FFFFFFD4", session.Items[firstMethodRva].Name);
        Assert.IsTrue(session.Items[secondMethodRva].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == firstMethodRva));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == secondMethodRva));
    }

    [TestMethod]
    public async Task BasicAnalysisReadsVmtMethodsAndScansMethodTargets()
    {
        const uint imageBase = 0x400000;
        const uint dataRva = 0x2000;
        const int selfPointerOffset = 0x10;
        const int methodTableOffset = 0x150;
        const int classNameOffset = 0x180;
        const uint methodRva = 0x3008;
        const uint extendedMethodRva = 0x3010;
        const int extendedMethodRecordOffset = 0x170;
        byte[] image = new byte[0x240];
        uint vmtRva = dataRva + selfPointerOffset + 0x4c;
        WriteUInt32(image, selfPointerOffset, imageBase + vmtRva);
        WriteUInt32(image, selfPointerOffset + 0x18, imageBase + dataRva + methodTableOffset);
        WriteUInt32(image, selfPointerOffset + 0x20, imageBase + dataRva + classNameOffset);
        WriteUInt32(image, selfPointerOffset + 0x24, 0x20);
        image[classNameOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameOffset + 1);
        WriteUInt16(image, methodTableOffset, 1);
        const ushort methodRecordLength = 13;
        WriteUInt16(image, methodTableOffset + 2, methodRecordLength);
        WriteUInt32(image, methodTableOffset + 4, imageBase + methodRva);
        image[methodTableOffset + 8] = 6;
        Encoding.ASCII.GetBytes("DoWork").CopyTo(image, methodTableOffset + 9);
        int extendedMethodCountOffset = methodTableOffset + 2 + methodRecordLength;
        WriteUInt16(image, extendedMethodCountOffset, 1);
        int extendedEntryOffset = extendedMethodCountOffset + 2;
        WriteUInt32(
            image,
            extendedEntryOffset,
            imageBase + dataRva + (uint)extendedMethodRecordOffset);
        WriteUInt16(image, extendedEntryOffset + 4, 1);
        WriteUInt16(image, extendedEntryOffset + 6, 2);
        WriteUInt16(image, extendedMethodRecordOffset, 14);
        WriteUInt32(image, extendedMethodRecordOffset + 2, imageBase + extendedMethodRva);
        image[extendedMethodRecordOffset + 6] = 7;
        Encoding.ASCII.GetBytes("Execute").CopyTo(image, extendedMethodRecordOffset + 7);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        BasicAnalysisService service = new(new ReturnDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem vmt = session.Items[vmtRva];
        Assert.AreEqual(dataRva + methodTableOffset, vmt.MethodTableAddress);
        Assert.AreEqual(2, vmt.Methods.Count);
        Assert.AreEqual("DoWork", vmt.Methods[0].Name);
        Assert.AreEqual(methodRva, vmt.Methods[0].CodeAddress);
        Assert.AreEqual("Execute", vmt.Methods[1].Name);
        Assert.AreEqual(extendedMethodRva, vmt.Methods[1].CodeAddress);
        Assert.IsTrue(vmt.Methods[1].IsExtended);
        Assert.AreEqual((ushort)1, vmt.Methods[1].Flags);
        Assert.AreEqual((ushort)2, vmt.Methods[1].VirtualIndex);
        Assert.AreEqual("TTestClass.DoWork", session.Items[methodRva].Name);
        Assert.AreEqual("TTestClass.Execute", session.Items[extendedMethodRva].Name);
        Assert.IsTrue(session.Items[methodRva].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == methodRva));
        Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == extendedMethodRva));

        AnalysisSession olderVersionSession = new(
            "older.exe",
            image,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase,
            DelphiVersion.Delphi2009)
        {
            EntryPointRva = 0x3000
        };
        await service.AnalyzeAsync(olderVersionSession, null, CancellationToken.None);
        Assert.AreEqual(1, olderVersionSession.Items[vmtRva].Methods.Count);
        Assert.IsFalse(olderVersionSession.Items.ContainsKey(extendedMethodRva));

        byte[] malformedImage = (byte[])image.Clone();
        WriteUInt16(malformedImage, methodTableOffset + 2, ushort.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [
                new PeSection(".rdata", dataRva, 0x200, 0, 0x200, false),
                new PeSection(".text", 0x3000, 0x20, 0x200, 0x20, true)
            ],
            imageBase)
        {
            EntryPointRva = 0x3000,
            SelectedDelphiVersion = DelphiVersion.Delphi2010
        };
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);

        AnalysisItem malformedVmt = malformedSession.Items[vmtRva];
        Assert.IsTrue(malformedVmt.Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual(0, malformedVmt.Methods.Count);
        Assert.IsFalse(malformedSession.Items.ContainsKey(methodRva));
    }

    [TestMethod]
    public async Task BasicAnalysisRejectsMalformedRttiTypeNameWithoutDiscardingVmt()
    {
        const uint imageBase = 0x400000;
        const int selfPointerRawOffset = 0x10;
        const uint sectionRva = 0x2000;
        byte[] image = new byte[0x200];
        uint classVmtRva = sectionRva + selfPointerRawOffset + 0x4c;
        WriteUInt32(image, selfPointerRawOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerRawOffset + 0x10, imageBase + sectionRva + 0x170);
        WriteUInt32(image, selfPointerRawOffset + 0x20, imageBase + sectionRva + 0x180);
        WriteUInt32(image, selfPointerRawOffset + 0x24, 0x20);
        image[0x170] = (byte)DelphiTypeKind.Integer;
        image[0x171] = 3;
        Encoding.ASCII.GetBytes("T..").CopyTo(image, 0x172);
        image[0x180] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, 0x181);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", sectionRva, 0x200, 0, 0x200, false)],
            imageBase);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[classVmtRva].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.IsTrue(session.Items[sectionRva + 0x170].Flags.HasFlag(AnalysisFlags.Rtti));
        Assert.IsNull(session.Items[sectionRva + 0x170].Name);
        Assert.IsNull(session.Items[sectionRva + 0x170].TypeKind);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsClassRttiUnitAndParentType()
    {
        const uint imageBase = 0x400000;
        const uint sectionRva = 0x2000;
        const int selfPointerRawOffset = 0x10;
        const int typeInfoRawOffset = 0x170;
        const int classNameRawOffset = 0x1c0;
        const int parentTypeInfoRawOffset = 0x140;
        const int classVmtRawOffset = selfPointerRawOffset + 0x4c;
        byte[] image = new byte[0x200];
        uint classVmtRva = sectionRva + (uint)classVmtRawOffset;
        uint typeInfoRva = sectionRva + typeInfoRawOffset;
        uint parentTypeInfoRva = sectionRva + parentTypeInfoRawOffset;
        WriteUInt32(image, selfPointerRawOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerRawOffset + 0x10, imageBase + typeInfoRva);
        WriteUInt32(
            image,
            selfPointerRawOffset + 0x20,
            imageBase + sectionRva + classNameRawOffset);
        WriteUInt32(image, selfPointerRawOffset + 0x24, 0x20);
        image[typeInfoRawOffset] = (byte)DelphiTypeKind.Class;
        image[typeInfoRawOffset + 1] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, typeInfoRawOffset + 2);
        int classRttiFieldsOffset = typeInfoRawOffset + 12;
        WriteUInt32(image, classRttiFieldsOffset, imageBase + classVmtRva);
        WriteUInt32(image, classRttiFieldsOffset + 4, imageBase + parentTypeInfoRva);
        WriteUInt16(image, classRttiFieldsOffset + 8, 0);
        image[classRttiFieldsOffset + 10] = 6;
        Encoding.ASCII.GetBytes("System").CopyTo(image, classRttiFieldsOffset + 11);
        int propertyCountOffset = classRttiFieldsOffset + 17;
        WriteUInt16(image, propertyCountOffset, 1);
        int propertyNameOffset = propertyCountOffset + 2 + 26;
        image[propertyNameOffset] = 7;
        Encoding.ASCII.GetBytes("Caption").CopyTo(image, propertyNameOffset + 1);
        image[classNameRawOffset] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, classNameRawOffset + 1);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", sectionRva, 0x200, 0, 0x200, false)],
            imageBase);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem classRtti = session.Items[typeInfoRva];
        Assert.AreEqual(DelphiTypeKind.Class, classRtti.TypeKind);
        Assert.AreEqual("TTestClass", classRtti.Name);
        Assert.AreEqual(classVmtRva, classRtti.ClassVmtAddress);
        Assert.AreEqual(parentTypeInfoRva, classRtti.ParentTypeInfoAddress);
        Assert.AreEqual("System", classRtti.UnitName);
        CollectionAssert.AreEqual(new[] { "Caption" }, classRtti.PropertyNames.ToArray());

        byte[] malformedImage = (byte[])image.Clone();
        WriteUInt16(malformedImage, propertyCountOffset, ushort.MaxValue);
        AnalysisSession malformedSession = new(
            "malformed.exe",
            malformedImage,
            [new PeSection(".rdata", sectionRva, 0x200, 0, 0x200, false)],
            imageBase);
        await service.AnalyzeAsync(malformedSession, null, CancellationToken.None);

        AnalysisItem malformedRtti = malformedSession.Items[typeInfoRva];
        Assert.AreEqual(DelphiTypeKind.Class, malformedRtti.TypeKind);
        Assert.IsNull(malformedRtti.ClassVmtAddress);
        Assert.AreEqual(0, malformedRtti.PropertyNames.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisIgnoresMalformedVmtTypeInfo()
    {
        const uint imageBase = 0x400000;
        const int selfPointerRawOffset = 0x10;
        const uint sectionRva = 0x2000;
        byte[] image = new byte[0x200];
        uint classVmtRva = sectionRva + selfPointerRawOffset + 0x4c;
        WriteUInt32(image, selfPointerRawOffset, imageBase + classVmtRva);
        WriteUInt32(image, selfPointerRawOffset + 0x10, imageBase + sectionRva + 0x170);
        WriteUInt32(image, selfPointerRawOffset + 0x20, imageBase + sectionRva + 0x180);
        WriteUInt32(image, selfPointerRawOffset + 0x24, 0x20);
        image[0x170] = 0xff;
        image[0x180] = 10;
        Encoding.ASCII.GetBytes("TTestClass").CopyTo(image, 0x181);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".rdata", sectionRva, 0x200, 0, 0x200, false)],
            imageBase);
        BasicAnalysisService service = new(new FakeDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(classVmtRva));
    }

    [TestMethod]
    public async Task BasicAnalysisChecksCancellationDuringVmtScan()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[0x4000],
            [new PeSection(".rdata", 0x2000, 0x4000, 0, 0x4000, false)]);
        using CancellationTokenSource cancellation = new();
        BasicAnalysisService service = new(new FakeDecoder());

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => service.AnalyzeAsync(
                session,
                new CancelOnVmtProgress(cancellation),
                cancellation.Token));
    }

    [TestMethod]
    public async Task BasicAnalysisHonorsCancellationDuringRuntimeSymbolScan()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[0x2000],
            [new PeSection(".text", 0x1000, 0x2000, 0, 0x2000, true)]);
        session.LoadKnowledgeBase(new TestKnowledgeBase(
            [new KnowledgeBaseModule(1, "System")],
            [
                new KnowledgeBaseProcedure(
                    1,
                    "@HandleOnException",
                    (byte)'C',
                    new byte[] { 0x55, 0x8b, 0xec, 0xc3 },
                    new byte[4])
            ]));
        using CancellationTokenSource cancellation = new();
        BasicAnalysisService service = new(new FakeDecoder());

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => service.AnalyzeAsync(
                session,
                new CancelOnRuntimeSymbolsProgress(cancellation),
                cancellation.Token));
    }

    [TestMethod]
    public async Task BasicAnalysisRejectsSectionRawDataOutsideImage()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[8],
            [new PeSection(".rdata", 0x2000, 8, 4, 8, false)]);
        BasicAnalysisService service = new(new FakeDecoder());

        await Assert.ThrowsExactlyAsync<InvalidDataException>(
            () => service.AnalyzeAsync(session, null, CancellationToken.None));
    }

    [TestMethod]
    public async Task BasicAnalysisHonorsCancellationDuringStringScan()
    {
        AnalysisSession session = CreateAnalysisSession();
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        BasicAnalysisService service = new(new FakeDecoder());

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => service.AnalyzeAsync(session, null, cancellation.Token));
    }

    [TestMethod]
    public async Task BasicAnalysisDecodesSequentialInstructionsAndClassifiesControlFlow()
    {
        byte[] code = [0xe8, 0x05, 0, 0, 0, 0x74, 0x02, 0x90, 0xc3, 0x90];
        AnalysisSession session = new(
            "sample.exe",
            code,
            [new PeSection(".text", 0x1000, 10, 0, 10, true)])
        {
            EntryPointRva = 0x1000
        };
        RecordingProgress progress = new();
        BasicAnalysisService service = new(new X86TestDecoder());

        await service.AnalyzeAsync(session, progress, CancellationToken.None);

        Assert.AreEqual(5, session.Items.Count);
        Assert.AreEqual(CrossReferenceKind.Call, session.Items[0x1000].CrossReferences[0].Kind);
        Assert.AreEqual(0x100aU, session.Items[0x1000].CrossReferences[0].TargetAddress);
        Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.Call));
        Assert.AreEqual("Call", session.Items[0x1000].Name);
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x100a).Count);
        Assert.IsNull(session.GetIncomingCrossReferences(0x100a)[0].TargetName);
        Assert.AreEqual("E805000000", session.DisassemblyLines[0].Bytes);
        Assert.AreEqual("Call", session.DisassemblyLines[0].Mnemonic);
        Assert.AreEqual((uint?)0x100a, session.DisassemblyLines[0].BranchTarget);
        Assert.AreEqual(CrossReferenceKind.Jump, session.Items[0x1005].CrossReferences[0].Kind);
        Assert.AreEqual(0x1009U, session.Items[0x1005].CrossReferences[0].TargetAddress);
        Assert.AreEqual("Jz", session.Items[0x1005].Name);
        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.ProcedureEnd));
        Assert.AreEqual("Ret", session.Items[0x1008].Name);
        Assert.AreEqual("C3", session.DisassemblyLines[3].Bytes);
        Assert.AreEqual(InstructionFlowControl.Return, session.DisassemblyLines[3].FlowControl);
        Assert.AreEqual("Nop", session.Items[0x1009].Name);
        Assert.IsFalse(session.Items[0x1009].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        AnalysisProgress finalProgress = progress.Values.Last(value => value.Phase == "CodeScan");
        Assert.AreEqual(2, finalProgress.Completed);
        Assert.AreEqual(16384, finalProgress.Total);
    }

    [TestMethod]
    public async Task BasicAnalysisScansDirectCallTargetAsProcedure()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[12],
            [new PeSection(".text", 0x1000, 12, 0, 12, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new DirectProcedureTargetDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.Instruction));
        Assert.IsTrue(session.Items[0x1009].Flags.HasFlag(AnalysisFlags.ProcedureEnd));
        Assert.AreEqual(4, session.DisassemblyLines.Count);
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x1008).Count);
    }

    [TestMethod]
    public async Task BasicAnalysisScansConditionalBranchTargetWithoutMarkingProcedureStart()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[16],
            [new PeSection(".text", 0x1000, 16, 0, 16, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new ConditionalBranchTargetDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.Instruction));
        Assert.IsFalse(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(1, session.GetIncomingCrossReferences(0x1008).Count);
        Assert.AreEqual(3, session.DisassemblyLines.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisScansUnconditionalBranchTargetWithoutFollowingFallThrough()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[16],
            [new PeSection(".text", 0x1000, 16, 0, 16, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new UnconditionalBranchTargetDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.Instruction));
        Assert.IsFalse(session.Items[0x1008].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsFalse(session.Items.ContainsKey(0x1002));
        Assert.AreEqual(2, session.DisassemblyLines.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReadsExceptionHandlersAfterKnownDelphiHandlerJump()
    {
        const uint imageBase = 0x400000;
        byte[] image = new byte[0x40];
        WriteUInt32(image, 5, 1);
        WriteUInt32(image, 9, imageBase + 0x1020);
        WriteUInt32(image, 13, imageBase + 0x1030);
        byte[] handlerSignature = [0x55, 0x11, 0xec, 0x90, 0xc3];
        handlerSignature.CopyTo(image, 0x18);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
            imageBase)
        {
            EntryPointRva = 0x1000
        };
        session.LoadKnowledgeBase(new TestKnowledgeBase(
            [new KnowledgeBaseModule(1, "System")],
            [
                new KnowledgeBaseProcedure(
                    1,
                    "@HandleOnException",
                    (byte)'C',
                    new byte[] { 0x55, 0, 0xec, 0x90, 0xc3 },
                    new byte[] { 0, byte.MaxValue, 0, 0, 0 })
            ]));
        BasicAnalysisService service = new(new ExceptionHandlerBranchDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        AnalysisItem table = session.Items[0x1005];
        Assert.IsTrue(table.Flags.HasFlag(AnalysisFlags.ExceptionTable));
        Assert.IsTrue(table.Flags.HasFlag(AnalysisFlags.Data));
        Assert.AreEqual(1, table.ExceptionHandlers.Count);
        Assert.AreEqual(0x1020U, table.ExceptionHandlers[0].ExceptionInfoAddress);
        Assert.AreEqual(0x1030U, table.ExceptionHandlers[0].ProcedureAddress);
        Assert.AreEqual("@HandleOnException", session.Items[0x1018].Name);
        Assert.IsTrue(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.Items[0x1030].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.AreEqual(3, session.DisassemblyLines.Count);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReportsTruncatedExceptionHandlerTable()
    {
        byte[] image = new byte[0x40];
        WriteUInt32(image, 5, int.MaxValue);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)])
        {
            EntryPointRva = 0x1000
        };
        session.GetOrAddItem(0x1018).Name = "@HandleOnException";
        BasicAnalysisService service = new(new ExceptionHandlerBranchDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x1005));
        CollectionAssert.Contains(
            result.Diagnostics.ToArray(),
            "Exception-handler table at RVA 0x00001005 is invalid or truncated.");
        Assert.AreEqual(2, session.DisassemblyLines.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisDoesNotTreatUnknownJumpDataAsExceptionTable()
    {
        byte[] image = new byte[0x40];
        WriteUInt32(image, 5, 1);
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new ExceptionHandlerBranchDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items.ContainsKey(0x1005));
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisClassifiesKnownDelphiExceptionAndFinallyHandlerJumps()
    {
        (string Name, AnalysisFlags Flag)[] cases =
        [
            ("@HandleOnException", AnalysisFlags.ExceptionHandler),
            ("@HandleAnyException", AnalysisFlags.ExceptionHandler),
            ("@HandleAutoException", AnalysisFlags.ExceptionHandler),
            ("@HandleFinally", AnalysisFlags.FinallyHandler)
        ];

        foreach ((string name, AnalysisFlags expectedFlag) in cases)
        {
            byte[] image = new byte[0x40];
            AnalysisSession session = new(
                "sample.exe",
                image,
                [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)])
            {
                EntryPointRva = 0x1000
            };
            session.GetOrAddItem(0x1018).Name = name;
            BasicAnalysisService service = new(new ExceptionHandlerBranchDecoder());

            AnalysisResult result = await service.AnalyzeAsync(
                session,
                null,
                CancellationToken.None);

            Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(expectedFlag), name);
            Assert.IsTrue(session.Items[0x1018].Flags.HasFlag(expectedFlag), name);
            Assert.AreEqual(0, result.Diagnostics.Count, name);
        }
    }

    [TestMethod]
    public async Task BasicAnalysisReturnsBeforeDecoderFinishes()
    {
        AnalysisSession session = CreateAnalysisSession();
        using ManualResetEventSlim decodeStarted = new();
        using ManualResetEventSlim releaseDecoder = new();
        BlockingDecoder decoder = new(decodeStarted, releaseDecoder);
        BasicAnalysisService service = new(decoder);
        Task<Task<AnalysisResult>> invocation = Task.Factory.StartNew(
            () => service.AnalyzeAsync(session, null, CancellationToken.None));

        try
        {
            Task completed = await Task.WhenAny(invocation, Task.Delay(TimeSpan.FromSeconds(5)));
            Assert.AreSame(invocation, completed);
            Assert.IsTrue(decodeStarted.Wait(TimeSpan.FromSeconds(5)));
        }
        finally
        {
            releaseDecoder.Set();
        }

        await (await invocation);
    }

    [TestMethod]
    public async Task BasicAnalysisReportsProcedureStartLimit()
    {
        const int procedureCount = 16390;
        AnalysisSession session = new(
            "sample.exe",
            new byte[procedureCount * 2],
            [new PeSection(".text", 0x1000, procedureCount * 2u, 0, procedureCount * 2u, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new ManyCallTargetsDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(16384, session.DisassemblyLines.Count / 2);
        CollectionAssert.Contains(
            result.Diagnostics.ToArray(),
            "Code scan stopped after 16384 code starts.");
    }

    [TestMethod]
    public async Task BasicAnalysisDoesNotCountAlreadyDecodedBranchTargetsAgainstStartLimit()
    {
        const int procedureCount = 10000;
        AnalysisSession session = new(
            "sample.exe",
            new byte[procedureCount * 3],
            [new PeSection(".text", 0x1000, procedureCount * 3u, 0, procedureCount * 3u, true)])
        {
            EntryPointRva = 0x1000
        };
        for (uint index = 1; index < procedureCount; index++)
        {
            session.GetOrAddItem(0x1000 + index * 3).SetFlags(AnalysisFlags.ProcedureStart);
        }

        BasicAnalysisService service = new(new AdjacentBranchTargetDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(procedureCount * 2, session.DisassemblyLines.Count);
        Assert.IsFalse(result.Diagnostics.Any(diagnostic =>
            diagnostic.StartsWith("Code scan stopped after", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task BasicAnalysisRejectsInvalidDecodedInstructionLength()
    {
        AnalysisSession session = CreateAnalysisSession();
        BasicAnalysisService service = new(new InvalidLengthDecoder());

        await Assert.ThrowsExactlyAsync<InvalidDataException>(
            () => service.AnalyzeAsync(session, null, CancellationToken.None));
    }

    [TestMethod]
    public async Task BasicAnalysisStopsSequentialDecodeAtUnconditionalBranch()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[] { 0xeb, 0x02, 0x90, 0x90 },
            [new PeSection(".text", 0x1000, 4, 0, 4, true)])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new X86TestDecoder());

        await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(1, session.Items.Count);
        Assert.AreEqual(CrossReferenceKind.Jump, session.Items[0x1000].CrossReferences[0].Kind);
        Assert.AreEqual(0x1004U, session.Items[0x1000].CrossReferences[0].TargetAddress);
        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ProcedureEnd));
    }

    [TestMethod]
    public async Task BasicAnalysisChecksCancellationBetweenDecodedInstructions()
    {
        AnalysisSession session = new(
            "sample.exe",
            new byte[8],
            [new PeSection(".text", 0x1000, 8, 0, 8, true)])
        {
            EntryPointRva = 0x1000
        };
        using CancellationTokenSource cancellation = new();
        BasicAnalysisService service = new(new CancelAfterFirstDecode(cancellation));

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => service.AnalyzeAsync(session, null, cancellation.Token));
        Assert.AreEqual(1, session.Items.Count);
    }

    [TestMethod]
    public async Task BasicAnalysisReportsInstructionLimit()
    {
        const int instructionLimit = 4096;
        AnalysisSession session = new(
            "sample.exe",
            new byte[instructionLimit + 88],
            [
                new PeSection(
                    ".text",
                    0x1000,
                    instructionLimit + 88u,
                    0,
                    instructionLimit + 88u,
                    true)
            ])
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService service = new(new FakeDecoder());

        AnalysisResult result = await service.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.AreEqual(instructionLimit, session.Items.Count);
        CollectionAssert.Contains(
            result.Diagnostics.ToArray(),
            $"Entry-point disassembly stopped after {instructionLimit} instructions.");
    }

    [TestMethod]
    public async Task BasicAnalysisRejectsDecoderAddressMismatch()
    {
        AnalysisSession session = CreateAnalysisSession();
        BasicAnalysisService service = new(new MismatchedAddressDecoder());

        await Assert.ThrowsExactlyAsync<InvalidDataException>(
            () => service.AnalyzeAsync(session, null, CancellationToken.None));
    }

    private static AnalysisSession CreateAnalysisSession()
    {
        return new AnalysisSession(
            "sample.exe",
            new byte[0x200],
            [new PeSection(".text", 0x1000, 0x100, 0, 0x100, true)])
        {
            EntryPointRva = 0x1000
        };
    }

    private static void WriteUInt32(byte[] image, int offset, uint value)
    {
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(
            image.AsSpan(offset, sizeof(uint)),
            value);
    }

    private static void WriteInt32(byte[] image, int offset, int value)
    {
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(
            image.AsSpan(offset, sizeof(int)),
            value);
    }

    private static void WriteUInt16(byte[] image, int offset, ushort value)
    {
        System.Buffers.Binary.BinaryPrimitives.WriteUInt16LittleEndian(
            image.AsSpan(offset, sizeof(ushort)),
            value);
    }

    private sealed class RecordingProgress : IProgress<AnalysisProgress>
    {
        public List<AnalysisProgress> Values { get; } = [];

        public void Report(AnalysisProgress value)
        {
            Values.Add(value);
        }
    }

    private sealed class FakeDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(
            ReadOnlySpan<byte> bytes,
            ulong address,
            int bitness)
        {
            return new DecodedInstruction(address, 1, "Nop", null);
        }
    }

    private sealed class X86TestDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            return bytes[0] switch
            {
                0xe8 => new DecodedInstruction(address, 5, "Call", address + 10, InstructionFlowControl.Call),
                0x74 => new DecodedInstruction(address, 2, "Jz", address + 4, InstructionFlowControl.ConditionalBranch),
                0xeb => new DecodedInstruction(address, 2, "Jmp", address + 4, InstructionFlowControl.UnconditionalBranch),
                0xc3 => new DecodedInstruction(address, 1, "Ret", null, InstructionFlowControl.Return),
                _ => new DecodedInstruction(address, 1, "Nop", null)
            };
        }
    }

    private sealed class InvalidLengthDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness) =>
            new(address, 0, "Invalid", null);
    }

    private sealed class MismatchedAddressDecoder : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness) =>
            new(address + 1, 1, "Invalid", null);
    }

    private sealed class CancelAfterFirstDecode(CancellationTokenSource cancellation) : IInstructionDecoder
    {
        public DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness)
        {
            cancellation.Cancel();
            return new DecodedInstruction(address, 1, "Nop", null);
        }
    }

    private sealed class TestKnowledgeBase(
        IReadOnlyList<KnowledgeBaseModule> modules,
        IReadOnlyList<KnowledgeBaseProcedure>? procedures = null) : IKnowledgeBase
    {
        public uint FormatVersion => 2;

        public IReadOnlyList<KnowledgeBaseModule> Modules { get; } = modules;

        public IReadOnlyList<KnowledgeBaseProcedure> Procedures { get; } =
            procedures ?? Array.Empty<KnowledgeBaseProcedure>();

        public ushort GetModuleId(string moduleName) => 0;

        public string? GetModuleName(ushort moduleId) => null;
    }
}
