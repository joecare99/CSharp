using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Base.AI.Services;
using AA98_AvlnCodeStudio.Base.Components.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelContextProtocol.Protocol;
using System;
using System.Linq;
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
    public async Task WorkbenchMcpRegistration_ExportsOnlyExplicitlyOptedInContributions()
    {
        ServiceCollection services = new();
        services.AddWorkbenchMcpCommand<ExplicitWorkbenchMcpCommand>();
        services.AddSingleton<IWorkbenchCommandContribution, LocalOnlyWorkbenchCommand>();

        using ServiceProvider provider = services.BuildServiceProvider();
        ComponentMcpCommandCatalog catalog = new(provider.GetServices<IComponentMcpCommand>());

        Assert.AreEqual(1, catalog.Descriptors.Count);
        Assert.AreEqual("sample.workbench.echo", catalog.Descriptors[0].CommandId);
        Assert.AreEqual(2, provider.GetServices<IWorkbenchCommandContribution>().Count());
        Assert.AreEqual(1, provider.GetServices<IAIChatTool>().Count());
        Assert.AreSame(
            provider.GetRequiredService<ExplicitWorkbenchMcpCommand>(),
            provider.GetServices<IWorkbenchCommandContribution>().OfType<ExplicitWorkbenchMcpCommand>().Single());

        using JsonDocument arguments = JsonDocument.Parse("""{"message":"from MCP"}""");
        Assert.AreEqual(
            "from MCP",
            await catalog.ExecuteAsync("sample.workbench.echo", arguments.RootElement, CancellationToken.None));

        AIChatToolCatalog chatTools = new(provider.GetServices<IAIChatTool>());
        Assert.AreEqual("sample.workbench.echo", chatTools.Descriptors[0].Name);
        Assert.AreEqual(
            "from MCP",
            await chatTools.ExecuteAsync("sample.workbench.echo", arguments.RootElement, CancellationToken.None));
    }

    [TestMethod]
    public async Task ComponentMcpRegistration_AlsoMakesExplicitToolAvailableToAIChat()
    {
        ServiceCollection services = new();
        services.AddComponentMcpCommand<ExplicitMcpCommand>();

        using ServiceProvider provider = services.BuildServiceProvider();
        Assert.AreEqual(1, provider.GetServices<IComponentMcpCommand>().Count());
        AIChatToolCatalog chatTools = new(provider.GetServices<IAIChatTool>());
        Assert.AreEqual(1, chatTools.Descriptors.Count);
        Assert.IsTrue(chatTools.Descriptors[0].RequiresConfirmation is false);

        using JsonDocument arguments = JsonDocument.Parse("""{"message":"from chat"}""");
        Assert.AreEqual(
            "from chat",
            await chatTools.ExecuteAsync("sample.mcp.echo", arguments.RootElement, CancellationToken.None));
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

    private sealed class ExplicitWorkbenchMcpCommand : IWorkbenchMcpCommandContribution
    {
        public WorkbenchCommandDescriptor Descriptor { get; } = new("sample.workbench.echo", "Echo");

        public ComponentMcpCommandDescriptor McpDescriptor { get; } = new(
            "sample.workbench.echo",
            "Echo",
            "Returns the supplied message.",
            """{"type":"object","properties":{"message":{"type":"string"}},"required":["message"],"additionalProperties":false}""",
            isReadOnly: true);

        public bool CanExecute(IWorkbenchCommandContext context) => true;

        public Task ExecuteAsync(IWorkbenchCommandContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<string> ExecuteMcpAsync(JsonElement arguments, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(arguments.GetProperty("message").GetString()!);
        }
    }

    private sealed class LocalOnlyWorkbenchCommand : IWorkbenchCommandContribution
    {
        public WorkbenchCommandDescriptor Descriptor { get; } = new("sample.workbench.local", "Local only");

        public bool CanExecute(IWorkbenchCommandContext context) => true;

        public Task ExecuteAsync(IWorkbenchCommandContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class ExplicitMcpCommand : IComponentMcpCommand
    {
        public ComponentMcpCommandDescriptor Descriptor { get; } = new(
            "sample.mcp.echo",
            "Echo",
            "Returns the supplied message.",
            """{"type":"object","properties":{"message":{"type":"string"}},"required":["message"],"additionalProperties":false}""",
            isReadOnly: true);

        public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(arguments.GetProperty("message").GetString()!);
        }
    }
}
