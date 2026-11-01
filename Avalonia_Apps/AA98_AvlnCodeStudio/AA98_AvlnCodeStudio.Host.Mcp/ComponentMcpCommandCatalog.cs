using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Host.Mcp;

public sealed class ComponentMcpCommandCatalog
{
    private readonly Dictionary<string, IComponentMcpCommand> _commands;

    public ComponentMcpCommandCatalog(IEnumerable<IComponentMcpCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        IComponentMcpCommand[] items = commands.ToArray();
        if (items.Any(static command => command is null))
        {
            throw new ArgumentException("MCP command exports cannot contain null entries.", nameof(commands));
        }

        _commands = new Dictionary<string, IComponentMcpCommand>(StringComparer.Ordinal);
        foreach (IComponentMcpCommand command in items)
        {
            if (!_commands.TryAdd(command.Descriptor.CommandId, command))
            {
                throw new ArgumentException(
                    $"The MCP command identifier '{command.Descriptor.CommandId}' is registered more than once.",
                    nameof(commands));
            }
        }
    }

    public IReadOnlyList<ComponentMcpCommandDescriptor> Descriptors => _commands.Values
        .Select(static command => command.Descriptor)
        .OrderBy(static descriptor => descriptor.CommandId, StringComparer.Ordinal)
        .ToArray();

    public Task<string> ExecuteAsync(string commandId, JsonElement arguments, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandId);
        if (arguments.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("MCP command arguments must be a JSON object.", nameof(arguments));
        }

        if (!_commands.TryGetValue(commandId, out IComponentMcpCommand? command))
        {
            throw new KeyNotFoundException($"The component does not export the MCP command '{commandId}'.");
        }

        ValidateArguments(command.Descriptor.InputSchema, arguments);
        return command.ExecuteAsync(arguments, cancellationToken);
    }

    private static void ValidateArguments(JsonElement schema, JsonElement arguments)
    {
        JsonElement properties = schema.GetProperty("properties");
        foreach (JsonProperty argument in arguments.EnumerateObject())
        {
            if (!properties.TryGetProperty(argument.Name, out JsonElement propertySchema))
            {
                if (schema.TryGetProperty("additionalProperties", out JsonElement additionalProperties)
                    && additionalProperties.ValueKind == JsonValueKind.False)
                {
                    throw new ArgumentException($"The MCP command does not accept the '{argument.Name}' argument.");
                }

                continue;
            }

            if (propertySchema.TryGetProperty("type", out JsonElement type)
                && !MatchesType(type.GetString(), argument.Value))
            {
                throw new ArgumentException($"The MCP command argument '{argument.Name}' must be a {type.GetString()}.");
            }
        }

        if (schema.TryGetProperty("required", out JsonElement required))
        {
            foreach (JsonElement name in required.EnumerateArray())
            {
                string propertyName = name.GetString()
                    ?? throw new InvalidOperationException("An MCP input schema contains an invalid required property.");
                if (!arguments.TryGetProperty(propertyName, out _))
                {
                    throw new ArgumentException($"The MCP command requires the '{propertyName}' argument.");
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
