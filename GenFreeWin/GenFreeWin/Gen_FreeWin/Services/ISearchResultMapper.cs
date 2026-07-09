// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="ISearchResultMapper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Interface for mapping search results to UI-bindable items</summary>
// ***********************************************************************

using Gen_FreeWin.ViewModels.Models;
using GenFree.Helper;
using System.Collections.Generic;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Maps SearchResult domain objects to ListItem UI-bindable representations.
    /// Encapsulates display logic for the name search result lists.
    /// </summary>
    public interface ISearchResultMapper
    {
        /// <summary>
        /// Converts a single SearchResult to a ListItem for UI binding.
        /// </summary>
        /// <param name="result">Domain model to convert.</param>
        /// <returns>ListItem suitable for ListBox/ComboBox binding.</returns>
        ListItem<int> MapToListItem(SearchResult result);

        /// <summary>
        /// Converts a collection of SearchResults to ListItems.
        /// </summary>
        /// <param name="results">Domain models to convert.</param>
        /// <returns>Collection of ListItems.</returns>
        IList<ListItem<int>> MapToListItems(IEnumerable<SearchResult> results);

        /// <summary>
        /// Maps a SearchResult to a complex ListItem with multiple identifiers (for composite results).
        /// </summary>
        /// <param name="result">Domain model to convert.</param>
        /// <returns>Complex ListItem with name and multiple IDs.</returns>
        ListItem<(int Id, int SecondaryId, short TertiaryId)> MapToComplexListItem(SearchResult result);

        /// <summary>
        /// Maps formatted display text from a SearchResult.
        /// Used for label/string binding in results display.
        /// </summary>
        /// <param name="result">Domain model to format.</param>
        /// <param name="format">Format specifier (e.g., "short", "full", "family").</param>
        /// <returns>Formatted display text.</returns>
        string MapToDisplayText(SearchResult result, string format = "full");

        /// <summary>
        /// Formats the result header/label for result lists.
        /// </summary>
        /// <param name="totalResults">Total number of results returned.</param>
        /// <param name="searchText">Original search text.</param>
        /// <returns>Formatted label text.</returns>
        string MapToResultsLabel(int totalResults, string searchText);
    }
}
