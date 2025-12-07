using System;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TPanel component (container panel).
/// </summary>
public partial class TPanel : LfmComponentBase
{
    [ObservableProperty]
    private TextAlignment _alignment = TextAlignment.Center;

    [ObservableProperty]
    private PanelBevelStyle _bevelInner = PanelBevelStyle.None;

    [ObservableProperty]
    private PanelBevelStyle _bevelOuter = PanelBevelStyle.Raised;

    [ObservableProperty]
    private int _bevelWidth = 1;

    [ObservableProperty]
    private int _borderWidth;

    [ObservableProperty]
    private PanelBorderStyle _borderStyle = PanelBorderStyle.None;

    [ObservableProperty]
    private bool _parentBackground = true;

    public TPanel()
    {
        Height = 185;
        Width = 185;
        Color = Color.FromRgb(240, 240, 240);
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "alignment":
                Alignment = ParseAlignment(value?.ToString());
                break;
            case "bevelinner":
                BevelInner = ParseBevelStyle(value?.ToString());
                break;
            case "bevelouter":
                BevelOuter = ParseBevelStyle(value?.ToString());
                break;
            case "bevelwidth":
                BevelWidth = ConvertToInt(value, 1);
                break;
            case "borderwidth":
                BorderWidth = ConvertToInt(value);
                break;
            case "borderstyle":
                BorderStyle = ParseBorderStyle(value?.ToString());
                break;
            case "parentbackground":
                ParentBackground = ConvertToBool(value, true);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static TextAlignment ParseAlignment(string? value) => value?.ToLower() switch
    {
        "taleftjustify" => TextAlignment.Left,
        "tacenter" => TextAlignment.Center,
        "tarightjustify" => TextAlignment.Right,
        _ => TextAlignment.Center
    };

    private static PanelBevelStyle ParseBevelStyle(string? value) => value?.ToLower() switch
    {
        "bvnone" => PanelBevelStyle.None,
        "bvlowered" => PanelBevelStyle.Lowered,
        "bvraised" => PanelBevelStyle.Raised,
        "bvspace" => PanelBevelStyle.Space,
        _ => PanelBevelStyle.None
    };

    private static PanelBorderStyle ParseBorderStyle(string? value) => value?.ToLower() switch
    {
        "bsnone" => PanelBorderStyle.None,
        "bssingle" => PanelBorderStyle.Single,
        _ => PanelBorderStyle.None
    };
}

public enum PanelBevelStyle
{
    None,
    Lowered,
    Raised,
    Space
}

public enum PanelBorderStyle
{
    None,
    Single
}

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
