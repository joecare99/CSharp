using Workbench.Builder.Core.Models.Loading;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Loads and evaluates a project file into a host-neutral intermediate model.
/// </summary>
public interface IProjectLoader
{
    /// <summary>
    /// Loads the specified project file request.
    /// </summary>
    /// <param name="request">The project load request.</param>
    /// <returns>The loaded project model.</returns>
    LoadedProjectModel Load(ProjectLoadRequest request);
}
