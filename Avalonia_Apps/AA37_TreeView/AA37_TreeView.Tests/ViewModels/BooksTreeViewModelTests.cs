using System.Collections.Generic;
using System.Linq;
using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using NSubstitute;

namespace AA37_TreeView.Tests.ViewModels;

[TestClass]
public class BooksTreeViewModelTests
{
    [TestMethod]
    public void ConstructorGroupsBooksByCategoryTest()
    {
        var service = Substitute.For<IBooksService>();
        service.GetBooks().Returns(
        [
            new Book("Book 1", "Author 1", BookCategories.Fantasy, [5]),
            new Book("Book 2", "Author 2", BookCategories.Fantasy, [4]),
            new Book("Book 3", "Author 3", BookCategories.Documentary, [3]),
        ]);

        var viewModel = new BooksTreeViewModel(service);

        Assert.AreEqual(2, viewModel.Books.Count);
        Assert.AreEqual(BookCategories.Fantasy, viewModel.Books[0].Category);
        Assert.AreEqual(2, viewModel.Books[0].Books.Count);
        Assert.AreEqual(BookCategories.Documentary, viewModel.Books[1].Category);
    }

    [TestMethod]
    public void SelectedNodeUpdatesSelectedBookTest()
    {
        var viewModel = new BooksTreeViewModel(SubstituteBooksService());
        var selectedNode = viewModel.Books.SelectMany(group => group.Books).First();

        viewModel.SelectedNode = selectedNode;

        Assert.AreSame(selectedNode.Book, viewModel.SelectedBook);
    }

    [TestMethod]
    public void ParameterlessConstructorUsesConfigurableFactoryTest()
    {
        var originalFactory = BooksTreeViewModel.CreateService;
        var service = SubstituteBooksService();

        try
        {
            BooksTreeViewModel.CreateService = () => service;

            var viewModel = new BooksTreeViewModel();

            Assert.AreEqual(2, viewModel.Books.Count);
        }
        finally
        {
            BooksTreeViewModel.CreateService = originalFactory;
        }
    }

    private static IBooksService SubstituteBooksService()
    {
        var service = Substitute.For<IBooksService>();
        service.GetBooks().Returns(CreateBooks());
        return service;
    }

    private static IEnumerable<Book> CreateBooks()
        =>
        [
            new Book("Book 1", "Author 1", BookCategories.Fantasy, [5]),
            new Book("Book 2", "Author 2", BookCategories.Documentary, [4]),
        ];
}
