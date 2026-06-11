using CommunityToolkit.Mvvm.ComponentModel;

namespace AA37_TreeView.ViewModels;

/// <summary>
/// Provides the root view model for desktop and browser hosts.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
        : this(new BooksTreeViewModel())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    /// <param name="booksTree">The tree view model.</param>
    public MainWindowViewModel(BooksTreeViewModel booksTree)
    {
        BooksTree = booksTree;
    }

    /// <summary>
    /// Gets the tree view model displayed by the window.
    /// </summary>
    public BooksTreeViewModel BooksTree { get; }
}
