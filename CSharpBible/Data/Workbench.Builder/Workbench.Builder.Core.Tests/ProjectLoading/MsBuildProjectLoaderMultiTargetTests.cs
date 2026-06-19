using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectLoading;

/// <summary>
/// Verifies visible multi-target loading behavior for SDK-style sample projects.
/// </summary>
[TestClass]
public class MsBuildProjectLoaderMultiTargetTests
{
    /// <summary>
    /// Verifies that a multi-target project defaults to the first declared target framework when no explicit target is requested.
    /// </summary>
    [TestMethod]
    public void Load_MultiTargetProjectWithoutExplicitTargetFramework_LeavesTargetFrameworkVisibleAsKnownGap()
    {
        MsBuildProjectLoader loader = new();

        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath));

        Assert.AreEqual("MultiTargetLibrary", project.Properties.AssemblyName);
        Assert.AreEqual("Workbench.Builder.TestData.MultiTargetLibrary", project.Properties.RootNamespace);
        Assert.AreEqual(string.Empty, project.Properties.TargetFramework);
        Assert.AreEqual(0, project.CompileItems.Count);
        Assert.IsTrue(project.IsSdkStyle);
    }

    /// <summary>
    /// Verifies that an explicitly requested target framework is honored for a multi-target project.
    /// </summary>
    [TestMethod]
    public void Load_MultiTargetProjectWithExplicitTargetFramework_UsesRequestedTargetFramework()
    {
        MsBuildProjectLoader loader = new();

        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath, targetFramework: "net10.0"));

        Assert.AreEqual("net10.0", project.Properties.TargetFramework);
        Assert.AreEqual(1, project.CompileItems.Count);
        Assert.AreEqual("SharedValue.cs", project.CompileItems[0].Include);
    }
}
