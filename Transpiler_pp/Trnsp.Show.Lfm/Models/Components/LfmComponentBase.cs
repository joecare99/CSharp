using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Base class for all Pascal/Delphi visual components.
/// </summary>
public abstract partial class LfmComponentBase : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _left;

    [ObservableProperty]
    private int _top;

    [ObservableProperty]
    private int _width = 100;

    [ObservableProperty]
    private int _height = 25;

    [ObservableProperty]
    private string _caption = string.Empty;

    [ObservableProperty]
    private bool _visible = true;

    [ObservableProperty]
    private bool _enabled = true;

    [ObservableProperty]
    private string _hint = string.Empty;

    [ObservableProperty]
    private int _tabOrder;

    [ObservableProperty]
    private Color _color = Colors.Transparent;

    [ObservableProperty]
    private string _fontName = "Segoe UI";

    [ObservableProperty]
    private double _fontSize = 12;

    [ObservableProperty]
    private Color _fontColor = Colors.Black;

    [ObservableProperty]
    private FontWeight _fontWeight = FontWeights.Normal;

    [ObservableProperty]
    private FontStyle _fontStyle = FontStyles.Normal;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private LfmComponentBase? _parent;

    /// <summary>
    /// Picture/Bitmap data for components that support images.
    /// </summary>
    private TBitmap? _picture;
    public TBitmap? Picture
    {
        get => _picture;
        set => SetProperty(ref _picture, value);
    }

    /// <summary>
    /// Glyph/Icon bitmap data for buttons and similar components.
    /// </summary>
    private TBitmap? _glyph;
    public TBitmap? Glyph
    {
        get => _glyph;
        set => SetProperty(ref _glyph, value);
    }

    /// <summary>
    /// Name of the linked action (from Action property in LFM).
    /// </summary>
    private string _actionName = string.Empty;
    public string ActionName
    {
        get => _actionName;
        set => SetProperty(ref _actionName, value);
    }

    /// <summary>
    /// Reference to the resolved TAction component.
    /// </summary>
    private TAction? _linkedAction;
    public TAction? LinkedAction
    {
        get => _linkedAction;
        set
        {
            if (SetProperty(ref _linkedAction, value))
            {
                OnActionLinked();
            }
        }
    }

    /// <summary>
    /// Indicates whether the Caption was explicitly set in the LFM file.
    /// </summary>
    protected bool CaptionExplicitlySet { get; private set; }

    public ObservableCollection<LfmComponentBase> Children { get; } = [];

    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// Temporary storage for multi-line hex data during property parsing.
    /// </summary>
    private readonly Dictionary<string, List<string>> _pendingHexData = [];

    /// <summary>
    /// Applies properties from an LfmObject to this component.
    /// </summary>
    public virtual void ApplyProperties(LfmObject lfmObject)
    {
        Name = lfmObject.Name;
        TypeName = lfmObject.TypeName;

        foreach (var prop in lfmObject.Properties)
        {
            ApplyProperty(prop.Name, prop.Value);
        }

        // Process any pending hex data
        FinalizePendingHexData();
    }

    /// <summary>
    /// Applies a single property value.
    /// </summary>
    protected virtual void ApplyProperty(string name, object? value)
    {
        var lowerName = name.ToLower();
        switch (lowerName)
        {
            case "left":
                Left = ConvertToInt(value);
                break;
            case "top":
                Top = ConvertToInt(value);
                break;
            case "width":
                Width = ConvertToInt(value, 100);
                break;
            case "height":
                Height = ConvertToInt(value, 25);
                break;
            case "caption":
                Caption = value?.ToString() ?? string.Empty;
                CaptionExplicitlySet = !string.IsNullOrEmpty(Caption);
                break;
            case "text":
                Caption = value?.ToString() ?? string.Empty;
                CaptionExplicitlySet = !string.IsNullOrEmpty(Caption);
                break;
            case "visible":
                Visible = ConvertToBool(value, true);
                break;
            case "enabled":
                Enabled = ConvertToBool(value, true);
                break;
            case "hint":
                Hint = value?.ToString() ?? string.Empty;
                break;
            case "taborder":
                TabOrder = ConvertToInt(value);
                break;
            case "color":
                Color = ConvertToColor(value);
                break;
            case "font.name":
                FontName = value?.ToString() ?? "Segoe UI";
                break;
            case "font.size":
                FontSize = ConvertToDouble(value, 12);
                break;
            case "font.color":
                FontColor = ConvertToColor(value, Colors.Black);
                break;
            case "parentcolor":
                // Handled by parent assignment
                break;
            case "action":
                ActionName = value?.ToString() ?? string.Empty;
                break;
            case "picture.data":
                ApplyHexData("picture", value);
                break;
            case "glyph.data":
                ApplyHexData("glyph", value);
                break;
            case "bitmap.data":
                ApplyHexData("bitmap", value);
                break;
        }
    }

    /// <summary>
    /// Called when an action is linked to this component.
    /// Override in derived classes to inherit specific properties from the action.
    /// </summary>
    protected virtual void OnActionLinked()
    {
        if (LinkedAction == null) return;

        // Default behavior: inherit Caption if not explicitly set
        if (!CaptionExplicitlySet && !string.IsNullOrEmpty(LinkedAction.Caption))
        {
            Caption = LinkedAction.Caption;
        }

        // Inherit Hint if not set
        if (string.IsNullOrEmpty(Hint) && !string.IsNullOrEmpty(LinkedAction.Hint))
        {
            Hint = LinkedAction.Hint;
        }

        // Inherit Enabled state
        // Note: In Delphi, Action.Enabled typically controls the component's enabled state
    }

    /// <summary>
    /// Gets the effective caption, considering linked action.
    /// </summary>
    public virtual string EffectiveCaption => 
        CaptionExplicitlySet || LinkedAction == null 
            ? Caption 
            : (string.IsNullOrEmpty(LinkedAction.Caption) ? Caption : LinkedAction.Caption);

    /// <summary>
    /// Gets the effective hint, considering linked action.
    /// </summary>
    public virtual string EffectiveHint =>
        !string.IsNullOrEmpty(Hint) || LinkedAction == null
            ? Hint
            : LinkedAction.Hint;

    /// <summary>
    /// Applies hex data for bitmap properties.
    /// </summary>
    protected void ApplyHexData(string propertyName, object? value)
    {
        if (value is IEnumerable<object> lines)
        {
            if (!_pendingHexData.ContainsKey(propertyName))
                _pendingHexData[propertyName] = [];

            foreach (var line in lines)
            {
                var lineStr = line?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(lineStr))
                {
                    _pendingHexData[propertyName].Add(lineStr);
                }
            }
        }
        else if (value is string hexStr && !string.IsNullOrWhiteSpace(hexStr))
        {
            if (!_pendingHexData.ContainsKey(propertyName))
                _pendingHexData[propertyName] = [];
            _pendingHexData[propertyName].Add(hexStr.Trim());
        }
    }

    /// <summary>
    /// Finalizes pending hex data into TBitmap objects.
    /// </summary>
    protected virtual void FinalizePendingHexData()
    {
        foreach (var kvp in _pendingHexData)
        {
            var combinedHex = string.Join("", kvp.Value);
            if (!string.IsNullOrEmpty(combinedHex))
            {
                var bitmap = TBitmap.FromHexData(combinedHex);
                switch (kvp.Key)
                {
                    case "picture":
                        Picture = bitmap;
                        break;
                    case "glyph":
                        Glyph = bitmap;
                        break;
                    case "bitmap":
                        // Default to glyph for generic bitmap property
                        Glyph ??= bitmap;
                        break;
                }
            }
        }
        _pendingHexData.Clear();
    }

    protected static int ConvertToInt(object? value, int defaultValue = 0)
    {
        return value switch
        {
            int i => i,
            string s when int.TryParse(s, out var result) => result,
            _ => defaultValue
        };
    }

    protected static double ConvertToDouble(object? value, double defaultValue = 0)
    {
        return value switch
        {
            double d => d,
            int i => i,
            string s when double.TryParse(s, out var result) => result,
            _ => defaultValue
        };
    }

    protected static bool ConvertToBool(object? value, bool defaultValue = false)
    {
        return value switch
        {
            bool b => b,
            string s => s.Equals("true", StringComparison.OrdinalIgnoreCase),
            _ => defaultValue
        };
    }

    protected static Color ConvertToColor(object? value, Color? defaultColor = null)
    {
        var def = defaultColor ?? Colors.Transparent;
        if (value is string colorStr)
        {
            return colorStr.ToLower() switch
            {
                "clbtnface" => Color.FromRgb(240, 240, 240),
                "clwindow" => Colors.White,
                "clwindowtext" => Colors.Black,
                "clbtntext" => Colors.Black,
                "clhighlight" => Color.FromRgb(0, 120, 215),
                "clhighlighttext" => Colors.White,
                "clgraytext" => Colors.Gray,
                "clblack" => Colors.Black,
                "clwhite" => Colors.White,
                "clred" => Colors.Red,
                "clgreen" => Colors.Green,
                "clblue" => Colors.Blue,
                "clyellow" => Colors.Yellow,
                "clnavy" => Colors.Navy,
                "clmaroon" => Colors.Maroon,
                "clsilver" => Colors.Silver,
                "clteal" => Colors.Teal,
                "claqua" => Colors.Aqua,
                "cllime" => Colors.Lime,
                "clfuchsia" => Colors.Fuchsia,
                "clpurple" => Colors.Purple,
                "clolive" => Colors.Olive,
                "cldefault" => def,
                "clnone" => Colors.Transparent,
                _ when colorStr.StartsWith("$") => ParseHexColor(colorStr),
                _ => def
            };
        }
        return def;
    }

    private static Color ParseHexColor(string hex)
    {
        try
        {
            hex = hex.TrimStart('$');
            if (hex.Length == 6)
            {
                // BGR format in Delphi
                var b = Convert.ToByte(hex.Substring(0, 2), 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var r = Convert.ToByte(hex.Substring(4, 2), 16);
                return Color.FromRgb(r, g, b);
            }
            if (hex.Length == 8)
            {
                var a = Convert.ToByte(hex.Substring(0, 2), 16);
                var b = Convert.ToByte(hex.Substring(2, 2), 16);
                var g = Convert.ToByte(hex.Substring(4, 2), 16);
                var r = Convert.ToByte(hex.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
        }
        catch { }
        return Colors.Transparent;
    }
}
