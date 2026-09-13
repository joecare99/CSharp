using System;
using System.Collections.Generic;
using System.Linq;

namespace Property.Editor;

/// <summary>
/// Provides the default implementation of a mutable, neutral property entry.
/// </summary>
public sealed class PropertyItem : IPropertyItem
{
    private readonly Func<object?, PropertyValidationResult>? _validator;

    /// <summary>
    /// Initializes a property entry and validates its initial value.
    /// </summary>
    public PropertyItem(
        PropertyCategory category,
        string name,
        string displayName,
        Type valueType,
        object? value,
        bool isNullable = false,
        bool isEditable = true,
        bool isSensitive = false,
        IReadOnlyList<PropertyOption>? options = null,
        Func<object?, PropertyValidationResult>? validator = null,
        int sortOrder = 0)
    {
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(valueType);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A property name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A property display name is required.", nameof(displayName));
        }

        Category = category;
        Name = name;
        DisplayName = displayName;
        ValueType = valueType;
        IsNullable = isNullable;
        IsEditable = isEditable;
        IsSensitive = isSensitive;
        Options = options?.ToArray() ?? Array.Empty<PropertyOption>();
        SortOrder = sortOrder;
        _validator = validator;

        var validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException(validationResult.ErrorMessage, nameof(value));
        }

        Value = value;
        ValidationResult = PropertyValidationResult.Valid;
    }

    /// <inheritdoc/>
    public PropertyCategory Category { get; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string DisplayName { get; }

    /// <inheritdoc/>
    public Type ValueType { get; }

    /// <inheritdoc/>
    public PropertyEditorKind EditorKind =>
        ValueType == typeof(bool) ? PropertyEditorKind.Boolean :
        ValueType.IsEnum ? PropertyEditorKind.Enum :
        PropertyEditorKind.Scalar;

    /// <inheritdoc/>
    public bool IsNullable { get; }

    /// <inheritdoc/>
    public bool IsEditable { get; }

    /// <inheritdoc/>
    public bool IsSensitive { get; }

    /// <inheritdoc/>
    public object? Value { get; private set; }

    /// <inheritdoc/>
    public IReadOnlyList<PropertyOption> Options { get; }

    /// <inheritdoc/>
    public PropertyValidationResult ValidationResult { get; private set; }

    /// <inheritdoc/>
    public int SortOrder { get; }

    /// <inheritdoc/>
    public event EventHandler<PropertyValueChangedEventArgs>? ValueChanged;

    /// <inheritdoc/>
    public bool TrySetValue(object? value)
    {
        if (!IsEditable)
        {
            ValidationResult = PropertyValidationResult.Invalid("This property is read-only.");
            return false;
        }

        var validationResult = Validate(value);
        ValidationResult = validationResult;
        if (!validationResult.IsValid)
        {
            return false;
        }

        if (Equals(Value, value))
        {
            return true;
        }

        var oldValue = Value;
        Value = value;
        ValueChanged?.Invoke(this, new PropertyValueChangedEventArgs(oldValue, value));
        return true;
    }

    private PropertyValidationResult Validate(object? value)
    {
        if (value is null)
        {
            return IsNullable
                ? ValidateCustom(value)
                : PropertyValidationResult.Invalid("A value is required.");
        }

        if (!ValueType.IsInstanceOfType(value))
        {
            return PropertyValidationResult.Invalid("The value does not match the property's value type.");
        }

        if (Options.Count > 0 && !Options.Any(option => Equals(option.Value, value)))
        {
            return PropertyValidationResult.Invalid("The value is not one of the available options.");
        }

        return ValidateCustom(value);
    }

    private PropertyValidationResult ValidateCustom(object? value)
    {
        var validationResult = _validator?.Invoke(value) ?? PropertyValidationResult.Valid;
        if (validationResult is null)
        {
            throw new InvalidOperationException("A property validator must return a validation result.");
        }

        return validationResult;
    }
}
