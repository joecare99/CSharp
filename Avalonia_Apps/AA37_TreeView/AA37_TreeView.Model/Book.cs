using System.Collections.Generic;

namespace AA37_TreeView.Model;

/// <summary>
/// Represents a book entry shown in the tree view sample.
/// </summary>
/// <param name="Title">The book title.</param>
/// <param name="Author">The author name.</param>
/// <param name="Category">The category used for grouping.</param>
/// <param name="Ratings">The available rating values.</param>
public sealed record Book(string Title, string Author, string Category, IEnumerable<int> Ratings);
