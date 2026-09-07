using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Media.Tests;

[TestClass]
public sealed class TesseractOcrEngineTests
{
    [TestMethod]
    public async Task RecognizeAsync_PassesPngToTesseractAndReadsOutput()
    {
        var runner = new FixtureProcessRunner();
        var engine = new TesseractOcrEngine(
            new TesseractOcrOptions("tesseract.exe", "deu", 6, TimeSpan.FromSeconds(3)),
            runner);

        var text = await engine.RecognizeAsync(
            new RenderedPdfPage(1, [137, 80, 78, 71], "image/png"));

        Assert.AreEqual("fixture OCR text", text);
        Assert.AreEqual("tesseract.exe", runner.Request.ExecutablePath);
        CollectionAssert.AreEqual(
            new[] { "-l", "deu", "--psm", "6" },
            runner.Request.Arguments.Skip(2).ToArray());
        CollectionAssert.AreEqual(new byte[] { 137, 80, 78, 71 }, runner.InputImageBytes);
    }

    [TestMethod]
    public async Task RecognizeAsync_RejectsUnsupportedImageType()
    {
        var engine = new TesseractOcrEngine(
            new TesseractOcrOptions("tesseract.exe"),
            new FixtureProcessRunner());

        await AssertThrowsAsync<NotSupportedException>(() =>
            engine.RecognizeAsync(new RenderedPdfPage(1, [1], "image/jpeg")));
    }

    private static async Task AssertThrowsAsync<TException>(Func<Task> action)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException)
        {
            return;
        }

        Assert.Fail($"Expected {typeof(TException).Name}.");
    }

    private sealed class FixtureProcessRunner : IExternalProcessRunner
    {
        public ExternalProcessRequest Request { get; private set; } = null!;
        public byte[] InputImageBytes { get; private set; } = [];

        public Task<ExternalProcessResult> RunAsync(
            ExternalProcessRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;
            InputImageBytes = File.ReadAllBytes(request.Arguments[0]);
            File.WriteAllText(request.Arguments[1] + ".txt", "fixture OCR text");
            return Task.FromResult(new ExternalProcessResult(0, string.Empty, string.Empty));
        }
    }
}
