using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectInspection;

/// <summary>
/// Verifies end-to-end V1.1 inspection composition.
/// </summary>
[TestClass]
public class ProjectInspectionServiceTests
{
    /// <summary>
    /// Verifies that a simple console app produces a structured inspection result with resolved references.
    /// </summary>
    [TestMethod]
    public void Inspect_SimpleConsoleProject_ReturnsStructuredInspectionResult()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var service = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());

        var result = service.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        Assert.AreEqual("SimpleConsoleApp", result.Project.AssemblyName);
        Assert.AreEqual("net8.0", result.Project.TargetFramework);
        Assert.AreEqual(2, result.CompileItems.Count);
        Assert.IsFalse(result.IsTestProject);
        Assert.IsTrue(result.ResolvedReferences.Any(reference => reference.Exists));
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Severity == Models.Diagnostics.BuildDiagnosticSeverity.Error));
    }

    /// <summary>
    /// Verifies that the inspection service reports a sample test project as a test project.
    /// </summary>
    [TestMethod]
    public void Inspect_SimpleTestProject_ReturnsIsTestProject()
    {
        var service = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());

        var result = service.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleTestProjectProjectPath));

        Assert.IsTrue(result.IsTestProject);
        Assert.IsTrue(result.PackageReferences.Count >= 2);
    }
}
