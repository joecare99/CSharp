// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="LicenseViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>ViewModel for License dialog with business logic and state management</summary>
// ***********************************************************************

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeWin.Models;
using GenFreeWin.Services;
using GenFreeWin.Services.Interfaces;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using MVVM.ViewModel;
using System;

namespace GenFreeWin.ViewModels
{
    /// <summary>
    /// ViewModel for the license dialog.
    /// Handles validation, verification, and persistence of license data.
    /// </summary>
    public partial class LicenseViewModel : BaseViewModelCT, ILizenzViewModel
    {
        private IInteraction _interaction;
        private readonly IProjectData _projectData;
        private readonly ILicensePersistenceService _persistenceService;

        public IInteraction Interaction { set => _interaction = value; }

        public Action? DoClose { get; set; }
        public Action? DoEndProg { get; set; }

        /// <summary>
        /// Gets or sets the product identifier (10 digits).
        /// </summary>
        [ObservableProperty]
        public partial string LicText1 { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer identifier (10 digits).
        /// </summary>
        [ObservableProperty]
        public partial string LicText2 { get; set; }

        /// <summary>
        /// Gets or sets the license key (at least 5 digits).
        /// </summary>
        [ObservableProperty]
        public partial string LicText3 { get; set; }

        /// <summary>
        /// Gets the number of verification attempts made.
        /// </summary>
        [ObservableProperty]
        public partial short AttemptCounter { get; set; }

        /// <summary>
        /// Gets a value indicating whether verification is in progress.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(VerifyCommand))]
        public partial bool IsVerificationInProgress { get; set; }

        /// <summary>
        /// Gets a value indicating whether the license has been verified successfully.
        /// </summary>
        [ObservableProperty]
        public partial bool IsVerified { get; set; }

        /// <summary>
        /// Gets a value indicating whether the license has been verified successfully.
        /// </summary>
        [ObservableProperty]
        public partial bool DisplayHintVisible { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseViewModel"/> class.
        /// </summary>
        public LicenseViewModel(IModul1 modul1, IInteraction interaction, IProjectData projectData, IStrings strings)
        {
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _projectData = projectData ?? throw new ArgumentNullException(nameof(projectData));
            _persistenceService = new LicensePersistenceService(modul1);
        }


        [RelayCommand]
        private void ReqHint()
        {
            DisplayHintVisible = true;
        }

        [RelayCommand(CanExecute = nameof(IsVerificationInProgress))]
        private void Verify()
        {
            if (AreAllFieldsComplete())
            {
                if (VerifyLicense())
                {
                    DoClose?.Invoke();
                }
            }
            else
            {
                _interaction.MsgBox("Bitte alle Felder korrekt ausfüllen!", title: "Fehler");
            }
        }

        /// <summary>
        /// Verifies the license and persists it if valid.
        /// </summary>
        /// <returns>True if verification was successful; otherwise false.</returns>
        public bool VerifyLicense()
        {
            IsVerificationInProgress = true;

            try
            {
                AttemptCounter++;

                // Create license data model
                var licenseData = new LicenseData
                {
                    ProductId = LicText1,
                    ManufacturerId = LicText2,
                    LicenseKey = LicText3
                };

                // Validate license format and checksum
                if (!licenseData.IsValid())
                {
                    if (AttemptCounter > 4)
                    {
                        _interaction.MsgBox("Sie hatten vier Versuche die Lizenz-Nr. einzugeben. Das Programm wird beendet!");
                        _projectData.EndApp();
                        return false;
                    }

                    // Show retry dialog
                    var result = _interaction.MsgBox(
                        "Die eingegebene Lizenz-Nr. ist falsch",
                        title: "Versuch " + AttemptCounter.ToString(),
                        mb: System.Windows.Forms.MessageBoxButtons.RetryCancel);

                    if ((int)result == 2) // Cancel
                    {
                        _projectData.EndApp();
                        return false;
                    }

                    return false;
                }

                // License is valid - persist it
                PersistLicense(licenseData);
                IsVerified = true;
                return true;
            }
            finally
            {
                IsVerificationInProgress = false;
            }
        }

        /// <summary>
        /// Persists the verified license to storage.
        /// </summary>
        private void PersistLicense(LicenseData licenseData)
        {
            try
            {
                // Write license serial number
                _persistenceService.SaveLicenseSerialNumber(licenseData.SerialNumber);

                // Update system state
                _persistenceService.ActivateLicense();

                // Read and set owner information
                string ownerInfo = _persistenceService.LoadOwnerInformation();
                if (string.IsNullOrWhiteSpace(ownerInfo))
                {
                    ownerInfo = "Adresse eingeben";
                }

                // Update menu button state (if accessible via static reference)
                // Note: This should ideally be refactored to use dependency injection
                if (Menue.Default != null)
                {
                    Menue.Default.btnEnterLizenz.Visible = false;
                    Menue.Default.lblOwner.Text = ownerInfo.Trim();
                }
            }
            catch (Exception ex)
            {
                // Log error and show message
                _interaction.MsgBox($"Fehler beim Speichern der Lizenz: {ex.Message}", title: "Fehler");
                throw;
            }
        }

        /// <summary>
        /// Checks if all required fields are filled with correct length.
        /// </summary>
        /// <returns>True if all fields are complete; otherwise false.</returns>
        public bool AreAllFieldsComplete()
        {
            return !string.IsNullOrEmpty(LicText1) && LicText1.Length == 10 &&
                   !string.IsNullOrEmpty(LicText2) && LicText2.Length == 10 &&
                   !string.IsNullOrEmpty(LicText3) && LicText3.Length >= 5;
        }

        /// <summary>
        /// Resets the verification state for retry.
        /// </summary>
        public void ResetForRetry()
        {
            LicText1 = string.Empty;
            LicText2 = string.Empty;
            LicText3 = string.Empty;
            IsVerified = false;
        }

        [RelayCommand]
        private void Cancel()
        {
            DoClose?.Invoke();
        }
    }
}


