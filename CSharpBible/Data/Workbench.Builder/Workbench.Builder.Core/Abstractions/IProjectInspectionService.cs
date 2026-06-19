using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Produces the stable V1.1 project inspection result used by hosts and later Workbench integration.
/// </summary>
public interface IProjectInspectionService
{
    /// <summary>
    /// Inspects the specified project request.
    /// </summary>
    /// <param name="request">The project load request.</param>
    /// <returns>The structured project inspection result.</returns>
    ProjectInspectionResult Inspect(ProjectLoadRequest request);
}
