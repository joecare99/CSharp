using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Models.Interfaces;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.ContentAnalysis;

/// <summary>
/// Provides a first local image analysis tool based on file metadata and simple file signature heuristics.
/// </summary>
public sealed class ImageAnalysisTool : IContentAnalysisTool
{
    private readonly IFile _file;

    private static readonly OllamaToolSchema ToolSchema = new()
    {
        Summary = "Accepts a content analysis request for an image file and returns a structured local evaluation based on file metadata.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "contentKind",
                Type = "number",
                Description = "Must be Image for this analysis tool.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "sourceKind",
                Type = "number",
                Description = "Must be FilePath for this analysis tool.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "mediaType",
                Type = "string",
                Description = "Should describe an image media type such as image/png or image/jpeg.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "filePath",
                Type = "string",
                Description = "The image file path to inspect.",
                Required = true,
            },
        ],
    };

    /// <inheritdoc/>
    public string Name => "analyze_image";

    /// <inheritdoc/>
    public string Description => "Evaluates image files with local metadata heuristics such as size, signature, dimensions, and likely usability hints.";

    /// <inheritdoc/>
    public OllamaToolSchema Schema => ToolSchema;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageAnalysisTool"/> class.
    /// </summary>
    /// <param name="file">The file abstraction used for metadata access.</param>
    public ImageAnalysisTool(IFile? file = null)
    {
        _file = file ?? new BaseLib.Models.FileProxy();
    }

    /// <inheritdoc/>
    public ContentAnalysisRequestValidationResult Validate(ContentAnalysisRequest? request)
    {
        ContentAnalysisRequestValidationResult baseResult = ContentAnalysisRequestValidator.Validate(request);
        List<ContentAnalysisValidationIssue> issues = [.. baseResult.Issues];

        if (request is null)
        {
            return baseResult;
        }

        if (request.ContentKind != OllamaContentKind.Image)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.ContentKind),
                Code = "contentKind.image.required",
                Message = "The image analysis tool only supports image requests.",
            });
        }

        if (request.SourceKind != OllamaContentSourceKind.FilePath)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.SourceKind),
                Code = "sourceKind.image.filePath.required",
                Message = "The image analysis tool currently requires a file path source.",
            });
        }

        if (!request.MediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.MediaType),
                Code = "mediaType.image.invalid",
                Message = "The image analysis tool requires an image media type.",
            });
        }

        if (!string.IsNullOrWhiteSpace(request.FilePath) && !_file.GetFileInfo(request.FilePath).Exists)
        {
            issues.Add(new ContentAnalysisValidationIssue
            {
                Field = nameof(ContentAnalysisRequest.FilePath),
                Code = "filePath.notFound",
                Message = "The referenced image file does not exist.",
            });
        }

        return new ContentAnalysisRequestValidationResult
        {
            IsValid = issues.Count == 0,
            Issues = issues,
        };
    }

    /// <inheritdoc/>
    public Task<ContentAnalysisResult> AnalyzeAsync(ContentAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        IFileInfo fileInfo = _file.GetFileInfo(request.FilePath);
        ImageInspection imageInspection = InspectImageFile(fileInfo.FullName);
        long fileSizeBytes = fileInfo.Length;

        List<ContentAnalysisFinding> findings = [];
        List<ContentAnalysisSuggestion> suggestions = [];
        double score = 1.0;

        findings.Add(new ContentAnalysisFinding
        {
            Title = "Image file detected",
            Message = "The file could be inspected as an image candidate.",
            Severity = ContentAnalysisSeverity.Info,
            Evidence = $"Extension: {fileInfo.Extension}, Size: {fileSizeBytes} bytes",
        });

        if (!string.IsNullOrWhiteSpace(imageInspection.Format))
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Image format detected",
                Message = "A known image signature was detected.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Format: {imageInspection.Format}",
            });
        }
        else
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Unknown image signature",
                Message = "The file extension suggests an image, but no known signature was confirmed by the local heuristic.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Media type: {request.MediaType}",
            });
            score -= 0.15;
        }

        if (fileSizeBytes < 10 * 1024)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Very small image file",
                Message = "The file size is very small, which may indicate a thumbnail, icon, or low-detail image.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Size: {fileSizeBytes} bytes",
            });
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Use a larger source image",
                Description = "Consider providing a higher-resolution or less compressed image when more visual detail is required.",
                Priority = "medium",
            });
            score -= 0.2;
        }

        if (imageInspection.Width is int width && imageInspection.Height is int height)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Image dimensions detected",
                Message = "The local file inspection detected image dimensions.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"{width} x {height}",
            });

            if (width < 256 || height < 256)
            {
                findings.Add(new ContentAnalysisFinding
                {
                    Title = "Low image resolution",
                    Message = "The image dimensions are quite small for detailed visual inspection.",
                    Severity = ContentAnalysisSeverity.Warning,
                    Evidence = $"{width} x {height}",
                });
                suggestions.Add(new ContentAnalysisSuggestion
                {
                    Title = "Provide a higher-resolution image",
                    Description = "Use a larger image if the goal is to assess detailed content or small visual elements.",
                    Priority = "high",
                });
                score -= 0.2;
            }
        }
        else
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Image dimensions unavailable",
                Message = "The local heuristic could not determine image dimensions from the file header.",
                Severity = ContentAnalysisSeverity.Warning,
                Evidence = $"Format: {imageInspection.Format}",
            });
            score -= 0.1;
        }

        if (imageInspection.BitsPerPixel is int bitsPerPixel)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Image color depth detected",
                Message = "The local file inspection detected a bit depth value.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Bits per pixel: {bitsPerPixel}",
            });
        }

        if (request.ImageMetadata.PixelWidth is int requestedWidth && request.ImageMetadata.PixelHeight is int requestedHeight)
        {
            findings.Add(new ContentAnalysisFinding
            {
                Title = "Caller-supplied image metadata present",
                Message = "The request already contains image metadata that can be compared with local inspection results.",
                Severity = ContentAnalysisSeverity.Info,
                Evidence = $"Requested: {requestedWidth} x {requestedHeight}",
            });
        }

        if (suggestions.Count == 0)
        {
            suggestions.Add(new ContentAnalysisSuggestion
            {
                Title = "Add semantic image understanding later",
                Description = "This first version evaluates image metadata only. A later milestone can add OCR or model-based visual understanding.",
                Priority = "low",
            });
        }

        ContentAnalysisResult result = new()
        {
            Summary = BuildSummary(score, imageInspection.Format, imageInspection.Width, imageInspection.Height),
            Score = Math.Clamp(score, 0.0, 1.0),
            Confidence = CalculateConfidence(imageInspection.Width, imageInspection.Height, !string.IsNullOrWhiteSpace(imageInspection.Format)),
            Rationale = $"The local image analysis evaluated file metadata for '{fileInfo.Name}', including file size, detected format signature, and available image dimensions.",
            Findings = findings,
            Suggestions = suggestions,
        };

        return Task.FromResult(result);
    }

    private static string BuildSummary(double score, string format, int? width, int? height)
    {
        string formatText = string.IsNullOrWhiteSpace(format) ? "unknown format" : format;
        string sizeText = width is int w && height is int h ? $"{w} x {h}" : "unknown dimensions";

        if (score >= 0.85)
        {
            return $"The image file looks structurally healthy with {formatText} metadata and {sizeText}.";
        }

        if (score >= 0.6)
        {
            return $"The image file is usable but shows metadata signals that may limit deeper visual analysis.";
        }

        return "The image file would benefit from a higher-quality or more clearly structured source before deeper visual analysis.";
    }

    private static double CalculateConfidence(int? width, int? height, bool hasKnownFormat)
    {
        double confidence = hasKnownFormat ? 0.7 : 0.45;
        if (width is > 0 && height is > 0)
        {
            confidence += 0.15;
        }

        return Math.Clamp(confidence, 0.0, 0.95);
    }

    private static ImageInspection InspectImageFile(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        using BinaryReader reader = new(stream);

        if (stream.Length >= 24)
        {
            byte[] pngSignature = reader.ReadBytes(8);
            if (pngSignature.Length == 8
                && pngSignature[0] == 137
                && pngSignature[1] == 80
                && pngSignature[2] == 78
                && pngSignature[3] == 71)
            {
                stream.Position = 16;
                int width = ReadBigEndianInt32(reader);
                int height = ReadBigEndianInt32(reader);
                return new ImageInspection("PNG", width, height, null);
            }
        }

        stream.Position = 0;
        if (stream.Length >= 10)
        {
            byte[] gifSignature = reader.ReadBytes(6);
            if (gifSignature.Length == 6
                && gifSignature[0] == 'G'
                && gifSignature[1] == 'I'
                && gifSignature[2] == 'F')
            {
                int width = reader.ReadUInt16();
                int height = reader.ReadUInt16();
                return new ImageInspection("GIF", width, height, null);
            }
        }

        stream.Position = 0;
        if (stream.Length >= 30)
        {
            byte[] bmpSignature = reader.ReadBytes(2);
            if (bmpSignature.Length == 2 && bmpSignature[0] == 'B' && bmpSignature[1] == 'M')
            {
                stream.Position = 18;
                int width = reader.ReadInt32();
                int height = Math.Abs(reader.ReadInt32());
                stream.Position = 28;
                int bitsPerPixel = reader.ReadUInt16();
                return new ImageInspection("BMP", width, height, bitsPerPixel);
            }
        }

        stream.Position = 0;
        if (stream.Length >= 2)
        {
            byte first = reader.ReadByte();
            byte second = reader.ReadByte();
            if (first == 0xFF && second == 0xD8)
            {
                return new ImageInspection("JPEG", null, null, null);
            }
        }

        return new ImageInspection(string.Empty, null, null, null);
    }

    private static int ReadBigEndianInt32(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(4);
        if (bytes.Length < 4)
        {
            return 0;
        }

        return (bytes[0] << 24)
            | (bytes[1] << 16)
            | (bytes[2] << 8)
            | bytes[3];
    }

    private readonly record struct ImageInspection(string Format, int? Width, int? Height, int? BitsPerPixel);
}
