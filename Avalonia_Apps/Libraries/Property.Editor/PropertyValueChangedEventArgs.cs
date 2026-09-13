using System;

namespace Property.Editor;

/// <summary>
/// Provides the previous and current values of a successful property update.
/// </summary>
public sealed class PropertyValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes value-change event data.
    /// </summary>
    /// <param name="oldValue">The value before the update.</param>
    /// <param name="newValue">The value after the update.</param>
    public PropertyValueChangedEventArgs(object? oldValue, object? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>Gets the value before the update.</summary>
    public object? OldValue { get; }

    /// <summary>Gets the value after the update.</summary>
    public object? NewValue { get; }
}
