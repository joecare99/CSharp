using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TToolBar component.
/// </summary>
public partial class TToolBar : LfmComponentBase
{
    [ObservableProperty]
    private string _images = string.Empty;

    [ObservableProperty]
    private bool _flat = true;

    [ObservableProperty]
    private bool _showCaptions;

    [ObservableProperty]
    private bool _wrapable = true;

    [ObservableProperty]
    private int _buttonWidth = 23;

    [ObservableProperty]
    private int _buttonHeight = 22;

    public List<TToolButton> Buttons { get; } = [];

    public TToolBar()
    {
        Height = 26;
        Width = 400;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "images":
                Images = value?.ToString() ?? string.Empty;
                break;
            case "flat":
                Flat = ConvertToBool(value, true);
                break;
            case "showcaptions":
                ShowCaptions = ConvertToBool(value);
                break;
            case "wrapable":
                Wrapable = ConvertToBool(value, true);
                break;
            case "buttonwidth":
                ButtonWidth = ConvertToInt(value, 23);
                break;
            case "buttonheight":
                ButtonHeight = ConvertToInt(value, 22);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TToolButton component.
/// </summary>
public partial class TToolButton : LfmComponentBase
{
    [ObservableProperty]
    private string _action = string.Empty;

    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private ToolButtonStyle _style = ToolButtonStyle.Button;

    [ObservableProperty]
    private bool _down;

    [ObservableProperty]
    private int _groupIndex;

    [ObservableProperty]
    private bool _allowAllUp;

    [ObservableProperty]
    private bool _marked;

    [ObservableProperty]
    private bool _wrap;

    public TToolButton()
    {
        Width = 23;
        Height = 22;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "action":
                Action = value?.ToString() ?? string.Empty;
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                break;
            case "style":
                Style = ParseStyle(value?.ToString());
                break;
            case "down":
                Down = ConvertToBool(value);
                break;
            case "groupindex":
                GroupIndex = ConvertToInt(value);
                break;
            case "allowallup":
                AllowAllUp = ConvertToBool(value);
                break;
            case "marked":
                Marked = ConvertToBool(value);
                break;
            case "wrap":
                Wrap = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ToolButtonStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "tbsbutton" => ToolButtonStyle.Button,
        "tbscheck" => ToolButtonStyle.Check,
        "tbsdropdown" => ToolButtonStyle.DropDown,
        "tbsseparator" => ToolButtonStyle.Separator,
        "tbsdivider" => ToolButtonStyle.Divider,
        _ => ToolButtonStyle.Button
    };
}

public enum ToolButtonStyle
{
    Button,
    Check,
    DropDown,
    Separator,
    Divider
}

/// <summary>
/// Represents a TCoolBar component.
/// </summary>
public partial class TCoolBar : LfmComponentBase
{
    [ObservableProperty]
    private bool _autoSize = true;

    [ObservableProperty]
    private bool _bandBorderStyle;

    public TCoolBar()
    {
        Height = 30;
        Width = 400;
    }
}
