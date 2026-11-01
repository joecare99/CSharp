using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DotNetCSharpProjectPackageServiceTests
{
    [TestMethod]
    public async Task AddPackageAsync_RejectsInvalidProjectPackageAndVersionBeforeLaunchingDotNet()
    {
        DotNetCSharpProjectPackageService service = new("missing-dotnet-executable");

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            service.AddPackageAsync("not-a-project.txt", "Example.Package"));

        string directory = Path.Combine(Path.GetTempPath(), $"AA98_PackageManager_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string projectPath = Path.Combine(directory, "Example.csproj");
            File.WriteAllText(projectPath, "<Project />");

            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                service.AddPackageAsync(projectPath, "-unsafe-package"));
            await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                service.AddPackageAsync(projectPath, "Example.Package", "--unsafe"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task RemovePackageAsync_RejectsMissingProject()
    {
        DotNetCSharpProjectPackageService service = new();

        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() =>
            service.RemovePackageAsync(Path.Combine(Path.GetTempPath(), "missing-project.csproj"), "Example.Package"));
    }

    [TestMethod]
    public async Task SearchPackagesAsync_EscapesQueryAndReturnsStablePackageMetadata()
    {
        RecordingHttpMessageHandler handler = new(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """{"data":[{"id":"Example.Package","version":"2.1.0","description":"Example package"}]}"""),
            });
        using HttpClient client = new(handler);
        DotNetCSharpProjectPackageService service = new(httpClient: client);

        var packages = await service.SearchPackagesAsync("example & helper", 7);

        Assert.AreEqual(1, packages.Count);
        Assert.AreEqual("Example.Package", packages[0].Id);
        Assert.AreEqual("2.1.0", packages[0].LatestVersion);
        StringAssert.Contains(handler.RequestUri!.Query, "q=example%20%26%20helper");
        StringAssert.Contains(handler.RequestUri.Query, "take=7");
        StringAssert.Contains(handler.RequestUri.Query, "prerelease=false");
    }

    [TestMethod]
    public async Task SearchPackagesAsync_RejectsInvalidInputAndSurfacesHttpFailure()
    {
        DotNetCSharpProjectPackageService service = new(httpClient: new HttpClient(
            new RecordingHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable))));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => service.SearchPackagesAsync(" "));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => service.SearchPackagesAsync("example", 101));
        await Assert.ThrowsExactlyAsync<HttpRequestException>(() => service.SearchPackagesAsync("example"));
    }

    [TestMethod]
    public void ParsePackageListJson_ReturnsDirectReferencesForEachTargetFramework()
    {
        const string output = """
            {
              "version": 1,
              "projects": [{
                "frameworks": [
                  {
                    "framework": "net10.0",
                    "topLevelPackages": [
                      {"id": "Zeta", "requestedVersion": "2.0", "resolvedVersion": "2.0.1"},
                      {"id": "Alpha", "requestedVersion": "1.0", "resolvedVersion": "1.0.0"}
                    ]
                  },
                  {"framework": "net8.0"}
                ]
              }]
            }
            """;

        var packages = DotNetCSharpProjectPackageService.ParsePackageListJson(output);

        Assert.AreEqual(2, packages.Count);
        Assert.AreEqual("Alpha", packages[0].Id);
        Assert.AreEqual("net10.0", packages[0].TargetFramework);
        Assert.AreEqual("2.0.1", packages[1].ResolvedVersion);
        Assert.ThrowsExactly<InvalidDataException>(() =>
            DotNetCSharpProjectPackageService.ParsePackageListJson("{}"));
    }

    private sealed class RecordingHttpMessageHandler(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(responseFactory(request));
        }
    }
}
