using AA98_AvlnCodeStudio.Host.Mcp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelContextProtocol.Protocol;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ComponentMcpToolsTests
{
    [TestMethod]
    public async Task Catalog_ListsAndValidatesOnlyExplicitlyExportedCommands()
    {
        int executions = 0;
        DelegateComponentMcpCommand command = new(
            new ComponentMcpCommandDescriptor(
                "sample.echo",
                "Echo",
                "Returns a message.",
                """{"type":"object","properties":{"message":{"type":"string"}},"required":["message"],"additionalProperties":false}""",
                isReadOnly: true),
            (arguments, _) =>
            {
                executions++;
                return Task.FromResult(arguments.GetProperty("message").GetString()!);
            });
        ComponentMcpCommandCatalog catalog = new([command]);

        Assert.AreEqual(1, catalog.Descriptors.Count);
        using JsonDocument validArguments = JsonDocument.Parse("""{"message":"hello"}""");
        Assert.AreEqual(
            "hello",
            await catalog.ExecuteAsync("sample.echo", validArguments.RootElement, CancellationToken.None));

        using JsonDocument missingArguments = JsonDocument.Parse("{}");
        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            catalog.ExecuteAsync("sample.echo", missingArguments.RootElement, CancellationToken.None));
        await Assert.ThrowsExactlyAsync<System.Collections.Generic.KeyNotFoundException>(() =>
            catalog.ExecuteAsync("sample.unexported", missingArguments.RootElement, CancellationToken.None));
        Assert.AreEqual(1, executions);
    }

    [TestMethod]
    public async Task ScreenshotTool_ReturnsAnImageContentBlock()
    {
        byte[] imageBytes = [137, 80, 78, 71, 13, 10, 26, 10, 1];
        ComponentMcpTools tools = new(
            new ComponentMcpCommandCatalog([]),
            [new FakeScreenshotSource("designer", imageBytes)]);

        CallToolResult result = await tools.CaptureScreenshot("designer");

        Assert.IsInstanceOfType<ImageContentBlock>(result.Content[0]);
        ImageContentBlock image = (ImageContentBlock)result.Content[0];
        Assert.AreEqual("image/png", image.MimeType);
        CollectionAssert.AreEqual(imageBytes, image.DecodedData.ToArray());
    }

    [TestMethod]
    public async Task ScreenshotTool_RejectsUnknownComponent()
    {
        ComponentMcpTools tools = new(new ComponentMcpCommandCatalog([]), []);

        await Assert.ThrowsExactlyAsync<System.Collections.Generic.KeyNotFoundException>(
            () => tools.CaptureScreenshot("missing"));
    }

    private sealed class FakeScreenshotSource(string componentId, byte[] imageBytes) : IComponentScreenshotSource
    {
        public string ComponentId => componentId;

        public Task<byte[]> CapturePngAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(imageBytes);
        }
    }
}
