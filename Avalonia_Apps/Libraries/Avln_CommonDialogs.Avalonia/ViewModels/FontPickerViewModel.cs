using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Avln_CommonDialogs.Base.Models;

namespace Avln_CommonDialogs.Avalonia.ViewModels;

/// <summary>
/// Represents the state of the Avalonia font picker dialog.
/// </summary>
public partial class FontPickerViewModel : ObservableObject
{
    private const string DefaultPreviewText = "The quick brown fox jumps over the lazy dog. 1234567890";

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerViewModel"/> class.
    /// </summary>
    /// <param name="fontFamilies">The available font families.</param>
    /// <param name="selection">The initial selection.</param>
    /// <param name="previewText">The optional preview text.</param>
    public FontPickerViewModel(
        IReadOnlyList<FontFamily> fontFamilies,
        FontDialogSelection selection,
        string? previewText)
    {
        FontFamilies = fontFamilies;

        SelectedFontFamily = fontFamilies.FirstOrDefault(f => f.Name == selection.FamilyName)
            ?? fontFamilies.FirstOrDefault();
        FontSize = NormalizeFontSize(selection.Size);
        IsBold = selection.IsBold;
        IsItalic = selection.IsItalic;
        IsUnderline = selection.IsUnderline;
        IsStrikethrough = selection.IsStrikethrough;
        IsOverline = selection.IsOverline;
        SelectedColor = Color.FromUInt32(selection.ArgbColor);
        PreviewText = string.IsNullOrWhiteSpace(previewText) ? DefaultPreviewText : previewText;
    }

    /// <summary>
    /// Gets the available font families.
    /// </summary>
    public IReadOnlyList<FontFamily> FontFamilies { get; }

    /// <summary>
    /// Gets or sets the selected font family.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewFontFamily))]
    private FontFamily? selectedFontFamily;

    /// <summary>
    /// Gets or sets the selected font size.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewFontSize))]
    private decimal fontSize;

    /// <summary>
    /// Gets or sets a value indicating whether bold is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewFontWeight))]
    private bool isBold;

    /// <summary>
    /// Gets or sets a value indicating whether italic is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewFontStyle))]
    private bool isItalic;

    /// <summary>
    /// Gets or sets a value indicating whether underline is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTextDecorations))]
    private bool isUnderline;

    /// <summary>
    /// Gets or sets a value indicating whether strikethrough is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTextDecorations))]
    private bool isStrikethrough;

    /// <summary>
    /// Gets or sets a value indicating whether overline is selected.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTextDecorations))]
    private bool isOverline;

    /// <summary>
    /// Gets or sets the selected preview color.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewForeground))]
    private Color selectedColor;

    /// <summary>
    /// Gets or sets the preview text.
    /// </summary>
    [ObservableProperty]
    private string previewText = DefaultPreviewText;

    /// <summary>
    /// Gets the preview font family.
    /// </summary>
    public FontFamily PreviewFontFamily => SelectedFontFamily ?? FontFamilies.FirstOrDefault() ?? new FontFamily(string.Empty);

    /// <summary>
    /// Gets the preview font size.
    /// </summary>
    public double PreviewFontSize => (double)FontSize;

    /// <summary>
    /// Gets the preview font weight.
    /// </summary>
    public FontWeight PreviewFontWeight => IsBold ? FontWeight.Bold : FontWeight.Normal;

    /// <summary>
    /// Gets the preview font style.
    /// </summary>
    public FontStyle PreviewFontStyle => IsItalic ? FontStyle.Italic : FontStyle.Normal;

    /// <summary>
    /// Gets the preview foreground brush.
    /// </summary>
    public IBrush PreviewForeground => new SolidColorBrush(SelectedColor);

    /// <summary>
    /// Gets the preview text decorations.
    /// </summary>
    public TextDecorationCollection? PreviewTextDecorations => CreateTextDecorations(CreateSelection());

    /// <summary>
    /// Creates a UI-agnostic selection from the current view model state.
    /// </summary>
    /// <returns>The current font dialog selection.</returns>
    public FontDialogSelection CreateSelection()
        => new()
        {
            FamilyName = SelectedFontFamily?.Name,
            Size = (double)FontSize,
            IsBold = IsBold,
            IsItalic = IsItalic,
            IsUnderline = IsUnderline,
            IsStrikethrough = IsStrikethrough,
            IsOverline = IsOverline,
            ArgbColor = SelectedColor.ToUInt32()
        };

    private static decimal NormalizeFontSize(double fontSize)
    {
        var normalizedSize = fontSize <= 0d ? 12d : fontSize;
        return decimal.Round((decimal)normalizedSize, 2);
    }

    private static TextDecorationCollection? CreateTextDecorations(FontDialogSelection selection)
    {
        var decorations = new List<TextDecoration>();

        if (selection.IsUnderline)
            decorations.AddRange(TextDecorations.Underline);

        if (selection.IsStrikethrough)
            decorations.AddRange(TextDecorations.Strikethrough);

        if (selection.IsOverline)
            decorations.AddRange(TextDecorations.Overline);

        return decorations.Count == 0 ? null : new TextDecorationCollection(decorations);
    }
}
