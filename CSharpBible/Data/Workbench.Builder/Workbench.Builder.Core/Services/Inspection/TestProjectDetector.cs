using System;
using System.Collections.Generic;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Loading;

namespace Workbench.Builder.Core.Services.Inspection;

/// <summary>
/// Detects whether a loaded project should be treated as a test project by combining evaluated properties,
/// package references, and common naming conventions.
/// </summary>
public sealed class TestProjectDetector : ITestProjectDetector
{
    private static readonly HashSet<string> KnownTestPackages = new(StringComparer.OrdinalIgnoreCase)
    {
        "MSTest",
        "MSTest.TestAdapter",
        "MSTest.TestFramework",
        "Microsoft.NET.Test.Sdk",
        "NUnit",
        "NUnit3TestAdapter",
        "xunit",
        "xunit.runner.visualstudio",
    };

    /// <inheritdoc/>
    public bool IsTestProject(LoadedProjectModel project)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        if (bool.TryParse(project.Properties.IsTestProject, out bool isTestProject))
        {
            return isTestProject;
        }

        foreach (Models.Projects.PackageReferenceInfo packageReference in project.PackageReferences)
        {
            if (KnownTestPackages.Contains(packageReference.PackageId))
            {
                return true;
            }
        }

        string projectFileName = System.IO.Path.GetFileNameWithoutExtension(project.ProjectFilePath);
        if (projectFileName.EndsWith(".Tests", StringComparison.OrdinalIgnoreCase) ||
            projectFileName.EndsWith("Tests", StringComparison.OrdinalIgnoreCase) ||
            projectFileName.EndsWith(".Test", StringComparison.OrdinalIgnoreCase) ||
            projectFileName.EndsWith("Test", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
