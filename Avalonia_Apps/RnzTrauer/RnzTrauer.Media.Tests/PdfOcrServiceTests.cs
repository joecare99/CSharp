using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Media.Tests;

[TestClass]
public sealed class PdfOcrServiceTests
{
    [TestMethod]
    public async Task ExtractAsync_RendersPagesInOrderAndAggregatesText()
    {
        var pdfPath = CreatePdfFixture();
        try
        {
            var renderer = new FixtureRenderer(
                new RenderedPdfPage(2, [2], "image/png"),
                new RenderedPdfPage(1, [1], "image/png"));
            var ocr = new FixtureOcrEngine(
                (1, "First page"),
                (2, "Second page"));

            var result = await new PdfOcrService(renderer, ocr)
                .ExtractAsync(new PdfOcrRequest(pdfPath));

            Assert.AreEqual("First page" + Environment.NewLine + "Second page", result.Text);
            CollectionAssert.AreEqual(new[] { "First page", "Second page" }, result.PageTexts.ToArray());
            CollectionAssert.AreEqual(new[] { 0 }, renderer.RequestedMaxPages.ToArray());
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    [TestMethod]
    public async Task ExtractAsync_PreservesPageBoundariesAndOmitsBlankAggregateLines()
    {
        var pdfPath = CreatePdfFixture();
        try
        {
            var ocr = new FixtureOcrEngine((1, "Text"), (2, " "), (3, "More"));
            var result = await new PdfOcrService(
                new FixtureRenderer(
                    new RenderedPdfPage(1, [1], "image/png"),
                    new RenderedPdfPage(2, [2], "image/png"),
                    new RenderedPdfPage(3, [3], "image/png")),
                ocr).ExtractAsync(new PdfOcrRequest(pdfPath));

            CollectionAssert.AreEqual(new[] { "Text", " ", "More" }, result.PageTexts.ToArray());
            Assert.AreEqual("Text" + Environment.NewLine + "More", result.Text);
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    [TestMethod]
    public async Task ExtractAsync_RejectsMissingPdfAndNegativePageLimit()
    {
        await AssertThrowsAsync<FileNotFoundException>(() =>
            new PdfOcrService(new FixtureRenderer(), new FixtureOcrEngine())
                .ExtractAsync(new PdfOcrRequest("missing.pdf")));

        var pdfPath = CreatePdfFixture();
        try
        {
            await AssertThrowsAsync<ArgumentOutOfRangeException>(() =>
                new PdfOcrService(new FixtureRenderer(), new FixtureOcrEngine())
                    .ExtractAsync(new PdfOcrRequest(pdfPath, -1)));
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    [TestMethod]
    public async Task ExtractAsync_RejectsInvalidOrDuplicateRenderedPages()
    {
        var pdfPath = CreatePdfFixture();
        try
        {
            await AssertThrowsAsync<InvalidDataException>(() =>
                new PdfOcrService(
                    new FixtureRenderer(
                        new RenderedPdfPage(1, [1], "image/png"),
                        new RenderedPdfPage(1, [2], "image/png")),
                    new FixtureOcrEngine())
                    .ExtractAsync(new PdfOcrRequest(pdfPath)));

            await AssertThrowsAsync<InvalidDataException>(() =>
                new PdfOcrService(
                    new FixtureRenderer(new RenderedPdfPage(0, [], "image/png")),
                    new FixtureOcrEngine())
                    .ExtractAsync(new PdfOcrRequest(pdfPath)));
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    [TestMethod]
    public async Task ExtractAsync_HonorsCancellationBeforeRendering()
    {
        var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var pdfPath = CreatePdfFixture();
        try
        {
            var renderer = new FixtureRenderer();
            await AssertThrowsAsync<OperationCanceledException>(() =>
                new PdfOcrService(renderer, new FixtureOcrEngine())
                    .ExtractAsync(new PdfOcrRequest(pdfPath), cancellation.Token));
            Assert.AreEqual(0, renderer.RenderCallCount);
        }
        finally
        {
            File.Delete(pdfPath);
        }
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

    private static string CreatePdfFixture()
    {
        var path = Path.Combine(Path.GetTempPath(), "rnz-ocr-" + Guid.NewGuid() + ".pdf");
        File.WriteAllBytes(path, [0]);
        return path;
    }

    private sealed class FixtureRenderer : IPdfImageRenderer
    {
        private readonly IReadOnlyList<RenderedPdfPage> _pages;

        public FixtureRenderer(params RenderedPdfPage[] pages) => _pages = pages;

        public List<int> RequestedMaxPages { get; } = [];
        public int RenderCallCount { get; private set; }

        public Task<IReadOnlyList<RenderedPdfPage>> RenderAsync(
            PdfImageRenderRequest request,
            CancellationToken cancellationToken = default)
        {
            RenderCallCount++;
            RequestedMaxPages.Add(request.MaxPages);
            return Task.FromResult(_pages);
        }
    }

    private sealed class FixtureOcrEngine : IOcrEngine
    {
        private readonly IReadOnlyDictionary<int, string> _texts;

        public FixtureOcrEngine(params (int Page, string Text)[] texts) =>
            _texts = texts.ToDictionary(item => item.Page, item => item.Text);

        public Task<string> RecognizeAsync(
            RenderedPdfPage page,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_texts.TryGetValue(page.PageNumber, out var text) ? text : string.Empty);
    }
}
