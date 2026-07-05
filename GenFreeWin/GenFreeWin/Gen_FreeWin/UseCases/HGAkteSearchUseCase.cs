// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="HGAkteSearchUseCase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Business logic and workflows for HGakte (Grundbuchakte) operations</summary>
// ***********************************************************************

using BaseLib.Helper;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Gen_FreeWin.UseCases
{
    /// <summary>
    /// Business logic layer for Akte (Grundbuchakte) search, load, and management workflows.
    /// Orchestrates data service and applies business rules and validation.
    /// </summary>
    public class HGAkteSearchUseCase
    {
        private readonly IHGAkteDataService _dataService;
        private readonly IModul1 _modul1;

        /// <summary>
        /// Creates a new HGAkteSearchUseCase instance.
        /// </summary>
        /// <param name="dataService">Data access service for Akte operations.</param>
        /// <param name="modul1">Module reference for person name resolution.</param>
        public HGAkteSearchUseCase(IHGAkteDataService dataService, IModul1 modul1)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _modul1 = modul1 ?? throw new ArgumentNullException(nameof(modul1));
        }

        /// <summary>
        /// Loads all Akten as a collection of UI model (MyListItem).
        /// </summary>
        /// <returns>ObservableCollection of MyListItem for UI binding.</returns>
        public async Task<ObservableCollection<IListItem<int>>> LoadAktenAsync()
        {
            try
            {
                var items = new ObservableCollection<IListItem<int>>();
                var akten = await _dataService.LoadAllAktenAsync();

                foreach (var akte in akten)
                {
                    items.Add(new MyListItem(akte.DisplayText, akte.Id));
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadAktenAsync error: {ex.Message}");
                return new ObservableCollection<IListItem<int>>();
            }
        }

        /// <summary>
        /// Loads a single Akte and its associated GBE records.
        /// </summary>
        /// <param name="akteId">Numeric Akte ID.</param>
        /// <returns>Tuple of (HGAkteModel, List of GBEModel). Returns (null, empty list) if not found.</returns>
        public async Task<(HGAkteModel akte, List<GBEModel> gbes)> LoadAkteDetailsAsync(int akteId)
        {
            try
            {
                var akte = await _dataService.LoadAkteByIdAsync(akteId);
                if (akte == null)
                {
                    return (null, new List<GBEModel>());
                }

                var gbes = await _dataService.LoadGBEsForAkteAsync(akte.AkteNumber);
                akte.Grundbucheintraege = gbes;

                return (akte, gbes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadAkteDetailsAsync error: {ex.Message}");
                return (null, new List<GBEModel>());
            }
        }

        /// <summary>
        /// Loads GBEs as a collection of UI model (MyListItem) for ComboBox binding.
        /// </summary>
        /// <param name="akteNumber">Akte number to load GBEs for.</param>
        /// <returns>ObservableCollection of MyListItem for UI binding.</returns>
        public async Task<ObservableCollection<IListItem<int>>> LoadGBEsForAkteAsync(string akteNumber)
        {
            try
            {
                var items = new ObservableCollection<IListItem<int>>();
                if (string.IsNullOrWhiteSpace(akteNumber))
                {
                    return items;
                }

                var gbes = await _dataService.LoadGBEsForAkteAsync(akteNumber);
                foreach (var gbe in gbes)
                {
                    // Display text: "Jahr Name   [Extra space to reach column 200]  Nr"
                    string displayText = $"{gbe.Jahr} {gbe.Name}".Trim() + 
                        new string(' ', 240).Substring(0, 200) + 
                        gbe.Id.ToString();
                    items.Add(new MyListItem(displayText, gbe.Id));
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadGBEsForAkteAsync error: {ex.Message}");
                return new ObservableCollection<IListItem<int>>();
            }
        }

        /// <summary>
        /// Loads property usages (persons/entities linked to an Akte).
        /// </summary>
        /// <param name="akteNumber">Akte number to load usages for.</param>
        /// <returns>ObservableCollection of MyListItem with person info.</returns>
        public async Task<ObservableCollection<IListItem<int>>> LoadPropertyUsagesAsync(string akteNumber)
        {
            try
            {
                var items = new ObservableCollection<IListItem<int>>();
                if (string.IsNullOrWhiteSpace(akteNumber))
                {
                    return items;
                }

                var usages = await _dataService.LoadPropertyUsagesAsync(akteNumber);
                foreach (var usage in usages)
                {
                    // Resolve person name using Modul1
                    _modul1.PersInArb = usage.PersonId;
                    _modul1.Person_ReadNames(usage.PersonId, _modul1.Person);

                    string displayText = (_modul1.Person.SurName.Trim() + ", " + 
                        _modul1.Person.Givennames.Trim() + 
                        new string(' ', 240)).Substring(0, 200) + 
                        usage.PersonId.ToString();

                    items.Add(new MyListItem(displayText, usage.PersonId));
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadPropertyUsagesAsync error: {ex.Message}");
                return new ObservableCollection<IListItem<int>>();
            }
        }

        /// <summary>
        /// Searches Akten by pattern.
        /// </summary>
        /// <param name="searchPattern">Search pattern for Akte number.</param>
        /// <returns>ObservableCollection of MyListItem matching the pattern.</returns>
        public async Task<ObservableCollection<IListItem<int>>> SearchAktenAsync(string searchPattern)
        {
            try
            {
                var items = new ObservableCollection<IListItem<int>>();
                if (string.IsNullOrWhiteSpace(searchPattern))
                {
                    return items;
                }

                var results = await _dataService.SearchAktenAsync(searchPattern);
                foreach (var akte in results)
                {
                    items.Add(new MyListItem(akte.DisplayText, akte.Id));
                }

                return items;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchAktenAsync error: {ex.Message}");
                return new ObservableCollection<IListItem<int>>();
            }
        }

        /// <summary>
        /// Saves a new Akte and optionally links it to a GBE.
        /// </summary>
        /// <param name="akte">HGAkteModel to save.</param>
        /// <param name="gbeId">Optional GBE ID to link to.</param>
        /// <returns>The ID of the newly saved Akte.</returns>
        public async Task<int> SaveAkteWithGBEAsync(HGAkteModel akte, int gbeId = 0)
        {
            try
            {
                ValidateAkte(akte);

                // Get next ID if not assigned
                if (akte.Id <= 0)
                {
                    akte.Id = await _dataService.GetNextAkteIdAsync();
                }

                // Save Akte
                int akteId = await _dataService.SaveAkteAsync(akte);

                // If GBE ID provided, ensure it's linked to this Akte
                if (gbeId > 0)
                {
                    var gbe = await _dataService.LoadGBEByIdAsync(gbeId);
                    if (gbe != null)
                    {
                        gbe.AkteNumber = akte.AkteNumber;
                        await _dataService.UpdateGBEAsync(gbe);
                    }
                }

                return akteId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveAkteWithGBEAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing Akte.
        /// </summary>
        /// <param name="akte">HGAkteModel with updated values.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task UpdateAkteAsync(HGAkteModel akte)
        {
            try
            {
                ValidateAkte(akte);
                await _dataService.UpdateAkteAsync(akte);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAkteAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an Akte by its ID.
        /// </summary>
        /// <param name="akteId">Numeric Akte ID.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task DeleteAkteAsync(int akteId)
        {
            try
            {
                await _dataService.DeleteAkteAsync(akteId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAkteAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the next available Akte ID.
        /// </summary>
        /// <returns>The next numeric Akte ID.</returns>
        public async Task<int> GetNextAkteIdAsync()
        {
            try
            {
                return await _dataService.GetNextAkteIdAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetNextAkteIdAsync error: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Validates an Akte for persistence.
        /// </summary>
        /// <param name="akte">HGAkteModel to validate.</param>
        /// <exception cref="ArgumentException">Thrown if Akte is invalid.</exception>
        public void ValidateAkte(HGAkteModel akte)
        {
            if (akte == null)
            {
                throw new ArgumentException("Akte cannot be null.");
            }

            if (!akte.IsValid())
            {
                throw new ArgumentException("Akte number cannot be empty.");
            }
        }

        /// <summary>
        /// Saves a new GBE (Grundbucheintrag).
        /// </summary>
        /// <param name="gbe">GBEModel to save.</param>
        /// <returns>The ID of the newly saved GBE.</returns>
        public async Task<int> SaveGBEAsync(GBEModel gbe)
        {
            try
            {
                if (gbe == null || string.IsNullOrWhiteSpace(gbe.AkteNumber))
                {
                    throw new ArgumentException("GBE must be valid and linked to an Akte.");
                }

                if (gbe.Id <= 0)
                {
                    gbe.Id = await _dataService.GetNextAkteIdAsync(); // Reuse for GBE ID if needed
                }

                return await _dataService.SaveGBEAsync(gbe);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveGBEAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing GBE.
        /// </summary>
        /// <param name="gbe">GBEModel with updated values.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task UpdateGBEAsync(GBEModel gbe)
        {
            try
            {
                if (gbe == null || !gbe.IsValid())
                {
                    throw new ArgumentException("GBE must be valid before updating.");
                }

                await _dataService.UpdateGBEAsync(gbe);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateGBEAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a GBE by its ID.
        /// </summary>
        /// <param name="gbeId">Numeric GBE ID.</param>
        /// <returns>Task representing the operation.</returns>
        public async Task DeleteGBEAsync(int gbeId)
        {
            try
            {
                await _dataService.DeleteGBEAsync(gbeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteGBEAsync error: {ex.Message}");
                throw;
            }
        }
    }
}
