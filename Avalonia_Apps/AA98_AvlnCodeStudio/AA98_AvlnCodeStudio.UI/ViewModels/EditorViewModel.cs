using AA98_AvlnCodeStudio.Base.Services;
using AA98_AvlnCodeStudio.Base.ViewModels;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.Resources;
using AA98_AvlnCodeStudio.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Provides the main editor workflow for the first Code Studio prototype.
/// </summary>
public partial class EditorViewModel : CodeStudioViewModelBase
{
    private readonly FileEditorDocument _document;
    private readonly IEditorFileDialogService _fileDialogService;
    private readonly ITextDocumentStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModel"/> class.
    /// </summary>
    /// <param name="document">The document state model.</param>
    /// <param name="fileDialogService">The dialog service.</param>
    /// <param name="storageService">The persistence service.</param>
    public EditorViewModel(
        FileEditorDocument document,
        IEditorFileDialogService fileDialogService,
        ITextDocumentStorageService storageService)
    {
        _document = document;
        _fileDialogService = fileDialogService;
        _storageService = storageService;

        _text = _document.Content;
        _statusText = UiStrings.ReadyStatusText;
        _notificationText = UiStrings.ReadyStatusText;
        SynchronizeFromDocument();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModel"/> class for design-time usage.
    /// </summary>
    public EditorViewModel()
        : this(new FileEditorDocument(), new DesignEditorFileDialogService(), new DesignTextDocumentStorageService())
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
    public string WindowTitle => $"{UiStrings.ApplicationTitle} - {DocumentName ?? _document.DisplayName}{(IsDirty ? UiStrings.WindowTitleDirtySuffix : string.Empty)}";

    partial void OnTextChanged(string value)
    {
        _document.UpdateContent(value);
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
        _document.Reset();
        Text = _document.Content;
        SynchronizeFromDocument();
        StatusText = UiStrings.NewDocumentStatusText;
        NotificationText = UiStrings.CreatedNewDocumentNotificationText;
    }

    /// <summary>
    /// Opens an existing text document.
    /// </summary>
    /// <returns>A task that completes when the file was loaded.</returns>
    [RelayCommand]
    private async Task OpenAsync()
    {
        var filePath = await _fileDialogService
            .ShowOpenFileDialogAsync(GetCurrentDirectory());

        if (string.IsNullOrWhiteSpace(filePath))
        {
            StatusText = UiStrings.OpenCanceledStatusText;
            NotificationText = UiStrings.OpenCanceledNotificationText;
            return;
        }

        var content = await _storageService.ReadAllTextAsync(filePath);
        _document.Load(filePath, content);

        Text = _document.Content;
        SynchronizeFromDocument();
        StatusText = string.Format(UiStrings.OpenedDocumentStatusFormat, _document.DisplayName);
        NotificationText = string.Format(UiStrings.LoadedDocumentNotificationFormat, _document.DisplayName);
    }

    /// <summary>
    /// Saves the current document.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsync()
    {
        var filePath = _document.FilePath;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = await _fileDialogService
                .ShowSaveFileDialogAsync(GetCurrentDirectory(), _document.DisplayName);
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            StatusText = UiStrings.SaveCanceledStatusText;
            NotificationText = UiStrings.SaveCanceledNotificationText;
            return;
        }

        await _storageService.SaveAllTextAsync(filePath, Text ?? string.Empty);
        _document.MarkSaved(filePath);
        SynchronizeFromDocument();
        StatusText = string.Format(UiStrings.SavedDocumentStatusFormat, _document.DisplayName);
        NotificationText = string.Format(UiStrings.SavedDocumentNotificationFormat, _document.DisplayName);
    }

    /// <summary>
    /// Saves the current document to a new file path.
    /// </summary>
    /// <returns>A task that completes when the file was persisted.</returns>
    [RelayCommand]
    private async Task SaveAsAsync()
    {
        var filePath = await _fileDialogService
            .ShowSaveFileDialogAsync(GetCurrentDirectory(), _document.DisplayName);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            StatusText = UiStrings.SaveAsCanceledStatusText;
            NotificationText = UiStrings.SaveAsCanceledNotificationText;
            return;
        }

        await _storageService.SaveAllTextAsync(filePath, Text ?? string.Empty);
        _document.MarkSaved(filePath);
        SynchronizeFromDocument();
        StatusText = string.Format(UiStrings.SavedDocumentStatusFormat, _document.DisplayName);
        NotificationText = string.Format(UiStrings.SavedDocumentNotificationFormat, _document.DisplayName);
    }

    private string? GetCurrentDirectory()
    {
        return string.IsNullOrWhiteSpace(_document.FilePath)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : Path.GetDirectoryName(_document.FilePath);
    }

    private void SynchronizeFromDocument()
    {
        DocumentName = _document.DisplayName;
        CurrentFilePath = _document.FilePath;
        IsDirty = _document.IsDirty;
    }

}
