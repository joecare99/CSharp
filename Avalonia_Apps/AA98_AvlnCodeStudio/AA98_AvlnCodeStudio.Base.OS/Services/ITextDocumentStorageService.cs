using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.OS.Services;

/// <summary>
/// Defines asynchronous persistence operations for plain text documents.
/// </summary>
public interface ITextDocumentStorageService
{
    /// <summary>
    /// Reads the complete content of a text file.
    /// </summary>
    /// <param name="filePath">The absolute path of the file to read.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded text content.</returns>
    Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the complete content of a text file.
    /// </summary>
    /// <param name="filePath">The absolute path of the file to write.</param>
    /// <param name="content">The content to persist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the file was written.</returns>
    Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default);
}