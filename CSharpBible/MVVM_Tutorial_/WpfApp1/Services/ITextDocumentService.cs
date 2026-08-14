using System.Threading;
using System.Threading.Tasks;
using WpfApp.Models;

namespace WpfApp.Services;

/// <summary>
/// Provides file-based load and save operations for text documents.
/// </summary>
public interface ITextDocumentService
{
    /// <summary>
    /// Loads a text document from the specified file path.
    /// </summary>
    /// <param name="filePath">The file path to load.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded document.</returns>
    Task<TextDocumentModel> LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a text document to its associated file path.</summary>
    /// <param name="document">The document to save.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveAsync(TextDocumentModel document, CancellationToken cancellationToken = default);
}
