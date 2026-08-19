using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Tools.ContentAnalysis;
using PdfMarkdownDeterministic.ConsoleApp;
using PdfProgram = PdfMarkdownDeterministic.ConsoleApp.Program;

namespace PeripheralProduction.Tests;

[TestClass]
[DoNotParallelize]
public sealed class PdfMarkdownDeterministicTests
{
    [TestMethod]
    public void Inspect_ExtractsDeterministicStructureMetadataAndContentHints()
    {
        using TestDirectory directory = new();
        string path = directory.Write("rich.pdf", Encoding.Latin1.GetBytes(CreateRichPdf()));

        PdfDocumentInspection inspection = PdfStructureInspector.Inspect(path);

        Assert.IsTrue(inspection.HasTextOperators);
        Assert.IsTrue(inspection.HasVectorDrawingHints);
        Assert.IsTrue(inspection.HasToUnicodeMap);
        Assert.AreEqual(1, inspection.ImageObjectCount);
        Assert.IsTrue(inspection.XObjectReferenceCount > 0);
        Assert.IsTrue(inspection.InlineImageMarkerCount > 0);
        Assert.AreEqual("A (nested) title", inspection.Metadata["Title"]);
        CollectionAssert.Contains((System.Collections.ICollection)inspection.Fonts, "Helvetica");
        CollectionAssert.Contains((System.Collections.ICollection)inspection.Encodings, "WinAnsiEncoding");
        CollectionAssert.Contains((System.Collections.ICollection)inspection.XObjects, "Im0");
        CollectionAssert.Contains((System.Collections.ICollection)inspection.ContentHints, "Do operator present");
        CollectionAssert.Contains((System.Collections.ICollection)inspection.ContentHints, "XML stream present");
        CollectionAssert.Contains((System.Collections.ICollection)inspection.ContentHints, "Vector drawing operators present in stream content");

        PdfObjectSummary content = inspection.Objects.Single(static item => item.ObjectId == "5 0");
        Assert.AreEqual("Content", content.StreamKind);
        Assert.IsTrue(content.OperatorSummary.Count > 0);
        Assert.IsTrue(content.DrawingHints.Count > 0);
        Assert.IsTrue(content.GlyphCandidates.Count > 0);
    }

    [TestMethod]
    public void Inspect_DecodesZlibAndGracefullyHandlesMalformedStreams()
    {
        using TestDirectory directory = new();
        string decoded = "BT /F1 12 Tf (decoded) Tj ET";
        byte[] compressed = CompressZlib(decoded);
        string flateBody = Encoding.Latin1.GetString(compressed);
        string document = "%PDF-1.4\n"
            + "1 0 obj\n<< /Filter /FlateDecode >>\nstream\n" + flateBody + "\nendstream\nendobj\n"
            + "2 0 obj\n<< /Filter [/ASCII85Decode /FlateDecode] >>\nstream\nnot compressed\nendstream\nendobj\n"
            + "3 0 obj\n<< /Filter /LZWDecode >>\nstream\nBT\nendstream\nendobj\n"
            + "4 0 obj\n<< /Length 1 >>\nstream\n";
        string path = directory.Write("streams.pdf", Encoding.Latin1.GetBytes(document));

        PdfDocumentInspection inspection = PdfStructureInspector.Inspect(path);

        PdfObjectSummary decodedObject = inspection.Objects.Single(static item => item.ObjectId == "1 0");
        Assert.AreEqual("FlateDecode", decodedObject.Filter);
        Assert.AreEqual(decoded, decodedObject.DecodedStreamContent);
        Assert.AreEqual("Text", decodedObject.StreamKind);
        CollectionAssert.Contains((System.Collections.ICollection)inspection.ContentHints, "FlateDecode stream present");

        PdfObjectSummary malformedObject = inspection.Objects.Single(static item => item.ObjectId == "2 0");
        Assert.IsNull(malformedObject.DecodedStreamContent);
        Assert.IsNotNull(malformedObject.RawStreamPreview);
    }

    [TestMethod]
    public void Inspect_HandlesIdentifierMetadataPreviewAndDeflateEdgeCases()
    {
        using TestDirectory directory = new();
        string deflated = "0 0 m 1 1 l S";
        string deflateBody = Encoding.Latin1.GetString(CompressDeflate(deflated));
        string document = "%PDF-1.4\n"
            + "1 0 obj\n<< >>\nendobj\n"
            + "2 0 obj\n<< /Type /Info /Author /NotAString >>\nendobj\n"
            + "3 0 obj\n<< /Type /Info /Author (unclosed >>\nendobj\n"
            + "4 0 obj\n<< >>\nstream\n<?xml version=\"1.0\"?>\nendstream\nendobj\n"
            + "5 0 obj\n<< /Filter /FlateDecode >>\nstream\n" + deflateBody + "\nendstream\nendobj\n"
            + "6 0 obj\n<< >>\nstream\nBT ET\nendstream\nendobj\n"
            + "7 0 obj\n<< >>\nstream\n0 0 m 1 1 m 2 2 l S\nendstream\nendobj\n"
            + "8 0 obj\n<< >>\nstream\nendstream\nendobj\n"
            + "9 0 obj\n<< >>\nstream\n" + new string('x', 300) + "\nendstream\nendobj\n"
            + "10 0 obj\n<< /Filter [/ASCII85Decode] >>\nstream\nraw\nendstream\nendobj\n"
            + "11 0 obj\n<< >>\nstream\tBT ET\nendstream\nendobj\n"
            + "12 0 obj\n<< >>\nstreamendstream\nendobj\n"
            + "13 0 obj\n<< >>\nstream\n0 0 m 1 1 l S 0 0 m 1 1 l S\nendstream\nendobj\n"
            + "14 0 obj\n<< >>\nstream\n0 0 m f 0 0 m f* 0 0 m S 0 0 m s 0 0 m B 0 0 m B* 0 0 m Do BT ET BDC EMC Q q\nendstream\nendobj\n"
            + "15 0 obj\nstream\nendobj\n";
        string path = directory.Write("edges.pdf", Encoding.Latin1.GetBytes(document));

        PdfDocumentInspection inspection = PdfStructureInspector.Inspect(path);

        Assert.AreEqual("4 0 (XML stream)", inspection.Objects.Single(static item => item.ObjectId == "4 0").Identifier);
        Assert.AreEqual("5 0 (FlateDecode stream)", inspection.Objects.Single(static item => item.ObjectId == "5 0").Identifier);
        Assert.AreEqual("Text", inspection.Objects.Single(static item => item.ObjectId == "6 0").StreamKind);
        Assert.AreEqual("Vector", inspection.Objects.Single(static item => item.ObjectId == "7 0").StreamKind);
        Assert.AreEqual(string.Empty, inspection.Objects.Single(static item => item.ObjectId == "8 0").RawStreamPreview);
        StringAssert.EndsWith(inspection.Objects.Single(static item => item.ObjectId == "9 0").RawStreamPreview!, "…");
        Assert.AreEqual(deflated, inspection.Objects.Single(static item => item.ObjectId == "5 0").DecodedStreamContent);
        Assert.AreEqual("/ASCII85Decode", inspection.Objects.Single(static item => item.ObjectId == "10 0").Filter);

        PdfObjectSummary summary = new(
            "id",
            "identifier",
            null,
            null,
            null,
            null,
            Array.Empty<string>(),
            false,
            false,
            false,
            false,
            Array.Empty<string>(),
            null,
            null,
            null,
            null,
            null,
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>());
        Assert.AreEqual(string.Empty, PdfStructureInspector.ExportDecodedStreamForLlm(summary));
        foreach (string token in new[] { "BT", "ET", "BDC", "BMC", "EMC", "Do", "q", "Q" })
        {
            Assert.AreEqual($"<{token}>", PdfStructureInspector.NormalizePdfOperator(token));
        }
        Assert.AreEqual("Tf", PdfStructureInspector.NormalizePdfOperator("Tf"));
    }

    [TestMethod]
    public void BuildMarkdown_ReportsBothEmptyAndDetailedInspectionBranches()
    {
        PdfExtractionResult emptyExtraction = PdfExtractionResult.Success("input.pdf", string.Empty);
        PdfDocumentInspection emptyInspection = new(
            false,
            false,
            false,
            0,
            0,
            0,
            new Dictionary<string, string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<PdfObjectSummary>());

        string emptyMarkdown = PdfProgram.BuildMarkdown("input.pdf", emptyExtraction, emptyInspection);
        StringAssert.Contains(emptyMarkdown, "_No page objects detected._");
        StringAssert.Contains(emptyMarkdown, "_No PDF objects detected._");
        StringAssert.Contains(emptyMarkdown, "_No image objects detected._");
        StringAssert.Contains(emptyMarkdown, "_No decoded streams available._");
        StringAssert.Contains(emptyMarkdown, "_No text extracted._");

        PdfObjectSummary page = new(
            "1 0",
            "Page",
            "Page",
            "Demo",
            "Helvetica",
            "FontName",
            ["WinAnsiEncoding"],
            true,
            true,
            true,
            true,
            ["Im0"],
            "FlateDecode",
            "Content",
            "decoded content",
            "raw preview",
            "decoded preview",
            ["BT×1"],
            ["drawing hint"],
            ["m l×1"]);
        PdfDocumentInspection detailedInspection = new(
            true,
            true,
            true,
            1,
            2,
            2,
            new Dictionary<string, string> { ["Title"] = "Document" },
            ["Helvetica"],
            ["WinAnsiEncoding"],
            ["Im0"],
            ["Do operator present"],
            [page]);
        PdfExtractionResult detailedExtraction = PdfExtractionResult.Success(
            "input.pdf",
            "extracted text",
            2,
            new ContentAnalysisFileMetadata { FileName = "input.pdf" });

        string detailedMarkdown = PdfProgram.BuildMarkdown("input.pdf", detailedExtraction, detailedInspection);
        StringAssert.Contains(detailedMarkdown, "- Pages: 2");
        StringAssert.Contains(detailedMarkdown, "- PDF metadata:");
        StringAssert.Contains(detailedMarkdown, "- Fonts:");
        StringAssert.Contains(detailedMarkdown, "- XObjects:");
        StringAssert.Contains(detailedMarkdown, "Type/Subtype: Page/Demo");
        StringAssert.Contains(detailedMarkdown, "Raw stream preview: raw preview");
        StringAssert.Contains(detailedMarkdown, "Exact Decoded Streams");
        StringAssert.Contains(detailedMarkdown, "extracted text");
        StringAssert.Contains(detailedMarkdown, "_Detected 1 image object");

        PdfObjectSummary partiallySubtypedPage = page with
        {
            ObjectId = "3 0",
            Type = "Page",
            Subtype = null,
        };
        PdfDocumentInspection partiallyTypedInspection = detailedInspection with
        {
            Objects = [partiallySubtypedPage],
        };
        string partiallyTypedMarkdown = PdfProgram.BuildMarkdown("input.pdf", detailedExtraction, partiallyTypedInspection);
        StringAssert.Contains(partiallyTypedMarkdown, "Type/Subtype: Page/unknown");
    }

    [TestMethod]
    public async Task Main_ValidatesInputUsesInjectedExtractorAndWritesMarkdown()
    {
        using TestDirectory directory = new();
        string inputPath = directory.Write("input.pdf", Encoding.Latin1.GetBytes(CreateRichPdf()));
        string outputPath = Path.Combine(directory.Path, "output.md");

        (int usageResult, string usageOutput) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main([]));
        Assert.AreEqual(1, usageResult);
        StringAssert.Contains(usageOutput, "Usage:");

        (int missingResult, string missingOutput) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main(["missing.pdf", outputPath]));
        Assert.AreEqual(1, missingResult);
        StringAssert.Contains(missingOutput, "Input PDF not found");

        IPdfTextExtractor failedExtractor = Substitute.For<IPdfTextExtractor>();
        failedExtractor.ExtractAsync(Arg.Any<PdfExtractionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(PdfExtractionResult.Failure(inputPath, "extraction failed")));
        PdfProgram.PdfTextExtractorFactory = () => failedExtractor;
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main([inputPath, outputPath]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "extraction failed");

        IPdfTextExtractor noMessageExtractor = Substitute.For<IPdfTextExtractor>();
        noMessageExtractor.ExtractAsync(Arg.Any<PdfExtractionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PdfExtractionResult { IsSuccessful = false }));
        PdfProgram.PdfTextExtractorFactory = () => noMessageExtractor;
        (int noMessageResult, string noMessageOutput) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main([inputPath, outputPath]));
        Assert.AreEqual(1, noMessageResult);
        StringAssert.Contains(noMessageOutput, "PDF extraction failed.");

        IPdfTextExtractor extractor = Substitute.For<IPdfTextExtractor>();
        extractor.ExtractAsync(Arg.Any<PdfExtractionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(PdfExtractionResult.Success(inputPath, "deterministic text")));
        PdfProgram.PdfTextExtractorFactory = () => extractor;
        (int successResult, string successOutput) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main([inputPath, outputPath]));
        Assert.AreEqual(0, successResult);
        StringAssert.Contains(successOutput, "Markdown written");
        StringAssert.Contains(await File.ReadAllTextAsync(outputPath), "deterministic text");
        await extractor.Received(1).ExtractAsync(Arg.Is<PdfExtractionRequest>(request => request.FilePath == inputPath), Arg.Any<CancellationToken>());

        string localOutputPath = "pdf-markdown-local-output.md";
        try
        {
            (int localOutputResult, _) = await ConsoleOutput.CaptureAsync(() => PdfProgram.Main([inputPath, localOutputPath]));
            Assert.AreEqual(0, localOutputResult);
        }
        finally
        {
            File.Delete(Path.Combine(Environment.CurrentDirectory, localOutputPath));
        }
    }

    [TestMethod]
    public void CreatePdfTextExtractor_CreatesTheProductionExtractorWithoutExtractingFiles()
    {
        IPdfTextExtractor extractor = PdfProgram.CreatePdfTextExtractor();

        Assert.IsInstanceOfType<PdfPigTextExtractor>(extractor);
    }

    [TestMethod]
    public void SelfCheck_UsesCallerControlledWorkingDirectory()
    {
        using TestDirectory directory = new();

        PdfInspectorSelfCheck.Run(directory.Path);

        Assert.AreEqual(0, Directory.GetFiles(directory.Path).Length);
        Assert.ThrowsExactly<ArgumentException>(() => PdfInspectorSelfCheck.Run(" "));
        Assert.ThrowsExactly<ArgumentNullException>(() => PdfInspectorSelfCheck.Run(directory.Path, null!));
        Assert.ThrowsExactly<InvalidOperationException>(() => PdfInspectorSelfCheck.Run(directory.Path, _ => new PdfDocumentInspection(
            false,
            false,
            false,
            0,
            0,
            0,
            new Dictionary<string, string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<PdfObjectSummary>())));
    }

    private static string CreateRichPdf()
        => "%PDF-1.4\n"
            + "1 0 obj\n<< /Type /Page /Title (A \\(nested\\) title) /XObject << /Im0 4 0 R /Fm1 6 0 R >> >>\nendobj\n"
            + "2 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /FontName /HelveticaNeue /Encoding /WinAnsiEncoding /ToUnicode 7 0 R >>\nendobj\n"
            + "3 0 obj\n<< /Type /XObject /Subtype /Image /Width 10 /Height 10 >>\nendobj\n"
            + "4 0 obj\n<< /Type /Metadata >>\nstream\n<?xml version=\"1.0\"?><x:xmpmeta />\nendstream\nendobj\n"
            + "5 0 obj\n<< /Length 80 /ToUnicode 7 0 R >>\nstream\nq 1 0 0 1 0 0 cm BT /F1 12 Tf (text) Tj ET 0 0 m 10 10 l S /Im0 Do BI ID abc EI Q\nendstream\nendobj\n"
            + "6 0 obj\n<< /Type /Unknown >>\nendobj\n";

    private static byte[] CompressZlib(string content)
    {
        using MemoryStream output = new();
        using (ZLibStream stream = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            stream.Write(Encoding.Latin1.GetBytes(content));
        }

        return output.ToArray();
    }

    private static byte[] CompressDeflate(string content)
    {
        using MemoryStream output = new();
        using (DeflateStream stream = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            stream.Write(Encoding.Latin1.GetBytes(content));
        }

        return output.ToArray();
    }

    private sealed class TestDirectory : IDisposable
    {
        public TestDirectory()
        {
            Path = System.IO.Path.Combine(Environment.CurrentDirectory, "pdf-markdown-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public string Write(string fileName, byte[] content)
        {
            string filePath = System.IO.Path.Combine(Path, fileName);
            File.WriteAllBytes(filePath, content);
            return filePath;
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
