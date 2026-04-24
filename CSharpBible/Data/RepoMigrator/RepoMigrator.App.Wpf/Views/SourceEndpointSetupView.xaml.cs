using System.Windows;
using System.Windows.Controls;
using RepoMigrator.App.Wpf.ViewModels;

namespace RepoMigrator.App.Wpf.Views;

/// <summary>
/// Displays the source endpoint setup controls and keeps the source password box synchronized with the view model.
/// </summary>
public partial class SourceEndpointSetupView : UserControl
{
    private bool _xSynchronizingPassword;

    public SourceEndpointSetupView() => InitializeComponent();

    private void UserControlLoaded(object sender, RoutedEventArgs e)
        => SynchronizePasswordFromViewModel();

    private void UserControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => SynchronizePasswordFromViewModel();

    private void SourcePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_xSynchronizingPassword || DataContext is not MainViewModel vm)
            return;

        vm.SourcePassword = edtSourcePassword.Password;
    }

    private void SynchronizePasswordFromViewModel()
    {
        if (DataContext is not MainViewModel vm)
            return;

        var sPassword = vm.SourcePassword ?? string.Empty;
        if (edtSourcePassword.Password == sPassword)
            return;

        _xSynchronizingPassword = true;
        edtSourcePassword.Password = sPassword;
        _xSynchronizingPassword = false;
    }
}
