using System;
using System.Collections.Generic;

namespace Property.Editor;

/// <summary>
/// Exposes a mutable, UI-neutral property entry for a properties editor.
/// </summary>
public interface IPropertyItem
{
    /// <summary>Gets the property's category.</summary>
    PropertyCategory Category { get; }

    /// <summary>Gets the stable technical property name.</summary>
    string Name { get; }

    /// <summary>Gets the display text supplied by the consumer.</summary>
    string DisplayName { get; }

    /// <summary>Gets the runtime type of non-null property values.</summary>
    Type ValueType { get; }

    /// <summary>Gets the neutral editor behavior for the property's type.</summary>
    PropertyEditorKind EditorKind { get; }

    /// <summary>Gets whether the property accepts a null value.</summary>
    bool IsNullable { get; }

    /// <summary>Gets whether the property accepts value changes.</summary>
    bool IsEditable { get; }

    /// <summary>Gets whether UI consumers must mask the property's value.</summary>
    bool IsSensitive { get; }

    /// <summary>Gets the current valid property value.</summary>
    object? Value { get; }

    /// <summary>Gets the available closed-choice values, if supplied.</summary>
    IReadOnlyList<PropertyOption> Options { get; }

    /// <summary>Gets the latest value validation result.</summary>
    PropertyValidationResult ValidationResult { get; }

    /// <summary>Gets the primary deterministic ordering value within its category.</summary>
    int SortOrder { get; }

    /// <summary>Occurs after a valid value change has been applied.</summary>
    event EventHandler<PropertyValueChangedEventArgs>? ValueChanged;

    /// <summary>
    /// Attempts to replace the current value.
    /// </summary>
    /// <param name="value">The proposed value.</param>
    /// <returns><see langword="true"/> only when the new value was applied.</returns>
    bool TrySetValue(object? value);
}
