using Avalonia.Controls;
using Avalonia.Interactivity;
using AA98_AvlnCodeStudio.Config.Secrets;
using AA98_AvlnCodeStudio.UI.ViewModels;
using System;

namespace AA98_AvlnCodeStudio.UI.Views;

public partial class AICredentialsDialog : Window
{
    public AICredentialsDialog()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs eventArgs)
    {
        base.OnOpened(eventArgs);
        var screen = Screens.ScreenFromWindow(this);
        if (screen is not null && screen.Scaling > 0)
        {
            double availableHeight = screen.WorkingArea.Height / screen.Scaling - 32;
            if (availableHeight > 0)
            {
                MaxHeight = Math.Min(MaxHeight, availableHeight);
            }
        }
    }

    protected override void OnClosed(EventArgs eventArgs)
    {
        VaultPassphrase.Text = string.Empty;
        VaultPassphraseConfirmation.Text = string.Empty;
        VaultUnlockPassphrase.Text = string.Empty;
        base.OnClosed(eventArgs);
    }

    private void OnCloseClick(object? sender, RoutedEventArgs eventArgs) => Close();

    private async void OnInitializeVaultClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        using CredentialSecret passphrase = new((VaultPassphrase.Text ?? string.Empty).AsSpan());
        using CredentialSecret confirmation = new((VaultPassphraseConfirmation.Text ?? string.Empty).AsSpan());
        VaultPassphrase.Text = string.Empty;
        VaultPassphraseConfirmation.Text = string.Empty;
        await viewModel.InitializeEncryptedVaultAsync(passphrase, confirmation);
    }

    private async void OnUnlockVaultClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        using CredentialSecret passphrase = new((VaultUnlockPassphrase.Text ?? string.Empty).AsSpan());
        VaultUnlockPassphrase.Text = string.Empty;
        await viewModel.UnlockEncryptedVaultAsync(passphrase);
    }

    private async void OnLockVaultClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.LockEncryptedVaultAsync();
        }
    }

    protected override void OnClosing(WindowClosingEventArgs eventArgs)
    {
        if (DataContext is MainWindowViewModel { CredentialOperationBusy: true })
        {
            eventArgs.Cancel = true;
        }

        base.OnClosing(eventArgs);
    }
}
