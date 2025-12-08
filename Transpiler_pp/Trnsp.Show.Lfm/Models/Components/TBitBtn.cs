using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TBitBtn component (button with bitmap/glyph).
/// </summary>
public partial class TBitBtn : TButton
{
    [ObservableProperty]
    private BitBtnKind _kind = BitBtnKind.Custom;

    [ObservableProperty]
    private ButtonLayout _layout = ButtonLayout.Left;

    [ObservableProperty]
    private int _spacing = 4;

    [ObservableProperty]
    private int _margin = -1;

    public TBitBtn()
    {
        Height = 30;
        Width = 75;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "kind":
                Kind = ParseKind(value?.ToString());
                break;
            case "layout":
                Layout = ParseLayout(value?.ToString());
                break;
            case "spacing":
                Spacing = ConvertToInt(value, 4);
                break;
            case "margin":
                Margin = ConvertToInt(value, -1);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static BitBtnKind ParseKind(string? value) => value?.ToLower() switch
    {
        "bkcustom" => BitBtnKind.Custom,
        "bkok" => BitBtnKind.OK,
        "bkcancel" => BitBtnKind.Cancel,
        "bkhelp" => BitBtnKind.Help,
        "bkyes" => BitBtnKind.Yes,
        "bkno" => BitBtnKind.No,
        "bkclose" => BitBtnKind.Close,
        "bkabort" => BitBtnKind.Abort,
        "bkretry" => BitBtnKind.Retry,
        "bkignore" => BitBtnKind.Ignore,
        "bkall" => BitBtnKind.All,
        _ => BitBtnKind.Custom
    };

    private static ButtonLayout ParseLayout(string? value) => value?.ToLower() switch
    {
        "blleft" => ButtonLayout.Left,
        "blright" => ButtonLayout.Right,
        "bltop" => ButtonLayout.Top,
        "blbottom" => ButtonLayout.Bottom,
        _ => ButtonLayout.Left
    };
}
