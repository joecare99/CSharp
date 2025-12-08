using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TSpeedButton component (flat button, usually in toolbar).
/// </summary>
public partial class TSpeedButton : LfmComponentBase
{
    [ObservableProperty]
    private bool _flat = true;

    [ObservableProperty]
    private int _groupIndex;

    [ObservableProperty]
    private bool _down;

    [ObservableProperty]
    private bool _allowAllUp;

    public TSpeedButton()
    {
        Height = 22;
        Width = 23;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "flat":
                Flat = ConvertToBool(value, true);
                break;
            case "groupindex":
                GroupIndex = ConvertToInt(value);
                break;
            case "down":
                Down = ConvertToBool(value);
                break;
            case "allowallup":
                AllowAllUp = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
