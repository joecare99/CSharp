using System.Linq;
using AA37_TreeView.Model;

namespace AA37_TreeView.Tests.Models;

[TestClass]
public class BookTests
{
    [TestMethod]
    public void BookStoresConstructorValuesTest()
    {
        var book = new Book("Title", "Author", BookCategories.Documentary, [1, 2, 3]);

        Assert.AreEqual("Title", book.Title);
        Assert.AreEqual("Author", book.Author);
        Assert.AreEqual(BookCategories.Documentary, book.Category);
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, book.Ratings.ToArray());
    }
}
