using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Adds or removes NuGet package references from C# project files.
/// </summary>
public interface ICSharpProjectPackageService
{
    Task<IReadOnlyList<CSharpProjectPackageReference>> ListPackagesAsync(
        string projectPath,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NuGetPackageSearchResult>> SearchPackagesAsync(
        string searchText,
        int take = 20,
        CancellationToken cancellationToken = default);

    Task<string> AddPackageAsync(
        string projectPath,
        string packageId,
        string? version = null,
        CancellationToken cancellationToken = default);

    Task<string> RemovePackageAsync(
        string projectPath,
        string packageId,
        CancellationToken cancellationToken = default);
}
