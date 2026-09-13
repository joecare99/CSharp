using System.Threading;
using System.Threading.Tasks;

namespace Project.Explorer;

/// <summary>Provides host-specific activation for an available file item.</summary>
public interface IProjectExplorerItemOpener
{
    /// <summary>Opens the supplied file item in the host's document capability.</summary>
    Task OpenAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default);
}
