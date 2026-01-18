using SharpHack.WPF2D.ViewModels;
using System.Windows;

namespace SharpHack.WPF2D;

/// <summary>
/// Interaction logic for MainWindow.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="vm">The view model.</param>
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
