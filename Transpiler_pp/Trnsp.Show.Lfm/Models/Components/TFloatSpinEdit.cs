using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

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
