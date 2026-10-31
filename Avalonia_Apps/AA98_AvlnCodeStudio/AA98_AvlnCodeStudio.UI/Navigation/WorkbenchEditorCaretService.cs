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
    private readonly EditorDocumentWorkspace _editorWorkspace;

    /// <summary>
    /// Initializes a caret service for the composed workbench editor.
    /// </summary>
    /// <param name="editorWorkspace">The editor workspace.</param>
    public WorkbenchEditorCaretService(EditorDocumentWorkspace editorWorkspace)
    {
        _editorWorkspace = editorWorkspace ?? throw new ArgumentNullException(nameof(editorWorkspace));
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

        EditorTabViewModel? tab = _editorWorkspace.FindByPath(document.FilePath);
        if (tab is null)
        {
            throw new InvalidOperationException($"The document '{document.FilePath}' is not open in the workbench.");
        }

        _editorWorkspace.Activate(tab);
        if (tab.View is not EditorTextArea editor)
        {
            throw new InvalidOperationException("The workbench editor does not provide a caret-capable text area.");
        }

        editor.FocusAndMoveCaret(location);
        return Task.CompletedTask;
    }
}
