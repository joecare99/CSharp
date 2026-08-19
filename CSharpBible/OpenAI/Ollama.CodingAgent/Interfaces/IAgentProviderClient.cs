using Ollama.CodingAgent.Models;
namespace Ollama.CodingAgent.Interfaces;

/// <summary>
/// Defines an agent model client together with provider capability metadata.
/// </summary>
public interface IAgentProviderClient : IAgentModelClient
{
    /// <summary>
    /// Gets the capabilities of the configured provider and model.
    /// </summary>
    AgentProviderCapabilities Capabilities { get; }
}
