#if NET10_0
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies process and redirected-stream lifecycle behavior of the Git command runner.
/// </summary>
[TestClass]
public sealed class GitCommandRunnerTests
{
    [TestMethod]
    [Timeout(30000)]
    public async Task RunAsync_DrainsLargeStandardOutputAndStandardErrorConcurrently()
    {
        EnsureWindows();
        string testDirectory = CreateTestDirectory();
        try
        {
            string scriptPath = Path.Combine(testDirectory, "large-output.ps1");
            string script = """
                for ($i = 0; $i -lt 12000; $i++) {
                    [Console]::Out.WriteLine('stdout-payload-' + $i.ToString('D6') + '-0123456789012345678901234567890123456789012345678901234567890123456789')
                    [Console]::Error.WriteLine('stderr-payload-' + $i.ToString('D6') + '-0123456789012345678901234567890123456789012345678901234567890123456789')
                }
                """;
            await File.WriteAllTextAsync(scriptPath, script).ConfigureAwait(false);

            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(20));
            GitCommandResult result = await new GitCommandRunner(GetPowerShellPath())
                .RunAsync(testDirectory, BuildPowerShellArguments(scriptPath), timeout.Token)
                .ConfigureAwait(false);

            Assert.AreEqual(0, result.ExitCode);
            Assert.IsTrue(result.StandardOutput.Length > 500_000, $"stdout length was {result.StandardOutput.Length}.");
            Assert.IsTrue(result.StandardError.Length > 500_000, $"stderr length was {result.StandardError.Length}.");
            StringAssert.Contains(result.StandardOutput, "stdout-payload-");
            StringAssert.Contains(result.StandardError, "stderr-payload-");
        }
        finally
        {
            DeleteTestDirectory(testDirectory);
        }
    }

    [TestMethod]
    [Timeout(30000)]
    public async Task RunAsync_CancellationTerminatesChildProcessTreeAndReapsParent()
    {
        EnsureWindows();
        string testDirectory = CreateTestDirectory();
        string readyPath = Path.Combine(testDirectory, "parent-ready.txt");
        string childStartedPath = Path.Combine(testDirectory, "child-started.txt");
        string childCompletedPath = Path.Combine(testDirectory, "child-completed.txt");
        using CancellationTokenSource cancellation = new();
        Task<GitCommandResult>? runTask = null;
        try
        {
            string childScriptPath = Path.Combine(testDirectory, "child.ps1");
            string parentScriptPath = Path.Combine(testDirectory, "parent.ps1");
            await File.WriteAllTextAsync(
                childScriptPath,
                $"[IO.File]::WriteAllText('{childStartedPath}', 'started'){Environment.NewLine}Start-Sleep -Seconds 6{Environment.NewLine}[IO.File]::WriteAllText('{childCompletedPath}', 'completed'){Environment.NewLine}")
                .ConfigureAwait(false);
            await File.WriteAllTextAsync(
                parentScriptPath,
                $"$startInfo = [System.Diagnostics.ProcessStartInfo]::new(){Environment.NewLine}$startInfo.FileName = '{GetPowerShellPath()}'{Environment.NewLine}$startInfo.Arguments = '{BuildPowerShellArguments(childScriptPath)}'{Environment.NewLine}$startInfo.UseShellExecute = $false{Environment.NewLine}$startInfo.CreateNoWindow = $true{Environment.NewLine}[System.Diagnostics.Process]::Start($startInfo) | Out-Null{Environment.NewLine}[IO.File]::WriteAllText('{readyPath}', 'ready'){Environment.NewLine}while ($true) {{ Start-Sleep -Seconds 1 }}{Environment.NewLine}")
                .ConfigureAwait(false);

            runTask = new GitCommandRunner(GetPowerShellPath())
                .RunAsync(testDirectory, BuildPowerShellArguments(parentScriptPath), cancellation.Token);

            await WaitForFileAsync(readyPath, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            await WaitForFileAsync(childStartedPath, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            cancellation.Cancel();

            try
            {
                await runTask.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                Assert.Fail("The command runner completed instead of observing cancellation.");
            }
            catch (OperationCanceledException)
            {
            }

            await Task.Delay(TimeSpan.FromSeconds(7)).ConfigureAwait(false);
            Assert.IsFalse(File.Exists(childCompletedPath), "A descendant process continued after cancellation.");
        }
        finally
        {
            cancellation.Cancel();
            if (runTask is not null)
            {
                try
                {
                    await runTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            DeleteTestDirectory(testDirectory);
        }
    }

    private static string CreateTestDirectory()
    {
        string testDirectory = Path.Combine(
            Environment.CurrentDirectory,
            ".git-command-runner-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testDirectory);
        return testDirectory;
    }

    private static void DeleteTestDirectory(string testDirectory)
    {
        Directory.Delete(testDirectory, recursive: true);
        string parentDirectory = Path.GetDirectoryName(testDirectory)!;
        if (Directory.Exists(parentDirectory) && Directory.GetFileSystemEntries(parentDirectory).Length == 0)
        {
            Directory.Delete(parentDirectory);
        }
    }

    private static string GetPowerShellPath() => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        "System32",
        "WindowsPowerShell",
        "v1.0",
        "powershell.exe");

    private static string BuildPowerShellArguments(string scriptPath) => $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"";

    private static void EnsureWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("The process fixture currently targets Windows.");
        }
    }

    private static async Task WaitForFileAsync(string path, TimeSpan timeout)
    {
        DateTime deadline = DateTime.UtcNow + timeout;
        while (!File.Exists(path))
        {
            if (DateTime.UtcNow >= deadline)
            {
                Assert.Fail($"Timed out waiting for {Path.GetFileName(path)}.");
            }

            await Task.Delay(50).ConfigureAwait(false);
        }
    }
}
#endif
