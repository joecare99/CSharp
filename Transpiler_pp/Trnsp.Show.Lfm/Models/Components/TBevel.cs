using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TBevel component.
/// </summary>
public partial class TBevel : LfmComponentBase
{
    [ObservableProperty]
    private EBevelShape _bevelShapeKind = EBevelShape.Box;

    [ObservableProperty]
    private EBevelStyle _bevelStyleKind = EBevelStyle.Lowered;

    public TBevel()
    {
        Height = 50;
        Width = 50;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "shape":
                BevelShapeKind = ParseShape(value?.ToString());
                break;
            case "style":
                BevelStyleKind = ParseStyle(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static EBevelShape ParseShape(string? value) => value?.ToLower() switch
    {
        "bsbox" => EBevelShape.Box,
        "bsframe" => EBevelShape.Frame,
        "bstopline" => EBevelShape.TopLine,
        "bsbottomline" => EBevelShape.BottomLine,
        "bsleftline" => EBevelShape.LeftLine,
        "bsrightline" => EBevelShape.RightLine,
        "bsspacer" => EBevelShape.Spacer,
        _ => EBevelShape.Box
    };

    private static EBevelStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "bslowered" => EBevelStyle.Lowered,
        "bsraised" => EBevelStyle.Raised,
        _ => EBevelStyle.Lowered
    };
}
