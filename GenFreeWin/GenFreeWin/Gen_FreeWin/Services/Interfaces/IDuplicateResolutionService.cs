// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IDuplicateResolutionService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Service contract for duplicate person/family merging</summary>
// ***********************************************************************

using GenFreeWin.Models;
using System.Threading.Tasks;

namespace GenFreeWin.Services.Interfaces
{
    /// <summary>
    /// Service for orchestrating person or family duplicate merge/resolution operations.
    /// Handles event swapping, data consolidation, link removal, and final deletion.
    /// </summary>
    public interface IDuplicateResolutionService
    {
        /// <summary>
        /// Merges two person records, consolidating all links and events to target person.
        /// </summary>
        /// <param name="state">The operation state containing both person IDs and context.</param>
        /// <param name="targetPersonId">The person ID to merge into (survivor).</param>
        /// <returns>True if merge completed successfully; false otherwise.</returns>
        Task<bool> MergePersonsAsync(DubOperationState state, int targetPersonId);

        /// <summary>
        /// Merges two family records by consolidating children, events, and links.
        /// </summary>
        /// <param name="state">The operation state containing both family IDs.</param>
        /// <param name="targetFamilyId">The family ID to merge into (survivor).</param>
        /// <returns>True if family merge succeeded; false otherwise.</returns>
        Task<bool> MergeFamiliesAsync(DubOperationState state, int targetFamilyId);

        /// <summary>
        /// Compares events between two persons to identify differences.
        /// </summary>
        /// <param name="person1Id">First person ID.</param>
        /// <param name="person2Id">Second person ID.</param>
        /// <returns>Populated state with differing event types.</returns>
        Task<DubOperationState> ComparePersonEventsAsync(int person1Id, int person2Id);

        /// <summary>
        /// Swaps/exchanges event data between two person records (used in some merge strategies).
        /// </summary>
        /// <param name="person1Id">First person ID.</param>
        /// <param name="person2Id">Second person ID.</param>
        /// <returns>True if swap succeeded; false otherwise.</returns>
        Task<bool> SwapPersonEventsAsync(int person1Id, int person2Id);

        /// <summary>
        /// Validates that a merge operation is safe (no orphaned links, constraint violations, etc.).
        /// </summary>
        /// <param name="person1Id">First person ID.</param>
        /// <param name="person2Id">Second person ID.</param>
        /// <returns>True if merge is safe; false otherwise.</returns>
        Task<bool> ValidateMergeAsync(int person1Id, int person2Id);
    }
}
