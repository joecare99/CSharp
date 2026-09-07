using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Configuration for the command-line Tesseract OCR adapter.</summary>
public sealed record TesseractOcrOptions(
    string ExecutablePath,
    string Language = "deu",
    int PageSegmentationMode = 6,
    TimeSpan? Timeout = null);

/// <summary>
/// Runs Tesseract against one rendered page without using a viewer or clipboard.
/// </summary>
public sealed class TesseractOcrEngine : IOcrEngine
{
    private readonly TesseractOcrOptions _options;
    private readonly IExternalProcessRunner _processRunner;

    public TesseractOcrEngine(
        TesseractOcrOptions options,
        IExternalProcessRunner processRunner)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        if (string.IsNullOrWhiteSpace(_options.ExecutablePath))
            throw new ArgumentException("The Tesseract executable path is required.", nameof(options));
        if (string.IsNullOrWhiteSpace(_options.Language))
            throw new ArgumentException("The Tesseract language is required.", nameof(options));
        if (_options.PageSegmentationMode is < 0 or > 13)
            throw new ArgumentOutOfRangeException(nameof(options), "The page segmentation mode must be between 0 and 13.");
    }

    public async Task<string> RecognizeAsync(
        RenderedPdfPage page,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);
        if (page.ImageBytes is null || page.ImageBytes.Length == 0)
            throw new InvalidDataException("The rendered page contains no image data.");
        if (!string.Equals(page.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException($"Unsupported OCR image type: {page.ContentType}");

        var temporaryDirectory = Path.Combine(Path.GetTempPath(), "rnz-ocr-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temporaryDirectory);
        var imagePath = Path.Combine(temporaryDirectory, "page.png");
        var outputBasePath = Path.Combine(temporaryDirectory, "result");
        var outputTextPath = outputBasePath + ".txt";
        try
        {
            await File.WriteAllBytesAsync(imagePath, page.ImageBytes, cancellationToken)
                .ConfigureAwait(false);
            var result = await _processRunner.RunAsync(
                new ExternalProcessRequest(
                    _options.ExecutablePath,
                    [imagePath, outputBasePath, "-l", _options.Language, "--psm", _options.PageSegmentationMode.ToString()],
                    temporaryDirectory,
                    _options.Timeout ?? TimeSpan.FromMinutes(2)),
                cancellationToken).ConfigureAwait(false);
            if (result.ExitCode != 0)
                throw new InvalidOperationException(
                    $"Tesseract failed with exit code {result.ExitCode}: {result.StandardError}");
            if (!File.Exists(outputTextPath))
                throw new FileNotFoundException("Tesseract did not create its text output.", outputTextPath);

            return await File.ReadAllTextAsync(outputTextPath, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(temporaryDirectory))
                Directory.Delete(temporaryDirectory, recursive: true);
        }
    }
}

