using System;
using System.Threading.Tasks;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Loading;

namespace Workbench.Builder.Analysis;

/// <summary>
/// Coordinates argument parsing, project inspection, and output emission for the V1.1 analysis host.
/// </summary>
public sealed class AnalysisApplication
{
    private readonly AnalysisCommandLineParser _commandLineParser;
    private readonly IProjectInspectionService _projectInspectionService;
    private readonly IProjectInspectionFormatter _projectInspectionFormatter;
    private readonly IHostConsole _console;

    /// <summary>
    /// Initializes a new instance of <see cref="AnalysisApplication"/>.
    /// </summary>
    /// <param name="commandLineParser">The command-line parser.</param>
    /// <param name="projectInspectionService">The project inspection service.</param>
    /// <param name="projectInspectionFormatter">The inspection result formatter.</param>
    /// <param name="console">The console abstraction.</param>
    public AnalysisApplication(
        AnalysisCommandLineParser commandLineParser,
        IProjectInspectionService projectInspectionService,
        IProjectInspectionFormatter projectInspectionFormatter,
        IHostConsole console)
    {
        _commandLineParser = commandLineParser ?? throw new ArgumentNullException(nameof(commandLineParser));
        _projectInspectionService = projectInspectionService ?? throw new ArgumentNullException(nameof(projectInspectionService));
        _projectInspectionFormatter = projectInspectionFormatter ?? throw new ArgumentNullException(nameof(projectInspectionFormatter));
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
            AnalysisCommandOptions options = _commandLineParser.Parse(args);
            if (options.ShowHelp || string.IsNullOrWhiteSpace(options.ProjectFilePath))
            {
                _console.WriteLine(CreateUsageText());
                return Task.FromResult(options.ShowHelp ? HostExitCodes.Success : HostExitCodes.InvalidArguments);
            }

            string output = _projectInspectionFormatter.Format(
                _projectInspectionService.Inspect(new ProjectLoadRequest(options.ProjectFilePath)),
                options.OutputFormat);

            _console.WriteLine(output);
            return Task.FromResult(HostExitCodes.Success);
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
        return "Usage: Workbench.Builder.Analysis <project.csproj> [--format plain|json] [--help]";
    }
}
