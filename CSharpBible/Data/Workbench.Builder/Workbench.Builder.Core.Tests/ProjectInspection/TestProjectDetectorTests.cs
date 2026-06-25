using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectInspection;

/// <summary>
/// Verifies test project classification for the V1.1 inspection slice.
/// </summary>
[TestClass]
public class TestProjectDetectorTests
{
    /// <summary>
    /// Verifies that an evaluated IsTestProject property is honored.
    /// </summary>
    [TestMethod]
    public void IsTestProject_WhenProjectHasIsTestProjectProperty_ReturnsTrue()
    {
        var loader = new MsBuildProjectLoader();
        var detector = new TestProjectDetector();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleTestProjectProjectPath));

        bool isTestProject = detector.IsTestProject(project);

        Assert.IsTrue(isTestProject);
    }

    /// <summary>
    /// Verifies that a plain library project is not treated as a test project.
    /// </summary>
    [TestMethod]
    public void IsTestProject_WhenProjectIsPlainLibrary_ReturnsFalse()
    {
        var loader = new MsBuildProjectLoader();
        var detector = new TestProjectDetector();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));

        bool isTestProject = detector.IsTestProject(project);

        Assert.IsFalse(isTestProject);
    }

    /// <summary>
    /// Verifies that known test package references classify a project as a test project.
    /// </summary>
    [TestMethod]
    public void IsTestProject_WhenProjectReferencesKnownTestPackage_ReturnsTrue()
    {
        var detector = new TestProjectDetector();
        LoadedProjectModel project = CreateProject(
            projectFilePath: @"C:\Projects\Sample\Sample.Library.csproj",
            packageReferences:
            [
                new PackageReferenceInfo("Microsoft.NET.Test.Sdk", "17.12.0", privateAssets: null),
            ]);

        bool isTestProject = detector.IsTestProject(project);

        Assert.IsTrue(isTestProject);
    }

    /// <summary>
    /// Verifies that unrelated package references do not classify a plain library as a test project.
    /// </summary>
    [TestMethod]
    public void IsTestProject_WhenProjectReferencesOnlyNonTestPackages_ReturnsFalse()
    {
        var detector = new TestProjectDetector();
        LoadedProjectModel project = CreateProject(
            projectFilePath: @"C:\Projects\Sample\Sample.Library.csproj",
            packageReferences:
            [
                new PackageReferenceInfo("Newtonsoft.Json", "13.0.3", privateAssets: null),
            ]);

        bool isTestProject = detector.IsTestProject(project);

        Assert.IsFalse(isTestProject);
    }

    /// <summary>
    /// Verifies that common test naming conventions classify a project as a test project.
    /// </summary>
    [TestMethod]
    [DataRow(@"C:\Projects\Sample\Sample.Tests.csproj")]
    [DataRow(@"C:\Projects\Sample\Sample.Test.csproj")]
    public void IsTestProject_WhenProjectNameMatchesTestConvention_ReturnsTrue(string projectFilePath)
    {
        var detector = new TestProjectDetector();
        LoadedProjectModel project = CreateProject(projectFilePath);

        bool isTestProject = detector.IsTestProject(project);

        Assert.IsTrue(isTestProject);
    }

    private static LoadedProjectModel CreateProject(
        string projectFilePath,
        string? isTestProject = null,
        IReadOnlyList<PackageReferenceInfo>? packageReferences = null)
    {
        return new LoadedProjectModel(
            new ProjectLoadRequest(projectFilePath),
            projectFilePath,
            System.IO.Path.GetDirectoryName(projectFilePath)!,
            new ProjectPropertySet(
                assemblyName: System.IO.Path.GetFileNameWithoutExtension(projectFilePath),
                rootNamespace: "Sample",
                targetFramework: "net10.0",
                outputType: "Library",
                langVersion: null,
                nullable: null,
                defineConstants: null,
                implicitUsings: null,
                isPackable: null,
                isTestProject: isTestProject,
                configuration: null,
                runtimeIdentifier: null,
                outputPath: null,
                intermediateOutputPath: null,
                projectAssetsFile: null),
            compileItems: [],
            projectReferences: [],
            packageReferences: packageReferences ?? [],
            isSdkStyle: true,
            diagnostics: []);
    }
}
