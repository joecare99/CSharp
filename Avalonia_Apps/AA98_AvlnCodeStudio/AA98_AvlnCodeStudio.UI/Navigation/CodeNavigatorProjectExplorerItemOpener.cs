using Code.Navigation;
using Project.Explorer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>Adapts shared explorer file activation to the CodeStudio navigation capability.</summary>
public sealed class CodeNavigatorProjectExplorerItemOpener : IProjectExplorerItemOpener
{
    private readonly ICodeNavigator _codeNavigator;

    /// <summary>Initializes the opener with the host's registered code navigator.</summary>
    public CodeNavigatorProjectExplorerItemOpener(ICodeNavigator codeNavigator)
    {
        _codeNavigator = codeNavigator ?? throw new ArgumentNullException(nameof(codeNavigator));
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
}
