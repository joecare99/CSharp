using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McpTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PeripheralProduction.Tests;

[TestClass]
[DoNotParallelize]
public sealed class McpToolsTests
{
    [TestMethod]
    public async Task GetSourceFileDependenciesAsync_BuildsEveryOptionalArgument()
    {
        IReadOnlyList<string>? capturedArguments = null;
        ScriptTools.PowerShellExecutionRunner = (arguments, timeout, _) =>
        {
            capturedArguments = [.. arguments];
            return Task.FromResult("dependencies");
        };

        string result = await new ScriptTools().GetSourceFileDependenciesAsync(
            ["first.cs", "second.cs"],
            "C:\\workspace",
            skipWorkspaceScan: true,
            detailedText: true,
            asJson: true);

        Assert.AreEqual("dependencies", result);
        CollectionAssert.AreEqual(
        new[]
        {
            "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", @"C:\Projekte\CSharp\Tools\Skills\SourceDependencies\Get-SourceFileDependencies.ps1",
            "-SourceFilePath", "first.cs", "-SourceFilePath", "second.cs",
            "-WorkspaceRoot", @"C:\workspace", "-SkipWorkspaceScan", "-DetailedText", "-AsJson",
        },
        (System.Collections.ICollection)capturedArguments!);
    }

    [TestMethod]
    public async Task GetSourceFileDependenciesAsync_SkipsOptionalArguments()
    {
        IReadOnlyList<string>? capturedArguments = null;
        ScriptTools.PowerShellExecutionRunner = (arguments, timeout, _) =>
        {
            capturedArguments = [.. arguments];
            return Task.FromResult(string.Empty);
        };

        await new ScriptTools().GetSourceFileDependenciesAsync(["single.cs"], " ");

        CollectionAssert.AreEqual(
        new[]
        {
            "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", @"C:\Projekte\CSharp\Tools\Skills\SourceDependencies\Get-SourceFileDependencies.ps1",
            "-SourceFilePath", "single.cs",
        },
        (System.Collections.ICollection)capturedArguments!);
    }

    [TestMethod]
    public async Task InvokeTestProjectCoverageAsync_BuildsEveryOptionalArgument()
    {
        IReadOnlyList<string>? capturedArguments = null;
        ScriptTools.PowerShellExecutionRunner = (arguments, timeout, _) =>
        {
            capturedArguments = [.. arguments];
            return Task.FromResult("coverage");
        };

        string result = await new ScriptTools().InvokeTestProjectCoverageAsync(
            "sample.tests.csproj",
            configuration: "Release",
            framework: "net10.0",
            resultsDirectory: "results",
            includeFilePathPatterns: ["*.cs"],
            includeClassPatterns: ["*.Tests"],
            includeAssemblyPatterns: ["Sample"],
            rangeGapTolerance: 2,
            topN: 5,
            coverageThreshold: 12.5,
            disableTopNFilter: true,
            disableCoverageThresholdFilter: true,
            includeStrictRanges: true,
            noBuild: true,
            asJson: true);

        Assert.AreEqual("coverage", result);
        CollectionAssert.AreEqual(
        new[]
        {
            "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", @"C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1",
            "-TestProjectPath", "sample.tests.csproj", "-Configuration", "Release", "-RangeGapTolerance", "2", "-TopN", "5", "-CoverageThreshold", "12.5",
            "-Framework", "net10.0", "-ResultsDirectory", "results",
            "-IncludeFilePathPatterns", "*.cs", "-IncludeClassPatterns", "*.Tests", "-IncludeAssemblyPatterns", "Sample",
            "-DisableTopNFilter", "-DisableCoverageThresholdFilter", "-IncludeStrictRanges", "-NoBuild", "-AsJson",
        },
        (System.Collections.ICollection)capturedArguments!);
    }

    [TestMethod]
    public async Task InvokeTestProjectCoverageAsync_SkipsOptionalArguments()
    {
        IReadOnlyList<string>? capturedArguments = null;
        ScriptTools.PowerShellExecutionRunner = (arguments, timeout, _) =>
        {
            capturedArguments = [.. arguments];
            return Task.FromResult(string.Empty);
        };

        await new ScriptTools().InvokeTestProjectCoverageAsync("sample.tests.csproj");

        CollectionAssert.AreEqual(
        new[]
        {
            "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", @"C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1",
            "-TestProjectPath", "sample.tests.csproj", "-Configuration", "Debug", "-RangeGapTolerance", "1", "-TopN", "50", "-CoverageThreshold", "100",
        },
        (System.Collections.ICollection)capturedArguments!);
    }

    [TestMethod]
    public async Task RunPowerShellAsync_ReturnsStandardOutputForSuccessfulProcess()
    {
        string output = await ScriptTools.RunPowerShellAsync(
        [
            "-NoProfile",
            "-Command",
            "[Console]::Out.Write('deterministic output'); [Console]::Error.Write('diagnostic')",
        ],
        TimeSpan.FromMinutes(1));

        Assert.AreEqual("deterministic output", output);
    }

    [TestMethod]
    public async Task RunPowerShellAsync_ThrowsStandardErrorForFailedProcess()
    {
        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => ScriptTools.RunPowerShellAsync(["-NoProfile", "-Command", "[Console]::Error.Write('expected failure'); exit 7"], TimeSpan.FromMinutes(1)));

        StringAssert.Contains(exception.Message, "expected failure");
    }

    [TestMethod]
    public async Task RunPowerShellAsync_UsesExitCodeWhenStandardErrorIsEmpty()
    {
        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => ScriptTools.RunPowerShellAsync(["-NoProfile", "-Command", "exit 9"], TimeSpan.FromMinutes(1)));

        StringAssert.Contains(exception.Message, "exit code 9");
    }

    [TestMethod]
    public async Task RunPowerShellAsync_TerminatesProcessOnTimeout()
    {
        TimeoutException exception = await Assert.ThrowsExactlyAsync<TimeoutException>(
            () => ScriptTools.RunPowerShellAsync(
                ["-NoProfile", "-Command", "Start-Sleep -Seconds 30"],
                TimeSpan.FromMilliseconds(200)));

        StringAssert.Contains(exception.Message, "terminated");
    }

    [TestMethod]
    public void ResolveMcpToolsOptions_PrefersConfigurationThenEnvironmentThenDefaults()
    {
        Microsoft.Extensions.Configuration.IConfiguration configuration = new StubConfiguration(new Dictionary<string, string?>
        {
            ["McpTools:SourceDependenciesScript"] = @"C:\configured\dependencies.ps1",
            ["McpTools:ExecutionTimeoutMinutes"] = "5",
        });
        Environment.SetEnvironmentVariable("MCP_SOURCE_DEPENDENCIES_SCRIPT", @"C:\env\dependencies.ps1");
        Environment.SetEnvironmentVariable("MCP_TEST_COVERAGE_SCRIPT", @"C:\env\coverage.ps1");
        try
        {
            McpToolsOptions options = global::Program.ResolveMcpToolsOptions(configuration);

            Assert.AreEqual(@"C:\configured\dependencies.ps1", options.SourceDependenciesScript);
            Assert.AreEqual(@"C:\env\coverage.ps1", options.TestCoverageScript);
            Assert.AreEqual(5, options.ExecutionTimeoutMinutes);
        }
        finally
        {
            Environment.SetEnvironmentVariable("MCP_SOURCE_DEPENDENCIES_SCRIPT", null);
            Environment.SetEnvironmentVariable("MCP_TEST_COVERAGE_SCRIPT", null);
        }
    }

    [TestMethod]
    public void ResolveMcpToolsOptions_FallsBackToDefaultsWhenUnset()
    {
        McpToolsOptions options = global::Program.ResolveMcpToolsOptions(
            new StubConfiguration([]));

        Assert.AreEqual(@"C:\Projekte\CSharp\Tools\Skills\SourceDependencies\Get-SourceFileDependencies.ps1", options.SourceDependenciesScript);
        Assert.AreEqual(@"C:\Projekte\CSharp\Tools\Skills\TestCoverage\Invoke-TestProjectCoverage.ps1", options.TestCoverageScript);
        Assert.AreEqual(10, options.ExecutionTimeoutMinutes);
    }

    private sealed class StubConfiguration(Dictionary<string, string?> values)
        : Microsoft.Extensions.Configuration.IConfiguration
    {
        public string? this[string key]
        {
            get => values.GetValueOrDefault(key);
            set => values[key] = value;
        }

        public IEnumerable<Microsoft.Extensions.Configuration.IConfigurationSection> GetChildren() => [];

        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken()
            => throw new NotSupportedException("Not required for this stub.");

        public Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key)
            => throw new NotSupportedException("Not required for this stub.");
    }

    [TestMethod]
    public async Task Program_Main_UsesDeterministicApplicationRunner()
    {
        bool runnerInvoked = false;
        global::Program.ApplicationRunner = application =>
        {
            runnerInvoked = true;
            Assert.IsNotNull(application.Services);
            return Task.CompletedTask;
        };

        await global::Program.Main([]);

        Assert.IsTrue(runnerInvoked);
    }

    [TestMethod]
    public async Task RunApplicationAsync_ExitsWhenTheApplicationLifetimeIsAlreadyStopped()
    {
        await using WebApplication application = global::Program.CreateApplication([]);
        application.Lifetime.StopApplication();

        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => global::Program.RunApplicationAsync(application));
    }
}
