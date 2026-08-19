using System.Collections.Generic;
using System.Linq;

namespace Ollama.CodingAgent.Git;

/// <summary>
/// Is the exact structured preview presented for one requested mutation.
/// </summary>
public sealed record GitOperationPreview(
    string Operation,
    string WorkspacePath,
    IReadOnlyDictionary<string, string> Parameters)
{
    /// <summary>
    /// Renders a deterministic text representation for the application approval request.
    /// </summary>
    public string Render() => string.Join(
        System.Environment.NewLine,
        new[] { $"Operation: {Operation}", $"Workspace: {WorkspacePath}" }
            .Concat(Parameters.OrderBy(parameter => parameter.Key, System.StringComparer.Ordinal)
                .Select(parameter => $"{parameter.Key}: {parameter.Value}")));
}
