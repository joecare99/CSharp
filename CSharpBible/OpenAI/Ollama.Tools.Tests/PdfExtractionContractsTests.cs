using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class PdfExtractionContractsTests
{
    [TestMethod]
    public void Request_DefaultsToEmptyState()
    {
        PdfExtractionRequest request = new();

        Assert.AreEqual(string.Empty, request.FilePath);
        Assert.IsNull(request.Password);
        Assert.IsNull(request.FileMetadata);
    }

    [TestMethod]
    public void Result_SuccessFactoryCapturesPayload()
    {
        ContentAnalysisFileMetadata fileMetadata = new()
        {
            FileName = "sample.pdf",
            Extension = ".pdf",
            SizeBytes = 1024,
        };

        PdfExtractionResult result = PdfExtractionResult.Success("sample.pdf", "hello world", 3, fileMetadata);

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("sample.pdf", result.FilePath);
        Assert.AreEqual("hello world", result.ExtractedText);
        Assert.AreEqual(3, result.PageCount);
        Assert.AreEqual(fileMetadata, result.FileMetadata);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Result_FailureFactoryCapturesError()
    {
        PdfExtractionResult result = PdfExtractionResult.Failure("broken.pdf", "Unsupported PDF format");

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("broken.pdf", result.FilePath);
        Assert.AreEqual(string.Empty, result.ExtractedText);
        Assert.IsNull(result.PageCount);
        Assert.AreEqual("Unsupported PDF format", result.ErrorMessage);
    }
}