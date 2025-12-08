using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

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
