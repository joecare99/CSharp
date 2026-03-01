using System.Windows;
using SharpHack.ViewModel;

namespace SharpHack.View.FirstPerson;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public MainWindow(FirstPersonGameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
