using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Acquisition;

/// <summary>
/// UI-free acquisition service with explicit source schemes, cancellation, and
/// bounded reads. It does not retry implicitly or hide transport failures.
/// </summary>
public sealed class HtmlAcquisitionService : IHtmlAcquisitionService
{
    private readonly HttpClient _httpClient;

    /// <summary>Creates an acquisition service using the supplied HTTP client.</summary>
    public HtmlAcquisitionService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async Task<HtmlAcquisitionResult> AcquireAsync(
        HtmlAcquisitionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Source is null)
            throw new ArgumentNullException(nameof(request.Source));
        if (request.MaxBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.MaxBytes));

        var (content, mediaType) = request.Source.Scheme.ToLowerInvariant() switch
        {
            "file" => (await ReadFileAsync(request.Source, request.MaxBytes, cancellationToken)
                .ConfigureAwait(false), "text/html"),
            "http" or "https" => await ReadHttpAsync(request.Source, request.MaxBytes, cancellationToken)
                .ConfigureAwait(false),
            _ => throw new NotSupportedException(
                $"The acquisition source scheme '{request.Source.Scheme}' is not supported."),
        };

        var archivedPath = request.ArchivePath is null
            ? null
            : await ArchiveAtomicallyAsync(request.ArchivePath, content, cancellationToken)
                .ConfigureAwait(false);

        return new HtmlAcquisitionResult(request.Source, content, mediaType, archivedPath);
    }

    private async Task<(byte[] Content, string? MediaType)> ReadHttpAsync(
        Uri source,
        long maxBytes,
        CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(
            source,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var contentLength = response.Content.Headers.ContentLength;
        if (contentLength.HasValue && contentLength.Value > maxBytes)
            throw new InvalidDataException($"The response exceeds the {maxBytes}-byte limit.");

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
        var content = await ReadBoundedAsync(stream, maxBytes, cancellationToken).ConfigureAwait(false);
        return (content, response.Content.Headers.ContentType?.MediaType);
    }

    private static async Task<byte[]> ReadFileAsync(
        Uri source,
        long maxBytes,
        CancellationToken cancellationToken)
    {
        var path = source.LocalPath;
        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return await ReadBoundedAsync(stream, maxBytes, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<byte[]> ReadBoundedAsync(
        Stream stream,
        long maxBytes,
        CancellationToken cancellationToken)
    {
        await using var buffer = new MemoryStream();
        var chunk = new byte[64 * 1024];
        while (true)
        {
            var read = await stream.ReadAsync(chunk.AsMemory(), cancellationToken).ConfigureAwait(false);
            if (read == 0)
                break;
            if (buffer.Length + read > maxBytes)
                throw new InvalidDataException($"The input exceeds the {maxBytes}-byte limit.");
            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
        }

        return buffer.ToArray();
    }

    private static async Task<string> ArchiveAtomicallyAsync(
        string archivePath,
        byte[] content,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(archivePath);
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException("The archive path has no directory.");
        Directory.CreateDirectory(directory);
        var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await File.WriteAllBytesAsync(temporaryPath, content, cancellationToken)
                .ConfigureAwait(false);
            File.Move(temporaryPath, fullPath, true);
            return fullPath;
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}
