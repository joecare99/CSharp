using System.Linq;
using AA37_TreeView.Model;

namespace AA37_TreeView.Tests.Services;

[TestClass]
public class BooksServiceTests
{
    [TestMethod]
    public void GetBooksReturnsSeedDataTest()
    {
        var service = new BooksService();

        var books = service.GetBooks().ToArray();

        Assert.AreEqual(10, books.Length);
        Assert.IsTrue(books.Any(book => book.Title == "Drachenfeuer"));
    }

    [TestMethod]
    [DataRow(BookCategories.SoftwareDevelopment)]
    [DataRow(BookCategories.Fantasy)]
    [DataRow(BookCategories.Documentary)]
    public void GetBooksContainsExpectedCategoriesTest(string category)
    {
        var service = new BooksService();

        var books = service.GetBooks().ToArray();

        Assert.IsTrue(books.Any(book => book.Category == category));
    }
}
