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
    public void Validate_ReturnsFailureForNonTextMediaType()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "application/json",
            Language = "csharp",
            Content = "public class Sample { }",
        };

        ContentAnalysisRequestValidationResult result = tool.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.csharp.invalid"));
    }

    [TestMethod]
    public void Validate_ReturnsFailureForNullRequest()
    {
        CSharpCodeAnalysisTool tool = new();

        ContentAnalysisRequestValidationResult result = tool.Validate(null);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "request.null"));
    }

    [TestMethod]
    public void Metadata_And_Schema_AreExposed()
    {
        CSharpCodeAnalysisTool tool = new();

        Assert.AreEqual("analyze_csharp", tool.Name);
        StringAssert.Contains(tool.Description, "maintainability");
        Assert.AreEqual("Accepts a content analysis request for C# source code and returns a structured local evaluation.", tool.Schema.Summary);
        Assert.AreEqual(5, tool.Schema.Parameters.Count);
        Assert.AreEqual("contentKind", tool.Schema.Parameters[0].Name);
        Assert.AreEqual("filePath", tool.Schema.Parameters[4].Name);
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
        StringAssert.Contains(result.Summary, "usable");
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

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsLowSummaryForUnstructuredCode()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = "public class Sample\n{\n    // TODO: finish\n    public void Run() => Console.WriteLine(\"" + new string('x', 140) + "\");\n}\n" + new string('\n', 6),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        StringAssert.Contains(result.Summary, "refactoring");
        Assert.IsTrue(result.Score < 0.6);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsConfidenceForThirtyPlusLines()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = BuildLinesContent(35, includeNamespace: true, includeMethods: true),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.AreEqual(0.75, result.Confidence);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsConfidenceForTenPlusLines()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = BuildLinesContent(12, includeNamespace: true, includeMethods: true),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.AreEqual(0.6, result.Confidence);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsHighConfidenceForLongCode()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = BuildLinesContent(90, includeNamespace: true, includeMethods: true),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.AreEqual(0.9, result.Confidence);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningForNoMethodsAndNoNamespace()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = "public class Sample\n{\n    // TODO: fill in\n}\n" + new string('x', 130),
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "No methods detected"));
        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Missing namespace declaration"));
        Assert.IsTrue(result.Score < 0.6);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsWarningForLongLines()
    {
        CSharpCodeAnalysisTool tool = new();
        ContentAnalysisRequest request = new()
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = "namespace Demo;\npublic class Sample\n{\n    public void Run() => System.Console.WriteLine(\"" + new string('x', 140) + "\");\n}",
        };

        ContentAnalysisResult result = await tool.AnalyzeAsync(request);

        Assert.IsTrue(result.Findings.Any(static finding => finding.Title == "Long lines detected"));
    }

    private static string BuildLinesContent(int lineCount, bool includeNamespace, bool includeMethods)
    {
        string[] lines = new string[lineCount];
        int index = 0;

        if (includeNamespace)
        {
            lines[index++] = "namespace Demo;";
        }

        lines[index++] = "using System;";
        lines[index++] = "public sealed class Sample";
        lines[index++] = "{";

        if (includeMethods)
        {
            lines[index++] = "    public void Run() { }";
        }

        while (index < lineCount)
        {
            lines[index++] = "    // filler";
        }

        return string.Join("\n", lines);
    }
}
