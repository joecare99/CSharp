using IDR.Core.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Core.Services;

public sealed class BasicAnalysisService : IAnalysisService
{
    private const int MaximumInstructionsPerProcedure = 4096;
    private const int MaximumCodeStarts = 16384;
    private const int MaximumMetadataEntryCount = 10000;
    private const int MaximumKnowledgeBaseSignatureSize = 512;
    private const int MaximumSwitchTableEntryCount = 4096;
    private static readonly VmtLayout[] VmtLayouts =
    [
        new(0x34, 0x18, 0x1c, 0x20, 0x08, 0x04, 0x0c, 0x10, 0x14, null, null, false),
        new(0x40, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x18, 0x1c, 0x08, 0x04, false),
        new(0x4c, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x18, 0x1c, 0x08, 0x04, true),
        new(0x58, 0x20, 0x24, 0x28, 0x10, 0x0c, 0x14, 0x18, 0x1c, 0x08, 0x04, true)
    ];
    private readonly IInstructionDecoder _instructionDecoder;

    public BasicAnalysisService(IInstructionDecoder instructionDecoder)
    {
        _instructionDecoder = instructionDecoder
            ?? throw new ArgumentNullException(nameof(instructionDecoder));
    }

    public Task<AnalysisResult> AnalyzeAsync(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.Run(
            () => AnalyzeCore(session, progress, cancellationToken),
            cancellationToken);
    }

    private AnalysisResult AnalyzeCore(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        List<string> diagnostics = [];
        if (session.HasKnowledgeBase)
        {
            progress?.Report(new AnalysisProgress("KnowledgeBase", 0, 1));
            KnowledgeBaseModule? systemModule = null;
            foreach (KnowledgeBaseModule module in session.KnowledgeBaseModules)
            {
                if (string.Equals(module.Name, "System", StringComparison.OrdinalIgnoreCase))
                {
                    systemModule = module;
                    break;
                }
            }

            if (systemModule is null)
            {
                diagnostics.Add("The loaded Knowledge Base does not contain the System module.");
            }
            else
            {
                session.SetSystemModuleId(systemModule.Id);
            }

            progress?.Report(new AnalysisProgress("KnowledgeBase", 1, 1));
        }

        AnalyzeImports(session, progress, cancellationToken);
        AnalyzeExports(session, progress, cancellationToken);
        AnalyzeStrings(session, progress, cancellationToken);
        AnalyzeVmts(session, progress, cancellationToken);
        AnalyzeRtti(session, progress, cancellationToken);
        AnalyzeKnowledgeBaseHandlers(session, progress, cancellationToken);
        AnalyzeCode(session, progress, cancellationToken, diagnostics);
        return new AnalysisResult(session, diagnostics);
    }

    private static void AnalyzeKnowledgeBaseHandlers(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        KnowledgeBaseProcedure[] procedures = session.KnowledgeBaseProcedures
            .Where(procedure => DelphiRuntimeHandlerSymbols.IsSupported(procedure.Name))
            .DistinctBy(procedure => (
                procedure.Name.ToUpperInvariant(),
                Convert.ToHexString(procedure.Code.Span),
                Convert.ToHexString(procedure.Relocations.Span)))
            .ToArray();
        if (procedures.Length == 0)
        {
            return;
        }

        ReadOnlySpan<byte> image = session.Image.Span;
        progress?.Report(new AnalysisProgress("RuntimeSymbols", 0, procedures.Length));
        for (int index = 0; index < procedures.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            KnowledgeBaseProcedure procedure = procedures[index];
            ReadOnlySpan<byte> signature = procedure.Code.Span;
            ReadOnlySpan<byte> relocations = procedure.Relocations.Span;
            if (signature.Length is >= 4 and <= MaximumKnowledgeBaseSignatureSize
                && relocations.Length == signature.Length
                && signature.Length - CountWildcardBytes(relocations) >= 4)
            {
                foreach (PeSection section in session.Sections)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!section.ContainsCode)
                    {
                        continue;
                    }

                    ulong rawEnd = (ulong)section.RawAddress + section.RawSize;
                    if (rawEnd > (ulong)image.Length)
                    {
                        throw new InvalidDataException(
                            $"Code section '{section.Name}' extends beyond the image's file data.");
                    }

                    int sectionLength = checked((int)Math.Min(
                        (ulong)section.RawSize,
                        section.VirtualSize));
                    if (sectionLength < signature.Length)
                    {
                        continue;
                    }

                    int lastRawOffset = checked(
                        (int)section.RawAddress + sectionLength - signature.Length);
                    int anchorOffset = FindFirstFixedByte(relocations);
                    for (int rawOffset = checked((int)section.RawAddress);
                        rawOffset <= lastRawOffset;
                        rawOffset++)
                    {
                        if ((rawOffset & 0xFFF) == 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        if (image[rawOffset + anchorOffset] != signature[anchorOffset]
                            || !MatchesKnowledgeBaseSignature(
                                session,
                                section,
                                image,
                                rawOffset,
                                signature,
                                relocations))
                        {
                            continue;
                        }

                        uint procedureRva = checked(
                            section.VirtualAddress + (uint)(rawOffset - section.RawAddress));
                        AnalysisItem item = session.GetOrAddItem(procedureRva);
                        item.SetFlags(AnalysisFlags.ProcedureStart);
                        item.Name ??= procedure.Name;
                    }
                }
            }

            progress?.Report(new AnalysisProgress("RuntimeSymbols", index + 1, procedures.Length));
        }
    }

    private static int CountWildcardBytes(ReadOnlySpan<byte> relocations)
    {
        int wildcardCount = 0;
        foreach (byte relocation in relocations)
        {
            if (relocation == byte.MaxValue)
            {
                wildcardCount++;
            }
        }

        return wildcardCount;
    }

    private static int FindFirstFixedByte(ReadOnlySpan<byte> relocations)
    {
        for (int index = 0; index < relocations.Length; index++)
        {
            if (relocations[index] != byte.MaxValue)
            {
                return index;
            }
        }

        return 0;
    }

    private static bool MatchesKnowledgeBaseSignature(
        AnalysisSession session,
        PeSection section,
        ReadOnlySpan<byte> image,
        int rawOffset,
        ReadOnlySpan<byte> signature,
        ReadOnlySpan<byte> relocations)
    {
        uint rva = checked(section.VirtualAddress + (uint)(rawOffset - section.RawAddress));
        for (int index = 0; index < signature.Length; index++)
        {
            if (relocations[index] != byte.MaxValue && image[rawOffset + index] != signature[index])
            {
                return false;
            }

            if (session.Items.TryGetValue(checked(rva + (uint)index), out AnalysisItem? item)
                && (item.Flags.HasFlag(AnalysisFlags.Code) || item.Flags.HasFlag(AnalysisFlags.Data)))
            {
                return false;
            }
        }

        return true;
    }

    private static void AnalyzeRtti(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        List<AnalysisItem> rttiItems = session.Items.Values
            .Where(item => item.Flags.HasFlag(AnalysisFlags.Rtti))
            .ToList();
        if (rttiItems.Count == 0)
        {
            return;
        }

        HashSet<uint> queuedRttiAddresses = rttiItems
            .Select(item => item.Address)
            .ToHashSet();
        ReadOnlySpan<byte> image = session.Image.Span;
        progress?.Report(new AnalysisProgress("RttiScan", 0, rttiItems.Count));
        for (int index = 0; index < rttiItems.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AnalysisItem item = rttiItems[index];
            if (TryGetRawOffset(session, item.Address, out int rawOffset)
                && rawOffset < image.Length)
            {
                byte rawKind = image[rawOffset];
                if (rawKind is >= (byte)DelphiTypeKind.Integer and <= (byte)DelphiTypeKind.Procedure
                    && TryReadPascalTypeName(image, rawOffset + 1, out string? typeName))
                {
                    DelphiTypeKind typeKind = (DelphiTypeKind)rawKind;
                    item.TypeKind = typeKind;
                    item.Name ??= typeName;
                    if (typeKind == DelphiTypeKind.Class
                        && TryReadClassRtti(
                            session,
                            image,
                            rawOffset,
                            out uint classVmtAddress,
                            out uint? parentTypeInfoAddress,
                            out string? unitName,
                            out string[] propertyNames))
                    {
                        item.ClassVmtAddress = classVmtAddress;
                        item.ParentTypeInfoAddress = parentTypeInfoAddress;
                        item.UnitName = unitName;
                        item.PropertyNames = propertyNames;
                    }
                    else if (typeKind == DelphiTypeKind.Record
                        && TryReadRecordRtti(
                            session,
                            image,
                            session.SelectedDelphiVersion,
                            item.Address,
                            rawOffset,
                            out uint recordSize,
                            out DelphiRttiRecordField[] recordFields))
                    {
                        item.RecordSizeBytes = recordSize;
                        item.RecordFields = recordFields;
                        foreach (DelphiRttiRecordField field in recordFields)
                        {
                            if (field.TypeInfoAddress is uint fieldTypeInfoAddress)
                            {
                                AnalysisItem fieldType = session.GetOrAddItem(fieldTypeInfoAddress);
                                fieldType.SetFlags(AnalysisFlags.Data | AnalysisFlags.Rtti);
                                if (queuedRttiAddresses.Add(fieldTypeInfoAddress))
                                {
                                    rttiItems.Add(fieldType);
                                }
                            }
                        }
                    }
                }
            }

            progress?.Report(new AnalysisProgress("RttiScan", index + 1, rttiItems.Count));
        }
    }

    private static void AnalyzeVmts(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        PeSection[] scannedSections = session.Sections
            .Where(section => section.RawSize >= 4)
            .ToArray();
        if (scannedSections.Length == 0)
        {
            return;
        }

        long totalCandidates = scannedSections.Sum(section => (long)section.RawSize / 4);
        int progressTotal = (int)Math.Min(totalCandidates, int.MaxValue);
        long scannedCandidates = 0;
        HashSet<uint> discoveredVmtAddresses = [];
        ReadOnlySpan<byte> image = session.Image.Span;
        progress?.Report(new AnalysisProgress("VmtScan", 0, progressTotal));

        foreach (PeSection section in scannedSections)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ulong rawEnd = (ulong)section.RawAddress + section.RawSize;
            if (rawEnd > (ulong)image.Length)
            {
                throw new InvalidDataException(
                    $"Section '{section.Name}' extends beyond the image's file data.");
            }

            for (uint sectionOffset = 0;
                section.RawSize >= 4 && sectionOffset <= section.RawSize - 4;
                sectionOffset += 4)
            {
                if ((scannedCandidates & 0xFFF) == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progress?.Report(new AnalysisProgress(
                        "VmtScan",
                        (int)Math.Min(scannedCandidates, progressTotal),
                        progressTotal));
                }

                int rawOffset = checked((int)(section.RawAddress + sectionOffset));
                uint candidateRva = checked(section.VirtualAddress + sectionOffset);
                foreach (VmtLayout layout in VmtLayouts)
                {
                    if (TryReadVmt(
                        session,
                        image,
                        rawOffset,
                        candidateRva,
                        layout,
                        out uint vmtRva,
                        out string? className,
                        out uint classNameRva,
                        out uint classInstanceSize,
                        out uint? fieldTableRva,
                        out uint? initializationTableRva,
                        out uint? dynamicMethodTableRva,
                        out uint? autoMethodTableRva,
                        out uint? interfaceTableRva,
                        out uint? methodTableRva,
                        out uint? parentVmtRva,
                        out uint? typeInfoRva)
                        && discoveredVmtAddresses.Add(vmtRva))
                    {
                        AnalysisItem vmtItem = session.GetOrAddItem(vmtRva);
                        vmtItem.SetFlags(AnalysisFlags.Data | AnalysisFlags.Vmt);
                        vmtItem.Name ??= className;
                        vmtItem.ClassInstanceSizeBytes = classInstanceSize;
                        vmtItem.ParentAddress = parentVmtRva;
                        vmtItem.FieldTableAddress = fieldTableRva;
                        vmtItem.InitializationTableAddress = initializationTableRva;
                        vmtItem.DynamicMethodTableAddress = dynamicMethodTableRva;
                        vmtItem.AutoMethodTableAddress = autoMethodTableRva;
                        vmtItem.InterfaceTableAddress = interfaceTableRva;
                        vmtItem.MethodTableAddress = methodTableRva;
                        session.GetOrAddItem(classNameRva).SetFlags(AnalysisFlags.Data);
                        if (TryReadVmtVirtualMethods(
                                session,
                                image,
                                rawOffset,
                                candidateRva,
                                vmtRva,
                                layout,
                                out DelphiVmtVirtualMethod[] virtualMethods))
                        {
                            vmtItem.VirtualMethods = virtualMethods;
                            foreach (DelphiVmtVirtualMethod method in virtualMethods)
                            {
                                AnalysisItem methodItem = session.GetOrAddItem(method.CodeAddress);
                                methodItem.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
                                methodItem.Name ??= $"{className}.$Virtual_{method.SlotOffset:X}";
                            }
                        }

                        if (fieldTableRva is uint fieldTableAddress
                            && TryReadVmtFields(
                                session,
                                image,
                                fieldTableAddress,
                                session.SelectedDelphiVersion,
                                out DelphiVmtField[] fields))
                        {
                            vmtItem.Fields = fields;
                            foreach (DelphiVmtField field in fields)
                            {
                                if (field.TypeInfoAddress is uint fieldTypeInfoAddress)
                                {
                                    session.GetOrAddItem(fieldTypeInfoAddress)
                                        .SetFlags(AnalysisFlags.Data | AnalysisFlags.Rtti);
                                }
                            }
                        }

                        if (initializationTableRva is uint initializationAddress
                            && TryReadVmtInitializationFields(
                                session,
                                image,
                                initializationAddress,
                                out DelphiVmtInitializationField[] initializationFields))
                        {
                            vmtItem.InitializationFields = initializationFields;
                            foreach (DelphiVmtInitializationField field in initializationFields)
                            {
                                if (field.TypeInfoAddress is uint referencedTypeInfoAddress)
                                {
                                    session.GetOrAddItem(referencedTypeInfoAddress)
                                        .SetFlags(AnalysisFlags.Data | AnalysisFlags.Rtti);
                                }
                            }
                        }

                        if (interfaceTableRva is uint interfaceTableAddress
                            && TryReadVmtInterfaces(
                                session,
                                image,
                                interfaceTableAddress,
                                session.SelectedDelphiVersion,
                                layout.HasInterfaceImplementationGetter,
                                out DelphiVmtInterface[] interfaces))
                        {
                            vmtItem.Interfaces = interfaces;
                            foreach (DelphiVmtInterface intf in interfaces)
                            {
                                if (intf.TypeInfoAddress is uint interfaceTypeInfoAddress)
                                {
                                    session.GetOrAddItem(interfaceTypeInfoAddress)
                                        .SetFlags(AnalysisFlags.Data | AnalysisFlags.Rtti);
                                }
                            }
                        }

                        if (dynamicMethodTableRva is uint dynamicTableAddress
                            && TryReadVmtDynamicMethods(
                                session,
                                image,
                                dynamicTableAddress,
                                out DelphiVmtDynamicMethod[] dynamicMethods))
                        {
                            vmtItem.DynamicMethods = dynamicMethods;
                            foreach (DelphiVmtDynamicMethod method in dynamicMethods)
                            {
                                if (method.CodeAddress is uint methodAddress)
                                {
                                    AnalysisItem methodItem = session.GetOrAddItem(methodAddress);
                                    methodItem.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
                                    methodItem.Name ??= $"{className}.$Dynamic_{method.MessageId:X4}";
                                }
                            }
                        }

                        if (autoMethodTableRva is uint autoTableAddress
                            && TryReadVmtAutoMethods(
                                session,
                                image,
                                autoTableAddress,
                                vmtRva,
                                className!,
                                out DelphiVmtAutoMethod[] autoMethods))
                        {
                            vmtItem.AutoMethods = autoMethods;
                            foreach (DelphiVmtAutoMethod method in autoMethods)
                            {
                                AnalysisItem methodItem = session.GetOrAddItem(method.CodeAddress);
                                methodItem.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
                                methodItem.Name ??= method.Name;
                            }
                        }

                        if (methodTableRva is uint methodTableAddress
                            && TryReadVmtMethods(
                                session,
                                image,
                                methodTableAddress,
                                session.SelectedDelphiVersion,
                                out DelphiVmtMethod[] methods))
                        {
                            vmtItem.Methods = methods;
                            foreach (DelphiVmtMethod method in methods)
                            {
                                if (method.CodeAddress is uint methodAddress)
                                {
                                    AnalysisItem methodItem = session.GetOrAddItem(methodAddress);
                                    methodItem.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
                                    methodItem.Name ??= $"{className}.{method.Name}";
                                }
                            }
                        }

                        if (typeInfoRva is uint typeInfoAddress)
                        {
                            session.GetOrAddItem(typeInfoAddress)
                                .SetFlags(AnalysisFlags.Data | AnalysisFlags.Rtti);
                        }

                        break;
                    }
                }

                scannedCandidates++;
            }
        }

        progress?.Report(new AnalysisProgress("VmtScan", progressTotal, progressTotal));
    }

    private static bool TryReadVmt(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        int selfPointerRawOffset,
        uint selfPointerRva,
        VmtLayout layout,
        out uint vmtRva,
        out string? className,
        out uint classNameRva,
        out uint classInstanceSize,
        out uint? fieldTableRva,
        out uint? initializationTableRva,
        out uint? dynamicMethodTableRva,
        out uint? autoMethodTableRva,
        out uint? interfaceTableRva,
        out uint? methodTableRva,
        out uint? parentVmtRva,
        out uint? typeInfoRva)
    {
        vmtRva = 0;
        className = null;
        classNameRva = 0;
        classInstanceSize = 0;
        fieldTableRva = null;
        initializationTableRva = null;
        dynamicMethodTableRva = null;
        autoMethodTableRva = null;
        interfaceTableRva = null;
        methodTableRva = null;
        parentVmtRva = null;
        typeInfoRva = null;
        ulong candidateVmtRva = (ulong)selfPointerRva + layout.SelfPointerDisplacement;
        if (candidateVmtRva > uint.MaxValue)
        {
            return false;
        }

        if (session.ImageBase > uint.MaxValue
            || candidateVmtRva > uint.MaxValue - session.ImageBase)
        {
            return false;
        }

        uint expectedVmtVa = (uint)(session.ImageBase + candidateVmtRva);
        if (!TryReadUInt32(image, selfPointerRawOffset, out uint storedVmtVa)
            || storedVmtVa != expectedVmtVa)
        {
            return false;
        }

        if (!TryGetRawOffset(session, (uint)candidateVmtRva, out _))
        {
            return false;
        }

        for (int fieldOffset = 4; fieldOffset < layout.ClassNameDisplacement; fieldOffset += 4)
        {
            if (!TryReadUInt32(image, (long)selfPointerRawOffset + fieldOffset, out uint fieldVa)
                || (fieldVa != 0 && !TryVaToRawOffset(session, fieldVa, out _)))
            {
                return false;
            }
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.ClassNameDisplacement,
                out uint classNameVa)
            || !TryVaToRawOffset(session, classNameVa, out int classNameRawOffset)
            || !TryVaToRva(session, classNameVa, out classNameRva)
            || !TryReadPascalIdentifier(image, classNameRawOffset, out className))
        {
            return false;
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.InstanceSizeDisplacement,
                out uint instanceSize)
            || instanceSize == 0
            || !TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.ParentDisplacement,
                out uint parentVa))
        {
            return false;
        }
        classInstanceSize = instanceSize;

        if (parentVa != 0)
        {
            if (!TryVaToRawOffset(session, parentVa, out _)
                || !TryVaToRva(session, parentVa, out uint resolvedParentVmtRva))
            {
                return false;
            }

            parentVmtRva = resolvedParentVmtRva;
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.TypeInfoDisplacement,
                out uint typeInfoVa))
        {
            return false;
        }

        if (typeInfoVa != 0)
        {
            if (!TryVaToRawOffset(session, typeInfoVa, out int typeInfoRawOffset)
                || typeInfoRawOffset >= image.Length
                || image[typeInfoRawOffset] == 0
                || image[typeInfoRawOffset] > 0x15
                || !TryVaToRva(session, typeInfoVa, out uint resolvedTypeInfoRva))
            {
                return false;
            }

            typeInfoRva = resolvedTypeInfoRva;
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.MethodTableDisplacement,
                out uint methodTableVa))
        {
            return false;
        }

        if (methodTableVa != 0)
        {
            if (!TryVaToRawOffset(session, methodTableVa, out _)
                || !TryVaToRva(session, methodTableVa, out uint resolvedMethodTableRva))
            {
                return false;
            }

            methodTableRva = resolvedMethodTableRva;
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.FieldTableDisplacement,
                out uint fieldTableVa))
        {
            return false;
        }

        if (fieldTableVa != 0)
        {
            if (!TryVaToRawOffset(session, fieldTableVa, out _)
                || !TryVaToRva(session, fieldTableVa, out uint resolvedFieldTableRva))
            {
                return false;
            }

            fieldTableRva = resolvedFieldTableRva;
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.InitializationTableDisplacement,
                out uint initializationTableVa))
        {
            return false;
        }

        if (initializationTableVa != 0)
        {
            if (!TryVaToRawOffset(session, initializationTableVa, out _)
                || !TryVaToRva(session, initializationTableVa, out uint resolvedInitializationTableRva))
            {
                return false;
            }

            initializationTableRva = resolvedInitializationTableRva;
        }

        if (layout.InterfaceTableDisplacement is int interfaceTableDisplacement)
        {
            if (!TryReadUInt32(
                    image,
                    (long)selfPointerRawOffset + interfaceTableDisplacement,
                    out uint interfaceTableVa))
            {
                return false;
            }

            if (interfaceTableVa != 0)
            {
                if (!TryVaToRawOffset(session, interfaceTableVa, out _)
                    || !TryVaToRva(session, interfaceTableVa, out uint resolvedInterfaceTableRva))
                {
                    return false;
                }

                interfaceTableRva = resolvedInterfaceTableRva;
            }
        }

        if (!TryReadUInt32(
                image,
                (long)selfPointerRawOffset + layout.DynamicMethodTableDisplacement,
                out uint dynamicMethodTableVa))
        {
            return false;
        }

        if (dynamicMethodTableVa != 0)
        {
            if (!TryVaToRawOffset(session, dynamicMethodTableVa, out _)
                || !TryVaToRva(session, dynamicMethodTableVa, out uint resolvedDynamicMethodTableRva))
            {
                return false;
            }

            dynamicMethodTableRva = resolvedDynamicMethodTableRva;
        }

        if (layout.AutoMethodTableDisplacement is int autoMethodTableDisplacement)
        {
            if (!TryReadUInt32(
                    image,
                    (long)selfPointerRawOffset + autoMethodTableDisplacement,
                    out uint autoMethodTableVa))
            {
                return false;
            }

            if (autoMethodTableVa != 0)
            {
                if (!TryVaToRawOffset(session, autoMethodTableVa, out _)
                    || !TryVaToRva(session, autoMethodTableVa, out uint resolvedAutoMethodTableRva))
                {
                    return false;
                }

                autoMethodTableRva = resolvedAutoMethodTableRva;
            }
        }

        vmtRva = (uint)candidateVmtRva;
        return true;
    }

    private static bool TryReadVmtMethods(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint methodTableRva,
        DelphiVersion delphiVersion,
        out DelphiVmtMethod[] methods)
    {
        methods = [];
        if (!TryGetRawOffset(session, methodTableRva, out int methodTableRawOffset)
            || !TryGetMappedRawSectionEnd(
                session,
                methodTableRva,
                methodTableRawOffset,
                out long methodTableRawEnd)
            || !TryReadUInt16(image, methodTableRawOffset, out ushort methodCount)
            || methodTableRawOffset + sizeof(ushort) > methodTableRawEnd
            || methodCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        List<DelphiVmtMethod> parsedMethods = new(methodCount);
        long entryOffset = (long)methodTableRawOffset + sizeof(ushort);
        for (int index = 0; index < methodCount; index++)
        {
            if (!TryReadVmtMethodRecord(
                    session,
                    image,
                    entryOffset,
                    methodTableRawEnd,
                    out DelphiVmtMethod? method,
                    out ushort entryLength))
            {
                return false;
            }

            parsedMethods.Add(method!);
            entryOffset += entryLength;
        }

        if (delphiVersion >= DelphiVersion.Delphi2010)
        {
            if (entryOffset + sizeof(ushort) > methodTableRawEnd
                || !TryReadUInt16(image, entryOffset, out ushort extendedMethodCount)
                || extendedMethodCount > MaximumMetadataEntryCount)
            {
                return false;
            }

            entryOffset += sizeof(ushort);
            for (int index = 0; index < extendedMethodCount; index++)
            {
                if (entryOffset + 2L * sizeof(uint) > methodTableRawEnd
                    || !TryReadUInt32(image, entryOffset, out uint methodEntryVa)
                    || !TryReadUInt16(image, entryOffset + sizeof(uint), out ushort flags)
                    || !TryReadUInt16(image, entryOffset + sizeof(uint) + sizeof(ushort), out ushort virtualIndex)
                    || !TryVaToRawOffset(session, methodEntryVa, out int methodEntryRawOffset)
                    || !TryVaToRva(session, methodEntryVa, out uint methodEntryRva)
                    || !TryGetMappedRawSectionEnd(
                        session,
                        methodEntryRva,
                        methodEntryRawOffset,
                        out long methodEntryRawEnd)
                    || !TryReadVmtMethodRecord(
                        session,
                        image,
                        methodEntryRawOffset,
                        methodEntryRawEnd,
                        out DelphiVmtMethod? method,
                        out _))
                {
                    return false;
                }

                parsedMethods.Add(method! with
                {
                    IsExtended = true,
                    Flags = flags,
                    VirtualIndex = virtualIndex
                });
                entryOffset += sizeof(uint) + 2L * sizeof(ushort);
            }
        }

        methods = parsedMethods.ToArray();
        return true;
    }

    private static bool TryReadVmtFields(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint fieldTableRva,
        DelphiVersion delphiVersion,
        out DelphiVmtField[] fields)
    {
        fields = [];
        if (!TryGetRawOffset(session, fieldTableRva, out int fieldTableRawOffset)
            || !TryGetMappedRawSectionEnd(session, fieldTableRva, fieldTableRawOffset, out long fieldTableRawEnd)
            || fieldTableRawOffset + sizeof(ushort) + sizeof(uint) > fieldTableRawEnd
            || !TryReadUInt16(image, fieldTableRawOffset, out ushort fieldCount)
            || fieldCount > MaximumMetadataEntryCount
            || !TryReadUInt32(image, fieldTableRawOffset + sizeof(ushort), out uint typesTableVa)
            || !TryVaToRawOffset(session, typesTableVa, out int typesTableRawOffset)
            || !TryVaToRva(session, typesTableVa, out uint typesTableRva)
            || !TryGetMappedRawSectionEnd(session, typesTableRva, typesTableRawOffset, out long typesTableRawEnd)
            || !TryReadUInt16(image, typesTableRawOffset, out ushort typeCount)
            || typeCount > MaximumMetadataEntryCount
            || (long)typesTableRawOffset + sizeof(ushort) + (long)typeCount * sizeof(uint) > typesTableRawEnd)
        {
            return false;
        }

        uint[] typeInfoAddresses = new uint[typeCount];
        for (int index = 0; index < typeCount; index++)
        {
            long typeReferenceOffset = (long)typesTableRawOffset + sizeof(ushort) + (long)index * sizeof(uint);
            if (!TryReadUInt32(image, typeReferenceOffset, out uint typeInfoVa)
                || (typeInfoVa != 0
                    && (!TryVaToRawOffset(session, typeInfoVa, out _)
                        || !TryVaToRva(session, typeInfoVa, out typeInfoAddresses[index]))))
            {
                return false;
            }
        }

        List<DelphiVmtField> parsedFields = new(fieldCount);
        long cursor = (long)fieldTableRawOffset + sizeof(ushort) + sizeof(uint);
        for (int index = 0; index < fieldCount; index++)
        {
            if (cursor + sizeof(int) + sizeof(ushort) > fieldTableRawEnd
                || !TryReadInt32(image, cursor, out int offset)
                || !TryReadUInt16(image, cursor + sizeof(int), out ushort typeIndex))
            {
                return false;
            }

            long nameLengthOffset = cursor + sizeof(int) + sizeof(ushort);
            if (nameLengthOffset >= fieldTableRawEnd
                || typeIndex >= typeCount
                || !TryReadPascalTypeName(image, checked((int)nameLengthOffset), out string? name))
            {
                return false;
            }

            int nameLength = image[checked((int)nameLengthOffset)];
            long nextFieldOffset = nameLengthOffset + 1L + nameLength;
            if (nextFieldOffset > fieldTableRawEnd)
            {
                return false;
            }

            uint? typeInfoAddress = typeInfoAddresses[typeIndex] == 0
                ? null
                : typeInfoAddresses[typeIndex];
            parsedFields.Add(new DelphiVmtField(name!, offset, typeInfoAddress));
            cursor = nextFieldOffset;
        }

        if (delphiVersion >= DelphiVersion.Delphi2010)
        {
            if (cursor + sizeof(ushort) > fieldTableRawEnd
                || !TryReadUInt16(image, cursor, out ushort extendedFieldCount)
                || extendedFieldCount > MaximumMetadataEntryCount)
            {
                return false;
            }

            cursor += sizeof(ushort);
            for (int index = 0; index < extendedFieldCount; index++)
            {
                if (cursor + sizeof(byte) + sizeof(uint) + sizeof(int) > fieldTableRawEnd)
                {
                    return false;
                }

                byte flags = image[checked((int)cursor)];
                long typeReferenceOffset = cursor + sizeof(byte);
                if (!TryReadUInt32(image, typeReferenceOffset, out uint typeInfoVa)
                    || !TryReadInt32(image, typeReferenceOffset + sizeof(uint), out int offset))
                {
                    return false;
                }

                long nameLengthOffset = typeReferenceOffset + sizeof(uint) + sizeof(int);
                if (nameLengthOffset >= fieldTableRawEnd
                    || !TryReadPascalTypeName(image, checked((int)nameLengthOffset), out string? name))
                {
                    return false;
                }

                int nameLength = image[checked((int)nameLengthOffset)];
                long attributeLengthOffset = nameLengthOffset + 1L + nameLength;
                if (attributeLengthOffset + sizeof(ushort) > fieldTableRawEnd
                    || !TryReadUInt16(image, attributeLengthOffset, out ushort attributeDataLength)
                    || attributeDataLength < sizeof(ushort)
                    || attributeLengthOffset + attributeDataLength > fieldTableRawEnd)
                {
                    return false;
                }

                uint? resolvedTypeInfoAddress = null;
                if (typeInfoVa != 0)
                {
                    if (!TryVaToRawOffset(session, typeInfoVa, out _)
                        || !TryVaToRva(session, typeInfoVa, out uint typeInfoAddress))
                    {
                        return false;
                    }

                    resolvedTypeInfoAddress = typeInfoAddress;
                }

                parsedFields.Add(new DelphiVmtField(
                    name!,
                    offset,
                    resolvedTypeInfoAddress,
                    true,
                    flags));
                cursor = attributeLengthOffset + attributeDataLength;
            }
        }

        fields = parsedFields.ToArray();
        return true;
    }

    private static bool TryReadVmtInterfaces(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint interfaceTableRva,
        DelphiVersion delphiVersion,
        bool hasImplementationGetter,
        out DelphiVmtInterface[] interfaces)
    {
        interfaces = [];
        if (!TryGetRawOffset(session, interfaceTableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, interfaceTableRva, tableRawOffset, out long tableRawEnd)
            || tableRawOffset + sizeof(int) > tableRawEnd
            || !TryReadInt32(image, tableRawOffset, out int entryCount)
            || entryCount < 0
            || entryCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        int entrySize = 16 + sizeof(uint) + sizeof(int) + (hasImplementationGetter ? sizeof(int) : 0);
        long entriesEnd = (long)tableRawOffset + sizeof(int) + (long)entryCount * entrySize;
        if (entriesEnd > tableRawEnd)
        {
            return false;
        }

        List<DelphiVmtInterface> parsedInterfaces = new(entryCount);
        long cursor = (long)tableRawOffset + sizeof(int);
        for (int index = 0; index < entryCount; index++)
        {
            Guid id = new(image.Slice(checked((int)cursor), 16));
            cursor += 16;
            if (!TryReadUInt32(image, cursor, out uint vTableVa)
                || !TryReadInt32(image, cursor + sizeof(uint), out int offset))
            {
                return false;
            }

            uint? vTableAddress = null;
            if (vTableVa != 0)
            {
                if (!TryVaToRawOffset(session, vTableVa, out _)
                    || !TryVaToRva(session, vTableVa, out uint resolvedVTableAddress))
                {
                    return false;
                }

                vTableAddress = resolvedVTableAddress;
            }

            int? implementationGetter = null;
            cursor += sizeof(uint) + sizeof(int);
            if (hasImplementationGetter)
            {
                if (!TryReadInt32(image, cursor, out int getter))
                {
                    return false;
                }

                implementationGetter = getter;
                cursor += sizeof(int);
            }

            parsedInterfaces.Add(new DelphiVmtInterface(id, vTableAddress, offset, implementationGetter));
        }

        if (delphiVersion >= DelphiVersion.DelphiXE2)
        {
            if (cursor + (long)entryCount * sizeof(uint) > tableRawEnd)
            {
                return false;
            }

            for (int index = 0; index < entryCount; index++)
            {
                if (!TryReadUInt32(image, cursor, out uint typeInfoReferenceVa))
                {
                    return false;
                }

                uint? typeInfoAddress = null;
                if (typeInfoReferenceVa != 0)
                {
                    if (!TryVaToRawOffset(session, typeInfoReferenceVa, out int typeInfoReferenceRawOffset)
                        || !TryReadUInt32(image, typeInfoReferenceRawOffset, out uint typeInfoVa))
                    {
                        return false;
                    }

                    if (typeInfoVa != 0)
                    {
                        if (!TryVaToRawOffset(session, typeInfoVa, out _)
                            || !TryVaToRva(session, typeInfoVa, out uint resolvedTypeInfoAddress))
                        {
                            return false;
                        }

                        typeInfoAddress = resolvedTypeInfoAddress;
                    }
                }

                parsedInterfaces[index] = parsedInterfaces[index] with
                {
                    TypeInfoAddress = typeInfoAddress
                };
                cursor += sizeof(uint);
            }
        }

        if (cursor > tableRawEnd)
        {
            return false;
        }

        interfaces = parsedInterfaces.ToArray();
        return true;
    }

    private static bool TryReadVmtDynamicMethods(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint dynamicMethodTableRva,
        out DelphiVmtDynamicMethod[] methods)
    {
        methods = [];
        if (!TryGetRawOffset(session, dynamicMethodTableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, dynamicMethodTableRva, tableRawOffset, out long tableRawEnd)
            || tableRawOffset + sizeof(ushort) > tableRawEnd
            || !TryReadUInt16(image, tableRawOffset, out ushort methodCount)
            || methodCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        long messageIdsOffset = (long)tableRawOffset + sizeof(ushort);
        long methodAddressesOffset = messageIdsOffset + (long)methodCount * sizeof(ushort);
        long tableEnd = methodAddressesOffset + (long)methodCount * sizeof(uint);
        if (tableEnd > tableRawEnd)
        {
            return false;
        }

        List<DelphiVmtDynamicMethod> parsedMethods = new(methodCount);
        for (int index = 0; index < methodCount; index++)
        {
            if (!TryReadUInt16(image, messageIdsOffset + (long)index * sizeof(ushort), out ushort messageId)
                || !TryReadUInt32(image, methodAddressesOffset + (long)index * sizeof(uint), out uint methodVa))
            {
                return false;
            }

            uint? methodRva = null;
            if (methodVa != 0)
            {
                if (!TryVaToRva(session, methodVa, out uint resolvedMethodRva)
                    || !TryGetCodeSection(session, resolvedMethodRva, out _, out _))
                {
                    return false;
                }

                methodRva = resolvedMethodRva;
            }

            parsedMethods.Add(new DelphiVmtDynamicMethod(messageId, methodRva));
        }

        methods = parsedMethods.ToArray();
        return true;
    }

    private static bool TryReadVmtAutoMethods(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint autoMethodTableRva,
        uint vmtRva,
        string className,
        out DelphiVmtAutoMethod[] methods)
    {
        methods = [];
        if (!TryGetRawOffset(session, autoMethodTableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, autoMethodTableRva, tableRawOffset, out long tableRawEnd)
            || !TryReadInt32(image, tableRawOffset, out int methodCount)
            || methodCount < 0
            || methodCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        const int entrySize = 5 * sizeof(uint);
        long entriesStart = (long)tableRawOffset + sizeof(int);
        long entriesEnd = entriesStart + (long)methodCount * entrySize;
        if (entriesEnd > tableRawEnd)
        {
            return false;
        }

        List<DelphiVmtAutoMethod> parsedMethods = new(methodCount);
        for (int index = 0; index < methodCount; index++)
        {
            long entryOffset = entriesStart + (long)index * entrySize;
            if (!TryReadInt32(image, entryOffset, out int dispatchId)
                || !TryReadUInt32(image, entryOffset + sizeof(int), out uint nameVa)
                || !TryReadInt32(image, entryOffset + 2L * sizeof(uint), out int flags)
                || !TryReadUInt32(image, entryOffset + 3L * sizeof(uint), out uint parametersVa)
                || !TryReadUInt32(image, entryOffset + 4L * sizeof(uint), out uint methodVa)
                || !TryVaToRawOffset(session, nameVa, out int nameRawOffset)
                || !TryReadPascalTypeName(image, nameRawOffset, out string? methodName))
            {
                return false;
            }

            uint methodRva;
            if ((flags & 8) != 0)
            {
                ulong virtualSlotRva = (ulong)vmtRva + methodVa;
                if (virtualSlotRva > uint.MaxValue
                    || !TryGetRawOffset(session, (uint)virtualSlotRva, out int virtualSlotRawOffset)
                    || !TryReadUInt32(image, virtualSlotRawOffset, out uint virtualMethodVa)
                    || !TryVaToRva(session, virtualMethodVa, out methodRva))
                {
                    return false;
                }
            }
            else if (!TryVaToRva(session, methodVa, out methodRva))
            {
                return false;
            }

            if (!TryGetCodeSection(session, methodRva, out _, out _))
            {
                return false;
            }

            byte? returnType = null;
            byte[] parameterTypes = [];
            if (parametersVa != 0)
            {
                if (!TryVaToRva(session, parametersVa, out uint parametersRva)
                    || !TryGetRawOffset(session, parametersRva, out int parametersRawOffset)
                    || !TryGetMappedRawSectionEnd(session, parametersRva, parametersRawOffset, out long parametersRawEnd)
                    || parametersRawOffset + 2 > parametersRawEnd)
                {
                    return false;
                }

                returnType = image[parametersRawOffset];
                int parameterCount = image[parametersRawOffset + 1];
                long parameterEnd = (long)parametersRawOffset + 2 + parameterCount;
                if (parameterEnd > parametersRawEnd)
                {
                    return false;
                }

                parameterTypes = image.Slice(parametersRawOffset + 2, parameterCount).ToArray();
            }

            string prefix = string.Concat(
                (flags & 2) != 0 ? "Get" : string.Empty,
                (flags & 4) != 0 ? "Set" : string.Empty);
            parsedMethods.Add(new DelphiVmtAutoMethod(
                dispatchId,
                $"{className}.{prefix}{methodName}",
                methodRva,
                flags,
                returnType,
                parameterTypes));
        }

        methods = parsedMethods.ToArray();
        return true;
    }

    private static bool TryReadVmtInitializationFields(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        uint initializationTableRva,
        out DelphiVmtInitializationField[] fields)
    {
        fields = [];
        if (!TryGetRawOffset(session, initializationTableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, initializationTableRva, tableRawOffset, out long tableRawEnd)
            || tableRawOffset + 10 > tableRawEnd
            || !TryReadUInt32(image, (long)tableRawOffset + 6, out uint fieldCount)
            || fieldCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        long recordsStart = (long)tableRawOffset + 10;
        long recordsEnd = recordsStart + (long)fieldCount * 2 * sizeof(uint);
        if (recordsEnd > tableRawEnd)
        {
            return false;
        }

        List<DelphiVmtInitializationField> parsedFields = new((int)fieldCount);
        for (int index = 0; index < fieldCount; index++)
        {
            long recordOffset = recordsStart + (long)index * 2 * sizeof(uint);
            if (!TryReadUInt32(image, recordOffset, out uint typeInfoVa)
                || !TryReadInt32(image, recordOffset + sizeof(uint), out int fieldOffset))
            {
                return false;
            }

            uint? typeInfoAddress = null;
            if (typeInfoVa != 0)
            {
                if (!TryVaToRawOffset(session, typeInfoVa, out _)
                    || !TryVaToRva(session, typeInfoVa, out uint resolvedTypeInfoAddress))
                {
                    return false;
                }

                typeInfoAddress = resolvedTypeInfoAddress;
            }

            parsedFields.Add(new DelphiVmtInitializationField(typeInfoAddress, fieldOffset));
        }

        fields = parsedFields.ToArray();
        return true;
    }

    private static bool TryReadVmtVirtualMethods(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        int selfPointerRawOffset,
        uint selfPointerRva,
        uint vmtRva,
        VmtLayout layout,
        out DelphiVmtVirtualMethod[] methods)
    {
        methods = [];
        long firstSlotRawOffset = (long)selfPointerRawOffset + layout.ParentDisplacement + sizeof(uint);
        long firstSlotRva = (long)selfPointerRva + layout.ParentDisplacement + sizeof(uint);
        if (firstSlotRawOffset < 0
            || firstSlotRva < 0
            || firstSlotRva > uint.MaxValue
            || firstSlotRawOffset > int.MaxValue
            || !TryGetMappedRawSectionEnd(
                session,
                (uint)firstSlotRva,
                (int)firstSlotRawOffset,
                out long sectionRawEnd))
        {
            return false;
        }

        long tableRawEnd = sectionRawEnd;
        for (int fieldOffset = 4; fieldOffset < layout.ClassNameDisplacement; fieldOffset += 4)
        {
            if (!TryReadUInt32(image, (long)selfPointerRawOffset + fieldOffset, out uint pointerVa)
                || pointerVa == 0
                || !TryVaToRva(session, pointerVa, out uint pointerRva)
                || pointerRva <= vmtRva
                || !TryVaToRawOffset(session, pointerVa, out int pointerRawOffset))
            {
                continue;
            }

            if (pointerRawOffset > firstSlotRawOffset
                && pointerRawOffset < tableRawEnd
                && TryGetMappedRawSectionEnd(
                    session,
                    pointerRva,
                    pointerRawOffset,
                    out long pointerSectionEnd)
                && pointerSectionEnd == sectionRawEnd)
            {
                tableRawEnd = pointerRawOffset;
            }
        }

        long tableLength = tableRawEnd - firstSlotRawOffset;
        if (tableLength < 0
            || tableLength % sizeof(uint) != 0
            || tableLength / sizeof(uint) > MaximumMetadataEntryCount)
        {
            return false;
        }

        List<DelphiVmtVirtualMethod> parsedMethods = [];
        int slotCount = (int)(tableLength / sizeof(uint));
        for (int index = 0; index < slotCount; index++)
        {
            long slotRawOffset = firstSlotRawOffset + (long)index * sizeof(uint);
            if (!TryReadUInt32(image, slotRawOffset, out uint methodVa) || methodVa == 0)
            {
                continue;
            }

            if (!TryVaToRva(session, methodVa, out uint methodRva)
                || !TryGetCodeSection(session, methodRva, out _, out _))
            {
                continue;
            }

            long slotRva = firstSlotRva + (long)index * sizeof(uint);
            long relativeSlotOffset = slotRva - vmtRva;
            if (relativeSlotOffset < int.MinValue || relativeSlotOffset > int.MaxValue)
            {
                return false;
            }

            parsedMethods.Add(new DelphiVmtVirtualMethod((int)relativeSlotOffset, methodRva));
        }

        methods = parsedMethods.ToArray();
        return true;
    }

    private static bool TryReadVmtMethodRecord(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        long entryOffset,
        long containingEnd,
        out DelphiVmtMethod? method,
        out ushort entryLength)
    {
        method = null;
        entryLength = 0;
        if (!TryReadUInt16(image, entryOffset, out entryLength)
            || entryLength < 8
            || entryOffset + entryLength > containingEnd
            || !TryReadUInt32(image, entryOffset + sizeof(ushort), out uint methodVa)
            || !TryReadPascalTypeName(image, checked((int)(entryOffset + 6)), out string? methodName))
        {
            return false;
        }

        int methodNameLength = image[checked((int)(entryOffset + 6))];
        if (entryOffset + 7 + methodNameLength > entryOffset + entryLength)
        {
            return false;
        }

        uint? methodRva = null;
        if (methodVa != 0)
        {
            if (!TryVaToRva(session, methodVa, out uint resolvedMethodRva)
                || !TryGetCodeSection(session, resolvedMethodRva, out _, out _))
            {
                return false;
            }

            methodRva = resolvedMethodRva;
        }

        method = new DelphiVmtMethod(methodName!, methodRva);
        return true;
    }

    private static bool TryReadPascalIdentifier(
        ReadOnlySpan<byte> image,
        int rawOffset,
        out string? identifier)
    {
        identifier = null;
        if (rawOffset < 0 || rawOffset >= image.Length)
        {
            return false;
        }

        int length = image[rawOffset];
        if (length < 2 || rawOffset > image.Length - length - 1)
        {
            return false;
        }

        ReadOnlySpan<byte> nameBytes = image.Slice(rawOffset + 1, length);
        if (!IsAsciiIdentifierStart(nameBytes[0]))
        {
            return false;
        }

        foreach (byte value in nameBytes.Length > 1 ? nameBytes[1..] : ReadOnlySpan<byte>.Empty)
        {
            if (!IsAsciiIdentifierPart(value))
            {
                return false;
            }
        }

        identifier = System.Text.Encoding.ASCII.GetString(nameBytes);
        return true;
    }

    private static bool TryReadPascalTypeName(
        ReadOnlySpan<byte> image,
        int lengthOffset,
        out string? typeName)
    {
        typeName = null;
        if (lengthOffset < 0 || lengthOffset >= image.Length)
        {
            return false;
        }

        int length = image[lengthOffset];
        if (length < 1 || lengthOffset > image.Length - length - 1)
        {
            return false;
        }

        ReadOnlySpan<byte> nameBytes = image.Slice(lengthOffset + 1, length);
        if (!IsAsciiIdentifierStart(nameBytes[0]))
        {
            return false;
        }

        bool previousWasSeparator = false;
        for (int index = 1; index < nameBytes.Length; index++)
        {
            byte value = nameBytes[index];
            if (value == (byte)'.')
            {
                if (previousWasSeparator || index == nameBytes.Length - 1)
                {
                    return false;
                }

                previousWasSeparator = true;
            }
            else if (previousWasSeparator
                ? IsAsciiIdentifierStart(value)
                : IsAsciiIdentifierPart(value))
            {
                previousWasSeparator = false;
            }
            else
            {
                return false;
            }
        }

        typeName = System.Text.Encoding.ASCII.GetString(nameBytes);
        return true;
    }

    private static bool TryReadClassRtti(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        int typeInfoRawOffset,
        out uint classVmtAddress,
        out uint? parentTypeInfoAddress,
        out string? unitName,
        out string[] propertyNames)
    {
        classVmtAddress = 0;
        parentTypeInfoAddress = null;
        unitName = null;
        propertyNames = [];
        int nameLengthOffset = typeInfoRawOffset + 1;
        if (nameLengthOffset >= image.Length)
        {
            return false;
        }

        long fieldsOffset = (long)nameLengthOffset + image[nameLengthOffset] + 1;
        if (!TryReadUInt32(image, fieldsOffset, out uint classVmtVa)
            || !TryVaToRawOffset(session, classVmtVa, out _)
            || !TryVaToRva(session, classVmtVa, out uint resolvedClassVmtAddress)
            || !TryReadUInt32(image, fieldsOffset + sizeof(uint), out uint parentTypeInfoVa)
            || !TryReadUInt16(image, fieldsOffset + 2L * sizeof(uint), out ushort propertyCount)
            || propertyCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        uint? resolvedParentTypeInfoAddress = null;
        if (parentTypeInfoVa != 0)
        {
            if (!TryVaToRawOffset(session, parentTypeInfoVa, out _)
                || !TryVaToRva(session, parentTypeInfoVa, out uint parentRva))
            {
                return false;
            }

            resolvedParentTypeInfoAddress = parentRva;
        }

        long unitNameLengthOffset = fieldsOffset + 2L * sizeof(uint) + sizeof(ushort);
        if (unitNameLengthOffset >= image.Length
            || !TryReadPascalTypeName(image, checked((int)unitNameLengthOffset), out string? parsedUnitName))
        {
            return false;
        }

        long propertyDataCountOffset = unitNameLengthOffset + image[(int)unitNameLengthOffset] + 1L;
        if (!TryReadUInt16(image, propertyDataCountOffset, out ushort propertyDataCount)
            || propertyDataCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        List<string> parsedPropertyNames = new(propertyDataCount);
        long propertyOffset = propertyDataCountOffset + sizeof(ushort);
        for (int index = 0; index < propertyDataCount; index++)
        {
            long propertyNameLengthOffset = propertyOffset + 26;
            if (propertyNameLengthOffset >= image.Length
                || !TryReadPascalTypeName(
                    image,
                    checked((int)propertyNameLengthOffset),
                    out string? propertyName))
            {
                return false;
            }

            parsedPropertyNames.Add(propertyName!);
            propertyOffset = propertyNameLengthOffset + image[checked((int)propertyNameLengthOffset)] + 1L;
        }

        classVmtAddress = resolvedClassVmtAddress;
        parentTypeInfoAddress = resolvedParentTypeInfoAddress;
        unitName = parsedUnitName;
        propertyNames = parsedPropertyNames.ToArray();
        return true;
    }

    private static bool TryReadRecordRtti(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        DelphiVersion delphiVersion,
        uint typeInfoRva,
        int typeInfoRawOffset,
        out uint parsedRecordSize,
        out DelphiRttiRecordField[] fields)
    {
        parsedRecordSize = 0;
        fields = [];
        if (!TryGetMappedRawSectionEnd(session, typeInfoRva, typeInfoRawOffset, out long rawEnd)
            || !TryReadPascalTypeName(image, typeInfoRawOffset + 1, out _))
        {
            return false;
        }

        long nameLengthOffset = (long)typeInfoRawOffset + 1;
        long recordSizeOffset = nameLengthOffset + image[checked((int)nameLengthOffset)] + 1L;
        if (!TryReadUInt32(image, recordSizeOffset, out uint recordSizeValue)
            || recordSizeValue > int.MaxValue
            || !TryReadUInt32(image, recordSizeOffset + sizeof(uint), out uint fieldCount)
            || fieldCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        long fieldOffset = recordSizeOffset + 2L * sizeof(uint);
        if ((long)fieldCount * (sizeof(uint) + sizeof(int)) > rawEnd - fieldOffset)
        {
            return false;
        }

        List<DelphiRttiRecordField> parsedFields = new((int)fieldCount);
        for (int index = 0; index < fieldCount; index++)
        {
            long typeInfoVaOffset = fieldOffset + (long)index * (sizeof(uint) + sizeof(int));
            long offsetValueOffset = typeInfoVaOffset + sizeof(uint);
            if (!TryReadUInt32(image, typeInfoVaOffset, out uint typeInfoVa)
                || !TryReadInt32(image, offsetValueOffset, out int offset)
                || offset < 0
                || (uint)offset >= recordSizeValue)
            {
                return false;
            }

            uint? resolvedTypeInfoAddress = null;
            if (typeInfoVa != 0)
            {
                if (!TryVaToRawOffset(session, typeInfoVa, out _)
                    || !TryVaToRva(session, typeInfoVa, out uint resolvedRva))
                {
                    return false;
                }

                resolvedTypeInfoAddress = resolvedRva;
            }

            parsedFields.Add(new DelphiRttiRecordField($"f{offset:X}", offset, resolvedTypeInfoAddress));
        }

        long extendedFieldsOffset = fieldOffset + (long)fieldCount * (sizeof(uint) + sizeof(int));
        if (delphiVersion >= DelphiVersion.Delphi2010
            && TryReadExtendedRecordRttiFields(
                session,
                image,
                rawEnd,
                extendedFieldsOffset,
                recordSizeValue,
                out DelphiRttiRecordField[] extendedFields))
        {
            parsedFields = extendedFields.ToList();
        }

        parsedRecordSize = recordSizeValue;
        fields = parsedFields.ToArray();
        return true;
    }

    private static bool TryReadExtendedRecordRttiFields(
        AnalysisSession session,
        ReadOnlySpan<byte> image,
        long rawEnd,
        long cursor,
        uint recordSize,
        out DelphiRttiRecordField[] fields)
    {
        fields = [];
        if (cursor < 0 || cursor >= rawEnd)
        {
            return false;
        }

        int operationCount = image[checked((int)cursor)];
        cursor++;
        if (cursor + (long)operationCount * sizeof(uint) + sizeof(uint) > rawEnd)
        {
            return false;
        }

        cursor += (long)operationCount * sizeof(uint);
        if (!TryReadUInt32(image, cursor, out uint fieldCount)
            || fieldCount > MaximumMetadataEntryCount)
        {
            return false;
        }

        cursor += sizeof(uint);
        List<DelphiRttiRecordField> parsedFields = new((int)fieldCount);
        for (int index = 0; index < fieldCount; index++)
        {
            if (cursor + sizeof(uint) + sizeof(int) + sizeof(byte) > rawEnd
                || !TryReadUInt32(image, cursor, out uint typeInfoVa)
                || !TryReadInt32(image, cursor + sizeof(uint), out int offset)
                || offset < 0
                || (uint)offset >= recordSize)
            {
                return false;
            }

            cursor += sizeof(uint) + sizeof(int) + sizeof(byte);
            if (!TryReadPascalRecordFieldName(
                    image,
                    cursor,
                    rawEnd,
                    out string? name,
                    out long nextOffset))
            {
                return false;
            }

            cursor = nextOffset;
            if (!TryReadUInt16(image, cursor, out ushort attributeDataLength)
                || attributeDataLength < sizeof(ushort)
                || cursor + attributeDataLength > rawEnd)
            {
                return false;
            }

            cursor += attributeDataLength;
            uint? resolvedTypeInfoAddress = null;
            if (typeInfoVa != 0)
            {
                if (!TryVaToRawOffset(session, typeInfoVa, out _)
                    || !TryVaToRva(session, typeInfoVa, out uint resolvedRva))
                {
                    return false;
                }

                resolvedTypeInfoAddress = resolvedRva;
            }

            parsedFields.Add(new DelphiRttiRecordField(
                string.IsNullOrEmpty(name) ? $"f{offset:X}" : name,
                offset,
                resolvedTypeInfoAddress));
        }

        fields = parsedFields.ToArray();
        return true;
    }

    private static bool TryReadPascalRecordFieldName(
        ReadOnlySpan<byte> image,
        long lengthOffset,
        long rawEnd,
        out string? name,
        out long nextOffset)
    {
        name = null;
        nextOffset = 0;
        if (lengthOffset < 0
            || lengthOffset >= rawEnd
            || lengthOffset >= image.Length)
        {
            return false;
        }

        int length = image[checked((int)lengthOffset)];
        nextOffset = lengthOffset + 1L + length;
        if (nextOffset > rawEnd)
        {
            return false;
        }

        ReadOnlySpan<byte> nameBytes = image.Slice(checked((int)lengthOffset + 1), length);
        if (nameBytes.Length > 0 && !IsAsciiIdentifierStart(nameBytes[0]))
        {
            return false;
        }

        foreach (byte value in nameBytes[1..])
        {
            if (!IsAsciiIdentifierPart(value))
            {
                return false;
            }
        }

        name = System.Text.Encoding.ASCII.GetString(nameBytes);
        return true;
    }

    private static bool IsAsciiIdentifierStart(byte value) =>
        value is >= (byte)'A' and <= (byte)'Z'
        or >= (byte)'a' and <= (byte)'z'
        or (byte)'_';

    private static bool IsAsciiIdentifierPart(byte value) =>
        IsAsciiIdentifierStart(value) || value is >= (byte)'0' and <= (byte)'9';

    private static bool TryReadUInt32(ReadOnlySpan<byte> image, long offset, out uint value)
    {
        if (offset < 0 || offset > image.Length - sizeof(uint))
        {
            value = 0;
            return false;
        }

        value = BinaryPrimitives.ReadUInt32LittleEndian(image.Slice((int)offset, sizeof(uint)));
        return true;
    }

    private static bool TryReadUInt16(ReadOnlySpan<byte> image, long offset, out ushort value)
    {
        if (offset < 0 || offset > image.Length - sizeof(ushort))
        {
            value = 0;
            return false;
        }

        value = BinaryPrimitives.ReadUInt16LittleEndian(image.Slice((int)offset, sizeof(ushort)));
        return true;
    }

    private static bool TryReadInt32(ReadOnlySpan<byte> image, long offset, out int value)
    {
        if (offset < 0 || offset > image.Length - sizeof(int))
        {
            value = 0;
            return false;
        }

        value = BinaryPrimitives.ReadInt32LittleEndian(image.Slice((int)offset, sizeof(int)));
        return true;
    }

    private static bool TryGetRawOffset(AnalysisSession session, uint rva, out int rawOffset)
    {
        if (session.AddressMap is not null
            && session.AddressMap.TryRvaToRaw(rva, out rawOffset)
            && rawOffset < session.Image.Length)
        {
            return true;
        }

        rawOffset = -1;
        return false;
    }

    private static bool TryAnalyzeSwitchJumpTable(
        AnalysisSession session,
        DecodedInstruction instruction,
        uint? lastCompareAddress,
        uint codeStart,
        Queue<(uint Address, bool IsProcedureStart)> codeStarts,
        HashSet<uint> queuedStarts,
        CancellationToken cancellationToken)
    {
        if (instruction.FlowControl != InstructionFlowControl.IndirectBranch
            || !string.Equals(instruction.Mnemonic, "Jmp", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands.Count != 1
            || instruction.Operands[0].Kind != DecodedOperandKind.Memory
            || instruction.Operands[0].BaseRegister is not null
            || instruction.Operands[0].IndexRegister is null
            || instruction.Operands[0].Displacement is not ulong tableVa
            || tableVa > uint.MaxValue
            || !TryVaToRva(session, (uint)tableVa, out uint tableRva)
            || !TryGetRawOffset(session, tableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, tableRva, tableRawOffset, out long tableRawEnd))
        {
            return false;
        }

        List<(uint EntryRva, uint TargetRva)> entries = [];
        for (int index = 0; index < MaximumSwitchTableEntryCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long entryRawOffset = (long)tableRawOffset + (long)index * sizeof(uint);
            if (entryRawOffset < 0
                || entryRawOffset + sizeof(uint) > tableRawEnd
                || entryRawOffset + sizeof(uint) > session.Image.Length)
            {
                break;
            }

            uint entryVa = BinaryPrimitives.ReadUInt32LittleEndian(
                session.Image.Span.Slice((int)entryRawOffset, sizeof(uint)));
            if (!TryVaToRva(session, entryVa, out uint targetRva)
                || targetRva < codeStart
                || !TryGetCodeSection(session, targetRva, out _, out _))
            {
                break;
            }

            uint entryRva = checked(tableRva + (uint)(index * sizeof(uint)));
            entries.Add((entryRva, targetRva));
        }

        if (entries.Count == 0)
        {
            return false;
        }

        session.GetOrAddItem(checked((uint)instruction.Address)).SetFlags(AnalysisFlags.Switch);
        if (lastCompareAddress is uint compareAddress)
        {
            session.GetOrAddItem(compareAddress).SetFlags(AnalysisFlags.Switch);
        }

        foreach ((uint entryRva, uint targetRva) in entries)
        {
            session.GetOrAddItem(entryRva).SetFlags(AnalysisFlags.Data | AnalysisFlags.SwitchTable);
            session.AddCrossReference(checked((uint)instruction.Address), targetRva, CrossReferenceKind.Jump);
            if (queuedStarts.Add(targetRva))
            {
                codeStarts.Enqueue((targetRva, false));
            }
        }

        return true;
    }

    private static void AnalyzeExceptionHandlerTable(
        AnalysisSession session,
        uint targetRva,
        uint tableRva,
        Queue<(uint Address, bool IsProcedureStart)> codeStarts,
        HashSet<uint> queuedStarts,
        List<string> diagnostics,
        CancellationToken cancellationToken)
    {
        if (!session.Items.TryGetValue(targetRva, out AnalysisItem? target)
            || !string.Equals(target.Name, "@HandleOnException", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!TryGetRawOffset(session, tableRva, out int tableRawOffset)
            || !TryGetMappedRawSectionEnd(session, tableRva, tableRawOffset, out long tableRawEnd)
            || tableRawOffset > tableRawEnd - sizeof(int)
            || !TryReadInt32(session.Image.Span, tableRawOffset, out int handlerCount)
            || handlerCount < 0
            || handlerCount > MaximumMetadataEntryCount
            || tableRawOffset + sizeof(int) + (long)handlerCount * 2 * sizeof(uint) > tableRawEnd)
        {
            diagnostics.Add(
                $"Exception-handler table at RVA 0x{tableRva:X8} is invalid or truncated.");
            return;
        }

        List<DelphiExceptionHandler> handlers = new(handlerCount);
        int entryRawOffset = tableRawOffset + sizeof(int);
        for (int index = 0; index < handlerCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint exceptionInfoVa = BinaryPrimitives.ReadUInt32LittleEndian(
                session.Image.Span.Slice(entryRawOffset, sizeof(uint)));
            uint procedureVa = BinaryPrimitives.ReadUInt32LittleEndian(
                session.Image.Span.Slice(entryRawOffset + sizeof(uint), sizeof(uint)));
            entryRawOffset += 2 * sizeof(uint);

            uint? exceptionInfoRva = TryVaToRva(session, exceptionInfoVa, out uint infoRva)
                && TryGetRawOffset(session, infoRva, out _)
                    ? infoRva
                    : null;
            uint? procedureRva = TryVaToRva(session, procedureVa, out uint procRva)
                && TryGetCodeSection(session, procRva, out _, out _)
                    ? procRva
                    : null;

            handlers.Add(new DelphiExceptionHandler(exceptionInfoRva, procedureRva));
            if (procedureRva is uint validProcedureRva)
            {
                session.GetOrAddItem(validProcedureRva).SetFlags(AnalysisFlags.ProcedureStart);
                if (queuedStarts.Add(validProcedureRva))
                {
                    codeStarts.Enqueue((validProcedureRva, true));
                }
            }
        }

        AnalysisItem tableItem = session.GetOrAddItem(tableRva);
        tableItem.SetFlags(AnalysisFlags.Data | AnalysisFlags.ExceptionTable);
        tableItem.ExceptionHandlers = handlers;
    }

    private static bool TryMatchDelphiTryBegin(
        AnalysisSession session,
        IReadOnlyList<DecodedInstruction> instructions,
        out uint tryStartRva,
        out uint handlerRva)
    {
        tryStartRva = 0;
        handlerRva = 0;
        int count = instructions.Count;
        if (count >= 5)
        {
            DecodedInstruction xor = instructions[count - 5];
            DecodedInstruction pushFrame = instructions[count - 4];
            DecodedInstruction pushHandler = instructions[count - 3];
            DecodedInstruction pushFs = instructions[count - 2];
            DecodedInstruction setFs = instructions[count - 1];
            if (AreContiguous(xor, pushFrame)
                && AreContiguous(pushFrame, pushHandler)
                && AreContiguous(pushHandler, pushFs)
                && AreContiguous(pushFs, setFs)
                && IsXorSameRegister(xor)
                && IsPushRegister(pushFrame, "EBP")
                && TryGetPushedImmediate(pushHandler, out uint handlerVa)
                && IsFsMemoryPush(pushFs, requireBaseRegister: true)
                && IsFsStackRegistration(setFs, requireBaseRegister: true))
            {
                return TryResolveExceptionRegion(
                    session,
                    xor.Address,
                    handlerVa,
                    out tryStartRva,
                    out handlerRva);
            }
        }

        if (count >= 4)
        {
            DecodedInstruction pushFrame = instructions[count - 4];
            DecodedInstruction pushHandler = instructions[count - 3];
            DecodedInstruction pushFs = instructions[count - 2];
            DecodedInstruction setFs = instructions[count - 1];
            if (AreContiguous(pushFrame, pushHandler)
                && AreContiguous(pushHandler, pushFs)
                && AreContiguous(pushFs, setFs)
                && IsPushRegister(pushFrame, "EBP")
                && TryGetPushedImmediate(pushHandler, out uint handlerVa)
                && IsFsMemoryPush(pushFs, requireBaseRegister: false)
                && IsFsStackRegistration(setFs, requireBaseRegister: false))
            {
                return TryResolveExceptionRegion(
                    session,
                    pushFrame.Address,
                    handlerVa,
                    out tryStartRva,
                    out handlerRva);
            }
        }

        return false;
    }

    private static bool TryMatchDelphiFinallyEnd(
        AnalysisSession session,
        IReadOnlyList<DecodedInstruction> instructions,
        out uint finallyStartRva,
        out uint finallyEndRva)
    {
        finallyStartRva = 0;
        finallyEndRva = 0;
        if (instructions.Count < 6)
        {
            return false;
        }

        int firstIndex = instructions.Count - 6;
        DecodedInstruction xor = instructions[firstIndex];
        DecodedInstruction popFirst = instructions[firstIndex + 1];
        DecodedInstruction popSecond = instructions[firstIndex + 2];
        DecodedInstruction popThird = instructions[firstIndex + 3];
        DecodedInstruction restoreFs = instructions[firstIndex + 4];
        DecodedInstruction end = instructions[firstIndex + 5];
        if (!AreContiguous(xor, popFirst)
            || !AreContiguous(popFirst, popSecond)
            || !AreContiguous(popSecond, popThird)
            || !AreContiguous(popThird, restoreFs)
            || !AreContiguous(restoreFs, end)
            || !IsXorSameRegister(xor)
            || !IsPopRegister(popFirst)
            || !IsPopRegister(popSecond)
            || !IsPopRegister(popThird)
            || !IsFsStackRestore(restoreFs))
        {
            return false;
        }

        uint endAddress = 0;
        bool hasEndAddress = false;
        if (TryGetPushedImmediate(end, out uint endVa)
            && TryVaToRva(session, endVa, out endAddress))
        {
            hasEndAddress = true;
        }
        else if (end.FlowControl == InstructionFlowControl.UnconditionalBranch
            && end.NearBranchTarget is ulong branchTarget
            && branchTarget <= uint.MaxValue)
        {
            endAddress = (uint)branchTarget;
            hasEndAddress = true;
        }

        if (!hasEndAddress)
        {
            return false;
        }

        if (xor.Address > uint.MaxValue
            || !TryGetCodeSection(session, endAddress, out _, out _))
        {
            return false;
        }

        finallyStartRva = (uint)xor.Address;
        finallyEndRva = endAddress;
        return true;
    }

    private static bool TryMatchDelphiInt64Comparison(
        AnalysisSession session,
        IReadOnlyDictionary<uint, DecodedInstruction> decodedInstructions,
        DecodedInstruction lastInstruction,
        out DecodedInstruction[] pattern,
        out uint endAddress)
    {
        pattern = [];
        endAddress = 0;
        if (lastInstruction.FlowControl != InstructionFlowControl.ConditionalBranch
            || lastInstruction.NearBranchTarget is not ulong finalTarget
            || finalTarget > uint.MaxValue)
        {
            return false;
        }

        if (lastInstruction.Address > uint.MaxValue)
        {
            return false;
        }

        uint lastAddress = (uint)lastInstruction.Address;
        uint earliestStart = lastAddress > 75
            ? lastAddress - 75
            : 0;
        for (uint startAddress = earliestStart;
            startAddress < lastAddress;
            startAddress++)
        {
            if (!decodedInstructions.TryGetValue(startAddress, out DecodedInstruction? first)
                || first is null)
            {
                continue;
            }

            DecodedInstruction[] candidate = new DecodedInstruction[6];
            candidate[0] = first;
            ulong nextAddress = (ulong)first.Address + (uint)first.Length;
            bool complete = true;
            for (int index = 1; index < candidate.Length; index++)
            {
                if (nextAddress > uint.MaxValue
                    || !decodedInstructions.TryGetValue((uint)nextAddress, out DecodedInstruction? instruction)
                    || instruction is null)
                {
                    complete = false;
                    break;
                }

                candidate[index] = instruction;
                nextAddress = (ulong)instruction.Address + (uint)instruction.Length;
            }

            if (!complete
                || candidate[^1].Address != lastInstruction.Address
                || !string.Equals(candidate[0].Mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase)
                || candidate[1].FlowControl != InstructionFlowControl.ConditionalBranch
                || !string.Equals(candidate[2].Mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase)
                || candidate[3].FlowControl != InstructionFlowControl.ConditionalBranch
                || candidate[4].FlowControl != InstructionFlowControl.UnconditionalBranch
                || candidate[5].FlowControl != InstructionFlowControl.ConditionalBranch
                || candidate[1].NearBranchTarget != candidate[5].Address)
            {
                continue;
            }

            uint maxTarget = 0;
            bool validTargets = true;
            foreach (DecodedInstruction instruction in candidate.Where(
                item => item.FlowControl is InstructionFlowControl.ConditionalBranch
                    or InstructionFlowControl.UnconditionalBranch))
            {
                if (instruction.NearBranchTarget is not ulong target
                    || target > uint.MaxValue
                    || !TryGetCodeSection(session, (uint)target, out _, out _))
                {
                    validTargets = false;
                    break;
                }

                maxTarget = Math.Max(maxTarget, (uint)target);
            }

            if (!validTargets)
            {
                continue;
            }

            pattern = candidate;
            endAddress = maxTarget;
            return true;
        }

        return false;
    }

    private static bool TryMatchDelphiInt64StackComparison(
        AnalysisSession session,
        IReadOnlyDictionary<uint, DecodedInstruction> decodedInstructions,
        DecodedInstruction lastInstruction,
        out uint patternStart,
        out uint comparisonAddress,
        out uint branchAddress,
        out uint endAddress)
    {
        patternStart = 0;
        comparisonAddress = 0;
        branchAddress = 0;
        endAddress = 0;
        if (lastInstruction.FlowControl != InstructionFlowControl.ConditionalBranch
            || !TryGetPreviousDecodedInstruction(decodedInstructions, lastInstruction, out DecodedInstruction secondPop)
            || !IsPopRegister(secondPop)
            || !TryGetPreviousDecodedInstruction(decodedInstructions, secondPop, out DecodedInstruction firstPop)
            || !IsPopRegister(firstPop)
            || !TryGetPreviousDecodedInstruction(decodedInstructions, firstPop, out DecodedInstruction secondCompare)
            || !IsCompareWithStackOffset(secondCompare, 0)
            || !TryGetPreviousDecodedInstruction(decodedInstructions, secondCompare, out DecodedInstruction firstBranch)
            || firstBranch.FlowControl != InstructionFlowControl.ConditionalBranch
            || !TryGetPreviousDecodedInstruction(decodedInstructions, firstBranch, out DecodedInstruction firstCompare)
            || !IsCompareWithStackOffset(firstCompare, 4)
            || firstBranch.NearBranchTarget != firstPop.Address
            || firstPop.Address > uint.MaxValue
            || !TryGetCodeSection(session, (uint)firstPop.Address, out _, out _))
        {
            return false;
        }

        if (!TryFindInt64StackPushPair(
                decodedInstructions,
                firstCompare,
                out uint firstPushAddress))
        {
            return false;
        }

        patternStart = firstPushAddress;
        comparisonAddress = checked((uint)firstCompare.Address);
        branchAddress = checked((uint)firstBranch.Address);
        endAddress = checked((uint)firstPop.Address);
        return true;
    }

    private static bool TryMatchDelphiInt64StackComparisonWithSplitCleanup(
        AnalysisSession session,
        IReadOnlyDictionary<uint, DecodedInstruction> decodedInstructions,
        DecodedInstruction lastInstruction,
        out DecodedInstruction[] pattern,
        out uint patternStart,
        out uint endAddress)
    {
        pattern = [];
        patternStart = 0;
        endAddress = 0;
        if (lastInstruction.FlowControl != InstructionFlowControl.ConditionalBranch)
        {
            return false;
        }

        DecodedInstruction[] candidate = new DecodedInstruction[10];
        candidate[^1] = lastInstruction;
        for (int index = candidate.Length - 2; index >= 0; index--)
        {
            if (!TryGetPreviousDecodedInstruction(
                    decodedInstructions,
                    candidate[index + 1],
                    out candidate[index]))
            {
                return false;
            }
        }

        if (!IsCompareWithStackOffset(candidate[0], 4)
            || candidate[1].FlowControl != InstructionFlowControl.ConditionalBranch
            || !IsCompareWithStackOffset(candidate[2], 0)
            || !IsPopRegister(candidate[3])
            || !IsPopRegister(candidate[4])
            || candidate[5].FlowControl != InstructionFlowControl.ConditionalBranch
            || candidate[6].FlowControl != InstructionFlowControl.UnconditionalBranch
            || !IsPopRegister(candidate[7])
            || !IsPopRegister(candidate[8])
            || candidate[9].FlowControl != InstructionFlowControl.ConditionalBranch
            || candidate[1].NearBranchTarget != candidate[7].Address
            || !TryFindInt64StackPushPair(
                decodedInstructions,
                candidate[0],
                out uint firstPushAddress))
        {
            return false;
        }

        uint maximumTarget = 0;
        foreach (DecodedInstruction instruction in candidate.Where(
            item => item.FlowControl is InstructionFlowControl.ConditionalBranch
                or InstructionFlowControl.UnconditionalBranch))
        {
            if (instruction.NearBranchTarget is not ulong target
                || target > uint.MaxValue
                || !TryGetCodeSection(session, (uint)target, out _, out _))
            {
                return false;
            }

            maximumTarget = Math.Max(maximumTarget, (uint)target);
        }

        pattern = candidate;
        patternStart = firstPushAddress;
        endAddress = maximumTarget;
        return true;
    }

    private static bool TryFindInt64StackPushPair(
        IReadOnlyDictionary<uint, DecodedInstruction> decodedInstructions,
        DecodedInstruction firstCompare,
        out uint patternStart)
    {
        DecodedInstruction cursor = firstCompare;
        for (int index = 0; index < MaximumInstructionsPerProcedure; index++)
        {
            if (!TryGetPreviousDecodedInstruction(decodedInstructions, cursor, out DecodedInstruction previous))
            {
                patternStart = 0;
                return false;
            }

            if (previous.FlowControl is InstructionFlowControl.ConditionalBranch
                or InstructionFlowControl.UnconditionalBranch
                or InstructionFlowControl.IndirectBranch
                or InstructionFlowControl.Return
                or InstructionFlowControl.Interrupt)
            {
                patternStart = 0;
                return false;
            }

            if (IsPushInstruction(previous))
            {
                if (!IsPushRegister(previous)
                    || !TryGetPreviousDecodedInstruction(
                        decodedInstructions,
                        previous,
                        out DecodedInstruction firstPush)
                    || !IsPushRegister(firstPush))
                {
                    patternStart = 0;
                    return false;
                }

                patternStart = checked((uint)firstPush.Address);
                return true;
            }

            cursor = previous;
        }

        patternStart = 0;
        return false;
    }

    private static bool TryGetPreviousDecodedInstruction(
        IReadOnlyDictionary<uint, DecodedInstruction> decodedInstructions,
        DecodedInstruction instruction,
        out DecodedInstruction previous)
    {
        for (int length = 1; length <= 15 && instruction.Address >= (uint)length; length++)
        {
            uint previousAddress = (uint)instruction.Address - (uint)length;
            if (decodedInstructions.TryGetValue(previousAddress, out DecodedInstruction? candidate)
                && candidate is not null
                && AreContiguous(candidate, instruction))
            {
                previous = candidate;
                return true;
            }
        }

        previous = null!;
        return false;
    }

    private static bool IsCompareWithStackOffset(
        DecodedInstruction instruction,
        ulong offset) =>
        string.Equals(instruction.Mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands.Count == 2
        && instruction.Operands[1].Kind == DecodedOperandKind.Memory
        && string.Equals(instruction.Operands[1].BaseRegister, "ESP", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands[1].IndexRegister is null
        && instruction.Operands[1].Displacement == offset;

    private static bool IsPushInstruction(DecodedInstruction instruction) =>
        string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase);

    private static bool IsPushRegister(DecodedInstruction instruction) =>
        IsPushInstruction(instruction)
        && instruction.Operands.Count == 1
        && instruction.Operands[0].Kind == DecodedOperandKind.Register;

    private static bool TryResolveExceptionRegion(
        AnalysisSession session,
        ulong tryStartAddress,
        uint handlerVa,
        out uint tryStartRva,
        out uint handlerRva)
    {
        if (tryStartAddress <= uint.MaxValue
            && TryVaToRva(session, handlerVa, out handlerRva)
            && TryGetCodeSection(session, handlerRva, out _, out _))
        {
            tryStartRva = (uint)tryStartAddress;
            return true;
        }

        tryStartRva = 0;
        handlerRva = 0;
        return false;
    }

    private static bool AreContiguous(
        DecodedInstruction first,
        DecodedInstruction second) =>
        first.Length > 0 && (ulong)first.Address + (uint)first.Length == second.Address;

    private static bool IsXorSameRegister(DecodedInstruction instruction) =>
        string.Equals(instruction.Mnemonic, "Xor", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands.Count == 2
        && instruction.Operands[0].Kind == DecodedOperandKind.Register
        && instruction.Operands[1].Kind == DecodedOperandKind.Register
        && string.Equals(
            instruction.Operands[0].Register,
            instruction.Operands[1].Register,
            StringComparison.OrdinalIgnoreCase);

    private static bool IsPushRegister(DecodedInstruction instruction, string register) =>
        string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands.Count == 1
        && instruction.Operands[0].Kind == DecodedOperandKind.Register
        && string.Equals(
            instruction.Operands[0].Register,
            register,
            StringComparison.OrdinalIgnoreCase);

    private static bool IsPopRegister(DecodedInstruction instruction) =>
        string.Equals(instruction.Mnemonic, "Pop", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands.Count == 1
        && instruction.Operands[0].Kind == DecodedOperandKind.Register;

    private static bool TryGetPushedImmediate(DecodedInstruction instruction, out uint immediate)
    {
        if (string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 1
            && instruction.Operands[0] is
            {
                Kind: DecodedOperandKind.Immediate,
                Immediate: <= uint.MaxValue
            } operand)
        {
            immediate = (uint)operand.Immediate!.Value;
            return true;
        }

        immediate = 0;
        return false;
    }

    private static bool IsFsMemoryPush(
        DecodedInstruction instruction,
        bool requireBaseRegister)
    {
        if (!string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands.Count != 1)
        {
            return false;
        }

        DecodedOperand memory = instruction.Operands[0];
        return IsFsMemory(memory)
            && memory.IndexRegister is null
            && (requireBaseRegister
                ? memory.BaseRegister is not null
                : memory.BaseRegister is null && memory.Displacement == 0);
    }

    private static bool IsFsStackRegistration(
        DecodedInstruction instruction,
        bool requireBaseRegister)
    {
        if (!string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands.Count != 2
            || instruction.Operands[1].Kind != DecodedOperandKind.Register
            || !string.Equals(instruction.Operands[1].Register, "ESP", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        DecodedOperand memory = instruction.Operands[0];
        return IsFsMemory(memory)
            && memory.IndexRegister is null
            && (requireBaseRegister
                ? memory.BaseRegister is not null
                : memory.BaseRegister is null && memory.Displacement == 0);
    }

    private static bool IsFsStackRestore(DecodedInstruction instruction) =>
        string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
        && instruction.Operands.Count == 2
        && IsFsMemory(instruction.Operands[0])
        && instruction.Operands[0].BaseRegister is not null
        && instruction.Operands[0].IndexRegister is null
        && instruction.Operands[1].Kind == DecodedOperandKind.Register;

    private static bool IsFsMemory(DecodedOperand operand) =>
        operand.Kind == DecodedOperandKind.Memory
        && string.Equals(operand.SegmentRegister, "FS", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetMappedRawSectionEnd(
        AnalysisSession session,
        uint rva,
        int rawOffset,
        out long rawEnd)
    {
        foreach (PeSection section in session.Sections)
        {
            ulong virtualEnd = (ulong)section.VirtualAddress + section.VirtualSize;
            ulong sectionRawEnd = (ulong)section.RawAddress + section.RawSize;
            if (rva >= section.VirtualAddress
                && (ulong)rva < virtualEnd
                && rawOffset >= section.RawAddress
                && (ulong)rawOffset < sectionRawEnd
                && (ulong)rawOffset - section.RawAddress == (ulong)rva - section.VirtualAddress
                && sectionRawEnd <= (ulong)session.Image.Length)
            {
                rawEnd = (long)sectionRawEnd;
                return true;
            }
        }

        rawEnd = 0;
        return false;
    }

    private static bool TryVaToRawOffset(AnalysisSession session, uint va, out int rawOffset)
    {
        if (TryVaToRva(session, va, out uint rva)
            && TryGetRawOffset(session, rva, out rawOffset))
        {
            return true;
        }

        rawOffset = -1;
        return false;
    }

    private static void AddDataAddressCrossReferences(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, ulong> registerAddressValues)
    {
        bool isMoveRegisterImmediate = string.Equals(
                instruction.Mnemonic,
                "Mov",
                StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && instruction.Operands[1].Kind == DecodedOperandKind.Immediate;

        uint sourceRva = checked((uint)instruction.Address);
        foreach (DecodedOperand operand in instruction.Operands)
        {
            ulong? absoluteAddress = operand.Kind switch
            {
                DecodedOperandKind.Immediate when isMoveRegisterImmediate => operand.Immediate,
                DecodedOperandKind.Memory
                    when operand.BaseRegister is null
                        && operand.IndexRegister is null
                        && IsDefaultDataSegment(operand.SegmentRegister) => operand.Displacement,
                DecodedOperandKind.Memory
                    when operand.IndexRegister is null
                        && IsDefaultDataSegment(operand.SegmentRegister)
                        && GetRegisterAddressIndex(operand.BaseRegister) is int baseIndex
                        && registerAddressValues.TryGetValue(baseIndex, out ulong baseAddress)
                        && operand.Displacement is ulong displacement
                        && displacement <= int.MaxValue
                        && baseAddress <= uint.MaxValue - displacement =>
                            baseAddress + displacement,
                _ => null
            };
            if (absoluteAddress is ulong address
                && address <= uint.MaxValue
                && TryVaToRva(session, (uint)address, out uint targetRva)
                && IsMappedDataRva(session, targetRva))
            {
                AddConstantCrossReferenceIfMissing(session, sourceRva, targetRva);
            }
        }
    }

    private static void AddConstantCrossReferenceIfMissing(
        AnalysisSession session,
        uint sourceRva,
        uint targetRva)
    {
        AnalysisItem source = session.GetOrAddItem(sourceRva);
        if (!source.CrossReferences.Any(reference =>
                reference.TargetAddress == targetRva
                && reference.Kind == CrossReferenceKind.Constant))
        {
            session.AddCrossReference(sourceRva, targetRva, CrossReferenceKind.Constant);
        }
    }

    private static bool IsDefaultDataSegment(string? segmentRegister) =>
        string.IsNullOrEmpty(segmentRegister)
        || string.Equals(segmentRegister, "DS", StringComparison.OrdinalIgnoreCase);

    private static bool TryVaToRva(AnalysisSession session, uint va, out uint rva)
    {
        if (va < session.ImageBase || (ulong)va - session.ImageBase > uint.MaxValue)
        {
            rva = 0;
            return false;
        }

        rva = (uint)((ulong)va - session.ImageBase);
        return true;
    }

    private readonly record struct VmtLayout(
        uint SelfPointerDisplacement,
        int ClassNameDisplacement,
        int InstanceSizeDisplacement,
        int ParentDisplacement,
        int TypeInfoDisplacement,
        int InitializationTableDisplacement,
        int FieldTableDisplacement,
        int MethodTableDisplacement,
        int DynamicMethodTableDisplacement,
        int? AutoMethodTableDisplacement,
        int? InterfaceTableDisplacement,
        bool HasInterfaceImplementationGetter);

    private void AnalyzeCode(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken,
        List<string> diagnostics)
    {
        Queue<(uint Address, bool IsProcedureStart)> codeStarts = new();
        Dictionary<uint, uint> returnedCallTargets = [];
        Dictionary<uint, string> procedureOwnerClassNames = BuildProcedureOwnerClassNames(session);
        HashSet<uint> functionCandidateTargets = [];
        HashSet<uint> queuedStarts = [session.EntryPointRva];
        HashSet<uint> decodedAddresses = [];
        Dictionary<uint, DecodedInstruction> decodedInstructions = [];
        codeStarts.Enqueue((session.EntryPointRva, true));
        foreach (AnalysisItem item in session.Items.Values)
        {
            if (item.Flags.HasFlag(AnalysisFlags.ProcedureStart)
                && item.Address != session.EntryPointRva
                && TryGetCodeSection(session, item.Address, out _, out _)
                && queuedStarts.Add(item.Address))
            {
                codeStarts.Enqueue((item.Address, true));
            }
        }

        int completedStarts = 0;
        progress?.Report(new AnalysisProgress("CodeScan", 0, MaximumCodeStarts));

        while (codeStarts.Count > 0 && completedStarts < MaximumCodeStarts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            (uint codeStart, bool isProcedureStart) = codeStarts.Dequeue();
            if (decodedAddresses.Contains(codeStart))
            {
                continue;
            }

            if (!TryGetCodeSection(session, codeStart, out PeSection? section, out int rawOffset))
            {
                if (codeStart == session.EntryPointRva)
                {
                    diagnostics.Add("The entry point is not mapped to raw code-section data.");
                }

                completedStarts++;
                progress?.Report(new AnalysisProgress(
                    "CodeScan",
                    completedStarts,
                    MaximumCodeStarts));
                continue;
            }

            AnalysisItem startItem = session.GetOrAddItem(codeStart);
            startItem.SetFlags(AnalysisFlags.Code);
            if (isProcedureStart)
            {
                startItem.SetFlags(AnalysisFlags.ProcedureStart);
            }

            ulong rawEnd = (ulong)section.RawAddress + section.RawSize;
            if (rawEnd > (ulong)session.Image.Length)
            {
                throw new InvalidDataException(
                    $"Code section '{section.Name}' extends beyond the image's file data.");
            }

            uint currentRva = codeStart;
            int currentRawOffset = rawOffset;
            int decodedCount = 0;
            uint? lastCompareAddress = null;
            uint? procedureEnd = null;
            Dictionary<uint, StackArgument> stackArguments = [];
            List<(uint SourceRva, uint TargetRva)> codePointerCandidates = [];
            uint? pendingReturnCallTarget = null;
            uint? pendingFunctionResultTarget = null;
            uint? pendingX87ResultTarget = null;
            bool[] availableRegisterArguments = [true, true, true];
            bool[] usedRegisterArguments = [false, false, false];
            Dictionary<int, string> classRegisterTypes = [];
            if (procedureOwnerClassNames.TryGetValue(codeStart, out string? ownerClassName))
            {
                classRegisterTypes[0] = ownerClassName;
            }

            Dictionary<int, ulong> registerAddressValues = [];
            Dictionary<int, int> registerLocalOffsets = [];
            Dictionary<int, uint> registerStackArgumentOffsets = [];
            Dictionary<int, StackLocalVariable> stackLocalVariables = [];
            string? returnTypeCandidate = null;
            bool reachedTerminator = false;
            List<DecodedInstruction> recentInstructions = [];
            List<DecodedInstruction> procedureInstructions = [];
            bool markNextInstructionAsFinallyExit = false;
            bool pendingTlsBase = false;
            for (int instructionCount = 0;
                instructionCount < MaximumInstructionsPerProcedure;
                instructionCount++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (instructionCount == 0 || instructionCount % 16 == 0)
                {
                    progress?.Report(new AnalysisProgress(
                        "CodeScan",
                        completedStarts,
                        MaximumCodeStarts));
                }

                if (!TryGetCodeSection(session, currentRva, out PeSection? currentSection, out currentRawOffset)
                    || !ReferenceEquals(section, currentSection))
                {
                    break;
                }

                rawEnd = (ulong)section.RawAddress + section.RawSize;
                int remainingBytes = checked((int)rawEnd - currentRawOffset);
                if (remainingBytes <= 0)
                {
                    break;
                }

                DecodedInstruction instruction;
                try
                {
                    instruction = _instructionDecoder.Decode(
                        session.Image.Span.Slice(currentRawOffset, remainingBytes),
                        currentRva,
                        32);
                }
                catch (InstructionDecodeException exception)
                {
                    diagnostics.Add(
                        $"Stopped decoding code at RVA 0x{currentRva:X8}: {exception.Message}");
                    break;
                }
                if (instruction.Length <= 0 || instruction.Length > remainingBytes)
                {
                    throw new InvalidDataException(
                        $"The decoder returned an invalid instruction length at RVA 0x{currentRva:X8}.");
                }

                if (instruction.Address != currentRva)
                {
                    throw new InvalidDataException(
                        $"The decoder returned address 0x{instruction.Address:X8} for instruction RVA 0x{currentRva:X8}.");
                }

                if (!decodedAddresses.Add(currentRva))
                {
                    break;
                }

                decodedInstructions.Add(currentRva, instruction);
                decodedCount++;
                DecodedInstruction classTypeInstruction = instruction;
                AnalysisItem item = session.GetOrAddItem(currentRva);
                item.SetFlags(AnalysisFlags.Code | AnalysisFlags.Instruction);
                if (markNextInstructionAsFinallyExit)
                {
                    item.SetFlags(AnalysisFlags.FinallyExit);
                    markNextInstructionAsFinallyExit = false;
                }

                item.Name ??= instruction.Mnemonic;
                if (pendingTlsBase)
                {
                    pendingTlsBase = false;
                    item.ThreadVariableCandidate = GetTlsThreadVariableCandidate(instruction);
                }

                bool isCallPopSequence = IsCallToNextInstructionFollowedByPop(session, instruction);
                AnalyzeStackInstruction(instruction, currentRva, codeStart, isProcedureStart, startItem, item);
                if (isProcedureStart)
                {
                    ApplyFloatingPointMemoryTypeCandidate(
                        instruction,
                        session,
                        startItem,
                        registerAddressValues,
                        stackLocalVariables);

                    if (pendingX87ResultTarget is uint x87Target)
                    {
                        if (IsX87ResultStore(instruction))
                        {
                            functionCandidateTargets.Add(x87Target);
                        }

                        pendingX87ResultTarget = null;
                    }

                    if (instruction.FlowControl == InstructionFlowControl.Call && !isCallPopSequence)
                    {
                        pendingFunctionResultTarget = instruction.NearBranchTarget is ulong calledAddress
                            && calledAddress <= uint.MaxValue
                                ? (uint)calledAddress
                                : null;
                        pendingX87ResultTarget = instruction.NearBranchTarget is ulong x87CalledAddress
                            && x87CalledAddress <= uint.MaxValue
                                ? (uint)x87CalledAddress
                                : null;
                    }
                    else if (pendingFunctionResultTarget is uint calledTarget)
                    {
                        if (ReadsRegisterArgument(instruction, 0))
                        {
                            functionCandidateTargets.Add(calledTarget);
                            pendingFunctionResultTarget = null;
                        }
                        else if (WritesRegisterArgument(instruction, 0))
                        {
                            pendingFunctionResultTarget = null;
                        }
                        else if (instruction.FlowControl == InstructionFlowControl.Return)
                        {
                            functionCandidateTargets.Add(calledTarget);
                            pendingFunctionResultTarget = null;
                        }
                    }

                    bool preservesRegisterArguments = isCallPopSequence
                        || IsRegisterPreservingCall(instruction, session);
                    AnalyzeRegisterArgumentCandidates(
                        instruction,
                        availableRegisterArguments,
                        usedRegisterArguments,
                        preservesRegisterArguments);
                    if (instruction.FlowControl == InstructionFlowControl.Call && !isCallPopSequence)
                    {
                        List<uint> resolvedCallTargets = [];
                        if (TryResolveDynamicMethodTarget(
                                instruction,
                                session,
                                classRegisterTypes,
                                procedureInstructions,
                                out uint dynamicMethodTarget))
                        {
                            resolvedCallTargets.Add(dynamicMethodTarget);
                        }

                        if (TryResolveVirtualMethodTarget(
                                instruction,
                                session,
                                classRegisterTypes,
                                out uint virtualMethodTarget))
                        {
                            resolvedCallTargets.Add(virtualMethodTarget);
                        }

                        uint[] uniqueResolvedCallTargets = resolvedCallTargets.Distinct().ToArray();
                        foreach (uint resolvedCallTarget in uniqueResolvedCallTargets)
                        {
                            session.AddCrossReference(currentRva, resolvedCallTarget, CrossReferenceKind.Call);
                            item.SetFlags(AnalysisFlags.Call);
                            session.GetOrAddItem(resolvedCallTarget)
                                .SetFlags(AnalysisFlags.ProcedureStart);
                            if (queuedStarts.Add(resolvedCallTarget))
                            {
                                codeStarts.Enqueue((resolvedCallTarget, true));
                            }
                        }

                        if (IsTryFinallyExitCall(instruction, session))
                        {
                            foreach (DecodedInstruction previousInstruction in procedureInstructions.AsEnumerable().Reverse())
                            {
                                if (previousInstruction.FlowControl == InstructionFlowControl.ConditionalBranch)
                                {
                                    break;
                                }

                                session.GetOrAddItem(checked((uint)previousInstruction.Address))
                                    .SetFlags(AnalysisFlags.FinallyExit);
                            }

                            item.SetFlags(AnalysisFlags.FinallyExit);
                            markNextInstructionAsFinallyExit = true;
                        }

                        DecodedInstruction knownCallInstruction = instruction;
                        if (instruction.NearBranchTarget is null
                            && uniqueResolvedCallTargets.Length == 1)
                        {
                            knownCallInstruction = instruction with
                            {
                                NearBranchTarget = uniqueResolvedCallTargets[0]
                            };
                        }

                        classTypeInstruction = knownCallInstruction;
                        ApplyKnownCallDataTypeCandidate(
                            knownCallInstruction,
                            session,
                            registerAddressValues,
                            classRegisterTypes);
                        ApplyKnownCallResourceString(
                            knownCallInstruction,
                            session,
                            registerAddressValues,
                            item);
                        ApplyKnownCallLocalDataTypes(
                            knownCallInstruction,
                            session,
                            registerLocalOffsets,
                            registerAddressValues,
                            classRegisterTypes,
                            stackLocalVariables);
                        ApplyKnownCallStackArgumentTypes(
                            knownCallInstruction,
                            session,
                            registerStackArgumentOffsets,
                            registerAddressValues,
                            classRegisterTypes,
                            stackArguments);
                        returnTypeCandidate = GetKnownCallReturnTypeCandidate(
                            knownCallInstruction,
                            session,
                            classRegisterTypes);
                        pendingTlsBase = string.Equals(
                            returnTypeCandidate,
                            "#TLS",
                            StringComparison.Ordinal);
                        pendingReturnCallTarget = knownCallInstruction.NearBranchTarget is ulong callTarget
                            && callTarget <= uint.MaxValue
                                ? (uint)callTarget
                                : null;
                    }
                    else if (WritesRegisterArgument(instruction, 0))
                    {
                        pendingReturnCallTarget = null;
                        returnTypeCandidate = instruction.Mnemonic.StartsWith(
                            "Set",
                            StringComparison.OrdinalIgnoreCase)
                            ? "Boolean"
                            : null;
                    }

                    ApplyKnownClassFieldAccess(instruction, session, classRegisterTypes, item);
                    ApplyKnownClassStoreTypeCandidate(
                        instruction,
                        session,
                        classRegisterTypes,
                        registerAddressValues,
                        startItem,
                        stackLocalVariables);
                    string? returnedClassType = instruction.FlowControl == InstructionFlowControl.Return
                        && !startItem.Flags.HasFlag(AnalysisFlags.ConstructorCandidate)
                        && !startItem.Flags.HasFlag(AnalysisFlags.DestructorCandidate)
                        && classRegisterTypes.TryGetValue(0, out string? eaxClassType)
                            ? eaxClassType
                            : null;
                    UpdateClassRegisterTypes(
                        classTypeInstruction,
                        session,
                        classRegisterTypes,
                        stackLocalVariables,
                        stackArguments,
                        registerAddressValues);
                    if (returnTypeCandidate is null && returnedClassType is not null)
                    {
                        returnTypeCandidate = returnedClassType;
                    }

                    UpdateRegisterAddressValues(instruction, session, registerAddressValues);
                    UpdateRegisterLocalOffsets(instruction, session, registerLocalOffsets);
                    UpdateRegisterStackArgumentOffsets(
                        instruction,
                        session,
                        registerStackArgumentOffsets);
                }

                if (isProcedureStart && startItem.UsesFramePointer)
                {
                    foreach (DecodedOperand operand in instruction.Operands)
                    {
                        if (operand.Kind == DecodedOperandKind.Memory
                            && string.Equals(
                                operand.BaseRegister,
                                "EBP",
                                StringComparison.OrdinalIgnoreCase)
                            && operand.IndexRegister is null
                            && operand.Displacement is >= 8 and <= 0x10000
                            && (operand.MemorySizeBytes is > 0 and <= int.MaxValue
                                || string.Equals(
                                    instruction.Mnemonic,
                                    "Lea",
                                    StringComparison.OrdinalIgnoreCase)
                                && instruction.Operands.Count == 2
                                && instruction.Operands[0].Kind == DecodedOperandKind.Register))
                        {
                            uint offset = (uint)operand.Displacement.Value;
                            int accessSize = operand.MemorySizeBytes is > 0 and <= int.MaxValue
                                ? operand.MemorySizeBytes.Value
                                : sizeof(uint);
                            int argumentSize = checked((Math.Max(accessSize, sizeof(uint)) + 3) / 4 * 4);
                            StackArgument argument = new(
                                offset,
                                argumentSize,
                                accessSize == 10 ? "Extended" : null);
                            if (!stackArguments.TryGetValue(offset, out StackArgument? previous)
                                || argument.SizeBytes > previous.SizeBytes)
                            {
                                stackArguments[offset] = argument;
                            }
                        }
                    }

                    ApplyKnownClassStackArgumentStoreTypeCandidate(
                        instruction,
                        classRegisterTypes,
                        stackArguments);
                }

                if (string.Equals(instruction.Mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase))
                {
                    lastCompareAddress = currentRva;
                }

                AddDataAddressCrossReferences(instruction, session, registerAddressValues);

                if (isProcedureStart
                    && instruction.Operands.Count > 1
                    && instruction.Operands[1] is
                    {
                        Kind: DecodedOperandKind.Immediate,
                        Immediate: <= uint.MaxValue
                    } immediateOperand
                    && TryVaToRva(session, (uint)immediateOperand.Immediate.Value, out uint targetRva)
                    && TryGetCodeSection(session, targetRva, out _, out _))
                {
                    codePointerCandidates.Add((currentRva, targetRva));
                }

                uint? storedBranchTarget = null;
                bool isHaltCall = false;
                if (instruction.NearBranchTarget is ulong branchTarget
                    && instruction.FlowControl is InstructionFlowControl.Call
                        or InstructionFlowControl.ConditionalBranch
                        or InstructionFlowControl.UnconditionalBranch)
                {
                    if (branchTarget > uint.MaxValue)
                    {
                        diagnostics.Add($"Control-flow target 0x{branchTarget:X} is outside the 32-bit address range.");
                    }
                    else
                    {
                        storedBranchTarget = (uint)branchTarget;
                        CrossReferenceKind referenceKind = instruction.FlowControl == InstructionFlowControl.Call
                            ? CrossReferenceKind.Call
                            : CrossReferenceKind.Jump;
                        session.AddCrossReference(currentRva, (uint)branchTarget, referenceKind);
                        if (referenceKind == CrossReferenceKind.Jump
                            && session.Items.TryGetValue((uint)branchTarget, out AnalysisItem? handler)
                            && TryGetExceptionHandlerFlag(handler.Name, out AnalysisFlags handlerFlag))
                        {
                            item.SetFlags(handlerFlag);
                            handler.SetFlags(handlerFlag);
                        }

                        if (referenceKind == CrossReferenceKind.Call)
                        {
                            item.SetFlags(AnalysisFlags.Call);
                            if (session.Items.TryGetValue((uint)branchTarget, out AnalysisItem? callee))
                            {
                                if (string.Equals(callee.Name, "@Halt0", StringComparison.OrdinalIgnoreCase))
                                {
                                    isHaltCall = true;
                                }
                                else if (string.Equals(
                                    callee.Name,
                                    "@ClassCreate",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    startItem.SetFlags(AnalysisFlags.ConstructorCandidate);
                                }
                                else if (string.Equals(
                                    callee.Name,
                                    "@ClassDestroy",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    startItem.SetFlags(AnalysisFlags.DestructorCandidate);
                                }
                                else if (isProcedureStart
                                    && string.Equals(
                                        callee.Name,
                                        "@ClassCreate",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    startItem.SetFlags(AnalysisFlags.ConstructorCandidate);
                                }
                                else if (isProcedureStart
                                    && string.Equals(
                                        callee.Name,
                                        "@ClassDestroy",
                                        StringComparison.OrdinalIgnoreCase))
                                {
                                    startItem.SetFlags(AnalysisFlags.DestructorCandidate);
                                }
                            }

                            if (!isCallPopSequence
                                && TryGetCodeSection(session, (uint)branchTarget, out _, out _))
                            {
                                session.GetOrAddItem((uint)branchTarget)
                                    .SetFlags(AnalysisFlags.ProcedureStart);
                                if (queuedStarts.Add((uint)branchTarget))
                                {
                                    codeStarts.Enqueue(((uint)branchTarget, true));
                                }
                            }
                        }
                        else
                        {
                            if (branchTarget < currentRva
                                && (item.Flags & (
                                    AnalysisFlags.ExceptionHandler
                                    | AnalysisFlags.FinallyHandler)) == AnalysisFlags.None
                                && TryGetCodeSection(session, (uint)branchTarget, out _, out _))
                            {
                                session.GetOrAddItem((uint)branchTarget).SetFlags(AnalysisFlags.Loop);
                            }

                            if ((instruction.FlowControl is InstructionFlowControl.ConditionalBranch
                                or InstructionFlowControl.UnconditionalBranch)
                                && TryGetCodeSection(session, (uint)branchTarget, out _, out _)
                                && queuedStarts.Add((uint)branchTarget))
                            {
                                codeStarts.Enqueue(((uint)branchTarget, false));
                            }
                        }
                    }
                }

                session.AddDisassemblyLine(new DisassemblyLine(
                    currentRva,
                    Convert.ToHexString(session.Image.Span.Slice(currentRawOffset, instruction.Length)),
                    instruction.Mnemonic,
                    instruction.FormattedText,
                    storedBranchTarget,
                    instruction.FlowControl));
                if (isProcedureStart)
                {
                    procedureInstructions.Add(instruction);
                }

                recentInstructions.Add(instruction);
                if (recentInstructions.Count > 6)
                {
                    recentInstructions.RemoveAt(0);
                }

                if (TryMatchDelphiTryBegin(
                        session,
                        recentInstructions,
                        out uint tryStartRva,
                        out uint handlerRva)
                    && TryGetCodeSection(session, handlerRva, out _, out _))
                {
                    AnalysisItem region = session.GetOrAddItem(tryStartRva);
                    region.SetFlags(AnalysisFlags.ExceptionRegion);
                    region.ExceptionRegionHandlerAddress = handlerRva;
                    session.GetOrAddItem(handlerRva).SetFlags(AnalysisFlags.ExceptionHandler);
                    if (queuedStarts.Add(handlerRva))
                    {
                        codeStarts.Enqueue((handlerRva, false));
                    }
                }

                if (TryMatchDelphiFinallyEnd(
                        session,
                        recentInstructions,
                        out uint finallyStartRva,
                        out uint finallyEndRva))
                {
                    AnalysisItem finallyRegion = session.GetOrAddItem(finallyStartRva);
                    finallyRegion.SetFlags(AnalysisFlags.FinallyRegion);
                    finallyRegion.FinallyRegionEndAddress = finallyEndRva;
                }

                if (TryMatchDelphiInt64Comparison(
                        session,
                        decodedInstructions,
                        instruction,
                        out DecodedInstruction[] int64Pattern,
                        out uint int64EndAddress))
                {
                    foreach (int patternIndex in new[] { 0, 1, 3, 4 })
                    {
                        session.GetOrAddItem(checked((uint)int64Pattern[patternIndex].Address))
                            .SetFlags(AnalysisFlags.Int64Comparison);
                    }

                    session.GetOrAddItem(checked((uint)int64Pattern[0].Address))
                        .Int64ComparisonEndAddress = int64EndAddress;
                }

                if (TryMatchDelphiInt64StackComparison(
                        session,
                        decodedInstructions,
                        instruction,
                        out uint int64StackStart,
                        out uint int64StackCompare,
                        out uint int64StackBranch,
                        out uint int64StackEnd))
                {
                    session.GetOrAddItem(int64StackStart).Int64ComparisonEndAddress = int64StackEnd;
                    session.GetOrAddItem(int64StackCompare).SetFlags(AnalysisFlags.Int64Comparison);
                    session.GetOrAddItem(int64StackBranch).SetFlags(AnalysisFlags.Int64Comparison);
                }

                if (TryMatchDelphiInt64StackComparisonWithSplitCleanup(
                        session,
                        decodedInstructions,
                        instruction,
                        out DecodedInstruction[] int64SplitPattern,
                        out uint int64SplitStart,
                        out uint int64SplitEnd))
                {
                    foreach (int patternIndex in new[] { 0, 1, 3, 4, 5, 6 })
                    {
                        session.GetOrAddItem(checked((uint)int64SplitPattern[patternIndex].Address))
                            .SetFlags(AnalysisFlags.Int64Comparison);
                    }

                    session.GetOrAddItem(int64SplitStart)
                        .Int64ComparisonEndAddress = int64SplitEnd;
                }

                TryAnalyzeSwitchJumpTable(
                    session,
                    instruction,
                    lastCompareAddress,
                    codeStart,
                    codeStarts,
                    queuedStarts,
                    cancellationToken);

                uint nextRva = checked(currentRva + (uint)instruction.Length);
                if (instruction.FlowControl == InstructionFlowControl.UnconditionalBranch
                    && storedBranchTarget is uint exceptionHandlerRva)
                {
                    AnalyzeExceptionHandlerTable(
                        session,
                        exceptionHandlerRva,
                        nextRva,
                        codeStarts,
                        queuedStarts,
                        diagnostics,
                        cancellationToken);
                }

                currentRva = nextRva;

                if (isHaltCall)
                {
                    reachedTerminator = true;
                    break;
                }

                if (instruction.FlowControl is InstructionFlowControl.Return
                    or InstructionFlowControl.UnconditionalBranch
                    or InstructionFlowControl.IndirectBranch
                    or InstructionFlowControl.Interrupt)
                {
                    reachedTerminator = true;
                    if (instruction.FlowControl is InstructionFlowControl.Return or InstructionFlowControl.Interrupt)
                    {
                        procedureEnd = checked(currentRva - (uint)instruction.Length);
                        if (isProcedureStart)
                        {
                            startItem.ProcedureSizeBytes = checked(currentRva - codeStart);
                            startItem.StackArguments = stackArguments.Values
                                .OrderBy(argument => argument.Offset)
                                .ToArray();
                            startItem.StackLocalVariables = stackLocalVariables.Values
                                .OrderBy(variable => variable.EbpOffset)
                                .ToArray();
                            startItem.RegisterArgumentCandidates = GetRegisterArgumentCandidates(
                                usedRegisterArguments);
                            if (instruction.FlowControl == InstructionFlowControl.Return)
                            {
                                ushort returnStackBytes = GetReturnStackBytes(instruction);
                                startItem.ReturnStackBytes = returnStackBytes;
                                if (stackArguments.Values.Sum(argument => (long)argument.SizeBytes)
                                    > returnStackBytes)
                                {
                                    startItem.SetFlags(AnalysisFlags.StackArgumentSizeMismatch);
                                }

                                startItem.ReturnTypeCandidate = returnTypeCandidate;
                                if (pendingReturnCallTarget is uint callTarget)
                                {
                                    returnedCallTargets[codeStart] = callTarget;
                                }
                            }
                        }
                    }

                    break;
                }
            }

            if (isProcedureStart && procedureEnd is not null)
            {
                foreach ((uint sourceRva, uint targetRva) in codePointerCandidates)
                {
                    if (targetRva >= codeStart && targetRva < currentRva)
                    {
                        continue;
                    }

                    AnalysisItem targetItem = session.GetOrAddItem(targetRva);
                    bool isUnclassified = targetItem.Flags == AnalysisFlags.None
                        && targetItem.Name is null;
                    if (!isUnclassified
                        && !targetItem.Flags.HasFlag(AnalysisFlags.ProcedureStart))
                    {
                        continue;
                    }
                    if (isUnclassified && !LooksLikeProcedure(session, targetRva))
                    {
                        continue;
                    }

                    AddConstantCrossReferenceIfMissing(session, sourceRva, targetRva);
                    targetItem.SetFlags(AnalysisFlags.Code | AnalysisFlags.ProcedureStart);
                    if (queuedStarts.Add(targetRva))
                    {
                        codeStarts.Enqueue((targetRva, true));
                    }
                }
            }

            if (decodedCount == MaximumInstructionsPerProcedure && !reachedTerminator)
            {
                diagnostics.Add(codeStart == session.EntryPointRva
                    ? $"Entry-point disassembly stopped after {MaximumInstructionsPerProcedure} instructions."
                    : isProcedureStart
                        ? $"Procedure at RVA 0x{codeStart:X8} stopped after {MaximumInstructionsPerProcedure} instructions."
                        : $"Code path at RVA 0x{codeStart:X8} stopped after {MaximumInstructionsPerProcedure} instructions.");
            }

            if (procedureEnd is uint endAddress)
            {
                session.GetOrAddItem(endAddress).SetFlags(AnalysisFlags.ProcedureEnd);
            }

            completedStarts++;
            progress?.Report(new AnalysisProgress(
                "CodeScan",
                completedStarts,
                MaximumCodeStarts));
        }

        if (codeStarts.Count > 0)
        {
            diagnostics.Add(
                $"Code scan stopped after {MaximumCodeStarts} code starts.");
        }

        PropagateCallReturnTypeCandidates(session, returnedCallTargets);
        MarkFunctionCandidates(session, functionCandidateTargets);
    }

    private bool IsCallToNextInstructionFollowedByPop(
        AnalysisSession session,
        DecodedInstruction instruction)
    {
        if (instruction.FlowControl != InstructionFlowControl.Call
            || instruction.NearBranchTarget is not ulong target
            || target > uint.MaxValue
            || instruction.Address > uint.MaxValue
            || instruction.Length <= 0
            || target != instruction.Address + (uint)instruction.Length
            || !TryGetCodeSection(session, (uint)target, out PeSection? section, out int rawOffset))
        {
            return false;
        }

        ulong rawEnd = (ulong)section.RawAddress + section.RawSize;
        if (rawEnd > (ulong)session.Image.Length
            || rawOffset < section.RawAddress
            || (ulong)rawOffset >= rawEnd)
        {
            return false;
        }

        int remainingBytes = checked((int)(rawEnd - (ulong)rawOffset));
        DecodedInstruction nextInstruction;
        try
        {
            nextInstruction = _instructionDecoder.Decode(
                session.Image.Span.Slice(rawOffset, remainingBytes),
                target,
                32);
        }
        catch (InstructionDecodeException)
        {
            return false;
        }

        return nextInstruction.Address == target
            && nextInstruction.Length > 0
            && nextInstruction.Length <= remainingBytes
            && string.Equals(nextInstruction.Mnemonic, "Pop", StringComparison.OrdinalIgnoreCase)
            && nextInstruction.Operands.Count == 1
            && nextInstruction.Operands[0].Kind == DecodedOperandKind.Register;
    }

    private bool LooksLikeProcedure(AnalysisSession session, uint startRva)
    {
        if (!TryGetCodeSection(session, startRva, out PeSection section, out int rawOffset))
        {
            return false;
        }

        int sectionRawEnd = checked((int)((ulong)section.RawAddress + section.RawSize));
        bool hasFramePrologue = false;
        bool hasMatchingPushPopEpilogue = false;
        string? firstPushedRegister = null;
        string? lastPoppedRegister = null;
        int instructionCount = 0;
        uint currentRva = startRva;
        int currentRawOffset = rawOffset;
        while (instructionCount < MaximumInstructionsPerProcedure
            && currentRawOffset < sectionRawEnd)
        {
            int remainingBytes = sectionRawEnd - currentRawOffset;
            DecodedInstruction instruction;
            try
            {
                instruction = _instructionDecoder.Decode(
                    session.Image.Span.Slice(currentRawOffset, remainingBytes),
                    currentRva,
                    32);
            }
            catch (InstructionDecodeException)
            {
                return false;
            }

            if (instruction.Address != currentRva
                || instruction.Length <= 0
                || instruction.Length > remainingBytes)
            {
                return false;
            }

            if (instructionCount == 0
                && instruction.Operands.Count == 1
                && string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
                && instruction.Operands[0].Kind == DecodedOperandKind.Register)
            {
                firstPushedRegister = instruction.Operands[0].Register;
            }
            else if (instructionCount == 1
                && firstPushedRegister is not null
                && string.Equals(firstPushedRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
                && instruction.Operands.Count == 2
                && IsRegisterOperand(instruction.Operands[0], "EBP")
                && IsRegisterOperand(instruction.Operands[1], "ESP"))
            {
                hasFramePrologue = true;
            }

            if (string.Equals(instruction.Mnemonic, "Pop", StringComparison.OrdinalIgnoreCase)
                && instruction.Operands.Count == 1
                && instruction.Operands[0].Kind == DecodedOperandKind.Register)
            {
                lastPoppedRegister = instruction.Operands[0].Register;
            }
            else if (instruction.FlowControl != InstructionFlowControl.Return)
            {
                lastPoppedRegister = null;
            }

            instructionCount++;
            if (instruction.FlowControl == InstructionFlowControl.Return)
            {
                hasMatchingPushPopEpilogue = firstPushedRegister is not null
                    && string.Equals(
                        firstPushedRegister,
                        lastPoppedRegister,
                        StringComparison.OrdinalIgnoreCase);
                return hasFramePrologue || hasMatchingPushPopEpilogue;
            }

            if (instruction.FlowControl != InstructionFlowControl.Next)
            {
                return false;
            }

            currentRva = checked(currentRva + (uint)instruction.Length);
            currentRawOffset = checked(currentRawOffset + instruction.Length);
        }

        return false;
    }

    private static ushort GetReturnStackBytes(DecodedInstruction instruction)
    {
        if (instruction.Operands.Count == 0)
        {
            return 0;
        }

        DecodedOperand operand = instruction.Operands[0];
        if (operand.Kind != DecodedOperandKind.Immediate
            || operand.Immediate is not ulong value
            || value > ushort.MaxValue)
        {
            throw new InvalidDataException(
                $"The decoder returned an invalid return-stack adjustment at RVA 0x{instruction.Address:X8}.");
        }

        return (ushort)value;
    }

    private static void PropagateCallReturnTypeCandidates(
        AnalysisSession session,
        IReadOnlyDictionary<uint, uint> returnedCallTargets)
    {
        bool changed;
        do
        {
            changed = false;
            foreach ((uint procedureAddress, uint callTargetAddress) in returnedCallTargets)
            {
                if (!session.Items.TryGetValue(procedureAddress, out AnalysisItem? procedure)
                    || procedure.ReturnTypeCandidate is not null
                    || !session.Items.TryGetValue(callTargetAddress, out AnalysisItem? target)
                    || target.ReturnTypeCandidate is null)
                {
                    continue;
                }

                procedure.ReturnTypeCandidate = target.ReturnTypeCandidate;
                changed = true;
            }
        }
        while (changed);
    }

    private static void MarkFunctionCandidates(
        AnalysisSession session,
        IEnumerable<uint> functionCandidateTargets)
    {
        foreach (uint targetAddress in functionCandidateTargets)
        {
            if (!session.Items.TryGetValue(targetAddress, out AnalysisItem? target)
                || target.Flags.HasFlag(AnalysisFlags.Import)
                || target.Flags.HasFlag(AnalysisFlags.ConstructorCandidate)
                || target.Flags.HasFlag(AnalysisFlags.DestructorCandidate)
                || string.Equals(target.Name, "@ClassCreate", StringComparison.OrdinalIgnoreCase)
                || string.Equals(target.Name, "@ClassDestroy", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            target.SetFlags(AnalysisFlags.FunctionCandidate);
        }
    }

    private static bool IsX87ResultStore(DecodedInstruction instruction) =>
        string.Equals(instruction.Mnemonic, "Fst", StringComparison.OrdinalIgnoreCase)
        || string.Equals(instruction.Mnemonic, "Fstp", StringComparison.OrdinalIgnoreCase);

    private static bool ReadsRegisterArgument(DecodedInstruction instruction, int argumentIndex)
    {
        bool[] available = [true, true, true];
        bool[] used = [false, false, false];
        AnalyzeRegisterArgumentCandidates(instruction, available, used, preservesRegisterArguments: false);
        return used[argumentIndex];
    }

    private static void AnalyzeStackInstruction(
        DecodedInstruction instruction,
        uint currentRva,
        uint codeStart,
        bool isProcedureStart,
        AnalysisItem procedureStartItem,
        AnalysisItem instructionItem)
    {
        if (string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 1)
        {
            instructionItem.SetFlags(AnalysisFlags.StackPush);
            instructionItem.StackPointerDeltaBytes = -sizeof(uint);
            if (currentRva == codeStart
                && IsRegisterOperand(instruction.Operands[0], "EBP"))
            {
                instructionItem.SetFlags(AnalysisFlags.FrameInstruction);
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Pop", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 1)
        {
            instructionItem.SetFlags(AnalysisFlags.StackPop);
            instructionItem.StackPointerDeltaBytes = sizeof(uint);
            return;
        }

        if (string.Equals(instruction.Mnemonic, "Enter", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Immediate
            && instruction.Operands[0].Immediate is ulong localAllocation
            && localAllocation <= ushort.MaxValue
            && instruction.Operands[1].Kind == DecodedOperandKind.Immediate
            && instruction.Operands[1].Immediate is ulong nestingLevel
            && nestingLevel <= 31)
        {
            instructionItem.SetFlags(AnalysisFlags.FrameInstruction | AnalysisFlags.StackAdjustment);
            instructionItem.StackPointerDeltaBytes = -(
                sizeof(uint) * (1L + (long)nestingLevel) + (long)localAllocation);
            if (isProcedureStart)
            {
                procedureStartItem.UsesFramePointer = true;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Leave", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 0)
        {
            instructionItem.SetFlags(AnalysisFlags.FrameInstruction);
            return;
        }

        if (string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && (IsRegisterOperand(instruction.Operands[0], "EBP")
                    && IsRegisterOperand(instruction.Operands[1], "ESP")
                || IsRegisterOperand(instruction.Operands[0], "ESP")
                    && IsRegisterOperand(instruction.Operands[1], "EBP")))
        {
            instructionItem.SetFlags(AnalysisFlags.FrameInstruction);
            if (isProcedureStart
                && IsRegisterOperand(instruction.Operands[0], "EBP"))
            {
                procedureStartItem.UsesFramePointer = true;
            }

            return;
        }

        if (instruction.Operands.Count != 2
            || !IsRegisterOperand(instruction.Operands[0], "ESP")
            || instruction.Operands[1].Kind != DecodedOperandKind.Immediate
            || instruction.Operands[1].Immediate is not ulong amount
            || !(string.Equals(instruction.Mnemonic, "Add", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Sub", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        long adjustment = amount > uint.MaxValue
            ? unchecked((long)amount)
            : (long)amount;
        if (string.Equals(instruction.Mnemonic, "Sub", StringComparison.OrdinalIgnoreCase))
        {
            adjustment = -adjustment;
        }

        instructionItem.SetFlags(AnalysisFlags.StackAdjustment);
        instructionItem.StackPointerDeltaBytes = adjustment;
    }

    private static void AnalyzeRegisterArgumentCandidates(
        DecodedInstruction instruction,
        bool[] available,
        bool[] used,
        bool preservesRegisterArguments)
    {
        string mnemonic = instruction.Mnemonic;
        bool isCall = instruction.FlowControl == InstructionFlowControl.Call;
        bool isPush = string.Equals(mnemonic, "Push", StringComparison.OrdinalIgnoreCase);
        if (string.Equals(mnemonic, "Pushad", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Pusha", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            MarkRegisterArgumentRead("EDX", available, used);
            MarkRegisterArgumentRead("ECX", available, used);
            return;
        }

        if (string.Equals(mnemonic, "Popad", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Popa", StringComparison.OrdinalIgnoreCase))
        {
            Array.Fill(available, false);
            return;
        }

        if (string.Equals(mnemonic, "Cpuid", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            MarkRegisterArgumentRead("ECX", available, used);
            Array.Fill(available, false);
            return;
        }

        if (string.Equals(mnemonic, "Rdpmc", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("ECX", available, used);
            available[0] = false;
            available[1] = false;
            return;
        }

        if (string.Equals(mnemonic, "Rdtsc", StringComparison.OrdinalIgnoreCase))
        {
            available[0] = false;
            available[1] = false;
            return;
        }

        if (string.Equals(mnemonic, "Rdtscp", StringComparison.OrdinalIgnoreCase))
        {
            Array.Fill(available, false);
            return;
        }

        if (string.Equals(mnemonic, "Rdmsr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Xgetbv", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("ECX", available, used);
            available[0] = false;
            available[1] = false;
            return;
        }

        if (string.Equals(mnemonic, "Wrmsr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Xsetbv", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            MarkRegisterArgumentRead("EDX", available, used);
            MarkRegisterArgumentRead("ECX", available, used);
            return;
        }

        if (string.Equals(mnemonic, "Out", StringComparison.OrdinalIgnoreCase))
        {
            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register)
                {
                    MarkRegisterArgumentRead(operand.Register, available, used);
                }
            }

            return;
        }

        if (string.Equals(mnemonic, "Cmpxchg8b", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            MarkRegisterArgumentRead("EDX", available, used);
            MarkRegisterArgumentRead("ECX", available, used);
            available[0] = false;
            available[1] = false;
            return;
        }

        if (string.Equals(mnemonic, "Sahf", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            return;
        }

        if (string.Equals(mnemonic, "Lahf", StringComparison.OrdinalIgnoreCase))
        {
            available[0] = false;
            return;
        }

        if (IsBcdAccumulatorInstruction(mnemonic))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            available[0] = false;
            return;
        }

        if (IsStringInstruction(mnemonic))
        {
            if (IsStringLoad(mnemonic))
            {
                available[0] = false;
            }
            else if (IsStringAccumulatorInput(mnemonic))
            {
                MarkRegisterArgumentRead("EAX", available, used);
            }

            if (IsStringPortInstruction(mnemonic))
            {
                MarkRegisterArgumentRead("EDX", available, used);
            }

            if (instruction.HasRepeatPrefix)
            {
                MarkRegisterArgumentRead("ECX", available, used);
                available[2] = false;
            }

            return;
        }

        if (IsLoopInstruction(mnemonic))
        {
            MarkRegisterArgumentRead("ECX", available, used);
            available[2] = false;
            return;
        }

        if (string.Equals(mnemonic, "Jecxz", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Jcxz", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("ECX", available, used);
            return;
        }

        if (string.Equals(mnemonic, "Cdq", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Cwd", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            available[1] = false;
            return;
        }

        if (string.Equals(mnemonic, "Cbw", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Cwde", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            available[0] = false;
            return;
        }

        bool isImplicitMultiplyOrDivide = instruction.Operands.Count == 1
            && (string.Equals(mnemonic, "Mul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mnemonic, "Imul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mnemonic, "Div", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mnemonic, "Idiv", StringComparison.OrdinalIgnoreCase));
        if (isImplicitMultiplyOrDivide)
        {
            MarkRegisterArgumentRead("EAX", available, used);
            if (string.Equals(mnemonic, "Div", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mnemonic, "Idiv", StringComparison.OrdinalIgnoreCase))
            {
                MarkRegisterArgumentRead("EDX", available, used);
            }

            DecodedOperand source = instruction.Operands[0];
            if (source.Kind == DecodedOperandKind.Register)
            {
                MarkRegisterArgumentRead(source.Register, available, used);
            }
            else if (source.Kind == DecodedOperandKind.Memory)
            {
                MarkRegisterArgumentRead(source.BaseRegister, available, used);
                MarkRegisterArgumentRead(source.IndexRegister, available, used);
            }

            available[0] = false;
            available[1] = false;
            return;
        }

        bool isCompare = string.Equals(mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Test", StringComparison.OrdinalIgnoreCase);
        bool isReadOnlyRegisterDestination = IsReadOnlyRegisterDestination(mnemonic);
        bool isReadModifyWrite = isCompare
            || string.Equals(mnemonic, "Add", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Adc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Sub", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Sbb", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "And", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Or", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Xor", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Arpl", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Inc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Dec", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Neg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Not", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Shl", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Shr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Sar", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Rol", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Ror", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Rcl", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Rcr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Bt", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Bts", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Btr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Btc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Shld", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Shrd", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Cmpxchg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Xadd", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Bswap", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Adcx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Adox", StringComparison.OrdinalIgnoreCase)
            || mnemonic.StartsWith("Cmov", StringComparison.OrdinalIgnoreCase)
            || string.Equals(mnemonic, "Imul", StringComparison.OrdinalIgnoreCase)
                && instruction.Operands.Count == 2;
        bool isSelfXor = string.Equals(mnemonic, "Xor", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && instruction.Operands[1].Kind == DecodedOperandKind.Register
            && string.Equals(
                instruction.Operands[0].Register,
                instruction.Operands[1].Register,
                StringComparison.OrdinalIgnoreCase);

        if (!isCall)
        {
            for (int operandIndex = 0; operandIndex < instruction.Operands.Count; operandIndex++)
            {
                DecodedOperand operand = instruction.Operands[operandIndex];
                if (operand.Kind == DecodedOperandKind.Register)
                {
                    bool readsRegister = !isSelfXor
                        && (operandIndex > 0
                            || isReadModifyWrite
                            || isReadOnlyRegisterDestination
                            || isPush);
                    if (readsRegister)
                    {
                        MarkRegisterArgumentRead(operand.Register, available, used);
                    }
                }
                else if (operand.Kind == DecodedOperandKind.Memory)
                {
                    MarkRegisterArgumentRead(operand.BaseRegister, available, used);
                    MarkRegisterArgumentRead(operand.IndexRegister, available, used);
                }
            }
        }
        else if (instruction.NearBranchTarget is null)
        {
            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register)
                {
                    MarkRegisterArgumentRead(operand.Register, available, used);
                }
                else if (operand.Kind == DecodedOperandKind.Memory)
                {
                    MarkRegisterArgumentRead(operand.BaseRegister, available, used);
                    MarkRegisterArgumentRead(operand.IndexRegister, available, used);
                }
            }
        }

        if (string.Equals(mnemonic, "Cmpxchg", StringComparison.OrdinalIgnoreCase))
        {
            MarkRegisterArgumentRead("EAX", available, used);
            available[0] = false;
        }

        if (string.Equals(mnemonic, "Xadd", StringComparison.OrdinalIgnoreCase))
        {
            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register
                    && GetRegisterArgumentIndex(operand.Register) is int index)
                {
                    available[index] = false;
                }
            }
        }

        if (IsRegisterDestinationWritten(instruction))
        {
            int? destinationIndex = GetRegisterArgumentIndex(instruction.Operands[0].Register);
            if (destinationIndex is int index)
            {
                available[index] = false;
            }
        }

        if (string.Equals(mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase))
        {
            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register
                    && GetRegisterArgumentIndex(operand.Register) is int index)
                {
                    available[index] = false;
                }
            }
        }

        if (isCall && !preservesRegisterArguments)
        {
            Array.Fill(available, false);
        }
    }

    private static bool IsStringInstruction(string mnemonic) =>
        mnemonic is "Cmpsb" or "Cmpsd" or "Cmpsw"
            or "Insb" or "Insd" or "Insw"
            or "Lodsb" or "Lodsd" or "Lodsw"
            or "Movsb" or "Movsd" or "Movsw"
            or "Outsb" or "Outsd" or "Outsw"
            or "Scasb" or "Scasd" or "Scasw"
            or "Stosb" or "Stosd" or "Stosw";

    private static bool IsBcdAccumulatorInstruction(string mnemonic) =>
        mnemonic is "Aaa" or "Aad" or "Aam" or "Aas" or "Daa" or "Das";

    private static bool IsStringLoad(string mnemonic) =>
        mnemonic is "Lodsb" or "Lodsd" or "Lodsw";

    private static bool IsStringAccumulatorInput(string mnemonic) =>
        mnemonic is "Scasb" or "Scasd" or "Scasw"
            or "Stosb" or "Stosd" or "Stosw";

    private static bool IsStringPortInstruction(string mnemonic) =>
        mnemonic is "Insb" or "Insd" or "Insw"
            or "Outsb" or "Outsd" or "Outsw";

    private static bool IsLoopInstruction(string mnemonic) =>
        mnemonic is "Loop" or "Loope" or "Loopne";

    private static bool IsReadOnlyRegisterDestination(string mnemonic) =>
        mnemonic is "Bound" or "Verr" or "Verw";

    private static bool IsRegisterPreservingCall(
        DecodedInstruction instruction,
        AnalysisSession session)
    {
        return instruction.FlowControl == InstructionFlowControl.Call
            && instruction.NearBranchTarget is ulong targetAddress
            && targetAddress <= uint.MaxValue
            && session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target)
            && (string.Equals(target.Name, "@IntOver", StringComparison.OrdinalIgnoreCase)
                || string.Equals(target.Name, "@BoundErr", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsTryFinallyExitCall(
        DecodedInstruction instruction,
        AnalysisSession session)
    {
        return instruction.FlowControl == InstructionFlowControl.Call
            && instruction.NearBranchTarget is ulong targetAddress
            && targetAddress <= uint.MaxValue
            && session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target)
            && string.Equals(target.Name, "@TryFinallyExit", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryResolveDynamicMethodTarget(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        IReadOnlyList<DecodedInstruction> precedingInstructions,
        out uint methodAddress)
    {
        methodAddress = 0;
        if (instruction.NearBranchTarget is not ulong helperAddress
            || helperAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)helperAddress, out AnalysisItem? helper)
            || !classRegisterTypes.TryGetValue(0, out string? className))
        {
            return false;
        }

        string helperName = helper.Name is string resolvedName && resolvedName.StartsWith('@')
            ? resolvedName[1..]
            : helper.Name ?? string.Empty;
        string? idRegister;
        if (string.Equals(helperName, "CallDynaInst", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "CallDynaClass", StringComparison.OrdinalIgnoreCase))
        {
            idRegister = session.SelectedDelphiVersion == DelphiVersion.Unknown
                ? null
                : session.SelectedDelphiVersion <= DelphiVersion.Delphi5
                    ? "BX"
                    : "SI";
        }
        else if (string.Equals(helperName, "FindDynaInst", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "FindDynaClass", StringComparison.OrdinalIgnoreCase))
        {
            idRegister = "DX";
        }
        else
        {
            return false;
        }

        if (idRegister is null
            || !TryGetRecentImmediateRegisterValue(precedingInstructions, idRegister, session, out ushort methodId))
        {
            return false;
        }

        AnalysisItem? vmt = session.Items.Values.FirstOrDefault(item =>
            item.Flags.HasFlag(AnalysisFlags.Vmt)
            && string.Equals(item.Name, className, StringComparison.Ordinal));
        HashSet<uint> visitedVmts = [];
        while (vmt is not null && visitedVmts.Add(vmt.Address))
        {
            DelphiVmtDynamicMethod? method = vmt.DynamicMethods.FirstOrDefault(candidate =>
                candidate.MessageId == methodId);
            if (method?.CodeAddress is uint targetAddress
                && TryGetCodeSection(session, targetAddress, out _, out _))
            {
                methodAddress = targetAddress;
                return true;
            }

            vmt = vmt.ParentAddress is uint parentAddress
                && session.Items.TryGetValue(parentAddress, out AnalysisItem? parent)
                && parent.Flags.HasFlag(AnalysisFlags.Vmt)
                    ? parent
                    : null;
        }

        return false;
    }

    private static bool TryResolveVirtualMethodTarget(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        out uint methodAddress)
    {
        methodAddress = 0;
        if (instruction.FlowControl != InstructionFlowControl.Call
            || instruction.NearBranchTarget is not null
            || instruction.Operands.Count == 0
            || instruction.Operands[0] is not
            {
                Kind: DecodedOperandKind.Memory,
                IndexRegister: null,
                Displacement: <= int.MaxValue
            } callTarget
            || GetClassRegisterTypeIndex(callTarget.BaseRegister) is not int baseRegisterIndex
            || !classRegisterTypes.TryGetValue(baseRegisterIndex, out string? className))
        {
            return false;
        }

        AnalysisItem? vmt = session.Items.Values.FirstOrDefault(item =>
            item.Flags.HasFlag(AnalysisFlags.Vmt)
            && string.Equals(item.Name, className, StringComparison.Ordinal));
        HashSet<uint> visitedVmts = [];
        while (vmt is not null && visitedVmts.Add(vmt.Address))
        {
            DelphiVmtVirtualMethod? method = vmt.VirtualMethods.FirstOrDefault(candidate =>
                candidate.SlotOffset == (int)callTarget.Displacement!.Value);
            if (method is not null
                && TryGetCodeSection(session, method.CodeAddress, out _, out _))
            {
                methodAddress = method.CodeAddress;
                return true;
            }

            vmt = vmt.ParentAddress is uint parentAddress
                && session.Items.TryGetValue(parentAddress, out AnalysisItem? parent)
                && parent.Flags.HasFlag(AnalysisFlags.Vmt)
                    ? parent
                    : null;
        }

        return false;
    }

    private static void ApplyKnownClassFieldAccess(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        AnalysisItem instructionItem)
    {
        if (instruction.FlowControl == InstructionFlowControl.Call)
        {
            return;
        }

        if (string.Equals(instruction.Mnemonic, "Add", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetClassRegisterTypeIndex(instruction.Operands[0].Register) is int destinationRegisterIndex
            && classRegisterTypes.TryGetValue(destinationRegisterIndex, out string? ownerTypeName)
            && instruction.Operands[1].Kind == DecodedOperandKind.Immediate
            && instruction.Operands[1].Immediate is ulong arithmeticOffset and > 0 and <= int.MaxValue
            && TryGetTypedField(
                session,
                ownerTypeName,
                (int)arithmeticOffset,
                out string? arithmeticFieldName,
                out uint? arithmeticFieldTypeInfoAddress))
        {
            string? arithmeticTypeName = arithmeticFieldTypeInfoAddress is uint typeInfoAddress
                && session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeInfo)
                    ? typeInfo.Name
                    : null;
            MemberAccessCandidate arithmeticAccess = new(
                ownerTypeName,
                (int)arithmeticOffset,
                arithmeticFieldName!,
                arithmeticFieldTypeInfoAddress,
                arithmeticTypeName);
            if (!instructionItem.MemberAccessCandidates.Contains(arithmeticAccess))
            {
                instructionItem.MemberAccessCandidates =
                [
                    .. instructionItem.MemberAccessCandidates,
                    arithmeticAccess
                ];
            }
        }

        foreach (DecodedOperand operand in instruction.Operands)
        {
            if (operand.Kind != DecodedOperandKind.Memory
                || operand.IndexRegister is not null
                || operand.Displacement is not ulong displacement
                || displacement is 0 or > int.MaxValue
                || GetClassRegisterTypeIndex(operand.BaseRegister) is not int registerIndex
                || !classRegisterTypes.TryGetValue(registerIndex, out string? className))
            {
                continue;
            }

            if (TryGetTypedField(
                    session,
                    className,
                    (int)displacement,
                    out string? fieldName,
                    out uint? fieldTypeInfoAddress))
            {
                string? typeName = fieldTypeInfoAddress is uint typeInfoAddress
                    && session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeInfo)
                        ? typeInfo.Name
                        : null;
                if (string.IsNullOrWhiteSpace(typeName)
                    && (string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase))
                    && instruction.Operands.Count == 2
                    && instruction.Operands[0].Kind == DecodedOperandKind.Memory
                    && instruction.Operands[0] == operand
                    && instruction.Operands[1].Kind == DecodedOperandKind.Register
                    && GetClassRegisterTypeIndex(instruction.Operands[1].Register) is int sourceRegisterIndex
                    && classRegisterTypes.TryGetValue(sourceRegisterIndex, out string? sourceTypeName))
                {
                    typeName = sourceTypeName;
                }

                MemberAccessCandidate access = new(
                    className,
                    (int)displacement,
                    fieldName!,
                    fieldTypeInfoAddress,
                    typeName);
                if (!instructionItem.MemberAccessCandidates.Contains(access))
                {
                    instructionItem.MemberAccessCandidates =
                    [
                        .. instructionItem.MemberAccessCandidates,
                        access
                    ];
                }
            }
        }
    }

    private static bool TryGetTypedField(
        AnalysisSession session,
        string typeName,
        int offset,
        out string? fieldName,
        out uint? typeInfoAddress)
    {
        return TryGetTypedField(
            session,
            typeName,
            offset,
            [],
            out fieldName,
            out typeInfoAddress);
    }

    private static bool TryGetTypedField(
        AnalysisSession session,
        string typeName,
        int offset,
        HashSet<uint> visitedRecordTypes,
        out string? fieldName,
        out uint? typeInfoAddress)
    {
        if (offset < 0)
        {
            fieldName = null;
            typeInfoAddress = null;
            return false;
        }

        AnalysisItem? vmt = session.Items.Values.FirstOrDefault(candidate =>
            candidate.Flags.HasFlag(AnalysisFlags.Vmt)
            && string.Equals(candidate.Name, typeName, StringComparison.Ordinal));

        if (vmt is not null && vmt.ClassInstanceSizeBytes is uint classSize)
        {
            if ((uint)offset >= classSize)
            {
                fieldName = null;
                typeInfoAddress = null;
                return false;
            }

            HashSet<uint> visitedSizedVmts = [vmt.Address];
            while (vmt.ParentAddress is uint parentAddress
                && session.Items.TryGetValue(parentAddress, out AnalysisItem? parent)
                && parent is not null
                && parent.Flags.HasFlag(AnalysisFlags.Vmt)
                && parent.ClassInstanceSizeBytes is uint parentSize
                && (uint)offset < parentSize
                && visitedSizedVmts.Add(parent.Address))
            {
                vmt = parent;
            }

            DelphiVmtField? exactField = vmt.Fields.FirstOrDefault(candidate =>
                candidate.Offset == offset);
            if (exactField is not null)
            {
                fieldName = exactField.Name;
                typeInfoAddress = exactField.TypeInfoAddress;
                return true;
            }

            if (TryResolveNestedRecordField(
                    session,
                    vmt.Fields,
                    offset,
                    vmt.ClassInstanceSizeBytes,
                    visitedRecordTypes,
                    out fieldName,
                    out typeInfoAddress))
            {
                return true;
            }

            if (TryGetVmtFieldAtOffset(vmt, offset, out DelphiVmtField? classField)
                && classField is not null)
            {
                fieldName = classField.Name;
                typeInfoAddress = classField.TypeInfoAddress;
                return true;
            }

            fieldName = null;
            typeInfoAddress = null;
            return false;
        }

        HashSet<uint> visitedVmts = [];
        while (vmt is not null && visitedVmts.Add(vmt.Address))
        {
            DelphiVmtField? exactField = vmt.Fields.FirstOrDefault(candidate =>
                candidate.Offset == offset);
            if (exactField is not null)
            {
                fieldName = exactField.Name;
                typeInfoAddress = exactField.TypeInfoAddress;
                return true;
            }

            if (TryResolveNestedRecordField(
                    session,
                    vmt.Fields,
                    offset,
                    vmt.ClassInstanceSizeBytes,
                    visitedRecordTypes,
                    out fieldName,
                    out typeInfoAddress))
            {
                return true;
            }

            if (TryGetFieldAtOffset(
                    vmt.Fields,
                    offset,
                    vmt.ClassInstanceSizeBytes,
                    static candidate => candidate.Offset,
                    out DelphiVmtField? classField)
                && classField is not null)
            {
                fieldName = classField.Name;
                typeInfoAddress = classField.TypeInfoAddress;
                return true;
            }

            vmt = vmt.ParentAddress is uint parentAddress
                && session.Items.TryGetValue(parentAddress, out AnalysisItem? parent)
                && parent.Flags.HasFlag(AnalysisFlags.Vmt)
                    ? parent
                    : null;
        }

        AnalysisItem? recordType = session.Items.Values.FirstOrDefault(candidate =>
            candidate.TypeKind == DelphiTypeKind.Record
            && string.Equals(candidate.Name, typeName, StringComparison.Ordinal));
        if (recordType is not null)
        {
            DelphiRttiRecordField? exactField = recordType.RecordFields.FirstOrDefault(candidate =>
                candidate.Offset == offset);
            if (exactField is not null)
            {
                fieldName = exactField.Name;
                typeInfoAddress = exactField.TypeInfoAddress;
                return true;
            }

            if (recordType.RecordSizeBytes is uint recordSize
                && (uint)offset < recordSize
                && TryResolveNestedRecordField(
                    session,
                    recordType.RecordFields,
                    offset,
                    recordSize,
                    visitedRecordTypes,
                    out string? nestedFieldName,
                    out uint? nestedTypeInfoAddress))
            {
                fieldName = nestedFieldName;
                typeInfoAddress = nestedTypeInfoAddress;
                return true;
            }

            if (TryGetFieldAtOffset(
                    recordType.RecordFields,
                    offset,
                    recordType.RecordSizeBytes,
                    static candidate => candidate.Offset,
                    out DelphiRttiRecordField? recordField)
                && recordField is not null)
            {
                fieldName = recordField.Name;
                typeInfoAddress = recordField.TypeInfoAddress;
                return true;
            }
        }

        fieldName = null;
        typeInfoAddress = null;
        return false;
    }

    private static bool TryGetVmtFieldAtOffset(
        AnalysisItem vmt,
        int offset,
        out DelphiVmtField? field)
    {
        return TryGetFieldAtOffset(
            vmt.Fields,
            offset,
            vmt.ClassInstanceSizeBytes,
            static candidate => candidate.Offset,
            out field);
    }

    private static bool TryGetFieldAtOffset<TField>(
        IEnumerable<TField> fields,
        int offset,
        uint? containingSize,
        Func<TField, int> getOffset,
        out TField? selectedField)
        where TField : class
    {
        selectedField = null;
        if (offset < 0 || (containingSize is uint size && (uint)offset >= size))
        {
            return false;
        }

        int selectedOffset = int.MinValue;
        bool hasNextField = false;
        foreach (TField field in fields)
        {
            int fieldOffset = getOffset(field);
            if (fieldOffset <= offset && fieldOffset > selectedOffset)
            {
                selectedField = field;
                selectedOffset = fieldOffset;
            }
        }

        if (selectedField is null)
        {
            return false;
        }

        foreach (TField field in fields)
        {
            if (getOffset(field) > selectedOffset)
            {
                hasNextField = true;
                break;
            }
        }

        return offset == selectedOffset || containingSize is not null || hasNextField;
    }

    private static bool TryResolveNestedRecordField<TField>(
        AnalysisSession session,
        IEnumerable<TField> fields,
        int absoluteOffset,
        uint? containingSize,
        HashSet<uint> visitedRecordTypes,
        out string? fieldName,
        out uint? typeInfoAddress)
        where TField : class
    {
        TField? containingField = null;
        int containingFieldOffset = int.MinValue;
        int nextFieldOffset = int.MaxValue;
        foreach (TField field in fields)
        {
            int fieldOffset;
            switch (field)
            {
                case DelphiVmtField classField:
                    fieldOffset = classField.Offset;
                    break;
                case DelphiRttiRecordField recordField:
                    fieldOffset = recordField.Offset;
                    break;
                default:
                    continue;
            }

            if (fieldOffset <= absoluteOffset && fieldOffset > containingFieldOffset)
            {
                containingField = field;
                containingFieldOffset = fieldOffset;
            }
            else if (fieldOffset > absoluteOffset && fieldOffset < nextFieldOffset)
            {
                nextFieldOffset = fieldOffset;
            }
        }

        int nestedOffset = absoluteOffset - containingFieldOffset;
        if (containingField is null
            || nestedOffset <= 0
            || (containingSize is uint size && (uint)absoluteOffset >= size)
            || absoluteOffset >= nextFieldOffset
            || !TryGetFieldTypeInfoAddress(containingField, out string? name, out uint? fieldTypeInfoAddress)
            || fieldTypeInfoAddress is not uint typeInfoAddressValue
            || visitedRecordTypes.Contains(typeInfoAddressValue)
            || !session.Items.TryGetValue(typeInfoAddressValue, out AnalysisItem? nestedType)
            || nestedType.TypeKind != DelphiTypeKind.Record
            || nestedType.RecordSizeBytes is not uint nestedRecordSize
            || (uint)nestedOffset >= nestedRecordSize)
        {
            fieldName = null;
            typeInfoAddress = null;
            return false;
        }

        HashSet<uint> nestedRecordPath = [.. visitedRecordTypes, typeInfoAddressValue];
        if (!TryGetTypedField(
                session,
                nestedType.Name ?? string.Empty,
                nestedOffset,
                nestedRecordPath,
                out string? nestedName,
                out uint? nestedTypeInfoAddress))
        {
            fieldName = null;
            typeInfoAddress = null;
            return false;
        }

        fieldName = $"{name}.{nestedName}";
        typeInfoAddress = nestedTypeInfoAddress;
        return true;
    }

    private static bool TryGetFieldTypeInfoAddress<TField>(
        TField field,
        out string? name,
        out uint? typeInfoAddress)
    {
        switch (field)
        {
            case DelphiVmtField classField:
                name = classField.Name;
                typeInfoAddress = classField.TypeInfoAddress;
                return true;
            case DelphiRttiRecordField recordField:
                name = recordField.Name;
                typeInfoAddress = recordField.TypeInfoAddress;
                return true;
            default:
                name = null;
                typeInfoAddress = null;
                return false;
        }
    }

    private static bool TryGetRecentImmediateRegisterValue(
        IReadOnlyList<DecodedInstruction> precedingInstructions,
        string register,
        AnalysisSession session,
        out ushort value)
    {
        for (int index = precedingInstructions.Count - 1; index >= 0; index--)
        {
            DecodedInstruction instruction = precedingInstructions[index];
            if (instruction.FlowControl != InstructionFlowControl.Next)
            {
                if (instruction.FlowControl == InstructionFlowControl.Call
                    && IsRegisterPreservingCall(instruction, session))
                {
                    continue;
                }

                break;
            }

            if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase)
                && instruction.Operands.Any(operand =>
                    operand.Kind == DecodedOperandKind.Register
                    && IsRegisterAlias(operand.Register, register)))
            {
                break;
            }

            if (instruction.Operands.Count == 0
                || instruction.Operands[0].Kind != DecodedOperandKind.Register
                || !IsRegisterAlias(instruction.Operands[0].Register, register))
            {
                continue;
            }

            if (!string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
                || !IsFullRegisterAlias(instruction.Operands[0].Register, register)
                || instruction.Operands.Count != 2
                || instruction.Operands[1].Kind != DecodedOperandKind.Immediate
                || instruction.Operands[1].Immediate is not ulong immediate)
            {
                break;
            }

            value = unchecked((ushort)immediate);
            return true;
        }

        value = 0;
        return false;
    }

    private static bool IsRegisterAlias(string? candidate, string register)
    {
        if (candidate is null)
        {
            return false;
        }

        string[] aliases = register switch
        {
            "BX" => ["BX", "EBX", "BH", "BL"],
            "SI" => ["SI", "ESI", "SIL"],
            "DX" => ["DX", "EDX", "DH", "DL"],
            _ => []
        };
        return aliases.Any(alias => string.Equals(candidate, alias, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsFullRegisterAlias(string? candidate, string register)
    {
        string[] aliases = register switch
        {
            "BX" => ["BX", "EBX"],
            "SI" => ["SI", "ESI"],
            "DX" => ["DX", "EDX"],
            _ => []
        };
        return aliases.Any(alias => string.Equals(candidate, alias, StringComparison.OrdinalIgnoreCase));
    }

    private static string? GetKnownCallReturnTypeCandidate(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, string> classRegisterTypes)
    {
        if (instruction.FlowControl != InstructionFlowControl.Call
            || instruction.NearBranchTarget is not ulong targetAddress
            || targetAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target))
        {
            return null;
        }

        if (string.Equals(target.Name, "@IsClass", StringComparison.OrdinalIgnoreCase))
        {
            return "Boolean";
        }

        if (string.Equals(target.Name, "@GetTls", StringComparison.OrdinalIgnoreCase))
        {
            return "#TLS";
        }

        if (string.Equals(target.Name, "@BeforeDestruction", StringComparison.OrdinalIgnoreCase)
            && classRegisterTypes.TryGetValue(0, out string? destroyedClassName))
        {
            return destroyedClassName;
        }

        if (string.Equals(target.Name, "@ClassCreate", StringComparison.OrdinalIgnoreCase)
            && classRegisterTypes.TryGetValue(0, out string? createdClassName))
        {
            return createdClassName;
        }

        if (string.Equals(target.Name, "@AsClass", StringComparison.OrdinalIgnoreCase)
            && classRegisterTypes.TryGetValue(1, out string? className))
        {
            return className;
        }

        return null;
    }

    private static string? GetTlsThreadVariableCandidate(DecodedInstruction instruction)
    {
        DecodedOperand? tlsMemoryOperand = instruction.Operands.FirstOrDefault(operand =>
            operand.Kind == DecodedOperandKind.Memory
            && string.Equals(operand.BaseRegister, "EAX", StringComparison.OrdinalIgnoreCase));
        if (tlsMemoryOperand is null)
        {
            return null;
        }

        int displacement = unchecked((int)(tlsMemoryOperand.Displacement ?? 0));
        return $"threadvar_{displacement}";
    }

    private static void ApplyKnownCallLocalDataTypes(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, int> registerLocalOffsets,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        Dictionary<int, StackLocalVariable> stackLocalVariables)
    {
        if (instruction.NearBranchTarget is not ulong targetAddress
            || targetAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target))
        {
            return;
        }

        string helperName = target.Name is string resolvedName && resolvedName.StartsWith('@')
            ? resolvedName[1..]
            : target.Name ?? string.Empty;
        if (string.Equals(helperName, "TApplication.CreateForm", StringComparison.OrdinalIgnoreCase)
            && registerLocalOffsets.TryGetValue(2, out int formLocalOffset)
            && classRegisterTypes.TryGetValue(1, out string? formClassName))
        {
            AddStackLocalVariable(stackLocalVariables, formLocalOffset, formClassName, overwrite: false);
        }
        else if ((string.Equals(helperName, "IntfClear", StringComparison.OrdinalIgnoreCase)
                || string.Equals(helperName, "VarClr", StringComparison.OrdinalIgnoreCase))
            && registerLocalOffsets.TryGetValue(0, out int clearedLocalOffset))
        {
            string clearedTypeName = string.Equals(helperName, "IntfClear", StringComparison.OrdinalIgnoreCase)
                ? "IInterface"
                : "Variant";
            AddStackLocalVariable(stackLocalVariables, clearedLocalOffset, clearedTypeName, overwrite: false);
        }
        else if (IsLocalStringArrayCleanupHelper(helperName)
            && registerLocalOffsets.TryGetValue(0, out int firstLocalOffset)
            && registerAddressValues.TryGetValue(1, out ulong countValue)
            && countValue is > 0 and <= 256)
        {
            string typeName = string.Equals(helperName, "LStrArrayClr", StringComparison.OrdinalIgnoreCase)
                ? "AnsiString"
                : string.Equals(helperName, "WStrArrayClr", StringComparison.OrdinalIgnoreCase)
                    ? "WideString"
                    : "UString";
            for (int index = 0; index < (int)countValue; index++)
            {
                int localOffset;
                try
                {
                    localOffset = checked(firstLocalOffset + (index * sizeof(uint)));
                }
                catch (OverflowException)
                {
                    break;
                }

                AddStackLocalVariable(stackLocalVariables, localOffset, typeName, overwrite: false);
            }

            return;
        }

        if (string.Equals(helperName, "FinalizeRecord", StringComparison.OrdinalIgnoreCase)
            && registerLocalOffsets.TryGetValue(0, out int recordLocalOffset)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? recordTypeName))
        {
            AddStackLocalVariable(stackLocalVariables, recordLocalOffset, recordTypeName, overwrite: false);
        }
        else if (string.Equals(helperName, "FinalizeArray", StringComparison.OrdinalIgnoreCase)
            && registerLocalOffsets.TryGetValue(0, out int finalizedArrayLocalOffset)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? arrayElementTypeName)
            && registerAddressValues.TryGetValue(2, out ulong arrayElementCount)
            && arrayElementCount <= int.MaxValue)
        {
            AddStackLocalVariable(
                stackLocalVariables,
                finalizedArrayLocalOffset,
                $"array[{arrayElementCount}] of {arrayElementTypeName}",
                overwrite: false);
        }
        else if (string.Equals(helperName, "DynArrayAddRef", StringComparison.OrdinalIgnoreCase)
            && registerLocalOffsets.TryGetValue(0, out int arrayLocalOffset))
        {
            AddStackLocalVariable(stackLocalVariables, arrayLocalOffset, "array of ?", overwrite: false);
        }
        else if (IsDynamicArrayTypeHelper(helperName)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? elementTypeName))
        {
            if (registerLocalOffsets.TryGetValue(0, out int sourceLocalOffset))
            {
                AddStackLocalVariable(stackLocalVariables, sourceLocalOffset, elementTypeName, overwrite: false);
            }

            if (string.Equals(helperName, "DynArrayCopy", StringComparison.OrdinalIgnoreCase)
                && registerLocalOffsets.TryGetValue(2, out int destinationOffset))
            {
                AddStackLocalVariable(stackLocalVariables, destinationOffset, elementTypeName, overwrite: false);
            }
        }
    }

    private static void ApplyKnownCallStackArgumentTypes(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, uint> registerStackArgumentOffsets,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        Dictionary<uint, StackArgument> stackArguments)
    {
        if (instruction.NearBranchTarget is not ulong targetAddress
            || targetAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target))
        {
            return;
        }

        string helperName = target.Name is string resolvedName && resolvedName.StartsWith('@')
            ? resolvedName[1..]
            : target.Name ?? string.Empty;
        if (string.Equals(helperName, "TApplication.CreateForm", StringComparison.OrdinalIgnoreCase)
            && registerStackArgumentOffsets.TryGetValue(2, out uint formArgumentOffset)
            && classRegisterTypes.TryGetValue(1, out string? formClassName))
        {
            SetStackArgumentType(stackArguments, formArgumentOffset, formClassName);
        }
        else if ((string.Equals(helperName, "IntfClear", StringComparison.OrdinalIgnoreCase)
                || string.Equals(helperName, "VarClr", StringComparison.OrdinalIgnoreCase))
            && registerStackArgumentOffsets.TryGetValue(0, out uint clearedArgumentOffset))
        {
            string clearedTypeName = string.Equals(helperName, "IntfClear", StringComparison.OrdinalIgnoreCase)
                ? "IInterface"
                : "Variant";
            SetStackArgumentType(stackArguments, clearedArgumentOffset, clearedTypeName);
        }
        else if (string.Equals(helperName, "FinalizeRecord", StringComparison.OrdinalIgnoreCase)
            && registerStackArgumentOffsets.TryGetValue(0, out uint recordArgumentOffset)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? recordTypeName))
        {
            SetStackArgumentType(stackArguments, recordArgumentOffset, recordTypeName);
        }
        else if (string.Equals(helperName, "FinalizeArray", StringComparison.OrdinalIgnoreCase)
            && registerStackArgumentOffsets.TryGetValue(0, out uint finalizedArrayArgumentOffset)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? arrayElementTypeName)
            && registerAddressValues.TryGetValue(2, out ulong arrayElementCount)
            && arrayElementCount <= int.MaxValue)
        {
            SetStackArgumentType(
                stackArguments,
                finalizedArrayArgumentOffset,
                $"array[{arrayElementCount}] of {arrayElementTypeName}");
        }
        else if (string.Equals(helperName, "DynArrayAddRef", StringComparison.OrdinalIgnoreCase)
            && registerStackArgumentOffsets.TryGetValue(0, out uint arrayArgumentOffset))
        {
            SetStackArgumentType(stackArguments, arrayArgumentOffset, "array of ?");
        }
        else if (IsDynamicArrayTypeHelper(helperName)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? elementTypeName))
        {
            if (registerStackArgumentOffsets.TryGetValue(0, out uint sourceArgumentOffset))
            {
                SetStackArgumentType(stackArguments, sourceArgumentOffset, elementTypeName);
            }

            if (string.Equals(helperName, "DynArrayCopy", StringComparison.OrdinalIgnoreCase)
                && registerStackArgumentOffsets.TryGetValue(2, out uint destinationArgumentOffset))
            {
                SetStackArgumentType(stackArguments, destinationArgumentOffset, elementTypeName);
            }
        }
    }

    private static void SetStackArgumentType(
        Dictionary<uint, StackArgument> stackArguments,
        uint offset,
        string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)
            || !stackArguments.TryGetValue(offset, out StackArgument? argument)
            || argument is null
            || !string.IsNullOrWhiteSpace(argument.TypeName))
        {
            return;
        }

        stackArguments[offset] = argument with { TypeName = typeName };
    }

    private static bool IsLocalStringArrayCleanupHelper(string helperName) =>
        string.Equals(helperName, "LStrArrayClr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "WStrArrayClr", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "UStrArrayClr", StringComparison.OrdinalIgnoreCase);

    private static void AddStackLocalVariable(
        Dictionary<int, StackLocalVariable> stackLocalVariables,
        int localOffset,
        string? typeName,
        bool overwrite,
        int sizeBytes = sizeof(uint))
    {
        if (string.IsNullOrWhiteSpace(typeName) || sizeBytes <= 0)
        {
            return;
        }

        if (!stackLocalVariables.TryGetValue(localOffset, out StackLocalVariable? existing))
        {
            stackLocalVariables.Add(localOffset, new StackLocalVariable(localOffset, sizeBytes, typeName));
        }
        else if (overwrite)
        {
            stackLocalVariables[localOffset] = existing with { TypeName = typeName };
        }
    }

    private static void ApplyFloatingPointMemoryTypeCandidate(
        DecodedInstruction instruction,
        AnalysisSession session,
        AnalysisItem procedure,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        Dictionary<int, StackLocalVariable> stackLocalVariables)
    {
        if (!instruction.Mnemonic.StartsWith("F", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        foreach (DecodedOperand operand in instruction.Operands)
        {
            if (operand.Kind != DecodedOperandKind.Memory)
            {
                continue;
            }

            int? sizeBytes = operand.MemorySizeBytes;
            string typeName = sizeBytes switch
            {
                4 => "Single",
                8 => "Double",
                10 => "Extended",
                _ => "Float"
            };

            if (string.IsNullOrEmpty(operand.BaseRegister)
                && string.IsNullOrEmpty(operand.IndexRegister)
                && operand.Displacement is ulong absoluteAddress
                && absoluteAddress <= uint.MaxValue)
            {
                ApplyDataTypeCandidate(
                    session,
                    absoluteAddress,
                    typeName,
                    overwrite: false,
                    AnalysisFlags.None);
                continue;
            }

            if (string.IsNullOrEmpty(operand.IndexRegister)
                && operand.Displacement is 0
                && GetRegisterAddressIndex(operand.BaseRegister) is int baseRegisterIndex
                && registerAddressValues.TryGetValue(baseRegisterIndex, out ulong registerAddress)
                && registerAddress <= uint.MaxValue)
            {
                ApplyDataTypeCandidate(
                    session,
                    registerAddress,
                    typeName,
                    overwrite: false,
                    AnalysisFlags.None);
                continue;
            }

            if (!procedure.UsesFramePointer
                || !string.Equals(operand.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                || !string.IsNullOrEmpty(operand.IndexRegister)
                || sizeBytes is not > 0
                || operand.Displacement is not ulong stackDisplacement)
            {
                continue;
            }

            int localOffset = unchecked((int)(uint)stackDisplacement);
            if (localOffset < 0)
            {
                AddStackLocalVariable(
                    stackLocalVariables,
                    localOffset,
                    typeName,
                    overwrite: false,
                    sizeBytes.Value);
            }
        }
    }

    private static void ApplyKnownCallDataTypeCandidate(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        IReadOnlyDictionary<int, string> classRegisterTypes)
    {
        if (instruction.NearBranchTarget is not ulong targetAddress
            || targetAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target))
        {
            return;
        }

        string helperName = target.Name is string resolvedName && resolvedName.StartsWith('@')
            ? resolvedName[1..]
            : target.Name ?? string.Empty;
        if (string.Equals(helperName, "TApplication.CreateForm", StringComparison.OrdinalIgnoreCase))
        {
            if (!classRegisterTypes.TryGetValue(1, out string? formClassName)
                && (!registerAddressValues.TryGetValue(1, out ulong selfPointerValue)
                    || !TryGetVmtClassNameFromSelfPointer(
                        session,
                        selfPointerValue,
                        out formClassName)))
            {
                return;
            }

            ApplyRegisterDataTypeCandidate(
                session,
                registerAddressValues,
                2,
                formClassName);
        }
        else if (string.Equals(helperName, "IntfClear", StringComparison.OrdinalIgnoreCase))
        {
            ApplyRegisterDataTypeCandidate(session, registerAddressValues, 0, "IInterface");
        }
        else if (string.Equals(helperName, "VarClr", StringComparison.OrdinalIgnoreCase))
        {
            ApplyRegisterDataTypeCandidate(session, registerAddressValues, 0, "Variant");
        }
        else if (string.Equals(helperName, "DynArrayAddRef", StringComparison.OrdinalIgnoreCase))
        {
            ApplyRegisterDataTypeCandidate(
                session,
                registerAddressValues,
                0,
                null,
                overwrite: false,
                additionalFlags: AnalysisFlags.DynamicArrayCandidate);
        }
        else if (string.Equals(helperName, "FinalizeRecord", StringComparison.OrdinalIgnoreCase)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? recordTypeName))
        {
            ApplyRegisterDataTypeCandidate(session, registerAddressValues, 0, recordTypeName);
        }
        else if (string.Equals(helperName, "FinalizeArray", StringComparison.OrdinalIgnoreCase)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? elementTypeName)
            && registerAddressValues.TryGetValue(2, out ulong elementCount)
            && elementCount <= int.MaxValue)
        {
            ApplyRegisterDataTypeCandidate(
                session,
                registerAddressValues,
                0,
                $"array[{elementCount}] of {elementTypeName}");
        }
        else if (IsDynamicArrayTypeHelper(helperName)
            && TryGetKnownTypeInfoName(session, registerAddressValues, out string? dynamicArrayTypeName))
        {
            bool overwrite = string.Equals(helperName, "DynArrayCopy", StringComparison.OrdinalIgnoreCase);
            ApplyRegisterDataTypeCandidate(
                session,
                registerAddressValues,
                0,
                dynamicArrayTypeName,
                overwrite,
                AnalysisFlags.DynamicArrayCandidate);
            if (string.Equals(helperName, "DynArrayCopy", StringComparison.OrdinalIgnoreCase))
            {
                ApplyRegisterDataTypeCandidate(
                    session,
                    registerAddressValues,
                    2,
                    dynamicArrayTypeName,
                    overwrite: true,
                    additionalFlags: AnalysisFlags.DynamicArrayCandidate);
            }
        }
    }

    private static void ApplyKnownCallResourceString(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        AnalysisItem instructionItem)
    {
        if (instruction.NearBranchTarget is not ulong targetAddress
            || targetAddress > uint.MaxValue
            || !session.Items.TryGetValue((uint)targetAddress, out AnalysisItem? target))
        {
            return;
        }

        string helperName = target.Name is string resolvedName && resolvedName.StartsWith('@')
            ? resolvedName[1..]
            : target.Name ?? string.Empty;
        if (!string.Equals(helperName, "LoadStr", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(helperName, "FmtLoadStr", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(helperName, "LoadResString", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (registerAddressValues.TryGetValue(0, out ulong resourceId)
            && resourceId <= ushort.MaxValue
            && session.TryGetUnambiguousResourceString((uint)resourceId, out string? value))
        {
            instructionItem.ResourceStringCandidate = value;
        }
    }

    private static bool IsDynamicArrayTypeHelper(string helperName) =>
        string.Equals(helperName, "DynArrayClear", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "DynArraySetLength", StringComparison.OrdinalIgnoreCase)
            || string.Equals(helperName, "DynArrayCopy", StringComparison.OrdinalIgnoreCase);

    private static void ApplyRegisterDataTypeCandidate(
        AnalysisSession session,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        int registerIndex,
        string? typeName,
        bool overwrite = true,
        AnalysisFlags additionalFlags = AnalysisFlags.None)
    {
        if (registerAddressValues.TryGetValue(registerIndex, out ulong value))
        {
            ApplyDataTypeCandidate(session, value, typeName, overwrite, additionalFlags);
        }
    }

    private static void ApplyDataTypeCandidate(
        AnalysisSession session,
        ulong value,
        string? typeName,
        bool overwrite,
        AnalysisFlags additionalFlags)
    {
        if ((typeName is null && additionalFlags == AnalysisFlags.None)
            || value > uint.MaxValue
            || !TryVaToRva(session, (uint)value, out uint dataRva)
            || !IsMappedDataRva(session, dataRva))
        {
            return;
        }

        AnalysisItem dataItem = session.GetOrAddItem(dataRva);
        if (dataItem.Flags.HasFlag(AnalysisFlags.Code)
            || dataItem.Flags.HasFlag(AnalysisFlags.Instruction)
            || dataItem.Flags.HasFlag(AnalysisFlags.Rtti)
            || dataItem.Flags.HasFlag(AnalysisFlags.Vmt))
        {
            return;
        }

        if (typeName is not null && (overwrite || string.IsNullOrEmpty(dataItem.DataTypeCandidate)))
        {
            dataItem.DataTypeCandidate = typeName;
        }

        dataItem.SetFlags(AnalysisFlags.Data | additionalFlags);
    }

    private static bool IsMappedDataRva(AnalysisSession session, uint rva) =>
        session.Sections.Any(section =>
            !section.ContainsCode
            && rva >= section.VirtualAddress
            && (ulong)rva < (ulong)section.VirtualAddress + Math.Max(section.VirtualSize, section.RawSize));

    private static void ApplyKnownClassStoreTypeCandidate(
        DecodedInstruction instruction,
        AnalysisSession session,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        AnalysisItem procedure,
        Dictionary<int, StackLocalVariable> stackLocalVariables)
    {
        if (!string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands.Count != 2
            || instruction.Operands[0] is not
            {
                Kind: DecodedOperandKind.Memory,
                MemorySizeBytes: sizeof(uint)
            }
            || instruction.Operands[1].Kind != DecodedOperandKind.Register
            || GetClassRegisterTypeIndex(instruction.Operands[1].Register) is not int sourceRegisterIndex
            || !classRegisterTypes.TryGetValue(sourceRegisterIndex, out string? className))
        {
            return;
        }

        DecodedOperand destination = instruction.Operands[0];
        if (destination.BaseRegister is null
            && destination.IndexRegister is null
            && IsDefaultDataSegment(destination.SegmentRegister)
            && destination.Displacement is ulong address)
        {
            ApplyKnownClassGlobalStore(
                instruction,
                session,
                address,
                className);
        }
        else if (destination.IndexRegister is null
            && IsDefaultDataSegment(destination.SegmentRegister)
            && GetRegisterAddressIndex(destination.BaseRegister) is int baseRegisterIndex
            && registerAddressValues.TryGetValue(baseRegisterIndex, out ulong baseAddress)
            && destination.Displacement is ulong displacement
            && displacement <= int.MaxValue
            && baseAddress <= uint.MaxValue - displacement)
        {
            ApplyKnownClassGlobalStore(
                instruction,
                session,
                baseAddress + displacement,
                className);
        }
        else if (procedure.UsesFramePointer
            && string.Equals(destination.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
            && destination.IndexRegister is null
            && IsDefaultStackSegment(destination.SegmentRegister)
            && destination.Displacement is ulong stackDisplacement)
        {
            int localOffset = unchecked((int)(uint)stackDisplacement);
            if (localOffset < 0)
            {
                AddStackLocalVariable(
                    stackLocalVariables,
                    localOffset,
                    className,
                    overwrite: false,
                    sizeof(uint));
            }
        }
    }

    private static bool IsDefaultStackSegment(string? segmentRegister) =>
        string.IsNullOrEmpty(segmentRegister)
        || string.Equals(segmentRegister, "SS", StringComparison.OrdinalIgnoreCase);

    private static void ApplyKnownClassGlobalStore(
        DecodedInstruction instruction,
        AnalysisSession session,
        ulong address,
        string className)
    {
        ApplyDataTypeCandidate(
            session,
            address,
            className,
            overwrite: false,
            AnalysisFlags.None);
        if (address <= uint.MaxValue
            && TryVaToRva(session, (uint)address, out uint dataRva)
            && IsMappedDataRva(session, dataRva))
        {
            AddConstantCrossReferenceIfMissing(
                session,
                checked((uint)instruction.Address),
                dataRva);
        }
    }

    private static void ApplyKnownClassStackArgumentStoreTypeCandidate(
        DecodedInstruction instruction,
        IReadOnlyDictionary<int, string> classRegisterTypes,
        Dictionary<uint, StackArgument> stackArguments)
    {
        if (!string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands.Count != 2
            || instruction.Operands[0] is not
            {
                Kind: DecodedOperandKind.Memory,
                BaseRegister: string baseRegister,
                IndexRegister: null,
                Displacement: >= 8 and <= 0x10000,
                MemorySizeBytes: sizeof(uint)
            } destination
            || !string.Equals(baseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
            || instruction.Operands[1].Kind != DecodedOperandKind.Register
            || GetClassRegisterTypeIndex(instruction.Operands[1].Register) is not int sourceIndex
            || !classRegisterTypes.TryGetValue(sourceIndex, out string? typeName))
        {
            return;
        }

        uint offset = checked((uint)destination.Displacement!.Value);
        if (stackArguments.TryGetValue(offset, out StackArgument? argument)
            && argument is not null
            && string.IsNullOrWhiteSpace(argument.TypeName))
        {
            stackArguments[offset] = argument with { TypeName = typeName };
        }
    }

    private static bool TryGetKnownTypeInfoName(
        AnalysisSession session,
        IReadOnlyDictionary<int, ulong> registerAddressValues,
        out string? typeName)
    {
        if (registerAddressValues.TryGetValue(1, out ulong typeInfoValue)
            && typeInfoValue <= uint.MaxValue
            && TryVaToRva(session, (uint)typeInfoValue, out uint typeInfoRva)
            && session.Items.TryGetValue(typeInfoRva, out AnalysisItem? typeInfoItem)
            && typeInfoItem.Flags.HasFlag(AnalysisFlags.Rtti)
            && typeInfoItem.TypeKind is not null
            && !string.IsNullOrWhiteSpace(typeInfoItem.Name))
        {
            typeName = typeInfoItem.Name;
            return true;
        }

        typeName = null;
        return false;
    }

    private static void UpdateRegisterAddressValues(
        DecodedInstruction instruction,
        AnalysisSession session,
        Dictionary<int, ulong> registerAddressValues)
    {
        if (instruction.FlowControl != InstructionFlowControl.Next)
        {
            if (instruction.FlowControl == InstructionFlowControl.Call
                && IsRegisterPreservingCall(instruction, session))
            {
                return;
            }

            registerAddressValues.Clear();
            return;
        }

        bool isMov = string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase);
        bool isLea = string.Equals(instruction.Mnemonic, "Lea", StringComparison.OrdinalIgnoreCase);
        if ((isMov || isLea)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetRegisterAddressIndex(instruction.Operands[0].Register) is int destinationIndex)
        {
            ulong? value = null;
            DecodedOperand source = instruction.Operands[1];
            if (isMov
                && source.Kind == DecodedOperandKind.Immediate
                && source.Immediate is ulong immediate)
            {
                value = immediate;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Register
                && GetRegisterAddressIndex(source.Register) is int sourceIndex
                && registerAddressValues.TryGetValue(sourceIndex, out ulong registerValue))
            {
                value = registerValue;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && source.BaseRegister is null
                && source.IndexRegister is null
                && source.Displacement is ulong absoluteAddress)
            {
                value = absoluteAddress;
            }

            registerAddressValues.Remove(destinationIndex);
            if (value is ulong knownValue)
            {
                registerAddressValues[destinationIndex] = knownValue;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase))
        {
            if (instruction.Operands.Count == 2
                && instruction.Operands[0].Kind == DecodedOperandKind.Register
                && instruction.Operands[1].Kind == DecodedOperandKind.Register
                && GetRegisterAddressIndex(instruction.Operands[0].Register) is int firstIndex
                && GetRegisterAddressIndex(instruction.Operands[1].Register) is int secondIndex)
            {
                if (firstIndex != secondIndex)
                {
                    bool hasFirstValue = registerAddressValues.TryGetValue(firstIndex, out ulong firstValue);
                    bool hasSecondValue = registerAddressValues.TryGetValue(secondIndex, out ulong secondValue);
                    registerAddressValues.Remove(firstIndex);
                    registerAddressValues.Remove(secondIndex);
                    if (hasFirstValue)
                    {
                        registerAddressValues[secondIndex] = firstValue;
                    }

                    if (hasSecondValue)
                    {
                        registerAddressValues[firstIndex] = secondValue;
                    }
                }

                return;
            }

            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register
                    && GetClassRegisterFamilyIndex(operand.Register) is int exchangedIndex)
                {
                    registerAddressValues.Remove(exchangedIndex);
                }
            }

            return;
        }

        for (int argumentIndex = 0; argumentIndex < 6; argumentIndex++)
        {
            if (WritesTrackedRegister(instruction, argumentIndex))
            {
                registerAddressValues.Remove(argumentIndex);
            }
        }
    }

    private static void UpdateRegisterLocalOffsets(
        DecodedInstruction instruction,
        AnalysisSession session,
        Dictionary<int, int> registerLocalOffsets)
    {
        if (instruction.FlowControl != InstructionFlowControl.Next)
        {
            if (instruction.FlowControl == InstructionFlowControl.Call
                && IsRegisterPreservingCall(instruction, session))
            {
                return;
            }

            registerLocalOffsets.Clear();
            return;
        }

        bool isMov = string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase);
        bool isLea = string.Equals(instruction.Mnemonic, "Lea", StringComparison.OrdinalIgnoreCase);
        if ((isMov || isLea)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetFullRegisterArgumentIndex(instruction.Operands[0].Register) is int destinationIndex)
        {
            int? localOffset = null;
            DecodedOperand source = instruction.Operands[1];
            if (isMov
                && source.Kind == DecodedOperandKind.Register
                && GetFullRegisterArgumentIndex(source.Register) is int sourceIndex
                && registerLocalOffsets.TryGetValue(sourceIndex, out int sourceOffset))
            {
                localOffset = sourceOffset;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && string.Equals(source.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && source.IndexRegister is null
                && source.Displacement is ulong displacement
                && displacement <= uint.MaxValue)
            {
                int signedOffset = unchecked((int)(uint)displacement);
                if (signedOffset < 0)
                {
                    localOffset = signedOffset;
                }
            }

            registerLocalOffsets.Remove(destinationIndex);
            if (localOffset is int knownOffset)
            {
                registerLocalOffsets[destinationIndex] = knownOffset;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase))
        {
            if (instruction.Operands.Count == 2
                && instruction.Operands[0].Kind == DecodedOperandKind.Register
                && instruction.Operands[1].Kind == DecodedOperandKind.Register
                && GetFullRegisterArgumentIndex(instruction.Operands[0].Register) is int firstIndex
                && GetFullRegisterArgumentIndex(instruction.Operands[1].Register) is int secondIndex)
            {
                if (firstIndex != secondIndex)
                {
                    bool hasFirstOffset = registerLocalOffsets.TryGetValue(firstIndex, out int firstOffset);
                    bool hasSecondOffset = registerLocalOffsets.TryGetValue(secondIndex, out int secondOffset);
                    registerLocalOffsets.Remove(firstIndex);
                    registerLocalOffsets.Remove(secondIndex);
                    if (hasFirstOffset)
                    {
                        registerLocalOffsets[secondIndex] = firstOffset;
                    }

                    if (hasSecondOffset)
                    {
                        registerLocalOffsets[firstIndex] = secondOffset;
                    }
                }

                return;
            }

            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register
                    && GetRegisterArgumentIndex(operand.Register) is int exchangedIndex)
                {
                    registerLocalOffsets.Remove(exchangedIndex);
                }
            }

            return;
        }

        for (int argumentIndex = 0; argumentIndex < 3; argumentIndex++)
        {
            if (WritesRegisterArgument(instruction, argumentIndex))
            {
                registerLocalOffsets.Remove(argumentIndex);
            }
        }
    }

    private static void UpdateRegisterStackArgumentOffsets(
        DecodedInstruction instruction,
        AnalysisSession session,
        Dictionary<int, uint> registerStackArgumentOffsets)
    {
        if (instruction.FlowControl != InstructionFlowControl.Next)
        {
            if (instruction.FlowControl == InstructionFlowControl.Call
                && IsRegisterPreservingCall(instruction, session))
            {
                return;
            }

            registerStackArgumentOffsets.Clear();
            return;
        }

        bool isMov = string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase);
        bool isLea = string.Equals(instruction.Mnemonic, "Lea", StringComparison.OrdinalIgnoreCase);
        if ((isMov || isLea)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetFullRegisterArgumentIndex(instruction.Operands[0].Register) is int destinationIndex)
        {
            uint? argumentOffset = null;
            DecodedOperand source = instruction.Operands[1];
            if (isMov
                && source.Kind == DecodedOperandKind.Register
                && GetFullRegisterArgumentIndex(source.Register) is int sourceIndex
                && registerStackArgumentOffsets.TryGetValue(sourceIndex, out uint sourceOffset))
            {
                argumentOffset = sourceOffset;
            }
            else if (source.Kind == DecodedOperandKind.Memory
                && string.Equals(source.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && source.IndexRegister is null
                && source.Displacement is >= 8 and <= 0x10000
                && (isMov || isLea))
            {
                argumentOffset = (uint)source.Displacement.Value;
            }

            registerStackArgumentOffsets.Remove(destinationIndex);
            if (argumentOffset is uint knownOffset)
            {
                registerStackArgumentOffsets[destinationIndex] = knownOffset;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && instruction.Operands[1].Kind == DecodedOperandKind.Register
            && GetFullRegisterArgumentIndex(instruction.Operands[0].Register) is int firstIndex
            && GetFullRegisterArgumentIndex(instruction.Operands[1].Register) is int secondIndex)
        {
            if (firstIndex != secondIndex)
            {
                bool hasFirstOffset = registerStackArgumentOffsets.TryGetValue(firstIndex, out uint firstOffset);
                bool hasSecondOffset = registerStackArgumentOffsets.TryGetValue(secondIndex, out uint secondOffset);
                registerStackArgumentOffsets.Remove(firstIndex);
                registerStackArgumentOffsets.Remove(secondIndex);
                if (hasFirstOffset)
                {
                    registerStackArgumentOffsets[secondIndex] = firstOffset;
                }

                if (hasSecondOffset)
                {
                    registerStackArgumentOffsets[firstIndex] = secondOffset;
                }
            }

            return;
        }

        for (int argumentIndex = 0; argumentIndex < 3; argumentIndex++)
        {
            if (WritesRegisterArgument(instruction, argumentIndex))
            {
                registerStackArgumentOffsets.Remove(argumentIndex);
            }
        }
    }

    private static void UpdateClassRegisterTypes(
        DecodedInstruction instruction,
        AnalysisSession session,
        Dictionary<int, string> classRegisterTypes,
        Dictionary<int, StackLocalVariable> stackLocalVariables,
        Dictionary<uint, StackArgument> stackArguments,
        IReadOnlyDictionary<int, ulong> registerAddressValues)
    {
        if (instruction.FlowControl != InstructionFlowControl.Next)
        {
            if (instruction.FlowControl == InstructionFlowControl.Call
                && IsRegisterPreservingCall(instruction, session))
            {
                return;
            }

            string? callReturnType = instruction.FlowControl == InstructionFlowControl.Call
                ? GetKnownCallReturnTypeCandidate(instruction, session, classRegisterTypes)
                : null;
            classRegisterTypes.Clear();
            if (!string.IsNullOrWhiteSpace(callReturnType)
                && !string.Equals(callReturnType, "Boolean", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(callReturnType, "#TLS", StringComparison.OrdinalIgnoreCase))
            {
                classRegisterTypes[0] = callReturnType;
            }

            return;
        }

        bool isMov = string.Equals(instruction.Mnemonic, "Mov", StringComparison.OrdinalIgnoreCase);
        bool isLea = string.Equals(instruction.Mnemonic, "Lea", StringComparison.OrdinalIgnoreCase);
        if ((isMov || isLea)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetClassRegisterTypeIndex(instruction.Operands[0].Register) is int destinationIndex)
        {
            string? className = null;
            DecodedOperand source = instruction.Operands[1];
            if (isMov
                && source.Kind == DecodedOperandKind.Immediate
                && source.Immediate is ulong value
                && TryGetVmtClassName(session, value, out string? immediateClassName))
            {
                className = immediateClassName;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Immediate
                && source.Immediate is ulong immediateGlobalAddress
                && TryGetKnownGlobalDataType(session, immediateGlobalAddress, out string? immediateGlobalType))
            {
                className = immediateGlobalType;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Register
                && GetClassRegisterTypeIndex(source.Register) is int sourceIndex)
            {
                classRegisterTypes.TryGetValue(sourceIndex, out className);
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Memory
                && source.IndexRegister is null
                && IsDefaultDataSegment(source.SegmentRegister)
                && source.Displacement is null or 0
                && GetClassRegisterTypeIndex(source.BaseRegister) is int baseRegisterIndex)
            {
                if (!classRegisterTypes.TryGetValue(baseRegisterIndex, out className)
                    && registerAddressValues.TryGetValue(baseRegisterIndex, out ulong indirectGlobalAddress)
                    && TryGetKnownGlobalDataTypeOrVmtClass(
                        session,
                        indirectGlobalAddress,
                        source.MemorySizeBytes == sizeof(uint),
                        out string? indirectGlobalType))
                {
                    className = indirectGlobalType;
                }
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Memory
                && source.IndexRegister is null
                && IsDefaultDataSegment(source.SegmentRegister)
                && source.Displacement is > 0 and <= int.MaxValue
                && GetRegisterAddressIndex(source.BaseRegister) is int globalBaseRegisterIndex
                && registerAddressValues.TryGetValue(globalBaseRegisterIndex, out ulong globalBaseAddress)
                && globalBaseAddress <= uint.MaxValue - source.Displacement.Value
                && TryGetKnownGlobalDataTypeOrVmtClass(
                    session,
                    globalBaseAddress + source.Displacement.Value,
                    source.MemorySizeBytes == sizeof(uint),
                    out string? displacedGlobalType))
            {
                className = displacedGlobalType;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Memory
                && source.BaseRegister is null
                && source.IndexRegister is null
                && IsDefaultDataSegment(source.SegmentRegister)
                && source.Displacement is ulong absoluteGlobalAddress
                && TryGetKnownGlobalDataTypeOrVmtClass(
                    session,
                    absoluteGlobalAddress,
                    source.MemorySizeBytes == sizeof(uint),
                    out string? globalDataType))
            {
                className = globalDataType;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Memory
                && string.Equals(source.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && source.IndexRegister is null
                && source.Displacement is ulong loadedLocalDisplacement
                && loadedLocalDisplacement <= uint.MaxValue
                && unchecked((int)(uint)loadedLocalDisplacement) < 0
                && stackLocalVariables.TryGetValue(
                    unchecked((int)(uint)loadedLocalDisplacement),
                    out StackLocalVariable? loadedLocal)
                && loadedLocal is not null)
            {
                className = loadedLocal.TypeName;
            }
            else if (isMov
                && source.Kind == DecodedOperandKind.Memory
                && source.IndexRegister is null
                && source.Displacement is > 0 and <= int.MaxValue
                && GetClassRegisterTypeIndex(source.BaseRegister) is int objectRegisterIndex
                && classRegisterTypes.TryGetValue(objectRegisterIndex, out string? objectClassName)
                && TryGetTypedField(
                    session,
                    objectClassName,
                    (int)source.Displacement.Value,
                    out _,
                    out uint? fieldTypeInfoAddress)
                && fieldTypeInfoAddress is uint resolvedFieldTypeInfoAddress
                && session.Items.TryGetValue(resolvedFieldTypeInfoAddress, out AnalysisItem? fieldTypeInfo)
                && fieldTypeInfo.TypeKind is DelphiTypeKind.Class or DelphiTypeKind.Record)
            {
                if (fieldTypeInfo.TypeKind == DelphiTypeKind.Class
                    && fieldTypeInfo.ClassVmtAddress is uint fieldClassVmtAddress
                    && session.Items.TryGetValue(fieldClassVmtAddress, out AnalysisItem? fieldClassVmt)
                    && fieldClassVmt.Flags.HasFlag(AnalysisFlags.Vmt))
                {
                    className = fieldClassVmt.Name;
                }
                else if (fieldTypeInfo.TypeKind == DelphiTypeKind.Record)
                {
                    className = fieldTypeInfo.Name;
                }
            }
            else if ((isMov || isLea)
                && source.Kind == DecodedOperandKind.Memory
                && string.Equals(source.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && source.IndexRegister is null
                && source.Displacement is >= 8 and <= 0x10000
                && stackArguments.TryGetValue((uint)source.Displacement.Value, out StackArgument? argument)
                && argument is not null
                && !string.IsNullOrWhiteSpace(argument.TypeName))
            {
                className = argument.TypeName;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && source.BaseRegister is null
                && source.IndexRegister is null
                && source.Displacement is ulong absoluteAddress
                && TryGetVmtClassName(session, absoluteAddress, out string? addressClassName))
            {
                className = addressClassName;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && source.BaseRegister is null
                && source.IndexRegister is null
                && IsDefaultDataSegment(source.SegmentRegister)
                && source.Displacement is ulong typedGlobalAddress
                && TryGetKnownGlobalDataType(
                    session,
                    typedGlobalAddress,
                    out string? absoluteGlobalType))
            {
                className = absoluteGlobalType;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && string.Equals(source.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                && source.IndexRegister is null
                && source.Displacement is ulong localDisplacement
                && localDisplacement <= uint.MaxValue
                && unchecked((int)(uint)localDisplacement) < 0
                && stackLocalVariables.TryGetValue(
                    unchecked((int)(uint)localDisplacement),
                    out StackLocalVariable? localVariable)
                && localVariable is not null)
            {
                className = localVariable.TypeName;
            }
            else if (isLea
                && source.Kind == DecodedOperandKind.Memory
                && source.IndexRegister is null
                && source.Displacement is > 0 and <= int.MaxValue
                && GetClassRegisterTypeIndex(source.BaseRegister) is int leaBaseRegisterIndex
                && classRegisterTypes.TryGetValue(leaBaseRegisterIndex, out string? ownerTypeName)
                && TryGetTypedField(
                    session,
                    ownerTypeName,
                    (int)source.Displacement.Value,
                    out _,
                    out uint? leaFieldTypeInfoAddress)
                && leaFieldTypeInfoAddress is uint resolvedLeaFieldTypeInfoAddress
                && session.Items.TryGetValue(resolvedLeaFieldTypeInfoAddress, out AnalysisItem? leaFieldTypeInfo)
                && leaFieldTypeInfo.TypeKind == DelphiTypeKind.Record
                && !string.IsNullOrWhiteSpace(leaFieldTypeInfo.Name))
            {
                className = leaFieldTypeInfo.Name;
            }

            classRegisterTypes.Remove(destinationIndex);
            if (className is not null)
            {
                classRegisterTypes[destinationIndex] = className;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Add", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Count == 2
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && GetClassRegisterTypeIndex(instruction.Operands[0].Register) is int addDestinationIndex)
        {
            string? fieldTypeName = null;
            DecodedOperand addSource = instruction.Operands[1];
            if (addSource.Kind == DecodedOperandKind.Immediate
                && addSource.Immediate is ulong addOffset
                && addOffset is > 0 and <= int.MaxValue
                && classRegisterTypes.TryGetValue(addDestinationIndex, out string? addOwnerTypeName)
                && TryGetTypedField(
                    session,
                    addOwnerTypeName,
                    (int)addOffset,
                    out _,
                    out uint? addFieldTypeInfoAddress)
                && addFieldTypeInfoAddress is uint resolvedAddFieldTypeInfoAddress
                && session.Items.TryGetValue(resolvedAddFieldTypeInfoAddress, out AnalysisItem? addFieldTypeInfo)
                && addFieldTypeInfo.TypeKind is DelphiTypeKind.Class or DelphiTypeKind.Record)
            {
                if (addFieldTypeInfo.TypeKind == DelphiTypeKind.Class
                    && addFieldTypeInfo.ClassVmtAddress is uint addFieldClassVmtAddress
                    && session.Items.TryGetValue(addFieldClassVmtAddress, out AnalysisItem? addFieldClassVmt)
                    && addFieldClassVmt.Flags.HasFlag(AnalysisFlags.Vmt))
                {
                    fieldTypeName = addFieldClassVmt.Name;
                }
                else if (addFieldTypeInfo.TypeKind == DelphiTypeKind.Record)
                {
                    fieldTypeName = addFieldTypeInfo.Name;
                }
            }

            classRegisterTypes.Remove(addDestinationIndex);
            if (!string.IsNullOrWhiteSpace(fieldTypeName))
            {
                classRegisterTypes[addDestinationIndex] = fieldTypeName;
            }

            return;
        }

        if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase))
        {
            if (instruction.Operands.Count == 2
                && instruction.Operands[0].Kind == DecodedOperandKind.Register
                && instruction.Operands[1].Kind == DecodedOperandKind.Register
                && GetClassRegisterTypeIndex(instruction.Operands[0].Register) is int firstIndex
                && GetClassRegisterTypeIndex(instruction.Operands[1].Register) is int secondIndex)
            {
                if (firstIndex != secondIndex)
                {
                    bool hasFirstType = classRegisterTypes.TryGetValue(firstIndex, out string? firstType);
                    bool hasSecondType = classRegisterTypes.TryGetValue(secondIndex, out string? secondType);
                    classRegisterTypes.Remove(firstIndex);
                    classRegisterTypes.Remove(secondIndex);
                    if (hasFirstType)
                    {
                        classRegisterTypes[secondIndex] = firstType!;
                    }

                    if (hasSecondType)
                    {
                        classRegisterTypes[firstIndex] = secondType!;
                    }
                }

                return;
            }

            if (instruction.Operands.Count == 2)
            {
                DecodedOperand memoryOperand = instruction.Operands[0].Kind == DecodedOperandKind.Memory
                    ? instruction.Operands[0]
                    : instruction.Operands[1];
                DecodedOperand registerOperand = instruction.Operands[0].Kind == DecodedOperandKind.Register
                    ? instruction.Operands[0]
                    : instruction.Operands[1];
                if (memoryOperand.Kind == DecodedOperandKind.Memory
                    && memoryOperand.MemorySizeBytes == sizeof(uint)
                    && registerOperand.Kind == DecodedOperandKind.Register
                    && GetClassRegisterTypeIndex(registerOperand.Register) is int exchangedRegisterIndex
                    && string.Equals(memoryOperand.BaseRegister, "EBP", StringComparison.OrdinalIgnoreCase)
                    && memoryOperand.IndexRegister is null
                    && IsDefaultStackSegment(memoryOperand.SegmentRegister))
                {
                    classRegisterTypes.TryGetValue(exchangedRegisterIndex, out string? exchangedRegisterTypeName);
                    string? exchangedTypeName = null;
                    if (memoryOperand.Displacement is >= 8 and <= 0x10000
                        && stackArguments.TryGetValue(
                            (uint)memoryOperand.Displacement.Value,
                            out StackArgument? argument))
                    {
                        exchangedTypeName = argument?.TypeName;
                    }
                    else if (memoryOperand.Displacement is ulong localDisplacement
                        && localDisplacement <= uint.MaxValue
                        && unchecked((int)(uint)localDisplacement) < 0)
                    {
                        int localOffset = unchecked((int)(uint)localDisplacement);
                        exchangedTypeName = stackLocalVariables.TryGetValue(
                            localOffset,
                            out StackLocalVariable? localVariable)
                                ? localVariable?.TypeName
                                : null;
                        if (string.IsNullOrWhiteSpace(exchangedTypeName)
                            && !string.IsNullOrWhiteSpace(exchangedRegisterTypeName))
                        {
                            AddStackLocalVariable(
                                stackLocalVariables,
                                localOffset,
                                exchangedRegisterTypeName,
                                overwrite: false,
                                sizeof(uint));
                        }
                    }

                    classRegisterTypes.Remove(exchangedRegisterIndex);
                    if (!string.IsNullOrWhiteSpace(exchangedTypeName))
                    {
                        classRegisterTypes[exchangedRegisterIndex] = exchangedTypeName;
                    }

                    return;
                }
            }

            if (instruction.Operands.Count == 2)
            {
                DecodedOperand memoryOperand = instruction.Operands[0].Kind == DecodedOperandKind.Memory
                    ? instruction.Operands[0]
                    : instruction.Operands[1];
                DecodedOperand registerOperand = instruction.Operands[0].Kind == DecodedOperandKind.Register
                    ? instruction.Operands[0]
                    : instruction.Operands[1];
                if (memoryOperand.Kind == DecodedOperandKind.Memory
                    && registerOperand.Kind == DecodedOperandKind.Register
                    && memoryOperand.IndexRegister is null
                    && IsDefaultDataSegment(memoryOperand.SegmentRegister)
                    && memoryOperand.Displacement is > 0 and <= int.MaxValue
                    && GetClassRegisterTypeIndex(memoryOperand.BaseRegister) is int objectRegisterIndex
                    && classRegisterTypes.TryGetValue(objectRegisterIndex, out string? objectTypeName)
                    && GetClassRegisterTypeIndex(registerOperand.Register) is int exchangedRegisterIndex
                    && TryGetClassOrRecordFieldTypeName(
                        session,
                        objectTypeName,
                        (int)memoryOperand.Displacement.Value,
                        out string? exchangedFieldTypeName))
                {
                    classRegisterTypes.Remove(exchangedRegisterIndex);
                    if (!string.IsNullOrWhiteSpace(exchangedFieldTypeName))
                    {
                        classRegisterTypes[exchangedRegisterIndex] = exchangedFieldTypeName;
                    }

                    return;
                }
            }

            foreach (DecodedOperand operand in instruction.Operands)
            {
                if (operand.Kind == DecodedOperandKind.Register
                    && GetClassRegisterFamilyIndex(operand.Register) is int exchangedIndex)
                {
                    classRegisterTypes.Remove(exchangedIndex);
                }
            }

            return;
        }

        for (int argumentIndex = 0; argumentIndex < 6; argumentIndex++)
        {
            if (WritesTrackedRegister(instruction, argumentIndex))
            {
                classRegisterTypes.Remove(argumentIndex);
            }
        }
    }

    private static bool TryGetClassOrRecordFieldTypeName(
        AnalysisSession session,
        string ownerTypeName,
        int offset,
        out string? fieldTypeName)
    {
        if (TryGetTypedField(session, ownerTypeName, offset, out _, out uint? fieldTypeInfoAddress)
            && fieldTypeInfoAddress is uint resolvedFieldTypeInfoAddress
            && session.Items.TryGetValue(resolvedFieldTypeInfoAddress, out AnalysisItem? fieldTypeInfo))
        {
            if (fieldTypeInfo.TypeKind == DelphiTypeKind.Class
                && fieldTypeInfo.ClassVmtAddress is uint fieldClassVmtAddress
                && session.Items.TryGetValue(fieldClassVmtAddress, out AnalysisItem? fieldClassVmt)
                && fieldClassVmt.Flags.HasFlag(AnalysisFlags.Vmt)
                && !string.IsNullOrWhiteSpace(fieldClassVmt.Name))
            {
                fieldTypeName = fieldClassVmt.Name;
                return true;
            }

            if (fieldTypeInfo.TypeKind == DelphiTypeKind.Record
                && !string.IsNullOrWhiteSpace(fieldTypeInfo.Name))
            {
                fieldTypeName = fieldTypeInfo.Name;
                return true;
            }
        }

        fieldTypeName = null;
        return false;
    }

    private static bool TryGetVmtClassName(
        AnalysisSession session,
        ulong address,
        out string? className)
    {
        if (address <= uint.MaxValue
            && TryVaToRva(session, (uint)address, out uint rva)
            && session.Items.TryGetValue(rva, out AnalysisItem? item)
            && item.Flags.HasFlag(AnalysisFlags.Vmt)
            && !string.IsNullOrWhiteSpace(item.Name))
        {
            className = item.Name;
            return true;
        }

        className = null;
        return false;
    }

    private static Dictionary<uint, string> BuildProcedureOwnerClassNames(AnalysisSession session)
    {
        Dictionary<uint, List<AnalysisItem>> referencingVmtsByProcedure = [];
        foreach (AnalysisItem vmt in session.Items.Values.Where(item =>
                     item.Flags.HasFlag(AnalysisFlags.Vmt)
                     && !string.IsNullOrWhiteSpace(item.Name)))
        {
            IEnumerable<uint> methodAddresses = vmt.VirtualMethods
                .Select(method => method.CodeAddress)
                .Concat(vmt.DynamicMethods
                    .Select(method => method.CodeAddress)
                    .Where(address => address.HasValue)
                    .Select(address => address.GetValueOrDefault()))
                .Distinct();
            foreach (uint methodAddress in methodAddresses)
            {
                if (!referencingVmtsByProcedure.TryGetValue(methodAddress, out List<AnalysisItem>? referencingVmts))
                {
                    referencingVmts = [];
                    referencingVmtsByProcedure.Add(methodAddress, referencingVmts);
                }

                referencingVmts.Add(vmt);
            }
        }

        Dictionary<uint, string> procedureOwnerClassNames = [];
        foreach ((uint procedureAddress, List<AnalysisItem> referencingVmts) in referencingVmtsByProcedure)
        {
            AnalysisItem[] commonOwnerVmts = referencingVmts
                .Where(candidate => referencingVmts.All(referencingVmt =>
                    IsVmtAncestorOrSelf(session, candidate, referencingVmt)))
                .ToArray();
            AnalysisItem[] mostGeneralOwnerVmts = commonOwnerVmts
                .Where(candidate => !commonOwnerVmts.Any(other =>
                    other.Address != candidate.Address
                    && IsVmtAncestorOrSelf(session, other, candidate)))
                .ToArray();
            if (mostGeneralOwnerVmts.Length == 1
                && mostGeneralOwnerVmts[0].Name is string className)
            {
                procedureOwnerClassNames.Add(procedureAddress, className);
            }
        }

        return procedureOwnerClassNames;
    }

    private static bool IsVmtAncestorOrSelf(
        AnalysisSession session,
        AnalysisItem ancestor,
        AnalysisItem descendant)
    {
        HashSet<uint> visitedVmts = [];
        AnalysisItem? current = descendant;
        while (current is not null && visitedVmts.Add(current.Address))
        {
            if (current.Address == ancestor.Address)
            {
                return true;
            }

            current = current.ParentAddress is uint parentAddress
                && session.Items.TryGetValue(parentAddress, out AnalysisItem? parent)
                && parent.Flags.HasFlag(AnalysisFlags.Vmt)
                    ? parent
                    : null;
        }

        return false;
    }

    private static bool TryGetKnownGlobalDataType(
        AnalysisSession session,
        ulong address,
        out string? typeName)
    {
        if (address <= uint.MaxValue
            && TryVaToRva(session, (uint)address, out uint rva)
            && IsMappedDataRva(session, rva)
            && session.Items.TryGetValue(rva, out AnalysisItem? dataItem)
            && dataItem.Flags.HasFlag(AnalysisFlags.Data)
            && !string.IsNullOrWhiteSpace(dataItem.DataTypeCandidate))
        {
            typeName = dataItem.DataTypeCandidate;
            return true;
        }

        typeName = null;
        return false;
    }

    private static bool TryGetKnownGlobalDataTypeOrVmtClass(
        AnalysisSession session,
        ulong address,
        bool allowVmtPointerRead,
        out string? typeName)
    {
        if (TryGetKnownGlobalDataType(session, address, out typeName))
        {
            return true;
        }

        if (allowVmtPointerRead
            && address <= uint.MaxValue
            && TryVaToRva(session, (uint)address, out uint globalRva)
            && IsMappedDataRva(session, globalRva)
            && TryGetRawOffset(session, globalRva, out int rawOffset)
            && TryReadUInt32(session.Image.Span, rawOffset, out uint vmtAddress)
            && TryGetVmtClassName(session, vmtAddress, out typeName))
        {
            return true;
        }

        typeName = null;
        return false;
    }

    private static bool TryGetVmtClassNameFromSelfPointer(
        AnalysisSession session,
        ulong selfPointerAddress,
        out string? className)
    {
        if (selfPointerAddress <= uint.MaxValue
            && TryVaToRva(session, (uint)selfPointerAddress, out uint selfPointerRva))
        {
            foreach (VmtLayout layout in VmtLayouts)
            {
                ulong vmtRvaValue = (ulong)selfPointerRva + layout.SelfPointerDisplacement;
                if (vmtRvaValue <= uint.MaxValue
                    && session.Items.TryGetValue((uint)vmtRvaValue, out AnalysisItem? vmt)
                    && vmt.Flags.HasFlag(AnalysisFlags.Vmt)
                    && !string.IsNullOrWhiteSpace(vmt.Name))
                {
                    className = vmt.Name;
                    return true;
                }
            }
        }

        className = null;
        return false;
    }

    private static int? GetFullRegisterArgumentIndex(string? register)
    {
        if (string.Equals(register, "EAX", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (string.Equals(register, "EDX", StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        return string.Equals(register, "ECX", StringComparison.OrdinalIgnoreCase)
            ? 2
            : null;
    }

    private static int? GetClassRegisterTypeIndex(string? register)
    {
        if (GetFullRegisterArgumentIndex(register) is int argumentIndex)
        {
            return argumentIndex;
        }

        return register?.ToUpperInvariant() switch
        {
            "EBX" => 3,
            "ESI" => 4,
            "EDI" => 5,
            _ => null
        };
    }

    private static int? GetRegisterAddressIndex(string? register) =>
        GetClassRegisterTypeIndex(register);

    private static int? GetClassRegisterFamilyIndex(string? register)
    {
        if (GetRegisterArgumentIndex(register) is int argumentIndex)
        {
            return argumentIndex;
        }

        return register?.ToUpperInvariant() switch
        {
            "EBX" or "BX" or "BH" or "BL" => 3,
            "ESI" or "SI" => 4,
            "EDI" or "DI" => 5,
            _ => null
        };
    }

    private static bool WritesTrackedRegister(DecodedInstruction instruction, int registerIndex)
    {
        if (registerIndex < 3 && WritesRegisterArgument(instruction, registerIndex))
        {
            return true;
        }

        if (IsRegisterDestinationWritten(instruction)
            && GetClassRegisterFamilyIndex(instruction.Operands[0].Register) == registerIndex)
        {
            return true;
        }

        if ((string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Xadd", StringComparison.OrdinalIgnoreCase))
            && instruction.Operands.Any(operand =>
                operand.Kind == DecodedOperandKind.Register
                && GetClassRegisterFamilyIndex(operand.Register) == registerIndex))
        {
            return true;
        }

        if (registerIndex == 3
            && string.Equals(instruction.Mnemonic, "Cpuid", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (registerIndex is 3 or 4 or 5
            && (string.Equals(instruction.Mnemonic, "Popa", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Popad", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return registerIndex is 4 or 5 && IsStringInstruction(instruction.Mnemonic);
    }

    private static bool IsRegisterDestinationWritten(DecodedInstruction instruction)
    {
        return instruction.Operands.Count > 0
            && instruction.Operands[0].Kind == DecodedOperandKind.Register
            && instruction.FlowControl is not (
                InstructionFlowControl.Call
                or InstructionFlowControl.ConditionalBranch
                or InstructionFlowControl.UnconditionalBranch
                or InstructionFlowControl.IndirectBranch
                or InstructionFlowControl.Return
                or InstructionFlowControl.Interrupt)
            && !string.Equals(instruction.Mnemonic, "Push", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(instruction.Mnemonic, "Cmp", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(instruction.Mnemonic, "Test", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(instruction.Mnemonic, "Bt", StringComparison.OrdinalIgnoreCase)
                && !IsReadOnlyRegisterDestination(instruction.Mnemonic);
    }

    private static bool WritesRegisterArgument(DecodedInstruction instruction, int argumentIndex)
    {
        if (argumentIndex == 0
            && (string.Equals(instruction.Mnemonic, "Lahf", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Xlatb", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Cbw", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Cwde", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (argumentIndex == 0 && IsStringLoad(instruction.Mnemonic))
        {
            return true;
        }

        if (argumentIndex == 0 && IsBcdAccumulatorInstruction(instruction.Mnemonic))
        {
            return true;
        }

        if (argumentIndex == 2
            && (IsLoopInstruction(instruction.Mnemonic)
                || instruction.HasRepeatPrefix && IsStringInstruction(instruction.Mnemonic)))
        {
            return true;
        }

        if (argumentIndex == 0
            && string.Equals(instruction.Mnemonic, "Cmpxchg", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (argumentIndex is 0 or 1
            && string.Equals(instruction.Mnemonic, "Cmpxchg8b", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(instruction.Mnemonic, "Xadd", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Any(operand =>
                operand.Kind == DecodedOperandKind.Register
                && GetRegisterArgumentIndex(operand.Register) == argumentIndex))
        {
            return true;
        }

        if (argumentIndex is >= 0 and <= 2
            && (string.Equals(instruction.Mnemonic, "Popad", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Popa", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (argumentIndex is >= 0 and <= 2
            && string.Equals(instruction.Mnemonic, "Cpuid", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (argumentIndex is 0 or 1
            && (string.Equals(instruction.Mnemonic, "Rdpmc", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Rdtsc", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Rdmsr", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Xgetbv", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (argumentIndex is >= 0 and <= 2
            && string.Equals(instruction.Mnemonic, "Rdtscp", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (IsRegisterDestinationWritten(instruction)
            && GetRegisterArgumentIndex(instruction.Operands[0].Register) == argumentIndex)
        {
            return true;
        }

        if (string.Equals(instruction.Mnemonic, "Xchg", StringComparison.OrdinalIgnoreCase)
            && instruction.Operands.Any(operand =>
                operand.Kind == DecodedOperandKind.Register
                && GetRegisterArgumentIndex(operand.Register) == argumentIndex))
        {
            return true;
        }

        if (argumentIndex == 1
            && (string.Equals(instruction.Mnemonic, "Cdq", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Cwd", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return argumentIndex is 0 or 1
            && instruction.Operands.Count == 1
            && (string.Equals(instruction.Mnemonic, "Mul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Imul", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Div", StringComparison.OrdinalIgnoreCase)
                || string.Equals(instruction.Mnemonic, "Idiv", StringComparison.OrdinalIgnoreCase));
    }

    private static void MarkRegisterArgumentRead(
        string? register,
        bool[] available,
        bool[] used)
    {
        int? index = GetRegisterArgumentIndex(register);
        if (index is int argumentIndex && available[argumentIndex])
        {
            used[argumentIndex] = true;
        }
    }

    private static int? GetRegisterArgumentIndex(string? register)
    {
        if (register is null)
        {
            return null;
        }

        return register.ToUpperInvariant() switch
        {
            "EAX" or "AX" or "AH" or "AL" => 0,
            "EDX" or "DX" or "DH" or "DL" => 1,
            "ECX" or "CX" or "CH" or "CL" => 2,
            _ => null
        };
    }

    private static string[] GetRegisterArgumentCandidates(bool[] used)
    {
        int lastUsedIndex = Array.FindLastIndex(used, argument => argument);
        string[] registerNames = ["EAX", "EDX", "ECX"];
        return lastUsedIndex < 0
            ? []
            : registerNames[..(lastUsedIndex + 1)];
    }

    private static bool IsRegisterOperand(DecodedOperand operand, string register) =>
        operand.Kind == DecodedOperandKind.Register
        && string.Equals(operand.Register, register, StringComparison.OrdinalIgnoreCase);

    private static bool TryGetExceptionHandlerFlag(
        string? name,
        out AnalysisFlags flags)
    {
        if (string.Equals(name, "@HandleFinally", StringComparison.OrdinalIgnoreCase))
        {
            flags = AnalysisFlags.FinallyHandler;
            return true;
        }

        if (string.Equals(name, "@HandleOnException", StringComparison.OrdinalIgnoreCase)
            || string.Equals(name, "@HandleAnyException", StringComparison.OrdinalIgnoreCase)
            || string.Equals(name, "@HandleAutoException", StringComparison.OrdinalIgnoreCase))
        {
            flags = AnalysisFlags.ExceptionHandler;
            return true;
        }

        flags = AnalysisFlags.None;
        return false;
    }

    private static bool TryGetCodeSection(
        AnalysisSession session,
        uint rva,
        out PeSection section,
        out int rawOffset)
    {
        if (session.AddressMap is not null
            && session.AddressMap.TryRvaToRaw(rva, out rawOffset))
        {
            foreach (PeSection candidate in session.Sections)
            {
                if (candidate.ContainsCode
                    && rva >= candidate.VirtualAddress
                    && (ulong)rva < (ulong)candidate.VirtualAddress + candidate.VirtualSize)
                {
                    section = candidate;
                    return true;
                }
            }
        }

        section = null!;
        rawOffset = -1;
        return false;
    }

    private static void AnalyzeImports(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        int total = session.Imports.Sum(module => module.Symbols.Count);
        progress?.Report(new AnalysisProgress("Imports", 0, total));
        int completed = 0;
        foreach (PeImportModule module in session.Imports)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (PeImportSymbol symbol in module.Symbols)
            {
                cancellationToken.ThrowIfCancellationRequested();
                AnalysisItem item = session.GetOrAddItem(symbol.AddressRva);
                item.SetFlags(AnalysisFlags.Import | AnalysisFlags.Data);
                string symbolName = symbol.Name ?? $"ordinal_{symbol.Ordinal}";
                item.Name = $"{module.Name}!{symbolName}";
                progress?.Report(new AnalysisProgress("Imports", ++completed, total));
            }
        }
    }

    private static void AnalyzeExports(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        int total = session.Exports.Count;
        progress?.Report(new AnalysisProgress("Exports", 0, total));
        int completed = 0;
        foreach (PeExport export in session.Exports)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AnalysisItem item = session.GetOrAddItem(export.AddressRva);
            item.SetFlags(AnalysisFlags.Export);
            item.Name = export.Name ?? $"ordinal_{export.Ordinal}";
            progress?.Report(new AnalysisProgress("Exports", ++completed, total));
        }
    }

    private static void AnalyzeStrings(
        AnalysisSession session,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        int total = session.Sections.Count;
        progress?.Report(new AnalysisProgress("Strings", 0, total));
        int completed = 0;

        foreach (PeSection section in session.Sections)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ulong rawStart = section.RawAddress;
            ulong rawEnd = rawStart + section.RawSize;
            if (rawEnd > (ulong)session.Image.Length)
            {
                throw new InvalidDataException(
                    $"Section '{section.Name}' extends beyond the image's file data.");
            }

            ReadOnlySpan<byte> bytes = session.Image.Span.Slice(
                checked((int)rawStart),
                checked((int)section.RawSize));
            List<(int Start, int End)> managedStringRanges = [];
            if (!section.ContainsCode
                && session.SelectedDelphiVersion >= DelphiVersion.Delphi2009)
            {
                FindManagedStringLiterals(session, section, bytes, managedStringRanges);
            }

            int runStart = -1;
            int managedRangeIndex = 0;
            for (int offset = 0; offset <= bytes.Length; offset++)
            {
                if ((offset & 0x3fff) == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }


                while (managedRangeIndex < managedStringRanges.Count
                    && offset >= managedStringRanges[managedRangeIndex].End)
                {
                    managedRangeIndex++;
                }

                bool isManagedStringData = managedRangeIndex < managedStringRanges.Count
                    && offset >= managedStringRanges[managedRangeIndex].Start
                    && offset < managedStringRanges[managedRangeIndex].End;
                if (isManagedStringData)
                {
                    if (runStart >= 0)
                    {
                        AddAsciiString(session, section, bytes, runStart, offset);
                        runStart = -1;
                    }

                    continue;
                }

                if (offset < bytes.Length
                    && bytes[offset] is >= 0x20 and <= 0x7e)
                {
                    runStart = runStart < 0 ? offset : runStart;
                    continue;
                }

                if (runStart >= 0 && offset - runStart >= 4)
                {
                    AddAsciiString(session, section, bytes, runStart, offset);
                }

                runStart = -1;
            }

            progress?.Report(new AnalysisProgress("Strings", ++completed, total));
        }
    }

    private static void FindManagedStringLiterals(
        AnalysisSession session,
        PeSection section,
        ReadOnlySpan<byte> bytes,
        List<(int Start, int End)> managedStringRanges)
    {
        for (int offset = 0; offset <= bytes.Length - 12; offset += sizeof(uint))
        {
            if (!TryReadUInt16(bytes, offset, out ushort codePage)
                || !TryReadUInt16(bytes, offset + sizeof(ushort), out ushort elementSize)
                || elementSize is not (1 or 2 or 4)
                || !TryReadInt32(bytes, offset + 2L * sizeof(ushort), out int referenceCount)
                || referenceCount != -1
                || !TryReadInt32(bytes, offset + 2L * sizeof(ushort) + sizeof(uint), out int length)
                || length < 4
                || length > MaximumMetadataEntryCount)
            {
                continue;
            }

            long payloadStart = (long)offset + 12;
            long payloadEnd = payloadStart + ((long)length + 1) * elementSize;
            long alignedEnd = (payloadEnd + 3) & ~3L;
            if (alignedEnd > bytes.Length
                || !HasZeroTerminator(bytes, payloadStart + (long)length * elementSize, elementSize))
            {
                continue;
            }

            string value;
            try
            {
                value = elementSize switch
                {
                    1 when codePage == 65001 =>
                        new System.Text.UTF8Encoding(false, true)
                            .GetString(bytes.Slice((int)payloadStart, length)),
                    1 => System.Text.Encoding.Latin1.GetString(bytes.Slice((int)payloadStart, length)),
                    2 => System.Text.Encoding.Unicode.GetString(bytes.Slice((int)payloadStart, length * 2)),
                    4 => System.Text.Encoding.UTF32.GetString(bytes.Slice((int)payloadStart, length * 4)),
                    _ => string.Empty
                };
            }
            catch (System.Text.DecoderFallbackException)
            {
                continue;
            }

            if (!IsDisplayableManagedString(value))
            {
                continue;
            }

            uint contentRva = checked(section.VirtualAddress + (uint)payloadStart);
            AnalysisItem item = session.GetOrAddItem(contentRva);
            item.SetFlags(AnalysisFlags.Data | AnalysisFlags.String);
            item.TypeKind = elementSize == 1
                ? DelphiTypeKind.AnsiString
                : DelphiTypeKind.UnicodeString;
            item.StringCodePage = codePage;
            item.StringElementSize = elementSize;
            item.Name ??= value;
            managedStringRanges.Add((offset, checked((int)alignedEnd)));
            offset = checked((int)alignedEnd - sizeof(uint));
        }
    }

    private static bool HasZeroTerminator(ReadOnlySpan<byte> bytes, long offset, ushort elementSize)
    {
        if (offset < 0 || offset > bytes.Length - elementSize)
        {
            return false;
        }

        foreach (byte value in bytes.Slice((int)offset, elementSize))
        {
            if (value != 0)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsDisplayableManagedString(string value)
    {
        if (value.Length < 4)
        {
            return false;
        }

        int visibleCharacterCount = 0;
        foreach (char character in value)
        {
            if (char.IsControl(character))
            {
                return false;
            }

            if (!char.IsWhiteSpace(character))
            {
                visibleCharacterCount++;
            }
        }

        return visibleCharacterCount >= 4;
    }

    private static void AddAsciiString(
        AnalysisSession session,
        PeSection section,
        ReadOnlySpan<byte> bytes,
        int start,
        int end)
    {
        uint rva = checked(section.VirtualAddress + (uint)start);
        AnalysisItem item = session.GetOrAddItem(rva);
        item.SetFlags(AnalysisFlags.Data | AnalysisFlags.String);
        item.Name = System.Text.Encoding.ASCII.GetString(bytes[start..end]);
    }
}
