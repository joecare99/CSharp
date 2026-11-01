using System;
using System.Diagnostics;
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

            if (!string.IsNullOrWhiteSpace(request.TargetFramework))
            {
                startInfo.ArgumentList.Add("--framework");
                startInfo.ArgumentList.Add(request.TargetFramework);
            }

            string filter = CreateTestFilter(request);
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
        return targets.Length == 0
            ? string.Empty
            : string.Join("|", targets.Select(target => $"FullyQualifiedName~{EscapeFilterValue(target)}"));
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

        foreach (XElement result in document.Descendants().Where(static element => element.Name.LocalName == "UnitTestResult"))
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

    private static int ReadCount(XElement counters, string name)
    {
        return int.TryParse((string?)counters.Attribute(name), out int value) ? value : 0;
    }
}
