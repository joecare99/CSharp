// RepoMigrator.App.Wpf/MainWindow.xaml.cs
using System.Windows;
using RepoMigrator.App.Wpf.ViewModels;

namespace RepoMigrator.App.Wpf;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void SourcePasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is System.Windows.Controls.PasswordBox pb)
            vm.SourcePassword = pb.Password;
    }
}
