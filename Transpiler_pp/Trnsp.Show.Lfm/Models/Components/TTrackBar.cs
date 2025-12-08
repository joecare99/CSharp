using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

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
