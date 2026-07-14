// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchResultMapper.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation for mapping search results to UI representations</summary>
// ***********************************************************************

using Gen_FreeWin.ViewModels.Models;
using GenFree.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Maps SearchResult domain objects to ListItem UI representations.
    /// Handles formatting, localization, and display logic for result rendering.
    /// </summary>
    public class SearchResultMapper : ISearchResultMapper
    {
        /// <summary>
        /// Converts a single SearchResult to a ListItem for UI binding.
        /// Display format: "Name (Date, Year, ID)"
        /// </summary>
        public ListItem<int> MapToListItem(SearchResult result)
        {
            if (result == null)
                return new ListItem<int>("", -1);

            var displayText = result.GetDisplayText(false);
            return new ListItem<int>(displayText, result.Id);
        }

        /// <summary>
        /// Converts multiple SearchResults to ListItems.
        /// </summary>
        public IList<ListItem<int>> MapToListItems(IEnumerable<SearchResult> results)
        {
            if (results == null)
                return new List<ListItem<int>>();

            return results
                .Where(r => r != null)
                .Select(MapToListItem)
                .ToList();
        }

        /// <summary>
        /// Maps a SearchResult to a complex ListItem with multiple IDs.
        /// Used for composite result lists (e.g., with family/event IDs).
        /// </summary>
        public ListItem<(int Id, int SecondaryId, short TertiaryId)> MapToComplexListItem(SearchResult result)
        {
            if (result == null)
                return new ListItem<(int, int, short)>("", (-1, -1, 0));

            var displayText = result.GetDisplayText(includeAdditional: true);
            var data = (result.Id, result.SecondaryId, result.TertiaryId);
            return new ListItem<(int, int, short)>(displayText, data);
        }

        /// <summary>
        /// Formats result display text based on format specifier.
        /// Formats:
        ///   - "short": "Name (Year)"
        ///   - "full": "Name (Date Year ID) [Additional]"
        ///   - "family": "Name (Family ID)"
        ///   - "event": "Name (Event Date Type)"
        /// </summary>
        public string MapToDisplayText(SearchResult result, string format = "full")
        {
            if (result == null)
                return "";

            return format?.ToLower() switch
            {
                "short" => FormatShort(result),
                "family" => FormatFamily(result),
                "event" => FormatEvent(result),
                "full" or _ => result.GetDisplayText(includeAdditional: true)
            };
        }

        /// <summary>
        /// Formats result list header/label showing search summary.
        /// Format: "Found N items for 'searchtext'"
        /// </summary>
        public string MapToResultsLabel(int totalResults, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                searchText = "(empty)";

            var pluralForm = totalResults == 1 ? "item" : "items";
            return $"Found {totalResults} {pluralForm} for '{searchText}'";
        }

        // ====================================================================
        // Private Formatting Helpers
        // ====================================================================

        /// <summary>Short format: "Name (Year)"</summary>
        private string FormatShort(SearchResult result)
        {
            return $"{result.Name.TrimEnd()} ({result.Year})";
        }

        /// <summary>Family format: "Name (Family ID)"</summary>
        private string FormatFamily(SearchResult result)
        {
            return $"{result.Name.TrimEnd()} (Fam.#{result.SecondaryId})";
        }

        /// <summary>Event format: "Name (Event Date Type)"</summary>
        private string FormatEvent(SearchResult result)
        {
            var eventType = result.AdditionalInfo ?? "Event";
            var dateStr = result.Date.Year > 1900 ? result.Date.ToString("dd.MM.yyyy") : "?";
            return $"{result.Name.TrimEnd()} ({dateStr} - {eventType})";
        }
    }
}
