using System;
using System.Collections.ObjectModel;
using System.Linq;
using AA37_TreeView.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA37_TreeView.ViewModels;

/// <summary>
/// Provides the grouped book tree and the currently selected book.
/// </summary>
public partial class BooksTreeViewModel : ObservableObject
{
    /// <summary>
    /// Gets or sets the default service factory used by the parameterless constructor.
    /// </summary>
    public static Func<IBooksService> CreateService { get; set; } = static () => new BooksService();

    private readonly IBooksService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="BooksTreeViewModel"/> class.
    /// </summary>
    public BooksTreeViewModel()
        : this(CreateService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BooksTreeViewModel"/> class.
    /// </summary>
    /// <param name="service">The book service.</param>
    public BooksTreeViewModel(IBooksService service)
    {
        _service = service;

        foreach (var groupedBooks in _service.GetBooks().GroupBy(book => book.Category))
        {
            Books.Add(new CategorizedBooksViewModel
            {
                Category = groupedBooks.Key,
                Books = new ObservableCollection<CategorizedBooksViewModel>(
                    groupedBooks.Select(book => new CategorizedBooksViewModel
                    {
                        Category = $"{book.Title} - {book.Author}",
                        Book = book,
                    })),
            });
        }
    }

    /// <summary>
    /// Gets or sets the root tree nodes.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<CategorizedBooksViewModel> _books = [];

    /// <summary>
    /// Gets or sets the selected tree node.
    /// </summary>
    [ObservableProperty]
    private CategorizedBooksViewModel? _selectedNode;

    /// <summary>
    /// Gets or sets the selected book.
    /// </summary>
    [ObservableProperty]
    private Book? _selectedBook;

    partial void OnSelectedNodeChanged(CategorizedBooksViewModel? value)
        => SelectedBook = value?.Book;
}
