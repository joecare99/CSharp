using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.UI.Controls;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Code.Navigation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>
/// Focuses the Avalonia workbench editor and positions its caret.
/// </summary>
public sealed class WorkbenchEditorCaretService : IEditorCaretService
{
    private readonly Func<EditorDocumentWorkspace> _editorWorkspaceFactory;

    /// <summary>
    /// Initializes a caret service for the composed workbench editor.
    /// </summary>
    /// <param name="editorWorkspaceFactory">Creates the editor workspace on first use.</param>
    public WorkbenchEditorCaretService(Func<EditorDocumentWorkspace> editorWorkspaceFactory)
    {
        _editorWorkspaceFactory = editorWorkspaceFactory ?? throw new ArgumentNullException(nameof(editorWorkspaceFactory));
    }

    /// <inheritdoc/>
    public Task FocusAndMoveCaretAsync(
        IEditorDocument document,
        CodeLocation location,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(location);
        cancellationToken.ThrowIfCancellationRequested();

        EditorDocumentWorkspace editorWorkspace = _editorWorkspaceFactory();
        EditorTabViewModel? tab = editorWorkspace.FindByPath(document.FilePath);
        if (tab is null)
        {
            throw new InvalidOperationException($"The document '{document.FilePath}' is not open in the workbench.");
        }

        editorWorkspace.Activate(tab);
        if (tab.View is not EditorTextArea editor)
        {
            throw new InvalidOperationException("The workbench editor does not provide a caret-capable text area.");
        }

        editor.FocusAndMoveCaret(location);
        return Task.CompletedTask;
    }
}
