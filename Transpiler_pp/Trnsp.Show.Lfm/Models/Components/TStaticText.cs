using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TStaticText component (static text label with border).
/// </summary>
public partial class TStaticText : TLabel
{
    [ObservableProperty]
    private EStaticBorderStyle _staticBorderStyleKind = EStaticBorderStyle.None;

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "borderstyle":
                StaticBorderStyleKind = ParseBorderStyle(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static EStaticBorderStyle ParseBorderStyle(string? value) => value?.ToLower() switch
    {
        "sbnone" => EStaticBorderStyle.None,
        "sbsingle" => EStaticBorderStyle.Single,
        "sbsunken" => EStaticBorderStyle.Sunken,
        _ => EStaticBorderStyle.None
    };
}
