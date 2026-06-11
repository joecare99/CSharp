using System.Collections.Generic;

namespace AA37_TreeView.Model;

/// <summary>
/// Provides the in-memory sample book data.
/// </summary>
public sealed class BooksService : IBooksService
{
    private static readonly Book[] _books =
    [
        new("Schrödinger programmiert c#", "Bernhard Wurm", BookCategories.SoftwareDevelopment, [5, 5, 4, 5, 4, 4, 5, 4, 5, 4, 4, 3, 4]),
        new("Patterns of Enterprise Application Architecture", "Marin Fowler", BookCategories.SoftwareDevelopment, [3, 4, 5, 2, 5, 4, 5, 5, 1, 5, 3]),
        new("Drachenfeuer", "Wolfgang & Heike Hohlbein", BookCategories.Fantasy, [2, 3, 5, 2, 1, 4, 3, 3, 2, 4, 3, 4]),
        new("Der pragmatische Programmierer", "Andrew Hunt", BookCategories.SoftwareDevelopment, [4, 5, 5, 1, 5, 3, 3, 4, 5, 2, 5]),
        new("Programmieren Lernen", "Bernhard Wurm", BookCategories.SoftwareDevelopment, [5, 4, 5, 4, 4, 3, 4, 5, 5, 4, 5, 4, 4]),
        new("Herr der Ringe", "John R. Tolkin", BookCategories.Fantasy, [3, 2, 4, 3, 4, 2, 3, 5, 2, 1, 4, 3]),
        new("Limit", "Frank Schätzing", BookCategories.Fantasy, [4, 3, 4, 5, 4, 5, 4, 5, 4, 1, 4, 5, 5, 2]),
        new("Per Anhalter durch die Galaxis", "Douglas Adams", BookCategories.Fantasy, [4, 3, 4, 5, 4, 5, 4, 5, 4, 1, 4, 5, 5, 2]),
        new("Ender's Game", "Orson Scott Card", BookCategories.Fantasy, [4, 3, 4, 5, 4, 5, 4, 5, 4, 1, 4, 5, 5, 2]),
        new("Kuckucksei", "Clifford Stoll", BookCategories.Documentary, [5, 5, 5, 5, 4, 5, 4, 5, 4, 3, 4, 5, 5, 2]),
    ];

    /// <inheritdoc />
    public IEnumerable<Book> GetBooks() => _books;
}
