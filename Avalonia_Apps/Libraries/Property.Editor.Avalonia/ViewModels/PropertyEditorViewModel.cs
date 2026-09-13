using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Property.Editor.Avalonia;

/// <summary>
/// Provides ordered, reusable editor state for neutral property items.
/// </summary>
public sealed class PropertyEditorViewModel : ObservableObject
{
    /// <summary>
    /// Gets the items rendered by a Property.Editor Avalonia control.
    /// </summary>
    public ObservableCollection<PropertyEditorItemViewModel> Items { get; } = [];

    /// <summary>
    /// Replaces the editor items using the neutral deterministic ordering.
    /// </summary>
    /// <param name="items">The neutral items to display.</param>
    public void SetItems(IEnumerable<IPropertyItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items.Clear();
        foreach (IPropertyItem item in PropertyItemOrdering.Order(items))
        {
            Items.Add(new PropertyEditorItemViewModel(item));
        }
    }
}
