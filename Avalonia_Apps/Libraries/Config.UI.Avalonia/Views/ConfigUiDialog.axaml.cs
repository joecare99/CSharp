using Avalonia.Controls;
using Avalonia.Interactivity;
using Config.UI.Avalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace Config.UI.Avalonia.Views;

/// <summary>
/// Hosts configuration sections with explicit apply and discard boundaries.
/// </summary>
public partial class ConfigUiDialog : Window
{
    private bool _allowClose;
    private bool _discardingOnClose;

    public ConfigUiDialog()
    {
        InitializeComponent();
    }

    public ConfigUiDialog(ConfigUiViewModel viewModel)
        : this()
    {
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    private async void OnOkClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not ConfigUiViewModel viewModel)
        {
            return;
        }

        if (await viewModel.ApplyChangesAsync())
        {
            _allowClose = true;
            Close();
        }
    }

    private async void OnApplyClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is ConfigUiViewModel viewModel)
        {
            await viewModel.ApplyChangesAsync();
        }
    }

    private async void OnCancelClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not ConfigUiViewModel viewModel || await viewModel.DiscardChangesAsync())
        {
            _allowClose = true;
            Close();
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs eventArgs)
    {
        if (!_allowClose && !_discardingOnClose && DataContext is ConfigUiViewModel viewModel
            && viewModel.HasUnsavedChanges)
        {
            eventArgs.Cancel = true;
            _discardingOnClose = true;
            try
            {
                if (await viewModel.DiscardChangesAsync())
                {
                    _allowClose = true;
                    Close();
                }
            }
            finally
            {
                _discardingOnClose = false;
            }
        }

        base.OnClosing(eventArgs);
    }
}
