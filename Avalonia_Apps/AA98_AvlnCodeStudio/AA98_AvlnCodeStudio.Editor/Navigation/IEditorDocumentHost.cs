using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Navigation;

/// <summary>
/// Provides access to documents managed by an editor host.
/// </summary>
public interface IEditorDocumentHost
{
    /// <summary>
    /// Opens the document or activates the already open document for the path.
    /// </summary>
    /// <param name="filePath">The normalized source path.</param>
    /// <param name="cancellationToken">Cancels the host operation.</param>
    /// <returns>The active document, or <see langword="null"/> when the file is unavailable.</returns>
    Task<IEditorDocument?> OpenOrActivateAsync(string filePath, CancellationToken cancellationToken = default);
}