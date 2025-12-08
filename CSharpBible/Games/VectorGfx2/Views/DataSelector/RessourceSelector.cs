using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VectorGfx2.Models.Interfaces;

namespace VectorGfx2.Views.DataSelector;

/// <summary>
/// Class KeyString.
/// </summary>
public class KeyString
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key { get; set; } = "";
}

/// <summary>
/// Class RessourceSelector.
/// Implements the <see cref="DataTemplateSelector" />
/// </summary>
/// <seealso cref="DataTemplateSelector" />
public class RessourceSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the item keys.
    /// </summary>
    /// <value>The item keys.</value>
    public List<KeyString> ItemKeys { get; set; } = new List<KeyString>();
    /// <summary>
    /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
    /// </summary>
    /// <param name="item">The data object for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>Returns a <see cref="T:System.Windows.DataTemplate" /> or <see langword="null" />. The default value is <see langword="null" />.</returns>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (container is FrameworkElement element && item is ITypedObject op && op.IType < ItemKeys.Count)
        {
            return element.FindResource(ItemKeys[op.IType].Key) as DataTemplate;
        }
        else return null;
    }
}
