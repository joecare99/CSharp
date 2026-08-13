using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Models;

namespace Ollama.CodingAgent;

/// <summary>
/// Provides the Ollama operations required by the baseline checks.
/// </summary>
public interface IOllamaBaselineClient
{
    /// <summary>
    /// Gets the names of models available at the endpoint.
    /// </summary>
    Task<IReadOnlyList<string>> GetAvailableModelsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes one baseline chat request.
    /// </summary>
    Task<OllamaChatCompletion> CompleteChatAsync(string prompt, CancellationToken cancellationToken = default);
}
