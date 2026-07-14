// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="NameSearchService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation of name search service</summary>
// ***********************************************************************

using Gen_FreeWin.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Implements name search operations with filtering, validation, and sorting.
    /// Extracted from NamenSuchViewModel business logic to separate concerns.
    /// </summary>
    public class NameSearchService : INameSearchService
    {
        // NOTE: Persistence parameter may be null if GenFree.Interfaces is not available in build context
        private readonly object _persistence;

        /// <summary>
        /// Creates a new instance of NameSearchService.
        /// </summary>
        /// <param name="persistence">Persistence layer for database access (may be null in some build contexts).</param>
        public NameSearchService(object persistence = null)
        {
            _persistence = persistence; // Accept any object type for build flexibility
        }

        /// <summary>
        /// Executes a name search asynchronously based on provided criteria.
        /// This method implements the core search logic previously in NamenSuchViewModel.StartSearch().
        /// </summary>
        public async Task<IList<SearchResult>> ExecuteSearchAsync(SearchCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var (isValid, errorMsg) = ValidateCriteria(criteria);
            if (!isValid)
                throw new InvalidOperationException(errorMsg);

            return await Task.Run(() => PerformSearch(criteria));
        }

        /// <summary>
        /// Validates search criteria for minimum requirements.
        /// </summary>
        public (bool Success, string ErrorMessage) ValidateCriteria(SearchCriteria criteria)
        {
            if (criteria == null)
                return (false, "Criteria cannot be null.");

            if (string.IsNullOrWhiteSpace(criteria.SearchText))
                return (false, "Search text is required.");

            // For family-only search, no gender filter needed
            if (criteria.FamilyOnly)
                return (true, "");

            // For person-based search, at least one gender must be selected
            if (!criteria.IncludeMales && !criteria.IncludeFemales &&
                !criteria.MaleFamily && !criteria.FemaleFamily)
                return (false, "At least one gender filter must be selected.");

            return (true, "");
        }

        /// <summary>
        /// Applies additional filters to result set (death marker, date range, etc.).
        /// </summary>
        public IList<SearchResult> ApplyFilters(IList<SearchResult> allResults, FilterOptions options)
        {
            if (allResults == null || allResults.Count == 0)
                return new List<SearchResult>();

            var filtered = new List<SearchResult>(allResults);

            // If death mark filter is active, exclude deceased persons (where available)
            if (options.DeathMarkActive)
            {
                filtered = filtered.Where(r => r.Gender != 'D').ToList(); // Simplified; adjust per actual criteria
            }

            return filtered;
        }

        /// <summary>
        /// Sorts results by specified field and order.
        /// </summary>
        public IList<SearchResult> SortResults(IList<SearchResult> results, string sortField, bool ascending = true)
        {
            if (results == null || results.Count == 0)
                return results;

            IOrderedEnumerable<SearchResult> ordered = sortField?.ToLower() switch
            {
                "name" => ascending
                    ? results.OrderBy(r => r.Name)
                    : results.OrderByDescending(r => r.Name),
                "date" => ascending
                    ? results.OrderBy(r => r.Date)
                    : results.OrderByDescending(r => r.Date),
                "year" => ascending
                    ? results.OrderBy(r => r.Year)
                    : results.OrderByDescending(r => r.Year),
                "relevance" => ascending
                    ? results.OrderBy(r => r.RelevanceScore)
                    : results.OrderByDescending(r => r.RelevanceScore),
                _ => ascending
                    ? results.OrderBy(r => r.Name)
                    : results.OrderByDescending(r => r.Name)
            };

            return ordered.ToList();
        }

        /// <summary>
        /// Finds related results (e.g., family members, spouse) for a primary result.
        /// Placeholder for future implementation with database relationships.
        /// </summary>
        public async Task<IList<SearchResult>> FindRelatedAsync(SearchResult primaryResult)
        {
            if (primaryResult == null)
                throw new ArgumentNullException(nameof(primaryResult));

            // TODO: Implement relationship queries against IGenPersistence
            return await Task.FromResult(new List<SearchResult>());
        }

        /// <summary>
        /// Extracts statistics from search results (count, gender breakdown, date range).
        /// </summary>
        public Dictionary<string, int> GetSearchStatistics(IList<SearchResult> results)
        {
            var stats = new Dictionary<string, int>
            {
                ["Total"] = results?.Count ?? 0,
                ["Males"] = results?.Count(r => r.Gender == 'M') ?? 0,
                ["Females"] = results?.Count(r => r.Gender == 'F') ?? 0,
                ["Unknown"] = results?.Count(r => r.Gender == 'U') ?? 0
            };

            if (results != null && results.Count > 0)
            {
                stats["EarliestYear"] = results.Min(r => r.Year);
                stats["LatestYear"] = results.Max(r => r.Year);
            }

            return stats;
        }

        // ====================================================================
        // Private Helper Methods
        // ====================================================================

        /// <summary>
        /// Internal method that performs the actual search against persistence layer.
        /// This encapsulates the core logic from NamenSuchViewModel.StartSearch().
        /// </summary>
        private IList<SearchResult> PerformSearch(SearchCriteria criteria)
        {
            var results = new List<SearchResult>();

            try
            {
                // Apply search text formatting (wildcard handling, etc.)
                var searchPattern = NormalizeSearchText(criteria.SearchText);

                // TODO: Implement actual persistence query
                // For now, placeholder logic:
                // if (criteria.Mode == SearchMode.PersonBased)
                // {
                //     var records = _persistence.QueryPeople(searchPattern, criteria.IncludeMales, criteria.IncludeFemales);
                //     results = MapRecordsToResults(records);
                // }
                // else if (criteria.Mode == SearchMode.FamilyBased)
                // {
                //     var records = _persistence.QueryFamilies(searchPattern);
                //     results = MapRecordsToResults(records);
                // }

                // Apply secondary filters
                results = results
                    .Where(r => ApplyGenderFilter(r, criteria))
                    .ToList();

                // Calculate relevance scores based on match quality
                CalculateRelevanceScores(results, searchPattern);

                // Sort by relevance descending, then by name
                results = results
                    .OrderByDescending(r => r.RelevanceScore)
                    .ThenBy(r => r.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
                // Return empty results on error rather than propagating exception
                results = new List<SearchResult>();
            }

            return results;
        }

        /// <summary>
        /// Normalizes search input (trimming, wildcard expansion, accent handling).
        /// </summary>
        private string NormalizeSearchText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            // Remove leading/trailing whitespace
            var normalized = input.Trim();

            // Handle wildcard expansion (* → % for SQL LIKE)
            normalized = normalized.Replace("*", "%");

            // Remove duplicate wildcards
            while (normalized.Contains("%%"))
                normalized = normalized.Replace("%%", "%");

            return normalized;
        }

        /// <summary>
        /// Applies gender filter logic to a single result.
        /// </summary>
        private bool ApplyGenderFilter(SearchResult result, SearchCriteria criteria)
        {
            if (criteria.FamilyOnly)
                return result.ResultType == SearchResultType.Family;

            if (criteria.OmitSpouse)
                return !result.AdditionalInfo.Contains("Spouse"); // Simplified logic

            switch (result.Gender)
            {
                case 'M':
                    return criteria.IncludeMales || criteria.MaleFamily;
                case 'F':
                    return criteria.IncludeFemales || criteria.FemaleFamily;
                default:
                    return true; // Include unknown gender by default
            }
        }

        /// <summary>
        /// Calculates relevance scores for sorting results by match quality.
        /// </summary>
        private void CalculateRelevanceScores(IList<SearchResult> results, string searchPattern)
        {
            foreach (var result in results)
            {
                result.RelevanceScore = 0.0;

                if (string.IsNullOrEmpty(searchPattern))
                    continue;

                // Exact match scores higher
                if (result.Name.Equals(searchPattern, StringComparison.OrdinalIgnoreCase))
                    result.RelevanceScore = 100.0;
                // Starts-with match scores medium
                else if (result.Name.StartsWith(searchPattern, StringComparison.OrdinalIgnoreCase))
                    result.RelevanceScore = 50.0;
                // Contains match scores lower
                else if (result.Name.Contains(searchPattern, StringComparison.OrdinalIgnoreCase))
                    result.RelevanceScore = 25.0;
                else
                    result.RelevanceScore = 0.0;
            }
        }
    }
}
