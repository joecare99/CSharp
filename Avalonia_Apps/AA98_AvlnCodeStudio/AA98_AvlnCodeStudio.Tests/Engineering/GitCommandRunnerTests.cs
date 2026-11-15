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
        EnsureSupportedPlatform();
        string testDirectory = CreateTestDirectory();
        try
        {
            bool windows = OperatingSystem.IsWindows();
            string scriptPath = Path.Combine(testDirectory, windows ? "large-output.ps1" : "large-output.sh");
            string script = windows ? """
                for ($i = 0; $i -lt 12000; $i++) {
                    [Console]::Out.WriteLine('stdout-payload-' + $i.ToString('D6') + '-0123456789012345678901234567890123456789012345678901234567890123456789')
                    [Console]::Error.WriteLine('stderr-payload-' + $i.ToString('D6') + '-0123456789012345678901234567890123456789012345678901234567890123456789')
                }
                """ : """
                i=0
                while [ "$i" -lt 12000 ]; do
                    printf 'stdout-payload-%06d-0123456789012345678901234567890123456789012345678901234567890123456789\n' "$i"
                    printf 'stderr-payload-%06d-0123456789012345678901234567890123456789012345678901234567890123456789\n' "$i" >&2
                    i=$((i + 1))
                done
                """;
            await File.WriteAllTextAsync(scriptPath, windows ? script : script.Replace("\r\n", "\n")).ConfigureAwait(false);

            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(20));
            GitCommandResult result = await new GitCommandRunner(GetShellPath())
                .RunAsync(testDirectory, BuildShellArguments(scriptPath), timeout.Token)
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
        EnsureSupportedPlatform();
        string testDirectory = CreateTestDirectory();
        string readyPath = Path.Combine(testDirectory, "parent-ready.txt");
        string childStartedPath = Path.Combine(testDirectory, "child-started.txt");
        string childCompletedPath = Path.Combine(testDirectory, "child-completed.txt");
        using CancellationTokenSource cancellation = new();
        Task<GitCommandResult>? runTask = null;
        try
        {
            bool windows = OperatingSystem.IsWindows();
            string childScriptPath = Path.Combine(testDirectory, windows ? "child.ps1" : "child.sh");
            string parentScriptPath = Path.Combine(testDirectory, windows ? "parent.ps1" : "parent.sh");
            await File.WriteAllTextAsync(
                childScriptPath,
                windows
                    ? $"[IO.File]::WriteAllText('{childStartedPath}', 'started'){Environment.NewLine}Start-Sleep -Seconds 6{Environment.NewLine}[IO.File]::WriteAllText('{childCompletedPath}', 'completed'){Environment.NewLine}"
                    : "printf started > child-started.txt\nsleep 6\nprintf completed > child-completed.txt\n")
                .ConfigureAwait(false);
            await File.WriteAllTextAsync(
                parentScriptPath,
                windows
                    ? $"$startInfo = [System.Diagnostics.ProcessStartInfo]::new(){Environment.NewLine}$startInfo.FileName = '{GetShellPath()}'{Environment.NewLine}$startInfo.Arguments = '{BuildShellArguments(childScriptPath)}'{Environment.NewLine}$startInfo.UseShellExecute = $false{Environment.NewLine}$startInfo.CreateNoWindow = $true{Environment.NewLine}[System.Diagnostics.Process]::Start($startInfo) | Out-Null{Environment.NewLine}[IO.File]::WriteAllText('{readyPath}', 'ready'){Environment.NewLine}while ($true) {{ Start-Sleep -Seconds 1 }}{Environment.NewLine}"
                    : "/bin/sh child.sh &\nprintf ready > parent-ready.txt\nwhile :; do sleep 1; done\n")
                .ConfigureAwait(false);

            runTask = new GitCommandRunner(GetShellPath())
                .RunAsync(testDirectory, BuildShellArguments(parentScriptPath), cancellation.Token);

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

    private static string GetShellPath() => OperatingSystem.IsWindows() ? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        "System32",
        "WindowsPowerShell",
        "v1.0",
        "powershell.exe") : "/bin/sh";

    private static string BuildShellArguments(string scriptPath) => OperatingSystem.IsWindows()
        ? $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\""
        : $"\"{scriptPath}\"";

    private static void EnsureSupportedPlatform()
    {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
        {
            Assert.Inconclusive("The process fixture targets Windows and Linux.");
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
