using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Describes a workbench-specific popup target for context-sensitive command routing.
/// </summary>
public sealed class WorkbenchPopupTarget
{
    private readonly string[] _contextKinds;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkbenchPopupTarget"/> class.
    /// </summary>
    /// <param name="targetId">The stable popup target identifier.</param>
    /// <param name="contextKinds">The supported workbench context kinds.</param>
    public WorkbenchPopupTarget(string targetId, IEnumerable<string>? contextKinds = null)
    {
        if (string.IsNullOrWhiteSpace(targetId))
        {
            throw new ArgumentException("A popup target identifier is required.", nameof(targetId));
        }

        var normalizedContextKinds = contextKinds?
            .Where(static contextKind => !string.IsNullOrWhiteSpace(contextKind))
            .Select(static contextKind => contextKind.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        TargetId = targetId.Trim();
        _contextKinds = normalizedContextKinds;
    }

    /// <summary>
    /// Gets the stable popup target identifier.
    /// </summary>
    public string TargetId { get; }

    /// <summary>
    /// Gets the supported workbench context kinds.
    /// </summary>
    public IReadOnlyList<string> ContextKinds => _contextKinds;
}
