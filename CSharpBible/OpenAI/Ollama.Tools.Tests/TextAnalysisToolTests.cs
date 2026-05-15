using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class TextAnalysisToolTests
{
    [TestMethod]
    public void Validate_ReturnsFailureForNonTextRequest()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "image/png",
            Content = "not-used",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "contentKind.text.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNonTextMediaType()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "application/json",
            Content = "hello world",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.text.invalid"));
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsInformationalResultForBalancedText()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "This is a concise sample. It has two sentences and enough words for a basic analysis.",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Score >= 0.85);
        Assert.IsTrue(result.Findings.Any(static finding => finding.Severity == ContentAnalysisSeverity.Info));
        StringAssert.Contains(result.Summary, "text");
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningsForVeryShortText()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "Too short",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Score < 1.0);
        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Very short text"));
        Assert.IsTrue(result.Suggestions.Any(static suggestion => suggestion.Title == "Add more context"));
        StringAssert.Contains(result.Summary, "benefit");
    }

    [TestMethod]
    public async Task AnalyzeAsync_DetectsBulletStructure()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = "Overview:\n- first point\n- second point\nThis closing sentence explains the list.",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Structured bullet points detected"));
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsMissingParagraphStructureForLongText()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = BuildLongText(90),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Missing paragraph structure"));
        Assert.IsTrue(result.Suggestions.Any(static suggestion => suggestion.Title == "Add paragraphs"));
    }

    [TestMethod]
    public void Metadata_And_Schema_AreExposed()
    {
        TextAnalysisTool tool = new();

        Assert.AreEqual("analyze_text", tool.Name);
        StringAssert.Contains(tool.Description, "readability");
        Assert.AreEqual("Accepts a content analysis request for plain text and returns a structured local evaluation.", tool.Schema.Summary);
        Assert.AreEqual(5, tool.Schema.Parameters.Count);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsConfidenceForFortyPlusWords()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = BuildLongText(42),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.AreEqual(0.75, result.Confidence);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsHighConfidenceForVeryLongText()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = BuildLongText(130),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.AreEqual(0.9, result.Confidence);
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNullRequest()
    {
        TextAnalysisTool tool = new();

        ContentAnalysisRequestValidationResult result = tool.Validate(null);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "request.null"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForEmptyInlineContent()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = string.Empty,
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "content.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForEmptyFilePath()
    {
        TextAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.FilePath,
            MediaType = "text/plain",
            FilePath = string.Empty,
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "filePath.required"));
    }

    private static string BuildLongText(int wordCount)
    {
        string[] words = new string[wordCount];

        for (int index = 0; index < wordCount; index++)
        {
            words[index] = "word" + index;
        }

        return string.Join(" ", words);
    }
}
