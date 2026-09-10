using Code.Navigation;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Navigation;

/// <summary>
/// Focuses an editor document and applies a source location to its caret.
/// </summary>
public interface IEditorCaretService
{
    /// <summary>
    /// Focuses the document and moves its caret. Implementations define how
    /// coordinates outside the document are clamped or rejected.
    /// </summary>
    /// <param name="document">The active editor document.</param>
    /// <param name="location">The requested source location.</param>
    /// <param name="cancellationToken">Cancels the caret operation.</param>
    Task FocusAndMoveCaretAsync(
        IEditorDocument document,
        CodeLocation location,
        CancellationToken cancellationToken = default);
}