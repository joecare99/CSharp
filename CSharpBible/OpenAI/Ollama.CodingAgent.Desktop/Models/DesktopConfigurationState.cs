using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Desktop.Models;

/// <summary>
/// Holds the configuration snapshot used by the next desktop prompt.
/// </summary>
public sealed class DesktopConfigurationState
{
    private DesktopConfiguration _current;

    /// <summary>
    /// Initializes the state with the startup configuration.
    /// </summary>
    public DesktopConfigurationState(DesktopConfiguration initial)
    {
        _current = initial?.Normalize() ?? throw new ArgumentNullException(nameof(initial));
    }

    /// <summary>
    /// Gets the current prompt configuration snapshot.
    /// </summary>
    public DesktopConfiguration Current => _current;

    /// <summary>
    /// Replaces the configuration for future prompts.
    /// </summary>
    public void Set(DesktopConfiguration configuration)
        => _current = configuration?.Normalize() ?? throw new ArgumentNullException(nameof(configuration));
}
