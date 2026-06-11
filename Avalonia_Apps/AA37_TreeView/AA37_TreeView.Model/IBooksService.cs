using System.Collections.Generic;

namespace AA37_TreeView.Model;

/// <summary>
/// Provides the books displayed by the sample application.
/// </summary>
public interface IBooksService
{
    /// <summary>
    /// Gets the available books.
    /// </summary>
    IEnumerable<Book> GetBooks();
}
