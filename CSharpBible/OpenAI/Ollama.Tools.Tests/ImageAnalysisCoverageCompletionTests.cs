using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ImageAnalysisCoverageCompletionTests
{
    [TestMethod]
    public async Task AnalyzeAsync_HandlesPartialCallerMetadataAndZeroDimensions()
    {
        await AnalyzeAsync(2, 2, new ContentAnalysisImageMetadata
        {
            PixelWidth = 2,
        });
        await AnalyzeAsync(2, 2, new ContentAnalysisImageMetadata
        {
            PixelHeight = 2,
        });
        await AnalyzeAsync(0, 2, new ContentAnalysisImageMetadata());
        await AnalyzeAsync(2, 0, new ContentAnalysisImageMetadata());
    }

    [TestMethod]
    public async Task AnalyzeAsync_HandlesPartialInspectedDimensions()
    {
        await AnalyzeWithInspectionAsync(2, null);
        await AnalyzeWithInspectionAsync(null, 2);
        await AnalyzeWithInspectionAsync(0, 2);
        await AnalyzeWithInspectionAsync(2, 0);
    }

    private static async Task AnalyzeAsync(int width, int height, ContentAnalysisImageMetadata imageMetadata)
    {
        string filePath = CreateArtifactPath(".png");
        await File.WriteAllBytesAsync(filePath, CreatePngBytes(width, height));

        try
        {
            ImageAnalysisTool tool = new();
            ContentAnalysisResult result = await tool.AnalyzeAsync(new ContentAnalysisRequest
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/png",
                FilePath = filePath,
                ImageMetadata = imageMetadata,
            });

            Assert.IsNotNull(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    private static async Task AnalyzeWithInspectionAsync(int? width, int? height)
    {
        string filePath = CreateArtifactPath(".img");
        await File.WriteAllBytesAsync(filePath, [0x00]);

        try
        {
            ImageAnalysisTool tool = new(
                null,
                _ => new ImageAnalysisTool.ImageInspection("PNG", width, height, null));
            ContentAnalysisResult result = await tool.AnalyzeAsync(new ContentAnalysisRequest
            {
                ContentKind = OllamaContentKind.Image,
                SourceKind = OllamaContentSourceKind.FilePath,
                MediaType = "image/png",
                FilePath = filePath,
            });

            Assert.IsNotNull(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    private static byte[] CreatePngBytes(int width, int height)
    {
        byte[] bytes =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x0D,
            0x49, 0x48, 0x44, 0x52,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
        ];
        WriteBigEndian(bytes, 16, width);
        WriteBigEndian(bytes, 20, height);
        return bytes;
    }

    private static void WriteBigEndian(byte[] bytes, int offset, int value)
    {
        bytes[offset] = (byte)(value >> 24);
        bytes[offset + 1] = (byte)(value >> 16);
        bytes[offset + 2] = (byte)(value >> 8);
        bytes[offset + 3] = (byte)value;
    }

    private static string CreateArtifactPath(string extension)
    {
        const string ArtifactDirectory = "Ollama.Tools.Tests\\TestResults\\CoverageArtifacts";
        Directory.CreateDirectory(ArtifactDirectory);
        return Path.Combine(ArtifactDirectory, Guid.NewGuid().ToString("N") + extension);
    }
}
