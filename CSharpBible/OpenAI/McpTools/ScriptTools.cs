using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol.Server;

namespace McpTools;

[McpServerToolType]
public sealed class ScriptTools
{
    private const string SourceDependenciesScript = @"C:\Projekte\CSharp\Tools\Skills\SourceDependencies\Get-SourceFileDependencies.ps1";
    private const string TestCoverageScript = @"C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1";

    [McpServerTool, Description("Lists direct dependencies for one or more C# source files.")]
    public async Task<string> GetSourceFileDependenciesAsync(
        [Description("One or more C# source file paths to analyze.")] string[] sourceFilePath,
        [Description("Optional workspace root used for dependency resolution.")] string? workspaceRoot = null,
        [Description("Skip scanning workspace files for matching declared types.")] bool skipWorkspaceScan = false,
        [Description("Include namespace, declared types, using namespaces, contexts, and resolution details in text mode.")] bool detailedText = false,
        [Description("Emit structured JSON instead of text.")] bool asJson = false)
    {
        var arguments = new List<string>
        {
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", SourceDependenciesScript
        };

        foreach (var path in sourceFilePath)
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

        return await RunPowerShellAsync(arguments);
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
        [Description("Emit structured JSON output.")] bool asJson = false)
    {
        var arguments = new List<string>
        {
            "-NoProfile",
            "-ExecutionPolicy", "Bypass",
            "-File", TestCoverageScript,
            "-TestProjectPath", testProjectPath,
            "-Configuration", configuration,
            "-RangeGapTolerance", rangeGapTolerance.ToString(),
            "-TopN", topN.ToString(),
            "-CoverageThreshold", coverageThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };

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
            foreach (var pattern in includeFilePathPatterns)
            {
                arguments.Add("-IncludeFilePathPatterns");
                arguments.Add(pattern);
            }
        }

        if (includeClassPatterns is { Length: > 0 })
        {
            foreach (var pattern in includeClassPatterns)
            {
                arguments.Add("-IncludeClassPatterns");
                arguments.Add(pattern);
            }
        }

        if (includeAssemblyPatterns is { Length: > 0 })
        {
            foreach (var pattern in includeAssemblyPatterns)
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

        return await RunPowerShellAsync(arguments);
    }

    private static async Task<string> RunPowerShellAsync(IEnumerable<string> arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
        var standardOutput = await process.StandardOutput.ReadToEndAsync();
        var standardError = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(standardError)
                ? $"PowerShell script failed with exit code {process.ExitCode}."
                : standardError.Trim());
        }

        return standardOutput;
    }
}
