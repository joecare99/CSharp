using System;

namespace Property.Editor;

/// <summary>
/// Represents a consumer-supplied selectable value and its display text.
/// </summary>
public sealed class PropertyOption
{
    /// <summary>
    /// Initializes a selectable property value.
    /// </summary>
    /// <param name="value">The non-null value to select.</param>
    /// <param name="displayName">The display text supplied by the consumer.</param>
    public PropertyOption(object value, string displayName)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("An option display name is required.", nameof(displayName));
        }

        Value = value;
        DisplayName = displayName;
    }

    /// <summary>Gets the value applied when this option is selected.</summary>
    public object Value { get; }

    /// <summary>Gets the display text supplied by the consumer.</summary>
    public string DisplayName { get; }
}
