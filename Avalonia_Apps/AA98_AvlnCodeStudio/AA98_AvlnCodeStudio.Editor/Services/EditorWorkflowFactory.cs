using AA98_AvlnCodeStudio.Base.OS.Services;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Model.Documents;

namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Creates default editor workflow instances.
/// </summary>
public sealed class EditorWorkflowFactory : IEditorWorkflowFactory
{
    private readonly IFileEditorDocumentFactory _documentFactory;
    private readonly IEditorFileDialogService _fileDialogService;
    private readonly ITextDocumentStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorWorkflowFactory"/> class.
    /// </summary>
    /// <param name="documentFactory">The document factory.</param>
    /// <param name="fileDialogService">The file dialog service.</param>
    /// <param name="storageService">The storage service.</param>
    public EditorWorkflowFactory(
        IFileEditorDocumentFactory documentFactory,
        IEditorFileDialogService fileDialogService,
        ITextDocumentStorageService storageService)
    {
        _documentFactory = documentFactory;
        _fileDialogService = fileDialogService;
        _storageService = storageService;
    }

    /// <inheritdoc/>
    public IEditorWorkflow Create()
    {
        return new EditorWorkflow(_documentFactory.Create(), _fileDialogService, _storageService);
    }
}