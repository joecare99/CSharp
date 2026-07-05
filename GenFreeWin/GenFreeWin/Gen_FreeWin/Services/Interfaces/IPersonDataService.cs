// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IPersonDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Service contract for person name and genealogy data access</summary>
// ***********************************************************************

using GenFree.Interfaces.Data;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services.Interfaces
{
    /// <summary>
    /// Service for accessing and manipulating person name and genealogy data.
    /// Abstracts direct DataModul access and provides structured data queries.
    /// </summary>
    public interface IPersonDataService
    {
        /// <summary>
        /// Gets the formatted full name for a person.
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <returns>Full name string including surname, given names, prefix, suffix, and alias.</returns>
        Task<string> GetFullNameAsync(int personId);

        /// <summary>
        /// Gets the sex indicator for a person (M/F).
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <returns>Sex indicator or empty string if not found.</returns>
        Task<string> GetSexAsync(int personId);

        /// <summary>
        /// Consolidates names and aliases when merging two persons.
        /// </summary>
        /// <param name="sourcePerId">Source person ID (to be deleted).</param>
        /// <param name="targetPersonId">Target person ID (survivor).</param>
        /// <returns>True if consolidation succeeded; false otherwise.</returns>
        Task<bool> ConsolidateNamesAsync(int sourcePerId, int targetPersonId);

        /// <summary>
        /// Gets ancestor/ancestry data indicators for a person.
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <returns>Tuple of (AncestorCount, AncestorData). Returns (0, empty string) if retrieval failed.</returns>
        Task<(int AncestorCount, string AncestorData)> GetAncestorDataAsync(int personId);

        /// <summary>
        /// Retrieves the parent link for a person (parents' marriage record).
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <returns>Family ID of parents, or 0 if no parent link exists.</returns>
        Task<int> GetParentFamilyAsync(int personId);

        /// <summary>
        /// Retrieves all marriage/partnership family IDs for a person.
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <returns>Array of family IDs where this person is a spouse/partner.</returns>
        Task<int[]> GetSpouseFamiliesAsync(int personId);
    }
}
