using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TToolBar component.
/// </summary>
public partial class TToolBar : LfmComponentBase
{
    [ObservableProperty]
    private string _images = string.Empty;

    [ObservableProperty]
    private bool _flat = true;

    [ObservableProperty]
    private bool _showCaptions;

    [ObservableProperty]
    private bool _wrapable = true;

    [ObservableProperty]
    private int _buttonWidth = 23;

    [ObservableProperty]
    private int _buttonHeight = 22;

    /// <summary>
    /// Reference to the resolved ImageList component.
    /// </summary>
    public TImageList? ImageList { get; set; }

    public List<TToolButton> Buttons { get; } = [];

    public TToolBar()
    {
        Height = 26;
        Width = 400;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "images":
                Images = value?.ToString() ?? string.Empty;
                break;
            case "flat":
                Flat = ConvertToBool(value, true);
                break;
            case "showcaptions":
                ShowCaptions = ConvertToBool(value);
                break;
            case "wrapable":
                Wrapable = ConvertToBool(value, true);
                break;
            case "buttonwidth":
                ButtonWidth = ConvertToInt(value, 23);
                break;
            case "buttonheight":
                ButtonHeight = ConvertToInt(value, 22);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    /// <summary>
    /// Gets an image from the associated ImageList by index.
    /// </summary>
    public ImageSource? GetButtonImage(int imageIndex)
    {
        return ImageList?.GetImage(imageIndex);
    }
}

/// <summary>
/// Represents a TToolButton component.
/// </summary>
public partial class TToolButton : LfmComponentBase
{
    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private ToolButtonStyle _style = ToolButtonStyle.Button;

    [ObservableProperty]
    private bool _down;

    [ObservableProperty]
    private int _groupIndex;

    [ObservableProperty]
    private bool _allowAllUp;

    [ObservableProperty]
    private bool _marked;

    [ObservableProperty]
    private bool _wrap;

    /// <summary>
    /// Indicates whether ImageIndex was explicitly set.
    /// </summary>
    private bool _imageIndexExplicitlySet;

    /// <summary>
    /// Gets the effective image index, considering linked action.
    /// </summary>
    public int EffectiveImageIndex =>
        _imageIndexExplicitlySet || LinkedAction == null
            ? ImageIndex
            : (LinkedAction.ImageIndex >= 0 ? LinkedAction.ImageIndex : ImageIndex);

    public TToolButton()
    {
        Width = 23;
        Height = 22;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "action":
                // Use the base class ActionName property
                ActionName = value?.ToString() ?? string.Empty;
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                _imageIndexExplicitlySet = ImageIndex >= 0;
                break;
            case "style":
                Style = ParseStyle(value?.ToString());
                break;
            case "down":
                Down = ConvertToBool(value);
                break;
            case "groupindex":
                GroupIndex = ConvertToInt(value);
                break;
            case "allowallup":
                AllowAllUp = ConvertToBool(value);
                break;
            case "marked":
                Marked = ConvertToBool(value);
                break;
            case "wrap":
                Wrap = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    protected override void OnActionLinked()
    {
        // For TToolButton: inherit Hint from Action (not Caption!)
        if (LinkedAction == null) return;

        // Inherit Hint if not set locally
        if (string.IsNullOrEmpty(Hint) && !string.IsNullOrEmpty(LinkedAction.Hint))
        {
            Hint = LinkedAction.Hint;
        }

        // Inherit ImageIndex if not explicitly set
        if (!_imageIndexExplicitlySet && LinkedAction.ImageIndex >= 0)
        {
            ImageIndex = LinkedAction.ImageIndex;
        }

        OnPropertyChanged(nameof(EffectiveImageIndex));
        OnPropertyChanged(nameof(EffectiveHint));
        // Note: We do NOT call base.OnActionLinked() because TToolButton 
        // does NOT inherit Caption from Action
    }

    /// <summary>
    /// Gets the image from the parent ToolBar's ImageList.
    /// </summary>
    public ImageSource? GetImage()
    {
        var effectiveIndex = EffectiveImageIndex;
        if (effectiveIndex < 0) return null;

        // Find parent ToolBar
        if (Parent is TToolBar toolBar)
        {
            return toolBar.GetButtonImage(effectiveIndex);
        }

        return null;
    }

    private static ToolButtonStyle ParseStyle(string? value) => value?.ToLower() switch
    {
        "tbsbutton" => ToolButtonStyle.Button,
        "tbscheck" => ToolButtonStyle.Check,
        "tbsdropdown" => ToolButtonStyle.DropDown,
        "tbsseparator" => ToolButtonStyle.Separator,
        "tbsdivider" => ToolButtonStyle.Divider,
        _ => ToolButtonStyle.Button
    };
}

public enum ToolButtonStyle
{
    Button,
    Check,
    DropDown,
    Separator,
    Divider
}

/// <summary>
/// Represents a TCoolBar component.
/// </summary>
public partial class TCoolBar : LfmComponentBase
{
    [ObservableProperty]
    private bool _autoSize = true;

    [ObservableProperty]
    private bool _bandBorderStyle;

    public TCoolBar()
    {
        Height = 30;
        Width = 400;
    }
}
