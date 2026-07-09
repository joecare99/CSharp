using System;
using System.Linq;
using System.Threading.Tasks;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Formatting;

namespace Workbench.Builder.Host;

/// <summary>
/// Coordinates argument parsing, project inspection, compilation, and output emission for the V1.2 compile host.
/// </summary>
public sealed class HostApplication
{
    private readonly HostCommandLineParser _commandLineParser;
    private readonly IProjectCompilationService _projectCompilationService;
    private readonly IProjectInspectionService _projectInspectionService;
    private readonly IHostConsole _console;

    /// <summary>
    /// Initializes a new instance of <see cref="HostApplication"/>.
    /// </summary>
    /// <param name="commandLineParser">The command-line parser.</param>
    /// <param name="projectCompilationService">The project compilation service.</param>
    /// <param name="projectInspectionService">The project inspection service.</param>
    /// <param name="console">The console abstraction.</param>
    public HostApplication(
        HostCommandLineParser commandLineParser,
        IProjectCompilationService projectCompilationService,
        IProjectInspectionService projectInspectionService,
        IHostConsole console)
    {
        _commandLineParser = commandLineParser ?? throw new ArgumentNullException(nameof(commandLineParser));
        _projectCompilationService = projectCompilationService ?? throw new ArgumentNullException(nameof(projectCompilationService));
        _projectInspectionService = projectInspectionService ?? throw new ArgumentNullException(nameof(projectInspectionService));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    /// <summary>
    /// Executes the host application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The process exit code.</returns>
    public Task<int> RunAsync(string[] args)
    {
        try
        {
            HostCommandOptions options = _commandLineParser.Parse(args);
            if (options.ShowHelp || string.IsNullOrWhiteSpace(options.ProjectFilePath))
            {
                _console.WriteLine(CreateUsageText());
                return Task.FromResult(options.ShowHelp ? HostExitCodes.Success : HostExitCodes.InvalidArguments);
            }

            WriteProgress(options, $"Inspecting project '{options.ProjectFilePath}'.");
            ProjectInspectionResult inspectionResult = _projectInspectionService.Inspect(new ProjectLoadRequest(options.ProjectFilePath));
            WriteProgress(options, $"Resolved target framework '{inspectionResult.Project.TargetFramework}' for assembly '{inspectionResult.Project.AssemblyName}'.");
            WriteProgress(options, $"Compiling project to '{options.OutputDirectory ?? inspectionResult.Project.ProjectDirectory}'.");
            ProjectCompilationResult compilationResult = _projectCompilationService.Compile(
                new ProjectCompilationRequest(inspectionResult, options.OutputDirectory, emitPortablePdb: true));

            foreach (BuildDiagnostic diagnostic in compilationResult.Diagnostics.Where(static diagnostic => diagnostic.Severity != BuildDiagnosticSeverity.Error))
            {
                _console.WriteLine(BuildDiagnosticTextFormatter.Format(diagnostic));
            }

            if (compilationResult.Succeeded)
            {
                _console.WriteLine(CreateEmitSuccessText(compilationResult));
                return Task.FromResult(HostExitCodes.Success);
            }

            foreach (BuildDiagnostic diagnostic in compilationResult.Diagnostics.Where(static diagnostic => diagnostic.Severity == BuildDiagnosticSeverity.Error))
            {
                _console.WriteErrorLine(BuildDiagnosticTextFormatter.Format(diagnostic));
            }

            return Task.FromResult(HostExitCodes.ExecutionFailed);
        }
        catch (ArgumentException exception)
        {
            _console.WriteErrorLine(exception.Message);
            _console.WriteLine(CreateUsageText());
            return Task.FromResult(HostExitCodes.InvalidArguments);
        }
        catch (Exception exception)
        {
            _console.WriteErrorLine(exception.ToString());
            return Task.FromResult(HostExitCodes.ExecutionFailed);
        }
    }

    internal static string CreateUsageText()
    {
        return "Usage: Workbench.Builder.Host <project.csproj> [--output <directory>] [--verbosity normal|detailed] [--help]";
    }

    private void WriteProgress(HostCommandOptions options, string text)
    {
        if (options.Verbosity < HostVerbosity.Detailed)
        {
            return;
        }

        _console.WriteLine($"[host] {text}");
    }

    private static string CreateEmitSuccessText(ProjectCompilationResult compilationResult)
    {
        string artifactLines = string.Join(
            Environment.NewLine,
            compilationResult.Artifacts.Select(static artifact => $"- {artifact.Kind}: {artifact.FilePath}"));

        return $"Emit succeeded for '{compilationResult.InspectionResult.Project.AssemblyName}'.{Environment.NewLine}{artifactLines}";
    }
}
