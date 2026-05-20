using System;
using System.Linq;
using System.Text;

namespace Ollama.Tools;

/// <summary>
/// Builds a simple prompt section that lists available tools.
/// </summary>
public static class OllamaToolPromptBuilder
{
    /// <summary>
    /// Creates a prompt fragment for the registered tools.
    /// </summary>
    /// <param name="toolRegistry">The tool registry.</param>
    /// <returns>A prompt fragment describing the available tools.</returns>
    public static string BuildToolInstructions(IOllamaToolRegistry toolRegistry)
    {
        ArgumentNullException.ThrowIfNull(toolRegistry);

        StringBuilder builder = new();
        builder.AppendLine("Available tools:");

        foreach (OllamaToolDescriptor descriptor in toolRegistry.GetDescriptors().OrderBy(static descriptor => descriptor.Name, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"- {descriptor.Name}: {descriptor.Description}");

            if (!string.IsNullOrWhiteSpace(descriptor.Schema.Summary))
            {
                builder.AppendLine($"  Schema: {descriptor.Schema.Summary}");
            }

            foreach (OllamaToolParameter parameter in descriptor.Schema.Parameters)
            {
                string requiredText = parameter.Required ? "required" : "optional";
                builder.AppendLine($"  - {parameter.Name} ({parameter.Type}, {requiredText}): {parameter.Description}");
            }
        }

        builder.AppendLine("Respond with a JSON object containing 'toolName' and 'input' when a tool should be invoked.");
        return builder.ToString();
    }
}
