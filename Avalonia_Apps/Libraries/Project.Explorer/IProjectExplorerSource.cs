using System.Threading;
using System.Threading.Tasks;

namespace Project.Explorer;

/// <summary>Loads a refreshed hierarchy from a provider-specific project source.</summary>
public interface IProjectExplorerSource
{
    /// <summary>Loads the current hierarchy for the supplied root path.</summary>
    Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default);
}
