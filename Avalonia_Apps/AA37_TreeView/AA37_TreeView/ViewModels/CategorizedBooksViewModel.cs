using System.Collections.ObjectModel;
using AA37_TreeView.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA37_TreeView.ViewModels;

/// <summary>
/// Represents a category node or book node in the tree.
/// </summary>
public partial class CategorizedBooksViewModel : ObservableObject
{
    /// <summary>
    /// Gets or sets the node caption.
    /// </summary>
    [ObservableProperty]
    private string _category = string.Empty;

    /// <summary>
    /// Gets or sets the represented book for leaf nodes.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBook))]
    private Book? _book;

    /// <summary>
    /// Gets or sets the child nodes.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<CategorizedBooksViewModel> _books = [];

    /// <summary>
    /// Gets a value indicating whether the node represents a book leaf.
    /// </summary>
    public bool HasBook => Book is not null;
}
