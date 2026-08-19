using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class WebLookupToolTests
{
    [TestMethod]
    public async Task ExecuteAsync_ReturnsBoundedCitationEnvelope()
    {
        RecordingHandler handler = new();
        using HttpClient httpClient = new(handler);
        WebLookupTool tool = new(httpClient, new WebKnowledgePolicy());

        OllamaToolResult result = await tool.ExecuteAsync(
            """{"source":"wikipedia","query":"C sharp"}""");

        Assert.IsTrue(result.Success);
        using JsonDocument document = JsonDocument.Parse(result.Output);
        JsonElement root = document.RootElement;
        Assert.AreEqual("wikipedia", root.GetProperty("Citation").GetProperty("Source").GetString());
        StringAssert.Contains(root.GetProperty("Citation").GetProperty("Url").GetString(), "wikipedia.org");
        Assert.AreEqual(200, root.GetProperty("StatusCode").GetInt32());
        Assert.AreEqual("knowledge response", root.GetProperty("ContentPreview").GetString());
        Assert.AreEqual("https://en.wikipedia.org/api/rest_v1/page/summary/C_sharp", handler.RequestUri?.ToString());
    }

    [TestMethod]
    public void Validate_RejectsUnknownSource()
    {
        WebLookupTool tool = new(new HttpClient(), new WebKnowledgePolicy());

        OllamaToolValidationResult validation = tool.Validate(
            """{"source":"example","query":"anything"}""");

        Assert.IsFalse(validation.IsValid);
        StringAssert.Contains(string.Join(" ", validation.Errors), "not allowed");
    }

    [TestMethod]
    public void Policy_RejectsNonAllowlistedCitationHost()
    {
        WebKnowledgePolicy policy = new();

        Assert.IsFalse(policy.IsAllowedCitationUri(new Uri("https://example.com/reference")));
        Assert.IsFalse(policy.IsAllowedCitationUri(new Uri("http://en.wikipedia.org/reference")));
        Assert.IsTrue(policy.IsAllowedCitationUri(new Uri("https://learn.microsoft.com/en-us/dotnet")));
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("knowledge response"),
            });
        }
    }
}
