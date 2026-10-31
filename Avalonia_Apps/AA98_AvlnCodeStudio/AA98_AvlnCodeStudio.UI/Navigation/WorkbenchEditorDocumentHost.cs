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
    private readonly EditorDocumentWorkspace _editorWorkspace;

    /// <summary>
    /// Initializes a document host for the composed workbench editor.
    /// </summary>
    /// <param name="editorWorkspace">The editor workspace.</param>
    public WorkbenchEditorDocumentHost(EditorDocumentWorkspace editorWorkspace)
    {
        _editorWorkspace = editorWorkspace ?? throw new ArgumentNullException(nameof(editorWorkspace));
    }

    /// <inheritdoc/>
    public async Task<IEditorDocument?> OpenOrActivateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await _editorWorkspace.OpenOrActivateAsync(filePath, cancellationToken).ConfigureAwait(false);
    }
}
