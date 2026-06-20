using System;
using System.Collections.Generic;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Services.Inspection;

/// <summary>
/// Composes loading, test-project detection, and reference resolution into the stable V1.1 inspection result.
/// </summary>
public sealed class ProjectInspectionService : IProjectInspectionService
{
    private readonly IProjectLoader _projectLoader;
    private readonly IReferenceResolver _referenceResolver;
    private readonly ITestProjectDetector _testProjectDetector;

    /// <summary>
    /// Initializes a new instance of <see cref="ProjectInspectionService"/>.
    /// </summary>
    /// <param name="projectLoader">The project loader used to evaluate the project.</param>
    /// <param name="referenceResolver">The reference resolver used to resolve framework and metadata references.</param>
    /// <param name="testProjectDetector">The detector used to classify test projects.</param>
    public ProjectInspectionService(
        IProjectLoader projectLoader,
        IReferenceResolver referenceResolver,
        ITestProjectDetector testProjectDetector)
    {
        _projectLoader = projectLoader ?? throw new ArgumentNullException(nameof(projectLoader));
        _referenceResolver = referenceResolver ?? throw new ArgumentNullException(nameof(referenceResolver));
        _testProjectDetector = testProjectDetector ?? throw new ArgumentNullException(nameof(testProjectDetector));
    }

    /// <inheritdoc/>
    public ProjectInspectionResult Inspect(ProjectLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LoadedProjectModel loadedProject = _projectLoader.Load(request);
        bool isTestProject = _testProjectDetector.IsTestProject(loadedProject);
        IReadOnlyList<ResolvedReferenceInfo> resolvedReferences = _referenceResolver.Resolve(loadedProject);
        List<BuildDiagnostic> diagnostics = CreateDiagnostics(loadedProject, resolvedReferences);
        BuildProjectInfo project = CreateProjectInfo(loadedProject);

        return new ProjectInspectionResult(
            project,
            loadedProject.CompileItems,
            loadedProject.ProjectReferences,
            loadedProject.PackageReferences,
            resolvedReferences,
            diagnostics,
            isTestProject);
    }

    private static BuildProjectInfo CreateProjectInfo(LoadedProjectModel loadedProject)
    {
        return new BuildProjectInfo(
            projectFilePath: loadedProject.ProjectFilePath,
            projectDirectory: loadedProject.ProjectDirectory,
            assemblyName: loadedProject.Properties.AssemblyName,
            rootNamespace: loadedProject.Properties.RootNamespace,
            targetFramework: loadedProject.Properties.TargetFramework,
            outputType: loadedProject.Properties.OutputType,
            langVersion: loadedProject.Properties.LangVersion,
            nullable: loadedProject.Properties.Nullable,
            defineConstants: loadedProject.Properties.DefineConstants,
            implicitUsings: loadedProject.Properties.ImplicitUsings,
            configuration: loadedProject.Properties.Configuration,
            runtimeIdentifier: loadedProject.Properties.RuntimeIdentifier,
            outputPath: loadedProject.Properties.OutputPath,
            intermediateOutputPath: loadedProject.Properties.IntermediateOutputPath,
            isSdkStyle: loadedProject.IsSdkStyle,
            isPackable: ParseBooleanOrNull(loadedProject.Properties.IsPackable));
    }

    private static List<BuildDiagnostic> CreateDiagnostics(
        LoadedProjectModel loadedProject,
        IReadOnlyList<ResolvedReferenceInfo> resolvedReferences)
    {
        List<BuildDiagnostic> diagnostics = new(loadedProject.Diagnostics);

        if (!loadedProject.IsSdkStyle)
        {
            diagnostics.Add(
                new BuildDiagnostic(
                    BuildDiagnosticSeverity.Warning,
                    "WB1001",
                    "The evaluated project does not appear to be SDK-style.",
                    loadedProject.ProjectFilePath));
        }

        if (string.IsNullOrWhiteSpace(loadedProject.Properties.TargetFramework))
        {
            diagnostics.Add(
                new BuildDiagnostic(
                    BuildDiagnosticSeverity.Warning,
                    "WB1002",
                    "The evaluated project did not produce a TargetFramework value.",
                    loadedProject.ProjectFilePath));
        }

        foreach (ProjectReferenceInfo projectReference in loadedProject.ProjectReferences)
        {
            if (!projectReference.Exists)
            {
                diagnostics.Add(
                    new BuildDiagnostic(
                        BuildDiagnosticSeverity.Warning,
                        "WB1101",
                        $"The project reference '{projectReference.Include}' could not be found.",
                        loadedProject.ProjectFilePath));
            }
        }

        foreach (CompileItemInfo compileItem in loadedProject.CompileItems)
        {
            if (!compileItem.Exists)
            {
                diagnostics.Add(
                    new BuildDiagnostic(
                        BuildDiagnosticSeverity.Warning,
                        "WB1102",
                        $"The compile item '{compileItem.Include}' could not be found.",
                        loadedProject.ProjectFilePath));
            }
        }

        foreach (ResolvedReferenceInfo resolvedReference in resolvedReferences)
        {
            if (!resolvedReference.Exists)
            {
                diagnostics.Add(
                    new BuildDiagnostic(
                        BuildDiagnosticSeverity.Warning,
                        "WB1103",
                        $"The resolved reference '{resolvedReference.DisplayName}' could not be found on disk.",
                        loadedProject.ProjectFilePath));
            }
        }

        return diagnostics;
    }

    private static bool? ParseBooleanOrNull(string? value)
    {
        if (bool.TryParse(value, out bool parsedValue))
        {
            return parsedValue;
        }

        return null;
    }
}
