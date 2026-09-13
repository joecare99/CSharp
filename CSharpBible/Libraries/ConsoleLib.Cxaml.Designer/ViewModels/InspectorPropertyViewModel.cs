using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ConsoleLib.Interfaces;
using Property.Editor;

namespace ConsoleLib.Cxaml.Designer.ViewModels;

public sealed partial class InspectorPropertyViewModel : ObservableObject, IPropertyItem
{
    private readonly IControl _control;
    private readonly PropertyDescriptor? _property;
    private readonly Action<string, string> _apply;

    public InspectorPropertyViewModel(
        IControl control,
        string name,
        string category,
        Type valueType,
        string value,
        bool isReadOnly,
        Action<string, string> apply)
    {
        _control = control;
        Name = name;
        Category = category;
        ValueType = valueType;
        Value = value;
        IsReadOnly = isReadOnly;
        _property = TypeDescriptor.GetProperties(control)[name];
        _apply = apply;
    }

    public string Name { get; }
    public string Category { get; }
    public Type ValueType { get; }
    public bool IsReadOnly { get; }
    public bool IsBoolean => ValueType == typeof(bool);
    public bool IsEnum => ValueType.IsEnum;
    public bool IsColor => ValueType == typeof(ConsoleColor);
    public IReadOnlyList<string> Choices => IsBoolean
        ? new[] { bool.FalseString, bool.TrueString }
        : IsEnum
            ? Enum.GetNames(ValueType)
            : Array.Empty<string>();

    [ObservableProperty]
    private string _value;

    /// <inheritdoc />
    PropertyCategory IPropertyItem.Category => new(Category, Category);

    /// <inheritdoc />
    string IPropertyItem.DisplayName => Name;

    /// <inheritdoc />
    PropertyEditorKind IPropertyItem.EditorKind => IsBoolean
        ? PropertyEditorKind.Boolean
        : IsEnum
            ? PropertyEditorKind.Enum
            : PropertyEditorKind.Scalar;

    /// <inheritdoc />
    bool IPropertyItem.IsNullable => false;

    /// <inheritdoc />
    bool IPropertyItem.IsEditable => !IsReadOnly;

    /// <inheritdoc />
    bool IPropertyItem.IsSensitive => false;

    /// <inheritdoc />
    object? IPropertyItem.Value => Value;

    /// <inheritdoc />
    IReadOnlyList<PropertyOption> IPropertyItem.Options => Choices
        .Select(choice => new PropertyOption(choice, choice))
        .ToArray();

    /// <inheritdoc />
    PropertyValidationResult IPropertyItem.ValidationResult => PropertyValidationResult.Valid;

    /// <inheritdoc />
    int IPropertyItem.SortOrder => 0;

    /// <inheritdoc />
    event EventHandler<PropertyValueChangedEventArgs>? IPropertyItem.ValueChanged
    {
        add { }
        remove { }
    }

    public void Refresh()
    {
        if (_property is not null)
            Value = Convert.ToString(_property.GetValue(_control), CultureInfo.InvariantCulture) ?? string.Empty;
    }

    public void ApplyValue(string value) => _apply(Name, value);

    /// <inheritdoc />
    bool IPropertyItem.TrySetValue(object? value)
    {
        if (IsReadOnly || value is null)
        {
            return false;
        }

        ApplyValue(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
        Refresh();
        return true;
    }
}
