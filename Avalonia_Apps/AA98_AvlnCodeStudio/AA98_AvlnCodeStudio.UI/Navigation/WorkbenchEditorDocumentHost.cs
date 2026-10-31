using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.UI.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>
/// Opens source documents through the workbench's composed single editor.
/// </summary>
public sealed class WorkbenchEditorDocumentHost : IEditorDocumentHost
{
    private readonly Func<EditorDocumentWorkspace> _editorWorkspaceFactory;

    /// <summary>
    /// Initializes a document host for the composed workbench editor.
    /// </summary>
    /// <param name="editorWorkspaceFactory">Creates the editor workspace on first use.</param>
    public WorkbenchEditorDocumentHost(Func<EditorDocumentWorkspace> editorWorkspaceFactory)
    {
        _editorWorkspaceFactory = editorWorkspaceFactory ?? throw new ArgumentNullException(nameof(editorWorkspaceFactory));
    }

    /// <inheritdoc/>
    public async Task<IEditorDocument?> OpenOrActivateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await _editorWorkspaceFactory().OpenOrActivateAsync(filePath, cancellationToken).ConfigureAwait(false);
    }
}
