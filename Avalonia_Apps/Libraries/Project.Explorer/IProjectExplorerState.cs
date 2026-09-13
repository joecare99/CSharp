using System.Collections.Generic;

namespace Project.Explorer;

/// <summary>Tracks selection and expansion independently from a source or UI framework.</summary>
public interface IProjectExplorerState
{
    /// <summary>Gets the selected item identity, or <see langword="null"/> when nothing is selected.</summary>
    string? SelectedItemId { get; }

    /// <summary>Gets expanded item identities.</summary>
    IReadOnlySet<string> ExpandedItemIds { get; }

    /// <summary>Selects an item, or clears selection with <see langword="null"/>.</summary>
    void Select(string? itemId);

    /// <summary>Updates the expansion state for one item.</summary>
    void SetExpanded(string itemId, bool isExpanded);
}
