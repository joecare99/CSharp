using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TComboBox component.
/// </summary>
public partial class TComboBox : LfmComponentBase
{
    [ObservableProperty]
    private ComboBoxStyle _style = ComboBoxStyle.DropDown;

    [ObservableProperty]
    private int _itemIndex = -1;

    [ObservableProperty]
    private int _dropDownCount = 8;

    public List<string> Items { get; } = [];

    public TComboBox()
    {
        Height = 23;
        Width = 145;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "style":
                Style = ParseStyle(value?.ToString());
                break;
            case "itemindex":
                ItemIndex = ConvertToInt(value, -1);
                break;
            case "dropdowncount":
                DropDownCount = ConvertToInt(value, 8);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ComboBoxStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "csdropdown" => ComboBoxStyle.DropDown,
        "cssimple" => ComboBoxStyle.Simple,
        "csdropdownlist" => ComboBoxStyle.DropDownList,
        "csownerdrawfixed" => ComboBoxStyle.OwnerDrawFixed,
        "csownerdrawvariable" => ComboBoxStyle.OwnerDrawVariable,
        _ => ComboBoxStyle.DropDown
    };
}
