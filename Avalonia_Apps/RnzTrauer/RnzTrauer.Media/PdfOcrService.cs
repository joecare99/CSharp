using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>
/// Coordinates internal PDF rendering and OCR while keeping engine-specific
/// implementations behind narrow interfaces.
/// </summary>
public sealed class PdfOcrService : IPdfOcrService
{
    private readonly IPdfImageRenderer _renderer;
    private readonly IOcrEngine _ocrEngine;

    public PdfOcrService(IPdfImageRenderer renderer, IOcrEngine ocrEngine)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _ocrEngine = ocrEngine ?? throw new ArgumentNullException(nameof(ocrEngine));
    }

    public async Task<PdfOcrResult> ExtractAsync(
        PdfOcrRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!File.Exists(request.PdfPath))
            throw new FileNotFoundException("The PDF input file does not exist.", request.PdfPath);
        if (request.MaxPages < 0)
            throw new ArgumentOutOfRangeException(nameof(request.MaxPages));
        cancellationToken.ThrowIfCancellationRequested();

        var pages = await _renderer.RenderAsync(
            new PdfImageRenderRequest(request.PdfPath, request.MaxPages),
            cancellationToken).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(pages);
        var orderedPages = pages.OrderBy(page => page.PageNumber).ToArray();
        if (orderedPages.Any(static page => page is null))
            throw new InvalidDataException("The renderer returned a null page.");
        if (orderedPages.Any(static page =>
                page.PageNumber <= 0 ||
                page.ImageBytes is null ||
                page.ImageBytes.Length == 0 ||
                string.IsNullOrWhiteSpace(page.ContentType)))
            throw new InvalidDataException("The renderer returned an invalid page.");
        if (orderedPages.Select(static page => page.PageNumber).Distinct().Count() != orderedPages.Length)
            throw new InvalidDataException("The renderer returned duplicate page numbers.");
        var pageTexts = new List<string>(orderedPages.Length);
        foreach (var page in orderedPages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            pageTexts.Add(await _ocrEngine.RecognizeAsync(page, cancellationToken)
                .ConfigureAwait(false));
        }

        var text = string.Join(
            Environment.NewLine,
            pageTexts.Where(static pageText => !string.IsNullOrWhiteSpace(pageText)));
        return new PdfOcrResult(request.PdfPath, pageTexts, text);
    }
}
