// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="ILicensePersistenceService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Interface for license persistence operations</summary>
// ***********************************************************************

namespace Gen_FreeWin.Services.Interfaces
{
    /// <summary>
    /// Interface for managing license data persistence operations.
    /// </summary>
    public interface ILicensePersistenceService
    {
        /// <summary>
        /// Persists the license serial number to storage.
        /// </summary>
        /// <param name="serialNumber">The full license serial number.</param>
        void SaveLicenseSerialNumber(string serialNumber);

        /// <summary>
        /// Retrieves the stored license serial number from persistent storage.
        /// </summary>
        /// <returns>The license serial number if found; otherwise an empty string.</returns>
        string LoadLicenseSerialNumber();

        /// <summary>
        /// Retrieves the owner/address information from persistent storage.
        /// </summary>
        /// <returns>The owner information if found; otherwise an empty string.</returns>
        string LoadOwnerInformation();

        /// <summary>
        /// Checks if a license is currently registered in the system.
        /// </summary>
        /// <returns>True if a license serial number is stored; otherwise false.</returns>
        bool IsLicenseRegistered();

        /// <summary>
        /// Sets the system state to indicate successful license verification.
        /// </summary>
        void ActivateLicense();

        /// <summary>
        /// Sets the system state to indicate demo/unlicensed mode.
        /// </summary>
        void DeactivateLicense();
    }
}
