using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TBitBtn component (button with bitmap/glyph).
/// </summary>
public partial class TBitBtn : TButton
{
    [ObservableProperty]
    private BitBtnKind _kind = BitBtnKind.Custom;

    [ObservableProperty]
    private ButtonLayout _layout = ButtonLayout.Left;

    [ObservableProperty]
    private int _spacing = 4;

    [ObservableProperty]
    private int _margin = -1;

    [ObservableProperty]
    private int _numGlyphs = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EffectiveImageIndex))]
    private int _imageIndex = -1;

    /// <summary>
    /// Indicates whether ImageIndex was explicitly set.
    /// </summary>
    private bool _imageIndexExplicitlySet;

    /// <summary>
    /// Gets the ImageSource for the glyph, derived from the Glyph property.
    /// </summary>
    public ImageSource? GlyphImageSource => Glyph?.ImageSource;

    /// <summary>
    /// Gets whether this button has a custom glyph image.
    /// </summary>
    public bool HasGlyph => Glyph?.HasData == true;

    /// <summary>
    /// Gets the effective image index, considering linked action.
    /// </summary>
    public int EffectiveImageIndex =>
        _imageIndexExplicitlySet || LinkedAction == null || !LinkedAction.TryGetTarget(out var t)
            ? ImageIndex
            : (t.ImageIndex >= 0 ? t.ImageIndex : ImageIndex);

    public TBitBtn()
    {
        Height = 30;
        Width = 75;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "kind":
                Kind = ParseKind(value?.ToString());
                break;
            case "layout":
                Layout = ParseLayout(value?.ToString());
                break;
            case "spacing":
                Spacing = ConvertToInt(value, 4);
                break;
            case "margin":
                Margin = ConvertToInt(value, -1);
                break;
            case "numglyphs":
                NumGlyphs = ConvertToInt(value, 1);
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                _imageIndexExplicitlySet = ImageIndex >= 0;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    protected override void OnActionChanged(TAction? reference)
    {
        base.OnActionChanged(reference);

        if (reference == null) return;

        // Inherit ImageIndex if not explicitly set
        if (!_imageIndexExplicitlySet && reference.ImageIndex >= 0)
        {
            ImageIndex = reference.ImageIndex;
        }
    }

    private static BitBtnKind ParseKind(string? value) => value?.ToLower() switch
    {
        "bkcustom" => BitBtnKind.Custom,
        "bkok" => BitBtnKind.OK,
        "bkcancel" => BitBtnKind.Cancel,
        "bkhelp" => BitBtnKind.Help,
        "bkyes" => BitBtnKind.Yes,
        "bkno" => BitBtnKind.No,
        "bkclose" => BitBtnKind.Close,
        "bkabort" => BitBtnKind.Abort,
        "bkretry" => BitBtnKind.Retry,
        "bkignore" => BitBtnKind.Ignore,
        "bkall" => BitBtnKind.All,
        _ => BitBtnKind.Custom
    };

    private static ButtonLayout ParseLayout(string? value) => value?.ToLower() switch
    {
        "blleft" => ButtonLayout.Left,
        "blright" => ButtonLayout.Right,
        "bltop" => ButtonLayout.Top,
        "blbottom" => ButtonLayout.Bottom,
        _ => ButtonLayout.Left
    };
}
