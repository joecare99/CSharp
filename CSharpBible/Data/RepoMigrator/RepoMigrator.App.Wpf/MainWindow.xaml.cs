// RepoMigrator.App.Wpf/MainWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using RepoMigrator.App.Wpf.ViewModels;

namespace RepoMigrator.App.Wpf;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        if (FindName("SourcePasswordBox") is PasswordBox sourcePasswordBox)
            sourcePasswordBox.Password = vm.SourcePassword ?? string.Empty;

        if (FindName("TargetPasswordBox") is PasswordBox targetPasswordBox)
            targetPasswordBox.Password = vm.TargetPassword ?? string.Empty;
    }

    private void SourcePasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is System.Windows.Controls.PasswordBox pb)
            vm.SourcePassword = pb.Password;
    }

    private void TargetPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm && sender is System.Windows.Controls.PasswordBox pb)
            vm.TargetPassword = pb.Password;
    }
}
