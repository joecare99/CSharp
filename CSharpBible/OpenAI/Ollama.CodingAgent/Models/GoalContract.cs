using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent.Models;

/// <summary>
/// Represents the primary run objective and success criteria.
/// </summary>
public sealed class GoalContract
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GoalContract"/> class.
    /// </summary>
    /// <param name="objective">The primary objective.</param>
    /// <param name="successCriteria">The success criteria.</param>
    public GoalContract(string objective, IReadOnlyList<string> successCriteria)
    {
        Objective = string.IsNullOrWhiteSpace(objective)
            ? throw new ArgumentException("Objective must not be empty.", nameof(objective))
            : objective;
        SuccessCriteria = successCriteria ?? throw new ArgumentNullException(nameof(successCriteria));
    }

    /// <summary>
    /// Gets the primary objective.
    /// </summary>
    public string Objective { get; }

    /// <summary>
    /// Gets the expected success criteria.
    /// </summary>
    public IReadOnlyList<string> SuccessCriteria { get; }
}
