using System;
using System.Collections.Generic;

namespace IDR.Core.Models;

public sealed class AnalysisItem
{
    private readonly List<CrossReference> _crossReferences = [];

    public AnalysisItem(uint address)
    {
        Address = address;
    }

    public uint Address { get; }

    public AnalysisFlags Flags { get; private set; }

    public string? Name { get; set; }

    public uint? ParentAddress { get; set; }

    public DelphiTypeKind? TypeKind { get; set; }

    public uint? ClassVmtAddress { get; set; }

    public uint? ClassInstanceSizeBytes { get; set; }

    public uint? ParentTypeInfoAddress { get; set; }

    public string? UnitName { get; set; }

    public ushort? StringCodePage { get; set; }

    public ushort? StringElementSize { get; set; }

    public IReadOnlyList<string> PropertyNames { get; set; } = Array.Empty<string>();

    public uint? MethodTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtMethod> Methods { get; set; } = Array.Empty<DelphiVmtMethod>();

    public uint? FieldTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtField> Fields { get; set; } = Array.Empty<DelphiVmtField>();

    public IReadOnlyList<DelphiRttiRecordField> RecordFields { get; set; } =
        Array.Empty<DelphiRttiRecordField>();

    public uint? RecordSizeBytes { get; set; }

    public IReadOnlyList<MemberAccessCandidate> MemberAccessCandidates { get; set; } =
        Array.Empty<MemberAccessCandidate>();

    public uint? InterfaceTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtInterface> Interfaces { get; set; } = Array.Empty<DelphiVmtInterface>();

    public uint? DynamicMethodTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtDynamicMethod> DynamicMethods { get; set; } =
        Array.Empty<DelphiVmtDynamicMethod>();

    public uint? AutoMethodTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtAutoMethod> AutoMethods { get; set; } =
        Array.Empty<DelphiVmtAutoMethod>();

    public uint? InitializationTableAddress { get; set; }

    public IReadOnlyList<DelphiVmtInitializationField> InitializationFields { get; set; } =
        Array.Empty<DelphiVmtInitializationField>();

    public IReadOnlyList<DelphiVmtVirtualMethod> VirtualMethods { get; set; } =
        Array.Empty<DelphiVmtVirtualMethod>();

    public IReadOnlyList<DelphiExceptionHandler> ExceptionHandlers { get; set; } =
        Array.Empty<DelphiExceptionHandler>();

    public uint? ExceptionRegionHandlerAddress { get; set; }

    public uint? FinallyRegionEndAddress { get; set; }

    public uint? Int64ComparisonEndAddress { get; set; }

    public uint? ProcedureSizeBytes { get; set; }

    public ushort? ReturnStackBytes { get; set; }

    public string? ReturnTypeCandidate { get; set; }

    public string? DataTypeCandidate { get; set; }

    public string? ResourceStringCandidate { get; set; }

    public string? ThreadVariableCandidate { get; set; }

    public bool UsesFramePointer { get; set; }

    public long? StackPointerDeltaBytes { get; set; }

    public IReadOnlyList<StackArgument> StackArguments { get; set; } = Array.Empty<StackArgument>();

    public IReadOnlyList<StackLocalVariable> StackLocalVariables { get; set; } =
        Array.Empty<StackLocalVariable>();

    public IReadOnlyList<string> RegisterArgumentCandidates { get; set; } = Array.Empty<string>();

    public IReadOnlyList<CrossReference> CrossReferences => _crossReferences;

    public void SetFlags(AnalysisFlags flags)
    {
        Flags |= flags;
    }

    public void ClearFlags(AnalysisFlags flags)
    {
        Flags &= ~flags;
    }

    public void AddCrossReference(CrossReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        _crossReferences.Add(reference);
    }
}
