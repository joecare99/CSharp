using System.Threading;
using System.Threading.Tasks;

namespace Code.Navigation;

/// <summary>
/// Provides UI-neutral navigation to a source location.
/// </summary>
public interface ICodeNavigator
{
    /// <summary>
    /// Opens or activates the document represented by the location and moves
    /// the caret to the requested position.
    /// </summary>
    /// <param name="location">The source location to activate.</param>
    /// <param name="cancellationToken">Cancels the navigation request.</param>
    /// <returns>A task that completes when the navigation request has finished.</returns>
    Task NavigateAsync(CodeLocation location, CancellationToken cancellationToken = default);
}
