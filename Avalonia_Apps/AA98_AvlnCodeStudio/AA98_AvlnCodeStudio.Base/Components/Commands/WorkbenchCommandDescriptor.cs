using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Describes a command contribution that can be surfaced by a workbench host.
/// </summary>
public sealed class WorkbenchCommandDescriptor
{
    private readonly WorkbenchCommandPlacement[] _placements;
    private readonly string[] _contextKinds;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkbenchCommandDescriptor"/> class.
    /// </summary>
    /// <param name="commandId">The stable command identifier.</param>
    /// <param name="displayTitle">The host-facing display title.</param>
    /// <param name="placements">The default contribution placements.</param>
    /// <param name="description">The optional localized description.</param>
    /// <param name="defaultGesture">The optional default gesture text.</param>
    /// <param name="contextKinds">The optional workbench context kinds supported by the command.</param>
    public WorkbenchCommandDescriptor(
        string commandId,
        string displayTitle,
        IEnumerable<WorkbenchCommandPlacement>? placements = null,
        string? description = null,
        string? defaultGesture = null,
        IEnumerable<string>? contextKinds = null)
    {
        if (string.IsNullOrWhiteSpace(commandId))
        {
            throw new ArgumentException("A stable command identifier is required.", nameof(commandId));
        }

        if (string.IsNullOrWhiteSpace(displayTitle))
        {
            throw new ArgumentException("A display title is required.", nameof(displayTitle));
        }

        var normalizedPlacements = placements?.ToArray() ?? Array.Empty<WorkbenchCommandPlacement>();
        if (normalizedPlacements.Any(static placement => placement is null))
        {
            throw new ArgumentException("Placements cannot contain null entries.", nameof(placements));
        }

        var normalizedContextKinds = contextKinds?
            .Where(static contextKind => !string.IsNullOrWhiteSpace(contextKind))
            .Select(static contextKind => contextKind.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        CommandId = commandId;
        DisplayTitle = displayTitle;
        Description = description;
        DefaultGesture = defaultGesture;
        _placements = normalizedPlacements;
        _contextKinds = normalizedContextKinds;
    }

    /// <summary>
    /// Gets the stable command identifier.
    /// </summary>
    public string CommandId { get; }

    /// <summary>
    /// Gets the host-facing display title.
    /// </summary>
    public string DisplayTitle { get; }

    /// <summary>
    /// Gets the optional localized description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the optional default gesture text.
    /// </summary>
    public string? DefaultGesture { get; }

    /// <summary>
    /// Gets the default contribution placements.
    /// </summary>
    public IReadOnlyList<WorkbenchCommandPlacement> Placements => _placements;

    /// <summary>
    /// Gets the optional supported workbench context kinds.
    /// </summary>
    public IReadOnlyList<string> ContextKinds => _contextKinds;
}
