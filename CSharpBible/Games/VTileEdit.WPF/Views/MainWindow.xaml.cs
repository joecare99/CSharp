using System.Windows;
using VTileEdit.WPF.ViewModels;

namespace VTileEdit.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext ??= new MainWindowViewModel();
    }
}
