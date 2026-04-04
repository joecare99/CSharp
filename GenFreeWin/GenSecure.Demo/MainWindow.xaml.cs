using System.Windows;
using GenSecure.Demo.ViewModels;

namespace GenSecure.Demo;

/// <summary>
/// Code-behind for <see cref="MainWindow"/>.
/// Handles PasswordBox interaction and log auto-scroll — concerns that
/// cannot be expressed via MVVM data binding alone.
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of <see cref="MainWindow"/> with the given view model.
    /// </summary>
    /// <param name="viewModel">The injected main view model.</param>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        // Auto-scroll the log TextBox whenever new log entries are appended.
        viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.LogText))
                _txtLog.ScrollToEnd();
        };
    }

    // ── PasswordBox bridge ────────────────────────────────────────────────────

    private void _btnCreateBackup_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.CreateRecoveryBackup(
            _pwdBackupPassphrase.Password,
            _pwdBackupConfirm.Password);

        _pwdBackupPassphrase.Clear();
        _pwdBackupConfirm.Clear();
    }

    private void _btnRestore_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.RestoreFromRecovery(_pwdRestorePassphrase.Password);
        _pwdRestorePassphrase.Clear();
    }
}
