using AA98_AvlnCodeStudio.Base.ViewModels;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    /// Saves the current document.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsync()
    {
        var result = await _workflow.SaveAsync();
        SynchronizeFromDocument();
        ApplyResult(result, string.Format(UiStrings.SavedDocumentStatusFormat, _workflow.Document.DisplayName), string.Format(UiStrings.SavedDocumentNotificationFormat, _workflow.Document.DisplayName), UiStrings.SaveCanceledStatusText, UiStrings.SaveCanceledNotificationText);
    }

    /// <summary>
    /// Saves the current document to a new file path.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsAsync()
    {
        var result = await _workflow.SaveAsAsync();
        SynchronizeFromDocument();
        ApplyResult(result, string.Format(UiStrings.SavedDocumentStatusFormat, _workflow.Document.DisplayName), string.Format(UiStrings.SavedDocumentNotificationFormat, _workflow.Document.DisplayName), UiStrings.SaveAsCanceledStatusText, UiStrings.SaveAsCanceledNotificationText);
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
