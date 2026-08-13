using System;
using System.Collections.Generic;

namespace Ollama.CodingAgent;

/// <summary>
/// Represents one planned subtask.
/// </summary>
public sealed class PlannedSubtask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlannedSubtask"/> class.
    /// </summary>
    /// <param name="id">The subtask id.</param>
    /// <param name="title">The subtask title.</param>
    /// <param name="rationale">The subtask rationale.</param>
    public PlannedSubtask(
        string id,
        string title,
        string rationale,
        IReadOnlyList<string>? dependencies = null)
    {
        Id = string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentException("Id must not be empty.", nameof(id))
            : id;
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title must not be empty.", nameof(title))
            : title;
        Rationale = string.IsNullOrWhiteSpace(rationale)
            ? throw new ArgumentException("Rationale must not be empty.", nameof(rationale))
            : rationale;
        Dependencies = dependencies ?? Array.Empty<string>();
        Status = PlannedSubtaskStatus.Pending;
    }

    /// <summary>
    /// Gets the subtask id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the subtask title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the subtask rationale.
    /// </summary>
    public string Rationale { get; }

    /// <summary>
    /// Gets the IDs of subtasks that must complete first.
    /// </summary>
    public IReadOnlyList<string> Dependencies { get; }

    /// <summary>
    /// Gets or sets the current subtask status.
    /// </summary>
    public PlannedSubtaskStatus Status { get; set; }
}
