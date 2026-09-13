using System;
using System.Collections.Generic;
using System.Linq;

namespace Property.Editor;

/// <summary>
/// Orders property entries consistently across UI and non-UI consumers.
/// </summary>
public static class PropertyItemOrdering
{
    /// <summary>
    /// Returns a deterministic category and property ordering.
    /// </summary>
    /// <param name="properties">The property entries to order.</param>
    /// <returns>The ordered property entries.</returns>
    public static IReadOnlyList<IPropertyItem> Order(IEnumerable<IPropertyItem> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        return properties
            .Select(property => property ?? throw new ArgumentException("Property entries cannot be null.", nameof(properties)))
            .OrderBy(static property => property.Category.SortOrder)
            .ThenBy(static property => property.Category.Name, StringComparer.Ordinal)
            .ThenBy(static property => property.SortOrder)
            .ThenBy(static property => property.Name, StringComparer.Ordinal)
            .ToArray();
    }
}
