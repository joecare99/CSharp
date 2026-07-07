using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Base.UI.Properties;

/// <summary>
/// Exposes a generic set of properties for a selected object.
/// </summary>
public interface IHasProperties
{
    /// <summary>
    /// Gets the available properties.
    /// </summary>
    IReadOnlyList<IPropertyItem> Properties { get; }
}
