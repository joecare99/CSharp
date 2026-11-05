using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlPropertyRowViewModel : ObservableObject
{
    private string _value;

    public AxamlPropertyRowViewModel(string controlType, AxamlPropertyDescriptor descriptor)
    {
        ControlType = controlType;
        Name = descriptor.Name;
        EditorKind = descriptor.EditorKind;
        Options = descriptor.EditorKind == AxamlPropertyEditorKind.Boolean
            ? ["True", "False"]
            : descriptor.Options;
        IsReadOnly = descriptor.IsReadOnly;
        ReadOnlyReason = descriptor.ReadOnlyReason;
        _value = descriptor.Value;
        ValidationError = descriptor.ValidationError;
    }

    public string ControlType { get; }

    public string Name { get; }

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
            if (SetProperty(ref _value, value))
            {
                ValidationError = IsReadOnly
                    ? ReadOnlyReason
                    : AxamlPropertyValueValidator.Validate(ControlType, Name, value);
            }
        }
    }

    public bool IsTextEditor => !IsReadOnly && EditorKind == AxamlPropertyEditorKind.Text;

    public bool IsNumberEditor => !IsReadOnly && EditorKind == AxamlPropertyEditorKind.Number;

    public bool IsChoiceEditor => !IsReadOnly && EditorKind is AxamlPropertyEditorKind.Boolean or AxamlPropertyEditorKind.Enum;

    public bool HasValidationError => !string.IsNullOrEmpty(ValidationError) && !IsReadOnly;

    public bool HasReadOnlyReason => IsReadOnly && !string.IsNullOrEmpty(ReadOnlyReason);

    public void Synchronize(AxamlPropertyDescriptor descriptor)
    {
        ValidationError = descriptor.ValidationError;
        if (!IsEditing)
        {
            Value = descriptor.Value;
            IsDraft = false;
        }
    }

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
