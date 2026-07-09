#if NET10_0
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Building.Workbench.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies deterministic result mapping for the Workbench-backed Code Studio builder service.
/// </summary>
[TestClass]
public class WorkbenchCodeStudioBuilderServiceTests
{
    /// <summary>
    /// Verifies that inspection results are mapped to the shared builder contract.
    /// </summary>
    [TestMethod]
    public async Task InspectProjectAsync_MapsStructuredInspectionResult()
    {
        IProjectInspectionService projectInspectionService = Substitute.For<IProjectInspectionService>();
        IProjectCompilationService projectCompilationService = Substitute.For<IProjectCompilationService>();
        ITestExecutionService testExecutionService = Substitute.For<ITestExecutionService>();
        ProjectInspectionResult inspectionResult = CreateInspectionResult();
        ProjectLoadRequest? capturedRequest = null;

        projectInspectionService
            .Inspect(Arg.Do<ProjectLoadRequest>(request => capturedRequest = request))
            .Returns(inspectionResult);

        WorkbenchCodeStudioBuilderService service = new(projectInspectionService, projectCompilationService, testExecutionService);
        BuilderProjectInspectionRequest request = new()
        {
            ProjectPath = @".\tests\SampleProject.csproj",
            Configuration = "Release",
            TargetFramework = "net10.0",
        };

        BuilderProjectInspectionResult result = await service.InspectProjectAsync(request).ConfigureAwait(false);

        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(Path.GetFullPath(request.ProjectPath), capturedRequest.ProjectFilePath);
        Assert.AreEqual(request.Configuration, capturedRequest.Configuration);
        Assert.AreEqual(request.TargetFramework, capturedRequest.TargetFramework);
        Assert.AreEqual(Path.GetFullPath(request.ProjectPath), result.ProjectPath);
        Assert.AreEqual("SampleProject", result.ProjectName);
        Assert.AreEqual("net10.0", result.TargetFramework);
        Assert.IsTrue(result.IsTestProject);
        CollectionAssert.AreEqual(new[] { Path.Combine("C:", "src", "SampleProject", "Program.cs") }, result.CompileItems.ToArray());
        Assert.AreEqual(1, result.ProjectReferences.Count);
        Assert.AreEqual(BuilderReferenceKind.Project, result.ProjectReferences[0].Kind);
        Assert.AreEqual("Shared", result.ProjectReferences[0].Name);
        Assert.AreEqual("..\\Shared\\Shared.csproj", result.ProjectReferences[0].Identity);
        Assert.AreEqual(Path.Combine("C:", "src", "Shared", "Shared.csproj"), result.ProjectReferences[0].Path);
        Assert.AreEqual(1, result.PackageReferences.Count);
        Assert.AreEqual(BuilderReferenceKind.Package, result.PackageReferences[0].Kind);
        Assert.AreEqual("NSubstitute", result.PackageReferences[0].Name);
        Assert.AreEqual("5.1.0", result.PackageReferences[0].Version);
        Assert.AreEqual(2, result.ResolvedReferences.Count);
        Assert.AreEqual(BuilderReferenceKind.Framework, result.ResolvedReferences[0].Kind);
        Assert.AreEqual(BuilderReferenceKind.Metadata, result.ResolvedReferences[1].Kind);
        Assert.AreEqual(1, result.Diagnostics.Count);
        Assert.AreEqual(BuilderDiagnosticSeverity.Warning, result.Diagnostics[0].Severity);
        Assert.AreEqual("WB1001", result.Diagnostics[0].Code);
        Assert.AreEqual("Inspection warning", result.Diagnostics[0].Message);
        Assert.AreEqual(Path.Combine("C:", "src", "SampleProject", "Program.cs"), result.Diagnostics[0].FilePath);
        Assert.AreEqual(5, result.Diagnostics[0].LineNumber);
        Assert.AreEqual(7, result.Diagnostics[0].ColumnNumber);
    }

    /// <summary>
    /// Verifies that build results are mapped to artifacts and diagnostics.
    /// </summary>
    [TestMethod]
    public async Task BuildProjectAsync_MapsCompilationArtifactsAndDiagnostics()
    {
        IProjectInspectionService projectInspectionService = Substitute.For<IProjectInspectionService>();
        IProjectCompilationService projectCompilationService = Substitute.For<IProjectCompilationService>();
        ITestExecutionService testExecutionService = Substitute.For<ITestExecutionService>();
        ProjectInspectionResult inspectionResult = CreateInspectionResult();
        ProjectCompilationRequest? capturedCompilationRequest = null;
        projectInspectionService
            .Inspect(Arg.Any<ProjectLoadRequest>())
            .Returns(inspectionResult);
        projectCompilationService
            .Compile(Arg.Do<ProjectCompilationRequest>(request => capturedCompilationRequest = request))
            .Returns(new ProjectCompilationResult(
                inspectionResult,
                new ProjectEmitSupport(true, ProjectEmitKind.Library),
                new List<CompilationArtifactInfo>
                {
                    new(CompilationArtifactKind.PrimaryOutput, Path.Combine("C:", "out", "SampleProject.dll"), true),
                    new(CompilationArtifactKind.DebugSymbols, Path.Combine("C:", "out", "SampleProject.pdb"), true),
                },
                new List<BuildDiagnostic>
                {
                    new(BuildDiagnosticSeverity.Error, "WB2001", "Compilation failed", Path.Combine("C:", "src", "SampleProject", "Program.cs"), 11, 3),
                },
                succeeded: false));

        WorkbenchCodeStudioBuilderService service = new(projectInspectionService, projectCompilationService, testExecutionService);
        BuilderBuildResult result = await service.BuildProjectAsync(new BuilderBuildRequest
        {
            ProjectPath = @".\tests\SampleProject.csproj",
            Configuration = "Debug",
            TargetFramework = "net10.0",
        }).ConfigureAwait(false);

        Assert.IsNotNull(capturedCompilationRequest);
        Assert.AreSame(inspectionResult, capturedCompilationRequest.InspectionResult);
        Assert.AreEqual(Path.GetFullPath(@".\tests\SampleProject.csproj"), result.ProjectPath);
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(2, result.Artifacts.Count);
        Assert.AreEqual(nameof(CompilationArtifactKind.PrimaryOutput), result.Artifacts[0].Kind);
        Assert.AreEqual("net10.0", result.Artifacts[0].TargetFramework);
        Assert.AreEqual(Path.Combine("C:", "out", "SampleProject.dll"), result.Artifacts[0].Path);
        Assert.AreEqual(nameof(CompilationArtifactKind.DebugSymbols), result.Artifacts[1].Kind);
        Assert.AreEqual(1, result.Diagnostics.Count);
        Assert.AreEqual(BuilderDiagnosticSeverity.Error, result.Diagnostics[0].Severity);
        Assert.AreEqual("WB2001", result.Diagnostics[0].Code);
        Assert.AreEqual("Compilation failed", result.Diagnostics[0].Message);
    }

    /// <summary>
    /// Verifies that targeted test execution is delegated and mapped to the builder contract.
    /// </summary>
    [TestMethod]
    public async Task RunTargetedTestsAsync_DelegatesToTestExecutionService()
    {
        IProjectInspectionService projectInspectionService = Substitute.For<IProjectInspectionService>();
        IProjectCompilationService projectCompilationService = Substitute.For<IProjectCompilationService>();
        ITestExecutionService testExecutionService = Substitute.For<ITestExecutionService>();
        TestRunRequest? capturedRequest = null;
        CancellationToken capturedCancellationToken = default;

        testExecutionService
            .RunTestsAsync(
                Arg.Do<TestRunRequest>(request => capturedRequest = request),
                Arg.Do<CancellationToken>(token => capturedCancellationToken = token))
            .Returns(Task.FromResult(new TestRunSummary
            {
                Outcome = TestRunOutcome.Partial,
                TotalCount = 3,
                PassedCount = 1,
                FailedCount = 1,
                SkippedCount = 1,
            }));

        WorkbenchCodeStudioBuilderService service = new(projectInspectionService, projectCompilationService, testExecutionService);
        BuilderTargetedTestRequest request = new()
        {
            WorkspaceRootPath = @"C:\workspace",
            ProjectPath = @"C:\tests\SampleProject.Tests.csproj",
            TargetFramework = "net10.0",
        };
        request.Targets.Add("Namespace.Tests.SampleTests.TestA");
        request.Targets.Add("Namespace.Tests.SampleTests.TestB");

        BuilderTargetedTestResult result = await service.RunTargetedTestsAsync(request, CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual(@"C:\tests\SampleProject.Tests.csproj", result.ProjectPath);
        Assert.AreEqual(BuilderTargetedTestOutcome.Partial, result.Outcome);
        Assert.AreEqual(3, result.TotalCount);
        Assert.AreEqual(1, result.PassedCount);
        Assert.AreEqual(1, result.FailedCount);
        Assert.AreEqual(1, result.SkippedCount);
        Assert.AreEqual(0, result.Diagnostics.Count);
        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(@"C:\workspace", capturedRequest.WorkspaceRootPath);
        Assert.AreEqual(@"C:\tests\SampleProject.Tests.csproj", capturedRequest.ProjectPath);
        Assert.AreEqual("net10.0", capturedRequest.TargetFramework);
        Assert.IsFalse(capturedRequest.CollectCoverage);
        CollectionAssert.AreEqual(request.Targets.ToArray(), capturedRequest.Targets.ToArray());
        Assert.AreEqual(CancellationToken.None, capturedCancellationToken);
    }

    private static ProjectInspectionResult CreateInspectionResult()
    {
        string projectPath = Path.GetFullPath(@".\tests\SampleProject.csproj");
        string projectDirectory = Path.GetDirectoryName(projectPath)!;
        string sourceFilePath = Path.Combine("C:", "src", "SampleProject", "Program.cs");

        BuildProjectInfo project = new(
            projectPath,
            projectDirectory,
            "SampleProject",
            "SampleProject",
            "net10.0",
            outputType: "Library",
            langVersion: "preview",
            nullable: "enable",
            defineConstants: "DEBUG;TRACE",
            implicitUsings: "disable",
            configuration: "Release",
            runtimeIdentifier: null,
            outputPath: Path.Combine(projectDirectory, "bin", "Release", "net10.0"),
            intermediateOutputPath: Path.Combine(projectDirectory, "obj", "Release", "net10.0"),
            isSdkStyle: true,
            isPackable: false);

        return new ProjectInspectionResult(
            project,
            new List<CompileItemInfo>
            {
                new("Program.cs", sourceFilePath, true),
            },
            new List<ProjectReferenceInfo>
            {
                new("..\\Shared\\Shared.csproj", Path.Combine("C:", "src", "Shared", "Shared.csproj"), true),
            },
            new List<PackageReferenceInfo>
            {
                new("NSubstitute", "5.1.0", privateAssets: null),
            },
            new List<ResolvedReferenceInfo>
            {
                new(ReferenceKind.Framework, "Microsoft.NETCore.App", "Microsoft.NETCore.App", Path.Combine("C:", "ref", "Microsoft.NETCore.App.dll"), true),
                new(ReferenceKind.Metadata, "System.Runtime", "System.Runtime", Path.Combine("C:", "ref", "System.Runtime.dll"), true),
            },
            new List<BuildDiagnostic>
            {
                new(BuildDiagnosticSeverity.Warning, "WB1001", "Inspection warning", sourceFilePath, 5, 7),
            },
            isTestProject: true);
    }
}
#endif
