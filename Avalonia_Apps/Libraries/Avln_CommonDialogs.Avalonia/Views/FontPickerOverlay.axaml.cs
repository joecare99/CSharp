using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Avln_CommonDialogs.Avalonia.ViewModels;
using Avln_CommonDialogs.Base.Models;
using global::Avalonia.Interactivity;

namespace Avln_CommonDialogs.Avalonia.Views;

/// <summary>
/// Represents an overlay host for the font picker in single-view environments.
/// </summary>
public partial class FontPickerOverlay : UserControl
{
    private TaskCompletionSource<bool?>? _completionSource;
    private FontPickerViewModel? _viewModel;

    /// <summary>
    /// Gets the embedded font picker view.
    /// </summary>
    public FontPickerView EmbeddedPickerView
        => this.FindControl<FontPickerView>("PickerView")
            ?? this.GetVisualDescendants().OfType<FontPickerView>().FirstOrDefault()
            ?? throw new InvalidOperationException("The FontPickerOverlay is missing its embedded FontPickerView.");

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPickerOverlay"/> class.
    /// </summary>
    public FontPickerOverlay()
    {
        AvaloniaXamlLoader.Load(this);
        KeyDown += OnOverlayKeyDown;
    }

    /// <summary>
    /// Gets the accepted selection.
    /// </summary>
    public FontDialogSelection? AcceptedSelection { get; private set; }

    /// <summary>
    /// Shows the overlay for the specified view model.
    /// </summary>
    /// <param name="viewModel">The font picker view model.</param>
    /// <returns>The asynchronous dialog result.</returns>
    public Task<bool?> ShowAsync(FontPickerViewModel viewModel)
    {
        if (_completionSource is { Task.IsCompleted: false })
            return _completionSource.Task;

        _completionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _viewModel = viewModel;
        DataContext = viewModel;
        AcceptedSelection = null;
        IsVisible = true;
        Focus();

        EmbeddedPickerView.OkButton.Click += OnOkButtonClick;
        EmbeddedPickerView.CancelButton.Click += OnCancelButtonClick;

        return _completionSource.Task;
    }

    private void OnOkButtonClick(object? sender, RoutedEventArgs e)
    {
        if (_viewModel is null)
        {
            Close(false);
            return;
        }

        AcceptedSelection = _viewModel.CreateSelection();
        Close(true);
    }

    private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
        => Close(false);

    private void OnOverlayKeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsVisible)
            return;

        if (e.Key == Key.Escape)
        {
            Close(false);
            e.Handled = true;
        }
        else if (e.Key == Key.Enter && e.KeyModifiers == KeyModifiers.None)
        {
            if (_viewModel is not null)
                AcceptedSelection = _viewModel.CreateSelection();
            Close(true);
            e.Handled = true;
        }
    }

    private void Close(bool? result)
    {
        EmbeddedPickerView.OkButton.Click -= OnOkButtonClick;
        EmbeddedPickerView.CancelButton.Click -= OnCancelButtonClick;

        IsVisible = false;
        DataContext = null;
        _viewModel = null;
        _completionSource?.TrySetResult(result);
        _completionSource = null;
    }

    /// <summary>
    /// Finds a suitable panel host for overlay injection.
    /// </summary>
    /// <param name="owner">The owner control or top level.</param>
    /// <returns>The host panel when available.</returns>
    public static Panel? FindOverlayHost(object? owner)
    {
        if (owner is Panel panel)
            return panel;

        if (owner is TopLevel topLevel)
            return topLevel.GetVisualDescendants().OfType<Panel>().FirstOrDefault();

        if (owner is Control control)
            return control.GetVisualAncestors().OfType<Panel>().FirstOrDefault()
                ?? TopLevel.GetTopLevel(control)?.GetVisualDescendants().OfType<Panel>().FirstOrDefault();

        return null;
    }
}
