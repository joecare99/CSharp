using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class ContentAnalysisRouterTests
{
    [TestMethod]
    public void Constructor_ThrowsForNullTextAnalysisTool()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new ContentAnalysisRouter(null!, new CSharpCodeAnalysisTool()));

    [TestMethod]
    public void Constructor_ThrowsForNullCSharpCodeAnalysisTool()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new ContentAnalysisRouter(new TextAnalysisTool(), null!));

    [TestMethod]
    public void Route_UsesExplicitTextMode()
    {
        TextAnalysisTool textAnalysisTool = new();
        CSharpCodeAnalysisTool cSharpCodeAnalysisTool = new();
        ContentAnalysisRouter router = new(textAnalysisTool, cSharpCodeAnalysisTool);

        ContentAnalysisRoutingContext context = router.Route("hello world", "sample.cs", ContentAnalysisMode.Text);

        Assert.AreSame(textAnalysisTool, context.Tool);
        Assert.AreEqual(OllamaContentKind.Text, context.Decision.ContentKind);
        Assert.AreEqual("Text analysis", context.Decision.AnalysisLabel);
        Assert.AreEqual("text/plain", context.Decision.MediaType);
        Assert.AreEqual(string.Empty, context.Decision.Language);
        Assert.AreEqual("Text analysis was selected explicitly.", context.Decision.Reason);
        Assert.AreEqual("sample.cs", context.Request.DisplayName);
        Assert.AreEqual("hello world", context.Request.Content);
    }

    [TestMethod]
    public void Route_UsesExplicitCSharpMode()
    {
        TextAnalysisTool textAnalysisTool = new();
        CSharpCodeAnalysisTool cSharpCodeAnalysisTool = new();
        ContentAnalysisRouter router = new(textAnalysisTool, cSharpCodeAnalysisTool);

        ContentAnalysisRoutingContext context = router.Route("plain text", "notes.txt", ContentAnalysisMode.CSharp);

        Assert.AreSame(cSharpCodeAnalysisTool, context.Tool);
        Assert.AreEqual(OllamaContentKind.SourceCode, context.Decision.ContentKind);
        Assert.AreEqual("C# source analysis", context.Decision.AnalysisLabel);
        Assert.AreEqual("text/x-csharp", context.Decision.MediaType);
        Assert.AreEqual("csharp", context.Decision.Language);
        Assert.AreEqual("C# source analysis was selected explicitly.", context.Decision.Reason);
    }

    [TestMethod]
    public void Route_AutoDetectsCSharpFromFileExtension()
    {
        ContentAnalysisRouter router = CreateRouter();

        ContentAnalysisRoutingContext context = router.Route("hello world", "Sample.CS");

        Assert.AreEqual(OllamaContentKind.SourceCode, context.Decision.ContentKind);
        Assert.AreEqual("The .cs file extension indicates C# source code.", context.Decision.Reason);
    }

    [TestMethod]
    public void Route_AutoDetectsCSharpFromContentPatterns()
    {
        ContentAnalysisRouter router = CreateRouter();
        string inputText = "namespace Demo; public class Sample { }";

        ContentAnalysisRoutingContext context = router.Route(inputText, "notes.txt");

        Assert.AreEqual(OllamaContentKind.SourceCode, context.Decision.ContentKind);
        Assert.AreEqual("C# language patterns were detected in the input content.", context.Decision.Reason);
        Assert.AreEqual("csharp", context.Request.Language);
    }

    [TestMethod]
    public void Route_AutoFallsBackToTextAndNormalizesInput()
    {
        ContentAnalysisRouter router = CreateRouter();

        ContentAnalysisRoutingContext context = router.Route(null!, " ");

        Assert.AreEqual(OllamaContentKind.Text, context.Decision.ContentKind);
        Assert.AreEqual("inline input", context.Request.DisplayName);
        Assert.AreEqual(string.Empty, context.Request.Content);
        Assert.AreEqual("No strong C# indicators were found, so the input was treated as plain text.", context.Decision.Reason);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ThrowsForInvalidRoutedRequest()
    {
        ContentAnalysisRouter router = CreateRouter();

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => router.AnalyzeAsync(string.Empty, " ", ContentAnalysisMode.Text));

        StringAssert.Contains(exception.Message, "Inline content must not be empty.");
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsExecutionResultForTextContent()
    {
        ContentAnalysisRouter router = CreateRouter();
        string inputText = "This is a concise sample. It has two sentences and enough words for a basic analysis.";

        ContentAnalysisExecutionResult result = await router.AnalyzeAsync(inputText, "notes.txt", ContentAnalysisMode.Auto);

        Assert.AreEqual(OllamaContentKind.Text, result.Decision.ContentKind);
        StringAssert.Contains(result.Result.Summary, "text");
        Assert.IsTrue(result.Result.Score >= 0.85);
    }

    [TestMethod]
    public async Task AnalyzeAsync_ReturnsExecutionResultForCSharpContent()
    {
        ContentAnalysisRouter router = CreateRouter();
        string inputText = "using System;\nnamespace Demo;\n\npublic sealed class Sample\n{\n    public void Run()\n    {\n        Console.WriteLine(\"hello\");\n    }\n}";

        ContentAnalysisExecutionResult result = await router.AnalyzeAsync(inputText, "Sample.cs", ContentAnalysisMode.Auto);

        Assert.AreEqual(OllamaContentKind.SourceCode, result.Decision.ContentKind);
        Assert.AreEqual("csharp", result.Decision.Language);
        StringAssert.Contains(result.Result.Summary, "C# source");
    }

    private static ContentAnalysisRouter CreateRouter() => new(new TextAnalysisTool(), new CSharpCodeAnalysisTool());
}
