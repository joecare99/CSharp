using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VTileEdit.WPF.Converters;

namespace VTileEdit.WPF.Views.Controls;

/// <summary>
/// Displays a palette of color swatches that can be reused for foreground and background selection.
/// </summary>
public partial class PaletteSelectorView : UserControl
{
    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(PaletteSelectorView));

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(PaletteSelectorView));

    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(PaletteSelectorView));

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
        nameof(SelectionMode),
        typeof(PaletteSelectionMode),
        typeof(PaletteSelectorView),
        new PropertyMetadata(PaletteSelectionMode.Foreground));

    /// <summary>
    /// Initializes a new instance of the <see cref="PaletteSelectorView"/> class.
    /// </summary>
    public PaletteSelectorView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the header displayed above the palette.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the palette items.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the command executed when a swatch is clicked.
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the selector mode used for highlighting badges.
    /// </summary>
    public PaletteSelectionMode SelectionMode
    {
        get => (PaletteSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }
}
