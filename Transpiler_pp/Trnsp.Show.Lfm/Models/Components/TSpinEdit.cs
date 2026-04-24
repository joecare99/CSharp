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
