using IDR.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Core.Services;

public interface IPeImageLoader
{
    Task<PeImage> LoadAsync(string sourcePath, CancellationToken cancellationToken);
}

public interface IDelphiVersionDetector
{
    DelphiVersionDetection Detect(PeImage image);
}

public interface IKnowledgeBaseProvider
{
    Task<IKnowledgeBase> OpenAsync(string filePath, CancellationToken cancellationToken);
}

public interface IKnowledgeBasePathResolver
{
    string ResolvePath(DelphiVersion version);
}

public interface IInstructionDecoder
{
    DecodedInstruction Decode(ReadOnlySpan<byte> bytes, ulong address, int bitness);
}

public interface IAnalysisService
{
    Task<AnalysisResult> AnalyzeAsync(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken);
}

public interface IKnowledgeBase
{
    uint FormatVersion { get; }

    IReadOnlyList<KnowledgeBaseModule> Modules { get; }

    IReadOnlyList<KnowledgeBaseProcedure> Procedures { get; }

    ushort GetModuleId(string moduleName);

    string? GetModuleName(ushort moduleId);
}

public sealed record PeImage(
    string SourcePath,
    ReadOnlyMemory<byte> Image,
    IReadOnlyList<PeSection> Sections,
    uint EntryPointRva)
{
    public ulong ImageBase { get; init; }

    public IReadOnlyList<PeImportModule> Imports { get; init; } = [];

    public IReadOnlyList<PeExport> Exports { get; init; } = [];
}

public sealed record PeImportModule(string Name, IReadOnlyList<PeImportSymbol> Symbols);

public sealed record PeImportSymbol(string? Name, ushort? Ordinal, uint AddressRva);

public sealed record PeExport(string? Name, uint Ordinal, uint AddressRva, string? Forwarder);

public enum DelphiVersion
{
    Unknown,
    Delphi2 = 2,
    Delphi3 = 3,
    Delphi4 = 4,
    Delphi5 = 5,
    Delphi6 = 6,
    Delphi7 = 7,
    Delphi2005 = 2005,
    Delphi2006 = 2006,
    Delphi2007 = 2007,
    Delphi2009 = 2009,
    Delphi2010 = 2010,
    DelphiXE1 = 2011,
    DelphiXE2 = 2012,
    DelphiXE3 = 2013,
    DelphiXE4 = 2014
}

public sealed record DelphiVersionDetection(
    DelphiVersion SelectedVersion,
    IReadOnlyList<DelphiVersion> Candidates,
    bool IsAmbiguous);

public sealed record DecodedInstruction(
    ulong Address,
    int Length,
    string Mnemonic,
    ulong? NearBranchTarget,
    InstructionFlowControl FlowControl = InstructionFlowControl.Next,
    string FormattedText = "")
{
    public IReadOnlyList<DecodedOperand> Operands { get; init; } = Array.Empty<DecodedOperand>();
    public bool HasRepeatPrefix { get; init; }
}

public sealed record DecodedOperand(
    DecodedOperandKind Kind,
    string? Register = null,
    string? SegmentRegister = null,
    string? BaseRegister = null,
    string? IndexRegister = null,
    ulong? Displacement = null,
    ulong? Immediate = null,
    int? MemorySizeBytes = null);

public enum DecodedOperandKind
{
    Other,
    Register,
    Immediate,
    Memory,
    NearBranch
}

public sealed record DisassemblyLine(
    uint Address,
    string Bytes,
    string Mnemonic,
    string FormattedText,
    uint? BranchTarget,
    InstructionFlowControl FlowControl);

public enum InstructionFlowControl
{
    Next,
    Call,
    ConditionalBranch,
    UnconditionalBranch,
    IndirectBranch,
    Return,
    Interrupt
}

public sealed record AnalysisProgress(string Phase, int Completed, int Total);

public sealed record AnalysisResult(
    AnalysisSession Session,
    IReadOnlyList<string> Diagnostics);

public sealed record KnowledgeBaseModule(ushort Id, string Name);

public sealed record KnowledgeBaseProcedure(
    ushort ModuleId,
    string Name,
    byte DumpType,
    ReadOnlyMemory<byte> Code,
    ReadOnlyMemory<byte> Relocations);

public static class DelphiRuntimeHandlerSymbols
{
    public static bool IsSupported(string name) =>
        string.Equals(name, "@HandleOnException", StringComparison.OrdinalIgnoreCase)
        || string.Equals(name, "@HandleAnyException", StringComparison.OrdinalIgnoreCase)
        || string.Equals(name, "@HandleAutoException", StringComparison.OrdinalIgnoreCase)
        || string.Equals(name, "@HandleFinally", StringComparison.OrdinalIgnoreCase);
}
