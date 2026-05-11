using System;
using System.Collections.Generic;
using System.Linq;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Stores and resolves registered Ollama tools.
/// </summary>
public sealed class OllamaToolRegistry : IOllamaToolRegistry
{
    private readonly Dictionary<string, IOllamaTool> _tools;
    private readonly IReadOnlyList<OllamaToolDescriptor> _descriptors;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolRegistry"/> class.
    /// </summary>
    /// <param name="tools">The tools to register.</param>
    public OllamaToolRegistry(IEnumerable<IOllamaTool> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);

        _tools = tools.ToDictionary(tool => tool.Name, StringComparer.OrdinalIgnoreCase);
        _descriptors = _tools.Values
            .Select(static tool => new OllamaToolDescriptor
            {
                Name = tool.Name,
                Description = tool.Description,
                Schema = tool.Schema,
            })
            .ToArray();
    }

    /// <inheritdoc/>
    public IReadOnlyList<OllamaToolDescriptor> GetDescriptors() => _descriptors;

    /// <inheritdoc/>
    public bool TryGetTool(string name, out IOllamaTool? tool)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _tools.TryGetValue(name, out tool);
    }
}
