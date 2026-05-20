using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ContentAnalysisRequestValidatorTests
{
    [TestMethod]
    public void Validate_ReturnsFailureForNullRequest()
    {
        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(null);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Issues.Count);
        Assert.AreEqual("request.null", result.Issues[0].Code);
    }

    [TestMethod]
    public void Validate_ReturnsSuccessForInlineTextRequest()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "hello world",
            Criteria =
            [
                new ContentAnalysisCriterion
                {
                    Name = "clarity",
                    Description = "Check whether the text is easy to understand.",
                    Weight = 0.8,
                },
            ],
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Issues.Count);
    }

    [TestMethod]
    public void Validate_ReturnsFailureForSourceCodeWithoutLanguage()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "public class Sample { }",
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "language.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForImageRequestWithNonImageMediaType()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "text/plain",
            FilePath = "image.png",
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.image.invalid"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForMissingInlineContent()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "content.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForMissingFilePath()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "text/plain",
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "filePath.required"));
    }

    [TestMethod]
    [DataRow(-0.1)]
    [DataRow(1.1)]
    public void Validate_ReturnsFailureForCriterionWeightOutsideRange(double weight)
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "hello world",
            Criteria =
            [
                new ContentAnalysisCriterion
                {
                    Name = "clarity",
                    Weight = weight,
                },
            ],
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "criteria.weight.range"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForCriterionWithoutName()
    {
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "hello world",
            Criteria =
            [
                new ContentAnalysisCriterion
                {
                    Name = string.Empty,
                },
            ],
        };

        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "criteria.name.required"));
    }
}
