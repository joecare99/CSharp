using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.References;

/// <summary>
/// Verifies pragmatic V1.1 reference resolution for SDK-style sample projects.
/// </summary>
[TestClass]
public class ReferenceResolverTests
{
    /// <summary>
    /// Verifies that framework references are resolved for a simple console project after restore.
    /// </summary>
    [TestMethod]
    public void Resolve_SimpleConsoleProject_ReturnsFrameworkReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        var references = resolver.Resolve(project);

        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }

    /// <summary>
    /// Verifies that project references are included in the resolved reference list.
    /// </summary>
    [TestMethod]
    public void Resolve_ProjectWithProjectReference_ReturnsProjectReference()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        var references = resolver.Resolve(project);

        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Project && reference.Exists));
        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }
}
