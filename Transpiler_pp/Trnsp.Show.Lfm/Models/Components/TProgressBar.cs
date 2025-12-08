using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

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
