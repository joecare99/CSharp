using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

[McpServerToolType]
public sealed class ComponentMcpTools(
    ComponentMcpCommandCatalog commandCatalog,
    IEnumerable<IComponentScreenshotSource> screenshotSources)
{
    private readonly IComponentScreenshotSource[] _screenshotSources = screenshotSources
        .OrderBy(static source => source.ComponentId, StringComparer.Ordinal)
        .ToArray();

    [McpServerTool(Name = "component_commands", ReadOnly = true), Description("Lists the explicitly exported commands of this component host, including each command's JSON input schema.")]
    public string ListCommands()
    {
        return JsonSerializer.Serialize(commandCatalog.Descriptors.Select(static descriptor => new
        {
            descriptor.CommandId,
            descriptor.DisplayName,
            descriptor.Description,
            descriptor.InputSchema,
            descriptor.IsReadOnly,
        }));
    }

    [McpServerTool(Name = "component_command_execute", Destructive = true), Description("Executes one explicitly exported component command. First call component_commands to inspect its required arguments.")]
    public Task<string> ExecuteCommand(
        string commandId,
        string argumentsJson = "{}",
        CancellationToken cancellationToken = default)
    {
        using JsonDocument arguments = JsonDocument.Parse(argumentsJson);
        return commandCatalog.ExecuteAsync(commandId, arguments.RootElement.Clone(), cancellationToken);
    }

    [McpServerTool(Name = "component_capture_screenshot", ReadOnly = true), Description("Captures the current visual design of a component and returns it as a PNG image.")]
    public async Task<CallToolResult> CaptureScreenshot(
        string componentId,
        CancellationToken cancellationToken = default)
    {
        IComponentScreenshotSource source = _screenshotSources.FirstOrDefault(
            item => string.Equals(item.ComponentId, componentId, StringComparison.Ordinal))
            ?? throw new KeyNotFoundException($"No screenshot source is registered for component '{componentId}'.");
        byte[] image = await source.CapturePngAsync(cancellationToken).ConfigureAwait(false);
        if (image.Length == 0)
        {
            throw new InvalidOperationException($"Screenshot capture for component '{componentId}' returned no image data.");
        }

        return new CallToolResult
        {
            Content = [ImageContentBlock.FromBytes(image, "image/png")],
        };
    }
}
