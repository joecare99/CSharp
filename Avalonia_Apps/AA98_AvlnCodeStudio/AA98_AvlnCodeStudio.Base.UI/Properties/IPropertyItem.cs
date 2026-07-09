namespace AA98_AvlnCodeStudio.Base.UI.Properties;

/// <summary>
/// Represents a single property in a generic properties panel.
/// </summary>
public interface IPropertyItem
{
    /// <summary>
    /// Gets the technical name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets or sets the current string representation of the value.
    /// </summary>
    string? Value { get; set; }

    /// <summary>
    /// Gets a value indicating whether the value is editable.
    /// </summary>
    bool IsEditable { get; }

    /// <summary>
    /// Gets a value indicating whether the value is read-only.
    /// </summary>
    bool IsReadOnly { get; }
}
