using System;
using System.Text.Json;

namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Describes a structured tool that an AI client may call.
/// </summary>
public sealed class AIChatToolDescriptor
{
    public AIChatToolDescriptor(
        string name,
        string description,
        string inputSchemaJson,
        bool requiresConfirmation = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputSchemaJson);

        using JsonDocument schema = JsonDocument.Parse(inputSchemaJson);
        if (schema.RootElement.ValueKind != JsonValueKind.Object
            || !schema.RootElement.TryGetProperty("type", out JsonElement type)
            || type.GetString() != "object")
        {
            throw new ArgumentException("The AI tool input schema must describe a JSON object.", nameof(inputSchemaJson));
        }

        Name = name;
        Description = description;
        InputSchema = schema.RootElement.Clone();
        RequiresConfirmation = requiresConfirmation;
    }

    public string Name { get; }

    public string Description { get; }

    public JsonElement InputSchema { get; }

    public bool RequiresConfirmation { get; }
}
