using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Acquisition.Tests;

[TestClass]
public sealed class HtmlAcquisitionServiceTests
{
    [TestMethod]
    public async Task AcquireAsync_ReadsLocalFileAndArchivesAtomically()
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample.html");
        var archivePath = Path.Combine(Path.GetTempPath(), "rnz-acquisition-" + Guid.NewGuid() + ".html");
        var service = new HtmlAcquisitionService(new HttpClient());

        try
        {
            var result = await service.AcquireAsync(new HtmlAcquisitionRequest(
                new Uri(sourcePath),
                archivePath));

            Assert.AreEqual(File.ReadAllText(sourcePath), Encoding.UTF8.GetString(result.Content));
            Assert.AreEqual(Path.GetFullPath(archivePath), result.ArchivedPath);
            Assert.IsTrue(File.Exists(archivePath));
            CollectionAssert.AreEqual(result.Content, await File.ReadAllBytesAsync(archivePath));
        }
        finally
        {
            if (File.Exists(archivePath))
                File.Delete(archivePath);
        }
    }

    [TestMethod]
    public async Task AcquireAsync_ReadsHttpContentAndPreservesMediaType()
    {
        using var httpClient = new HttpClient(new StubHandler(
            HttpStatusCode.OK,
            "text/html",
            "<html>remote</html>"));
        var service = new HtmlAcquisitionService(httpClient);

        var result = await service.AcquireAsync(new HtmlAcquisitionRequest(
            new Uri("https://example.test/page.html")));

        Assert.AreEqual("<html>remote</html>", Encoding.UTF8.GetString(result.Content));
        Assert.AreEqual("text/html", result.MediaType);
    }

    [TestMethod]
    public async Task AcquireAsync_RejectsContentAboveConfiguredLimit()
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample.html");
        var service = new HtmlAcquisitionService(new HttpClient());

        await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
            service.AcquireAsync(new HtmlAcquisitionRequest(
                new Uri(sourcePath),
                MaxBytes: 4)));
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _mediaType;
        private readonly string _content;

        public StubHandler(HttpStatusCode statusCode, string mediaType, string content)
        {
            _statusCode = statusCode;
            _mediaType = mediaType;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                RequestMessage = request,
                Content = new StringContent(_content, Encoding.UTF8, _mediaType),
            };
            return Task.FromResult(response);
        }
    }
}
