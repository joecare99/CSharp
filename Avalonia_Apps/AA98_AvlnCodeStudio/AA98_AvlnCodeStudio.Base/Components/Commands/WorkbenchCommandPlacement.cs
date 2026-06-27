using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Describes where a workbench command should appear within a host surface.
/// </summary>
public sealed class WorkbenchCommandPlacement
{
    private readonly string[] _pathSegments;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkbenchCommandPlacement"/> class.
    /// </summary>
    /// <param name="surface">The host surface that should expose the command.</param>
    /// <param name="path">The hierarchical contribution path.</param>
    /// <param name="order">The relative ordering value within the target group.</param>
    public WorkbenchCommandPlacement(WorkbenchCommandSurface surface, string path, int order = 0)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A command placement path is required.", nameof(path));
        }

        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (pathSegments.Length == 0)
        {
            throw new ArgumentException("A command placement path must contain at least one segment.", nameof(path));
        }

        Surface = surface;
        Path = string.Join('/', pathSegments);
        Order = order;
        _pathSegments = pathSegments;
    }

    /// <summary>
    /// Gets the host surface that should expose the command.
    /// </summary>
    public WorkbenchCommandSurface Surface { get; }

    /// <summary>
    /// Gets the normalized hierarchical contribution path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the relative ordering value within the target group.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Gets the normalized path segments.
    /// </summary>
    public IReadOnlyList<string> PathSegments => _pathSegments;
}
