using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TCheckBox component.
/// </summary>
public partial class TCheckBox : LfmComponentBase
{
    [ObservableProperty]
    private bool _checked;

    [ObservableProperty]
    private CheckBoxState _state = CheckBoxState.Unchecked;

    [ObservableProperty]
    private bool _allowGrayed;

    public TCheckBox()
    {
        Height = 19;
        Width = 97;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "checked":
                Checked = ConvertToBool(value);
                State = Checked ? CheckBoxState.Checked : CheckBoxState.Unchecked;
                break;
            case "state":
                State = ParseState(value?.ToString());
                Checked = State == CheckBoxState.Checked;
                break;
            case "allowgrayed":
                AllowGrayed = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static CheckBoxState ParseState(string? value) => value?.ToLower() switch
    {
        "cbunchecked" => CheckBoxState.Unchecked,
        "cbchecked" => CheckBoxState.Checked,
        "cbgrayed" => CheckBoxState.Grayed,
        _ => CheckBoxState.Unchecked
    };
}
