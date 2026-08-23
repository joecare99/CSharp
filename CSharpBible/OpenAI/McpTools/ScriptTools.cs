using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ModelContextProtocol.Server;

namespace McpTools;

/// <summary>
/// Configures the PowerShell skill scripts exposed by the MCP tools.
/// </summary>
public sealed class McpToolsOptions
{
    /// <summary>
    /// Gets or sets the absolute path to the source-dependencies PowerShell script.
    /// Configure via "McpTools:SourceDependenciesScript" or the environment variable MCP_SOURCE_DEPENDENCIES_SCRIPT.
    /// </summary>
    public string SourceDependenciesScript { get; set; } =
        @"C:\Projekte\CSharp\Tools\Skills\SourceDependencies\Get-SourceFileDependencies.ps1";

    /// <summary>
    /// Gets or sets the absolute path to the test-coverage PowerShell script.
    /// Configure via "McpTools:TestCoverageScript" or the environment variable MCP_TEST_COVERAGE_SCRIPT.
    /// </summary>
    public string TestCoverageScript { get; set; } =
        @"C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1";

    /// <summary>
    /// Gets or sets the maximum PowerShell execution time in minutes.
    /// </summary>
    public int ExecutionTimeoutMinutes { get; set; } = 10;
}

[McpServerToolType]
public sealed class ScriptTools
{
    internal static readonly McpToolsOptions DefaultOptions = new();

    internal static Func<IEnumerable<string>, TimeSpan, CancellationToken, Task<string>> PowerShellExecutionRunner { get; set; } = RunPowerShellAsync;

    private readonly McpToolsOptions _options;

    public ScriptTools()
        : this(DefaultOptions)
    {
    }

    public ScriptTools(McpToolsOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [McpServerTool, Description("Lists direct dependencies for one or more C# source files.")]
    public async Task<string> GetSourceFileDependenciesAsync(
        [Description("One or more C# source file paths to analyze.")] string[] sourceFilePath,
        [Description("Optional workspace root used for dependency resolution.")] string? workspaceRoot = null,
        [Description("Skip scanning workspace files for matching declared types.")] bool skipWorkspaceScan = false,
        [Description("Include namespace, declared types, using namespaces, contexts, and resolution details in text mode.")] bool detailedText = false,
        [Description("Emit structured JSON instead of text.")] bool asJson = false,
        CancellationToken cancellationToken = default)
    {
        string scriptPath = ResolveScriptPath(
            _options.SourceDependenciesScript,
            "McpTools:SourceDependenciesScript",
            "MCP_SOURCE_DEPENDENCIES_SCRIPT");

        List<string> arguments =
        [
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", scriptPath,
        ];

        foreach (string path in sourceFilePath)
        {
            arguments.Add("-SourceFilePath");
            arguments.Add(path);
        }

        if (!string.IsNullOrWhiteSpace(workspaceRoot))
        {
            arguments.Add("-WorkspaceRoot");
            arguments.Add(workspaceRoot);
        }

        if (skipWorkspaceScan)
        {
            arguments.Add("-SkipWorkspaceScan");
        }

        if (detailedText)
        {
            arguments.Add("-DetailedText");
        }

        if (asJson)
        {
            arguments.Add("-AsJson");
        }

        return await PowerShellExecutionRunner(arguments, ExecutionTimeout, cancellationToken);
    }

    [McpServerTool, Description("Runs a test project with coverage and returns the coverage summary.")]
    public async Task<string> InvokeTestProjectCoverageAsync(
        [Description("Path to the test project file (.csproj).")] string testProjectPath,
        [Description("Build configuration to use.")] string configuration = "Debug",
        [Description("Optional target framework moniker to test.")] string? framework = null,
        [Description("Root output folder for test and coverage artifacts.")] string? resultsDirectory = null,
        [Description("Wildcard file-scope filters.")] string[]? includeFilePathPatterns = null,
        [Description("Wildcard class-name filters.")] string[]? includeClassPatterns = null,
        [Description("Wildcard assembly-name filters.")] string[]? includeAssemblyPatterns = null,
        [Description("Merge uncovered ranges even if small covered gaps exist between uncovered lines.")] int rangeGapTolerance = 1,
        [Description("Maximum number of classes returned after sorting by lowest coverage first.")] int topN = 50,
        [Description("Include only classes below this coverage percent.")] double coverageThreshold = 100,
        [Description("Disable Top-N limiting.")] bool disableTopNFilter = false,
        [Description("Disable coverage-threshold filtering.")] bool disableCoverageThresholdFilter = false,
        [Description("Include strict uncovered ranges as a drill-down view.")] bool includeStrictRanges = false,
        [Description("Pass --no-build to dotnet test.")] bool noBuild = false,
        [Description("Emit structured JSON output.")] bool asJson = false,
        CancellationToken cancellationToken = default)
    {
        string scriptPath = ResolveScriptPath(
            _options.TestCoverageScript,
            "McpTools:TestCoverageScript",
            "MCP_TEST_COVERAGE_SCRIPT");

        List<string> arguments =
        [
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", scriptPath,
            "-TestProjectPath", testProjectPath,
            "-Configuration", configuration,
            "-RangeGapTolerance", rangeGapTolerance.ToString(),
            "-TopN", topN.ToString(),
            "-CoverageThreshold", coverageThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ];

        if (!string.IsNullOrWhiteSpace(framework))
        {
            arguments.Add("-Framework");
            arguments.Add(framework);
        }

        if (!string.IsNullOrWhiteSpace(resultsDirectory))
        {
            arguments.Add("-ResultsDirectory");
            arguments.Add(resultsDirectory);
        }

        if (includeFilePathPatterns is { Length: > 0 })
        {
            foreach (string pattern in includeFilePathPatterns)
            {
                arguments.Add("-IncludeFilePathPatterns");
                arguments.Add(pattern);
            }
        }

        if (includeClassPatterns is { Length: > 0 })
        {
            foreach (string pattern in includeClassPatterns)
            {
                arguments.Add("-IncludeClassPatterns");
                arguments.Add(pattern);
            }
        }

        if (includeAssemblyPatterns is { Length: > 0 })
        {
            foreach (string pattern in includeAssemblyPatterns)
            {
                arguments.Add("-IncludeAssemblyPatterns");
                arguments.Add(pattern);
            }
        }

        if (disableTopNFilter)
        {
            arguments.Add("-DisableTopNFilter");
        }

        if (disableCoverageThresholdFilter)
        {
            arguments.Add("-DisableCoverageThresholdFilter");
        }

        if (includeStrictRanges)
        {
            arguments.Add("-IncludeStrictRanges");
        }

        if (noBuild)
        {
            arguments.Add("-NoBuild");
        }

        if (asJson)
        {
            arguments.Add("-AsJson");
        }

        return await PowerShellExecutionRunner(arguments, ExecutionTimeout, cancellationToken);
    }

    private TimeSpan ExecutionTimeout
        => TimeSpan.FromMinutes(Math.Max(1, _options.ExecutionTimeoutMinutes));

    private static string ResolveScriptPath(string configuredPath, string configurationKey, string environmentVariable)
    {
        if (!File.Exists(configuredPath))
        {
            throw new InvalidOperationException(
                $"The PowerShell script '{configuredPath}' does not exist. " +
                $"Configure it via '{configurationKey}' in appsettings.json or the environment variable '{environmentVariable}'.");
        }

        return configuredPath;
    }

    internal static async Task<string> RunPowerShellAsync(
        IEnumerable<string> arguments,
        TimeSpan executionTimeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (executionTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(executionTimeout));
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = "powershell.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using Process process = new()
        {
            StartInfo = startInfo,
        };

        process.Start();

        Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> standardErrorTask = process.StandardError.ReadToEndAsync();

        using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(executionTimeout);

        try
        {
            await process.WaitForExitAsync(timeoutCts.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
                // The process already exited between the timeout and the kill attempt.
            }

            throw new TimeoutException(
                $"The PowerShell script did not complete within {executionTimeout.TotalMinutes:0.##} minutes and was terminated.");
        }

        string standardOutput = await standardOutputTask;
        string standardError = await standardErrorTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(standardError)
                ? $"PowerShell script failed with exit code {process.ExitCode}."
                : standardError.Trim());
        }

        return standardOutput;
    }
}
