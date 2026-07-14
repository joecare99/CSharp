// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IDuplicateDetectionService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Service contract for duplicate detection logic</summary>
// ***********************************************************************

using GenFreeWin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenFreeWin.Services.Interfaces
{
    /// <summary>
    /// Service for detecting and retrieving potential duplicate candidates.
    /// Encapsulates search by name, UID, or other duplicate-detection algorithms.
    /// </summary>
    public interface IDuplicateDetectionService
    {
        /// <summary>
        /// Searches for potential duplicates by name pattern (phonetic/text-based).
        /// </summary>
        /// <param name="searchText">The name or partial search term.</param>
        /// <param name="maxResults">Maximum number of results to retrieve.</param>
        /// <param name="filterByMaleOnly">Filter to males only if true.</param>
        /// <param name="filterByFemaleOnly">Filter to females only if true.</param>
        /// <param name="includeParents">Include parent name matching if true.</param>
        /// <param name="includeChildren">Include child name matching if true.</param>
        /// <returns>List of potential duplicate candidates.</returns>
        Task<List<DubSearchResult>> SearchByNameAsync(string searchText, int maxResults = 100,
            bool filterByMaleOnly = false, bool filterByFemaleOnly = false,
            bool includeParents = false, bool includeChildren = false);

        /// <summary>
        /// Searches for potential duplicates by UID (unique person identifier).
        /// Used for detecting records with duplicate unique IDs.
        /// </summary>
        /// <returns>List of candidates with matching or conflicting UIDs.</returns>
        Task<List<DubSearchResult>> SearchByUidAsync();

        /// <summary>
        /// Retrieves detailed person information for duplicate analysis.
        /// </summary>
        /// <param name="personId">The person ID to retrieve.</param>
        /// <returns>Extended person data for comparison.</returns>
        Task<DubSearchResult> GetPersonDetailsAsync(int personId);
    }
}
