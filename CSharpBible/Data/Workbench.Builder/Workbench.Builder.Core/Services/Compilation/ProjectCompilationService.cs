using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Services.Compilation;

/// <summary>
/// Compiles the current first-slice V1.2 project categories into assemblies and optional Portable PDB artifacts.
/// </summary>
public sealed class ProjectCompilationService : IProjectCompilationService
{
    private readonly IProjectEmitSupportEvaluator _projectEmitSupportEvaluator;

    /// <summary>
    /// Initializes a new instance of <see cref="ProjectCompilationService"/>.
    /// </summary>
    /// <param name="projectEmitSupportEvaluator">The evaluator that determines whether the inspected project can be emitted.</param>
    public ProjectCompilationService(IProjectEmitSupportEvaluator projectEmitSupportEvaluator)
    {
        _projectEmitSupportEvaluator = projectEmitSupportEvaluator ?? throw new ArgumentNullException(nameof(projectEmitSupportEvaluator));
    }

    /// <inheritdoc/>
    public ProjectCompilationResult Compile(ProjectCompilationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ProjectEmitSupport emitSupport = _projectEmitSupportEvaluator.Evaluate(request.InspectionResult);
        if (!emitSupport.CanEmit)
        {
            return new ProjectCompilationResult(
                request.InspectionResult,
                emitSupport,
                Array.Empty<CompilationArtifactInfo>(),
                new[]
                {
                    new BuildDiagnostic(
                        BuildDiagnosticSeverity.Warning,
                        "WB2001",
                        emitSupport.Reason ?? "The inspected project cannot be emitted in the current V1.2 slice.",
                        request.InspectionResult.Project.ProjectFilePath),
                },
                succeeded: false);
        }

        string outputDirectory = ResolveOutputDirectory(request);
        Directory.CreateDirectory(outputDirectory);

        string assemblyPath = Path.Combine(outputDirectory, request.InspectionResult.Project.AssemblyName + ".dll");
        string pdbPath = Path.Combine(outputDirectory, request.InspectionResult.Project.AssemblyName + ".pdb");

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: request.InspectionResult.Project.AssemblyName,
            syntaxTrees: CreateSyntaxTrees(request),
            references: CreateMetadataReferences(request),
            options: CreateCompilationOptions(emitSupport));

        using FileStream assemblyStream = File.Create(assemblyPath);
        using FileStream? pdbStream = request.EmitPortablePdb ? File.Create(pdbPath) : null;
        EmitResult emitResult = compilation.Emit(
            peStream: assemblyStream,
            pdbStream: pdbStream,
            options: request.EmitPortablePdb
                ? new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb)
                : null);

        List<CompilationArtifactInfo> artifacts = new()
        {
            new CompilationArtifactInfo(CompilationArtifactKind.PrimaryOutput, assemblyPath, File.Exists(assemblyPath)),
        };

        if (request.EmitPortablePdb)
        {
            artifacts.Add(new CompilationArtifactInfo(CompilationArtifactKind.DebugSymbols, pdbPath, File.Exists(pdbPath)));
        }

        IReadOnlyList<BuildDiagnostic> diagnostics = emitResult.Diagnostics
            .Select(MapDiagnostic)
            .ToArray();

        return new ProjectCompilationResult(
            request.InspectionResult,
            emitSupport,
            artifacts,
            diagnostics,
            emitResult.Success);
    }

    private static IEnumerable<SyntaxTree> CreateSyntaxTrees(ProjectCompilationRequest request)
    {
        foreach (Models.Projects.CompileItemInfo compileItem in request.InspectionResult.CompileItems)
        {
            if (!compileItem.Exists)
            {
                continue;
            }

            string sourceText = File.ReadAllText(compileItem.FilePath);
            yield return CSharpSyntaxTree.ParseText(SourceText.From(sourceText, Encoding.UTF8), path: compileItem.FilePath);
        }
    }

    private static IEnumerable<MetadataReference> CreateMetadataReferences(ProjectCompilationRequest request)
    {
        HashSet<string> seenPaths = new(StringComparer.OrdinalIgnoreCase);

        foreach (ResolvedReferenceInfo reference in request.InspectionResult.ResolvedReferences)
        {
            if (!reference.Exists || string.IsNullOrWhiteSpace(reference.ResolvedPath))
            {
                continue;
            }

            if (reference.Kind == ReferenceKind.Analyzer || reference.Kind == ReferenceKind.Project)
            {
                continue;
            }

            if (!IsMetadataReferencePath(reference.ResolvedPath) || !seenPaths.Add(reference.ResolvedPath))
            {
                continue;
            }

            yield return MetadataReference.CreateFromFile(reference.ResolvedPath);
        }
    }

    private static CSharpCompilationOptions CreateCompilationOptions(ProjectEmitSupport emitSupport)
    {
        OutputKind outputKind = emitSupport.EmitKind == ProjectEmitKind.Executable
            ? OutputKind.ConsoleApplication
            : OutputKind.DynamicallyLinkedLibrary;

        return new CSharpCompilationOptions(outputKind);
    }

    private static string ResolveOutputDirectory(ProjectCompilationRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.OutputDirectory))
        {
            return Path.GetFullPath(request.OutputDirectory);
        }

        string? outputPath = request.InspectionResult.Project.OutputPath;
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            return Path.IsPathRooted(outputPath)
                ? outputPath
                : Path.GetFullPath(Path.Combine(request.InspectionResult.Project.ProjectDirectory, outputPath));
        }

        return Path.Combine(request.InspectionResult.Project.ProjectDirectory, "bin", "Workbench.Builder");
    }

    private static bool IsMetadataReferencePath(string path)
    {
        string extension = Path.GetExtension(path);
        return string.Equals(extension, ".dll", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".exe", StringComparison.OrdinalIgnoreCase)
            || string.Equals(extension, ".winmd", StringComparison.OrdinalIgnoreCase);
    }

    private static BuildDiagnostic MapDiagnostic(Diagnostic diagnostic)
    {
        FileLinePositionSpan locationSpan = diagnostic.Location.GetMappedLineSpan();
        string? filePath = diagnostic.Location.IsInSource ? locationSpan.Path : null;
        int? line = diagnostic.Location.IsInSource ? locationSpan.StartLinePosition.Line + 1 : null;
        int? column = diagnostic.Location.IsInSource ? locationSpan.StartLinePosition.Character + 1 : null;

        return new BuildDiagnostic(
            MapSeverity(diagnostic.Severity),
            diagnostic.Id,
            diagnostic.GetMessage(),
            filePath,
            line,
            column);
    }

    private static BuildDiagnosticSeverity MapSeverity(DiagnosticSeverity severity)
    {
        return severity switch
        {
            DiagnosticSeverity.Hidden => BuildDiagnosticSeverity.Information,
            DiagnosticSeverity.Info => BuildDiagnosticSeverity.Information,
            DiagnosticSeverity.Warning => BuildDiagnosticSeverity.Warning,
            DiagnosticSeverity.Error => BuildDiagnosticSeverity.Error,
            _ => BuildDiagnosticSeverity.Information,
        };
    }
}
