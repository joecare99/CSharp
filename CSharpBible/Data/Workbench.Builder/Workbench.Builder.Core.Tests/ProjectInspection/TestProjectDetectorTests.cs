using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
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
}
