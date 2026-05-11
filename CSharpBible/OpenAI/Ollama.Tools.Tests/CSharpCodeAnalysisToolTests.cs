using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class CSharpCodeAnalysisToolTests
{
    [TestMethod]
    public void Validate_ReturnsFailureForNonSourceCodeRequest()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Language = "csharp",
            Content = "public class Sample { }",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "contentKind.sourceCode.required"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNonCSharpLanguage()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "vb",
            Content = "Public Class Sample\nEnd Class",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "language.csharp.required"));
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsInformationalResultForStructuredCode()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = "using System;\nnamespace Demo;\n\npublic sealed class Sample\n{\n    public void Run()\n    {\n        ArgumentNullException.ThrowIfNull(this);\n    }\n}",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Score >= 0.85);
        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Structured C# source detected"));
        StringAssert.Contains(result.Summary, "C# source");
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningsForConsoleAndTodoUsage()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "c#",
            Content = "using System;\n\n// TODO: refactor\npublic class Sample\n{\n    public void Run()\n    {\n        Console.WriteLine(\"hello\");\n    }\n}",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Score < 1.0);
        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "TODO markers detected"));
        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Direct console output detected"));
        Assert.IsTrue(result.Suggestions.Any(static suggestion => suggestion.Title == "Abstract console output"));
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningForMissingNamespace()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "cs",
            Content = "public class Sample\n{\n    public void Run()\n    {\n    }\n}",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Missing namespace declaration"));
        Assert.IsTrue(result.Suggestions.Any(static suggestion => suggestion.Title == "Declare a namespace"));
    }
}
