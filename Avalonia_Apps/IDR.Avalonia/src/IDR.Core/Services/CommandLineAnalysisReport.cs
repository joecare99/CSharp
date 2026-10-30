using IDR.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDR.Core.Services;

public static class CommandLineAnalysisReport
{
    public static CommandLineAnalysisOutput Create(
        AnalysisSession session,
        IKnowledgeBase knowledgeBase,
        string knowledgeBasePath,
        DelphiVersionDetection detection,
        AnalysisResult result,
        IReadOnlyList<DelphiForm> forms,
        IReadOnlyList<string> formDiagnostics,
        bool summaryOnly)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(knowledgeBase);
        ArgumentNullException.ThrowIfNull(knowledgeBasePath);
        ArgumentNullException.ThrowIfNull(detection);
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(forms);
        ArgumentNullException.ThrowIfNull(formDiagnostics);

        AnalysisItem[] items = session.Items.Values.OrderBy(item => item.Address).ToArray();
        return new CommandLineAnalysisOutput(
            1,
            session.SourcePath,
            session.ImageBase,
            session.EntryPointRva,
            session.SelectedDelphiVersion.ToString(),
            detection.Candidates.Select(candidate => candidate.ToString()).ToArray(),
            knowledgeBasePath,
            knowledgeBase.Modules.Count,
            knowledgeBase.Procedures.Count,
            session.SystemModuleId,
            items.Length,
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.Vmt)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.Rtti)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.String)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.ProcedureStart)),
            session.DisassemblyLines.Count,
            forms.Count,
            summaryOnly ? [] : forms.ToArray(),
            summaryOnly ? [] : items.Select(item => new CommandLineAnalysisItem(
                item.Address,
                $"0x{item.Address:X8}",
                $"0x{session.ImageBase + item.Address:X8}",
                item.Flags.ToString(),
                item.Name,
                item.TypeKind?.ToString(),
                item.RecordSizeBytes,
                item.ClassInstanceSizeBytes,
                item.ParentAddress,
                item.ExceptionRegionHandlerAddress,
                item.FinallyRegionEndAddress,
                item.Int64ComparisonEndAddress,
                item.ProcedureSizeBytes,
                item.ReturnStackBytes,
                item.ReturnTypeCandidate,
                item.DataTypeCandidate,
                item.ResourceStringCandidate,
                item.ThreadVariableCandidate,
                item.UsesFramePointer,
                item.StackPointerDeltaBytes,
                item.StackArguments.ToArray(),
                item.StackLocalVariables.ToArray(),
                item.RegisterArgumentCandidates.ToArray(),
                item.CrossReferences.Select(reference => new CommandLineCrossReference(
                    reference.TargetAddress,
                    $"0x{reference.TargetAddress:X8}",
                    reference.Kind.ToString(),
                    reference.TargetName)).ToArray(),
                session.GetIncomingCrossReferences(item.Address).Select(reference =>
                    new CommandLineCrossReference(
                        reference.SourceAddress,
                        $"0x{reference.SourceAddress:X8}",
                        reference.Kind.ToString(),
                        reference.TargetName)).ToArray(),
                item.ExceptionHandlers.ToArray(),
                item.Methods.ToArray(),
                item.Fields.ToArray(),
                item.RecordFields.ToArray(),
                item.MemberAccessCandidates.ToArray(),
                item.Interfaces.ToArray(),
                item.VirtualMethods.ToArray())).ToArray(),
            summaryOnly ? [] : session.DisassemblyLines.Select(line => new CommandLineDisassemblyLine(
                line.Address,
                $"0x{line.Address:X8}",
                line.Bytes,
                line.Mnemonic,
                line.FormattedText,
                line.BranchTarget,
                line.FlowControl.ToString())).ToArray(),
            formDiagnostics.Concat(result.Diagnostics).ToArray());
    }
}

public sealed record CommandLineAnalysisOutput(
    int SchemaVersion,
    string Input,
    ulong ImageBase,
    uint EntryPointRva,
    string DelphiVersion,
    string[] DetectedVersionCandidates,
    string KnowledgeBase,
    int KnowledgeBaseModuleCount,
    int KnowledgeBaseProcedureCount,
    ushort? SystemModuleId,
    int ItemCount,
    int VmtCount,
    int RttiCount,
    int StringCount,
    int ProcedureCount,
    int DisassemblyLineCount,
    int FormCount,
    DelphiForm[] Forms,
    CommandLineAnalysisItem[] Items,
    CommandLineDisassemblyLine[] Disassembly,
    string[] Diagnostics);

public sealed record CommandLineAnalysisItem(
    uint AddressRva,
    string Rva,
    string Va,
    string Flags,
    string? Name,
    string? TypeKind,
    uint? RecordSizeBytes,
    uint? ClassInstanceSizeBytes,
    uint? ParentRva,
    uint? ExceptionRegionHandlerRva,
    uint? FinallyRegionEndRva,
    uint? Int64ComparisonEndRva,
    uint? ProcedureSizeBytes,
    ushort? ReturnStackBytes,
    string? ReturnTypeCandidate,
    string? DataTypeCandidate,
    string? ResourceStringCandidate,
    string? ThreadVariableCandidate,
    bool UsesFramePointer,
    long? StackPointerDeltaBytes,
    StackArgument[] StackArguments,
    StackLocalVariable[] StackLocalVariables,
    string[] RegisterArgumentCandidates,
    CommandLineCrossReference[] OutgoingCrossReferences,
    CommandLineCrossReference[] IncomingCrossReferences,
    DelphiExceptionHandler[] ExceptionHandlers,
    DelphiVmtMethod[] Methods,
    DelphiVmtField[] Fields,
    DelphiRttiRecordField[] RecordFields,
    MemberAccessCandidate[] MemberAccessCandidates,
    DelphiVmtInterface[] Interfaces,
    DelphiVmtVirtualMethod[] VirtualMethods);

public sealed record CommandLineCrossReference(
    uint AddressRva,
    string Rva,
    string Kind,
    string? Name);

public sealed record CommandLineDisassemblyLine(
    uint AddressRva,
    string Rva,
    string Bytes,
    string Mnemonic,
    string FormattedText,
    uint? BranchTargetRva,
    string FlowControl);
