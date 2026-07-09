using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    private static readonly Regex ExternalBuildDiagnosticPattern = new(
        @"^(?<file>.+?)(?:\((?<line>\d+)(?:,(?<column>\d+))?\))?\s*:\s*(?<severity>warning|error)\s+(?<code>[A-Z]+\d+)\s*:\s*(?<message>.+?)(?:\s+\[(?<project>[^\]]+)\])?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

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
            options: CreateCompilationOptions(request, emitSupport));

        EmitResult emitResult;
        using (FileStream assemblyStream = File.Create(assemblyPath))
        {
            using FileStream? pdbStream = request.EmitPortablePdb ? File.Create(pdbPath) : null;
            emitResult = compilation.Emit(
                peStream: assemblyStream,
                pdbStream: pdbStream,
                options: request.EmitPortablePdb
                    ? new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb)
                    : null);
        }

        List<CompilationArtifactInfo> artifacts = new()
        {
            new CompilationArtifactInfo(CompilationArtifactKind.PrimaryOutput, assemblyPath, File.Exists(assemblyPath)),
        };

        if (request.EmitPortablePdb)
        {
            artifacts.Add(new CompilationArtifactInfo(CompilationArtifactKind.DebugSymbols, pdbPath, File.Exists(pdbPath)));
        }

        List<BuildDiagnostic> diagnostics = emitResult.Diagnostics
            .Select(MapDiagnostic)
            .ToList();

        bool succeeded = emitResult.Success;

        if (emitResult.Success)
        {
            AddCopiedReferenceArtifacts(request, outputDirectory, artifacts);
            IReadOnlyList<BuildDiagnostic> runtimeBuildDiagnostics = EnsureExecutableBuildArtifacts(request);
            diagnostics.AddRange(runtimeBuildDiagnostics);
            succeeded = !diagnostics.Any(static diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error);

            if (succeeded)
            {
                AddExecutableRuntimeArtifacts(request, outputDirectory, artifacts);
            }
        }

        return new ProjectCompilationResult(
            request.InspectionResult,
            emitSupport,
            artifacts,
            diagnostics,
            succeeded);
    }

    private static IEnumerable<SyntaxTree> CreateSyntaxTrees(ProjectCompilationRequest request)
    {
        CSharpParseOptions parseOptions = CreateParseOptions(request.InspectionResult.Project);

        foreach (SyntaxTree implicitUsingTree in CreateImplicitUsingSyntaxTrees(request, parseOptions))
        {
            yield return implicitUsingTree;
        }

        foreach (Models.Projects.CompileItemInfo compileItem in request.InspectionResult.CompileItems)
        {
            if (!compileItem.Exists)
            {
                continue;
            }

            string sourceText = File.ReadAllText(compileItem.FilePath);
            yield return CSharpSyntaxTree.ParseText(SourceText.From(sourceText, Encoding.UTF8), parseOptions, compileItem.FilePath);
        }
    }

    private static IEnumerable<SyntaxTree> CreateImplicitUsingSyntaxTrees(ProjectCompilationRequest request, CSharpParseOptions parseOptions)
    {
        if (!IsImplicitUsingsEnabled(request))
        {
            yield break;
        }

        if (request.InspectionResult.CompileItems.Any(static compileItem => compileItem.Exists && compileItem.FilePath.EndsWith(".GlobalUsings.g.cs", StringComparison.OrdinalIgnoreCase)))
        {
            yield break;
        }

        IReadOnlyList<string> generatedGlobalUsings = GetGeneratedGlobalUsingFiles(request.InspectionResult.Project);
        if (generatedGlobalUsings.Count > 0)
        {
            foreach (string generatedGlobalUsing in generatedGlobalUsings)
            {
                string sourceText = File.ReadAllText(generatedGlobalUsing);
                yield return CSharpSyntaxTree.ParseText(SourceText.From(sourceText, Encoding.UTF8), parseOptions, generatedGlobalUsing);
            }

            yield break;
        }

        string source = string.Join(
            Environment.NewLine,
            GetFallbackImplicitUsings().Select(static namespaceName => $"global using {namespaceName};"));

        yield return CSharpSyntaxTree.ParseText(
            SourceText.From(source, Encoding.UTF8),
            parseOptions,
            Path.Combine(request.InspectionResult.Project.ProjectDirectory, "Workbench.Builder.ImplicitUsings.g.cs"));
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

            if (reference.Kind == ReferenceKind.Analyzer)
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

    private static CSharpCompilationOptions CreateCompilationOptions(ProjectCompilationRequest request, ProjectEmitSupport emitSupport)
    {
        OutputKind outputKind = emitSupport.EmitKind == ProjectEmitKind.Executable
            ? OutputKind.ConsoleApplication
            : OutputKind.DynamicallyLinkedLibrary;

        return new CSharpCompilationOptions(outputKind)
            .WithNullableContextOptions(ResolveNullableContextOptions(request.InspectionResult.Project.Nullable));
    }

    private static CSharpParseOptions CreateParseOptions(Models.Projects.BuildProjectInfo project)
    {
        CSharpParseOptions parseOptions = new();

        if (!string.IsNullOrWhiteSpace(project.LangVersion)
            && LanguageVersionFacts.TryParse(project.LangVersion, out LanguageVersion languageVersion))
        {
            parseOptions = parseOptions.WithLanguageVersion(languageVersion);
        }

        string[] preprocessorSymbols = GetPreprocessorSymbols(project.DefineConstants);
        if (preprocessorSymbols.Length > 0)
        {
            parseOptions = parseOptions.WithPreprocessorSymbols(preprocessorSymbols);
        }

        return parseOptions;
    }

    private static string[] GetPreprocessorSymbols(string? defineConstants)
    {
        if (string.IsNullOrWhiteSpace(defineConstants))
        {
            return [];
        }

        return defineConstants
            .Split([';', ',', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    private static NullableContextOptions ResolveNullableContextOptions(string? nullable)
    {
        return nullable?.Trim().ToLowerInvariant() switch
        {
            "enable" => NullableContextOptions.Enable,
            "annotations" => NullableContextOptions.Annotations,
            "warnings" => NullableContextOptions.Warnings,
            _ => NullableContextOptions.Disable,
        };
    }

    private static bool IsImplicitUsingsEnabled(ProjectCompilationRequest request)
    {
        if (!request.InspectionResult.Project.IsSdkStyle)
        {
            return false;
        }

        return request.InspectionResult.Project.ImplicitUsings?.Trim().ToLowerInvariant() switch
        {
            "enable" => true,
            "true" => true,
            _ => false,
        };
    }

    private static IReadOnlyList<string> GetGeneratedGlobalUsingFiles(Models.Projects.BuildProjectInfo project)
    {
        string? intermediateOutputPath = project.IntermediateOutputPath;
        if (string.IsNullOrWhiteSpace(intermediateOutputPath))
        {
            return [];
        }

        string intermediateOutputDirectory = Path.IsPathRooted(intermediateOutputPath)
            ? intermediateOutputPath
            : Path.GetFullPath(Path.Combine(project.ProjectDirectory, intermediateOutputPath));

        if (!Directory.Exists(intermediateOutputDirectory))
        {
            return [];
        }

        return Directory
            .GetFiles(intermediateOutputDirectory, "*.GlobalUsings.g.cs", SearchOption.TopDirectoryOnly)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<string> GetFallbackImplicitUsings()
    {
        return
        [
            "System",
            "System.Collections.Generic",
            "System.IO",
            "System.Linq",
            "System.Net.Http",
            "System.Threading",
            "System.Threading.Tasks",
        ];
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

    private static void AddCopiedReferenceArtifacts(ProjectCompilationRequest request, string outputDirectory, ICollection<CompilationArtifactInfo> artifacts)
    {
        HashSet<string> copiedPaths = new(StringComparer.OrdinalIgnoreCase);

        foreach (ResolvedReferenceInfo reference in request.InspectionResult.ResolvedReferences)
        {
            if (!reference.Exists || string.IsNullOrWhiteSpace(reference.ResolvedPath) || !IsMetadataReferencePath(reference.ResolvedPath))
            {
                continue;
            }

            if (reference.Kind == ReferenceKind.Analyzer)
            {
                continue;
            }

            string sourcePath = reference.ResolvedPath;
            string destinationPath = Path.Combine(outputDirectory, Path.GetFileName(sourcePath));
            if (copiedPaths.Contains(destinationPath) || File.Exists(destinationPath))
            {
                copiedPaths.Add(destinationPath);
                continue;
            }

            File.Copy(sourcePath, destinationPath, overwrite: true);
            copiedPaths.Add(destinationPath);
            artifacts.Add(new CompilationArtifactInfo(CompilationArtifactKind.Dependency, destinationPath, File.Exists(destinationPath)));
        }
    }

    private static void AddExecutableRuntimeArtifacts(ProjectCompilationRequest request, string outputDirectory, ICollection<CompilationArtifactInfo> artifacts)
    {
        if (!IsExecutableProject(request))
        {
            return;
        }

        string assemblyFileName = request.InspectionResult.Project.AssemblyName;
        string? builderOutputDirectory = GetProjectBuildOutputDirectory(request);
        if (string.IsNullOrWhiteSpace(builderOutputDirectory) || !Directory.Exists(builderOutputDirectory))
        {
            return;
        }

        CopyArtifactIfExists(Path.Combine(builderOutputDirectory, assemblyFileName + ".deps.json"), outputDirectory, CompilationArtifactKind.RuntimeMetadata, artifacts);
        CopyArtifactIfExists(Path.Combine(builderOutputDirectory, assemblyFileName + ".runtimeconfig.json"), outputDirectory, CompilationArtifactKind.RuntimeMetadata, artifacts);
        CopyArtifactIfExists(Path.Combine(builderOutputDirectory, assemblyFileName + ".exe"), outputDirectory, CompilationArtifactKind.RuntimeHost, artifacts);
        CopyArtifactIfExists(Path.Combine(builderOutputDirectory, assemblyFileName), outputDirectory, CompilationArtifactKind.RuntimeHost, artifacts);
    }

    private static IReadOnlyList<BuildDiagnostic> EnsureExecutableBuildArtifacts(ProjectCompilationRequest request)
    {
        if (!IsExecutableProject(request))
        {
            return [];
        }

        string? builderOutputDirectory = GetProjectBuildOutputDirectory(request);
        if (string.IsNullOrWhiteSpace(builderOutputDirectory))
        {
            return [];
        }

        string assemblyName = request.InspectionResult.Project.AssemblyName;
        string depsJsonPath = Path.Combine(builderOutputDirectory, assemblyName + ".deps.json");
        string runtimeConfigPath = Path.Combine(builderOutputDirectory, assemblyName + ".runtimeconfig.json");
        if (File.Exists(depsJsonPath) && File.Exists(runtimeConfigPath))
        {
            return [];
        }

        ProcessStartInfo startInfo = new("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = request.InspectionResult.Project.ProjectDirectory,
        };

        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(request.InspectionResult.Project.ProjectFilePath);
        startInfo.ArgumentList.Add("--no-restore");

        if (!string.IsNullOrWhiteSpace(request.InspectionResult.Project.Configuration))
        {
            startInfo.ArgumentList.Add("-c");
            startInfo.ArgumentList.Add(request.InspectionResult.Project.Configuration);
        }

        if (!string.IsNullOrWhiteSpace(request.InspectionResult.Project.TargetFramework))
        {
            startInfo.ArgumentList.Add("-f");
            startInfo.ArgumentList.Add(request.InspectionResult.Project.TargetFramework);
        }

        if (!string.IsNullOrWhiteSpace(request.InspectionResult.Project.RuntimeIdentifier))
        {
            startInfo.ArgumentList.Add("-r");
            startInfo.ArgumentList.Add(request.InspectionResult.Project.RuntimeIdentifier);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet build for executable runtime artifacts.");

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        List<BuildDiagnostic> diagnostics = ParseExternalBuildDiagnostics(standardOutput, request.InspectionResult.Project.ProjectFilePath);
        diagnostics.AddRange(ParseExternalBuildDiagnostics(standardError, request.InspectionResult.Project.ProjectFilePath));

        if (process.ExitCode != 0
            && !diagnostics.Any(static diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error))
        {
            diagnostics.Add(CreateRuntimeArtifactBuildFailureDiagnostic(request.InspectionResult.Project.ProjectFilePath, standardOutput, standardError));
        }

        return diagnostics;
    }

    private static List<BuildDiagnostic> ParseExternalBuildDiagnostics(string buildOutput, string projectFilePath)
    {
        List<BuildDiagnostic> diagnostics = [];
        if (string.IsNullOrWhiteSpace(buildOutput))
        {
            return diagnostics;
        }

        using StringReader reader = new(buildOutput);
        while (reader.ReadLine() is { } line)
        {
            BuildDiagnostic? diagnostic = ParseExternalBuildDiagnostic(line, projectFilePath);
            if (diagnostic is not null)
            {
                diagnostics.Add(diagnostic);
            }
        }

        return diagnostics;
    }

    private static BuildDiagnostic? ParseExternalBuildDiagnostic(string line, string projectFilePath)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        Match match = ExternalBuildDiagnosticPattern.Match(line.Trim());
        if (!match.Success)
        {
            return null;
        }

        BuildDiagnosticSeverity severity = string.Equals(match.Groups["severity"].Value, "error", StringComparison.OrdinalIgnoreCase)
            ? BuildDiagnosticSeverity.Error
            : BuildDiagnosticSeverity.Warning;

        string code = match.Groups["code"].Value.Trim();
        string message = $"Target project build: {match.Groups["message"].Value.Trim()}";
        string? filePath = ResolveExternalDiagnosticFilePath(match.Groups["file"].Value, projectFilePath);

        return new BuildDiagnostic(
            severity,
            code,
            message,
            filePath,
            ParseNullableInt(match.Groups["line"].Value),
            ParseNullableInt(match.Groups["column"].Value));
    }

    private static string? ResolveExternalDiagnosticFilePath(string rawFilePath, string projectFilePath)
    {
        string filePath = rawFilePath.Trim();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return projectFilePath;
        }

        if (Path.IsPathRooted(filePath))
        {
            return filePath;
        }

        if (filePath.IndexOfAny([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]) >= 0)
        {
            string projectDirectory = Path.GetDirectoryName(projectFilePath) ?? Environment.CurrentDirectory;
            return Path.GetFullPath(Path.Combine(projectDirectory, filePath));
        }

        return projectFilePath;
    }

    private static int? ParseNullableInt(string value)
    {
        return int.TryParse(value, out int parsedValue)
            ? parsedValue
            : null;
    }

    private static BuildDiagnostic CreateRuntimeArtifactBuildFailureDiagnostic(string projectFilePath, string standardOutput, string standardError)
    {
        string detail = GetFirstNonEmptyLine(standardError) ?? GetFirstNonEmptyLine(standardOutput) ?? "The target project build did not produce a structured error line.";
        return new BuildDiagnostic(
            BuildDiagnosticSeverity.Error,
            "WB2002",
            $"Target project build failed while preparing executable runtime artifacts. {detail}",
            projectFilePath);
    }

    private static string? GetFirstNonEmptyLine(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        using StringReader reader = new(text);
        while (reader.ReadLine() is { } line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                return line.Trim();
            }
        }

        return null;
    }

    private static bool IsExecutableProject(ProjectCompilationRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.InspectionResult.Project.OutputType)
            && string.Equals(request.InspectionResult.Project.OutputType, "Exe", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetProjectBuildOutputDirectory(ProjectCompilationRequest request)
    {
        string? outputPath = request.InspectionResult.Project.OutputPath;
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return null;
        }

        return Path.IsPathRooted(outputPath)
            ? outputPath
            : Path.GetFullPath(Path.Combine(request.InspectionResult.Project.ProjectDirectory, outputPath));
    }

    private static void CopyArtifactIfExists(string sourcePath, string outputDirectory, CompilationArtifactKind artifactKind, ICollection<CompilationArtifactInfo> artifacts)
    {
        if (!File.Exists(sourcePath))
        {
            return;
        }

        string destinationPath = Path.Combine(outputDirectory, Path.GetFileName(sourcePath));
        if (!string.Equals(sourcePath, destinationPath, StringComparison.OrdinalIgnoreCase))
        {
            File.Copy(sourcePath, destinationPath, overwrite: true);
        }

        artifacts.Add(new CompilationArtifactInfo(artifactKind, destinationPath, File.Exists(destinationPath)));
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
