using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Media.Tests;

[TestClass]
public sealed class PopplerPdfImageRendererTests
{
    [TestMethod]
    public async Task RenderAsync_RendersAndOrdersPngPages()
    {
        var pdfPath = CreatePdfFixture();
        try
        {
            var runner = new FixtureProcessRunner();
            var renderer = new PopplerPdfImageRenderer(
                new PopplerPdfImageRendererOptions("pdftoppm", 300, 1024, TimeSpan.FromSeconds(5)),
                runner);

            var pages = await renderer.RenderAsync(new PdfImageRenderRequest(pdfPath, 2));

            Assert.AreEqual(2, pages.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, pages.Select(page => page.PageNumber).ToArray());
            CollectionAssert.AreEqual(new byte[] { 1 }, pages[0].ImageBytes);
            Assert.AreEqual("-png", runner.Request.Arguments[0]);
            CollectionAssert.Contains(runner.Request.Arguments.ToArray(), "-l");
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    [TestMethod]
    public async Task RenderAsync_RejectsOversizedImages()
    {
        var pdfPath = CreatePdfFixture();
        try
        {
            await AssertThrowsAsync<InvalidDataException>(() =>
                new PopplerPdfImageRenderer(
                    new PopplerPdfImageRendererOptions("pdftoppm", MaxImageBytes: 1),
                    new FixtureProcessRunner([1, 2]))
                    .RenderAsync(new PdfImageRenderRequest(pdfPath, 0)));
        }
        finally
        {
            File.Delete(pdfPath);
        }
    }

    private static string CreatePdfFixture()
    {
        var path = Path.Combine(Path.GetTempPath(), "rnz-render-" + Guid.NewGuid() + ".pdf");
        File.WriteAllBytes(path, [0]);
        return path;
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
        private readonly byte[] _imageBytes;

        public FixtureProcessRunner(byte[]? imageBytes = null) =>
            _imageBytes = imageBytes ?? [1];

        public ExternalProcessRequest Request { get; private set; } = null!;

        public Task<ExternalProcessResult> RunAsync(
            ExternalProcessRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;
            File.WriteAllBytes(Path.Combine(request.WorkingDirectory!, "page-2.png"), _imageBytes);
            File.WriteAllBytes(Path.Combine(request.WorkingDirectory!, "page-1.png"), _imageBytes);
            return Task.FromResult(new ExternalProcessResult(0, string.Empty, string.Empty));
        }
    }
}
