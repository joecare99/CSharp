using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TMainMenu component.
/// </summary>
public partial class TMainMenu : LfmComponentBase
{
    [ObservableProperty]
    private string _images = string.Empty;

    public List<TMenuItem> MenuItems { get; } = [];

    public TMainMenu()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "images":
                Images = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TPopupMenu component.
/// </summary>
public partial class TPopupMenu : TMainMenu
{
}

/// <summary>
/// Represents a TMenuItem component.
/// </summary>
public partial class TMenuItem : LfmComponentBase
{
    [ObservableProperty]
    private string _linkedAction = string.Empty;

    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private string _shortCut = string.Empty;

    [ObservableProperty]
    private bool _menuItemChecked;

    [ObservableProperty]
    private bool _radioItem;

    [ObservableProperty]
    private int _groupIndex;

    [ObservableProperty]
    private bool _rightJustify;

    [ObservableProperty]
    private bool _isSeparator;

    public List<TMenuItem> SubItems { get; } = [];

    public TMenuItem()
    {
        Width = 100;
        Height = 22;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "action":
                LinkedAction = value?.ToString() ?? string.Empty;
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                break;
            case "shortcut":
                ShortCut = value?.ToString() ?? string.Empty;
                break;
            case "checked":
                MenuItemChecked = ConvertToBool(value);
                break;
            case "radioitem":
                RadioItem = ConvertToBool(value);
                break;
            case "groupindex":
                GroupIndex = ConvertToInt(value);
                break;
            case "rightjustify":
                RightJustify = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                // Check if it's a separator
                if (name.ToLower() == "caption" && Caption == "-")
                {
                    IsSeparator = true;
                }
                break;
        }
    }
}
