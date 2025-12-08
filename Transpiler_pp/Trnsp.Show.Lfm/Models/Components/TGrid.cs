using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TDrawGrid component.
/// </summary>
public partial class TDrawGrid : LfmComponentBase
{
    [ObservableProperty]
    private int _colCount = 5;

    [ObservableProperty]
    private int _rowCount = 5;

    [ObservableProperty]
    private int _defaultColWidth = 64;

    [ObservableProperty]
    private int _defaultRowHeight = 24;

    [ObservableProperty]
    private int _fixedCols = 1;

    [ObservableProperty]
    private int _fixedRows = 1;

    [ObservableProperty]
    private Color _fixedColor = Color.FromRgb(240, 240, 240);

    [ObservableProperty]
    private bool _extendedSelect = true;

    [ObservableProperty]
    private EGridScrollBars _scrollBarsKind = EGridScrollBars.Both;

    [ObservableProperty]
    private EGridOptions _options = EGridOptions.Default;

    public TDrawGrid()
    {
        Height = 120;
        Width = 320;
        Color = Colors.White;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "colcount":
                ColCount = ConvertToInt(value, 5);
                break;
            case "rowcount":
                RowCount = ConvertToInt(value, 5);
                break;
            case "defaultcolwidth":
                DefaultColWidth = ConvertToInt(value, 64);
                break;
            case "defaultrowheight":
                DefaultRowHeight = ConvertToInt(value, 24);
                break;
            case "fixedcols":
                FixedCols = ConvertToInt(value, 1);
                break;
            case "fixedrows":
                FixedRows = ConvertToInt(value, 1);
                break;
            case "fixedcolor":
                FixedColor = ConvertToColor(value, Color.FromRgb(240, 240, 240));
                break;
            case "extendedselect":
                ExtendedSelect = ConvertToBool(value, true);
                break;
            case "scrollbars":
                ScrollBarsKind = ParseScrollBars(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static EGridScrollBars ParseScrollBars(string? value) => value?.ToLower() switch
    {
        "ssnone" => EGridScrollBars.None,
        "sshorizontal" => EGridScrollBars.Horizontal,
        "ssvertical" => EGridScrollBars.Vertical,
        "ssboth" => EGridScrollBars.Both,
        "ssautoboth" => EGridScrollBars.AutoBoth,
        "ssautohorizontal" => EGridScrollBars.AutoHorizontal,
        "ssautovertical" => EGridScrollBars.AutoVertical,
        _ => EGridScrollBars.Both
    };
}

/// <summary>
/// Represents a TStringGrid component.
/// </summary>
public partial class TStringGrid : TDrawGrid
{
}

/// <summary>
/// Represents a TValueListEditor component.
/// </summary>
public partial class TValueListEditor : TDrawGrid
{
    [ObservableProperty]
    private string _titleCaptions = string.Empty;

    public TValueListEditor()
    {
        ColCount = 2;
        FixedCols = 0;
    }
}

public enum EGridScrollBars
{
    None,
    Horizontal,
    Vertical,
    Both,
    AutoBoth,
    AutoHorizontal,
    AutoVertical
}

[Flags]
public enum EGridOptions
{
    None = 0,
    FixedVertLine = 1,
    FixedHorzLine = 2,
    VertLine = 4,
    HorzLine = 8,
    RangeSelect = 16,
    DrawFocusSelected = 32,
    RowSizing = 64,
    ColSizing = 128,
    RowMoving = 256,
    ColMoving = 512,
    Editing = 1024,
    Tabs = 2048,
    AlwaysShowEditor = 4096,
    Default = FixedVertLine | FixedHorzLine | VertLine | HorzLine | RangeSelect | DrawFocusSelected
}
