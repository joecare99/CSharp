namespace AA98_AvlnCodeStudio.Base.UI.Properties;

/// <summary>
/// Provides a default mutable property item implementation.
/// </summary>
public sealed class PropertyItem : IPropertyItem
{
    /// <inheritdoc/>
    public string Name { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string DisplayName { get; init; } = string.Empty;

    /// <inheritdoc/>
    public string? Value { get; set; }

    /// <inheritdoc/>
    public bool IsEditable { get; init; }

    /// <inheritdoc/>
    public bool IsReadOnly => !IsEditable;
}
