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
    public event EventHandler<PropertyEditorEditEventArgs>? EditStarted;

    public event EventHandler<PropertyEditorEditEventArgs>? EditCompleted;

    /// <summary>
    /// Gets the items rendered by a Property.Editor Avalonia control.
    /// </summary>
    public ObservableCollection<PropertyEditorItemViewModel> Items { get; } = [];

    public void NotifyEditStarted(IPropertyItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EditStarted?.Invoke(this, new PropertyEditorEditEventArgs(item));
    }

    public void NotifyEditCompleted(IPropertyItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EditCompleted?.Invoke(this, new PropertyEditorEditEventArgs(item));
    }

    /// <summary>
    /// Replaces the editor items using the neutral deterministic ordering.
    /// </summary>
    /// <param name="items">The neutral items to display.</param>
    public void SetItems(IEnumerable<IPropertyItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        IPropertyItem[] orderedItems = PropertyItemOrdering.Order(items).ToArray();
        if (Items.Count == orderedItems.Length
            && Items.Select(static item => item.PropertyItem)
                .Zip(orderedItems)
                .All(static pair => ReferenceEquals(pair.First, pair.Second)))
        {
            return;
        }

        Items.Clear();
        foreach (IPropertyItem item in orderedItems)
        {
            Items.Add(new PropertyEditorItemViewModel(item));
        }
    }
}

public sealed class PropertyEditorEditEventArgs(IPropertyItem propertyItem) : EventArgs
{
    public IPropertyItem PropertyItem { get; } = propertyItem;
}
