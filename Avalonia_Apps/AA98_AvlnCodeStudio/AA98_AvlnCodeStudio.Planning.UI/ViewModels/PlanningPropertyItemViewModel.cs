using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Property.Editor;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Represents a property entry in the planning properties panel.
/// </summary>
public sealed class PlanningPropertyItemViewModel : ObservableObject, IPropertyItem
{
    private readonly PropertyItem _propertyItem;
    private readonly Action<object?>? _applyValue;
    private readonly Func<string?, object?>? _parseDisplayValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningPropertyItemViewModel"/> class.
    /// </summary>
    /// <param name="name">The technical property name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="valueType">The type of the represented value.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="isEditable">Indicates whether the value can be edited.</param>
    /// <param name="applyValue">Optional callback for propagating changes.</param>
    /// <param name="options">The permitted values for closed-choice properties.</param>
    /// <param name="parseDisplayValue">Converts the Planning text editor value to the typed contract value.</param>
    public PlanningPropertyItemViewModel(
        PropertyCategory category,
        string name,
        string displayName,
        Type valueType,
        object? value,
        bool isEditable,
        Action<object?>? applyValue = null,
        IReadOnlyList<PropertyOption>? options = null,
        Func<string?, object?>? parseDisplayValue = null)
    {
        _propertyItem = new PropertyItem(
            category,
            name,
            displayName,
            valueType,
            value,
            isNullable: value is null,
            isEditable,
            options: options);
        _applyValue = applyValue;
        _parseDisplayValue = parseDisplayValue;
    }

    /// <inheritdoc/>
    public PropertyCategory Category => _propertyItem.Category;

    /// <inheritdoc/>
    public string Name => _propertyItem.Name;

    /// <inheritdoc/>
    public string DisplayName => _propertyItem.DisplayName;

    /// <inheritdoc/>
    public Type ValueType => _propertyItem.ValueType;

    /// <inheritdoc/>
    public PropertyEditorKind EditorKind => _propertyItem.EditorKind;

    /// <inheritdoc/>
    public bool IsNullable => _propertyItem.IsNullable;

    /// <inheritdoc/>
    public bool IsEditable => _propertyItem.IsEditable;

    /// <inheritdoc/>
    public bool IsSensitive => _propertyItem.IsSensitive;

    /// <summary>Gets whether the Planning editor displays this item as read-only.</summary>
    public bool IsReadOnly => !IsEditable;

    /// <inheritdoc/>
    public object? Value => _propertyItem.Value;

    /// <inheritdoc/>
    public IReadOnlyList<PropertyOption> Options => _propertyItem.Options;

    /// <inheritdoc/>
    public PropertyValidationResult ValidationResult => _propertyItem.ValidationResult;

    /// <inheritdoc/>
    public int SortOrder => _propertyItem.SortOrder;

    /// <inheritdoc/>
    public event EventHandler<PropertyValueChangedEventArgs>? ValueChanged
    {
        add => _propertyItem.ValueChanged += value;
        remove => _propertyItem.ValueChanged -= value;
    }

    /// <summary>
    /// Gets or sets the textual value displayed by the temporary Planning
    /// properties panel.
    /// </summary>
    public string? DisplayValue
    {
        get => Convert.ToString(Value, System.Globalization.CultureInfo.CurrentCulture);
        set
        {
            TrySetValue(_parseDisplayValue?.Invoke(value) ?? value);
            OnPropertyChanged();
        }
    }

    /// <inheritdoc/>
    public bool TrySetValue(object? value)
    {
        if (!_propertyItem.TrySetValue(value))
        {
            OnPropertyChanged(nameof(ValidationResult));
            return false;
        }

        _applyValue?.Invoke(value);
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(DisplayValue));
        OnPropertyChanged(nameof(ValidationResult));
        return true;
    }
}
