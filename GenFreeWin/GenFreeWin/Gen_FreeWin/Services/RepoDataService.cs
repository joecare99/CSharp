// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="RepoDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation of Repository data access and persistence operations</summary>
// ***********************************************************************

using BaseLib.Helper;
using GenFreeWin.Models;
using GenFreeWin.Services.Interfaces;
using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenFreeWin.Services
{
    /// <summary>
    /// Implements Repository data access and persistence operations.
    /// Encapsulates all direct DataModul interactions to isolate legacy database layer.
    /// </summary>
    public class RepoDataService : IRepoDataService
    {
        /// <summary>
        /// Loads all repositories from the database, ordered by Name and Place.
        /// </summary>
        public Task<List<RepoModel>> LoadAllRepositoriesAsync()
        {
            return Task.Run(() =>
            {
                var repos = new List<RepoModel>();
                try
                {
                    DataModul.DB_RepoTable.Index = "Name";
                    DataModul.DB_RepoTable.MoveFirst();

                    while (!DataModul.DB_RepoTable.EOF)
                    {
                        var repo = new RepoModel(
                            id: DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt(),
                            name: DataModul.DB_RepoTable.Fields[RepoFields.Name].AsString(),
                            place: DataModul.DB_RepoTable.Fields[RepoFields.Ort].AsString()
                        )
                        {
                            Street = DataModul.DB_RepoTable.Fields[RepoFields.Strasse].AsString(),
                            PostalCode = DataModul.DB_RepoTable.Fields[RepoFields.PLZ].AsString(),
                            Phone = DataModul.DB_RepoTable.Fields[RepoFields.Fon].AsString(),
                            Email = DataModul.DB_RepoTable.Fields[RepoFields.Mail].AsString(),
                            Website = DataModul.DB_RepoTable.Fields[RepoFields.Http].AsString(),
                            Remarks = DataModul.DB_RepoTable.Fields[RepoFields.Bem].AsString()
                        };
                        repos.Add(repo);
                        DataModul.DB_RepoTable.MoveNext();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadAllRepositoriesAsync error: {ex.Message}");
                }
                return repos;
            });
        }

        /// <summary>
        /// Loads a single repository by its ID.
        /// </summary>
        public Task<RepoModel?> LoadRepositoryByIdAsync(int repoId)
        {
            return Task.Run(() =>
            {
                RepoModel? repo = null;
                try
                {
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.Seek("=", repoId);

                    if (!DataModul.DB_RepoTable.NoMatch)
                    {
                        repo = new RepoModel(
                            id: DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt(),
                            name: DataModul.DB_RepoTable.Fields[RepoFields.Name].AsString(),
                            place: DataModul.DB_RepoTable.Fields[RepoFields.Ort].AsString()
                        )
                        {
                            Street = DataModul.DB_RepoTable.Fields[RepoFields.Strasse].AsString(),
                            PostalCode = DataModul.DB_RepoTable.Fields[RepoFields.PLZ].AsString(),
                            Phone = DataModul.DB_RepoTable.Fields[RepoFields.Fon].AsString(),
                            Email = DataModul.DB_RepoTable.Fields[RepoFields.Mail].AsString(),
                            Website = DataModul.DB_RepoTable.Fields[RepoFields.Http].AsString(),
                            Remarks = DataModul.DB_RepoTable.Fields[RepoFields.Bem].AsString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadRepositoryByIdAsync error: {ex.Message}");
                }
                return repo;
            });
        }

        /// <summary>
        /// Saves or updates a repository record in the database.
        /// </summary>
        public Task<int> SaveRepositoryAsync(RepoModel repo)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!repo.IsValid())
                    {
                        throw new ArgumentException("Repository name is required.");
                    }

                    // Get next ID for new record
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.MoveLast();
                    int nextId = DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt() + 1;

                    // Check if repository already exists (by name + place)
                    DataModul.DB_RepoTable.Index = "Such";
                    DataModul.DB_RepoTable.Seek("=", repo.Name.Trim(), repo.Place.Trim());

                    if (DataModul.DB_RepoTable.NoMatch)
                    {
                        // Add new record
                        DataModul.DB_RepoTable.AddNew();
                        DataModul.DB_RepoTable.Fields[RepoFields.Nr].Value = nextId;
                        SetFieldsFromModel(repo);
                        DataModul.DB_RepoTable.Update();
                        repo.Id = nextId;
                    }
                    else
                    {
                        // Update existing record
                        DataModul.DB_RepoTable.Edit();
                        int existingId = DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt();
                        SetFieldsFromModel(repo);
                        DataModul.DB_RepoTable.Update();
                        repo.Id = existingId;
                    }

                    return repo.Id;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SaveRepositoryAsync error: {ex.Message}");
                    return -1;
                }
            });
        }

        /// <summary>
        /// Updates an existing repository record.
        /// </summary>
        public Task<bool> UpdateRepositoryAsync(int repoId, RepoModel repo)
        {
            return Task.Run(() =>
            {
                try
                {
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.Seek("=", repoId);

                    if (DataModul.DB_RepoTable.NoMatch)
                    {
                        return false;
                    }

                    DataModul.DB_RepoTable.Edit();
                    SetFieldsFromModel(repo);
                    DataModul.DB_RepoTable.Update();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateRepositoryAsync error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Deletes a repository record by its ID.
        /// </summary>
        public Task<bool> DeleteRepositoryAsync(int repoId)
        {
            return Task.Run(() =>
            {
                try
                {
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.Seek("=", repoId);

                    if (DataModul.DB_RepoTable.NoMatch)
                    {
                        return false;
                    }

                    DataModul.DB_RepoTable.Delete();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DeleteRepositoryAsync error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Loads all sources (documents) associated with a given repository.
        /// </summary>
        public Task<List<RepoSourceModel>> LoadSourcesByRepositoryAsync(int repoId)
        {
            return Task.Run(() =>
            {
                var sources = new List<RepoSourceModel>();
                try
                {
                    DataModul.DB_RepoTab.Index = "leer";
                    DataModul.DB_RepoTab.Seek("=", repoId);

                    while (!DataModul.DB_RepoTab.NoMatch && !DataModul.DB_RepoTab.EOF)
                    {
                        int repoInRecord = DataModul.DB_RepoTab.Fields["Repo"].AsInt();
                        if (repoInRecord != repoId)
                        {
                            break;
                        }

                        int sourceId = DataModul.DB_RepoTab.Fields["Quelle"].AsInt();

                        // Load source description from QuTable
                        DataModul.DB_QuTable.Index = "Nr";
                        DataModul.DB_QuTable.Seek("=", sourceId);

                        if (!DataModul.DB_QuTable.NoMatch)
                        {
                            string sourceDesc = DataModul.DB_QuTable.Fields[QuFields._2].AsString();
                            sources.Add(new RepoSourceModel(sourceId, repoId, sourceDesc));
                        }

                        DataModul.DB_RepoTab.MoveNext();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadSourcesByRepositoryAsync error: {ex.Message}");
                }
                return sources;
            });
        }

        /// <summary>
        /// Adds or updates a source-repository association link in the database.
        /// </summary>
        public Task<bool> LinkSourceToRepositoryAsync(int sourceId, int repoId)
        {
            return Task.Run(() =>
            {
                try
                {
                    DataModul.DB_RepoTab.Index = "Dop";
                    DataModul.DB_RepoTab.Seek("=", sourceId, repoId);

                    if (DataModul.DB_RepoTab.NoMatch)
                    {
                        DataModul.DB_RepoTab.AddNew();
                        DataModul.DB_RepoTab.Fields["Repo"].Value = repoId;
                        DataModul.DB_RepoTab.Fields["Quelle"].Value = sourceId;
                        DataModul.DB_RepoTab.Update();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LinkSourceToRepositoryAsync error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Gets the next available repository ID (for new record creation).
        /// </summary>
        public Task<int> GetNextRepositoryIdAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    DataModul.DB_RepoTable.Index = "Nr";
                    DataModul.DB_RepoTable.MoveLast();
                    return DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt() + 1;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetNextRepositoryIdAsync error: {ex.Message}");
                    return 1;
                }
            });
        }

        /// <summary>
        /// Searches for repositories by name and/or place, returning matching results.
        /// </summary>
        public Task<List<RepoModel>> SearchRepositoriesAsync(string searchText)
        {
            return Task.Run(() =>
            {
                var repos = new List<RepoModel>();
                try
                {
                    DataModul.DB_RepoTable.Index = "Name";
                    DataModul.DB_RepoTable.Seek(">=", searchText);

                    while (!DataModul.DB_RepoTable.EOF)
                    {
                        var repo = new RepoModel(
                            id: DataModul.DB_RepoTable.Fields[RepoFields.Nr].AsInt(),
                            name: DataModul.DB_RepoTable.Fields[RepoFields.Name].AsString(),
                            place: DataModul.DB_RepoTable.Fields[RepoFields.Ort].AsString()
                        )
                        {
                            Street = DataModul.DB_RepoTable.Fields[RepoFields.Strasse].AsString(),
                            PostalCode = DataModul.DB_RepoTable.Fields[RepoFields.PLZ].AsString(),
                            Phone = DataModul.DB_RepoTable.Fields[RepoFields.Fon].AsString(),
                            Email = DataModul.DB_RepoTable.Fields[RepoFields.Mail].AsString(),
                            Website = DataModul.DB_RepoTable.Fields[RepoFields.Http].AsString(),
                            Remarks = DataModul.DB_RepoTable.Fields[RepoFields.Bem].AsString()
                        };
                        repos.Add(repo);
                        DataModul.DB_RepoTable.MoveNext();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SearchRepositoriesAsync error: {ex.Message}");
                }
                return repos;
            });
        }

        /// <summary>
        /// Helper method to set repository fields from a model instance.
        /// </summary>
        private static void SetFieldsFromModel(RepoModel repo)
        {
            DataModul.DB_RepoTable.Fields[RepoFields.Name].Value = repo.Name.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Strasse].Value = repo.Street.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Ort].Value = repo.Place.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.PLZ].Value = repo.PostalCode.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Fon].Value = repo.Phone.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Mail].Value = repo.Email.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Http].Value = repo.Website.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Bem].Value = repo.Remarks.Trim();
            DataModul.DB_RepoTable.Fields[RepoFields.Suchname].Value = $"{repo.Name} {repo.Place}".Trim();
        }
    }
}
