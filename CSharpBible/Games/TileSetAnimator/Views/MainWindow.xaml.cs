using System.Windows;
using TileSetAnimator.ViewModels;

namespace TileSetAnimator.Views;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
