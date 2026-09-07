using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Configuration for the Poppler pdftoppm image renderer.</summary>
public sealed record PopplerPdfImageRendererOptions(
    string ExecutablePath,
    int Dpi = 300,
    long MaxImageBytes = 20 * 1024 * 1024,
    TimeSpan? Timeout = null);

/// <summary>
/// Renders PDF pages to PNG using Poppler's non-interactive pdftoppm tool.
/// </summary>
public sealed class PopplerPdfImageRenderer : IPdfImageRenderer
{
    private readonly PopplerPdfImageRendererOptions _options;
    private readonly IExternalProcessRunner _processRunner;

    public PopplerPdfImageRenderer(
        PopplerPdfImageRendererOptions options,
        IExternalProcessRunner processRunner)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        if (string.IsNullOrWhiteSpace(_options.ExecutablePath))
            throw new ArgumentException("The pdftoppm executable path is required.", nameof(options));
        if (_options.Dpi <= 0)
            throw new ArgumentOutOfRangeException(nameof(options), "DPI must be positive.");
        if (_options.MaxImageBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(options), "The image-size limit must be positive.");
    }

    public async Task<IReadOnlyList<RenderedPdfPage>> RenderAsync(
        PdfImageRenderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!File.Exists(request.PdfPath))
            throw new FileNotFoundException("The PDF input file does not exist.", request.PdfPath);
        if (request.MaxPages < 0)
            throw new ArgumentOutOfRangeException(nameof(request.MaxPages));
        cancellationToken.ThrowIfCancellationRequested();

        var temporaryDirectory = Path.Combine(Path.GetTempPath(), "rnz-render-" + Guid.NewGuid().ToString("N"));
        var outputPrefix = Path.Combine(temporaryDirectory, "page");
        Directory.CreateDirectory(temporaryDirectory);
        try
        {
            var arguments = new List<string>
            {
                "-png",
                "-r", _options.Dpi.ToString(CultureInfo.InvariantCulture)
            };
            if (request.MaxPages > 0)
            {
                arguments.Add("-f");
                arguments.Add("1");
                arguments.Add("-l");
                arguments.Add(request.MaxPages.ToString(CultureInfo.InvariantCulture));
            }
            arguments.Add(request.PdfPath);
            arguments.Add(outputPrefix);

            var result = await _processRunner.RunAsync(
                new ExternalProcessRequest(
                    _options.ExecutablePath,
                    arguments,
                    temporaryDirectory,
                    _options.Timeout ?? TimeSpan.FromMinutes(2)),
                cancellationToken).ConfigureAwait(false);
            if (result.ExitCode != 0)
                throw new InvalidOperationException(
                    $"PDF rendering failed with exit code {result.ExitCode}: {result.StandardError}");

            var imagePaths = Directory.GetFiles(temporaryDirectory, "page-*.png")
                .Select(path => (Path: path, Page: ParsePageNumber(path)))
                .Where(static item => item.Page > 0)
                .OrderBy(static item => item.Page)
                .ToArray();
            if (imagePaths.Length == 0)
                throw new InvalidDataException("The renderer did not create any PNG pages.");

            var pages = new List<RenderedPdfPage>(imagePaths.Length);
            foreach (var image in imagePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var info = new FileInfo(image.Path);
                if (info.Length == 0 || info.Length > _options.MaxImageBytes)
                    throw new InvalidDataException($"Rendered page {image.Page} exceeds the image-size limits.");
                pages.Add(new RenderedPdfPage(
                    image.Page,
                    await File.ReadAllBytesAsync(image.Path, cancellationToken).ConfigureAwait(false),
                    "image/png"));
            }
            return pages;
        }
        finally
        {
            if (Directory.Exists(temporaryDirectory))
                Directory.Delete(temporaryDirectory, recursive: true);
        }
    }

    private static int ParsePageNumber(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var separator = name.LastIndexOf('-');
        return separator < 0 ||
               !int.TryParse(name[(separator + 1)..], NumberStyles.None,
                   CultureInfo.InvariantCulture, out var page)
            ? 0
            : page;
    }
}
