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
