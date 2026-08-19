using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.ContentAnalysis;
using TextAnalysisProgram = Ollama.Samples.TextAnalysis.Program;

namespace PeripheralProduction.Tests;

[TestClass]
[DoNotParallelize]
public sealed class TextAnalysisSampleTests
{
    [TestMethod]
    public void ResolveMode_RecognizesEachSelectionAndDefaultsToAuto()
    {
        Assert.AreEqual(ContentAnalysisMode.Text, TextAnalysisProgram.ResolveMode(["--text"]));
        Assert.AreEqual(ContentAnalysisMode.CSharp, TextAnalysisProgram.ResolveMode(["--csharp"]));
        Assert.AreEqual(ContentAnalysisMode.Auto, TextAnalysisProgram.ResolveMode(["--other"]));

        CollectionAssert.AreEqual(new[] { "first", "second" }, TextAnalysisProgram.FilterInputArguments(["--text", "first", "--csharp", "second"]));
    }

    [TestMethod]
    public async Task Main_AnalyzesInlineTextCSharpAndExistingFileInputs()
    {
        (int textResult, string textOutput) = await ConsoleOutput.CaptureAsync(() => TextAnalysisProgram.Main(["--text", "A concise test sentence. It has sufficient detail."]));
        Assert.AreEqual(0, textResult);
        StringAssert.Contains(textOutput, "Mode: Text analysis");
        StringAssert.Contains(textOutput, "Input: A concise test sentence. It has sufficient detail.");

        (int csharpResult, string csharpOutput) = await ConsoleOutput.CaptureAsync(() => TextAnalysisProgram.Main(["--csharp"]));
        Assert.AreEqual(0, csharpResult);
        StringAssert.Contains(csharpOutput, "Mode: C# source analysis");
        StringAssert.Contains(csharpOutput, "inline csharp input");

        string inputPath = Path.Combine(Environment.CurrentDirectory, "text-analysis-input.txt");
        await File.WriteAllTextAsync(inputPath, "Text loaded from a deterministic test file.");
        try
        {
            (int fileResult, string fileOutput) = await ConsoleOutput.CaptureAsync(() => TextAnalysisProgram.Main(["--text", inputPath]));
            Assert.AreEqual(0, fileResult);
            StringAssert.Contains(fileOutput, inputPath);
        }
        finally
        {
            File.Delete(inputPath);
        }
    }

    [TestMethod]
    public async Task Main_UsesDefaultAutoInputAndTreatsUnknownPathsAsInlineText()
    {
        (int defaultResult, string defaultOutput) = await ConsoleOutput.CaptureAsync(() => TextAnalysisProgram.Main([]));
        Assert.AreEqual(0, defaultResult);
        StringAssert.Contains(defaultOutput, "inline input");

        (int pathResult, string pathOutput) = await ConsoleOutput.CaptureAsync(() => TextAnalysisProgram.Main(["not-an-existing-file", "with", "words"]));
        Assert.AreEqual(0, pathResult);
        StringAssert.Contains(pathOutput, "not-an-existing-file");
    }
}
