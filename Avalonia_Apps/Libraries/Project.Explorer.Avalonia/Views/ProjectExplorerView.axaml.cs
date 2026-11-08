using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Project.Explorer.Avalonia.ViewModels;
using System.Linq;

namespace Project.Explorer.Avalonia.Views;

/// <summary>Hosts the reusable project explorer bindings.</summary>
public partial class ProjectExplorerView : UserControl
{
    /// <summary>Initializes the reusable explorer view.</summary>
    public ProjectExplorerView()
    {
        InitializeComponent();
    }

    private async void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not ProjectExplorerViewModel viewModel || e.Source is not global::Avalonia.Visual source)
        {
            return;
        }

        TreeViewItem? item = source as TreeViewItem ?? source.GetVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
        if (item?.DataContext is ProjectExplorerItemViewModel selected
            && ReferenceEquals(selected, viewModel.SelectedItem))
        {
            await viewModel.OpenAsync(selected);
        }
    }

    private async void OnTreeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ProjectExplorerViewModel { SelectedItem: { } selected } viewModel)
        {
            e.Handled = true;
            await viewModel.OpenAsync(selected);
        }
    }
}
