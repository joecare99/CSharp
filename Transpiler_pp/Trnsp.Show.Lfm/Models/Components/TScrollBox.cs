using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TScrollBox component (scrollable container).
/// </summary>
public partial class TScrollBox : TPanel
{
    [ObservableProperty]
    private bool _autoScroll = true;

    public TScrollBox()
    {
        BorderStyle = PanelBorderStyle.Single;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "autoscroll":
                AutoScroll = ConvertToBool(value, true);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
