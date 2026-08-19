using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Desktop.Models;

/// <summary>
/// Represents one validated Ollama desktop configuration.
/// </summary>
public sealed class DesktopConfiguration
{
    /// <summary>
    /// Gets the Ollama endpoint.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets the selected Ollama model.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Gets the workspace used for the next prompt.
    /// </summary>
    public required string WorkspacePath { get; init; }

    /// <summary>
    /// Creates a validated configuration copy.
    /// </summary>
    public DesktopConfiguration Normalize()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(Model);
        ArgumentException.ThrowIfNullOrWhiteSpace(WorkspacePath);
        if (!Uri.TryCreate(Endpoint, UriKind.Absolute, out Uri? endpoint)
            || (endpoint.Scheme != Uri.UriSchemeHttp && endpoint.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("The endpoint must be an absolute HTTP or HTTPS URL.", nameof(Endpoint));
        }

        return new DesktopConfiguration
        {
            Endpoint = endpoint.AbsoluteUri,
            Model = Model.Trim(),
            WorkspacePath = System.IO.Path.GetFullPath(WorkspacePath),
        };
    }
}
