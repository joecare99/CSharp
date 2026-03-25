using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TListBox component.
/// </summary>
public partial class TListBox : LfmComponentBase
{
    [ObservableProperty]
    private int _itemIndex = -1;

    [ObservableProperty]
    private bool _multiSelect;

    [ObservableProperty]
    private ListBoxStyle _style = ListBoxStyle.Standard;

    public List<string> Items { get; } = [];

    public TListBox()
    {
        Height = 97;
        Width = 121;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "itemindex":
                ItemIndex = ConvertToInt(value, -1);
                break;
            case "multiselect":
                MultiSelect = ConvertToBool(value);
                break;
            case "style":
                Style = ParseStyle(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ListBoxStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "lbstandard" => ListBoxStyle.Standard,
        "lbownerdrawfixed" => ListBoxStyle.OwnerDrawFixed,
        "lbownerdrawvariable" => ListBoxStyle.OwnerDrawVariable,
        _ => ListBoxStyle.Standard
    };
}
