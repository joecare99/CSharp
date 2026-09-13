using System;

namespace Property.Editor;

/// <summary>
/// Describes a named group of properties without imposing presentation or
/// resource-localization requirements on a consumer.
/// </summary>
public sealed class PropertyCategory
{
    /// <summary>
    /// Initializes a property category.
    /// </summary>
    /// <param name="name">The stable technical category name.</param>
    /// <param name="displayName">The category text supplied by the consumer.</param>
    /// <param name="sortOrder">The primary category ordering value.</param>
    public PropertyCategory(string name, string displayName, int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A category name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A category display name is required.", nameof(displayName));
        }

        Name = name;
        DisplayName = displayName;
        SortOrder = sortOrder;
    }

    /// <summary>Gets the stable technical category name.</summary>
    public string Name { get; }

    /// <summary>Gets the display text supplied by the consumer.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the primary deterministic category ordering value.</summary>
    public int SortOrder { get; }
}
