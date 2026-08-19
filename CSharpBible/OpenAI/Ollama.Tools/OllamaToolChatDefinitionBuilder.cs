using System;
using System.Collections.Generic;
using System.Linq;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Builds provider-neutral Ollama chat tool definitions from the registered tool descriptors.
/// </summary>
public static class OllamaToolChatDefinitionBuilder
{
    /// <summary>
    /// Builds the native tool definitions accepted by Ollama chat completion requests.
    /// </summary>
    /// <param name="toolRegistry">The registered tool registry.</param>
    /// <returns>The native chat tool definitions.</returns>
    public static IReadOnlyList<OllamaChatTool> Build(IOllamaToolRegistry toolRegistry)
    {
        ArgumentNullException.ThrowIfNull(toolRegistry);

        return toolRegistry.GetDescriptors().Select(static descriptor => new OllamaChatTool
        {
            Name = descriptor.Name,
            Description = descriptor.Description,
            Parameters = descriptor.Schema.Parameters.ToDictionary(
                static parameter => parameter.Name,
                static parameter => new OllamaChatToolParameter
                {
                    Type = parameter.Type,
                    Description = parameter.Description,
                    Required = parameter.Required,
                }),
        }).ToArray();
    }
}
