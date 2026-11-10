using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Evaluated project metadata; the target path may not exist until the project is built.
/// </summary>
public sealed class ProjectTargetInspection
{
    public required ProjectWorkflowContext Context { get; init; }

    public required IReadOnlyList<string> Configurations { get; init; }

    public required IReadOnlyList<string> TargetFrameworks { get; init; }

    public string? TargetFramework { get; init; }

    public string? TargetPath { get; init; }

    public string? OutputType { get; init; }

    public string? FrameworkIdentifier { get; init; }

    public string? IsTestProject { get; init; }

    public string? DebugUnavailableReason { get; init; }

    public bool CanDebug => DebugUnavailableReason is null;
}

/// <summary>
/// Evaluates project metadata only after the caller explicitly requests inspection.
/// </summary>
public interface IProjectTargetResolver
{
    Task<ProjectTargetInspection> InspectAsync(
        ProjectWorkflowContext context,
        CancellationToken cancellationToken = default);
}
