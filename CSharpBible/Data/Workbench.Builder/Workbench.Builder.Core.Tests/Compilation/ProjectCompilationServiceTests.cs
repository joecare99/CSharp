using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Compilation;
using Workbench.Builder.Core.Services.Inspection;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.Compilation;

/// <summary>
/// Verifies first-slice V1.2 compilation and emit behavior for supported sample projects.
/// </summary>
[TestClass]
public class ProjectCompilationServiceTests
{
    /// <summary>
    /// Verifies that a simple library project emits an assembly and Portable PDB.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectIsSimpleLibrary_EmitsAssemblyAndPortablePdb()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.AreEqual(ProjectEmitKind.Library, compilationResult.EmitSupport.EmitKind);
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.DebugSymbols && artifact.Exists));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that a simple console project emits an executable assembly and Portable PDB.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectIsSimpleConsoleApp_EmitsExecutableAssemblyAndPortablePdb()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.AreEqual(ProjectEmitKind.Executable, compilationResult.EmitSupport.EmitKind);
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.DebugSymbols && artifact.Exists));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that Portable PDB emission can be disabled explicitly for the first V1.2 slice.
    /// </summary>
    [TestMethod]
    public void Compile_WhenPortablePdbIsDisabled_DoesNotReportDebugSymbolArtifact()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: false));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.AreEqual(1, compilationResult.Artifacts.Count);
            Assert.AreEqual(CompilationArtifactKind.PrimaryOutput, compilationResult.Artifacts[0].Kind);
            Assert.IsFalse(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.DebugSymbols));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that a broken console project returns location-rich compiler diagnostics for the faulty source line.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectHasSourceError_ReturnsLocationRichDiagnostics()
    {
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.BrokenConsoleAppProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsFalse(compilationResult.Succeeded);
            Assert.AreEqual(ProjectEmitKind.Executable, compilationResult.EmitSupport.EmitKind);

            BuildDiagnostic errorDiagnostic = compilationResult.Diagnostics.First(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error);
            StringAssert.EndsWith(errorDiagnostic.FilePath, "Program.cs");
            Assert.AreEqual(4, errorDiagnostic.Line);
            Assert.IsTrue(errorDiagnostic.Column >= 19);
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that test projects remain non-emit and surface a diagnostic instead of producing artifacts.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectIsTestProject_ReturnsNonEmitResult()
    {
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleTestProjectProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsFalse(compilationResult.Succeeded);
            Assert.AreEqual(ProjectEmitKind.None, compilationResult.EmitSupport.EmitKind);
            Assert.AreEqual(0, compilationResult.Artifacts.Count);
            Assert.IsTrue(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Code == "WB2001"));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    private static string CreateOutputDirectory()
    {
        string outputDirectory = Path.Combine(Path.GetTempPath(), "Workbench.Builder.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);
        return outputDirectory;
    }

    private static void DeleteOutputDirectory(string outputDirectory)
    {
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, recursive: true);
        }
    }
}
