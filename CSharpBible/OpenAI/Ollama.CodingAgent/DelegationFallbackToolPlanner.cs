using System;
using Ollama.Tools;

namespace Ollama.CodingAgent;

/// <summary>
/// Selects a deterministic fallback tool call when model-based tool selection is unavailable.
/// </summary>
public static class DelegationFallbackToolPlanner
{
    /// <summary>
    /// Creates a fallback tool call from a user prompt.
    /// </summary>
    /// <param name="userPrompt">The user prompt.</param>
    /// <returns>The fallback tool call.</returns>
    public static OllamaToolCall CreateFallbackToolCall(string userPrompt)
    {
        string prompt = userPrompt ?? string.Empty;
        if (ContainsAny(prompt, "test", "unittest", "integration test"))
        {
            return new OllamaToolCall
            {
                ToolName = "run_dotnet_test",
                Input = "{\"relativePath\":\"Ollama.CodingAgent.Tests\\\\Ollama.CodingAgent.Tests.csproj\",\"configuration\":\"Debug\"}",
            };
        }

        if (ContainsAny(prompt, "build", "compile"))
        {
            return new OllamaToolCall
            {
                ToolName = "run_dotnet_build",
                Input = "{\"relativePath\":\"OpenAI.slnx\",\"configuration\":\"Debug\"}",
            };
        }

        return new OllamaToolCall
        {
            ToolName = "list_workspace_files",
            Input = "{\"relativePath\":\".\",\"maxFiles\":80}",
        };
    }

    private static bool ContainsAny(string input, params string[] terms)
    {
        foreach (string term in terms)
        {
            if (input.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }
}
