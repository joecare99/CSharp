using System;
using System.Collections.Generic;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools;

/// <summary>
/// Denies tools unless their names are explicitly allowlisted.
/// </summary>
public sealed class OllamaToolAllowlistPolicy : IOllamaToolExecutionPolicy
{
    private readonly HashSet<string> _allowedToolNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaToolAllowlistPolicy"/> class.
    /// </summary>
    public OllamaToolAllowlistPolicy(IEnumerable<string> allowedToolNames)
    {
        ArgumentNullException.ThrowIfNull(allowedToolNames);
        _allowedToolNames = new HashSet<string>(allowedToolNames, StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public OllamaToolPolicyDecision Evaluate(IOllamaTool tool)
    {
        ArgumentNullException.ThrowIfNull(tool);
        return _allowedToolNames.Contains(tool.Name)
            ? new OllamaToolPolicyDecision { IsAllowed = true }
            : new OllamaToolPolicyDecision
            {
                IsAllowed = false,
                Reason = $"Tool '{tool.Name}' is denied by the execution policy.",
            };
    }
}
