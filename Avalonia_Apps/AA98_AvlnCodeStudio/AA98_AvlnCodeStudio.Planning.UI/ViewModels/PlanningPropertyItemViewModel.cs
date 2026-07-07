using AA98_AvlnCodeStudio.Base.UI.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace AA98_AvlnCodeStudio.Planning.UI.ViewModels;

/// <summary>
/// Represents a property entry in the planning properties panel.
/// </summary>
public partial class PlanningPropertyItemViewModel : ObservableObject, IPropertyItem
{
    private readonly Action<string?>? _applyValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningPropertyItemViewModel"/> class.
    /// </summary>
    /// <param name="name">The technical property name.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="value">The initial value.</param>
    /// <param name="isEditable">Indicates whether the value can be edited.</param>
    /// <param name="applyValue">Optional callback for propagating changes.</param>
    public PlanningPropertyItemViewModel(string name, string displayName, string? value, bool isEditable, Action<string?>? applyValue = null)
    {
        Name = name;
        DisplayName = displayName;
        _value = value;
        IsEditable = isEditable;
        _applyValue = applyValue;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string DisplayName { get; }

    /// <inheritdoc/>
    public bool IsEditable { get; }

    /// <inheritdoc/>
    public bool IsReadOnly => !IsEditable;

    [ObservableProperty]
    private string? _value;

    partial void OnValueChanged(string? value)
    {
        if (!IsEditable)
        {
            return;
        }

        _applyValue?.Invoke(value);
    }
}
