using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectLoading;

/// <summary>
/// Verifies MSBuild-based loading of SDK-style sample projects.
/// </summary>
[TestClass]
public class MsBuildProjectLoaderTests
{
    /// <summary>
    /// Verifies that loading a simple console project evaluates the planned V1.1 core properties and compile items.
    /// </summary>
    [TestMethod]
    public void Load_SimpleConsoleProject_ReturnsExpectedProperties()
    {
        var loader = new MsBuildProjectLoader();

        var project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        Assert.AreEqual("SimpleConsoleApp", project.Properties.AssemblyName);
        Assert.AreEqual("Workbench.Builder.TestData.SimpleConsoleApp", project.Properties.RootNamespace);
        Assert.AreEqual("net8.0", project.Properties.TargetFramework);
        Assert.AreEqual("Exe", project.Properties.OutputType);
        Assert.AreEqual("enable", project.Properties.ImplicitUsings);
        Assert.AreEqual(2, project.CompileItems.Count);
        Assert.AreEqual(0, project.PackageReferences.Count);
        Assert.IsTrue(project.IsSdkStyle);
    }

    /// <summary>
    /// Verifies that loading a project with a project reference returns the referenced project path.
    /// </summary>
    [TestMethod]
    public void Load_ProjectWithProjectReference_ReturnsReferencedProject()
    {
        var loader = new MsBuildProjectLoader();

        var project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        Assert.AreEqual(1, project.ProjectReferences.Count);
        StringAssert.EndsWith(project.ProjectReferences[0].ProjectFilePath, "ReferencedLibrary.csproj");
        Assert.IsTrue(project.ProjectReferences[0].Exists);
    }
}
