using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.References;

namespace AA98_AvlnCodeStudio.Building.Workbench.Services;

/// <summary>
/// Adapts Workbench.Builder.Core services to the Code Studio builder contract.
/// </summary>
public sealed class WorkbenchCodeStudioBuilderService : ICodeStudioBuilderService
{
    private readonly IProjectInspectionService _projectInspectionService;
    private readonly IProjectCompilationService _projectCompilationService;
    private readonly ITestExecutionService _testExecutionService;

    /// <summary>
    /// Initializes a new instance of <see cref="WorkbenchCodeStudioBuilderService"/>.
    /// </summary>
    /// <param name="projectInspectionService">The project inspection service.</param>
    /// <param name="projectCompilationService">The project compilation service.</param>
    /// <param name="testExecutionService">The targeted test execution service.</param>
    public WorkbenchCodeStudioBuilderService(
        IProjectInspectionService projectInspectionService,
        IProjectCompilationService projectCompilationService,
        ITestExecutionService testExecutionService)
    {
        _projectInspectionService = projectInspectionService ?? throw new ArgumentNullException(nameof(projectInspectionService));
        _projectCompilationService = projectCompilationService ?? throw new ArgumentNullException(nameof(projectCompilationService));
        _testExecutionService = testExecutionService ?? throw new ArgumentNullException(nameof(testExecutionService));
    }

    /// <inheritdoc/>
    public Task<BuilderProjectInspectionResult> InspectProjectAsync(BuilderProjectInspectionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        ProjectInspectionResult inspectionResult = _projectInspectionService.Inspect(CreateLoadRequest(request.ProjectPath, request.Configuration, request.TargetFramework));
        return Task.FromResult(MapInspectionResult(inspectionResult));
    }

    /// <inheritdoc/>
    public Task<BuilderBuildResult> BuildProjectAsync(BuilderBuildRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        ProjectInspectionResult inspectionResult = _projectInspectionService.Inspect(CreateLoadRequest(request.ProjectPath, request.Configuration, request.TargetFramework));
        ProjectCompilationResult compilationResult = _projectCompilationService.Compile(new ProjectCompilationRequest(inspectionResult));
        BuilderBuildResult result = new()
        {
            ProjectPath = compilationResult.InspectionResult.Project.ProjectFilePath,
            Succeeded = compilationResult.Succeeded,
        };

        foreach (CompilationArtifactInfo artifact in compilationResult.Artifacts)
        {
            result.Artifacts.Add(new BuilderCompilationArtifact
            {
                Kind = artifact.Kind.ToString(),
                Path = artifact.FilePath,
                TargetFramework = compilationResult.InspectionResult.Project.TargetFramework,
            });
        }

        foreach (BuildDiagnostic diagnostic in compilationResult.Diagnostics)
        {
            result.Diagnostics.Add(MapDiagnostic(diagnostic));
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<BuilderTargetedTestResult> RunTargetedTestsAsync(BuilderTargetedTestRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        TestRunSummary summary = await _testExecutionService.RunTestsAsync(CreateTestRunRequest(request), cancellationToken).ConfigureAwait(false);

        BuilderTargetedTestResult result = new()
        {
            ProjectPath = request.ProjectPath,
            Outcome = MapOutcome(summary.Outcome),
            TotalCount = summary.TotalCount,
            PassedCount = summary.PassedCount,
            FailedCount = summary.FailedCount,
            SkippedCount = summary.SkippedCount,
        };

        return result;
    }

    private static ProjectLoadRequest CreateLoadRequest(string projectPath, string? configuration, string? targetFramework)
    {
        return new ProjectLoadRequest(Path.GetFullPath(projectPath), configuration, targetFramework);
    }

    private static TestRunRequest CreateTestRunRequest(BuilderTargetedTestRequest request)
    {
        TestRunRequest delegatedRequest = new()
        {
            WorkspaceRootPath = string.IsNullOrWhiteSpace(request.WorkspaceRootPath) ? null : request.WorkspaceRootPath,
            ProjectPath = request.ProjectPath,
            TargetFramework = request.TargetFramework,
            CollectCoverage = false,
        };

        foreach (string target in request.Targets)
        {
            delegatedRequest.Targets.Add(target);
        }

        return delegatedRequest;
    }

    private static BuilderProjectInspectionResult MapInspectionResult(ProjectInspectionResult inspectionResult)
    {
        BuilderProjectInspectionResult result = new()
        {
            ProjectPath = inspectionResult.Project.ProjectFilePath,
            ProjectName = inspectionResult.Project.AssemblyName,
            TargetFramework = inspectionResult.Project.TargetFramework,
            IsTestProject = inspectionResult.IsTestProject,
        };

        foreach (string compileItemPath in inspectionResult.CompileItems.Select(static item => item.FilePath))
        {
            result.CompileItems.Add(compileItemPath);
        }

        foreach (var projectReference in inspectionResult.ProjectReferences)
        {
            result.ProjectReferences.Add(new BuilderReferenceDescriptor
            {
                Kind = BuilderReferenceKind.Project,
                Name = Path.GetFileNameWithoutExtension(projectReference.ProjectFilePath),
                Identity = projectReference.Include,
                Path = projectReference.ProjectFilePath,
            });
        }

        foreach (var packageReference in inspectionResult.PackageReferences)
        {
            result.PackageReferences.Add(new BuilderReferenceDescriptor
            {
                Kind = BuilderReferenceKind.Package,
                Name = packageReference.PackageId,
                Identity = packageReference.PackageId,
                Version = packageReference.Version,
            });
        }

        foreach (ResolvedReferenceInfo reference in inspectionResult.ResolvedReferences)
        {
            result.ResolvedReferences.Add(new BuilderReferenceDescriptor
            {
                Kind = MapReferenceKind(reference.Kind),
                Name = reference.DisplayName,
                Identity = reference.Source,
                Path = reference.ResolvedPath,
            });
        }

        foreach (BuildDiagnostic diagnostic in inspectionResult.Diagnostics)
        {
            result.Diagnostics.Add(MapDiagnostic(diagnostic));
        }

        return result;
    }

    private static BuilderDiagnostic MapDiagnostic(BuildDiagnostic diagnostic)
    {
        return new BuilderDiagnostic
        {
            Severity = MapSeverity(diagnostic.Severity),
            Code = diagnostic.Code,
            Message = diagnostic.Message,
            FilePath = diagnostic.FilePath,
            LineNumber = diagnostic.Line,
            ColumnNumber = diagnostic.Column,
        };
    }

    private static BuilderDiagnosticSeverity MapSeverity(BuildDiagnosticSeverity severity)
    {
        return severity switch
        {
            BuildDiagnosticSeverity.Information => BuilderDiagnosticSeverity.Info,
            BuildDiagnosticSeverity.Warning => BuilderDiagnosticSeverity.Warning,
            BuildDiagnosticSeverity.Error => BuilderDiagnosticSeverity.Error,
            _ => BuilderDiagnosticSeverity.Unknown,
        };
    }

    private static BuilderReferenceKind MapReferenceKind(ReferenceKind kind)
    {
        return kind switch
        {
            ReferenceKind.Framework => BuilderReferenceKind.Framework,
            ReferenceKind.Package => BuilderReferenceKind.Package,
            ReferenceKind.Project => BuilderReferenceKind.Project,
            ReferenceKind.Metadata => BuilderReferenceKind.Metadata,
            _ => BuilderReferenceKind.Unknown,
        };
    }

    private static BuilderTargetedTestOutcome MapOutcome(TestRunOutcome outcome)
    {
        return outcome switch
        {
            TestRunOutcome.Passed => BuilderTargetedTestOutcome.Passed,
            TestRunOutcome.Failed => BuilderTargetedTestOutcome.Failed,
            TestRunOutcome.Canceled => BuilderTargetedTestOutcome.Canceled,
            TestRunOutcome.Partial => BuilderTargetedTestOutcome.Partial,
            _ => BuilderTargetedTestOutcome.Unknown,
        };
    }
}
