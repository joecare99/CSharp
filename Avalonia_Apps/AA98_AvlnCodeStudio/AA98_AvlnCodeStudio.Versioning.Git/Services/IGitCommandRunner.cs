using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Git.Services;

/// <summary>
/// Runs local Git commands for repository inspection.
/// </summary>
public interface IGitCommandRunner
{
    /// <summary>
    /// Runs a Git command and returns the captured result.
    /// </summary>
    /// <param name="workingDirectory">The working directory for the command.</param>
    /// <param name="arguments">The Git arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command result.</returns>
    Task<GitCommandResult> RunAsync(string workingDirectory, string arguments, CancellationToken cancellationToken = default);
}
