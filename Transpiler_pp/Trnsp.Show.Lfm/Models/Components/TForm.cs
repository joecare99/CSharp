using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TForm component (main form/window).
/// </summary>
public partial class TForm : LfmComponentBase
{
    [ObservableProperty]
    private int _clientWidth;

    [ObservableProperty]
    private int _clientHeight;

    [ObservableProperty]
    private FormBorderStyle _borderStyle = FormBorderStyle.Sizeable;

    [ObservableProperty]
    private FormPosition _position = FormPosition.Designed;

    [ObservableProperty]
    private FormWindowState _windowState = FormWindowState.Normal;

    [ObservableProperty]
    private bool _showHint;

    public TForm()
    {
        Color = Color.FromRgb(240, 240, 240); // clBtnFace
        Width = 800;
        Height = 600;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "clientwidth":
                ClientWidth = ConvertToInt(value, 800);
                Width = ClientWidth + 16;
                break;
            case "clientheight":
                ClientHeight = ConvertToInt(value, 600);
                Height = ClientHeight + 39;
                break;
            case "borderstyle":
                BorderStyle = ParseBorderStyle(value?.ToString());
                break;
            case "position":
                Position = ParsePosition(value?.ToString());
                break;
            case "windowstate":
                WindowState = ParseWindowState(value?.ToString());
                break;
            case "showhint":
                ShowHint = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    private static FormBorderStyle ParseBorderStyle(string? value) => value?.ToLower() switch
    {
        "bsnone" => FormBorderStyle.None,
        "bssingle" => FormBorderStyle.Single,
        "bssizeable" => FormBorderStyle.Sizeable,
        "bsdialog" => FormBorderStyle.Dialog,
        "bstoolwindow" => FormBorderStyle.ToolWindow,
        "bssizetoolwin" => FormBorderStyle.SizeToolWin,
        _ => FormBorderStyle.Sizeable
    };

    private static FormPosition ParsePosition(string? value) => value?.ToLower() switch
    {
        "podesigned" => FormPosition.Designed,
        "podefault" => FormPosition.Default,
        "podefaultposonly" => FormPosition.DefaultPosOnly,
        "podefaultsizeonly" => FormPosition.DefaultSizeOnly,
        "poscreencenter" => FormPosition.ScreenCenter,
        "podesktopcenter" => FormPosition.DesktopCenter,
        "pomainformcenter" => FormPosition.MainFormCenter,
        "poownerfomcenter" => FormPosition.OwnerFormCenter,
        _ => FormPosition.Designed
    };

    private static FormWindowState ParseWindowState(string? value) => value?.ToLower() switch
    {
        "wsnormal" => FormWindowState.Normal,
        "wsminimized" => FormWindowState.Minimized,
        "wsmaximized" => FormWindowState.Maximized,
        _ => FormWindowState.Normal
    };
}

public enum FormBorderStyle
{
    None,
    Single,
    Sizeable,
    Dialog,
    ToolWindow,
    SizeToolWin
}

public enum FormPosition
{
    Designed,
    Default,
    DefaultPosOnly,
    DefaultSizeOnly,
    ScreenCenter,
    DesktopCenter,
    MainFormCenter,
    OwnerFormCenter
}

public enum FormWindowState
{
    Normal,
    Minimized,
    Maximized
}
