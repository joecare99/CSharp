using System;
using System.Collections.Generic;
using System.IO;
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
    [TestCleanup]
    public void Cleanup()
    {
        MsBuildProjectLoader.ResetForTests();
    }

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

    /// <summary>
    /// Verifies that loading a null request throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [TestMethod]
    public void Load_NullRequest_ThrowsArgumentNullException()
    {
        MsBuildProjectLoader loader = new();

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() => loader.Load(null!));

        Assert.AreEqual("request", exception.ParamName);
    }

    /// <summary>
    /// Verifies that loading a request without a project file path throws <see cref="ArgumentException"/>.
    /// </summary>
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Load_MissingProjectFilePath_ThrowsArgumentException(string? projectFilePath)
    {
        MsBuildProjectLoader loader = new();
        ProjectLoadRequest request = new(projectFilePath!);

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() => loader.Load(request));

        Assert.AreEqual("request", exception.ParamName);
        StringAssert.StartsWith(exception.Message, "A project file path is required.");
    }

    /// <summary>
    /// Verifies that loading a non-existing project file throws <see cref="FileNotFoundException"/>.
    /// </summary>
    [TestMethod]
    public void Load_MissingProjectFile_ThrowsFileNotFoundException()
    {
        MsBuildProjectLoader loader = new();
        string missingProjectPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "MissingProject.csproj");

        FileNotFoundException exception = Assert.ThrowsExactly<FileNotFoundException>(() => loader.Load(new ProjectLoadRequest(missingProjectPath)));

        Assert.AreEqual(Path.GetFullPath(missingProjectPath), exception.FileName);
        Assert.AreEqual("The specified project file could not be found.", exception.Message.Split(Environment.NewLine)[0]);
    }

    /// <summary>
    /// Verifies that loading fails with a clear exception when no SDK path and no Visual Studio instance are available.
    /// </summary>
    [TestMethod]
    public void Load_WhenNoSdkPathAndNoVisualStudioInstanceExist_ThrowsInvalidOperationException()
    {
        MsBuildProjectLoader.ResetForTests();
        MsBuildProjectLoader.IsMsBuildLocatorRegisteredOverride = static () => false;
        MsBuildProjectLoader.FindDotNetSdkPathOverride = static () => null;
        MsBuildProjectLoader.QueryVisualStudioInstancesOverride = static () => Array.Empty<Microsoft.Build.Locator.VisualStudioInstance>();
        MsBuildProjectLoader loader = new();

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath)));

        Assert.AreEqual("No MSBuild instance or .NET SDK path could be detected for registration.", exception.Message);
    }
}
