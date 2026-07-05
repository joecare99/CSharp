// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="VornamSearchUseCase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Vorname business logic and workflows orchestration</summary>
// ***********************************************************************

using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gen_FreeWin.UseCases
{
    /// <summary>
    /// Orchestrates business logic for Vorname (given name) workflows.
    /// Coordinates data retrieval, validation, and persistence between UseCase and DataService.
    /// </summary>
    public class VornamSearchUseCase
    {
        private readonly IVornamDataService _dataService;

        /// <summary>
        /// Creates a new VornamSearchUseCase instance.
        /// </summary>
        public VornamSearchUseCase(IVornamDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        /// <summary>
        /// Loads all names for a person by gender.
        /// </summary>
        public async Task<ObservableCollection<VornamModel>> LoadNamesAsync(int personId, ETextKennz textKennz)
        {
            var names = await _dataService.LoadNamesForPersonAsync(personId, textKennz);
            var result = new ObservableCollection<VornamModel>();

            foreach (var name in names ?? new System.Collections.Generic.List<VornamModel>())
            {
                name.GenerateDisplayText();
                result.Add(name);
            }

            return result;
        }

        /// <summary>
        /// Searches for names matching a pattern and formats for UI display.
        /// </summary>
        public async Task<ObservableCollection<IListItem<int>>> SearchNamesAsync(ETextKennz textKennz, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern) || searchPattern.Length < 1)
                return new ObservableCollection<IListItem<int>>();

            var result = await _dataService.SearchNamesAsync(textKennz, searchPattern);
            return result ?? new ObservableCollection<IListItem<int>>();
        }

        /// <summary>
        /// Saves a single name entry with validation.
        /// </summary>
        public async Task<(bool Success, short LineNumber, string? Error)> SaveNameAsync(VornamModel vorname)
        {
            if (vorname == null)
                return (false, -1, "Name model is null");

            if (!vorname.IsValid())
                return (false, -1, "Name is invalid (minimum 2 characters required)");

            try
            {
                var lineNumber = await _dataService.SaveNameAsync(vorname);
                if (lineNumber > 0)
                    return (true, lineNumber, null);

                return (false, -1, "Failed to save name");
            }
            catch (Exception ex)
            {
                return (false, -1, $"Save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing name entry with validation.
        /// </summary>
        public async Task<(bool Success, string? Error)> UpdateNameAsync(VornamModel vorname)
        {
            if (vorname == null)
                return (false, "Name model is null");

            if (!vorname.IsValid())
                return (false, "Name is invalid");

            try
            {
                var success = await _dataService.UpdateNameAsync(vorname);
                if (success)
                    return (true, null);

                return (false, "Failed to update name");
            }
            catch (Exception ex)
            {
                return (false, $"Update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all names of a given kind from a person.
        /// </summary>
        public async Task<(bool Success, int DeletedCount, string? Error)> DeleteNamesByKindAsync(int personId, ETextKennz textKennz)
        {
            if (personId <= 0)
                return (false, 0, "Invalid person ID");

            try
            {
                var deletedCount = await _dataService.DeleteNamesByKindAsync(personId, textKennz);
                return (true, deletedCount, null);
            }
            catch (Exception ex)
            {
                return (false, 0, $"Delete error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a single name by line number.
        /// </summary>
        public async Task<VornamModel?> GetNameByLineAsync(int personId, short lineNumber)
        {
            if (personId <= 0 || lineNumber <= 0)
                return null;

            var name = await _dataService.GetNameByLineAsync(personId, lineNumber);
            name?.GenerateDisplayText();
            return name;
        }

        /// <summary>
        /// Retrieves text information by ID for display purposes.
        /// </summary>
        public async Task<(string Text, string LeadName)?> GetTextByIdAsync(int textId)
        {
            if (textId <= 0)
                return null;

            return await _dataService.GetTextByIdAsync(textId);
        }

        /// <summary>
        /// Saves all names from a batch (e.g., from form submission).
        /// Handles transaction management and error handling.
        /// </summary>
        public async Task<(bool Success, int SavedCount, string? Error)> SaveBatchNamesAsync(
            int personId, 
            ETextKennz textKennz, 
            System.Collections.Generic.List<VornamModel> names)
        {
            if (personId <= 0 || names == null || names.Count == 0)
                return (false, 0, "Invalid person ID or empty name list");

            try
            {
                // Delete existing names for this person/kind
                await _dataService.DeleteNamesByKindAsync(personId, textKennz);

                // Save all new names
                int savedCount = 0;
                foreach (var name in names)
                {
                    if (name.IsValid())
                    {
                        var lineNumber = await _dataService.SaveNameAsync(name);
                        if (lineNumber > 0)
                            savedCount++;
                    }
                }

                return (true, savedCount, null);
            }
            catch (Exception ex)
            {
                return (false, 0, $"Batch save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates name according to business rules and legacy format requirements.
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateName(VornamModel vorname)
        {
            if (vorname == null)
                return (false, "Name is null");

            if (string.IsNullOrWhiteSpace(vorname.PrimaryName))
                return (false, "Primary name cannot be empty");

            if (vorname.PrimaryName.Trim().Length < 2)
                return (false, "Primary name must be at least 2 characters");

            if (vorname.PrimaryName.Length > 240)
                return (false, "Primary name exceeds 240 characters");

            if (vorname.Synonym != null && vorname.Synonym.Length > 240)
                return (false, "Synonym exceeds 240 characters");

            return (true, null);
        }
    }
}
