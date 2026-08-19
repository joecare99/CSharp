using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent;

/// <summary>
/// Normalizes model responses for final answer extraction.
/// </summary>
public static class AgentResponseNormalizer
{
    private const string FinalMarker = "[[FINAL]]";

    /// <summary>
    /// Normalizes the model response and extracts marker information.
    /// </summary>
    /// <param name="response">The raw model response.</param>
    /// <param name="finalizedWithMarker">True when the response contained the final marker.</param>
    /// <returns>The normalized response text.</returns>
    public static string Normalize(string response, out bool finalizedWithMarker)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            finalizedWithMarker = false;
            return string.Empty;
        }

        int markerIndex = response.IndexOf(FinalMarker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            finalizedWithMarker = false;
            return response.Trim();
        }

        finalizedWithMarker = true;
        string normalized = response.Remove(markerIndex, FinalMarker.Length);
        return normalized.Trim();
    }
}
