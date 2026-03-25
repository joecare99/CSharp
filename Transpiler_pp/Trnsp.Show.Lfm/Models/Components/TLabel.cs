using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TLabel component (static text label).
/// </summary>
public partial class TLabel : LfmComponentBase
{
    [ObservableProperty]
    private TextAlignment _alignment = TextAlignment.Left;

    [ObservableProperty]
    private bool _autoSize = true;

    [ObservableProperty]
    private bool _wordWrap;

    [ObservableProperty]
    private bool _transparent = true;

    public TLabel()
    {
        Height = 15;
        Width = 50;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "alignment":
                Alignment = ParseAlignment(value?.ToString());
                break;
            case "autosize":
                AutoSize = ConvertToBool(value, true);
                break;
            case "wordwrap":
                WordWrap = ConvertToBool(value);
                break;
            case "transparent":
                Transparent = ConvertToBool(value, true);
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
        _ => TextAlignment.Left
    };
}
