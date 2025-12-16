using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

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
