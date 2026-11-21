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

    /// <summary>Gets or sets optional host-specific content displayed above the dialog actions.</summary>
    public Control? HostContent
    {
        get => HostContentPresenter.Content as Control;
        set => HostContentPresenter.Content = value;
    }

    private async void OnOkClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not ConfigUiViewModel viewModel || viewModel.IsBusy || _discardingOnClose)
        {
            return;
        }

        try
        {
            if (await viewModel.ApplyChangesAsync())
            {
                _allowClose = true;
                Close();
            }
        }
        catch (OperationCanceledException)
        {
            // The view model reports cancellation; the dialog must remain open.
        }
    }

    private async void OnApplyClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is ConfigUiViewModel { IsBusy: false } viewModel && !_discardingOnClose)
        {
            try
            {
                await viewModel.ApplyChangesAsync();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    private async void OnCancelClick(object? sender, RoutedEventArgs eventArgs)
    {
        if (_discardingOnClose || DataContext is ConfigUiViewModel { IsBusy: true })
        {
            return;
        }

        try
        {
            if (DataContext is not ConfigUiViewModel viewModel || await viewModel.DiscardChangesAsync())
            {
                _allowClose = true;
                Close();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    protected override async void OnClosing(WindowClosingEventArgs eventArgs)
    {
        if (!_allowClose && (_discardingOnClose || DataContext is ConfigUiViewModel { IsBusy: true }))
        {
            eventArgs.Cancel = true;
        }
        else if (!_allowClose && DataContext is ConfigUiViewModel viewModel && viewModel.HasUnsavedChanges)
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
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _discardingOnClose = false;
            }
        }

        base.OnClosing(eventArgs);
    }
}
