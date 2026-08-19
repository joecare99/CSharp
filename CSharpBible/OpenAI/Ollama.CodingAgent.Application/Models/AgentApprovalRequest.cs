using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Application.Models;

/// <summary>
/// Describes one user approval required before a state-changing operation.
/// </summary>
public sealed class AgentApprovalRequest
{
    /// <summary>
    /// Gets or sets the stable request identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets or sets the operation name shown to the user.
    /// </summary>
    public required string Operation { get; init; }

    /// <summary>
    /// Gets or sets the complete reviewable operation preview.
    /// </summary>
    public required string Preview { get; init; }

    /// <summary>
    /// Gets or sets when the request was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
