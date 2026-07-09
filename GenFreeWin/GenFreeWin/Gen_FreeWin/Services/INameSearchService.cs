// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="INameSearchService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Service interface for name search operations</summary>
// ***********************************************************************

using Gen_FreeWin.ViewModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Service for executing name searches with filtering, validation, and result mapping.
    /// Encapsulates search business logic previously embedded in NamenSuchViewModel.
    /// </summary>
    public interface INameSearchService
    {
        /// <summary>
        /// Executes a name search based on provided criteria.
        /// </summary>
        /// <param name="criteria">Search criteria (text, gender filter, family mode, etc.).</param>
        /// <returns>List of SearchResult items matching the criteria.</returns>
        Task<IList<SearchResult>> ExecuteSearchAsync(SearchCriteria criteria);

        /// <summary>
        /// Validates search criteria before execution.
        /// </summary>
        /// <param name="criteria">Criteria to validate.</param>
        /// <returns>ValidationResult with success flag and error message if invalid.</returns>
        (bool Success, string ErrorMessage) ValidateCriteria(SearchCriteria criteria);

        /// <summary>
        /// Filters a set of results against additional criteria (e.g., death filter, date range).
        /// </summary>
        /// <param name="allResults">Unfiltered result set.</param>
        /// <param name="options">Filter options to apply.</param>
        /// <returns>Filtered subset of results.</returns>
        IList<SearchResult> ApplyFilters(IList<SearchResult> allResults, FilterOptions options);

        /// <summary>
        /// Applies sorting to results (e.g., by name, date, relevance).
        /// </summary>
        /// <param name="results">Results to sort.</param>
        /// <param name="sortField">Field to sort by (Name, Date, Relevance).</param>
        /// <param name="ascending">Sort direction.</param>
        /// <returns>Sorted results.</returns>
        IList<SearchResult> SortResults(IList<SearchResult> results, string sortField, bool ascending = true);

        /// <summary>
        /// Finds related results for a given primary result (e.g., family members, spouse).
        /// </summary>
        /// <param name="primaryResult">Result to find relations for.</param>
        /// <returns>Related results.</returns>
        Task<IList<SearchResult>> FindRelatedAsync(SearchResult primaryResult);

        /// <summary>
        /// Extracts search statistics for display (total count, gender breakdown, etc.).
        /// </summary>
        /// <param name="results">Results to analyze.</param>
        /// <returns>Statistics dictionary.</returns>
        Dictionary<string, int> GetSearchStatistics(IList<SearchResult> results);
    }
}
