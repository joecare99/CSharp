// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchResult.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model representing a single search result</summary>
// ***********************************************************************

using System;

namespace GenFreeWin.ViewModels.Models
{
    /// <summary>
    /// Represents a single result item from a name search operation.
    /// Used for mapping database/persistence results to UI ListItems.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Unique identifier for this result item (typically person ID or family ID).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Secondary identifier (e.g., family number, event type).
        /// </summary>
        public int SecondaryId { get; set; }

        /// <summary>
        /// Identifier for composite searches (e.g., event art in multi-result contexts).
        /// </summary>
        public short TertiaryId { get; set; }

        /// <summary>
        /// Display name (last name, first name format or formatted string).
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Birth or occurrence date (for display in result).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Birth year or occurrence year (extracted from Date for sorting/filtering).
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Additional display information (spouse name, location, event detail, etc.).
        /// </summary>
        public string AdditionalInfo { get; set; } = "";

        /// <summary>
        /// Result type categorization (e.g., Person, Family, Event).
        /// </summary>
        public SearchResultType ResultType { get; set; }

        /// <summary>
        /// Gender indicator if applicable (M, F, U for unknown).
        /// </summary>
        public char Gender { get; set; } = 'U';

        /// <summary>
        /// Whether this result matches the primary search or secondary filter.
        /// </summary>
        public bool IsPrimaryMatch { get; set; } = true;

        /// <summary>
        /// Relevance score for ranking results (higher = more relevant).
        /// </summary>
        public double RelevanceScore { get; set; }

        /// <summary>
        /// Creates display text suitable for ListItem rendering.
        /// Format: "Name Date Year PersonId [Additional]"
        /// </summary>
        public string GetDisplayText(bool includeAdditional = false)
        {
            var display = $"{Name,-50} {Date:dd.MM.yyyy} {Year,4} {Id,8}";
            if (includeAdditional && !string.IsNullOrEmpty(AdditionalInfo))
                display += $" {AdditionalInfo}";
            return display;
        }
    }

    /// <summary>
    /// Categorizes the type of search result returned.
    /// </summary>
    public enum SearchResultType
    {
        /// <summary>Result represents a person.</summary>
        Person,
        /// <summary>Result represents a family unit.</summary>
        Family,
        /// <summary>Result represents a historical event.</summary>
        Event,
        /// <summary>Result represents a location or geographic reference.</summary>
        Location,
        /// <summary>Uncategorized or composite result.</summary>
        Other
    }
}
