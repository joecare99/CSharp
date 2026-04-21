using System.Windows;
using System.Windows.Controls;
using RepoMigrator.App.Wpf.ViewModels;

namespace RepoMigrator.App.Wpf.Views;

/// <summary>
/// Displays the target endpoint setup controls and keeps the target password box synchronized with the view model.
/// </summary>
public partial class TargetEndpointSetupView : UserControl
{
    private bool _xSynchronizingPassword;

    public TargetEndpointSetupView() => InitializeComponent();

    private void UserControlLoaded(object sender, RoutedEventArgs e)
        => SynchronizePasswordFromViewModel();

    private void UserControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => SynchronizePasswordFromViewModel();

    private void TargetPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_xSynchronizingPassword || DataContext is not MainViewModel vm)
            return;

        vm.TargetPassword = edtTargetPassword.Password;
    }

    private void SynchronizePasswordFromViewModel()
    {
        if (DataContext is not MainViewModel vm)
            return;

        var sPassword = vm.TargetPassword ?? string.Empty;
        if (edtTargetPassword.Password == sPassword)
            return;

        _xSynchronizingPassword = true;
        edtTargetPassword.Password = sPassword;
        _xSynchronizingPassword = false;
    }
}
