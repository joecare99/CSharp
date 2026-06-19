using Workbench.Builder.Core.Models.Loading;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Detects whether a loaded project should be treated as a test project.
/// </summary>
public interface ITestProjectDetector
{
    /// <summary>
    /// Determines whether the specified loaded project is a test project.
    /// </summary>
    /// <param name="project">The loaded project model.</param>
    /// <returns><see langword="true"/> when the project should be treated as a test project; otherwise <see langword="false"/>.</returns>
    bool IsTestProject(LoadedProjectModel project);
}
