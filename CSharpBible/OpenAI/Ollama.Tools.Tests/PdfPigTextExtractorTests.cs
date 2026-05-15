using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class PdfPigTextExtractorTests
{
    [TestMethod]
    public async Task ExtractAsync_ReturnsTextAndPageCountForSamplePdf()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        await File.WriteAllBytesAsync(tempFilePath, CreateSinglePagePdfBytes("Hello PDF"));

        try
        {
            PdfPigTextExtractor extractor = new();
            PdfExtractionResult result = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = tempFilePath,
            });

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(tempFilePath, result.FilePath);
            StringAssert.Contains(result.ExtractedText, "Hello PDF");
            Assert.AreEqual(1, result.PageCount);
            Assert.IsNotNull(result.FileMetadata);
            Assert.AreEqual(Path.GetFileName(tempFilePath), result.FileMetadata.FileName);
            Assert.AreEqual(".pdf", result.FileMetadata.Extension);
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [TestMethod]
    public async Task ExtractAsync_ReturnsSuccessWithEmptyTextForBlankPdf()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        await File.WriteAllBytesAsync(tempFilePath, CreateSinglePagePdfBytes(string.Empty));

        try
        {
            PdfPigTextExtractor extractor = new();
            PdfExtractionResult result = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = tempFilePath,
            });

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(string.Empty, result.ExtractedText);
            Assert.AreEqual(1, result.PageCount);
            Assert.IsNotNull(result.FileMetadata);
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [TestMethod]
    public async Task ExtractAsync_ReturnsFailureForInvalidPdf()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        await File.WriteAllBytesAsync(tempFilePath, [0x00, 0x01, 0x02, 0x03]);

        try
        {
            PdfPigTextExtractor extractor = new();
            PdfExtractionResult result = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = tempFilePath,
            });

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
            Assert.IsNotNull(result.FileMetadata);
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [TestMethod]
    public async Task ExtractAsync_MergesCallerSuppliedFileMetadata()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pdf");
        await File.WriteAllBytesAsync(tempFilePath, CreateSinglePagePdfBytes("Metadata test"));

        try
        {
            PdfPigTextExtractor extractor = new();
            PdfExtractionResult result = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = tempFilePath,
                FileMetadata = new ContentAnalysisFileMetadata
                {
                    FileName = "custom-name.pdf",
                    SizeBytes = 42,
                },
            });

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.FileMetadata);
            Assert.AreEqual("custom-name.pdf", result.FileMetadata.FileName);
            Assert.AreEqual(".pdf", result.FileMetadata.Extension);
            Assert.AreEqual(42, result.FileMetadata.SizeBytes);
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    private static byte[] CreateSinglePagePdfBytes(string text)
    {
        string escapedText = EscapePdfText(text);
        string contentStream = string.IsNullOrWhiteSpace(text)
            ? "q\nQ"
            : $"BT /F1 24 Tf 72 120 Td ({escapedText}) Tj ET";

        string header = "%PDF-1.4\n";
        string object1 = "1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n";
        string object2 = "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n";
        string object3 = "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\nendobj\n";
        string object4 = $"4 0 obj\n<< /Length {Encoding.ASCII.GetByteCount(contentStream)} >>\nstream\n{contentStream}\nendstream\nendobj\n";
        string object5 = "5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n";

        List<string> objects = [object1, object2, object3, object4, object5];
        List<int> offsets = [0];
        int currentOffset = Encoding.ASCII.GetByteCount(header);

        foreach (string pdfObject in objects)
        {
            offsets.Add(currentOffset);
            currentOffset += Encoding.ASCII.GetByteCount(pdfObject);
        }

        int xrefStart = currentOffset;

        StringBuilder builder = new();
        builder.Append(header);
        foreach (string pdfObject in objects)
        {
            builder.Append(pdfObject);
        }

        builder.Append("xref\n0 6\n");
        builder.Append("0000000000 65535 f \n");
        for (int index = 1; index < offsets.Count; index++)
        {
            builder.Append(offsets[index].ToString("D10", CultureInfo.InvariantCulture));
            builder.Append(" 00000 n \n");
        }

        builder.Append("trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n");
        builder.Append(xrefStart.ToString(CultureInfo.InvariantCulture));
        builder.Append("\n%%EOF\n");

        return Encoding.ASCII.GetBytes(builder.ToString());
    }

    private static string EscapePdfText(string text)
    {
        return text.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }
}
