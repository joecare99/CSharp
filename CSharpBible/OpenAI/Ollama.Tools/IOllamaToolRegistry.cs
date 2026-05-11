using System.Collections.Generic;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Defines lookup behavior for registered Ollama tools.
/// </summary>
public interface IOllamaToolRegistry
{
    /// <summary>
    /// Gets all registered tool descriptors.
    /// </summary>
    /// <returns>The registered tool descriptors.</returns>
    IReadOnlyList<OllamaToolDescriptor> GetDescriptors();

    /// <summary>
    /// Tries to resolve a tool by name.
    /// </summary>
    /// <param name="name">The tool name.</param>
    /// <param name="tool">The resolved tool if found.</param>
    /// <returns><c>true</c> if the tool was found; otherwise <c>false</c>.</returns>
    bool TryGetTool(string name, out IOllamaTool? tool);
}
