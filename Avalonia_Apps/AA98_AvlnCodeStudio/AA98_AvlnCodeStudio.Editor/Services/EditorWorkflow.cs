using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Implements the UI-agnostic workflow for a single text editor component.
/// </summary>
public sealed class EditorWorkflow : IEditorWorkflow
{
    private readonly IFileEditorDocument _document;
    private readonly IEditorFileDialogService _fileDialogService;
    private readonly ITextDocumentStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorWorkflow"/> class.
    /// </summary>
    /// <param name="document">The document state.</param>
    /// <param name="fileDialogService">The file dialog service.</param>
    /// <param name="storageService">The storage service.</param>
    public EditorWorkflow(
        IFileEditorDocument document,
        IEditorFileDialogService fileDialogService,
        ITextDocumentStorageService storageService)
    {
        _document = document;
        _fileDialogService = fileDialogService;
        _storageService = storageService;
    }

    /// <inheritdoc/>
    public IFileEditorDocument Document => _document;

    /// <inheritdoc/>
    public void UpdateText(string? text)
    {
        _document.UpdateContent(text ?? string.Empty);
    }

    /// <inheritdoc/>
    public Task<EditorOperationResult> NewDocumentAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _document.Reset();
        return Task.FromResult(EditorOperationResult.Completed(EditorOperationKind.NewDocument, _document.DisplayName, _document.FilePath));
    }

    /// <inheritdoc/>
    public async Task<EditorOperationResult> OpenAsync(CancellationToken cancellationToken = default)
    {
        var filePath = await _fileDialogService.ShowOpenFileDialogAsync(GetCurrentDirectory(), cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return EditorOperationResult.Canceled(EditorOperationKind.Open, _document.DisplayName, _document.FilePath);
        }

        var content = await _storageService.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
        _document.Load(filePath, content);
        return EditorOperationResult.Completed(EditorOperationKind.Open, _document.DisplayName, _document.FilePath);
    }

    /// <inheritdoc/>
    public async Task<EditorOperationResult> SaveAsync(CancellationToken cancellationToken = default)
    {
        var filePath = _document.FilePath;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = await _fileDialogService.ShowSaveFileDialogAsync(GetCurrentDirectory(), _document.DisplayName, cancellationToken).ConfigureAwait(false);
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return EditorOperationResult.Canceled(EditorOperationKind.Save, _document.DisplayName, _document.FilePath);
        }

        await _storageService.SaveAllTextAsync(filePath, _document.Content, cancellationToken).ConfigureAwait(false);
        _document.MarkSaved(filePath);
        return EditorOperationResult.Completed(EditorOperationKind.Save, _document.DisplayName, _document.FilePath);
    }

    /// <inheritdoc/>
    public async Task<EditorOperationResult> SaveAsAsync(CancellationToken cancellationToken = default)
    {
        var filePath = await _fileDialogService.ShowSaveFileDialogAsync(GetCurrentDirectory(), _document.DisplayName, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return EditorOperationResult.Canceled(EditorOperationKind.SaveAs, _document.DisplayName, _document.FilePath);
        }

        await _storageService.SaveAllTextAsync(filePath, _document.Content, cancellationToken).ConfigureAwait(false);
        _document.MarkSaved(filePath);
        return EditorOperationResult.Completed(EditorOperationKind.SaveAs, _document.DisplayName, _document.FilePath);
    }

    private string? GetCurrentDirectory()
    {
        return string.IsNullOrWhiteSpace(_document.FilePath)
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : Path.GetDirectoryName(_document.FilePath);
    }
}