using System.Collections.Generic;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TPageControl component (tabbed notebook).
/// </summary>
public partial class TPageControl : LfmComponentBase
{
    [ObservableProperty]
    private int _tabIndex;

    [ObservableProperty]
    private string _activePage = string.Empty;

    [ObservableProperty]
    private TabPosition _tabPosition = TabPosition.Top;

    [ObservableProperty]
    private TabStyle _style = TabStyle.Tabs;

    [ObservableProperty]
    private bool _multiLine;

    [ObservableProperty]
    private bool _hotTrack;

    [ObservableProperty]
    private int _tabHeight;

    [ObservableProperty]
    private int _tabWidth;

    /// <summary>
    /// Gets the tab sheets in this page control.
    /// </summary>
    public List<TTabSheet> TabSheets { get; } = [];

    public TPageControl()
    {
        Width = 300;
        Height = 200;
        Color = Color.FromRgb(240, 240, 240);
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "tabindex":
                TabIndex = ConvertToInt(value);
                break;
            case "activepage":
                ActivePage = value?.ToString() ?? string.Empty;
                break;
            case "tabposition":
                TabPosition = ParseTabPosition(value?.ToString());
                break;
            case "style":
                Style = ParseTabStyle(value?.ToString());
                break;
            case "multiline":
                MultiLine = ConvertToBool(value);
                break;
            case "hottrack":
                HotTrack = ConvertToBool(value);
                break;
            case "tabheight":
                TabHeight = ConvertToInt(value);
                break;
            case "tabwidth":
                TabWidth = ConvertToInt(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static TabPosition ParseTabPosition(string? value) => value?.ToLower() switch
    {
        "tptop" => TabPosition.Top,
        "tpbottom" => TabPosition.Bottom,
        "tpleft" => TabPosition.Left,
        "tpright" => TabPosition.Right,
        _ => TabPosition.Top
    };

    private static TabStyle ParseTabStyle(string? value) => value?.ToLower() switch
    {
        "tstabs" => TabStyle.Tabs,
        "tsbuttons" => TabStyle.Buttons,
        "tsflatbuttons" => TabStyle.FlatButtons,
        _ => TabStyle.Tabs
    };
}

/// <summary>
/// Represents a TTabSheet component (a page in a TPageControl).
/// </summary>
public partial class TTabSheet : LfmComponentBase
{
    [ObservableProperty]
    private int _pageIndex;

    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private bool _tabVisible = true;

    [ObservableProperty]
    private int _clientWidth;

    [ObservableProperty]
    private int _clientHeight;

    public TTabSheet()
    {
        Width = 300;
        Height = 200;
        Color = Color.FromRgb(240, 240, 240);
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "pageindex":
                PageIndex = ConvertToInt(value);
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                break;
            case "tabvisible":
                TabVisible = ConvertToBool(value, true);
                break;
            case "clientwidth":
                ClientWidth = ConvertToInt(value);
                break;
            case "clientheight":
                ClientHeight = ConvertToInt(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Tab position for TPageControl.
/// </summary>
public enum TabPosition
{
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
/// Tab style for TPageControl.
/// </summary>
public enum TabStyle
{
    Tabs,
    Buttons,
    FlatButtons
}

/// <summary>
/// Represents a TTabbedNotebook component (legacy Delphi component, mapped to TPageControl).
/// </summary>
public partial class TTabbedNotebook : TPageControl
{
}

/// <summary>
/// Represents a TNotebook component (legacy, mapped to TPageControl).
/// </summary>
public partial class TNotebook : TPageControl
{
}

/// <summary>
/// Represents a TPage component (legacy page in TNotebook).
/// </summary>
public partial class TPage : TTabSheet
{
}
