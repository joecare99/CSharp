using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ImageAnalysisToolTests
{
    [TestMethod]
    public void Validate_ReturnsFailureForNullRequest()
    {
        ImageAnalysisTool tool = new();

        ContentAnalysisRequestValidationResult result = tool.Validate(null);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "request.null"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNonImageContentKind()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "image/png",
            FilePath = "sample.png",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "contentKind.image.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForInlineImageRequest()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "image/png",
            Content = "not-used",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "sourceKind.image.filePath.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNonImageMediaType()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "text/plain",
            FilePath = "sample.png",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.image.invalid"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForMissingFile()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "image/png",
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png"),
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "filePath.notFound"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForWrongSourceKind()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "image/png",
            Content = "not-used",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "sourceKind.image.filePath.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForWrongMediaType()
    {
        ImageAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "application/json",
            FilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png"),
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.image.invalid"));
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsBitsPerPixelFindingForBmp()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".bmp");
        await File.WriteAllBytesAsync(tempFilePath, CreateTinyBmpBytes());

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/bmp",
                FilePath = tempFilePath,
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Image color depth detected"));
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
    public async Task AnalyzeAsync_ReturnsHighSummaryForKnownFormatAndDimensions()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".bmp");
        await File.WriteAllBytesAsync(tempFilePath, CreateHealthyBmpBytes());

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/bmp",
                FilePath = tempFilePath,
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            StringAssert.Contains(result.Summary, "structurally healthy");
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
    public async Task AnalyzeAsync_ReturnsLowerSummaryForVerySmallImage()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        await File.WriteAllBytesAsync(tempFilePath, CreateTinyPngBytes());

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/png",
                FilePath = tempFilePath,
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            Assert.IsTrue(result.Score < 0.85);
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
    public void Metadata_And_Schema_AreExposed()
    {
        ImageAnalysisTool tool = new();

        Assert.AreEqual("analyze_image", tool.Name);
        StringAssert.Contains(tool.Description, "image files");
        Assert.AreEqual("Accepts a content analysis request for an image file and returns a structured local evaluation based on file metadata.", tool.Schema.Summary);
        Assert.AreEqual(4, tool.Schema.Parameters.Count);
        Assert.AreEqual("contentKind", tool.Schema.Parameters[0].Name);
        Assert.AreEqual("filePath", tool.Schema.Parameters[3].Name);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningSummaryForUnknownSignature()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".img");
        await File.WriteAllBytesAsync(tempFilePath, [0x00, 0x01, 0x02, 0x03, 0x04]);

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/png",
                FilePath = tempFilePath,
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            StringAssert.Contains(result.Summary, "benefit");
            Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Unknown image signature"));
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
    public async Task AnalyzeAsync_ReturnsImageDimensionWarningForLargeUnknownFile()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".img");
        byte[] bytes = new byte[12 * 1024];
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        await File.WriteAllBytesAsync(tempFilePath, bytes);

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/jpeg",
                FilePath = tempFilePath,
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Image dimensions unavailable"));
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
    public async Task AnalyzeAsync_ReturnsStructuredImageMetadataResult()
    {
        string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
        await File.WriteAllBytesAsync(tempFilePath, CreateTinyPngBytes());

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisRequest request = new()
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/png",
                FilePath = tempFilePath,
                ImageMetadata = new ContentAnalysisImageMetadata
                {
                    PixelWidth = 2,
                    PixelHeight = 2,
                    Format = "PNG",
                },
            };

            ContentAnalysisResult result = await tool.AnalyzeAsync(request);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Findings.Count > 0);
            Assert.IsTrue(result.Suggestions.Count > 0);
            StringAssert.Contains(result.Summary, "image file");
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    private static byte[] CreateTinyPngBytes()
    {
        return
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D,
            0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x02,
            0x08, 0x02, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x49, 0x45, 0x4E, 0x44,
            0xAE, 0x42, 0x60, 0x82,
        ];
    }

    private static byte[] CreateTinyBmpBytes()
    {
        return
        [
            0x42, 0x4D,
            0x46, 0x00, 0x00, 0x00,
            0x00, 0x00,
            0x00, 0x00,
            0x36, 0x00, 0x00, 0x00,
            0x28, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00,
            0x01, 0x00,
            0x18, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x10, 0x00, 0x00, 0x00,
            0x13, 0x0B, 0x00, 0x00,
            0x13, 0x0B, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
        ];
    }

    private static byte[] CreateHealthyBmpBytes()
    {
        byte[] bytes = new byte[12 * 1024];

        bytes[0] = 0x42;
        bytes[1] = 0x4D;
        BitConverter.GetBytes(bytes.Length).CopyTo(bytes, 2);
        BitConverter.GetBytes(54).CopyTo(bytes, 10);
        BitConverter.GetBytes(40).CopyTo(bytes, 14);
        BitConverter.GetBytes(300).CopyTo(bytes, 18);
        BitConverter.GetBytes(300).CopyTo(bytes, 22);
        BitConverter.GetBytes((short)1).CopyTo(bytes, 26);
        BitConverter.GetBytes((short)24).CopyTo(bytes, 28);

        return bytes;
    }
}
