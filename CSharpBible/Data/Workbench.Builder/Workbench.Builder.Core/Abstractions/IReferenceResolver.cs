using System.Collections.Generic;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Abstractions;

/// <summary>
/// Resolves framework, metadata, package, and project references for a loaded project.
/// </summary>
public interface IReferenceResolver
{
    /// <summary>
    /// Resolves the references for the specified loaded project.
    /// </summary>
    /// <param name="project">The loaded project model.</param>
    /// <returns>The resolved reference list.</returns>
    IReadOnlyList<ResolvedReferenceInfo> Resolve(LoadedProjectModel project);
}
