using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Versioning.Svn.Services;

/// <summary>
/// Provides explicitly requested operations on a Subversion working copy.
/// </summary>
public interface ISvnWorkingCopyOperations
{
    Task<string> UpdateAsync(string workingCopyPath, CancellationToken cancellationToken = default);
}
