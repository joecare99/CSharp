// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="LicensePersistenceService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Data access service for license persistence, wraps IModul1.Persistence</summary>
// ***********************************************************************

using GenFreeWin.Services.Interfaces;
using GenFree.Interfaces.Sys;
using System;

namespace GenFreeWin.Services
{
    /// <summary>
    /// Service for managing license data persistence operations.
    /// Provides an abstraction layer over IModul1.Persistence for license-specific operations.
    /// </summary>
    public class LicensePersistenceService : ILicensePersistenceService
    {
        private readonly IModul1 _modul1;
        private const string LicenseFileName = "IDF.Dat";
        private const string AddressFileName = "adress.dat";
        private const int AddressFileLineIndex = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePersistenceService"/> class.
        /// </summary>
        /// <param name="modul1">The module 1 interface providing persistence access.</param>
        public LicensePersistenceService(IModul1 modul1)
        {
            _modul1 = modul1 ?? throw new ArgumentNullException(nameof(modul1));
        }

        /// <summary>
        /// Persists the license serial number to storage.
        /// </summary>
        /// <param name="serialNumber">The full license serial number (e.g., "LicText1-GB-LicText2-LicText3").</param>
        public void SaveLicenseSerialNumber(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                throw new ArgumentException("Serial number cannot be null or empty.", nameof(serialNumber));
            }

            try
            {
                _modul1.Persistence.WriteStringProg(LicenseFileName, serialNumber);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save license serial number to {LicenseFileName}.", ex);
            }
        }

        /// <summary>
        /// Retrieves the stored license serial number from persistent storage.
        /// </summary>
        /// <returns>The license serial number if found; otherwise an empty string.</returns>
        public string LoadLicenseSerialNumber()
        {
            try
            {
                var result = _modul1.Persistence.ReadStringProg(LicenseFileName);
                return result ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Retrieves the owner/address information from persistent storage.
        /// </summary>
        /// <returns>The owner information if found; otherwise an empty string.</returns>
        public string LoadOwnerInformation()
        {
            try
            {
                var result = _modul1.Persistence.ReadStringMLProg(AddressFileName, AddressFileLineIndex);
                return result ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if a license is currently registered in the system.
        /// </summary>
        /// <returns>True if a license serial number is stored; otherwise false.</returns>
        public bool IsLicenseRegistered()
        {
            string serialNumber = LoadLicenseSerialNumber();
            return !string.IsNullOrWhiteSpace(serialNumber);
        }

        /// <summary>
        /// Sets the system state to indicate successful license verification.
        /// </summary>
        public void ActivateLicense()
        {
            try
            {
                _modul1.System.xDemo = false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to activate license in system.", ex);
            }
        }

        /// <summary>
        /// Sets the system state to indicate demo/unlicensed mode.
        /// </summary>
        public void DeactivateLicense()
        {
            try
            {
                _modul1.System.xDemo = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deactivate license in system.", ex);
            }
        }
    }
}
