using System.Windows;
using PictureDB.UI.ViewModels;

namespace PictureDB.UI;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        vm.onShowDialog = dlg => dlg.ShowDialog(this.Parent as Window);
    }
}
