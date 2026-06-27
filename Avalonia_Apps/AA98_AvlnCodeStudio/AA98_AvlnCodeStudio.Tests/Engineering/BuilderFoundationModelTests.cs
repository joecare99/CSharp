using AA98_AvlnCodeStudio.Base.Building.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies baseline defaults for the shared builder foundation models.
/// </summary>
[TestClass]
public class BuilderFoundationModelTests
{
    /// <summary>
    /// Verifies that a builder project inspection request starts with empty paths.
    /// </summary>
    [TestMethod]
    public void BuilderProjectInspectionRequest_UsesExpectedDefaults()
    {
        var request = new BuilderProjectInspectionRequest();

        Assert.AreEqual(string.Empty, request.WorkspaceRootPath);
        Assert.AreEqual(string.Empty, request.ProjectPath);
        Assert.IsNull(request.Configuration);
        Assert.IsNull(request.TargetFramework);
    }

    /// <summary>
    /// Verifies that a builder project inspection result starts with empty collections.
    /// </summary>
    [TestMethod]
    public void BuilderProjectInspectionResult_UsesExpectedDefaults()
    {
        var result = new BuilderProjectInspectionResult();

        Assert.AreEqual(string.Empty, result.ProjectPath);
        Assert.IsNull(result.ProjectName);
        Assert.IsNull(result.TargetFramework);
        Assert.IsFalse(result.IsTestProject);
        Assert.AreEqual(0, result.CompileItems.Count);
        Assert.AreEqual(0, result.ProjectReferences.Count);
        Assert.AreEqual(0, result.PackageReferences.Count);
        Assert.AreEqual(0, result.ResolvedReferences.Count);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    /// <summary>
    /// Verifies that a builder build request starts with empty paths and optional settings.
    /// </summary>
    [TestMethod]
    public void BuilderBuildRequest_UsesExpectedDefaults()
    {
        var request = new BuilderBuildRequest();

        Assert.AreEqual(string.Empty, request.WorkspaceRootPath);
        Assert.AreEqual(string.Empty, request.ProjectPath);
        Assert.IsNull(request.Configuration);
        Assert.IsNull(request.TargetFramework);
    }

    /// <summary>
    /// Verifies that a builder build result starts as unsuccessful and without output.
    /// </summary>
    [TestMethod]
    public void BuilderBuildResult_UsesExpectedDefaults()
    {
        var result = new BuilderBuildResult();

        Assert.AreEqual(string.Empty, result.ProjectPath);
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(0, result.Artifacts.Count);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    /// <summary>
    /// Verifies that a targeted test request starts with empty paths and no targets.
    /// </summary>
    [TestMethod]
    public void BuilderTargetedTestRequest_UsesExpectedDefaults()
    {
        var request = new BuilderTargetedTestRequest();

        Assert.AreEqual(string.Empty, request.WorkspaceRootPath);
        Assert.AreEqual(string.Empty, request.ProjectPath);
        Assert.IsNull(request.TargetFramework);
        Assert.AreEqual(0, request.Targets.Count);
    }

    /// <summary>
    /// Verifies that a targeted test result starts with an unknown outcome and zero counters.
    /// </summary>
    [TestMethod]
    public void BuilderTargetedTestResult_UsesExpectedDefaults()
    {
        var result = new BuilderTargetedTestResult();

        Assert.AreEqual(string.Empty, result.ProjectPath);
        Assert.AreEqual(BuilderTargetedTestOutcome.Unknown, result.Outcome);
        Assert.AreEqual(0, result.TotalCount);
        Assert.AreEqual(0, result.PassedCount);
        Assert.AreEqual(0, result.FailedCount);
        Assert.AreEqual(0, result.SkippedCount);
        Assert.AreEqual(0, result.Diagnostics.Count);
    }

    /// <summary>
    /// Verifies that a builder diagnostic starts with empty text and unknown severity.
    /// </summary>
    [TestMethod]
    public void BuilderDiagnostic_UsesExpectedDefaults()
    {
        var diagnostic = new BuilderDiagnostic();

        Assert.AreEqual(string.Empty, diagnostic.Code);
        Assert.AreEqual(string.Empty, diagnostic.Message);
        Assert.AreEqual(BuilderDiagnosticSeverity.Unknown, diagnostic.Severity);
        Assert.IsNull(diagnostic.FilePath);
        Assert.IsNull(diagnostic.LineNumber);
        Assert.IsNull(diagnostic.ColumnNumber);
    }

    /// <summary>
    /// Verifies that a builder reference descriptor starts with unknown kind and empty name.
    /// </summary>
    [TestMethod]
    public void BuilderReferenceDescriptor_UsesExpectedDefaults()
    {
        var descriptor = new BuilderReferenceDescriptor();

        Assert.AreEqual(BuilderReferenceKind.Unknown, descriptor.Kind);
        Assert.AreEqual(string.Empty, descriptor.Name);
        Assert.IsNull(descriptor.Identity);
        Assert.IsNull(descriptor.Version);
        Assert.IsNull(descriptor.Path);
    }

    /// <summary>
    /// Verifies that a build artifact starts with empty values.
    /// </summary>
    [TestMethod]
    public void BuilderCompilationArtifact_UsesExpectedDefaults()
    {
        var artifact = new BuilderCompilationArtifact();

        Assert.AreEqual(string.Empty, artifact.Kind);
        Assert.AreEqual(string.Empty, artifact.Path);
        Assert.IsNull(artifact.TargetFramework);
    }
}
