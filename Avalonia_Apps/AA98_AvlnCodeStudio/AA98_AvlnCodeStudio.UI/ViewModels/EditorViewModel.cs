using AA98_AvlnCodeStudio.Base.ViewModels;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Provides the main editor workflow for the first Code Studio prototype.
/// </summary>
public partial class EditorViewModel : CodeStudioViewModelBase
{
    private readonly IEditorWorkflow _workflow;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModel"/> class.
    /// </summary>
    /// <param name="workflow">The editor workflow core.</param>
    public EditorViewModel(IEditorWorkflow workflow)
    {
        _workflow = workflow;

        _text = _workflow.Document.Content;
        _statusText = UiStrings.ReadyStatusText;
        _notificationText = UiStrings.ReadyStatusText;
        SynchronizeFromDocument();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModel"/> class for design-time usage.
    /// </summary>
    public EditorViewModel()
        : this(new EditorWorkflow(
            new Model.Documents.FileEditorDocument(),
            new DesignEditorFileDialogService(),
            new DesignTextDocumentStorageService()))
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    private string _text;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    private bool _isDirty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentFilePath))]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    private string? _documentName;

    [ObservableProperty]
    private string? _currentFilePath;

    [ObservableProperty]
    private string _statusText;

    [ObservableProperty]
    private string _notificationText;

    /// <summary>
    /// Gets the window title for the main editor shell.
    /// </summary>
    public string WindowTitle => $"{UiStrings.ApplicationTitle} - {DocumentName ?? _workflow.Document.DisplayName}{(IsDirty ? UiStrings.WindowTitleDirtySuffix : string.Empty)}";

    public event EventHandler? ContentChanging;

    partial void OnTextChanging(string value)
    {
        if (!string.Equals(value, _workflow.Document.Content, StringComparison.Ordinal))
        {
            ContentChanging?.Invoke(this, EventArgs.Empty);
        }
    }

    partial void OnTextChanged(string value)
    {
        _workflow.UpdateText(value);
        SynchronizeFromDocument();
        StatusText = IsDirty ? UiStrings.ModifiedStatusText : UiStrings.ReadyStatusText;
        NotificationText = IsDirty ? UiStrings.DocumentModifiedNotificationText : UiStrings.DocumentSynchronizedNotificationText;
    }

    /// <summary>
    /// Creates a new empty document.
    /// </summary>
    [RelayCommand]
    private void NewDocument()
    {
        var result = _workflow.NewDocumentAsync().GetAwaiter().GetResult();
        Text = _workflow.Document.Content;
        SynchronizeFromDocument();
        ApplyResult(result, UiStrings.NewDocumentStatusText, UiStrings.CreatedNewDocumentNotificationText, UiStrings.NewDocumentStatusText, UiStrings.CreatedNewDocumentNotificationText);
    }

    /// <summary>
    /// Opens an existing text document.
    /// </summary>
    /// <returns>A task that completes when the file was loaded.</returns>
    [RelayCommand]
    private async Task OpenAsync()
    {
        var result = await _workflow.OpenAsync();
        Text = _workflow.Document.Content;
        SynchronizeFromDocument();
        ApplyResult(result, string.Format(UiStrings.OpenedDocumentStatusFormat, _workflow.Document.DisplayName), string.Format(UiStrings.LoadedDocumentNotificationFormat, _workflow.Document.DisplayName), UiStrings.OpenCanceledStatusText, UiStrings.OpenCanceledNotificationText);
    }

    /// <summary>
    /// Opens a known document for a navigation request without showing a file
    /// dialog and synchronizes the editor view model with the loaded state.
    /// </summary>
    /// <param name="filePath">The path of the document to load.</param>
    /// <param name="cancellationToken">Cancels the open operation.</param>
    /// <returns>A task that completes when the document is available.</returns>
    public async Task OpenFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var result = await _workflow.OpenFileAsync(filePath, cancellationToken);
        Text = _workflow.Document.Content;
        SynchronizeFromDocument();
        ApplyResult(
            result,
            string.Format(UiStrings.OpenedDocumentStatusFormat, _workflow.Document.DisplayName),
            string.Format(UiStrings.LoadedDocumentNotificationFormat, _workflow.Document.DisplayName),
            UiStrings.OpenCanceledStatusText,
            UiStrings.OpenCanceledNotificationText);
    }

    public bool CanApplyExternalChange(string fullPath, string expectedContent)
    {
        return CanApplyExternalChange(fullPath, expectedContent, expectedContent);
    }

    public bool CanApplyExternalChange(
        string fullPath,
        string expectedContent,
        string expectedSavedContent)
    {
        ArgumentNullException.ThrowIfNull(expectedContent);
        ArgumentNullException.ThrowIfNull(expectedSavedContent);
        return CurrentFilePath is { } currentPath
            && WorkspacePathIdentity.AreEqual(currentPath, fullPath)
            && string.Equals(_workflow.Document.Content, expectedContent, StringComparison.Ordinal)
            && string.Equals(_workflow.Document.SavedContent, expectedSavedContent, StringComparison.Ordinal);
    }

    public void ApplyExternalChange(string fullPath, string expectedContent, string proposedContent)
    {
        ApplyExternalChange(fullPath, expectedContent, expectedContent, proposedContent, proposedContent);
    }

    public void ApplyExternalChange(
        string fullPath,
        string expectedContent,
        string expectedSavedContent,
        string proposedContent,
        string proposedSavedContent)
    {
        ArgumentNullException.ThrowIfNull(expectedContent);
        ArgumentNullException.ThrowIfNull(expectedSavedContent);
        ArgumentNullException.ThrowIfNull(proposedContent);
        ArgumentNullException.ThrowIfNull(proposedSavedContent);
        if (!CanApplyExternalChange(fullPath, expectedContent, expectedSavedContent))
        {
            throw new IOException("The open document or saved snapshot changed after the change review.");
        }

        string normalizedPath = WorkspacePathIdentity.Normalize(fullPath);
        _workflow.Document.LoadWithSavedContent(normalizedPath, proposedContent, proposedSavedContent);
        Text = proposedContent;
        SynchronizeFromDocument();
    }

    /// <summary>
    /// Saves the current document.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            var result = await _workflow.SaveAsync();
            SynchronizeFromDocument();
            ApplyResult(result, string.Format(UiStrings.SavedDocumentStatusFormat, _workflow.Document.DisplayName), string.Format(UiStrings.SavedDocumentNotificationFormat, _workflow.Document.DisplayName), UiStrings.SaveCanceledStatusText, UiStrings.SaveCanceledNotificationText);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException)
        {
            StatusText = "Save failed.";
            NotificationText = exception.Message;
        }
    }

    /// <summary>
    /// Saves the current document to a new file path.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsAsync()
    {
        try
        {
            var result = await _workflow.SaveAsAsync();
            SynchronizeFromDocument();
            ApplyResult(result, string.Format(UiStrings.SavedDocumentStatusFormat, _workflow.Document.DisplayName), string.Format(UiStrings.SavedDocumentNotificationFormat, _workflow.Document.DisplayName), UiStrings.SaveAsCanceledStatusText, UiStrings.SaveAsCanceledNotificationText);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or InvalidOperationException)
        {
            StatusText = "Save failed.";
            NotificationText = exception.Message;
        }
    }

    private void SynchronizeFromDocument()
    {
        DocumentName = _workflow.Document.DisplayName;
        CurrentFilePath = _workflow.Document.FilePath;
        IsDirty = _workflow.Document.IsDirty;
    }

    private void ApplyResult(
        EditorOperationResult result,
        string completedStatus,
        string completedNotification,
        string canceledStatus,
        string canceledNotification)
    {
        StatusText = result.IsCompleted ? completedStatus : canceledStatus;
        NotificationText = result.IsCompleted ? completedNotification : canceledNotification;
    }

}
