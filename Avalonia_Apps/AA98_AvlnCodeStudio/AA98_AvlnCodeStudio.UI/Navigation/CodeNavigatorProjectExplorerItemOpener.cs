using Code.Navigation;
using Project.Explorer;
using System;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.UI.ViewModels;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>Adapts shared explorer file activation to the CodeStudio navigation capability.</summary>
public sealed class CodeNavigatorProjectExplorerItemOpener : IProjectExplorerPreviewOpener
{
    private readonly ICodeNavigator _codeNavigator;
    private readonly Func<EditorDocumentWorkspace>? _editorWorkspace;

    /// <summary>Initializes the opener with the host's registered code navigator.</summary>
    public CodeNavigatorProjectExplorerItemOpener(
        ICodeNavigator codeNavigator,
        Func<EditorDocumentWorkspace>? editorWorkspace = null)
    {
        _codeNavigator = codeNavigator ?? throw new ArgumentNullException(nameof(codeNavigator));
        _editorWorkspace = editorWorkspace;
    }

    /// <inheritdoc />
    public Task OpenAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (item.Kind != ProjectExplorerItemKind.File || !item.IsAvailable)
        {
            throw new ArgumentException("Only available file items can be opened.", nameof(item));
        }

        return _codeNavigator.NavigateAsync(new CodeLocation(item.Path), cancellationToken);
    }

    public async Task PreviewAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_editorWorkspace is null)
        {
            await OpenAsync(item, cancellationToken);
            return;
        }

        await _editorWorkspace().OpenPreviewAsync(item.Path, cancellationToken);
    }

    public async Task ActivateAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_editorWorkspace is not null
            && await _editorWorkspace().OpenOrActivateAsync(item.Path, cancellationToken) is null)
        {
            return;
        }

        await OpenAsync(item, cancellationToken);
    }
}
