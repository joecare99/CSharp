namespace Ollama.CodingAgent;

/// <summary>
/// Builds standard system prompts for the coding-agent runtime.
/// </summary>
public static class AgentPromptBuilder
{
    /// <summary>
    /// Builds the default system prompt.
    /// </summary>
    /// <returns>The default system prompt text.</returns>
    public static string BuildDefaultSystemPrompt()
    {
        return "You are a C# coding agent. Answer accurately and concisely. " +
               "When the final answer is ready, prefix the final message with [[FINAL]] and then provide the result. " +
               "Avoid filler and keep implementation-oriented detail.";
    }
}
