using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Svn.Services;

/// <summary>
/// Implements explicit Subversion working-copy update operations.
/// </summary>
public sealed class SvnWorkingCopyOperations(ISvnCommandRunner svnCommandRunner) : ISvnWorkingCopyOperations
{
    private readonly ISvnCommandRunner _svnCommandRunner = svnCommandRunner
        ?? throw new ArgumentNullException(nameof(svnCommandRunner));

    public async Task<string> UpdateAsync(
        string workingCopyPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workingCopyPath);
        cancellationToken.ThrowIfCancellationRequested();

        string fullPath = Path.GetFullPath(workingCopyPath);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"The SVN working-copy directory '{fullPath}' does not exist.");
        }

        SvnCommandResult result = await _svnCommandRunner.RunAsync(
            fullPath,
            ["update", "--", fullPath],
            cancellationToken).ConfigureAwait(false);
        if (result.ExitCode != 0)
        {
            string detail = string.IsNullOrWhiteSpace(result.StandardError)
                ? result.StandardOutput.Trim()
                : result.StandardError.Trim();
            throw new InvalidOperationException(
                $"SVN update failed with exit code {result.ExitCode}: {detail}");
        }

        return result.StandardOutput.Trim();
    }
}
