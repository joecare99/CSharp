// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="IVornamDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Async data-access contract for Vorname (given name) operations</summary>
// ***********************************************************************

using Gen_FreeWin.Models;
using GenFree.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services.Interfaces
{
    /// <summary>
    /// Async data-access contract for name/Vorname persistence and retrieval.
    /// Abstracts all direct DataModul interactions for name operations.
    /// </summary>
    public interface IVornamDataService
    {
        /// <summary>
        /// Loads all names for a given person by gender/text kind.
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <param name="textKennz">The text kind (e.g., ETextKennz.F_ for female, ETextKennz.V_ for male).</param>
        /// <returns>List of VornamModel entries for the person.</returns>
        Task<List<VornamModel>> LoadNamesForPersonAsync(int personId, ETextKennz textKennz);

        /// <summary>
        /// Saves a new name entry to the database.
        /// </summary>
        /// <param name="vorname">The name model to save.</param>
        /// <returns>The sequence number (line number) of the saved name.</returns>
        Task<short> SaveNameAsync(VornamModel vorname);

        /// <summary>
        /// Updates an existing name entry.
        /// </summary>
        /// <param name="vorname">The updated name model.</param>
        /// <returns>True if update succeeded; otherwise, false.</returns>
        Task<bool> UpdateNameAsync(VornamModel vorname);

        /// <summary>
        /// Deletes all names of a given kind from a person.
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <param name="textKennz">The text kind to delete.</param>
        /// <returns>Number of names deleted.</returns>
        Task<int> DeleteNamesByKindAsync(int personId, ETextKennz textKennz);

        /// <summary>
        /// Searches for names matching a search pattern (for autocomplete/dropdown).
        /// </summary>
        /// <param name="textKennz">The text kind (gender).</param>
        /// <param name="searchPattern">The partial name to search for.</param>
        /// <returns>Observable collection of IListItem for UI binding.</returns>
        Task<ObservableCollection<GenFree.Helper.IListItem<int>>> SearchNamesAsync(ETextKennz textKennz, string searchPattern);

        /// <summary>
        /// Retrieves a single name by ID (if ID concept exists).
        /// </summary>
        /// <param name="personId">The person ID.</param>
        /// <param name="lineNumber">The line number of the name.</param>
        /// <returns>VornamModel if found; otherwise, null.</returns>
        Task<VornamModel?> GetNameByLineAsync(int personId, short lineNumber);

        /// <summary>
        /// Retrieves text/name information by text ID (for lookups).
        /// </summary>
        /// <param name="textId">The text ID to look up.</param>
        /// <returns>Tuple of (text, leadname) or null if not found.</returns>
        Task<(string Text, string LeadName)?> GetTextByIdAsync(int textId);
    }
}
