using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;

namespace AA98_AvlnCodeStudio.Base.Building.Services;

/// <summary>
/// Builds an executable project and its runtime dependencies before launch.
/// </summary>
public interface IProjectRuntimeBuilder
{
    Task BuildAsync(BuilderBuildRequest request, CancellationToken cancellationToken = default);
}
