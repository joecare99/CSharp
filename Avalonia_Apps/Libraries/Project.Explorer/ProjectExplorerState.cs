using System;
using System.Collections.Generic;

namespace Project.Explorer;

/// <summary>Default in-memory state implementation for a project explorer consumer.</summary>
public sealed class ProjectExplorerState : IProjectExplorerState
{
    private readonly HashSet<string> _expandedItemIds = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public string? SelectedItemId { get; private set; }

    /// <inheritdoc />
    public IReadOnlySet<string> ExpandedItemIds => _expandedItemIds;

    /// <inheritdoc />
    public void Select(string? itemId)
    {
        SelectedItemId = string.IsNullOrWhiteSpace(itemId) ? null : itemId;
    }

    /// <inheritdoc />
    public void SetExpanded(string itemId, bool isExpanded)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("An explorer item identifier is required.", nameof(itemId));
        }

        if (isExpanded)
        {
            _expandedItemIds.Add(itemId);
            return;
        }

        _expandedItemIds.Remove(itemId);
    }
}
