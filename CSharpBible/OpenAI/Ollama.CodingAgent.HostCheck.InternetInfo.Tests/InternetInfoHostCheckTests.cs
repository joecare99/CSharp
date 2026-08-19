using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.HostCheck.InternetInfo.Tests;

[TestClass]
public sealed class InternetInfoHostCheckTests
{
    [TestMethod]
    public async Task FetchMethods_ValidateSourcesResponsesAndMalformedPayloads()
    {
        using HttpClient httpClient = new(new ResponseHandler());
        Assert.AreEqual("Wikipedia extract", await InvokeAsync<string>("FetchAsync", httpClient, "wikipedia", "C#"));
        Assert.AreEqual("Wikipedia extract", await InvokeAsync<string>("FetchAsync", httpClient, "WIKIPEDIA", "C#"));
        Assert.AreEqual("plain content", await InvokeAsync<string>("FetchAsync", httpClient, "rosettacode", "C#"));
        Assert.AreEqual("plain content", await InvokeAsync<string>("FetchAsync", httpClient, "mslearn", "C#"));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => InvokeAsync<string>("FetchAsync", httpClient, string.Empty, "C#"));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => InvokeAsync<string>("FetchAsync", httpClient, "wikipedia", string.Empty));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => InvokeAsync<string>("FetchAsync", httpClient, "unknown", "C#"));

        using HttpClient missingExtractClient = new(new ResponseHandler("""{}"""));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => InvokeAsync<string>("FetchWikipediaSummaryAsync", missingExtractClient, "C#"));

        using HttpClient nullExtractClient = new(new ResponseHandler("""{"extract":null}"""));
        Assert.AreEqual(string.Empty, await InvokeAsync<string>("FetchWikipediaSummaryAsync", nullExtractClient, "C#"));
    }

    [TestMethod]
    public async Task RunAsync_UsesInjectedClientForNormalDefaultAndMalformedChecks()
    {
        using HttpClient httpClient = new(new ResponseHandler());

        Assert.AreEqual(0, await InvokeAsync<int>("RunAsync", new[] { "wikipedia", "C#" }, httpClient));
        Assert.AreEqual(0, await InvokeAsync<int>("RunAsync", Array.Empty<string>(), httpClient));
        Assert.AreEqual(0, await InvokeAsync<int>("RunAsync", new[] { "wikipedia" }, httpClient));
        Assert.AreEqual(0, await InvokeAsync<int>("RunAsync", new[] { "unknown", "C#" }, httpClient));

        string longExtract = new string('x', 601);
        using HttpClient longContentClient = new(new ResponseHandler($$"""{"extract":"{{longExtract}}"}"""));
        Assert.AreEqual(0, await InvokeAsync<int>("RunAsync", new[] { "wikipedia", "C#" }, longContentClient));
    }

    [TestMethod]
    public async Task Main_UsesInjectableHttpClientFactoryWithoutNetworkAccess()
    {
        Type programType = GetProgramType();
        FieldInfo factory = programType.GetField("HttpClientFactory", BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate original = (Delegate)factory.GetValue(null)!;
        using (HttpClient defaultClient = ((Func<HttpClient>)original)())
        {
        }
        try
        {
            factory.SetValue(null, (Func<HttpClient>)(() => new HttpClient(new ResponseHandler())));
            MethodInfo main = programType.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
            Assert.AreEqual(0, await (Task<int>)main.Invoke(null, [Array.Empty<string>()])!);
        }
        finally
        {
            factory.SetValue(null, original);
        }
    }

    private static Task<T> InvokeAsync<T>(string methodName, params object[] arguments)
    {
        MethodInfo method = GetProgramType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Task<T>)method.Invoke(null, arguments)!;
    }

    private static Type GetProgramType()
        => Assembly.Load("Ollama.CodingAgent.HostCheck.InternetInfo")
            .GetType("Ollama.CodingAgent.HostCheck.InternetInfo.Program", throwOnError: true)!;

    private sealed class ResponseHandler : HttpMessageHandler
    {
        private readonly string _wikipediaResponse;
        private readonly string _genericResponse;

        public ResponseHandler(
            string wikipediaResponse = """{"extract":"Wikipedia extract"}""",
            string genericResponse = """{"valid":true}""")
        {
            _wikipediaResponse = wikipediaResponse;
            _genericResponse = genericResponse;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string content = request.RequestUri!.Host == "en.wikipedia.org"
                ? _wikipediaResponse
                : request.RequestUri.Host == "example.com"
                    ? _genericResponse
                    : "plain content";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });
        }
    }
}
