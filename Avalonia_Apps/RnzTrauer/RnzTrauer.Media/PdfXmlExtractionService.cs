using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>PDF XML extraction adapter with explicit process and output limits.</summary>
public sealed class PdfXmlExtractionService : IPdfXmlExtractionService
{
    private readonly IExternalProcessRunner _processRunner;
    private readonly PdfXmlDocumentParser _parser;

    /// <summary>Creates an extraction service from independently testable seams.</summary>
    public PdfXmlExtractionService(
        IExternalProcessRunner processRunner,
        PdfXmlDocumentParser parser)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    /// <inheritdoc />
    public async Task<PdfXmlExtractionResult> ExtractAsync(
        PdfXmlExtractionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!File.Exists(request.PdfPath))
            throw new FileNotFoundException("The PDF input file does not exist.", request.PdfPath);
        if (request.MaxXmlBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(request.MaxXmlBytes));

        var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(request.XmlOutputPath));
        if (outputDirectory is null)
            throw new InvalidOperationException("The XML output path has no directory.");
        Directory.CreateDirectory(outputDirectory);

        var processResult = await _processRunner.RunAsync(
            new ExternalProcessRequest(
                request.ToolPath,
                [request.PdfPath, request.XmlOutputPath],
                outputDirectory,
                request.Timeout),
            cancellationToken).ConfigureAwait(false);
        if (processResult.ExitCode != 0)
            throw new InvalidOperationException(
                $"PDF XML conversion failed with exit code {processResult.ExitCode}: {processResult.StandardError}");
        if (!File.Exists(request.XmlOutputPath))
            throw new FileNotFoundException("The converter did not create its XML output.", request.XmlOutputPath);

        var xmlInfo = new FileInfo(request.XmlOutputPath);
        if (xmlInfo.Length > request.MaxXmlBytes)
            throw new InvalidDataException($"The XML output exceeds the {request.MaxXmlBytes}-byte limit.");

        var xml = await File.ReadAllTextAsync(request.XmlOutputPath, cancellationToken)
            .ConfigureAwait(false);
        var parsed = _parser.Parse(xml);
        return new PdfXmlExtractionResult(
            request.PdfPath,
            request.XmlOutputPath,
            parsed.Text,
            parsed.Images,
            processResult.StandardError);
    }
}
