using Avalonia.Controls;
using Avalonia;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Config.UI.Avalonia.ViewModels;
using System;
using System.Threading.Tasks;

namespace Config.UI.Avalonia.Views;

/// <summary>
/// Displays reusable configuration section navigation and editing.
/// </summary>
public partial class ConfigUiView : UserControl
{
    public static readonly StyledProperty<bool> ShowSectionActionsProperty =
        AvaloniaProperty.Register<ConfigUiView, bool>(nameof(ShowSectionActions), true);

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigUiView"/> class.
    /// </summary>
    public ConfigUiView()
    {
        InitializeComponent();
        SectionActionsPanel.IsVisible = ShowSectionActions;
    }

    public bool ShowSectionActions
    {
        get => GetValue(ShowSectionActionsProperty);
        set
        {
            SetValue(ShowSectionActionsProperty, value);
            if (SectionActionsPanel is not null)
            {
                SectionActionsPanel.IsVisible = value;
            }
        }
    }

    private async void OnLoadClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiCommandAsync((DataContext as ConfigUiViewModel)?.LoadCommand);

    private async void OnResetClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiCommandAsync((DataContext as ConfigUiViewModel)?.ResetCommand);

    private async void OnSaveClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiCommandAsync((DataContext as ConfigUiViewModel)?.SaveCommand);

    private async void OnRetryClick(object? sender, RoutedEventArgs e) =>
        await ExecuteUiCommandAsync((DataContext as ConfigUiViewModel)?.RetryFailedReloadsCommand);

    private static async Task ExecuteUiCommandAsync(IAsyncRelayCommand? command)
    {
        if (command?.CanExecute(null) != true)
        {
            return;
        }

        try
        {
            await command.ExecuteAsync(null);
        }
        catch (OperationCanceledException)
        {
            // The view model retains the cancellation state for the user.
        }
    }
}
