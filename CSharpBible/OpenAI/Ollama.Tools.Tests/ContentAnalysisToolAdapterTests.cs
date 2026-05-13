using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;
using Ollama.Tools.Tests.TestDoubles;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ContentAnalysisToolAdapterTests
{
    [TestMethod]
    public void Constructor_ThrowsForNullTool()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new ContentAnalysisToolAdapter(null!));
    }

    [TestMethod]
    public void Schema_DelegatesToInnerTool()
    {
        IOllamaTool adapter = new ContentAnalysisToolAdapter(new TextAnalysisTool());

        StringAssert.Contains(adapter.Schema.Summary, "plain text");
    }

    [TestMethod]
    public void NameAndDescription_DelegateToInnerTool()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_custom",
            Description = "Analyzes custom content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        Assert.AreEqual("analyze_custom", adapter.Name);
        Assert.AreEqual("Analyzes custom content.", adapter.Description);
    }

    [TestMethod]
    public void Validate_ReturnsFailureForEmptyInput()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolValidationResult result = adapter.Validate(string.Empty);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("The content analysis tool input must not be empty."));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForInvalidJson()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolValidationResult result = adapter.Validate("not json");

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("The content analysis tool input must be valid JSON."));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForInvalidRequest()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
            ValidationResult = new ContentAnalysisRequestValidationResult
            {
                IsValid = false,
                Issues =
                [
                    new ContentAnalysisValidationIssue
                    {
                        Field = nameof(ContentAnalysisRequest.Content),
                        Code = "content.required",
                        Message = "Inline content must not be empty.",
                    },
                ],
            },
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolValidationResult result = adapter.Validate("{\"contentKind\":0,\"sourceKind\":0,\"mediaType\":\"text/plain\",\"content\":\"hello\"}");

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("Inline content must not be empty."));
        Assert.IsNotNull(tool.LastValidatedRequest);
    }

    [TestMethod]
    public void Validate_ReturnsFailureForJsonNull()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolValidationResult result = adapter.Validate("null");

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("The content analysis tool input must deserialize to a content analysis request."));
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsFailureForEmptyInput()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolResult result = await adapter.ExecuteAsync(string.Empty);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("The content analysis tool input must not be empty.", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsFailureForJsonNull()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolResult result = await adapter.ExecuteAsync("null");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("The content analysis tool input must deserialize to a content analysis request.", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsFailureForInvalidRequest()
    {
        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
            ValidationResult = new ContentAnalysisRequestValidationResult
            {
                IsValid = false,
                Issues =
                [
                    new ContentAnalysisValidationIssue
                    {
                        Field = nameof(ContentAnalysisRequest.Content),
                        Code = "content.required",
                        Message = "Inline content must not be empty.",
                    },
                    new ContentAnalysisValidationIssue
                    {
                        Field = nameof(ContentAnalysisRequest.MediaType),
                        Code = "mediaType.required",
                        Message = "The media type must not be empty.",
                    },
                ],
            },
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolResult result = await adapter.ExecuteAsync("{\"contentKind\":0,\"sourceKind\":0,\"mediaType\":\"text/plain\",\"content\":\"hello\"}");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Inline content must not be empty. The media type must not be empty.", result.Output);
        Assert.IsNotNull(tool.LastValidatedRequest);
        Assert.IsNull(tool.LastAnalyzedRequest);
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsSerializedAnalysisResult()
    {
        JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
        };

        TestContentAnalysisTool tool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
            Result = new ContentAnalysisResult
            {
                Summary = "Looks good.",
                Score = 0.9,
                Confidence = 0.8,
                Rationale = "The content is clear.",
            },
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(tool);

        OllamaToolResult result = await adapter.ExecuteAsync("{\"contentKind\":0,\"sourceKind\":0,\"mediaType\":\"text/plain\",\"content\":\"hello\"}");
        ContentAnalysisResult? serialized = JsonSerializer.Deserialize<ContentAnalysisResult>(result.Output, serializerOptions);

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(serialized);
        Assert.AreEqual("Looks good.", serialized.Summary);
        Assert.AreEqual("hello", tool.LastAnalyzedRequest?.Content);
    }
}
