using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AxamlDesigner.Host.ViewModels;

public partial class AxamlDesignerHostViewModel : ObservableObject, IWorkspaceOpenDocumentChangeProvider
{
    private string _markup = string.Empty;
    private string _savedMarkup = string.Empty;
    private Func<CancellationToken, Task<byte[]>>? _screenshotCapture;
    private readonly WorkspaceFileChangeService _workspaceFileChangeService;
    private readonly CSharpWorkspaceLoader _workspaceLoader = new();
    private readonly CSharpSyntaxDiagnosticService _csharpSyntaxDiagnosticService = new();
    private CSharpWorkspace? _workspace;
    private CancellationTokenSource? _bindingContextRefresh;

    public AxamlDesignerHostViewModel()
    {
        Designer.MarkupChanged += OnMarkupChanged;
        _workspaceFileChangeService = new WorkspaceFileChangeService(this);
        _workspaceFileChangeService.HistoryChanged += OnWorkspaceChangeHistoryChanged;
        Designer.ApplyMissingBindingAsync = ApplyMissingBindingAsync;
    }

    public AxamlDesignerViewModel Designer { get; } = new();

    public WorkspaceFileChangeService WorkspaceFileChangeService => _workspaceFileChangeService;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _statusText = "Open an AXAML file or enter a path.";

    [ObservableProperty]
    private string _workspaceRootPath = string.Empty;

    [ObservableProperty]
    private string _workspacePathInput = string.Empty;

    [ObservableProperty]
    private string _workspaceEntryPath = string.Empty;

    [ObservableProperty]
    private string _workspaceContextStatus = "No project/workspace selected. Multi-file changes are unavailable.";

    public Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? ReviewWorkspaceFileChangesAsync { get; set; }

    public bool IsDirty => !string.Equals(_markup, _savedMarkup, StringComparison.Ordinal);

    public string WindowTitle => $"AA98 AXAML Designer{(IsDirty ? " *" : string.Empty)}";

    [RelayCommand]
    private void Open()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusText = "Enter an AXAML file path to open.";
            return;
        }

        OpenDocument(FilePath);
    }

    public bool OpenDocument(string path, bool discardUnsavedChanges = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (IsDirty && !discardUnsavedChanges)
        {
            StatusText = "Save or discard the current changes before opening another file.";
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(path);
            string markup = File.ReadAllText(fullPath);
            FilePath = fullPath;
            _markup = markup;
            _savedMarkup = markup;
            Designer.LoadMarkup(markup);
            NotifyDocumentStateChanged();
            StatusText = ClearWorkspaceContextIfDocumentIsOutside(fullPath)
                ?? $"Opened {Path.GetFileName(fullPath)}.";
            QueueBindingContextRefresh();
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Open failed: {exception.Message}";
            return false;
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveDocument();
    }

    public bool SaveDocument()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusText = "Enter a destination path before saving.";
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(FilePath);
            File.WriteAllText(fullPath, _markup);
            FilePath = fullPath;
            _savedMarkup = _markup;
            NotifyDocumentStateChanged();
            StatusText = ClearWorkspaceContextIfDocumentIsOutside(fullPath)
                ?? $"Saved {Path.GetFileName(fullPath)}.";
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Save failed: {exception.Message}";
            return false;
        }
    }

    public bool SaveDocumentAs(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            string fullPath = Path.GetFullPath(path);
            File.WriteAllText(fullPath, _markup);
            FilePath = fullPath;
            _savedMarkup = _markup;
            NotifyDocumentStateChanged();
            StatusText = ClearWorkspaceContextIfDocumentIsOutside(fullPath)
                ?? $"Saved {Path.GetFileName(fullPath)}.";
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Save failed: {exception.Message}";
            return false;
        }
    }

    public void SetScreenshotCapture(Func<CancellationToken, Task<byte[]>> screenshotCapture)
    {
        ArgumentNullException.ThrowIfNull(screenshotCapture);
        _screenshotCapture = screenshotCapture;
    }

    public bool SetWorkspaceContext(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            string fullPath = WorkspacePathIdentity.Normalize(path);
            if (!Directory.Exists(fullPath))
            {
                WorkspaceContextStatus = "The selected project/workspace directory does not exist.";
                StatusText = WorkspaceContextStatus;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(FilePath) && !IsWithinWorkspace(fullPath, FilePath))
            {
                WorkspaceContextStatus = "The open AXAML file is outside the selected project/workspace.";
                StatusText = WorkspaceContextStatus;
                return false;
            }

            string? entryPath = FindUnambiguousWorkspaceEntry(fullPath);
            if (entryPath is null)
            {
                WorkspaceRootPath = fullPath;
                WorkspacePathInput = fullPath;
                WorkspaceEntryPath = string.Empty;
                _workspace = null;
                _workspaceFileChangeService.ResetHistory();
                WorkspaceContextStatus =
                    "Workspace directory selected, but no unique .sln, .slnx, or .csproj entry was found. Select one explicitly.";
                StatusText = WorkspaceContextStatus;
                QueueBindingContextRefresh();
                return true;
            }

            return SetWorkspaceEntry(entryPath, fullPath);
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or NotSupportedException)
        {
            WorkspaceContextStatus = $"Invalid project/workspace path: {exception.Message}";
            StatusText = WorkspaceContextStatus;
            return false;
        }
    }

    public bool SetWorkspaceEntry(string entryPath, string? workspaceRoot = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
        try
        {
            string fullEntryPath = Path.GetFullPath(entryPath);
            if (!File.Exists(fullEntryPath)
                || !IsSupportedWorkspaceEntry(fullEntryPath))
            {
                WorkspaceContextStatus = "Select an existing .sln, .slnx, or .csproj workspace entry.";
                StatusText = WorkspaceContextStatus;
                return false;
            }

            string fullRoot = Path.GetFullPath(workspaceRoot
                ?? (string.IsNullOrWhiteSpace(WorkspaceRootPath)
                    ? Path.GetDirectoryName(fullEntryPath)!
                    : WorkspaceRootPath));
            if (!Directory.Exists(fullRoot) || !IsWithinWorkspace(fullRoot, fullEntryPath))
            {
                WorkspaceContextStatus = "The selected workspace entry must be inside the selected workspace directory.";
                StatusText = WorkspaceContextStatus;
                return false;
            }
            if (!string.IsNullOrWhiteSpace(FilePath) && !IsWithinWorkspace(fullRoot, FilePath))
            {
                WorkspaceContextStatus = "The open AXAML file is outside the selected project/workspace.";
                StatusText = WorkspaceContextStatus;
                return false;
            }

            CSharpWorkspace workspace = _workspaceLoader.Load(fullEntryPath);
            bool workspaceChanged = _workspace is null
                || !WorkspacePathIdentity.AreEqual(_workspace.EntryPath, workspace.EntryPath)
                || !WorkspacePathIdentity.AreEqual(WorkspaceRootPath, fullRoot);
            if (workspaceChanged)
            {
                _workspaceFileChangeService.ResetHistory();
            }

            WorkspaceRootPath = fullRoot;
            WorkspacePathInput = fullRoot;
            WorkspaceEntryPath = fullEntryPath;
            _workspace = workspace;
            WorkspaceContextStatus =
                $"Workspace loaded: {Path.GetFileName(fullEntryPath)} ({workspace.Projects.Count} C# project(s)).";
            StatusText = WorkspaceContextStatus;
            QueueBindingContextRefresh();
            return true;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or NotSupportedException
            or System.Xml.XmlException
            or InvalidDataException)
        {
            WorkspaceContextStatus = $"Could not load workspace entry: {exception.Message}";
            StatusText = WorkspaceContextStatus;
            return false;
        }
    }

    [RelayCommand]
    private void ApplyWorkspaceEntry()
    {
        if (string.IsNullOrWhiteSpace(WorkspaceEntryPath))
        {
            WorkspaceContextStatus = "Select or enter a .sln, .slnx, or .csproj workspace entry.";
            StatusText = WorkspaceContextStatus;
            return;
        }

        SetWorkspaceEntry(
            WorkspaceEntryPath,
            string.IsNullOrWhiteSpace(WorkspacePathInput) ? null : WorkspacePathInput);
    }

    private static string? FindUnambiguousWorkspaceEntry(string directory)
    {
        string[] entries = Directory.EnumerateFiles(directory)
            .Where(IsSupportedWorkspaceEntry)
            .ToArray();
        string[] solutions = entries
            .Where(path => string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase)
                || string.Equals(Path.GetExtension(path), ".slnx", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (solutions.Length == 1)
        {
            return solutions[0];
        }

        if (solutions.Length > 0)
        {
            return null;
        }

        string[] projects = entries
            .Where(path => string.Equals(Path.GetExtension(path), ".csproj", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        return projects.Length == 1 ? projects[0] : null;
    }

    private static bool IsSupportedWorkspaceEntry(string path)
    {
        string extension = Path.GetExtension(path);
        return string.Equals(extension, ".sln", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".slnx", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".csproj", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ProjectReferencesMvvmToolkit(string projectPath)
    {
        System.Xml.Linq.XDocument project = System.Xml.Linq.XDocument.Load(projectPath);
        return project.Descendants().Any(element =>
        {
            if (element.Name.LocalName is not ("PackageReference" or "PackageVersion"))
            {
                return false;
            }

            string? packageId = (string?)element.Attribute("Include")
                ?? (string?)element.Attribute("Update");
            return string.Equals(
                packageId?.Split('/')[0].Trim(),
                "CommunityToolkit.Mvvm",
                StringComparison.OrdinalIgnoreCase);
        });
    }

    public void ClearWorkspaceContext()
    {
        _bindingContextRefresh?.Cancel();
        _workspace = null;
        _workspaceFileChangeService.ResetHistory();
        WorkspaceRootPath = string.Empty;
        WorkspacePathInput = string.Empty;
        WorkspaceEntryPath = string.Empty;
        WorkspaceContextStatus = "No project/workspace selected. Multi-file changes are unavailable.";
        StatusText = WorkspaceContextStatus;
        QueueBindingContextRefresh();
    }

    private void QueueBindingContextRefresh()
    {
        _bindingContextRefresh?.Cancel();
        _bindingContextRefresh?.Dispose();
        CancellationTokenSource cancellation = new();
        _bindingContextRefresh = cancellation;
        _ = RefreshBindingContextAsync(
            _markup,
            FilePath,
            _workspace,
            cancellation.Token);
    }

    private async Task RefreshBindingContextAsync(
        string markup,
        string filePath,
        CSharpWorkspace? workspace,
        CancellationToken cancellationToken)
    {
        if (workspace is null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(markup))
        {
            Designer.SetBindingContext(
                [],
                "Select a C# solution or project and open a view with a resolvable runtime DataContext to select ViewModel members.");
            return;
        }

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300), cancellationToken);
            AxamlMvvmSemanticResolution resolution =
                await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                    markup,
                    filePath,
                    workspace,
                    cancellationToken,
                    allowExistingRuntimeDataContext: true);
            string? diagnostic = resolution.IsSuccessful
                ? null
                : string.Join(Environment.NewLine, resolution.Diagnostics);
            if (resolution.IsSuccessful
                && string.IsNullOrWhiteSpace(resolution.ViewModelTypeName))
            {
                diagnostic = "The view's runtime ViewModel type could not be resolved.";
            }
            else if (resolution.IsSuccessful
                && resolution.ProjectPath is not null
                && !ProjectReferencesMvvmToolkit(resolution.ProjectPath))
            {
                diagnostic = "Member creation requires CommunityToolkit.Mvvm in the owning project.";
            }

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!ReferenceEquals(workspace, _workspace)
                    || !WorkspacePathIdentity.AreEqual(filePath, FilePath)
                    || !string.Equals(markup, _markup, StringComparison.Ordinal))
                {
                    return;
                }

                Designer.SetBindingContext(
                    diagnostic is null ? resolution.ViewModelMembers : [],
                    diagnostic);
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or ArgumentException
            or InvalidDataException
            or AggregateException
            or System.Xml.XmlException)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (ReferenceEquals(workspace, _workspace)
                    && WorkspacePathIdentity.AreEqual(filePath, FilePath)
                    && string.Equals(markup, _markup, StringComparison.Ordinal))
                {
                    Designer.SetBindingContext([], $"ViewModel analysis failed: {exception.Message}");
                }
            });
        }
    }

    private async Task<bool> ApplyMissingBindingAsync(AxamlBindingRowViewModel row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (_workspace is not { Projects.Count: > 0 } workspace
            || string.IsNullOrWhiteSpace(WorkspaceRootPath)
            || string.IsNullOrWhiteSpace(FilePath)
            || !IsWithinWorkspace(WorkspaceRootPath, FilePath))
        {
            row.Diagnostic = "Select a workspace containing the open AXAML view before creating a ViewModel member.";
            return false;
        }

        string memberName = row.NewMemberName.Trim();
        if (!AxamlMvvmMemberName.IsValid(memberName))
        {
            row.Diagnostic = "Enter a valid simple C# member name.";
            return false;
        }

        try
        {
            _bindingContextRefresh?.Cancel();
            AxamlMvvmSemanticResolution resolution =
                await _csharpSyntaxDiagnosticService.AnalyzeAxamlMvvmAsync(
                    _markup,
                    FilePath,
                    workspace,
                    allowExistingRuntimeDataContext: true);
            if (!resolution.IsSuccessful
                || !resolution.HasRuntimeDataContext
                || string.IsNullOrWhiteSpace(resolution.ViewModelTypeName)
                || string.IsNullOrWhiteSpace(resolution.ProjectPath))
            {
                throw new InvalidDataException(
                    "The open view must have an unambiguous runtime ViewModel before a member can be created."
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, resolution.Diagnostics));
            }

            AxamlMvvmMemberRequirement requirement = new(
                memberName,
                row.MemberTypeName,
                row.IsCommand);
            AxamlMvvmMemberPlanEntry plan = new AxamlMvvmMemberPlanner()
                .Plan([requirement], resolution.ViewModelMembers, createMissingMembers: true)
                .Single();
            if (plan.Action != AxamlMvvmMemberPlanAction.Create)
            {
                throw new InvalidDataException(plan.Diagnostic
                    ?? $"ViewModel member '{memberName}' already exists; choose it from the existing member list.");
            }

            WorkspaceFileChangeRequest sourceRequest =
                await _csharpSyntaxDiagnosticService.PlanViewModelSourceAdditionAsync(
                    WorkspaceRootPath,
                    resolution.ProjectPath,
                    resolution.ViewModelTypeName,
                    [plan]);
            IReadOnlyList<string> newCompilerErrors =
                await _csharpSyntaxDiagnosticService.GetNewCompilerErrorsAsync(
                    resolution.ProjectPath,
                    sourceRequest);
            if (newCompilerErrors.Count > 0)
            {
                throw new InvalidDataException(
                    "Creating this member would introduce C# compiler errors:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, newCompilerErrors));
            }

            string proposedMarkup = Designer.CreateBindingMarkupForRow(row);
            WorkspaceFileChangeRequest axamlRequest = WorkspaceFileChangeRequest.ReplaceExisting(
                Path.GetRelativePath(WorkspaceRootPath, FilePath),
                _savedMarkup,
                proposedMarkup,
                expectedOpenDocumentContent: _markup);
            WorkspaceFileChangeProposal proposal = _workspaceFileChangeService.Prepare(
                WorkspaceRootPath,
                [axamlRequest, sourceRequest]);
            Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? review =
                ReviewWorkspaceFileChangesAsync;
            if (review is null)
            {
                row.Diagnostic = "This host does not provide a workspace change review dialog.";
                return false;
            }

            if (!await review(proposal, CancellationToken.None))
            {
                StatusText = "Binding and ViewModel member creation cancelled; no files were changed.";
                return false;
            }

            await _csharpSyntaxDiagnosticService.RefreshProjectContextAsync(resolution.ProjectPath);
            QueueBindingContextRefresh();
            StatusText = $"Created ViewModel member '{memberName}' and applied the AXAML binding.";
            return true;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or InvalidDataException
            or ArgumentException
            or AggregateException)
        {
            row.Diagnostic = exception.Message;
            StatusText = "Binding and ViewModel member creation failed; no changes were applied.";
            return false;
        }
    }

    public WorkspaceFileChangeProposal PrepareWorkspaceChanges(
        IEnumerable<WorkspaceFileChangeRequest> requests)
    {
        if (string.IsNullOrWhiteSpace(WorkspaceRootPath))
        {
            throw new InvalidOperationException("Select a workspace directory before preparing multi-file changes.");
        }

        return _workspaceFileChangeService.Prepare(WorkspaceRootPath, requests);
    }

    public void ApplyWorkspaceChanges(WorkspaceFileChangeProposal proposal, bool confirmed)
    {
        ArgumentNullException.ThrowIfNull(proposal);
        if (string.IsNullOrWhiteSpace(WorkspaceRootPath)
            || !WorkspacePathIdentity.AreEqual(WorkspaceRootPath, proposal.WorkspaceRoot))
        {
            throw new InvalidOperationException("The change proposal does not match the selected project/workspace.");
        }

        _workspaceFileChangeService.Apply(proposal, confirmed);
        StatusText = _workspaceFileChangeService.HistoryWarning
            ?? "Confirmed workspace changes applied.";
    }

    [RelayCommand(CanExecute = nameof(CanUndoWorkspaceChange))]
    private void UndoWorkspaceChange()
    {
        try
        {
            if (_workspaceFileChangeService.Undo())
            {
                StatusText = _workspaceFileChangeService.HistoryWarning
                    ?? "Undid the last confirmed workspace change.";
            }
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or ArgumentException
            or AggregateException)
        {
            StatusText = $"Workspace undo failed: {exception.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanRedoWorkspaceChange))]
    private void RedoWorkspaceChange()
    {
        try
        {
            if (_workspaceFileChangeService.Redo())
            {
                StatusText = _workspaceFileChangeService.HistoryWarning
                    ?? "Redid the last undone workspace change.";
            }
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or ArgumentException
            or AggregateException)
        {
            StatusText = $"Workspace redo failed: {exception.Message}";
        }
    }

    private bool CanUndoWorkspaceChange() => _workspaceFileChangeService.CanUndo;

    private bool CanRedoWorkspaceChange() => _workspaceFileChangeService.CanRedo;

    private void OnWorkspaceChangeHistoryChanged(object? sender, EventArgs e)
    {
        UndoWorkspaceChangeCommand.NotifyCanExecuteChanged();
        RedoWorkspaceChangeCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void ApplyWorkspaceContext()
    {
        if (string.IsNullOrWhiteSpace(WorkspacePathInput))
        {
            ClearWorkspaceContext();
            return;
        }

        if (!string.IsNullOrWhiteSpace(WorkspaceEntryPath))
        {
            SetWorkspaceEntry(WorkspaceEntryPath, WorkspacePathInput);
        }
        else
        {
            SetWorkspaceContext(WorkspacePathInput);
        }
    }

    public Task<byte[]> CaptureScreenshotPngAsync(CancellationToken cancellationToken = default)
    {
        return (_screenshotCapture
            ?? throw new InvalidOperationException("The designer preview view is not available for screenshot capture."))(cancellationToken);
    }

    public bool SelectElement(string nodePath)
    {
        ArgumentNullException.ThrowIfNull(nodePath);
        AxamlPreviewNode? node = Designer.PreviewRoot;
        if (node is null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(nodePath))
        {
            foreach (string segment in nodePath.Split('/'))
            {
                if (!int.TryParse(segment, out int index) || index < 0 || index >= node.Children.Count)
                {
                    return false;
                }

                node = node.Children[index];
            }
        }

        Designer.SelectedNode = node;
        return true;
    }

    public bool SetElementProperty(string nodePath, string propertyName, string value)
    {
        if (!SelectElement(nodePath))
        {
            return false;
        }

        Designer.SelectedProperty = propertyName;
        Designer.SelectedValue = value;
        if (!Designer.ApplySelectedValueCommand.CanExecute(null))
        {
            throw new InvalidOperationException($"The property '{propertyName}' cannot be edited on the selected element.");
        }

        Designer.ApplySelectedValueCommand.Execute(null);
        return true;
    }

    public string GetDesignerStateJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(new
        {
            FilePath,
            WorkspaceRootPath,
            WorkspaceContextStatus,
            Designer.ErrorText,
            SelectedElementName = Designer.SelectedElementName,
            SelectedProperty = Designer.SelectedProperty,
            SelectedValue = Designer.SelectedValue,
            Elements = Flatten(Designer.PreviewRoot, string.Empty),
        });
    }

    private static IReadOnlyList<object> Flatten(AxamlPreviewNode? node, string path)
    {
        if (node is null)
        {
            return [];
        }

        List<object> result = [new
        {
            Path = path,
            node.TypeName,
            node.Text,
            EditableProperties = node.IsSupported
                ? new AxamlPreviewService().GetEditableProperties(node)
                : [],
        }];
        for (int i = 0; i < node.Children.Count; i++)
        {
            result.AddRange(Flatten(node.Children[i], string.IsNullOrEmpty(path) ? i.ToString() : $"{path}/{i}"));
        }

        return result;
    }

    private void OnMarkupChanged(object? sender, AxamlDesignerMarkupChangedEventArgs e)
    {
        _markup = e.Markup;
        NotifyDocumentStateChanged();
        StatusText = "Designer changes are unsaved.";
        QueueBindingContextRefresh();
    }

    private void NotifyDocumentStateChanged()
    {
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(WindowTitle));
    }

    private string? ClearWorkspaceContextIfDocumentIsOutside(string filePath)
    {
        if (string.IsNullOrWhiteSpace(WorkspaceRootPath)
            || IsWithinWorkspace(WorkspaceRootPath, filePath))
        {
            return null;
        }

        ClearWorkspaceContext();
        return "Project/workspace context cleared because the current file is outside it.";
    }

    bool IWorkspaceOpenDocumentProvider.IsDocumentOpen(string fullPath)
    {
        return !string.IsNullOrWhiteSpace(FilePath)
            && WorkspacePathIdentity.AreEqual(FilePath, fullPath);
    }

    bool IWorkspaceOpenDocumentChangeProvider.CanApplyChange(
        string fullPath,
        string expectedContent,
        string expectedSavedContent)
    {
        return ((IWorkspaceOpenDocumentProvider)this).IsDocumentOpen(fullPath)
            && string.Equals(_markup, expectedContent, StringComparison.Ordinal)
            && string.Equals(_savedMarkup, expectedSavedContent, StringComparison.Ordinal);
    }

    void IWorkspaceOpenDocumentChangeProvider.ApplyChange(
        string fullPath,
        string expectedContent,
        string expectedSavedContent,
        string proposedContent,
        string proposedSavedContent)
    {
        if (!((IWorkspaceOpenDocumentChangeProvider)this).CanApplyChange(
                fullPath,
                expectedContent,
                expectedSavedContent))
        {
            string relativePath = string.IsNullOrWhiteSpace(WorkspaceRootPath)
                ? Path.GetFileName(fullPath)
                : Path.GetRelativePath(WorkspaceRootPath, fullPath);
            throw new WorkspaceOpenDocumentException(relativePath);
        }

        _markup = proposedContent;
        _savedMarkup = proposedSavedContent;
        Designer.LoadMarkup(proposedContent);
        NotifyDocumentStateChanged();
        StatusText = "Open AXAML document synchronized with confirmed workspace changes.";
    }

    private static bool IsWithinWorkspace(string workspaceRoot, string filePath)
    {
        string relativePath = Path.GetRelativePath(workspaceRoot, filePath);
        return !Path.IsPathRooted(relativePath)
            && relativePath != ".."
            && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            && !relativePath.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal);
    }
}
