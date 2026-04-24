using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TLabeledEdit component (edit with integrated label).
/// </summary>
public partial class TLabeledEdit : TEdit
{
    [ObservableProperty]
    private string _labelCaption = string.Empty;

    [ObservableProperty]
    private ELabelPosition _labelPosKind = ELabelPosition.Above;

    [ObservableProperty]
    private int _labelSpacing = 3;

    [ObservableProperty]
    private int _labelLeft;

    [ObservableProperty]
    private int _labelTop;

    [ObservableProperty]
    private int _labelWidth;

    [ObservableProperty]
    private int _labelHeight = 15;

    public TLabeledEdit()
    {
        Height = 23;
        Width = 121;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "editlabel.caption":
                LabelCaption = value?.ToString() ?? string.Empty;
                break;
            case "labelposition":
                LabelPosKind = ParseLabelPosition(value?.ToString());
                break;
            case "labelspacing":
                LabelSpacing = ConvertToInt(value, 3);
                break;
            case "editlabel.left":
                LabelLeft = ConvertToInt(value);
                break;
            case "editlabel.top":
                LabelTop = ConvertToInt(value);
                break;
            case "editlabel.width":
                LabelWidth = ConvertToInt(value);
                break;
            case "editlabel.height":
                LabelHeight = ConvertToInt(value, 15);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ELabelPosition ParseLabelPosition(string? value) => value?.ToLower() switch
    {
        "lpleft" => ELabelPosition.Left,
        "lpright" => ELabelPosition.Right,
        "lpabove" => ELabelPosition.Above,
        "lpbelow" => ELabelPosition.Below,
        _ => ELabelPosition.Above
    };
}
