using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ollama.CodingAgent;

/// <summary>
/// Persists and resumes plan state through a local JSON checkpoint.
/// </summary>
public sealed class PlanStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanStateStore"/> class.
    /// </summary>
    public PlanStateStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    /// <summary>
    /// Saves a plan checkpoint.
    /// </summary>
    public async Task SaveAsync(PlanState planState, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(planState);
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        PlanStateCheckpoint checkpoint = new()
        {
            Objective = planState.GoalContract.Objective,
            SuccessCriteria = planState.GoalContract.SuccessCriteria.ToArray(),
            Subtasks = planState.Subtasks.Select(static subtask => new PlannedSubtaskCheckpoint
            {
                Id = subtask.Id,
                Title = subtask.Title,
                Rationale = subtask.Rationale,
                Dependencies = subtask.Dependencies.ToArray(),
                Status = subtask.Status,
            }).ToArray(),
        };
        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, checkpoint, JsonOptions, cancellationToken);
    }

    /// <summary>
    /// Loads a plan checkpoint.
    /// </summary>
    public async Task<PlanState> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("Plan checkpoint was not found.", _filePath);
        }

        await using FileStream stream = File.OpenRead(_filePath);
        PlanStateCheckpoint checkpoint = await JsonSerializer.DeserializeAsync<PlanStateCheckpoint>(
            stream,
            JsonOptions,
            cancellationToken)
            ?? throw new InvalidDataException("Plan checkpoint is empty or invalid.");
        if (string.IsNullOrWhiteSpace(checkpoint.Objective) || checkpoint.Subtasks.Count == 0)
        {
            throw new InvalidDataException("Plan checkpoint does not contain a valid plan.");
        }

        List<PlannedSubtask> subtasks = checkpoint.Subtasks
            .Select(item =>
            {
                PlannedSubtask subtask = new(item.Id, item.Title, item.Rationale, item.Dependencies);
                subtask.Status = item.Status;
                return subtask;
            })
            .ToList();
        return new PlanState(new GoalContract(checkpoint.Objective, checkpoint.SuccessCriteria), subtasks);
    }

    private sealed class PlanStateCheckpoint
    {
        public string Objective { get; init; } = string.Empty;
        public IReadOnlyList<string> SuccessCriteria { get; init; } = [];
        public IReadOnlyList<PlannedSubtaskCheckpoint> Subtasks { get; init; } = [];
    }

    private sealed class PlannedSubtaskCheckpoint
    {
        public string Id { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Rationale { get; init; } = string.Empty;
        public IReadOnlyList<string> Dependencies { get; init; } = [];
        public PlannedSubtaskStatus Status { get; init; }
    }
}
