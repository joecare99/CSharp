using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Property.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlPropertyRowViewModel : ObservableObject, IPropertyItem
{
    private static readonly PropertyCategory s_category = new("AXAML", "AXAML");
    private string _value;
    private PropertyValidationResult _validationResult = PropertyValidationResult.Valid;

    public AxamlPropertyRowViewModel(string controlType, AxamlPropertyDescriptor descriptor)
    {
        ControlType = controlType;
        Name = descriptor.Name;
        EditorKind = descriptor.EditorKind;
        Options = descriptor.EditorKind == AxamlPropertyEditorKind.Boolean
            ? ["True", "False"]
            : descriptor.Options;
        PropertyOptions = EditorKind == AxamlPropertyEditorKind.Boolean
            ? [new PropertyOption(true, "True"), new PropertyOption(false, "False")]
            : Options.Select(static option => new PropertyOption(option, option)).ToArray();
        IsReadOnly = descriptor.IsReadOnly;
        ReadOnlyReason = descriptor.ReadOnlyReason;
        _value = descriptor.Value;
        ValidationError = descriptor.ValidationError;
        _validationResult = string.IsNullOrWhiteSpace(descriptor.ValidationError)
            ? PropertyValidationResult.Valid
            : PropertyValidationResult.Invalid(descriptor.ValidationError);
    }

    public string ControlType { get; }

    public string Name { get; }

    public string DisplayName => Name;

    public Type ValueType => EditorKind == AxamlPropertyEditorKind.Boolean
        ? typeof(bool)
        : typeof(string);

    PropertyEditorKind IPropertyItem.EditorKind => EditorKind switch
    {
        AxamlPropertyEditorKind.Boolean => PropertyEditorKind.Boolean,
        AxamlPropertyEditorKind.Enum => PropertyEditorKind.Enum,
        _ => PropertyEditorKind.Scalar,
    };

    public bool IsNullable => false;

    public bool IsEditable => !IsReadOnly;

    public bool IsSensitive => false;

    public PropertyCategory Category => s_category;

    public int SortOrder => 0;

    public IReadOnlyList<PropertyOption> PropertyOptions { get; }

    public PropertyValidationResult ValidationResult => _validationResult;

    public PropertyValueContext ValueContext => new(
        PropertyScope.Session,
        origin: ControlType,
        readOnlyReason: ReadOnlyReason);

    IReadOnlyList<PropertyOption> IPropertyItem.Options => PropertyOptions;

    object? IPropertyItem.Value => GetPropertyValue();

    public AxamlPropertyEditorKind EditorKind { get; }

    public IReadOnlyList<string> Options { get; }

    public bool IsReadOnly { get; }

    public string? ReadOnlyReason { get; }

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isDraft;

    [ObservableProperty]
    private string? _validationError;

    public string Value
    {
        get => _value;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            string oldValue = _value;
            if (SetProperty(ref _value, value))
            {
                ValidationError = IsReadOnly
                    ? ReadOnlyReason
                    : AxamlPropertyValueValidator.Validate(ControlType, Name, value);
                _validationResult = string.IsNullOrWhiteSpace(ValidationError)
                    ? PropertyValidationResult.Valid
                    : PropertyValidationResult.Invalid(ValidationError);
                OnPropertyChanged(nameof(ValidationResult));
                ValueChanged?.Invoke(
                    this,
                    new PropertyValueChangedEventArgs(ToPropertyValue(oldValue), GetPropertyValue()));
            }
        }
    }

    public event EventHandler<PropertyValueChangedEventArgs>? ValueChanged;

    public bool TrySetValue(object? value)
    {
        if (!IsEditable)
        {
            SetValidationError(ReadOnlyReason ?? "This property is read-only.");
            return false;
        }

        string? text = value switch
        {
            string stringValue => stringValue,
            bool booleanValue when EditorKind == AxamlPropertyEditorKind.Boolean =>
                booleanValue ? "True" : "False",
            _ => null,
        };
        if (text is null)
        {
            SetValidationError(EditorKind == AxamlPropertyEditorKind.Boolean
                ? "The value must be Boolean."
                : "The value must be text.");
            return false;
        }

        string? validationError = AxamlPropertyValueValidator.Validate(ControlType, Name, text);
        if (!string.IsNullOrWhiteSpace(validationError))
        {
            SetPropertyValue(text);
            SetValidationError(validationError);
            return false;
        }

        _validationResult = PropertyValidationResult.Valid;
        OnPropertyChanged(nameof(ValidationResult));
        return SetPropertyValue(text);
    }

    public bool IsTextEditor => !IsReadOnly && EditorKind == AxamlPropertyEditorKind.Text;

    public bool IsNumberEditor => !IsReadOnly && EditorKind == AxamlPropertyEditorKind.Number;

    public bool IsChoiceEditor => !IsReadOnly && EditorKind is AxamlPropertyEditorKind.Boolean or AxamlPropertyEditorKind.Enum;

    public bool HasValidationError => !string.IsNullOrEmpty(ValidationError) && !IsReadOnly;

    public bool HasReadOnlyReason => IsReadOnly && !string.IsNullOrEmpty(ReadOnlyReason);

    public void Synchronize(AxamlPropertyDescriptor descriptor)
    {
        if (!IsEditing && !IsDraft)
        {
            ValidationError = descriptor.ValidationError;
            _validationResult = string.IsNullOrWhiteSpace(descriptor.ValidationError)
                ? PropertyValidationResult.Valid
                : PropertyValidationResult.Invalid(descriptor.ValidationError);
            OnPropertyChanged(nameof(ValidationResult));
            Value = descriptor.Value;
            IsDraft = false;
        }
    }

    private bool SetPropertyValue(string value)
    {
        if (string.Equals(Value, value, StringComparison.Ordinal))
        {
            return true;
        }

        IsEditing = true;
        IsDraft = true;
        Value = value;
        return true;
    }

    private void SetValidationError(string message)
    {
        IsEditing = true;
        IsDraft = true;
        ValidationError = message;
        _validationResult = PropertyValidationResult.Invalid(message);
        OnPropertyChanged(nameof(ValidationResult));
    }

    private object GetPropertyValue() => ToPropertyValue(Value);

    private object ToPropertyValue(string value) =>
        EditorKind == AxamlPropertyEditorKind.Boolean
            && bool.TryParse(value, out bool booleanValue)
                ? booleanValue
                : value;

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsTextEditor));
        OnPropertyChanged(nameof(IsNumberEditor));
        OnPropertyChanged(nameof(IsChoiceEditor));
    }

    partial void OnValidationErrorChanged(string? value)
    {
        OnPropertyChanged(nameof(HasValidationError));
        OnPropertyChanged(nameof(HasReadOnlyReason));
    }
}
