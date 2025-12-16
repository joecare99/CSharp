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
