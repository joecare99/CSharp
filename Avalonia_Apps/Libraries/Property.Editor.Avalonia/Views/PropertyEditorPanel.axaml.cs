using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Property.Editor.Avalonia.Views;

/// <summary>
/// Displays reusable editors for neutral property items.
/// </summary>
public partial class PropertyEditorPanel : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyEditorPanel"/> class.
    /// </summary>
    public PropertyEditorPanel()
    {
        InitializeComponent();
    }

    private void OnEditorGotFocus(object? sender, RoutedEventArgs eventArgs)
    {
        if (sender is Control { DataContext: PropertyEditorItemViewModel item }
            && DataContext is PropertyEditorViewModel viewModel)
        {
            viewModel.NotifyEditStarted(item.PropertyItem);
        }
    }

    private void OnEditorLostFocus(object? sender, RoutedEventArgs eventArgs)
    {
        NotifyEditCompleted(sender);
    }

    private void OnOptionSelectionChanged(object? sender, SelectionChangedEventArgs eventArgs)
    {
        if (eventArgs.AddedItems.Count > 0)
        {
            NotifyEditCompleted(sender);
        }
    }

    private void NotifyEditCompleted(object? sender)
    {
        if (sender is Control { DataContext: PropertyEditorItemViewModel item }
            && DataContext is PropertyEditorViewModel viewModel)
        {
            viewModel.NotifyEditCompleted(item.PropertyItem);
        }
    }
}
