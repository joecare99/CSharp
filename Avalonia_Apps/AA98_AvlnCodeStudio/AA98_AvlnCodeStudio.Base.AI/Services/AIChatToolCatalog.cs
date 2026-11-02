using AA98_AvlnCodeStudio.Base.AI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Lists and dispatches the tools explicitly registered for AI chat.
/// </summary>
public sealed class AIChatToolCatalog
{
    private readonly Dictionary<string, IAIChatTool> _tools;

    public AIChatToolCatalog(IEnumerable<IAIChatTool> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);
        IAIChatTool[] items = tools.ToArray();
        if (items.Any(static tool => tool is null))
        {
            throw new ArgumentException("AI chat tools cannot contain null entries.", nameof(tools));
        }

        _tools = new Dictionary<string, IAIChatTool>(StringComparer.Ordinal);
        foreach (IAIChatTool tool in items)
        {
            ArgumentNullException.ThrowIfNull(tool.Descriptor);
            if (!_tools.TryAdd(tool.Descriptor.Name, tool))
            {
                throw new ArgumentException(
                    $"The AI chat tool '{tool.Descriptor.Name}' is registered more than once.",
                    nameof(tools));
            }
        }
    }

    public IReadOnlyList<AIChatToolDescriptor> Descriptors => _tools.Values
        .Select(static tool => tool.Descriptor)
        .OrderBy(static descriptor => descriptor.Name, StringComparer.Ordinal)
        .ToArray();

    public bool TryGetDescriptor(string name, out AIChatToolDescriptor? descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (_tools.TryGetValue(name, out IAIChatTool? tool))
        {
            descriptor = tool.Descriptor;
            return true;
        }

        descriptor = null;
        return false;
    }

    public Task<string> ExecuteAsync(string name, JsonElement arguments, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (arguments.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("AI chat tool arguments must be a JSON object.", nameof(arguments));
        }

        if (!_tools.TryGetValue(name, out IAIChatTool? tool))
        {
            throw new KeyNotFoundException($"The AI chat tool '{name}' is not registered.");
        }

        ValidateArguments(tool.Descriptor.InputSchema, arguments);
        return tool.ExecuteAsync(arguments, cancellationToken);
    }

    private static void ValidateArguments(JsonElement schema, JsonElement arguments)
    {
        bool hasProperties = schema.TryGetProperty("properties", out JsonElement properties)
            && properties.ValueKind == JsonValueKind.Object;
        foreach (JsonProperty argument in arguments.EnumerateObject())
        {
            if (!hasProperties || !properties.TryGetProperty(argument.Name, out JsonElement propertySchema))
            {
                if (schema.TryGetProperty("additionalProperties", out JsonElement additionalProperties)
                    && additionalProperties.ValueKind == JsonValueKind.False)
                {
                    throw new ArgumentException($"The AI chat tool does not accept the '{argument.Name}' argument.");
                }

                continue;
            }

            if (propertySchema.TryGetProperty("type", out JsonElement type)
                && !MatchesType(type.GetString(), argument.Value))
            {
                throw new ArgumentException($"The AI chat tool argument '{argument.Name}' must be a {type.GetString()}.");
            }
        }

        if (schema.TryGetProperty("required", out JsonElement required))
        {
            foreach (JsonElement name in required.EnumerateArray())
            {
                string propertyName = name.GetString()
                    ?? throw new InvalidOperationException("An AI tool schema contains an invalid required property.");
                if (!arguments.TryGetProperty(propertyName, out _))
                {
                    throw new ArgumentException($"The AI chat tool requires the '{propertyName}' argument.");
                }
            }
        }
    }

    private static bool MatchesType(string? type, JsonElement value)
    {
        return type switch
        {
            "string" => value.ValueKind == JsonValueKind.String,
            "boolean" => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
            "integer" => value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out _),
            "number" => value.ValueKind == JsonValueKind.Number,
            "object" => value.ValueKind == JsonValueKind.Object,
            "array" => value.ValueKind == JsonValueKind.Array,
            _ => true,
        };
    }
}
