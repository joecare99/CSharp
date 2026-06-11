using System.Collections.Generic;
using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using AA37_TreeView.Views;
using Avalonia.Headless.MSTest;

namespace AA37_TreeView.Tests.Views;

[TestClass]
public class MainWindowTests
{
    [AvaloniaTestMethod]
    public void MainWindowCanBeCreatedTest()
    {
        var window = new MainWindow
        {
            DataContext = new MainWindowViewModel(new BooksTreeViewModel(new TestBooksService())),
        };

        window.Show();

        Assert.IsNotNull(window);
        Assert.IsNotNull(window.Content);
    }

    [AvaloniaTestMethod]
    public void MainViewCanBeCreatedTest()
    {
        var view = new MainView
        {
            DataContext = new MainWindowViewModel(new BooksTreeViewModel(new TestBooksService())),
        };

        Assert.IsNotNull(view);
        Assert.IsNotNull(view.Content);
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
