using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views;

public partial class Frm_W2kMain : Window
{
    public Frm_W2kMain(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

