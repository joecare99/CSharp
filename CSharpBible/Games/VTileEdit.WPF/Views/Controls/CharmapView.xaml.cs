using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VTileEdit.WPF.Views.Controls;

/// <summary>
/// Presents a simple character map for assigning glyph values.
/// </summary>
public partial class CharmapView : UserControl
{
    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(CharmapView));

    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(CharmapView));

    /// <summary>
    /// Initializes a new instance of the <see cref="CharmapView"/> class.
    /// </summary>
    public CharmapView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the character collection rendered in the map.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked when a character is selected.
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
