using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using Microsoft.Extensions.DependencyInjection;
using AA98_AvlnCodeStudio.Building.Workbench.DependencyInjection;

namespace AA98.Builder.Host;

/// <summary>
/// Provides the thin AA98 builder micro-host entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// Runs the builder micro-host.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The process exit code.</returns>
    public static async Task<int> Main(string[] args)
    {
        return await RunAsync(args).ConfigureAwait(false);
    }

    internal static async Task<int> RunAsync(
        IReadOnlyList<string> args,
        ICodeStudioBuilderService? builderService = null,
        TextWriter? standardOutput = null,
        TextWriter? standardError = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);

        standardOutput ??= Console.Out;
        standardError ??= Console.Error;

        if (args.Count < 2 || IsHelp(args[0]))
        {
            WriteUsage(standardOutput);
            return args.Count == 0 || IsHelp(args[0]) ? 0 : 2;
        }

        string command = args[0];
        string projectPath = args[1];
        string? configuration = GetOptionValue(args, "--configuration");
        string? targetFramework = GetOptionValue(args, "--framework");

        using ServiceProvider? serviceProvider = builderService is null
            ? new ServiceCollection()
                .AddWorkbenchCodeStudioBuilding()
                .BuildServiceProvider()
            : null;
        builderService ??= serviceProvider!.GetRequiredService<ICodeStudioBuilderService>();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.Equals(command, "inspect", StringComparison.OrdinalIgnoreCase))
            {
                BuilderProjectInspectionResult result = await builderService.InspectProjectAsync(new BuilderProjectInspectionRequest
                {
                    ProjectPath = projectPath,
                    Configuration = configuration,
                    TargetFramework = targetFramework,
                }, cancellationToken).ConfigureAwait(false);
                WriteInspectionResult(standardOutput, result);
                return HasErrorDiagnostics(result.Diagnostics) ? 1 : 0;
            }

            if (string.Equals(command, "build", StringComparison.OrdinalIgnoreCase))
            {
                BuilderBuildResult result = await builderService.BuildProjectAsync(new BuilderBuildRequest
                {
                    ProjectPath = projectPath,
                    Configuration = configuration,
                    TargetFramework = targetFramework,
                }, cancellationToken).ConfigureAwait(false);
                WriteBuildResult(standardOutput, result);
                return result.Succeeded ? 0 : 1;
            }

            standardError.WriteLine($"Unknown command: {command}");
            WriteUsage(standardOutput);
            return 2;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            standardError.WriteLine("AA98_BUILDER_HOST_ERROR");
            standardError.WriteLine($"type={ex.GetType().FullName}");
            standardError.WriteLine($"message={ex.Message}");
            return 1;
        }
    }

    private static void WriteUsage(TextWriter standardOutput)
    {
        standardOutput.WriteLine("AA98 Builder Host");
        standardOutput.WriteLine("Usage:");
        standardOutput.WriteLine("  AA98.Builder.Host inspect <project.csproj> [--configuration Debug] [--framework net10.0]");
        standardOutput.WriteLine("  AA98.Builder.Host build <project.csproj> [--configuration Debug] [--framework net10.0]");
    }

    private static void WriteInspectionResult(TextWriter standardOutput, BuilderProjectInspectionResult result)
    {
        standardOutput.WriteLine("AA98_BUILDER_INSPECTION_RESULT");
        standardOutput.WriteLine($"project={result.ProjectPath}");
        standardOutput.WriteLine($"name={result.ProjectName ?? string.Empty}");
        standardOutput.WriteLine($"targetFramework={result.TargetFramework ?? string.Empty}");
        standardOutput.WriteLine($"isTestProject={result.IsTestProject}");
        standardOutput.WriteLine($"compileItems={result.CompileItems.Count}");
        standardOutput.WriteLine($"projectReferences={result.ProjectReferences.Count}");
        standardOutput.WriteLine($"packageReferences={result.PackageReferences.Count}");
        standardOutput.WriteLine($"resolvedReferences={result.ResolvedReferences.Count}");
        WriteDiagnostics(standardOutput, result.Diagnostics);
    }

    private static void WriteBuildResult(TextWriter standardOutput, BuilderBuildResult result)
    {
        standardOutput.WriteLine("AA98_BUILDER_BUILD_RESULT");
        standardOutput.WriteLine($"project={result.ProjectPath}");
        standardOutput.WriteLine($"succeeded={result.Succeeded}");
        standardOutput.WriteLine($"artifacts={result.Artifacts.Count}");
        foreach (BuilderCompilationArtifact artifact in result.Artifacts)
        {
            standardOutput.WriteLine($"artifact={artifact.Kind}|{artifact.TargetFramework ?? string.Empty}|{artifact.Path}");
        }

        WriteDiagnostics(standardOutput, result.Diagnostics);
    }

    private static void WriteDiagnostics(TextWriter standardOutput, IEnumerable<BuilderDiagnostic> diagnostics)
    {
        BuilderDiagnostic[] diagnosticArray = diagnostics.ToArray();
        standardOutput.WriteLine($"diagnostics={diagnosticArray.Length}");
        foreach (BuilderDiagnostic diagnostic in diagnosticArray)
        {
            standardOutput.WriteLine($"diagnostic={diagnostic.Severity}|{diagnostic.Code}|{diagnostic.FilePath ?? string.Empty}|{diagnostic.LineNumber?.ToString() ?? string.Empty}|{diagnostic.ColumnNumber?.ToString() ?? string.Empty}|{diagnostic.Message}");
        }
    }

    private static bool HasErrorDiagnostics(IEnumerable<BuilderDiagnostic> diagnostics)
    {
        return diagnostics.Any(static diagnostic => diagnostic.Severity == BuilderDiagnosticSeverity.Error);
    }

    private static bool IsHelp(string value)
    {
        return string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "help", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetOptionValue(IReadOnlyList<string> args, string optionName)
    {
        for (int index = 2; index < args.Count - 1; index++)
        {
            if (string.Equals(args[index], optionName, StringComparison.OrdinalIgnoreCase))
            {
                return args[index + 1];
            }
        }

        return null;
    }
}
