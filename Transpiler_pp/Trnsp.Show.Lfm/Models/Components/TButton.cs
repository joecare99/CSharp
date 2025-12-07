using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TButton component (standard button).
/// </summary>
public partial class TButton : LfmComponentBase
{
    [ObservableProperty]
    private bool _default;

    [ObservableProperty]
    private bool _cancel;

    [ObservableProperty]
    private ModalResult _modalResult = ModalResult.None;

    public TButton()
    {
        Height = 25;
        Width = 75;
        Color = Color.FromRgb(240, 240, 240);
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "default":
                Default = ConvertToBool(value);
                break;
            case "cancel":
                Cancel = ConvertToBool(value);
                break;
            case "modalresult":
                ModalResult = ParseModalResult(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ModalResult ParseModalResult(string? value) => value?.ToLower() switch
    {
        "mrnone" => ModalResult.None,
        "mrok" => ModalResult.OK,
        "mrcancel" => ModalResult.Cancel,
        "mrabort" => ModalResult.Abort,
        "mrretry" => ModalResult.Retry,
        "mrignore" => ModalResult.Ignore,
        "mryes" => ModalResult.Yes,
        "mrno" => ModalResult.No,
        "mrclose" => ModalResult.Close,
        _ => ModalResult.None
    };
}

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

public enum ModalResult
{
    None,
    OK,
    Cancel,
    Abort,
    Retry,
    Ignore,
    Yes,
    No,
    Close
}

public enum BitBtnKind
{
    Custom,
    OK,
    Cancel,
    Help,
    Yes,
    No,
    Close,
    Abort,
    Retry,
    Ignore,
    All
}

public enum ButtonLayout
{
    Left,
    Right,
    Top,
    Bottom
}
