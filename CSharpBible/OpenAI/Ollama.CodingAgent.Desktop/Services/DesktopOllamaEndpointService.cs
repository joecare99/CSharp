using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Client.Interfaces;
using Ollama.Client.Models;
using Ollama.Client.Services;
using Ollama.Protocol.Models;
using Ollama.CodingAgent.Desktop.Models;

namespace Ollama.CodingAgent.Desktop.Services;

/// <summary>
/// Tests an Ollama endpoint and retrieves its available models.
/// </summary>
public sealed class DesktopOllamaEndpointService
{
    /// <summary>
    /// Retrieves model names from the supplied endpoint.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetModelsAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        DesktopConfiguration configuration = new DesktopConfiguration
        {
            Endpoint = endpoint,
            Model = "endpoint-test",
            WorkspacePath = Environment.CurrentDirectory,
        }.Normalize();

        using HttpClient httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(15),
        };
        OllamaClient client = new(httpClient, new OllamaClientOptions(new Uri(configuration.Endpoint)));
        OllamaTagsResponse response = await client.GetTagsAsync(cancellationToken);
        return response.Models
            .Select(static model => model.Name)
            .Where(static name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
