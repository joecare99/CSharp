using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Request for rendering PDF pages as raster images.</summary>
public sealed record PdfImageRenderRequest(string PdfPath, int MaxPages);

/// <summary>One rendered PDF page passed to an OCR engine.</summary>
public sealed record RenderedPdfPage(int PageNumber, byte[] ImageBytes, string ContentType);

/// <summary>Renders PDF pages without exposing a UI or external window.</summary>
public interface IPdfImageRenderer
{
    Task<IReadOnlyList<RenderedPdfPage>> RenderAsync(
        PdfImageRenderRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>Recognizes text from one rendered PDF page.</summary>
public interface IOcrEngine
{
    Task<string> RecognizeAsync(
        RenderedPdfPage page,
        CancellationToken cancellationToken = default);
}

/// <summary>Request for internal PDF image rendering followed by OCR.</summary>
public sealed record PdfOcrRequest(string PdfPath, int MaxPages = 0);

/// <summary>OCR output for one PDF, preserving page-level results.</summary>
public interface IPdfOcrService
{
    Task<PdfOcrResult> ExtractAsync(
        PdfOcrRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>OCR output for one PDF, preserving page-level results.</summary>
public sealed record PdfOcrResult(
    string PdfPath,
    IReadOnlyList<string> PageTexts,
    string Text);
