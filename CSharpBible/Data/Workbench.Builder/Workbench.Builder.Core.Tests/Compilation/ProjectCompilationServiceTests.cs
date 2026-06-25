using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
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

            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
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
    /// Verifies that executable runtime artifacts can be prepared when compilation emits directly into the effective project output directory.
    /// </summary>
    [TestMethod]
    public void Compile_WhenExecutableUsesEffectiveProjectOutputDirectory_DoesNotFailWithFileLock()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());

        ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));
        string outputDirectory = ResolveProjectOutputDirectory(inspectionResult.Project);

        try
        {
            DeleteOutputDirectory(outputDirectory);

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory: null, emitPortablePdb: true));

            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.RuntimeMetadata && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.RuntimeHost && artifact.Exists));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that SDK implicit usings are available during builder compilation.
    /// </summary>
    [TestMethod]
    public void Compile_WhenSdkImplicitUsingsAreEnabled_ProvidesCommonGlobalUsings()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string tempProjectDirectory = CreateOutputDirectory();
        string sourceFilePath = Path.Combine(tempProjectDirectory, "Program.cs");

        try
        {
            File.WriteAllText(
                sourceFilePath,
                "namespace Sample; public static class Program { public static Task Main(string[] args) { Stream stream = Stream.Null; Exception? ex = null; _ = StringComparison.OrdinalIgnoreCase; _ = Environment.CurrentDirectory; _ = File.Exists(args.Length > 0 ? args[0] : string.Empty); return Task.CompletedTask; } }");

            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));
            ProjectInspectionResult adjustedInspectionResult = CreateInspectionResultWithProject(
                CreateInspectionResultWithCompileItems(
                    inspectionResult,
                    [new CompileItemInfo("Program.cs", sourceFilePath, exists: true)]),
                CreateProjectInfo(
                    inspectionResult.Project,
                    Path.Combine(tempProjectDirectory, "Sample.csproj"),
                    tempProjectDirectory,
                    outputPath: Path.Combine(tempProjectDirectory, "bin"),
                    outputType: "Library",
                    nullable: "enable",
                    implicitUsings: "enable",
                    intermediateOutputPath: null));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(adjustedInspectionResult, Path.Combine(tempProjectDirectory, "artifacts"), emitPortablePdb: false));

            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Code is "CS0246" or "CS0103" or "CS5001"));
        }
        finally
        {
            DeleteOutputDirectory(tempProjectDirectory);
        }
    }

    /// <summary>
    /// Verifies that nullable annotations compile without CS8632 when project nullable is enabled.
    /// </summary>
    [TestMethod]
    public void Compile_WhenNullableIsEnabled_AppliesNullableContextToCompilation()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string tempProjectDirectory = CreateOutputDirectory();
        string sourceFilePath = Path.Combine(tempProjectDirectory, "NullableSample.cs");

        try
        {
            File.WriteAllText(sourceFilePath, "namespace Sample; public sealed class NullableSample { public string? Value { get; set; } }");

            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult adjustedInspectionResult = CreateInspectionResultWithProject(
                CreateInspectionResultWithCompileItems(
                    inspectionResult,
                    [new CompileItemInfo("NullableSample.cs", sourceFilePath, exists: true)]),
                CreateProjectInfo(
                    inspectionResult.Project,
                    Path.Combine(tempProjectDirectory, "Sample.csproj"),
                    tempProjectDirectory,
                    outputPath: Path.Combine(tempProjectDirectory, "bin"),
                    nullable: "enable"));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(adjustedInspectionResult, Path.Combine(tempProjectDirectory, "artifacts"), emitPortablePdb: false));

            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Code == "CS8632"));
        }
        finally
        {
            DeleteOutputDirectory(tempProjectDirectory);
        }
    }

    /// <summary>
    /// Verifies that compilation includes project reference assemblies as metadata references.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectUsesProjectReference_EmitsSuccessfully()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(inspectionResult.ResolvedReferences.Any(reference => reference.Kind == ReferenceKind.Project && reference.Exists && string.Equals(Path.GetExtension(reference.ResolvedPath), ".dll", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.AreEqual(ProjectEmitKind.Executable, compilationResult.EmitSupport.EmitKind);
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.Dependency && artifact.Exists));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that a multi-target console project emits successfully for the highest builder-supported target framework without an explicit target framework request.
    /// </summary>
    [TestMethod]
    public void Compile_WhenProjectIsMultiTargetConsoleAppWithoutExplicitTargetFramework_UsesHighestSupportedTargetFrameworkAndEmitsSuccessfully()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.MultiTargetConsoleAppProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            var inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.MultiTargetConsoleAppProjectPath));
            var compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.AreEqual("net10.0", inspectionResult.Project.TargetFramework);
            Assert.IsTrue(inspectionResult.CompileItems.Count > 0);
            Assert.IsTrue(inspectionResult.ResolvedReferences.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.AreEqual(ProjectEmitKind.Executable, compilationResult.EmitSupport.EmitKind);
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.DebugSymbols && artifact.Exists));
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.RuntimeMetadata && artifact.Exists));
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
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.RuntimeMetadata && artifact.Exists));
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
            Assert.IsTrue(compilationResult.Artifacts.Any(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput && artifact.Exists));
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

    /// <summary>
    /// Verifies that missing resolved references are ignored during metadata reference creation.
    /// </summary>
    [TestMethod]
    public void Compile_WhenResolvedReferenceIsMissing_SkipsReferenceAndStillSucceeds()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult inspectionResultWithMissingReference = CreateInspectionResultWithResolvedReferences(
                inspectionResult,
                inspectionResult.ResolvedReferences.Concat(
                [
                    new ResolvedReferenceInfo(ReferenceKind.Metadata, "MissingReference", "Manual", resolvedPath: null, exists: false),
                ]).ToArray());

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithMissingReference, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that invalid and duplicate metadata reference paths are ignored.
    /// </summary>
    [TestMethod]
    public void Compile_WhenResolvedReferencesContainInvalidOrDuplicateMetadataPaths_SkipsThoseReferencesAndStillSucceeds()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            string duplicateReferencePath = inspectionResult.ResolvedReferences
                .Where(reference => reference.Exists && !string.IsNullOrWhiteSpace(reference.ResolvedPath))
                .Select(reference => reference.ResolvedPath!)
                .First(path => string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase));

            string invalidReferencePath = Path.Combine(outputDirectory, "not-a-metadata-reference.txt");
            File.WriteAllText(invalidReferencePath, "placeholder");

            ProjectInspectionResult inspectionResultWithFilteredReferences = CreateInspectionResultWithResolvedReferences(
                inspectionResult,
                inspectionResult.ResolvedReferences.Concat(
                [
                    new ResolvedReferenceInfo(ReferenceKind.Metadata, "InvalidExtension", "Manual", invalidReferencePath, exists: true),
                    new ResolvedReferenceInfo(ReferenceKind.Metadata, "DuplicateReference", "Manual", duplicateReferencePath, exists: true),
                ]).ToArray());

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithFilteredReferences, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that framework references are retained as Roslyn metadata references for SDK-style projects.
    /// </summary>
    [TestMethod]
    public void Compile_WhenInspectionContainsFrameworkReferences_UsesThemAndSucceeds()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));

            Assert.IsTrue(inspectionResult.ResolvedReferences.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResult, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(
                compilationResult.Succeeded,
                string.Join(Environment.NewLine, compilationResult.Diagnostics.Select(diagnostic => $"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath}")));
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that missing compile items are ignored during syntax-tree creation.
    /// </summary>
    [TestMethod]
    public void Compile_WhenCompileItemIsMissing_SkipsMissingItemAndStillSucceeds()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string outputDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult inspectionResultWithMissingCompileItem = CreateInspectionResultWithCompileItems(
                inspectionResult,
                inspectionResult.CompileItems.Concat(
                [
                    new CompileItemInfo("Missing.cs", Path.Combine(outputDirectory, "Missing.cs"), exists: false),
                ]).ToArray());

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithMissingCompileItem, outputDirectory, emitPortablePdb: true));

            Assert.IsTrue(compilationResult.Succeeded);
            Assert.IsFalse(compilationResult.Diagnostics.Any(diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error));
        }
        finally
        {
            DeleteOutputDirectory(outputDirectory);
        }
    }

    /// <summary>
    /// Verifies that a relative project output path is resolved against the project directory when no request output directory is provided.
    /// </summary>
    [TestMethod]
    public void Compile_WhenRequestOutputDirectoryIsMissingAndProjectOutputPathIsRelative_UsesProjectRelativeOutputPath()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string tempProjectDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult inspectionResultWithRelativeOutput = CreateInspectionResultWithProject(
                inspectionResult,
                CreateProjectInfo(
                    inspectionResult.Project,
                    Path.Combine(tempProjectDirectory, "Sample.csproj"),
                    tempProjectDirectory,
                    @"artifacts\Debug\net10.0\"));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithRelativeOutput, outputDirectory: null, emitPortablePdb: false));
            foreach (BuildDiagnostic diagnostic in compilationResult.Diagnostics)
            {
                Console.WriteLine($"{diagnostic.Severity} {diagnostic.Code} {diagnostic.Message} {diagnostic.FilePath} {diagnostic.Line}:{diagnostic.Column}");
            }

            Assert.IsTrue(compilationResult.Succeeded);
            StringAssert.StartsWith(
                compilationResult.Artifacts.Single(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput).FilePath,
                Path.GetFullPath(Path.Combine(tempProjectDirectory, @"artifacts\Debug\net10.0\")));
        }
        finally
        {
            DeleteOutputDirectory(tempProjectDirectory);
        }
    }

    /// <summary>
    /// Verifies that an absolute project output path is used as-is when no request output directory is provided.
    /// </summary>
    [TestMethod]
    public void Compile_WhenRequestOutputDirectoryIsMissingAndProjectOutputPathIsAbsolute_UsesProjectAbsoluteOutputPath()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string tempProjectDirectory = CreateOutputDirectory();
        string absoluteOutputPath = Path.Combine(tempProjectDirectory, "absolute-output");

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult inspectionResultWithAbsoluteOutput = CreateInspectionResultWithProject(
                inspectionResult,
                CreateProjectInfo(
                    inspectionResult.Project,
                    Path.Combine(tempProjectDirectory, "Sample.csproj"),
                    tempProjectDirectory,
                    absoluteOutputPath));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithAbsoluteOutput, outputDirectory: null, emitPortablePdb: false));

            Assert.IsTrue(compilationResult.Succeeded);
            StringAssert.StartsWith(
                compilationResult.Artifacts.Single(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput).FilePath,
                absoluteOutputPath);
        }
        finally
        {
            DeleteOutputDirectory(tempProjectDirectory);
        }
    }

    /// <summary>
    /// Verifies that the default Workbench.Builder output directory is used when no explicit output path is available.
    /// </summary>
    [TestMethod]
    public void Compile_WhenNoOutputDirectoryIsSpecified_UsesWorkbenchBuilderFallbackDirectory()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleLibraryProjectPath);
        var inspectionService = new ProjectInspectionService(new MsBuildProjectLoader(), new ReferenceResolver(), new TestProjectDetector());
        var compilationService = new ProjectCompilationService(new ProjectEmitSupportEvaluator());
        string tempProjectDirectory = CreateOutputDirectory();

        try
        {
            ProjectInspectionResult inspectionResult = inspectionService.Inspect(new ProjectLoadRequest(TestDataProjectPaths.SimpleLibraryProjectPath));
            ProjectInspectionResult inspectionResultWithDefaultOutput = CreateInspectionResultWithProject(
                inspectionResult,
                CreateProjectInfo(
                    inspectionResult.Project,
                    Path.Combine(tempProjectDirectory, "Sample.csproj"),
                    tempProjectDirectory,
                    outputPath: null));

            ProjectCompilationResult compilationResult = compilationService.Compile(new ProjectCompilationRequest(inspectionResultWithDefaultOutput, outputDirectory: null, emitPortablePdb: false));

            Assert.IsTrue(compilationResult.Succeeded);
            StringAssert.StartsWith(
                compilationResult.Artifacts.Single(artifact => artifact.Kind == CompilationArtifactKind.PrimaryOutput).FilePath,
                Path.Combine(tempProjectDirectory, "bin", "Workbench.Builder"));
        }
        finally
        {
            DeleteOutputDirectory(tempProjectDirectory);
        }
    }

    /// <summary>
    /// Verifies that Roslyn diagnostic severities are mapped to the expected builder severities.
    /// </summary>
    [TestMethod]
    [DataRow(DiagnosticSeverity.Hidden, BuildDiagnosticSeverity.Information)]
    [DataRow(DiagnosticSeverity.Info, BuildDiagnosticSeverity.Information)]
    [DataRow(DiagnosticSeverity.Warning, BuildDiagnosticSeverity.Warning)]
    [DataRow(DiagnosticSeverity.Error, BuildDiagnosticSeverity.Error)]
    public void MapSeverity_WhenSeverityIsKnown_ReturnsExpectedSeverity(DiagnosticSeverity severity, BuildDiagnosticSeverity expectedSeverity)
    {
        BuildDiagnosticSeverity mappedSeverity = InvokeMapSeverity(severity);

        Assert.AreEqual(expectedSeverity, mappedSeverity);
    }

    /// <summary>
    /// Verifies that unknown Roslyn diagnostic severities fall back to information.
    /// </summary>
    [TestMethod]
    public void MapSeverity_WhenSeverityIsUnknown_ReturnsInformation()
    {
        BuildDiagnosticSeverity mappedSeverity = InvokeMapSeverity((DiagnosticSeverity)999);

        Assert.AreEqual(BuildDiagnosticSeverity.Information, mappedSeverity);
    }

    private static string CreateOutputDirectory()
    {
        string outputDirectory = Path.Combine(Path.GetTempPath(), "Workbench.Builder.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);
        return outputDirectory;
    }

    private static string ResolveProjectOutputDirectory(BuildProjectInfo project)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(project.OutputPath));

        return Path.IsPathRooted(project.OutputPath)
            ? project.OutputPath
            : Path.GetFullPath(Path.Combine(project.ProjectDirectory, project.OutputPath));
    }

    private static ProjectInspectionResult CreateInspectionResultWithCompileItems(
        ProjectInspectionResult inspectionResult,
        IReadOnlyList<CompileItemInfo> compileItems)
    {
        return new ProjectInspectionResult(
            inspectionResult.Project,
            compileItems,
            inspectionResult.ProjectReferences,
            inspectionResult.PackageReferences,
            inspectionResult.ResolvedReferences,
            inspectionResult.Diagnostics,
            inspectionResult.IsTestProject);
    }

    private static ProjectInspectionResult CreateInspectionResultWithProject(
        ProjectInspectionResult inspectionResult,
        BuildProjectInfo project)
    {
        return new ProjectInspectionResult(
            project,
            inspectionResult.CompileItems,
            inspectionResult.ProjectReferences,
            inspectionResult.PackageReferences,
            inspectionResult.ResolvedReferences,
            inspectionResult.Diagnostics,
            inspectionResult.IsTestProject);
    }

    private static BuildProjectInfo CreateProjectInfo(
        BuildProjectInfo sourceProject,
        string projectFilePath,
        string projectDirectory,
        string? outputPath,
        string? outputType = null,
        string? nullable = null,
        string? implicitUsings = null,
        string? intermediateOutputPath = null)
    {
        return new BuildProjectInfo(
            projectFilePath,
            projectDirectory,
            sourceProject.AssemblyName,
            sourceProject.RootNamespace,
            sourceProject.TargetFramework,
            outputType ?? sourceProject.OutputType,
            sourceProject.LangVersion,
            nullable ?? sourceProject.Nullable,
            sourceProject.DefineConstants,
            implicitUsings ?? sourceProject.ImplicitUsings,
            sourceProject.Configuration,
            sourceProject.RuntimeIdentifier,
            outputPath,
            intermediateOutputPath ?? sourceProject.IntermediateOutputPath,
            sourceProject.IsSdkStyle,
            sourceProject.IsPackable);
    }

    private static ProjectInspectionResult CreateInspectionResultWithResolvedReferences(
        ProjectInspectionResult inspectionResult,
        IReadOnlyList<ResolvedReferenceInfo> resolvedReferences)
    {
        return new ProjectInspectionResult(
            inspectionResult.Project,
            inspectionResult.CompileItems,
            inspectionResult.ProjectReferences,
            inspectionResult.PackageReferences,
            resolvedReferences,
            inspectionResult.Diagnostics,
            inspectionResult.IsTestProject);
    }

    private static BuildDiagnosticSeverity InvokeMapSeverity(DiagnosticSeverity severity)
    {
        MethodInfo method = typeof(ProjectCompilationService).GetMethod("MapSeverity", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (BuildDiagnosticSeverity)method.Invoke(null, [severity])!;
    }

    private static void DeleteOutputDirectory(string outputDirectory)
    {
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, recursive: true);
        }
    }
}
