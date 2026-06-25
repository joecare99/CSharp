using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.References;

/// <summary>
/// Verifies visible reference-resolution behavior for the multi-target sample project.
/// </summary>
[TestClass]
public class ReferenceResolverMultiTargetTests
{
    /// <summary>
    /// Verifies that the highest builder-supported target framework resolves framework references for the multi-target sample.
    /// </summary>
    [TestMethod]
    public void Resolve_MultiTargetProjectWithoutExplicitTargetFramework_ReturnsFrameworkReferencesForHighestSupportedTargetFramework()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.MultiTargetLibraryProjectPath);
        MsBuildProjectLoader loader = new();
        ReferenceResolver resolver = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath));

        var references = resolver.Resolve(project);

        Assert.AreEqual("net10.0", project.Properties.TargetFramework);
        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }

    /// <summary>
    /// Verifies that an explicitly requested target framework still resolves framework references for the multi-target sample.
    /// </summary>
    [TestMethod]
    public void Resolve_MultiTargetProjectWithExplicitTargetFramework_ReturnsFrameworkReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.MultiTargetLibraryProjectPath);
        MsBuildProjectLoader loader = new();
        ReferenceResolver resolver = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath, targetFramework: "net10.0"));

        var references = resolver.Resolve(project);

        Assert.AreEqual("net10.0", project.Properties.TargetFramework);
        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }
}
