using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TStatusBar component.
/// </summary>
public partial class TStatusBar : LfmComponentBase
{
    [ObservableProperty]
    private bool _simplePanel;

    [ObservableProperty]
    private string _simpleText = string.Empty;

    public List<TStatusPanel> Panels { get; } = [];

    public TStatusBar()
    {
        Height = 23;
        Width = 400;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "simplepanel":
                SimplePanel = ConvertToBool(value);
                break;
            case "simpletext":
                SimpleText = value?.ToString() ?? string.Empty;
                break;
            case "panels":
                if (value is List<LfmItem> items)
                {
                    Panels.Clear();
                    foreach (var item in items)
                    {
                        var panel = new TStatusPanel();
                        foreach (var prop in item.Properties)
                        {
                            switch (prop.Name.ToLower())
                            {
                                case "width":
                                    panel.Width = ConvertToInt(prop.Value, 50);
                                    break;
                                case "text":
                                    panel.Text = prop.Value?.ToString() ?? string.Empty;
                                    break;
                                case "style":
                                    if (prop.Value?.ToString()?.Equals("psOwnerDraw", StringComparison.OrdinalIgnoreCase) == true)
                                        panel.Style = StatusPanelStyle.OwnerDraw;
                                    break;
                                case "bevel":
                                    panel.Bevel = prop.Value?.ToString()?.ToLower() switch
                                    {
                                        "pbnone" => StatusPanelBevel.None,
                                        "pbraised" => StatusPanelBevel.Raised,
                                        _ => StatusPanelBevel.Lowered
                                    };
                                    break;
                                case "alignment":
                                    panel.Alignment = prop.Value?.ToString()?.ToLower() switch
                                    {
                                        "tacenter" => System.Windows.TextAlignment.Center,
                                        "taright" => System.Windows.TextAlignment.Right,
                                        "tarightjustify" => System.Windows.TextAlignment.Right,
                                        _ => System.Windows.TextAlignment.Left
                                    };
                                    break;
                            }
                        }
                        Panels.Add(panel);
                    }
                }
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TStatusPanel (panel in a status bar).
/// </summary>
public partial class TStatusPanel : ObservableObject
{
    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private int _width = 50;

    [ObservableProperty]
    private StatusPanelStyle _style = StatusPanelStyle.Text;

    [ObservableProperty]
    private StatusPanelBevel _bevel = StatusPanelBevel.Lowered;

    [ObservableProperty]
    private System.Windows.TextAlignment _alignment = System.Windows.TextAlignment.Left;
}

public enum StatusPanelStyle
{
    Text,
    OwnerDraw
}

public enum StatusPanelBevel
{
    None,
    Lowered,
    Raised
}
