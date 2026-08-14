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
#if NET6_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
#else
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));
#endif
        string text;
#if NET6_0_OR_GREATER
        text = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
#else
        text = await new Task<string>(() => File.ReadAllText(filePath)).ConfigureAwait(false);
#endif
        return new TextDocumentModel
        {
            FilePath = filePath,
            Text = text
        };
    }

    /// <inheritdoc />
    public async Task SaveAsync(TextDocumentModel document, CancellationToken cancellationToken = default)
    {
#if NET6_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(document.FilePath);
#else
        if (string.IsNullOrWhiteSpace(document.FilePath))
            throw new ArgumentNullException(nameof(document.FilePath));
#endif       

        var sFilePath = document.FilePath;
        var sDirectory = Path.GetDirectoryName(sFilePath);
        if (!string.IsNullOrWhiteSpace(sDirectory))
        {
            Directory.CreateDirectory(sDirectory);
        }


#if NET6_0_OR_GREATER
        await File.WriteAllTextAsync(sFilePath, document.Text ?? string.Empty, cancellationToken).ConfigureAwait(false);
#else
        await new Task(() => File.WriteAllText(sFilePath, document.Text ?? string.Empty)).ConfigureAwait(false);
#endif
    }
}
