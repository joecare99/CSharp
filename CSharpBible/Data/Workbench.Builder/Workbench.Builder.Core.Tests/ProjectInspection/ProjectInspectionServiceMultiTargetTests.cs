using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectInspection;

/// <summary>
/// Verifies end-to-end V1.1 inspection behavior for the multi-target sample project.
/// </summary>
[TestClass]
public class ProjectInspectionServiceMultiTargetTests
{
    /// <summary>
    /// Verifies that the inspection service exposes the default first-target behavior for a multi-target project.
    /// </summary>
    [TestMethod]
    public void Inspect_MultiTargetProjectWithoutExplicitTargetFramework_ExposesCurrentTargetFrameworkGap()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.MultiTargetLibraryProjectPath);
        ProjectInspectionService service = new(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());

        var result = service.Inspect(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath));

        Assert.AreEqual("MultiTargetLibrary", result.Project.AssemblyName);
        Assert.AreEqual(string.Empty, result.Project.TargetFramework);
        Assert.AreEqual(0, result.CompileItems.Count);
        Assert.IsFalse(result.IsTestProject);
        Assert.IsTrue(result.Diagnostics.Any(diagnostic => diagnostic.Code == "WB1002" && diagnostic.Severity == BuildDiagnosticSeverity.Warning));
        Assert.IsTrue(result.Diagnostics.Any(diagnostic => diagnostic.Code == "WB1103" || diagnostic.Code == "WB1102") || result.ResolvedReferences.Count == 0);
        Assert.IsFalse(result.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
    }

    /// <summary>
    /// Verifies that the inspection service can inspect an explicitly requested target framework for the multi-target project.
    /// </summary>
    [TestMethod]
    public void Inspect_MultiTargetProjectWithExplicitTargetFramework_ReturnsRequestedTargetFrameworkResult()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.MultiTargetLibraryProjectPath);
        ProjectInspectionService service = new(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());

        var result = service.Inspect(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetLibraryProjectPath, targetFramework: "net10.0"));

        Assert.AreEqual("net10.0", result.Project.TargetFramework);
        Assert.AreEqual(1, result.CompileItems.Count);
        Assert.IsTrue(result.ResolvedReferences.Any(reference => reference.Exists));
    }
}
