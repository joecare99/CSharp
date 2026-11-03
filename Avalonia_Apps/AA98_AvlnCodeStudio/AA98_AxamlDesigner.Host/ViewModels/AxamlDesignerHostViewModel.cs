using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AxamlDesigner.Host.ViewModels;

public partial class AxamlDesignerHostViewModel : ObservableObject, IWorkspaceOpenDocumentChangeProvider
{
    private string _markup = string.Empty;
    private string _savedMarkup = string.Empty;
    private Func<CancellationToken, Task<byte[]>>? _screenshotCapture;
    private readonly WorkspaceFileChangeService _workspaceFileChangeService;

    public AxamlDesignerHostViewModel()
    {
        Designer.MarkupChanged += OnMarkupChanged;
        _workspaceFileChangeService = new WorkspaceFileChangeService(this);
        _workspaceFileChangeService.HistoryChanged += OnWorkspaceChangeHistoryChanged;
    }

    public AxamlDesignerViewModel Designer { get; } = new();

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _statusText = "Open an AXAML file or enter a path.";

    [ObservableProperty]
    private string _workspaceRootPath = string.Empty;

    [ObservableProperty]
    private string _workspacePathInput = string.Empty;

    [ObservableProperty]
    private string _workspaceContextStatus = "No project/workspace selected. Multi-file changes are unavailable.";

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

            if (!string.IsNullOrWhiteSpace(WorkspaceRootPath)
                && !WorkspacePathIdentity.AreEqual(WorkspaceRootPath, fullPath))
            {
                _workspaceFileChangeService.ResetHistory();
            }

            WorkspaceRootPath = fullPath;
            WorkspacePathInput = fullPath;
            WorkspaceContextStatus = "Project/workspace selected. Multi-file generation can be scoped here when enabled.";
            StatusText = WorkspaceContextStatus;
            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            WorkspaceContextStatus = $"Invalid project/workspace path: {exception.Message}";
            StatusText = WorkspaceContextStatus;
            return false;
        }
    }

    public void ClearWorkspaceContext()
    {
        _workspaceFileChangeService.ResetHistory();
        WorkspaceRootPath = string.Empty;
        WorkspacePathInput = string.Empty;
        WorkspaceContextStatus = "No project/workspace selected. Multi-file changes are unavailable.";
        StatusText = WorkspaceContextStatus;
    }

    public WorkspaceFileChangeProposal PrepareWorkspaceChanges(
        IEnumerable<WorkspaceFileChangeRequest> requests)
    {
        if (string.IsNullOrWhiteSpace(WorkspaceRootPath))
        {
            throw new InvalidOperationException("Select a project/workspace before preparing multi-file changes.");
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

        SetWorkspaceContext(WorkspacePathInput);
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

    bool IWorkspaceOpenDocumentChangeProvider.CanApplyChange(string fullPath, string expectedContent)
    {
        return ((IWorkspaceOpenDocumentProvider)this).IsDocumentOpen(fullPath)
            && !IsDirty
            && string.Equals(_markup, expectedContent, StringComparison.Ordinal)
            && string.Equals(_savedMarkup, expectedContent, StringComparison.Ordinal);
    }

    void IWorkspaceOpenDocumentChangeProvider.ApplyChange(
        string fullPath,
        string expectedContent,
        string proposedContent)
    {
        if (!((IWorkspaceOpenDocumentChangeProvider)this).CanApplyChange(fullPath, expectedContent))
        {
            string relativePath = string.IsNullOrWhiteSpace(WorkspaceRootPath)
                ? Path.GetFileName(fullPath)
                : Path.GetRelativePath(WorkspaceRootPath, fullPath);
            throw new WorkspaceOpenDocumentException(relativePath);
        }

        _markup = proposedContent;
        _savedMarkup = proposedContent;
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
