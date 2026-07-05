// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="RepoSearchUseCase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Business logic use case for repository search and initialization</summary>
// ***********************************************************************

using BaseLib.Helper;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Gen_FreeWin.UseCases
{
    /// <summary>
    /// Encapsulates business logic for repository search, loading, and list management.
    /// Decouples high-level search workflows from data access and UI presentation layers.
    /// </summary>
    public class RepoSearchUseCase
    {
        private readonly IRepoDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoSearchUseCase"/> class.
        /// </summary>
        /// <param name="dataService">The repository data service.</param>
        public RepoSearchUseCase(IRepoDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        /// <summary>
        /// Loads all repositories and populates the repository list.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. Returns the populated list.</returns>
        public async Task<ObservableCollection<IListItem<int>>> LoadRepositoriesAsync()
        {
            var items = new ObservableCollection<IListItem<int>>();
            try
            {
                var repos = await _dataService.LoadAllRepositoriesAsync();
                foreach (var repo in repos)
                {
                    items.Add(new MyListItem(repo.DisplayText, repo.Id));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadRepositoriesAsync error: {ex.Message}");
            }
            return items;
        }

        /// <summary>
        /// Searches for repositories by a given search text and populates the result list.
        /// </summary>
        /// <param name="searchText">The search text (partial name or place).</param>
        /// <returns>A task representing the asynchronous operation. Returns the search-result collection.</returns>
        public async Task<ObservableCollection<IListItem<int>>> SearchRepositoriesAsync(string searchText)
        {
            var items = new ObservableCollection<IListItem<int>>();
            try
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return await LoadRepositoriesAsync();
                }

                var repos = await _dataService.SearchRepositoriesAsync(searchText.Trim());
                foreach (var repo in repos)
                {
                    items.Add(new MyListItem(repo.DisplayText, repo.Id));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchRepositoriesAsync error: {ex.Message}");
            }
            return items;
        }

        /// <summary>
        /// Loads repository details and associated sources for display in the ViewModel.
        /// </summary>
        /// <param name="repoId">The repository ID to load.</param>
        /// <returns>A tuple containing the repository model and its associated sources, or (null, empty) if not found.</returns>
        public async Task<(RepoModel? repo, List<RepoSourceModel> sources)> LoadRepositoryDetailsAsync(int repoId)
        {
            try
            {
                var repo = await _dataService.LoadRepositoryByIdAsync(repoId);
                if (repo == null)
                {
                    return (null, new List<RepoSourceModel>());
                }

                var sources = await _dataService.LoadSourcesByRepositoryAsync(repoId);
                return (repo, sources);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadRepositoryDetailsAsync error: {ex.Message}");
                return (null, new List<RepoSourceModel>());
            }
        }

        /// <summary>
        /// Loads and formats sources for display in the ViewModel's source list.
        /// </summary>
        /// <param name="repoId">The repository ID.</param>
        /// <returns>A task representing the asynchronous operation. Returns the populated source list.</returns>
        public async Task<ObservableCollection<IListItem<int>>> LoadSourcesForRepositoryAsync(int repoId)
        {
            var items = new ObservableCollection<IListItem<int>>();
            try
            {
                var sources = await _dataService.LoadSourcesByRepositoryAsync(repoId);
                foreach (var source in sources.OrderBy(s => s.SourceDescription))
                {
                    items.Add(new MyListItem(source.SourceDescription.Trim() + " ", source.SourceId));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadSourcesForRepositoryAsync error: {ex.Message}");
            }
            return items;
        }

        /// <summary>
        /// Validates repository data before saving (business rules enforcement).
        /// </summary>
        /// <param name="repo">The repository model to validate.</param>
        /// <returns>A tuple containing (isValid, errorMessage).</returns>
        public (bool isValid, string errorMessage) ValidateRepository(RepoModel repo)
        {
            if (string.IsNullOrWhiteSpace(repo.Name))
            {
                return (false, "Repository name is required.");
            }

            if (repo.Name.Length > 100)
            {
                return (false, "Repository name cannot exceed 100 characters.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Persists a new or updated repository record and links it to a source.
        /// </summary>
        /// <param name="repo">The repository model to save.</param>
        /// <param name="sourceId">The source ID to link (optional, 0 = no link).</param>
        /// <returns>A task representing the asynchronous operation. Returns the repository ID if successful, -1 if failed.</returns>
        public async Task<int> SaveRepositoryWithSourceAsync(RepoModel repo, int sourceId = 0)
        {
            try
            {
                var (isValid, errorMessage) = ValidateRepository(repo);
                if (!isValid)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation error: {errorMessage}");
                    return -1;
                }

                int repoId = await _dataService.SaveRepositoryAsync(repo);
                if (repoId > 0 && sourceId > 0)
                {
                    await _dataService.LinkSourceToRepositoryAsync(sourceId, repoId);
                }

                return repoId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveRepositoryWithSourceAsync error: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Deletes a repository record from the database.
        /// </summary>
        /// <param name="repoId">The repository ID to delete.</param>
        /// <returns>A task representing the asynchronous operation. Returns true if successful; otherwise false.</returns>
        public async Task<bool> DeleteRepositoryAsync(int repoId)
        {
            try
            {
                return await _dataService.DeleteRepositoryAsync(repoId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteRepositoryAsync error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the next available repository ID for new record creation.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. Returns the next ID.</returns>
        public async Task<int> GetNextRepositoryIdAsync()
        {
            try
            {
                return await _dataService.GetNextRepositoryIdAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetNextRepositoryIdAsync error: {ex.Message}");
                return 1;
            }
        }
    }
}

