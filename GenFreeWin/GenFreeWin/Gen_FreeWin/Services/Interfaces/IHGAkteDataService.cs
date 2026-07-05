// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IHGAkteDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Data access contract for HGakte (Grundbuchakte) operations</summary>
// ***********************************************************************

using Gen_FreeWin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services.Interfaces
{
    /// <summary>
    /// Async contract for HGakte (Grundbuchakte / land register) data access.
    /// Abstracts all direct DataModul interactions.
    /// </summary>
    public interface IHGAkteDataService
    {
        /// <summary>
        /// Loads all Grundbuchakten in a single operation.
        /// </summary>
        /// <returns>List of all HGAkteModel records.</returns>
        Task<List<HGAkteModel>> LoadAllAktenAsync();

        /// <summary>
        /// Loads a single Akte by its numeric ID.
        /// </summary>
        /// <param name="akteId">Numeric Akte ID.</param>
        /// <returns>HGAkteModel if found, null otherwise.</returns>
        Task<HGAkteModel> LoadAkteByIdAsync(int akteId);

        /// <summary>
        /// Loads a single Akte by its Akte number (string identifier).
        /// </summary>
        /// <param name="akteNumber">Akte number (user-facing ID).</param>
        /// <returns>HGAkteModel if found, null otherwise.</returns>
        Task<HGAkteModel> LoadAkteByNumberAsync(string akteNumber);

        /// <summary>
        /// Saves a new Akte to the database.
        /// </summary>
        /// <param name="akte">HGAkteModel to save.</param>
        /// <returns>The ID of the newly saved Akte.</returns>
        Task<int> SaveAkteAsync(HGAkteModel akte);

        /// <summary>
        /// Updates an existing Akte record.
        /// </summary>
        /// <param name="akte">HGAkteModel with updated values.</param>
        /// <returns>Task representing the async operation.</returns>
        Task UpdateAkteAsync(HGAkteModel akte);

        /// <summary>
        /// Deletes an Akte by its ID.
        /// </summary>
        /// <param name="akteId">Numeric Akte ID.</param>
        /// <returns>Task representing the async operation.</returns>
        Task DeleteAkteAsync(int akteId);

        /// <summary>
        /// Gets the next available Akte ID (for creating new Akten).
        /// </summary>
        /// <returns>The next numeric Akte ID.</returns>
        Task<int> GetNextAkteIdAsync();

        /// <summary>
        /// Loads all Grundbucheinträge (GBE) for a given Akte.
        /// </summary>
        /// <param name="akteNumber">Akte number to search by.</param>
        /// <returns>List of GBEModel records for this Akte.</returns>
        Task<List<GBEModel>> LoadGBEsForAkteAsync(string akteNumber);

        /// <summary>
        /// Loads a single GBE by its numeric ID.
        /// </summary>
        /// <param name="gbeId">Numeric GBE ID.</param>
        /// <returns>GBEModel if found, null otherwise.</returns>
        Task<GBEModel> LoadGBEByIdAsync(int gbeId);

        /// <summary>
        /// Saves a new GBE record.
        /// </summary>
        /// <param name="gbe">GBEModel to save.</param>
        /// <returns>The ID of the newly saved GBE.</returns>
        Task<int> SaveGBEAsync(GBEModel gbe);

        /// <summary>
        /// Updates an existing GBE record.
        /// </summary>
        /// <param name="gbe">GBEModel with updated values.</param>
        /// <returns>Task representing the async operation.</returns>
        Task UpdateGBEAsync(GBEModel gbe);

        /// <summary>
        /// Deletes a GBE by its ID.
        /// </summary>
        /// <param name="gbeId">Numeric GBE ID.</param>
        /// <returns>Task representing the async operation.</returns>
        Task DeleteGBEAsync(int gbeId);

        /// <summary>
        /// Loads all properties/persons using a specific Akte.
        /// </summary>
        /// <param name="akteNumber">Akte number to search by.</param>
        /// <returns>List of PropertyUsageModel records.</returns>
        Task<List<PropertyUsageModel>> LoadPropertyUsagesAsync(string akteNumber);

        /// <summary>
        /// Searches Akten by Akte number pattern (partial match).
        /// </summary>
        /// <param name="searchPattern">Pattern to search (case-insensitive).</param>
        /// <returns>List of matching HGAkteModel records.</returns>
        Task<List<HGAkteModel>> SearchAktenAsync(string searchPattern);
    }
}
