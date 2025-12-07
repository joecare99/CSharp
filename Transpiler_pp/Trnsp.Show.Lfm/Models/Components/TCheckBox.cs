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

public enum CheckBoxState
{
    Unchecked,
    Checked,
    Grayed
}

/// <summary>
/// Represents a TRadioButton component.
/// </summary>
public partial class TRadioButton : LfmComponentBase
{
    [ObservableProperty]
    private bool _checked;

    public TRadioButton()
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
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

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

public enum ComboBoxStyle
{
    DropDown,
    Simple,
    DropDownList,
    OwnerDrawFixed,
    OwnerDrawVariable
}

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

public enum ListBoxStyle
{
    Standard,
    OwnerDrawFixed,
    OwnerDrawVariable
}

/// <summary>
/// Represents a TTrackBar component (slider).
/// </summary>
public partial class TTrackBar : LfmComponentBase
{
    [ObservableProperty]
    private int _min;

    [ObservableProperty]
    private int _max = 10;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    private TrackBarOrientation _orientation = TrackBarOrientation.Horizontal;

    [ObservableProperty]
    private int _frequency = 1;

    public TTrackBar()
    {
        Height = 25;
        Width = 150;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "min":
                Min = ConvertToInt(value);
                break;
            case "max":
                Max = ConvertToInt(value, 10);
                break;
            case "position":
                Position = ConvertToInt(value);
                break;
            case "orientation":
                Orientation = ParseOrientation(value?.ToString());
                break;
            case "frequency":
                Frequency = ConvertToInt(value, 1);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static TrackBarOrientation ParseOrientation(string? value) => value?.ToLower() switch
    {
        "trhorizontal" => TrackBarOrientation.Horizontal,
        "trvertical" => TrackBarOrientation.Vertical,
        _ => TrackBarOrientation.Horizontal
    };
}

public enum TrackBarOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Represents a TProgressBar component.
/// </summary>
public partial class TProgressBar : LfmComponentBase
{
    [ObservableProperty]
    private int _min;

    [ObservableProperty]
    private int _max = 100;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    private ProgressBarOrientation _orientation = ProgressBarOrientation.Horizontal;

    public TProgressBar()
    {
        Height = 17;
        Width = 150;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "min":
                Min = ConvertToInt(value);
                break;
            case "max":
                Max = ConvertToInt(value, 100);
                break;
            case "position":
                Position = ConvertToInt(value);
                break;
            case "orientation":
                Orientation = ParseOrientation(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ProgressBarOrientation ParseOrientation(string? value) => value?.ToLower() switch
    {
        "pbhorizontal" => ProgressBarOrientation.Horizontal,
        "pbvertical" => ProgressBarOrientation.Vertical,
        _ => ProgressBarOrientation.Horizontal
    };
}

public enum ProgressBarOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// Represents a TImage/TPaintBox component.
/// </summary>
public partial class TImage : LfmComponentBase
{
    [ObservableProperty]
    private bool _stretch;

    [ObservableProperty]
    private bool _proportional;

    [ObservableProperty]
    private bool _center;

    public TImage()
    {
        Height = 105;
        Width = 105;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "stretch":
                Stretch = ConvertToBool(value);
                break;
            case "proportional":
                Proportional = ConvertToBool(value);
                break;
            case "center":
                Center = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TPaintBox component (drawing area).
/// </summary>
public partial class TPaintBox : TImage
{
}

/// <summary>
/// Represents a TTimer component (non-visual).
/// </summary>
public partial class TTimer : LfmComponentBase
{
    [ObservableProperty]
    private int _interval = 1000;

    public TTimer()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "interval":
                Interval = ConvertToInt(value, 1000);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Generic component for unknown types.
/// </summary>
public partial class TUnknownComponent : LfmComponentBase
{
    public TUnknownComponent()
    {
        Width = 28;
        Height = 28;
    }
}
