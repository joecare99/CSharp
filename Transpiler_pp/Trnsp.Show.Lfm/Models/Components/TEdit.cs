using System;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TEdit component (single-line text input).
/// </summary>
public partial class TEdit : LfmComponentBase
{
    [ObservableProperty]
    private TextAlignment _alignment = TextAlignment.Left;

    [ObservableProperty]
    private bool _readOnly;

    [ObservableProperty]
    private int _maxLength;

    [ObservableProperty]
    private char _passwordChar;

    [ObservableProperty]
    private string _textHint = string.Empty;

    [ObservableProperty]
    private EditBorderStyle _borderStyle = EditBorderStyle.Single;

    public TEdit()
    {
        Height = 23;
        Width = 121;
        Color = Colors.White;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "alignment":
                Alignment = ParseAlignment(value?.ToString());
                break;
            case "readonly":
                ReadOnly = ConvertToBool(value);
                break;
            case "maxlength":
                MaxLength = ConvertToInt(value);
                break;
            case "passwordchar":
                var s = value?.ToString();
                PasswordChar = string.IsNullOrEmpty(s) ? '\0' : s[0];
                break;
            case "texthint":
                TextHint = value?.ToString() ?? string.Empty;
                break;
            case "borderstyle":
                BorderStyle = ParseBorderStyle(value?.ToString());
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

    private static EditBorderStyle ParseBorderStyle(string? value) => value?.ToLower() switch
    {
        "bsnone" => EditBorderStyle.None,
        "bssingle" => EditBorderStyle.Single,
        _ => EditBorderStyle.Single
    };
}

public enum EditBorderStyle
{
    None,
    Single
}

/// <summary>
/// Represents a TMemo component (multi-line text input).
/// </summary>
public partial class TMemo : TEdit
{
    [ObservableProperty]
    private bool _wordWrap = true;

    [ObservableProperty]
    private ScrollStyle _scrollBars = ScrollStyle.None;

    public TMemo()
    {
        Height = 89;
        Width = 185;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "wordwrap":
                WordWrap = ConvertToBool(value, true);
                break;
            case "scrollbars":
                ScrollBars = ParseScrollBars(value?.ToString());
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static ScrollStyle ParseScrollBars(string? value) => value?.ToLower() switch
    {
        "ssnone" => ScrollStyle.None,
        "sshorizontal" => ScrollStyle.Horizontal,
        "ssvertical" => ScrollStyle.Vertical,
        "ssboth" => ScrollStyle.Both,
        _ => ScrollStyle.None
    };
}

public enum ScrollStyle
{
    None,
    Horizontal,
    Vertical,
    Both
}
