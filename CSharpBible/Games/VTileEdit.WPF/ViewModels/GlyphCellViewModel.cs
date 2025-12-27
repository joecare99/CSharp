using System;
using System.Windows.Media;
using VTileEdit.WPF.Helpers;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Represents a single glyph inside a tile.
/// </summary>
public partial class GlyphCellViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GlyphCellViewModel"/> class.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    /// <param name="characterValue">The initial character.</param>
    /// <param name="foregroundColor">The initial foreground color.</param>
    /// <param name="backgroundColor">The initial background color.</param>
    public GlyphCellViewModel(int row, int column, char characterValue, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {
        Row = row;
        Column = column;
        character = characterValue;
        foreground = foregroundColor;
        background = backgroundColor;
    }

    /// <summary>
    /// Gets the visual row index.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the visual column index.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the brush representing the current foreground color.
    /// </summary>
    public Brush ForegroundBrush => ConsoleColorBrushCache.GetBrush(Foreground);

    /// <summary>
    /// Gets the brush representing the current background color.
    /// </summary>
    public Brush BackgroundBrush => ConsoleColorBrushCache.GetBrush(Background);

    /// <summary>
    /// Gets or sets the textual representation of <see cref="Character"/> for bindings.
    /// </summary>
    public string CharacterText
    {
        get => Character.ToString();
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            Character = value[0];
        }
    }

    [ObservableProperty]
    private char character;

    [ObservableProperty]
    private ConsoleColor foreground;

    [ObservableProperty]
    private ConsoleColor background;

    [ObservableProperty]
    private bool isSelected;

    partial void OnForegroundChanged(ConsoleColor value) => OnPropertyChanged(nameof(ForegroundBrush));

    partial void OnBackgroundChanged(ConsoleColor value) => OnPropertyChanged(nameof(BackgroundBrush));

    partial void OnCharacterChanged(char value) => OnPropertyChanged(nameof(CharacterText));

    /// <summary>
    /// Applies a new foreground color.
    /// </summary>
    /// <param name="color">The color that should be used.</param>
    public void SetForeground(ConsoleColor color) => Foreground = color;

    /// <summary>
    /// Applies a new background color.
    /// </summary>
    /// <param name="color">The color that should be used.</param>
    public void SetBackground(ConsoleColor color) => Background = color;

    /// <summary>
    /// Ensures the selection state matches the given flag.
    /// </summary>
    /// <param name="selected">True when the cell is selected.</param>
    public void SetSelection(bool selected) => IsSelected = selected;
}
