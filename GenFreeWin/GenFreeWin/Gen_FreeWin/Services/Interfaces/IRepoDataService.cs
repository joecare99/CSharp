// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IRepoDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Service interface for Repository data access and persistence</summary>
// ***********************************************************************

using Gen_FreeWin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services.Interfaces
{
    /// <summary>
    /// Service contract for Repository data access and database persistence operations.
    /// Abstracts the legacy DataModul layer for testability and maintainability.
    /// </summary>
    public interface IRepoDataService
    {
        /// <summary>
        /// Loads all repositories from the database, ordered by Name and Place.
        /// </summary>
        /// <returns>A list of repository models.</returns>
        Task<List<RepoModel>> LoadAllRepositoriesAsync();

        /// <summary>
        /// Loads a single repository by its ID.
        /// </summary>
        /// <param name="repoId">The repository ID.</param>
        /// <returns>The repository model, or null if not found.</returns>
        Task<RepoModel?> LoadRepositoryByIdAsync(int repoId);

        /// <summary>
        /// Saves or updates a repository record in the database.
        /// If the repository does not exist, adds a new record.
        /// If the repository exists, updates the existing record.
        /// </summary>
        /// <param name="repo">The repository model to save.</param>
        /// <returns>The ID of the saved repository (new or existing).</returns>
        Task<int> SaveRepositoryAsync(RepoModel repo);

        /// <summary>
        /// Updates an existing repository record.
        /// </summary>
        /// <param name="repoId">The repository ID.</param>
        /// <param name="repo">The updated repository model.</param>
        /// <returns>True if the update was successful; otherwise false.</returns>
        Task<bool> UpdateRepositoryAsync(int repoId, RepoModel repo);

        /// <summary>
        /// Deletes a repository record by its ID.
        /// </summary>
        /// <param name="repoId">The repository ID.</param>
        /// <returns>True if the deletion was successful; otherwise false.</returns>
        Task<bool> DeleteRepositoryAsync(int repoId);

        /// <summary>
        /// Loads all sources (documents) associated with a given repository.
        /// </summary>
        /// <param name="repoId">The repository ID.</param>
        /// <returns>A list of associated source models.</returns>
        Task<List<RepoSourceModel>> LoadSourcesByRepositoryAsync(int repoId);

        /// <summary>
        /// Adds or updates a source-repository association link in the database.
        /// </summary>
        /// <param name="sourceId">The source/document ID.</param>
        /// <param name="repoId">The repository ID.</param>
        /// <returns>True if the link was created or already exists; otherwise false.</returns>
        Task<bool> LinkSourceToRepositoryAsync(int sourceId, int repoId);

        /// <summary>
        /// Gets the next available repository ID (for new record creation).
        /// </summary>
        /// <returns>The next available repository ID.</returns>
        Task<int> GetNextRepositoryIdAsync();

        /// <summary>
        /// Searches for repositories by name and/or place, returning matching results.
        /// </summary>
        /// <param name="searchText">The search string (empty to return all).</param>
        /// <returns>A list of matching repositories.</returns>
        Task<List<RepoModel>> SearchRepositoriesAsync(string searchText);
    }
}
