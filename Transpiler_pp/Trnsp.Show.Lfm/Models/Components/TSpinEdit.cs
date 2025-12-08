using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TSpinEdit component (numeric input with up/down buttons).
/// </summary>
public partial class TSpinEdit : LfmComponentBase
{
    [ObservableProperty]
    private int _value;

    [ObservableProperty]
    private int _minValue = 0;

    [ObservableProperty]
    private int _maxValue = 100;

    [ObservableProperty]
    private int _increment = 1;

    [ObservableProperty]
    private bool _readOnly;

    public TSpinEdit()
    {
        Height = 23;
        Width = 50;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "value":
                Value = ConvertToInt(value);
                break;
            case "minvalue":
                MinValue = ConvertToInt(value, 0);
                break;
            case "maxvalue":
                MaxValue = ConvertToInt(value, 100);
                break;
            case "increment":
                Increment = ConvertToInt(value, 1);
                break;
            case "readonly":
                ReadOnly = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TFloatSpinEdit component (floating-point numeric input).
/// </summary>
public partial class TFloatSpinEdit : LfmComponentBase
{
    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private double _minValue = 0;

    [ObservableProperty]
    private double _maxValue = 100;

    [ObservableProperty]
    private double _increment = 1;

    [ObservableProperty]
    private int _decimalPlaces = 2;

    public TFloatSpinEdit()
    {
        Height = 23;
        Width = 65;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "value":
                Value = ConvertToDouble(value);
                break;
            case "minvalue":
                MinValue = ConvertToDouble(value, 0);
                break;
            case "maxvalue":
                MaxValue = ConvertToDouble(value, 100);
                break;
            case "increment":
                Increment = ConvertToDouble(value, 1);
                break;
            case "decimalplaces":
                DecimalPlaces = ConvertToInt(value, 2);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TUpDown component (spin button without edit).
/// </summary>
public partial class TUpDown : LfmComponentBase
{
    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    private int _min = 0;

    [ObservableProperty]
    private int _max = 100;

    [ObservableProperty]
    private int _increment = 1;

    [ObservableProperty]
    private UpDownOrientation _orientation = UpDownOrientation.Vertical;

    [ObservableProperty]
    private string _associate = string.Empty;

    public TUpDown()
    {
        Height = 23;
        Width = 17;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "position":
                Position = ConvertToInt(value);
                break;
            case "min":
                Min = ConvertToInt(value, 0);
                break;
            case "max":
                Max = ConvertToInt(value, 100);
                break;
            case "increment":
                Increment = ConvertToInt(value, 1);
                break;
            case "orientation":
                Orientation = ParseOrientation(value?.ToString());
                break;
            case "associate":
                Associate = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static UpDownOrientation ParseOrientation(string? value) => value?.ToLower() switch
    {
        "udvertical" => UpDownOrientation.Vertical,
        "udhorizontal" => UpDownOrientation.Horizontal,
        _ => UpDownOrientation.Vertical
    };
}

public enum UpDownOrientation
{
    Vertical,
    Horizontal
}
