using System;
using System.Collections.Generic;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TMainMenu component.
/// </summary>
public partial class TMainMenu : LfmComponentBase
{
    [ObservableProperty]
    private string _images = string.Empty;

    /// <summary>
    /// Reference to the resolved ImageList component.
    /// </summary>
    public TImageList? ImageList { get; set; }

    public List<TMenuItem> MenuItems { get; } = [];

    public TMainMenu()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "images":
                Images = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    /// <summary>
    /// Gets an image from the associated ImageList by index.
    /// </summary>
    public ImageSource? GetMenuImage(int imageIndex)
    {
        return ImageList?.GetImage(imageIndex);
    }
}

/// <summary>
/// Represents a TPopupMenu component.
/// </summary>
public partial class TPopupMenu : TMainMenu
{
}

/// <summary>
/// Represents a TMenuItem component.
/// </summary>
public partial class TMenuItem : LfmComponentBase
{
    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private string _shortCut = string.Empty;

    [ObservableProperty]
    private bool _menuItemChecked;

    [ObservableProperty]
    private bool _radioItem;

    [ObservableProperty]
    private int _groupIndex;

    [ObservableProperty]
    private bool _rightJustify;

    [ObservableProperty]
    private bool _isSeparator;

    /// <summary>
    /// Indicates whether ShortCut was explicitly set.
    /// </summary>
    private bool _shortCutExplicitlySet;

    /// <summary>
    /// Indicates whether ImageIndex was explicitly set.
    /// </summary>
    private bool _imageIndexExplicitlySet;

    /// <summary>
    /// Gets the ImageSource for the menu item bitmap (from embedded Bitmap.Data).
    /// </summary>
    public ImageSource? BitmapImageSource => Glyph?.ImageSource;

    /// <summary>
    /// Gets whether this menu item has an embedded bitmap icon.
    /// </summary>
    public bool HasBitmap => Glyph?.HasData == true;

    public List<TMenuItem> SubItems { get; } = [];

    /// <summary>
    /// Gets the effective shortcut, considering linked action.
    /// </summary>
    public string EffectiveShortCut =>
        _shortCutExplicitlySet || LinkedAction == null
            ? ShortCut
            : (string.IsNullOrEmpty(LinkedAction.ShortCut) ? ShortCut : LinkedAction.ShortCut);

    /// <summary>
    /// Gets the effective image index, considering linked action.
    /// </summary>
    public int EffectiveImageIndex =>
        _imageIndexExplicitlySet || LinkedAction == null
            ? ImageIndex
            : (LinkedAction.ImageIndex >= 0 ? LinkedAction.ImageIndex : ImageIndex);

    public TMenuItem()
    {
        Width = 100;
        Height = 22;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                _imageIndexExplicitlySet = ImageIndex >= 0;
                break;
            case "shortcut":
                ShortCut = value?.ToString() ?? string.Empty;
                _shortCutExplicitlySet = !string.IsNullOrEmpty(ShortCut);
                break;
            case "checked":
                MenuItemChecked = ConvertToBool(value);
                break;
            case "radioitem":
                RadioItem = ConvertToBool(value);
                break;
            case "groupindex":
                GroupIndex = ConvertToInt(value);
                break;
            case "rightjustify":
                RightJustify = ConvertToBool(value);
                break;
            default:
                base.ApplyProperty(name, value);
                // Check if it's a separator
                if (name.ToLower() == "caption" && Caption == "-")
                {
                    IsSeparator = true;
                }
                break;
        }
    }

    protected override void OnActionLinked()
    {
        base.OnActionLinked();
        
        if (LinkedAction == null) return;

        // Inherit ShortCut if not explicitly set
        if (!_shortCutExplicitlySet && !string.IsNullOrEmpty(LinkedAction.ShortCut))
        {
            ShortCut = LinkedAction.ShortCut;
        }

        // Inherit ImageIndex if not explicitly set
        if (!_imageIndexExplicitlySet && LinkedAction.ImageIndex >= 0)
        {
            ImageIndex = LinkedAction.ImageIndex;
        }

        // Notify property changes for effective values
        OnPropertyChanged(nameof(EffectiveShortCut));
        OnPropertyChanged(nameof(EffectiveImageIndex));
        OnPropertyChanged(nameof(EffectiveCaption));
    }

    /// <summary>
    /// Gets the image for this menu item, either from embedded bitmap or from parent menu's ImageList.
    /// </summary>
    public ImageSource? GetEffectiveImage()
    {
        // First, check if there's an embedded bitmap
        if (HasBitmap)
            return BitmapImageSource;

        // Otherwise, try to get from parent menu's ImageList
        var effectiveIndex = EffectiveImageIndex;
        if (effectiveIndex < 0)
            return null;

        // Walk up the tree to find the MainMenu/PopupMenu
        var current = Parent;
        while (current != null)
        {
            if (current is TMainMenu mainMenu)
            {
                return mainMenu.GetMenuImage(effectiveIndex);
            }
            current = current.Parent;
        }

        return null;
    }
}
