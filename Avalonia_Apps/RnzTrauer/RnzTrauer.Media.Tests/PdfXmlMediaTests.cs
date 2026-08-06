using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RnzTrauer.Media.Tests;

[TestClass]
public sealed class PdfXmlMediaTests
{
    [TestMethod]
    public void Parse_ExtractsLegacyTextAndImageStructure()
    {
        var xml = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "Fixtures", "sample.xml"));
        var result = new PdfXmlDocumentParser().Parse(xml);

        Assert.AreEqual("Max Mustermann" + Environment.NewLine + "Lahr", result.Text);
        Assert.AreEqual(1, result.Images.Count);
        Assert.AreEqual("profile.png", result.Images[0].Source);
        Assert.AreEqual(12.5, result.Images[0].X);
    }

    [TestMethod]
    public async Task ExtractAsync_RunsToolAndParsesCreatedXml()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures");
        var pdfPath = Path.Combine(fixtureDirectory, "sample.pdf");
        var xmlPath = Path.Combine(Path.GetTempPath(), "rnz-media-" + Guid.NewGuid() + ".xml");
        var service = new PdfXmlExtractionService(
            new FixtureProcessRunner(File.ReadAllText(Path.Combine(fixtureDirectory, "sample.xml"))),
            new PdfXmlDocumentParser());

        try
        {
            var result = await service.ExtractAsync(new PdfXmlExtractionRequest(
                pdfPath,
                "fake-pdftoxml",
                xmlPath,
                TimeSpan.FromSeconds(5)));

            Assert.AreEqual("Max Mustermann" + Environment.NewLine + "Lahr", result.Text);
            Assert.AreEqual(1, result.ImageCandidates.Count);
            Assert.AreEqual("profile.png", result.ImageCandidates.Single().Source);
        }
        finally
        {
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
        }
    }

    private sealed class FixtureProcessRunner : IExternalProcessRunner
    {
        private readonly string _xml;

        public FixtureProcessRunner(string xml)
        {
            _xml = xml;
        }

        public Task<ExternalProcessResult> RunAsync(
            ExternalProcessRequest request,
            System.Threading.CancellationToken cancellationToken = default)
        {
            File.WriteAllText(request.Arguments[1], _xml);
            return Task.FromResult(new ExternalProcessResult(0, string.Empty, "fixture"));
        }
    }
}
