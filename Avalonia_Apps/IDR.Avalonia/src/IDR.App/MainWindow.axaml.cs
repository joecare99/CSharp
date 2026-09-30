using Avalonia.Controls;
using IDR.App.ViewModels;

namespace IDR.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
