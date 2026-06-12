using AA37_TreeView.ViewModels;
using Avalonia.Controls;

namespace AA37_TreeView.Views;

/// <summary>
/// Displays the grouped book tree and the selected book details.
/// </summary>
public partial class BooksTreeView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooksTreeView"/> class.
    /// </summary>
    public BooksTreeView()
    {
        InitializeComponent();
    }

    private void BooksTree_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not BooksTreeViewModel viewModel || sender is not TreeView treeView)
        {
            return;
        }

        viewModel.SelectedNode = treeView.SelectedItem as CategorizedBooksViewModel;
    }
}
