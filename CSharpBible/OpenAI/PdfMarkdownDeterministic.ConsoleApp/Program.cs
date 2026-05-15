using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ollama.Tools.ContentAnalysis;

namespace PdfMarkdownDeterministic.ConsoleApp;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: PdfMarkdownDeterministic.ConsoleApp <input-pdf> <output-md>");
            return 1;
        }

        string inputPdfPath = args[0];
        string outputMarkdownPath = args[1];

        if (!File.Exists(inputPdfPath))
        {
            Console.WriteLine($"Input PDF not found: {inputPdfPath}");
            return 1;
        }

        IPdfTextExtractor extractor = new PdfPigTextExtractor();
        PdfExtractionResult extractionResult = await extractor.ExtractAsync(new PdfExtractionRequest
        {
            FilePath = inputPdfPath,
        });

        if (!extractionResult.IsSuccessful)
        {
            Console.WriteLine(extractionResult.ErrorMessage ?? "PDF extraction failed.");
            return 1;
        }

        PdfDocumentInspection inspection = PdfStructureInspector.Inspect(inputPdfPath);
        string markdown = BuildMarkdown(inputPdfPath, extractionResult, inspection);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputMarkdownPath)) ?? ".");
        await File.WriteAllTextAsync(outputMarkdownPath, markdown, Encoding.UTF8);

        Console.WriteLine($"Markdown written to {outputMarkdownPath}");
        return 0;
    }

    private static string BuildMarkdown(string inputPdfPath, PdfExtractionResult extractionResult, PdfDocumentInspection inspection)
    {
        StringBuilder builder = new();
        builder.AppendLine($"# {Path.GetFileNameWithoutExtension(inputPdfPath)}");
        builder.AppendLine();
        builder.AppendLine($"- Source: `{inputPdfPath}`");
        if (extractionResult.PageCount is int pageCount)
        {
            builder.AppendLine($"- Pages: {pageCount}");
        }
        if (extractionResult.FileMetadata is not null)
        {
            builder.AppendLine($"- File: `{extractionResult.FileMetadata.FileName}`");
        }
        builder.AppendLine($"- Text operators detected: {(inspection.HasTextOperators ? "yes" : "no")}");
        builder.AppendLine($"- Vector drawing hints detected: {(inspection.HasVectorDrawingHints ? "yes" : "no")}");
        builder.AppendLine($"- XObject references detected: {(inspection.XObjectReferenceCount > 0 ? "yes" : "no")}");
        builder.AppendLine($"- Inline image markers detected: {(inspection.InlineImageMarkerCount > 0 ? "yes" : "no")}");
        builder.AppendLine($"- Image objects detected: {(inspection.ImageObjectCount > 0 ? "yes" : "no")}");
        if (inspection.Metadata.Count > 0)
        {
            builder.AppendLine("- PDF metadata:");
            foreach (KeyValuePair<string, string> item in inspection.Metadata)
            {
                builder.AppendLine($"  - {item.Key}: {item.Value}");
            }
        }

        if (inspection.Fonts.Count > 0)
        {
            builder.AppendLine("- Fonts:");
            foreach (string font in inspection.Fonts)
            {
                builder.AppendLine($"  - {font}");
            }
        }

        if (inspection.XObjects.Count > 0)
        {
            builder.AppendLine("- XObjects:");
            foreach (string xObject in inspection.XObjects)
            {
                builder.AppendLine($"  - {xObject}");
            }
        }

        if (inspection.ContentHints.Count > 0)
        {
            builder.AppendLine("- Content hints:");
            foreach (string hint in inspection.ContentHints)
            {
                builder.AppendLine($"  - {hint}");
            }
        }

        AppendPagesSection(builder, inspection);
        AppendObjectsSection(builder, inspection);

        builder.AppendLine();
        builder.AppendLine("## Extracted Text");
        builder.AppendLine();
        builder.AppendLine(string.IsNullOrWhiteSpace(extractionResult.ExtractedText) ? "_No text extracted._" : extractionResult.ExtractedText);
        builder.AppendLine();
        builder.AppendLine("## Images");
        builder.AppendLine();
        if (inspection.ImageObjectCount > 0 || inspection.XObjectReferenceCount > 0 || inspection.InlineImageMarkerCount > 0)
        {
            builder.AppendLine($"_Detected {inspection.ImageObjectCount} image object(s), {inspection.XObjectReferenceCount} XObject reference(s), and {inspection.InlineImageMarkerCount} inline image marker(s). Image extraction/reporting hook reserved for later LLM-based analysis._");
        }
        else
        {
            builder.AppendLine("_No image objects detected._");
        }
        return builder.ToString();
    }

    private static void AppendPagesSection(StringBuilder builder, PdfDocumentInspection inspection)
    {
        IReadOnlyList<PdfObjectSummary> pageObjects = inspection.Objects.Where(static obj => string.Equals(obj.Type, "Page", StringComparison.OrdinalIgnoreCase)).ToList();
        builder.AppendLine();
        builder.AppendLine("## Pages");
        builder.AppendLine();

        if (pageObjects.Count == 0)
        {
            builder.AppendLine("_No page objects detected._");
            return;
        }

        foreach (PdfObjectSummary pageObject in pageObjects)
        {
            builder.AppendLine($"- Object `{pageObject.ObjectId}`");
            builder.AppendLine($"  - Stream: {(pageObject.HasStream ? "yes" : "no")}");
            builder.AppendLine($"  - Text operators: {(pageObject.HasTextOperators ? "yes" : "no")}");
            builder.AppendLine($"  - Vector drawing hints: {(pageObject.HasVectorDrawingHints ? "yes" : "no")}");
            builder.AppendLine($"  - ToUnicode map: {(pageObject.HasToUnicodeMap ? "yes" : "no")}");
            if (!string.IsNullOrWhiteSpace(pageObject.Type) || !string.IsNullOrWhiteSpace(pageObject.Subtype))
            {
                builder.AppendLine($"  - Type/Subtype: {pageObject.Type ?? "unknown"}/{pageObject.Subtype ?? "unknown"}");
            }
            if (pageObject.References.Count > 0)
            {
                builder.AppendLine("  - References:");
                foreach (string reference in pageObject.References)
                {
                    builder.AppendLine($"    - {reference}");
                }
            }
        }
    }

    private static void AppendObjectsSection(StringBuilder builder, PdfDocumentInspection inspection)
    {
        builder.AppendLine();
        builder.AppendLine("## Objects");
        builder.AppendLine();

        if (inspection.Objects.Count == 0)
        {
            builder.AppendLine("_No PDF objects detected._");
            return;
        }

        foreach (PdfObjectSummary pdfObject in inspection.Objects)
        {
            builder.AppendLine($"- Object `{pdfObject.ObjectId}`");
            builder.AppendLine($"  - Type: {pdfObject.Type ?? "unknown"}");
            builder.AppendLine($"  - Subtype: {pdfObject.Subtype ?? "unknown"}");
            builder.AppendLine($"  - Stream: {(pdfObject.HasStream ? "yes" : "no")}");
            builder.AppendLine($"  - Text operators: {(pdfObject.HasTextOperators ? "yes" : "no")}");
            builder.AppendLine($"  - Vector drawing hints: {(pdfObject.HasVectorDrawingHints ? "yes" : "no")}");
            builder.AppendLine($"  - ToUnicode map: {(pdfObject.HasToUnicodeMap ? "yes" : "no")}");

            if (!string.IsNullOrWhiteSpace(pdfObject.BaseFont))
            {
                builder.AppendLine($"  - Base font: {pdfObject.BaseFont}");
            }

            if (!string.IsNullOrWhiteSpace(pdfObject.FontName))
            {
                builder.AppendLine($"  - Font name: {pdfObject.FontName}");
            }

            if (pdfObject.Encodings.Count > 0)
            {
                builder.AppendLine("  - Encodings:");
                foreach (string encoding in pdfObject.Encodings)
                {
                    builder.AppendLine($"    - {encoding}");
                }
            }

            if (pdfObject.References.Count > 0)
            {
                builder.AppendLine("  - References:");
                foreach (string reference in pdfObject.References)
                {
                    builder.AppendLine($"    - {reference}");
                }
            }
        }
    }

}
