using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IDR.App.Services;
using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileSelectionService _fileSelectionService;
    private readonly IPeImageLoader _imageLoader;
    private readonly IDelphiVersionDetector _versionDetector;
    private readonly IKnowledgeBasePathResolver _knowledgeBasePathResolver;
    private readonly IKnowledgeBaseProvider _knowledgeBaseProvider;
    private readonly IAnalysisService _analysisService;
    private CancellationTokenSource? _analysisCancellation;
    private PeImage? _currentImage;
    private AnalysisSession? _currentSession;
    private IKnowledgeBase? _currentKnowledgeBase;

    public MainWindowViewModel(
        IFileSelectionService fileSelectionService,
        IPeImageLoader imageLoader,
        IDelphiVersionDetector versionDetector,
        IKnowledgeBasePathResolver knowledgeBasePathResolver,
        IKnowledgeBaseProvider knowledgeBaseProvider,
        IAnalysisService analysisService)
    {
        _fileSelectionService = fileSelectionService
            ?? throw new ArgumentNullException(nameof(fileSelectionService));
        _imageLoader = imageLoader
            ?? throw new ArgumentNullException(nameof(imageLoader));
        _versionDetector = versionDetector
            ?? throw new ArgumentNullException(nameof(versionDetector));
        _knowledgeBasePathResolver = knowledgeBasePathResolver
            ?? throw new ArgumentNullException(nameof(knowledgeBasePathResolver));
        _knowledgeBaseProvider = knowledgeBaseProvider
            ?? throw new ArgumentNullException(nameof(knowledgeBaseProvider));
        _analysisService = analysisService
            ?? throw new ArgumentNullException(nameof(analysisService));
    }

    public ObservableCollection<AnalysisItemViewModel> Items { get; } = [];

    public ObservableCollection<DisassemblyLineViewModel> DisassemblyLines { get; } = [];

    public ObservableCollection<DelphiVersionOption> VersionOptions { get; } = [];

    [ObservableProperty]
    private string _windowTitle = "IDR Avalonia";

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private string _diagnosticText = string.Empty;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _detectedVersionText = string.Empty;

    [ObservableProperty]
    private string _knowledgeBaseText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadKnowledgeBaseCommand))]
    private DelphiVersionOption? _selectedVersion;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private DisassemblyLineViewModel? _selectedDisassemblyLine;

    [ObservableProperty]
    private AnalysisItemViewModel? _selectedAnalysisItem;

    [RelayCommand]
    private async Task OpenAsync()
    {
        DiagnosticText = string.Empty;
        string? selectedPath = await _fileSelectionService
            .SelectPeImageAsync(CancellationToken.None)
            .ConfigureAwait(true);
        if (selectedPath is null)
        {
            return;
        }

        await AnalyzeAsync(selectedPath).ConfigureAwait(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        _analysisCancellation?.Cancel();
    }

    private async Task AnalyzeAsync(string path)
    {
        _analysisCancellation?.Dispose();
        _analysisCancellation = new CancellationTokenSource();
        IsBusy = true;
        StatusText = "Loading...";
        SourcePath = path;
        Items.Clear();
        SelectedAnalysisItem = null;
        DisassemblyLines.Clear();
        VersionOptions.Clear();
        SelectedVersion = null;
        _currentImage = null;
        _currentSession = null;
        _currentKnowledgeBase = null;
        KnowledgeBaseText = string.Empty;

        try
        {
            PeImage image = await _imageLoader
                .LoadAsync(path, _analysisCancellation.Token)
                .ConfigureAwait(true);
            _currentImage = image;
            DelphiVersionDetection detection = _versionDetector.Detect(image);
            VersionOptions.Clear();
            foreach (DelphiVersion candidate in detection.Candidates)
            {
                VersionOptions.Add(new DelphiVersionOption(candidate, candidate.ToString()));
            }

            SelectedVersion = detection.SelectedVersion == DelphiVersion.Unknown
                ? null
                : VersionOptions.FirstOrDefault(option => option.Version == detection.SelectedVersion);
            DetectedVersionText = SelectedVersion is null
                ? "Delphi version: select a candidate"
                : $"Delphi version: {SelectedVersion.DisplayName}";
            List<string> diagnostics = [];
            if (SelectedVersion is not null)
            {
                string? diagnostic = await TryLoadKnowledgeBaseAsync(
                    SelectedVersion.Version,
                    _analysisCancellation.Token).ConfigureAwait(true);
                if (diagnostic is not null)
                {
                    diagnostics.Add(diagnostic);
                }
            }
            else
            {
                KnowledgeBaseText = "Knowledge Base not loaded: select a Delphi version";
            }

            AnalysisSession session = new(path, image.Image, image.Sections, image.ImageBase)
            {
                EntryPointRva = image.EntryPointRva,
                SelectedDelphiVersion = SelectedVersion?.Version ?? DelphiVersion.Unknown
            };
            session.LoadPeDirectories(image.Imports, image.Exports);
            _currentSession = session;
            if (_currentKnowledgeBase is not null)
            {
                session.LoadKnowledgeBase(_currentKnowledgeBase);
            }

            Progress<AnalysisProgress> progress = new(value =>
            {
                StatusText = $"{value.Phase} ({value.Completed}/{value.Total})";
            });
            AnalysisResult result = await _analysisService
                .AnalyzeAsync(session, progress, _analysisCancellation.Token)
                .ConfigureAwait(true);

            foreach (AnalysisItem item in session.Items.Values)
            {
                Items.Add(new AnalysisItemViewModel(item, session));
            }

            RefreshDisassemblyLines();
            diagnostics.AddRange(result.Diagnostics);
            DiagnosticText = string.Join(Environment.NewLine, diagnostics);
            StatusText = diagnostics.Count == 0 ? "Analysis complete" : "Analysis completed with diagnostics";
        }
        catch (OperationCanceledException)
        {
            StatusText = "Analysis canceled";
        }
        catch (InvalidDataException exception)
        {
            DiagnosticText = exception.Message;
            StatusText = "Unable to load image";
        }
        catch (IOException exception)
        {
            DiagnosticText = exception.Message;
            StatusText = "Unable to read image";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanLoadKnowledgeBase))]
    private async Task LoadKnowledgeBaseAsync()
    {
        if (_currentImage is null || SelectedVersion is null)
        {
            return;
        }

        IsBusy = true;
        try
        {
            string? diagnostic = await TryLoadKnowledgeBaseAsync(
                SelectedVersion.Version,
                CancellationToken.None).ConfigureAwait(true);
            if (_currentSession is not null && _currentKnowledgeBase is not null)
            {
                _currentSession.SelectedDelphiVersion = SelectedVersion.Version;
                _currentSession.LoadKnowledgeBase(_currentKnowledgeBase);
                _currentSession.ClearAnalysisItems();
                AnalysisResult result = await _analysisService.AnalyzeAsync(
                    _currentSession,
                    null,
                    CancellationToken.None).ConfigureAwait(true);
                Items.Clear();
                foreach (AnalysisItem item in _currentSession.Items.Values)
                {
                    Items.Add(new AnalysisItemViewModel(item, _currentSession));
                }

                RefreshDisassemblyLines();
                diagnostic = JoinDiagnostics(diagnostic, result.Diagnostics);
            }

            DetectedVersionText = $"Delphi version: {SelectedVersion.DisplayName}";
            DiagnosticText = diagnostic ?? string.Empty;
            StatusText = diagnostic is null
                ? "Analysis complete"
                : "Analysis completed with diagnostics";
        }
        catch (InvalidDataException exception)
        {
            KnowledgeBaseText = "Knowledge Base invalid";
            DiagnosticText = exception.Message;
            StatusText = "Unable to load Knowledge Base";
        }
        catch (IOException exception)
        {
            KnowledgeBaseText = "Knowledge Base unavailable";
            DiagnosticText = exception.Message;
            StatusText = "Unable to read Knowledge Base";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanLoadKnowledgeBase() => SelectedVersion is not null && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanFollowBranch))]
    private void FollowBranch()
    {
        if (SelectedDisassemblyLine?.BranchTarget is not uint target)
        {
            return;
        }

        SelectedDisassemblyLine = DisassemblyLines
            .FirstOrDefault(line => line.Address == target);
    }

    private bool CanFollowBranch() =>
        SelectedDisassemblyLine?.BranchTarget is not null
        && DisassemblyLines.Any(line => line.Address == SelectedDisassemblyLine.BranchTarget);

    [RelayCommand(CanExecute = nameof(CanFollowCrossReference))]
    private void FollowCrossReference()
    {
        if (SelectedAnalysisItem is null
            || _currentSession is null
            || !_currentSession.Items.TryGetValue(SelectedAnalysisItem.Address, out AnalysisItem? source))
        {
            return;
        }

        uint? targetAddress = source.CrossReferences.FirstOrDefault()?.TargetAddress;
        if (targetAddress is uint address)
        {
            SelectedAnalysisItem = Items.FirstOrDefault(item => item.Address == address);
        }
    }

    private bool CanFollowCrossReference()
    {
        if (SelectedAnalysisItem is null
            || _currentSession is null
            || !_currentSession.Items.TryGetValue(SelectedAnalysisItem.Address, out AnalysisItem? source))
        {
            return false;
        }

        uint? targetAddress = source.CrossReferences.FirstOrDefault()?.TargetAddress;
        return targetAddress is uint address && Items.Any(item => item.Address == address);
    }

    private void RefreshDisassemblyLines()
    {
        DisassemblyLines.Clear();
        if (_currentSession is null)
        {
            return;
        }

        foreach (DisassemblyLine line in _currentSession.DisassemblyLines)
        {
            DisassemblyLines.Add(new DisassemblyLineViewModel(
                line.Address,
                line.Bytes,
                line.Mnemonic,
                line.FormattedText,
                line.BranchTarget,
                line.FlowControl));
        }

        SelectedDisassemblyLine = DisassemblyLines.FirstOrDefault();
    }

    private async Task<string?> TryLoadKnowledgeBaseAsync(
        DelphiVersion version,
        CancellationToken cancellationToken)
    {
        string knowledgeBasePath = _knowledgeBasePathResolver.ResolvePath(version);
        try
        {
            IKnowledgeBase knowledgeBase = await _knowledgeBaseProvider
                .OpenAsync(knowledgeBasePath, cancellationToken)
                .ConfigureAwait(true);
            _currentKnowledgeBase = knowledgeBase;
            KnowledgeBaseText = $"Knowledge Base: {knowledgeBase.Modules.Count} modules";
            return null;
        }
        catch (Exception exception) when (
            exception is FileNotFoundException or DirectoryNotFoundException)
        {
            KnowledgeBaseText = "Knowledge Base not found";
            _currentKnowledgeBase = null;
            return $"Knowledge Base file not found: {knowledgeBasePath}";
        }
        catch (InvalidDataException exception)
        {
            KnowledgeBaseText = "Knowledge Base invalid";
            _currentKnowledgeBase = null;
            return $"Invalid Knowledge Base file: {exception.Message}";
        }
        catch (IOException exception)
        {
            KnowledgeBaseText = "Knowledge Base unavailable";
            _currentKnowledgeBase = null;
            return $"Unable to read Knowledge Base file: {exception.Message}";
        }
    }

    private static string? JoinDiagnostics(string? first, IReadOnlyList<string> second)
    {
        string combined = string.Join(
            Environment.NewLine,
            new[] { first }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Concat(second));
        return combined.Length == 0 ? null : combined;
    }

    partial void OnSelectedVersionChanged(DelphiVersionOption? value)
    {
        LoadKnowledgeBaseCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedDisassemblyLineChanged(DisassemblyLineViewModel? value)
    {
        FollowBranchCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedAnalysisItemChanged(AnalysisItemViewModel? value)
    {
        FollowCrossReferenceCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value)
    {
        LoadKnowledgeBaseCommand.NotifyCanExecuteChanged();
    }
}

public sealed record DelphiVersionOption(DelphiVersion Version, string DisplayName);

public sealed record DisassemblyLineViewModel(
    uint Address,
    string Bytes,
    string Mnemonic,
    string FormattedText,
    uint? BranchTarget,
    InstructionFlowControl FlowControl);

public sealed record AnalysisItemViewModel(
    uint Address,
    AnalysisFlags Flags,
    string Name,
    int CrossReferenceCount,
    int IncomingCrossReferenceCount,
    string CrossReferenceTargets)
{
    public AnalysisItemViewModel(AnalysisItem item)
        : this(item.Address, item.Flags, item.Name ?? string.Empty, item.CrossReferences.Count, 0, string.Empty)
    {
    }

    public AnalysisItemViewModel(AnalysisItem item, AnalysisSession session)
        : this(
            item.Address,
            item.Flags,
            item.Name ?? string.Empty,
            item.CrossReferences.Count,
            session.GetIncomingCrossReferences(item.Address).Count,
            string.Join(", ", item.CrossReferences.Select(reference =>
                reference.TargetName is null
                    ? $"0x{reference.TargetAddress:X8}"
                    : $"{reference.TargetName} (0x{reference.TargetAddress:X8})")))
    {
        TypeKind = item.TypeKind?.ToString() ?? string.Empty;
        RecordSize = item.RecordSizeBytes is uint recordSizeBytes
            ? $"{recordSizeBytes} bytes"
            : string.Empty;
        ClassInstanceSize = item.ClassInstanceSizeBytes is uint classInstanceSizeBytes
            ? $"{classInstanceSizeBytes} bytes"
            : string.Empty;
        UnitName = item.UnitName ?? string.Empty;
        ClassVmt = item.ClassVmtAddress is uint classVmtAddress
            ? $"0x{classVmtAddress:X8}"
            : string.Empty;
        ParentTypeInfo = item.ParentTypeInfoAddress is uint parentTypeInfoAddress
            ? $"0x{parentTypeInfoAddress:X8}"
            : string.Empty;
        Properties = string.Join(", ", item.PropertyNames);
        Methods = string.Join(", ", item.Methods.Select(method =>
            method.CodeAddress is uint address
                ? $"{method.Name} (0x{address:X8})"
                : method.Name)
            .Select((methodText, index) => item.Methods[index].IsExtended
                ? $"{methodText} [virtual {item.Methods[index].VirtualIndex}]"
                : methodText));
        Fields = string.Join(", ", item.Fields.Select(field =>
            FormatField(field, session)));
        RecordFields = string.Join(", ", item.RecordFields.Select(field =>
            $"{field.Name} @ {field.Offset}"
                + (field.TypeInfoAddress is uint typeInfoAddress
                    && session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeInfo)
                    ? $": {typeInfo.Name ?? $"0x{typeInfoAddress:X8}"}"
                    : string.Empty)));
        MemberAccessCandidates = string.Join(", ", item.MemberAccessCandidates.Select(access =>
            $"{access.OwnerTypeName}.{access.FieldName} (+0x{access.Offset:X})"
                + (access.TypeName is null ? string.Empty : $": {access.TypeName}")));
        Interfaces = string.Join(", ", item.Interfaces.Select(intf =>
            FormatInterface(intf, session)));
        DynamicMethods = string.Join(", ", item.DynamicMethods.Select(method =>
            method.CodeAddress is uint address
                ? $"0x{method.MessageId:X4} -> 0x{address:X8}"
                : $"0x{method.MessageId:X4} -> unknown"));
        AutoMethods = string.Join(", ", item.AutoMethods.Select(method =>
            $"0x{method.DispatchId:X8} {method.Name} -> 0x{method.CodeAddress:X8}"));
        InitializationFields = string.Join(", ", item.InitializationFields.Select(field =>
            FormatInitializationField(field, session)));
        VirtualMethods = string.Join(", ", item.VirtualMethods.Select(method =>
            $"0x{method.SlotOffset:X} -> 0x{method.CodeAddress:X8}"));
        ExceptionHandlers = string.Join(", ", item.ExceptionHandlers.Select(handler =>
            $"{FormatAddress(handler.ExceptionInfoAddress)} -> {FormatAddress(handler.ProcedureAddress)}"));
        ExceptionRegionHandler = FormatAddress(item.ExceptionRegionHandlerAddress);
        FinallyRegionEnd = FormatAddress(item.FinallyRegionEndAddress);
        Int64ComparisonEnd = FormatAddress(item.Int64ComparisonEndAddress);
        ProcedureSizeBytes = item.ProcedureSizeBytes;
        ReturnStackBytes = item.ReturnStackBytes;
        ReturnTypeCandidate = item.ReturnTypeCandidate ?? string.Empty;
        DataTypeCandidate = item.DataTypeCandidate ?? string.Empty;
        UsesFramePointer = item.UsesFramePointer;
        StackPointerDeltaBytes = item.StackPointerDeltaBytes;
        StackArguments = string.Join(", ", item.StackArguments.Select(argument =>
            $"+{argument.Offset} ({argument.SizeBytes} bytes{(argument.TypeName is null ? string.Empty : $", {argument.TypeName}")})"));
        StackLocalVariables = string.Join(", ", item.StackLocalVariables.Select(variable =>
            $"{variable.TypeName} at EBP{variable.EbpOffset:+0;-0;0} ({variable.SizeBytes} bytes)"));
        RegisterArgumentCandidates = string.Join(", ", item.RegisterArgumentCandidates);
        ParentVmt = item.ParentAddress is uint parentAddress
            ? session.Items.TryGetValue(parentAddress, out AnalysisItem? parentItem)
                && parentItem.Flags.HasFlag(AnalysisFlags.Vmt)
                    ? $"{parentItem.Name ?? "VMT"} (0x{parentAddress:X8})"
                    : $"0x{parentAddress:X8}"
            : string.Empty;
    }

    public string TypeKind { get; init; } = string.Empty;

    public string RecordSize { get; init; } = string.Empty;

    public string ClassInstanceSize { get; init; } = string.Empty;

    public string UnitName { get; init; } = string.Empty;

    public string ClassVmt { get; init; } = string.Empty;

    public string ParentTypeInfo { get; init; } = string.Empty;

    public string Properties { get; init; } = string.Empty;

    public string Methods { get; init; } = string.Empty;

    public string Fields { get; init; } = string.Empty;

    public string RecordFields { get; init; } = string.Empty;

    public string MemberAccessCandidates { get; init; } = string.Empty;

    public string Interfaces { get; init; } = string.Empty;

    public string DynamicMethods { get; init; } = string.Empty;

    public string AutoMethods { get; init; } = string.Empty;

    public string InitializationFields { get; init; } = string.Empty;

    public string VirtualMethods { get; init; } = string.Empty;

    public string ExceptionHandlers { get; init; } = string.Empty;

    public string ExceptionRegionHandler { get; init; } = string.Empty;

    public string FinallyRegionEnd { get; init; } = string.Empty;

    public string Int64ComparisonEnd { get; init; } = string.Empty;

    public uint? ProcedureSizeBytes { get; init; }

    public ushort? ReturnStackBytes { get; init; }

    public string ReturnTypeCandidate { get; init; } = string.Empty;

    public string DataTypeCandidate { get; init; } = string.Empty;

    public bool UsesFramePointer { get; init; }

    public long? StackPointerDeltaBytes { get; init; }

    public string StackArguments { get; init; } = string.Empty;

    public string StackLocalVariables { get; init; } = string.Empty;

    public string RegisterArgumentCandidates { get; init; } = string.Empty;

    public string ParentVmt { get; init; } = string.Empty;

    private static string FormatAddress(uint? address) =>
        address is uint value ? $"0x{value:X8}" : "unknown";

    private static string FormatField(DelphiVmtField field, AnalysisSession session)
    {
        string typeDescription = field.TypeInfoAddress is uint typeInfoAddress
            ? session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeItem)
                ? $"{typeItem.Name ?? "RTTI"} (0x{typeInfoAddress:X8})"
                : $"0x{typeInfoAddress:X8}"
            : "type unknown";
        string extendedDescription = field.IsExtended
            ? $", flags 0x{field.Flags.GetValueOrDefault():X2}"
            : string.Empty;
        return $"{field.Name} @ {field.Offset} (type {typeDescription}{extendedDescription})";
    }

    private static string FormatInterface(DelphiVmtInterface intf, AnalysisSession session)
    {
        string identity = intf.TypeInfoAddress is uint typeInfoAddress
            && session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeItem)
            && typeItem.Name is not null
                ? $"{typeItem.Name} ({intf.Id:D})"
                : intf.Id.ToString("D");
        string vTable = intf.VTableAddress is uint address
            ? $"0x{address:X8}"
            : "unknown";
        string getter = intf.ImplementationGetter is int getterValue && getterValue != 0
            ? $", getter {getterValue}"
            : string.Empty;
        return $"{identity} (vtable {vTable}, offset {intf.Offset}{getter})";
    }

    private static string FormatInitializationField(
        DelphiVmtInitializationField field,
        AnalysisSession session)
    {
        string typeDescription = field.TypeInfoAddress is uint typeInfoAddress
            && session.Items.TryGetValue(typeInfoAddress, out AnalysisItem? typeItem)
            && typeItem.Name is not null
                ? typeItem.Name
                : field.TypeInfoAddress is uint address
                    ? $"0x{address:X8}"
                    : "type unknown";
        return $"{typeDescription} @ {field.Offset}";
    }
}
