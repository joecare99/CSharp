using System;
using System.Text.Json;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public sealed class ComponentMcpCommandDescriptor
{
    public ComponentMcpCommandDescriptor(
        string commandId,
        string displayName,
        string description,
        string inputSchemaJson,
        bool isReadOnly = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandId);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputSchemaJson);

        using JsonDocument schema = JsonDocument.Parse(inputSchemaJson);
        if (schema.RootElement.ValueKind != JsonValueKind.Object
            || !schema.RootElement.TryGetProperty("type", out JsonElement type)
            || type.GetString() != "object")
        {
            throw new ArgumentException("The MCP command input schema must describe a JSON object.", nameof(inputSchemaJson));
        }

        CommandId = commandId;
        DisplayName = displayName;
        Description = description;
        InputSchema = schema.RootElement.Clone();
        IsReadOnly = isReadOnly;
    }

    public string CommandId { get; }

    public string DisplayName { get; }

    public string Description { get; }

    public JsonElement InputSchema { get; }

    public bool IsReadOnly { get; }
}
