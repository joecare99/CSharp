using System.Collections.Generic;
using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using AA37_TreeView.Views;
using Avalonia.Controls;
using Avalonia.Headless.MSTest;

namespace AA37_TreeView.Tests.Views;

[TestClass]
public class BooksTreeViewTests
{
    [AvaloniaTestMethod]
    public void BooksTreeViewCanBeCreatedTest()
    {
        var view = new BooksTreeView
        {
            DataContext = new BooksTreeViewModel(new TestBooksService()),
        };

        Assert.IsNotNull(view);
        Assert.IsNotNull(view.DataContext);
    }

    [AvaloniaTestMethod]
    public void BooksTreeViewCanBeHostedInWindowTest()
    {
        var window = new Window
        {
            Content = new BooksTreeView
            {
                DataContext = new BooksTreeViewModel(new TestBooksService()),
            },
        };

        window.Show();

        Assert.IsNotNull(window.Content);
    }

    private sealed class TestBooksService : IBooksService
    {
        public IEnumerable<Book> GetBooks()
            =>
            [
                new Book("Book 1", "Author 1", BookCategories.Fantasy, [5]),
            ];
    }
}
