using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AA98_AvlnCodeStudio.Base.Testing.Models;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

/// <summary>
/// Executes .NET tests through the installed dotnet SDK and summarizes TRX results.
/// </summary>
public sealed class DotNetTestExecutionService : ITestExecutionService
{
    /// <inheritdoc/>
    public async Task<TestRunSummary> RunTestsAsync(
        TestRunRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.ProjectPath))
        {
            throw new ArgumentException("A solution or project path is required.", nameof(request));
        }

        string projectPath = Path.GetFullPath(request.ProjectPath);
        if (!File.Exists(projectPath))
        {
            throw new FileNotFoundException("The test solution or project does not exist.", projectPath);
        }

        string workingDirectory = string.IsNullOrWhiteSpace(request.WorkspaceRootPath)
            ? Path.GetDirectoryName(projectPath)!
            : Path.GetFullPath(request.WorkspaceRootPath);
        if (!Directory.Exists(workingDirectory))
        {
            throw new DirectoryNotFoundException($"The test workspace directory '{workingDirectory}' does not exist.");
        }

        string filter = CreateTestFilter(request);
        string resultsDirectory = Path.Combine(Path.GetTempPath(), $"AA98_TestResults_{Guid.NewGuid():N}");
        Directory.CreateDirectory(resultsDirectory);
        try
        {
            ProcessStartInfo startInfo = new("dotnet")
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            ClearInheritedMsBuildEnvironment(startInfo);
            startInfo.ArgumentList.Add("test");
            startInfo.ArgumentList.Add(projectPath);
            startInfo.ArgumentList.Add("--logger");
            startInfo.ArgumentList.Add("trx");
            startInfo.ArgumentList.Add("--results-directory");
            startInfo.ArgumentList.Add(resultsDirectory);

            if (!string.IsNullOrWhiteSpace(request.Configuration))
            {
                startInfo.ArgumentList.Add("--configuration");
                startInfo.ArgumentList.Add(request.Configuration);
            }

            if (!string.IsNullOrWhiteSpace(request.TargetFramework))
            {
                startInfo.ArgumentList.Add("--framework");
                startInfo.ArgumentList.Add(request.TargetFramework);
            }

            if (filter.Length > 0)
            {
                startInfo.ArgumentList.Add("--filter");
                startInfo.ArgumentList.Add(filter);
            }

            using Process process = new()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };
            if (!process.Start())
            {
                throw new InvalidOperationException("The dotnet test process could not be started.");
            }

            Task<string> standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            Task<string> standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);
            try
            {
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }

                await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
                throw;
            }

            string standardOutput = await standardOutputTask.ConfigureAwait(false);
            string standardError = await standardErrorTask.ConfigureAwait(false);
            string[] trxFiles = Directory.GetFiles(resultsDirectory, "*.trx", SearchOption.AllDirectories);
            if (trxFiles.Length == 0)
            {
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"dotnet test failed with exit code {process.ExitCode}.{Environment.NewLine}"
                        + standardOutput + Environment.NewLine + standardError);
                }

                return new TestRunSummary();
            }

            TestRunSummary summary = new();
            foreach (string trxFile in trxFiles)
            {
                AddTrxResults(summary, trxFile);
            }

            summary.Outcome = summary.FailedCount > 0
                ? summary.PassedCount > 0 ? TestRunOutcome.Partial : TestRunOutcome.Failed
                : summary.TotalCount == 0
                    ? TestRunOutcome.Unknown
                    : summary.SkippedCount > 0 ? TestRunOutcome.Partial : TestRunOutcome.Passed;
            return summary;
        }
        finally
        {
            Directory.Delete(resultsDirectory, recursive: true);
        }
    }

    private static string CreateTestFilter(TestRunRequest request)
    {
        string[] targets = request.Targets
            .Where(static target => !string.IsNullOrWhiteSpace(target))
            .Select(static target => target.Trim())
            .ToArray();
        if (targets.Length == 0)
        {
            return request.MatchTargetsExactly
                ? throw new ArgumentException("At least one exact test target is required.", nameof(request))
                : string.Empty;
        }

        string comparison = request.MatchTargetsExactly ? "=" : "~";
        return string.Join("|", targets.Select(target => $"FullyQualifiedName{comparison}{EscapeFilterValue(target)}"));
    }

    private static void ClearInheritedMsBuildEnvironment(ProcessStartInfo startInfo)
    {
        startInfo.Environment.Remove("MSBuildSDKsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath");
        startInfo.Environment.Remove("MSBuildExtensionsPath32");
        startInfo.Environment.Remove("MSBuildToolsPath");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
        startInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
    }

    private static string EscapeFilterValue(string value)
    {
        return string.Concat(value.Select(character =>
            character is '\\' or '&' or '|' or '=' or '!' or '~' or '(' or ')'
                ? "\\" + character
                : character.ToString()));
    }

    private static void AddTrxResults(TestRunSummary summary, string trxFile)
    {
        XDocument document = XDocument.Load(trxFile);
        XElement[] results = document.Descendants()
            .Where(static element => element.Name.LocalName == "UnitTestResult")
            .ToArray();
        var definitions = document.Descendants()
            .Where(static element => element.Name.LocalName == "UnitTest" && element.Attribute("id") is not null)
            .GroupBy(static element => (string)element.Attribute("id")!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(static group => group.Key, static group => group.First(), StringComparer.OrdinalIgnoreCase);
        foreach (XElement result in results)
        {
            string? testId = (string?)result.Attribute("testId");
            string? className = null;
            string? qualifiedTestName = null;
            if (testId is not null && definitions.TryGetValue(testId, out XElement? definition))
            {
                XElement? method = definition.Elements().FirstOrDefault(static element => element.Name.LocalName == "TestMethod");
                className = ((string?)method?.Attribute("className"))?.Split(',')[0].Trim();
                string? methodName = (string?)method?.Attribute("name");
                if (!string.IsNullOrWhiteSpace(className) && !string.IsNullOrWhiteSpace(methodName))
                {
                    qualifiedTestName = className + "." + methodName;
                }
            }

            string? durationText = (string?)result.Attribute("duration");
            TimeSpan? duration = TimeSpan.TryParse(durationText, CultureInfo.InvariantCulture, out TimeSpan parsed)
                ? parsed
                : null;
            summary.Cases.Add(new TestCaseResult(
                (string?)result.Attribute("testName") ?? string.Empty,
                (string?)result.Attribute("outcome") ?? string.Empty,
                duration,
                className,
                qualifiedTestName,
                testId,
                (string?)result.Attribute("executionId")));
        }
        AddTrxFailures(summary, results);

        XElement? counters = document.Descendants().FirstOrDefault(static element => element.Name.LocalName == "Counters");
        if (counters is not null)
        {
            int total = ReadCount(counters, "total");
            int passed = ReadCount(counters, "passed");
            int failed = ReadCount(counters, "failed");
            int aborted = ReadCount(counters, "aborted");
            int error = ReadCount(counters, "error");
            int timeout = ReadCount(counters, "timeout");
            int notRunnable = ReadCount(counters, "notRunnable");
            summary.TotalCount += total;
            summary.PassedCount += passed;
            summary.FailedCount += failed + aborted + error + timeout + notRunnable;
            summary.SkippedCount += Math.Max(0, total - passed - failed - aborted - error - timeout - notRunnable);

            return;
        }

        foreach (XElement result in results)
        {
            summary.TotalCount++;
            string? outcome = (string?)result.Attribute("outcome");
            switch (outcome)
            {
                case "Passed":
                    summary.PassedCount++;
                    break;
                case "Failed":
                case "Aborted":
                case "Error":
                case "Timeout":
                case "NotRunnable":
                    summary.FailedCount++;
                    break;
                default:
                    summary.SkippedCount++;
                    break;
            }
        }

    }

    private static void AddTrxFailures(TestRunSummary summary, IEnumerable<XElement> results)
    {
        foreach (XElement result in results)
        {
            string? outcome = (string?)result.Attribute("outcome");
            if (outcome is not ("Failed" or "Aborted" or "Error" or "Timeout" or "NotRunnable"))
            {
                continue;
            }

            XElement? errorInfo = result.Descendants()
                .FirstOrDefault(static element => element.Name.LocalName == "ErrorInfo");
            string? message = errorInfo?.Elements()
                .FirstOrDefault(static element => element.Name.LocalName == "Message")
                ?.Value;
            string? stackTrace = errorInfo?.Elements()
                .FirstOrDefault(static element => element.Name.LocalName == "StackTrace")
                ?.Value;
            (string? sourcePath, int? lineNumber) = TryGetSourceLocation(stackTrace);

            summary.Failures.Add(new TestRunFailure
            {
                TestName = (string?)result.Attribute("testName") ?? string.Empty,
                Message = message ?? string.Empty,
                StackTrace = stackTrace,
                SourcePath = sourcePath,
                LineNumber = lineNumber,
            });
        }
    }

    private static (string? SourcePath, int? LineNumber) TryGetSourceLocation(string? stackTrace)
    {
        if (string.IsNullOrWhiteSpace(stackTrace))
        {
            return (null, null);
        }

        foreach (string stackLine in stackTrace.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            int lineMarker = stackLine.LastIndexOf(":line ", StringComparison.Ordinal);
            if (lineMarker < 0
                || !int.TryParse(
                    stackLine[(lineMarker + 6)..].Trim(),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int lineNumber)
                || lineNumber <= 0)
            {
                continue;
            }

            string location = stackLine[..lineMarker];
            int pathMarker = location.IndexOf(" in ", StringComparison.Ordinal);
            if (pathMarker < 0)
            {
                continue;
            }

            string sourcePath = location[(pathMarker + 4)..].Trim();
            if (sourcePath.Length > 0)
            {
                return (sourcePath, lineNumber);
            }
        }

        return (null, null);
    }

    private static int ReadCount(XElement counters, string name)
    {
        return int.TryParse((string?)counters.Attribute(name), out int value) ? value : 0;
    }
}
