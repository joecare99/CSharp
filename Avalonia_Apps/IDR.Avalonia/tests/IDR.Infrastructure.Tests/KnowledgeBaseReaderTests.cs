using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using IDR.Infrastructure.KnowledgeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Tests;

[TestClass]
public sealed class KnowledgeBaseReaderTests
{
    private sealed class ExceptionHandlerDecoder : IInstructionDecoder
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

    [TestMethod]
    public void ParseReadsHeaderAndModules()
    {
        byte[] data = CreateKnowledgeBase("System", 7);
        KnowledgeBaseReader reader = new();

        KnowledgeBaseData result = reader.Parse(data);

        Assert.AreEqual(2u, result.FormatVersion);
        Assert.AreEqual((ushort)7, result.GetModuleId("system"));
        Assert.AreEqual("System", result.GetModuleName(7));
    }

    [TestMethod]
    public void ParseReadsCodeProcedureNamesAndMaskedSignatures()
    {
        byte[] code = [0x55, 0x8b, 0xec, 0xc3];
        byte[] data = CreateKnowledgeBase("System", 7, "@HandleOnException", code);

        KnowledgeBaseData result = new KnowledgeBaseReader().Parse(data);

        Assert.AreEqual(1, result.Procedures.Count);
        Assert.AreEqual((ushort)7, result.Procedures[0].ModuleId);
        Assert.AreEqual("@HandleOnException", result.Procedures[0].Name);
        CollectionAssert.AreEqual(code, result.Procedures[0].Code.ToArray());
        CollectionAssert.AreEqual(new byte[code.Length], result.Procedures[0].Relocations.ToArray());
    }

    [TestMethod]
    public void ParseRejectsProcedureDumpOutsideItsRecord()
    {
        const string procedureName = "@HandleOnException";
        byte[] data = CreateKnowledgeBase("System", 7, procedureName, [0x55, 0x8b, 0xec, 0xc3]);
        int moduleRecordSize = 4 + Encoding.ASCII.GetByteCount("System") + 1;
        int procedureOffset = 293 + 6 * 8 + 2 * 16 + moduleRecordSize;
        int dumpSizeOffset = procedureOffset + 2 + 2 + Encoding.ASCII.GetByteCount(procedureName) + 1 + 4 + 4 + 3 + 4;
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(dumpSizeOffset), uint.MaxValue);

        Assert.ThrowsExactly<InvalidDataException>(
            () => new KnowledgeBaseReader().Parse(data));
    }

    [TestMethod]
    public void ParseSkipsUnneededProcedureDumpRecords()
    {
        const string procedureName = "TActionBarAccessibility.accDoDefaultAction";
        byte[] data = CreateKnowledgeBase("System", 7, procedureName, [0x55, 0x8b, 0xec, 0xc3]);
        int moduleRecordSize = 4 + Encoding.ASCII.GetByteCount("System") + 1;
        int procedureOffset = 293 + 6 * 8 + 2 * 16 + moduleRecordSize;
        int dumpSizeOffset = procedureOffset + 2 + 2 + Encoding.ASCII.GetByteCount(procedureName) + 1 + 4 + 4 + 3 + 4;
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(dumpSizeOffset), uint.MaxValue);

        KnowledgeBaseData result = new KnowledgeBaseReader().Parse(data);

        Assert.AreEqual((ushort)7, result.GetModuleId("System"));
        Assert.AreEqual(0, result.Procedures.Count);
    }

    [TestMethod]
    public async Task ParsedKnowledgeBaseResolvesStaticExceptionHandlerDuringAnalysis()
    {
        const uint imageBase = 0x400000;
        byte[] code = [0x55, 0x8b, 0xec, 0xc3];
        byte[] image = new byte[0x40];
        WriteUInt32(image, 5, 1);
        WriteUInt32(image, 9, imageBase + 0x1020);
        WriteUInt32(image, 13, imageBase + 0x1030);
        code.CopyTo(image, 0x18);
        KnowledgeBaseData knowledgeBase = new KnowledgeBaseReader()
            .Parse(CreateKnowledgeBase("System", 7, "@HandleOnException", code));
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
            imageBase)
        {
            EntryPointRva = 0x1000
        };
        session.LoadKnowledgeBase(knowledgeBase);
        BasicAnalysisService analysisService = new(new ExceptionHandlerDecoder());

        AnalysisResult result = await analysisService.AnalyzeAsync(
            session,
            null,
            CancellationToken.None);

        Assert.AreEqual("@HandleOnException", session.Items[0x1018].Name);
        Assert.IsTrue(session.Items[0x1018].Flags.HasFlag(AnalysisFlags.ProcedureStart));
        Assert.IsTrue(session.Items[0x1005].Flags.HasFlag(AnalysisFlags.ExceptionTable));
        Assert.AreEqual(0x1030U, session.Items[0x1005].ExceptionHandlers[0].ProcedureAddress);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    [TestMethod]
    public async Task AnalysisFindsBothDelphiTryRegistrationPrologLayouts()
    {
        byte[][] prologs =
        [
            [
                0x33, 0xc0, 0x55,
                0x68, 0x20, 0x10, 0x40, 0x00,
                0x64, 0xff, 0x30,
                0x64, 0x89, 0x20,
                0xc3
            ],
            [
                0x55,
                0x68, 0x20, 0x10, 0x40, 0x00,
                0x64, 0xff, 0x35, 0x00, 0x00, 0x00, 0x00,
                0x64, 0x89, 0x25, 0x00, 0x00, 0x00, 0x00,
                0xc3
            ]
        ];

        foreach (byte[] prolog in prologs)
        {
            byte[] image = new byte[0x40];
            prolog.CopyTo(image, 0);
            image[0x20] = 0xc3;
            AnalysisSession session = new(
                "sample.exe",
                image,
                [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
                0x400000)
            {
                EntryPointRva = 0x1000
            };
            BasicAnalysisService analysisService = new(new IcedInstructionDecoder());

            AnalysisResult result = await analysisService.AnalyzeAsync(
                session,
                null,
                CancellationToken.None);

            Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ExceptionRegion));
            Assert.AreEqual(0x1020U, session.Items[0x1000].ExceptionRegionHandlerAddress);
            Assert.IsTrue(session.Items[0x1020].Flags.HasFlag(AnalysisFlags.ExceptionHandler));
            Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == 0x1020));
            Assert.AreEqual(0, result.Diagnostics.Count);
        }
    }

    [TestMethod]
    public async Task AnalysisFindsVmtStoredInExecutableSection()
    {
        byte[] image = new byte[0x200];
        WriteUInt32(image, 0, 0x401040);
        WriteUInt32(image, 0x14, 0x4010c0);
        WriteUInt32(image, 0x20, 0x401080);
        WriteUInt32(image, 0x24, 0x40);
        image[0x80] = 5;
        Encoding.ASCII.GetBytes("TForm").CopyTo(image, 0x81);
        WriteUInt16(image, 0xc0, 2);
        WriteUInt32(image, 0xc2, 0x401130);
        WriteUInt32(image, 0xc6, 8);
        WriteUInt16(image, 0xca, 0);
        image[0xcc] = 5;
        Encoding.ASCII.GetBytes("Count").CopyTo(image, 0xcd);
        WriteUInt32(image, 0xd2, 12);
        WriteUInt16(image, 0xd6, 1);
        image[0xd8] = 5;
        Encoding.ASCII.GetBytes("Point").CopyTo(image, 0xd9);
        WriteUInt16(image, 0xde, 0);
        WriteUInt16(image, 0x130, 2);
        WriteUInt32(image, 0x132, 0x4010f0);
        WriteUInt32(image, 0x136, 0x401140);
        image[0xf0] = (byte)DelphiTypeKind.Integer;
        image[0xf1] = 7;
        Encoding.ASCII.GetBytes("Integer").CopyTo(image, 0xf2);
        image[0x140] = (byte)DelphiTypeKind.Record;
        image[0x141] = 6;
        Encoding.ASCII.GetBytes("TPoint").CopyTo(image, 0x142);
        WriteUInt32(image, 0x148, 8);
        WriteUInt32(image, 0x14c, 2);
        WriteUInt32(image, 0x150, 0x4010f0);
        WriteUInt32(image, 0x154, 0);
        WriteUInt32(image, 0x158, 0x401120);
        WriteUInt32(image, 0x15c, 4);
        image[0x160] = 0;
        WriteUInt32(image, 0x161, 2);
        WriteUInt32(image, 0x165, 0x4010f0);
        WriteUInt32(image, 0x169, 0);
        image[0x16d] = 0;
        image[0x16e] = 1;
        image[0x16f] = (byte)'X';
        WriteUInt16(image, 0x170, sizeof(ushort));
        WriteUInt32(image, 0x172, 0x401120);
        WriteUInt32(image, 0x176, 4);
        image[0x17a] = 0;
        image[0x17b] = 1;
        image[0x17c] = (byte)'Y';
        WriteUInt16(image, 0x17d, sizeof(ushort));
        image[0x120] = (byte)DelphiTypeKind.Integer;
        image[0x121] = 7;
        Encoding.ASCII.GetBytes("Integer").CopyTo(image, 0x122);
        image[0x1f0] = 0xc3;
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x200, 0, 0x200, true)],
            0x400000)
        {
            EntryPointRva = 0x11f0
        };

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsTrue(session.Items[0x1040].Flags.HasFlag(AnalysisFlags.Vmt));
        Assert.AreEqual("TForm", session.Items[0x1040].Name);
        Assert.AreEqual("Count", session.Items[0x1040].Fields[0].Name);
        Assert.AreEqual("Point", session.Items[0x1040].Fields[1].Name);
        Assert.IsTrue(session.Items[0x10f0].Flags.HasFlag(AnalysisFlags.Rtti));
        Assert.AreEqual(DelphiTypeKind.Integer, session.Items[0x10f0].TypeKind);
        Assert.AreEqual("Integer", session.Items[0x10f0].Name);
        Assert.AreEqual("TPoint", session.Items[0x1140].Name);
        Assert.AreEqual(2, session.Items[0x1140].RecordFields.Count);
        Assert.AreEqual(8U, session.Items[0x1140].RecordSizeBytes);
        Assert.AreEqual("f4", session.Items[0x1140].RecordFields[1].Name);
        Assert.IsTrue(session.Items[0x1120].Flags.HasFlag(AnalysisFlags.Rtti));

        AnalysisSession modernSession = new(
            "modern-record.exe",
            image,
            [new PeSection(".text", 0x1000, 0x200, 0, 0x200, true)],
            0x400000,
            DelphiVersion.Delphi2010)
        {
            EntryPointRva = 0x11f0
        };
        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(modernSession, null, CancellationToken.None);
        CollectionAssert.AreEqual(
            new[] { "X", "Y" },
            modernSession.Items[0x1140].RecordFields.Select(field => field.Name).ToArray());

        WriteUInt16(image, 0x170, ushort.MaxValue);
        AnalysisSession malformedExtendedSession = new(
            "malformed-extended-record.exe",
            image,
            [new PeSection(".text", 0x1000, 0x200, 0, 0x200, true)],
            0x400000,
            DelphiVersion.Delphi2010)
        {
            EntryPointRva = 0x11f0
        };
        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(malformedExtendedSession, null, CancellationToken.None);
        CollectionAssert.AreEqual(
            new[] { "f0", "f4" },
            malformedExtendedSession.Items[0x1140].RecordFields.Select(field => field.Name).ToArray());

        WriteUInt32(image, 0x15c, 8);
        AnalysisSession malformedSession = new(
            "malformed-record.exe",
            image,
            [new PeSection(".text", 0x1000, 0x200, 0, 0x200, true)],
            0x400000)
        {
            EntryPointRva = 0x11f0
        };
        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(malformedSession, null, CancellationToken.None);
        Assert.AreEqual(0, malformedSession.Items[0x1140].RecordFields.Count);
    }

    [TestMethod]
    public async Task AnalysisDoesNotClassifyNearMissAsDelphiTryRegistration()
    {
        byte[] image =
        [
            0x33, 0xd8,
            0x55,
            0x68, 0x20, 0x10, 0x40, 0x00,
            0x64, 0xff, 0x30,
            0x64, 0x89, 0x20,
            0xc3
        ];
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, (uint)image.Length, 0, (uint)image.Length, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };
        BasicAnalysisService analysisService = new(new IcedInstructionDecoder());

        await analysisService.AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.ExceptionRegion));
        Assert.IsNull(session.Items[0x1000].ExceptionRegionHandlerAddress);
        Assert.IsFalse(session.Items.ContainsKey(0x1020));
    }

    [TestMethod]
    public async Task AnalysisFindsBothDelphiFinallyCleanupLayouts()
    {
        byte[][] cleanups =
        [
            [
                0x33, 0xc0,
                0x59, 0x5a, 0x5b,
                0x64, 0x89, 0x08,
                0x68, 0x30, 0x10, 0x40, 0x00,
                0xc3
            ],
            [
                0x33, 0xc0,
                0x59, 0x5a, 0x5b,
                0x64, 0x89, 0x08,
                0xeb, 0x16
            ]
        ];
        uint[] expectedEnds = [0x1030, 0x1020];

        for (int index = 0; index < cleanups.Length; index++)
        {
            byte[] image = new byte[0x40];
            cleanups[index].CopyTo(image, 0);
            image[(int)(expectedEnds[index] - 0x1000)] = 0xc3;
            AnalysisSession session = new(
                "sample.exe",
                image,
                [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
                0x400000)
            {
                EntryPointRva = 0x1000
            };

            AnalysisResult result = await new BasicAnalysisService(new IcedInstructionDecoder())
                .AnalyzeAsync(session, null, CancellationToken.None);

            Assert.IsTrue(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.FinallyRegion));
            Assert.AreEqual(expectedEnds[index], session.Items[0x1000].FinallyRegionEndAddress);
            if (index == 1)
            {
                Assert.IsTrue(session.DisassemblyLines.Any(line => line.Address == expectedEnds[index]));
            }

            Assert.AreEqual(0, result.Diagnostics.Count);
        }
    }

    [TestMethod]
    public async Task AnalysisDoesNotClassifyFinallyCleanupWithNonFsRestore()
    {
        byte[] cleanup =
        [
            0x33, 0xc0,
            0x59, 0x5a, 0x5b,
            0x65, 0x89, 0x08,
            0x68, 0x30, 0x10, 0x40, 0x00,
            0xc3
        ];
        byte[] image = new byte[0x40];
        cleanup.CopyTo(image, 0);
        image[0x30] = 0xc3;
        AnalysisSession session = new(
            "sample.exe",
            image,
            [new PeSection(".text", 0x1000, 0x40, 0, 0x40, true)],
            0x400000)
        {
            EntryPointRva = 0x1000
        };

        await new BasicAnalysisService(new IcedInstructionDecoder())
            .AnalyzeAsync(session, null, CancellationToken.None);

        Assert.IsFalse(session.Items[0x1000].Flags.HasFlag(AnalysisFlags.FinallyRegion));
        Assert.IsNull(session.Items[0x1000].FinallyRegionEndAddress);
    }

    [TestMethod]
    public void ParseRejectsUnsupportedSignature()
    {
        byte[] data = CreateKnowledgeBase("System", 7);
        data[0] = (byte)'X';

        Assert.ThrowsExactly<InvalidDataException>(
            () => new KnowledgeBaseReader().Parse(data));
    }

    [TestMethod]
    public async Task OpenAsyncReadsKnowledgeBaseFromPath()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bin");
        await File.WriteAllBytesAsync(filePath, CreateKnowledgeBase("System", 7));

        try
        {
            IKnowledgeBase result = await new KnowledgeBaseReader()
                .OpenAsync(filePath, CancellationToken.None);

            Assert.AreEqual((ushort)7, result.GetModuleId("System"));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public async Task OpenAsyncRejectsMissingFile()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bin");

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(
            () => new KnowledgeBaseReader().OpenAsync(filePath, CancellationToken.None));
    }

    [TestMethod]
    public async Task OpenAsyncRejectsEmptyPath()
    {
        await Assert.ThrowsExactlyAsync<ArgumentException>(
            () => new KnowledgeBaseReader().OpenAsync(" ", CancellationToken.None));
    }

    private static byte[] CreateKnowledgeBase(
        string moduleName,
        ushort moduleId,
        string? procedureName = null,
        byte[]? procedureCode = null)
    {
        const int headerSize = 293;
        byte[] record = new byte[4 + Encoding.ASCII.GetByteCount(moduleName) + 1];
        BitConverter.GetBytes(moduleId).CopyTo(record, 0);
        Encoding.ASCII.GetBytes(moduleName).CopyTo(record, 4);
        int sectionsOffset = headerSize;
        byte[]? procedureRecord = procedureName is null || procedureCode is null
            ? null
            : CreateProcedureRecord(moduleId, procedureName, procedureCode);
        int recordOffset = sectionsOffset + 6 * 8 + (procedureRecord is null ? 16 : 32);
        int procedureOffset = recordOffset + record.Length;
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream, Encoding.ASCII, leaveOpen: true);

        byte[] signature = new byte[24];
        Encoding.ASCII.GetBytes("IDR Knowledge Base File").CopyTo(signature, 0);
        writer.Write(signature);
        writer.Write(false);
        writer.Write(0);
        writer.Write(0u);
        writer.Write(new byte[256]);
        writer.Write(2u);
        writer.Write(1);
        writer.Write(record.Length);
        WriteSectionOffset(writer, recordOffset, record.Length);
        WriteEmptySection(writer);
        WriteEmptySection(writer);
        WriteEmptySection(writer);
        WriteEmptySection(writer);
        writer.Write(procedureRecord is null ? 0 : 1);
        writer.Write(procedureRecord?.Length ?? 0);
        if (procedureRecord is not null)
        {
            WriteSectionOffset(writer, procedureOffset, procedureRecord.Length);
        }

        writer.Write(record);
        if (procedureRecord is not null)
        {
            writer.Write(procedureRecord);
        }

        writer.Write(sectionsOffset);

        return stream.ToArray();
    }

    private static byte[] CreateProcedureRecord(ushort moduleId, string name, byte[] code)
    {
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream, Encoding.ASCII, leaveOpen: true);
        writer.Write(moduleId);
        WritePascalCString(writer, name);
        writer.Write((byte)0);
        writer.Write((byte)'C');
        writer.Write((byte)'P');
        writer.Write((byte)0);
        writer.Write(0);
        WritePascalCString(writer, string.Empty);
        writer.Write((uint)(2 * code.Length));
        writer.Write((uint)code.Length);
        writer.Write(0u);
        writer.Write(code);
        writer.Write(new byte[code.Length]);
        writer.Write(2u);
        writer.Write((ushort)0);
        return stream.ToArray();
    }

    private static void WritePascalCString(BinaryWriter writer, string value)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(value);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
        writer.Write((byte)0);
    }

    private static void WriteSectionOffset(BinaryWriter writer, int offset, int size)
    {
        writer.Write(offset);
        writer.Write(size);
        writer.Write(0);
        writer.Write(0);
    }

    private static void WriteUInt32(byte[] image, int offset, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(offset, sizeof(uint)), value);
    }

    private static void WriteUInt16(byte[] image, int offset, ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(offset, sizeof(ushort)), value);
    }

    private static void WriteEmptySection(BinaryWriter writer)
    {
        writer.Write(0);
        writer.Write(0);
    }
}
