using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Property.Editor.Avalonia;

/// <summary>
/// Adapts one neutral property item for scalar, Boolean, and enum templates.
/// </summary>
public sealed partial class PropertyEditorItemViewModel : ObservableObject
{
    private readonly IPropertyItem _propertyItem;
    private bool _isSynchronizing;

    /// <summary>
    /// Initializes a view model for one neutral property item.
    /// </summary>
    /// <param name="propertyItem">The neutral item to expose.</param>
    public PropertyEditorItemViewModel(IPropertyItem propertyItem)
    {
        _propertyItem = propertyItem ?? throw new ArgumentNullException(nameof(propertyItem));
        _propertyItem.ValueChanged += OnPropertyValueChanged;
        SynchronizeFromItem();
    }

    /// <summary>Gets the neutral property item.</summary>
    public IPropertyItem PropertyItem => _propertyItem;

    /// <summary>Gets the display name supplied by the consumer.</summary>
    public string DisplayName => _propertyItem.DisplayName;

    /// <summary>Gets whether this item accepts changes.</summary>
    public bool IsEditable => _propertyItem.IsEditable;

    /// <summary>Gets whether the scalar editor masks its value.</summary>
    public bool IsSensitive => _propertyItem.IsSensitive;

    /// <summary>Gets the masking character for sensitive scalar values.</summary>
    public char PasswordChar => IsSensitive ? '*' : '\0';

    /// <summary>Gets whether a scalar text editor is applicable.</summary>
    public bool IsScalar => _propertyItem.EditorKind == PropertyEditorKind.Scalar;

    /// <summary>Gets whether a Boolean editor is applicable.</summary>
    public bool IsBoolean => _propertyItem.EditorKind == PropertyEditorKind.Boolean;

    /// <summary>Gets whether an enum selection editor is applicable.</summary>
    public bool IsEnum => _propertyItem.EditorKind == PropertyEditorKind.Enum;

    /// <summary>Gets whether the scalar editor control is displayed.</summary>
    public bool ShowScalarEditor => IsScalar && IsEditable;

    /// <summary>Gets whether the Boolean editor control is displayed.</summary>
    public bool ShowBooleanEditor => IsBoolean && IsEditable;

    /// <summary>Gets whether the enum editor control is displayed.</summary>
    public bool ShowEnumEditor => IsEnum && IsEditable;

    /// <summary>Gets whether the read-only value text is displayed.</summary>
    public bool ShowReadOnlyValue => !IsEditable;

    /// <summary>Gets the options for an enum editor.</summary>
    public System.Collections.Generic.IReadOnlyList<PropertyOption> Options => _propertyItem.Options;

    /// <summary>Gets whether a validation error is currently shown.</summary>
    public bool HasValidationError => !string.IsNullOrWhiteSpace(ValidationErrorText);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasValidationError))]
    private string _validationErrorText = string.Empty;

    [ObservableProperty]
    private string? _scalarValue;

    [ObservableProperty]
    private bool? _booleanValue;

    [ObservableProperty]
    private PropertyOption? _selectedOption;

    partial void OnScalarValueChanged(string? value)
    {
        if (!_isSynchronizing)
        {
            ApplyValue(ParseScalar(value));
        }
    }

    partial void OnBooleanValueChanged(bool? value)
    {
        if (!_isSynchronizing)
        {
            ApplyValue(value);
        }
    }

    partial void OnSelectedOptionChanged(PropertyOption? value)
    {
        if (!_isSynchronizing && value is not null)
        {
            ApplyValue(value.Value);
        }
    }

    private void OnPropertyValueChanged(object? sender, PropertyValueChangedEventArgs eventArgs)
    {
        SynchronizeFromItem();
    }

    private void ApplyValue(object? value)
    {
        if (_propertyItem.TrySetValue(value))
        {
            ValidationErrorText = string.Empty;
            SynchronizeFromItem();
            return;
        }

        ValidationErrorText = _propertyItem.ValidationResult.ErrorMessage ?? "The value is invalid.";
    }

    private object? ParseScalar(string? value)
    {
        if (value is null && _propertyItem.IsNullable)
        {
            return null;
        }

        if (_propertyItem.ValueType == typeof(string))
        {
            return value;
        }

        if (string.IsNullOrWhiteSpace(value) && _propertyItem.IsNullable)
        {
            return null;
        }

        TypeConverter converter = TypeDescriptor.GetConverter(_propertyItem.ValueType);
        if (!converter.CanConvertFrom(typeof(string)))
        {
            return value;
        }

        try
        {
            return converter.ConvertFrom(null, CultureInfo.CurrentCulture, value ?? string.Empty);
        }
        catch (FormatException)
        {
            return value;
        }
        catch (NotSupportedException)
        {
            return value;
        }
        catch (ArgumentException)
        {
            return value;
        }
    }

    private void SynchronizeFromItem()
    {
        _isSynchronizing = true;
        try
        {
            object? value = _propertyItem.Value;
            ScalarValue = Convert.ToString(value, CultureInfo.CurrentCulture);
            BooleanValue = value as bool? ?? (value is bool booleanValue ? booleanValue : null);
            SelectedOption = _propertyItem.Options.FirstOrDefault(option => Equals(option.Value, value));
            ValidationErrorText = _propertyItem.ValidationResult.IsValid
                ? string.Empty
                : _propertyItem.ValidationResult.ErrorMessage ?? "The value is invalid.";
        }
        finally
        {
            _isSynchronizing = false;
        }
    }
}
