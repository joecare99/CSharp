using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
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
    /// Verifies that inspection rejects a null request.
    /// </summary>
    [TestMethod]
    public void Inspect_NullRequest_ThrowsArgumentNullException()
    {
        ProjectInspectionService service = new(
            Substitute.For<IProjectLoader>(),
            Substitute.For<IReferenceResolver>(),
            Substitute.For<ITestProjectDetector>());

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() => service.Inspect(null!));

        Assert.AreEqual("request", exception.ParamName);
    }

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
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
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

    /// <summary>
    /// Verifies that a non-SDK-style sample produces WB1001.
    /// </summary>
    [TestMethod]
    public void Inspect_NonSdkStyleProject_ReportsWarningWb1001()
    {
        ProjectInspectionResult result = Inspect(TestDataProjectPaths.NonSdkStyleProjectPath);

        AssertDiagnostic(result, "WB1001");
    }

    /// <summary>
    /// Verifies that a sample without TargetFramework produces WB1002.
    /// </summary>
    [TestMethod]
    public void Inspect_MissingTargetFrameworkProject_ReportsWarningWb1002()
    {
        ProjectInspectionResult result = Inspect(TestDataProjectPaths.MissingTargetFrameworkProjectPath);

        AssertDiagnostic(result, "WB1002");
    }

    /// <summary>
    /// Verifies that a sample with a missing project reference produces WB1101.
    /// </summary>
    [TestMethod]
    public void Inspect_MissingProjectReferenceProject_ReportsWarningWb1101()
    {
        ProjectInspectionResult result = Inspect(TestDataProjectPaths.MissingProjectReferenceProjectPath);

        AssertDiagnostic(result, "WB1101");
    }

    /// <summary>
    /// Verifies that a sample with a missing compile item produces WB1102.
    /// </summary>
    [TestMethod]
    public void Inspect_MissingCompileItemProject_ReportsWarningWb1102()
    {
        ProjectInspectionResult result = Inspect(TestDataProjectPaths.MissingCompileItemProjectPath);

        AssertDiagnostic(result, "WB1102");
    }

    /// <summary>
    /// Verifies that a sample with a missing metadata reference produces WB1103.
    /// </summary>
    [TestMethod]
    public void Inspect_MissingMetadataReferenceProject_ReportsWarningWb1103()
    {
        ProjectInspectionResult result = Inspect(TestDataProjectPaths.MissingMetadataReferenceProjectPath);

        AssertDiagnostic(result, "WB1103");
    }

    private static ProjectInspectionResult Inspect(string projectPath)
    {
        ProjectInspectionService service = new(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        return service.Inspect(new ProjectLoadRequest(projectPath));
    }

    private static void AssertDiagnostic(ProjectInspectionResult result, string diagnosticCode)
    {
        BuildDiagnostic? diagnostic = result.Diagnostics.FirstOrDefault(candidate => candidate.Code == diagnosticCode);
        Assert.IsNotNull(diagnostic, $"Expected diagnostic '{diagnosticCode}' but found: {string.Join(", ", result.Diagnostics.Select(candidate => candidate.Code))}");
        Assert.AreEqual(BuildDiagnosticSeverity.Warning, diagnostic.Severity);
        StringAssert.EndsWith(diagnostic.FilePath, ".csproj");
    }
}
