using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ContentAnalysisRequestMetadataTests
{
    [TestMethod]
    public void Validate_ReturnsFailureForImageInlineSource()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "image/png",
            Content = "not-used",
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "sourceKind.image.filePath.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNegativeFileSizeMetadata()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "hello world",
            FileMetadata = new ContentAnalysisFileMetadata
            {
                SizeBytes = -1,
            },
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "fileMetadata.sizeBytes.invalid"));
    }

    [DataTestMethod]
    [DataRow(-1, 100, 24, "imageMetadata.pixelWidth.invalid")]
    [DataRow(100, -1, 24, "imageMetadata.pixelHeight.invalid")]
    [DataRow(100, 100, -1, "imageMetadata.bitsPerPixel.invalid")]
    public void Validate_ReturnsFailureForNegativeImageMetadata(int pixelWidth, int pixelHeight, int bitsPerPixel, string expectedCode)
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "image/png",
            FilePath = "sample.png",
            ImageMetadata = new ContentAnalysisImageMetadata
            {
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                BitsPerPixel = bitsPerPixel,
            },
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(issue => issue.Code == expectedCode));
    }
}
