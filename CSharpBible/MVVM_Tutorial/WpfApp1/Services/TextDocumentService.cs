using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WpfApp.Models;

namespace WpfApp.Services;

/// <summary>
/// Implements file-based text document storage.
/// </summary>
public sealed class TextDocumentService : ITextDocumentService
{
    /// <inheritdoc />
    public async Task<TextDocumentModel> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var text = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
        return new TextDocumentModel
        {
            FilePath = filePath,
            Text = text
        };
    }

    /// <inheritdoc />
    public async Task SaveAsync(TextDocumentModel document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(document.FilePath);

        var sFilePath = document.FilePath;
        var sDirectory = Path.GetDirectoryName(sFilePath);
        if (!string.IsNullOrWhiteSpace(sDirectory))
        {
            Directory.CreateDirectory(sDirectory);
        }

        await File.WriteAllTextAsync(sFilePath, document.Text ?? string.Empty, cancellationToken).ConfigureAwait(false);
    }
}
