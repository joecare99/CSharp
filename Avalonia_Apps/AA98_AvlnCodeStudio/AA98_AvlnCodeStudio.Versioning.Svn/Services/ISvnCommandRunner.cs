using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Svn.Services;

/// <summary>
/// Runs local Subversion CLI commands.
/// </summary>
public interface ISvnCommandRunner
{
    Task<SvnCommandResult> RunAsync(
        string workingDirectory,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);
}
