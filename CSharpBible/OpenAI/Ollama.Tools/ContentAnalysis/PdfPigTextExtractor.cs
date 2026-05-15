using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PdfDocument = UglyToad.PdfPig.PdfDocument;
using Page = UglyToad.PdfPig.Content.Page;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Provides PDF text extraction using PdfPig.
/// </summary>
public sealed class PdfPigTextExtractor : IPdfTextExtractor
{
    /// <inheritdoc/>
    public Task<PdfExtractionResult> ExtractAsync(PdfExtractionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(request.FilePath))
        {
            return Task.FromResult(PdfExtractionResult.Failure(string.Empty, "A PDF file path is required."));
        }

        if (!File.Exists(request.FilePath))
        {
            return Task.FromResult(PdfExtractionResult.Failure(request.FilePath, "The PDF file does not exist."));
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(PdfExtractionResult.Failure(request.FilePath, "Password-protected PDFs are not supported yet."));
        }

        try
        {
            using PdfDocument document = PdfDocument.Open(request.FilePath);
            List<string> pageTexts = [];
            foreach (Page page in document.GetPages())
            {
                pageTexts.Add(page.Text ?? string.Empty);
            }

            string extractedText = string.Join(Environment.NewLine, pageTexts).Trim();
            ContentAnalysisFileMetadata fileMetadata = CreateFileMetadata(request.FilePath, request.FileMetadata);

            return Task.FromResult(PdfExtractionResult.Success(request.FilePath, extractedText, document.NumberOfPages, fileMetadata));
        }
        catch (Exception exception)
        {
            return Task.FromResult(PdfExtractionResult.Failure(request.FilePath, exception.Message, fileMetadata: CreateFileMetadata(request.FilePath, request.FileMetadata)));
        }
    }

    private static ContentAnalysisFileMetadata CreateFileMetadata(string filePath, ContentAnalysisFileMetadata? existingMetadata)
    {
        if (existingMetadata is not null)
        {
            return new ContentAnalysisFileMetadata
            {
                FileName = string.IsNullOrWhiteSpace(existingMetadata.FileName) ? Path.GetFileName(filePath) : existingMetadata.FileName,
                Extension = string.IsNullOrWhiteSpace(existingMetadata.Extension) ? Path.GetExtension(filePath) : existingMetadata.Extension,
                SizeBytes = existingMetadata.SizeBytes,
                LastWriteTimeUtc = existingMetadata.LastWriteTimeUtc,
            };
        }

        FileInfo fileInfo = new(filePath);
        return new ContentAnalysisFileMetadata
        {
            FileName = fileInfo.Name,
            Extension = fileInfo.Extension,
            SizeBytes = fileInfo.Exists ? fileInfo.Length : null,
            LastWriteTimeUtc = fileInfo.Exists ? fileInfo.LastWriteTimeUtc : null,
        };
    }
}
