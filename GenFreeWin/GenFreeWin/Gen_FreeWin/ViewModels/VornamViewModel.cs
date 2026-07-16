// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="VornamViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Modernized thin MVVM ViewModel for Vorname (given name) management with RelayCommands</summary>
// ***********************************************************************

using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeWin.Models;
using GenFreeWin.Services.Interfaces;
using GenFreeWin.UseCases;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenFreeWin.ViewModels
{
    /// <summary>
    /// Modernized thin MVVM ViewModel for Vorname (given name) management.
    /// Delegates business logic to VornamSearchUseCase and data persistence to IVornamDataService.
    /// Uses CommunityToolkit.Mvvm [RelayCommand] pattern for command binding.
    /// </summary>
    public partial class VornamViewModel : ObservableObject, IVornamViewModel
    {

        // ============================================================================
        // MVVM Observable Properties (bindable via CommunityToolkit.Mvvm)
        // Pattern (from copilot-instructions.md): 
        // [ObservableProperty] public partial Type PropertyName { get; set; } = defaultValue;
        // ============================================================================

        [ObservableProperty]
        public partial ObservableCollection<IListItem<int>> SearchResults { get; set; } = new();

        [ObservableProperty]
        public partial ObservableCollection<VornamModel> CurrentNames { get; set; } = new();

        [ObservableProperty]
        public partial string SearchPattern { get; set; } = "";

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

        [ObservableProperty]
        public partial string StatusMessage { get; set; } = "";

        // ========================================================================
        // Form UI State Properties (for MVVM binding, not direct View access)
        // ========================================================================

        /// <summary>
        /// Form heading text (replaces View.lblHeading.Text).
        /// </summary>
        [ObservableProperty]
        public partial string FormHeading { get; set; } = "";

        /// <summary>
        /// Form background color (replaces View.BackColor).
        /// Represented as int (ColorArgb) for serialization safety.
        /// </summary>
        [ObservableProperty]
        public partial int FormBackColor { get; set; } = unchecked((int)0xFFFFFFFF); // White

        /// <summary>
        /// Font size for form controls (replaces View.Font).
        /// </summary>
        [ObservableProperty]
        public partial float FormFontSize { get; set; } = 10f;

        /// <summary>
        /// Font family name for form controls.
        /// </summary>
        [ObservableProperty]
        public partial string FormFontName { get; set; } = "Arial";

        /// <summary>
        /// Observable collection of name field view models (15 fields, lines 1-15).
        /// Replaces direct access to View.Text_Renamed[i].
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<NameFieldViewModel> NameFieldsTyped { get; set; } = new();

        /// <summary>
        /// IVornamViewModel.NameFields implementation (typed as object for interface compatibility).
        /// Internal proxy to NameFieldsTyped.
        /// </summary>
        ObservableCollection<object> IVornamViewModel.NameFields
        {
            get => new ObservableCollection<object>(NameFieldsTyped.Cast<object>());
            set => NameFieldsTyped = new ObservableCollection<NameFieldViewModel>(value.Cast<NameFieldViewModel>());
        }

        /// <summary>
        /// Signal for View to close (replaces View?.Close()).
        /// Set to true when form should be closed from ViewModel.
        /// </summary>
        [ObservableProperty]
        public partial bool RequestClose { get; set; }

        /// <summary>
        /// Current active name field index for search result selection.
        /// Replaces _currentNameIndex tracking.
        /// </summary>
        [ObservableProperty]
        public partial int CurrentFieldIndex { get; set; } = -1;

        // ============================================================================
        // Private Fields & Dependencies
        // ============================================================================

        private readonly IVornamDataService _dataService;
        private readonly VornamSearchUseCase _searchUseCase;

        // Legacy Modul1 references
        private IModul1 Modul1 => _Modul1.Instance;

        // State tracking
        private int _currentPersonId;
        private ETextKennz _currentTextKennz;
        private int _currentNameIndex;

        // ============================================================================
        // Constructor
        // ============================================================================

        /// <summary>
        /// Creates a new VornamViewModel instance with dependency injection.
        /// </summary>
        /// <param name="dataService">Data access service for names</param>
        /// <param name="searchUseCase">Business logic orchestrator for name operations</param>
        public VornamViewModel(IVornamDataService dataService, VornamSearchUseCase searchUseCase)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _searchUseCase = searchUseCase ?? throw new ArgumentNullException(nameof(searchUseCase));

            SearchResults = new ObservableCollection<IListItem<int>>();
            CurrentNames = new ObservableCollection<VornamModel>();

            // Initialize 15 name field view models (lines 1-15)
            NameFieldsTyped = new ObservableCollection<NameFieldViewModel>();
            for (int i = 1; i <= 15; i++)
            {
                NameFieldsTyped.Add(new NameFieldViewModel(i));
            }
        }

        // ============================================================================
        // MVVM Relay Commands
        // (Use [CommandBinding] attribute on WinForms controls for declarative binding)
        // ============================================================================

        /// <summary>
        /// Async relay command: Loads names for the current person/gender on form load.
        /// MVVM pure: Sets Observable Properties instead of directly accessing View controls.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.LoadNamesCommand))]
        /// </summary>
        [RelayCommand]
        public async Task LoadNames()
        {
            try
            {
                // Set form UI state via Observable Properties (not View.Font, View.Text, etc.)
                if (Modul1.FontSize > 0f)
                {
                    FormFontSize = Modul1.FontSize;
                    FormFontName = "Arial";
                }

                FormHeading = "Vorname                                                            Rufname-Marker                 Leitname";
                FormBackColor = Modul1.HintFarb.ToArgb();

                // Determine current person and gender
                _currentPersonId = Personen.Instance.PersonNr;
                _currentTextKennz = Personen.Instance.edtSex.Text.Trim().ToUpper() == "F" ? ETextKennz.F_ : ETextKennz.V_;

                IsLoading = true;
                var names = await _searchUseCase.LoadNamesAsync(_currentPersonId, _currentTextKennz);

                // Update CurrentNames collection and refresh UI state
                CurrentNames.Clear();
                foreach (var name in names)
                {
                    CurrentNames.Add(name);
                }

                // Populate NameFields from CurrentNames
                RefreshNameFieldsFromCurrentNames();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadNames error: {ex.Message}");
                StatusMessage = $"Error loading names: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Async relay command: Searches for names matching user input pattern.
        /// Invoked on TextChanged event via command binding.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.SearchNamesCommand))]
        /// </summary>
        /// <param name="searchText">Search pattern to filter names</param>
        [RelayCommand]
        public async Task SearchNames(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SearchResults.Clear();
                return;
            }

            try
            {
                SearchPattern = searchText;
                var results = await _searchUseCase.SearchNamesAsync(_currentTextKennz, searchText.Trim());
                SearchResults = results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SearchNames error: {ex.Message}");
            }
        }

        /// <summary>
        /// Async relay command: Selects a search result and populates the name field with full data.
        /// MVVM pure: Updates NameFields Observable Collection instead of View.Text_Renamed directly.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.SelectSearchResultCommand))]
        /// </summary>
        /// <param name="selectedItem">The IListItem&lt;int&gt; selected from dropdown</param>
        [RelayCommand]
        public async Task SelectSearchResult(IListItem<int> selectedItem)
        {
            if (selectedItem == null)
                return;

            try
            {
                // Get full text/lead name info
                var textInfo = await _searchUseCase.GetTextByIdAsync(selectedItem.ItemData);
                if (textInfo.HasValue && CurrentFieldIndex >= 0 && CurrentFieldIndex < NameFieldsTyped.Count)
                {
                    // Update current name field via Observable NameFieldViewModel
                    var field = NameFieldsTyped[CurrentFieldIndex];
                    field.PrimaryName = textInfo.Value.Text;
                    field.Synonym = textInfo.Value.LeadName;
                    field.MarkModified();

                    SearchResults.Clear();
                    SearchPattern = "";
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Async relay command: Saves all edited names for the current person to the database.
        /// MVVM pure: Reads from NameFields Observable Properties instead of View.Text_Renamed directly.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.SaveAllNamesCommand))]
        /// </summary>
        [RelayCommand]
        public async Task SaveAllNames()
        {
            try
            {
                IsLoading = true;

                // Collect names from NameFieldsTyped ObservableCollection
                var names = new List<VornamModel>();
                for (int i = 0; i < NameFieldsTyped.Count; i++)
                {
                    var field = NameFieldsTyped[i];
                    if (!field.IsEmpty)
                    {
                        var name = new VornamModel
                        {
                            PersonId = _currentPersonId,
                            PrimaryName = field.PrimaryName.Left(240).TrimEnd(),
                            Synonym = field.Synonym.Left(240).TrimEnd(),
                            TextKennz = _currentTextKennz,
                            LineNumber = (short)(i + 1),
                            IsCalledName = false,
                            IsNickname = false
                        };

                        var (isValid, error) = _searchUseCase.ValidateName(name);
                        if (isValid)
                            names.Add(name);
                    }
                }

                // Save batch
                var (success, savedCount, saveError) = await _searchUseCase.SaveBatchNamesAsync(_currentPersonId, _currentTextKennz, names);

                StatusMessage = success ? $"Saved {savedCount} names" : $"Error: {saveError}";

                if (success)
                {
                    // Refresh and reload ancestor data (still accesses legacy globals – to be refactored separately)
                    Modul1.Person_ReadNames(_currentPersonId, Modul1.Person);
                    var ancestorData = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out string _);
                    Modul1.Kont[10] = ancestorData;
                    Modul1.Kont[97] = iAhn.AsString();
                    Personen.Instance.edtGivennames.Text = Modul1.Person.Givennames;

                    // Show duplicates if configured
                    if (Modul1.Aus[24].AsInt() == 1)
                    {
                        Personen.Instance.frmDublicates.Width = Personen.Instance.Width - 20;
                        Personen.Instance.lstDuplicates.Width = Personen.Instance.frmDublicates.Width - 40;
                        Personen.Instance.frmDublicates.Visible = true;
                        Personen.Instance.frmDublicates.Location = new Point(0, 166);
                        Personen.Instance.Zeigfam(Personen.Instance.edtSurnames.Text.Trim() + "," + Personen.Instance.edtGivennames.Text.Trim());
                    }

                    // Signal to View to close (instead of View?.Close())
                    RequestClose = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"SaveAllNames error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Sync relay command: Cancels edit mode and closes the form.
        /// MVVM pure: Clears NameFields and signals RequestClose instead of View?.Close() or Focus().
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.CancelEditCommand))]
        /// </summary>
        [RelayCommand]
        public void CancelEdit()
        {
            try
            {
                // Clear all name fields
                foreach (var field in NameFieldsTyped)
                {
                    field.Clear();
                }

                // Signal to View to close (instead of View?.Close())
                RequestClose = true;

                // Note: Focus() would require View access; that's handled in View code-behind
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CancelEdit error: {ex.Message}");
            }
        }

        /// <summary>
        /// Sync relay command: Completes edit mode and closes the form.
        /// MVVM pure: Clears NameFields and signals RequestClose instead of View?.Close() or Focus().
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.DoneEditCommand))]
        /// </summary>
        [RelayCommand]
        public void DoneEdit()
        {
            SaveAllNames().GetAwaiter();
        }

        /// <summary>
        /// Helper method: Populates NameFieldsTyped from CurrentNames collection.
        /// Synchronizes loaded VornamModel items into observable name field ViewModels.
        /// </summary>
        private void RefreshNameFieldsFromCurrentNames()
        {
            NameFieldsTyped.Clear();
            for (int i = 1; i <= 15; i++)
            {
                NameFieldsTyped.Add(new NameFieldViewModel(i));
            }

            // Populate from CurrentNames if available
            if (CurrentNames != null && CurrentNames.Count > 0)
            {
                int index = 0;
                foreach (var nameModel in CurrentNames.OfType<VornamModel>())
                {
                    if (index < NameFieldsTyped.Count)
                    {
                        NameFieldsTyped[index].PrimaryName = nameModel.PrimaryName ?? "";
                        NameFieldsTyped[index].Synonym = nameModel.Synonym ?? "";
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Async relay command: Deletes all names of the current gender/person.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.DeleteAllNamesCommand))]
        /// </summary>
        [RelayCommand]
        public async Task DeleteAllNames()
        {
            try
            {
                var result = MessageBox.Show(
                    "Delete all names for this gender?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    var (success, deletedCount, error) = await _searchUseCase.DeleteNamesByKindAsync(_currentPersonId, _currentTextKennz);
                    StatusMessage = success ? $"Deleted {deletedCount} names" : $"Error: {error}";

                    if (success)
                    {
                        CurrentNames.Clear();
                        NameFieldsTyped.Clear();
                        RefreshNameFieldsFromCurrentNames();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAllNames error: {ex.Message}");
            }
        }

        // ============================================================================
        // Interface Property Exposures (IVornamViewModel contract)
        // Explicit interface implementation for command/property binding
        // ============================================================================

        /// <summary>
        /// Interface exposure for SearchResults (get/set for binding).
        /// </summary>
        ObservableCollection<IListItem<int>> IVornamViewModel.SearchResults
        {
            get => SearchResults;
            set => SearchResults = value;
        }

        /// <summary>
        /// Interface exposure for CurrentNames (as object collection for cross-assembly safety).
        /// Internal type: ObservableCollection&lt;VornamModel&gt;
        /// </summary>
        ObservableCollection<object> IVornamViewModel.CurrentNames
        {
            get => new ObservableCollection<object>(CurrentNames.Cast<object>());
            set
            {
                CurrentNames.Clear();
                foreach (var item in value.OfType<VornamModel>())
                {
                    CurrentNames.Add(item);
                }
            }
        }

        // ============================================================================
        // Legacy Event Handlers (Backward Compatibility)
        // Prefer RelayCommands for new code; these are fallback implementations.
        // ============================================================================

        /// <summary>
        /// Legacy event handler: Form load. Use LoadNamesCommand via [CommandBinding] instead.
        /// </summary>
        public void Form_Load(object sender, EventArgs e)
        {
            try
            {
                var cmd = ((IVornamViewModel)this).LoadNamesCommand;
                if (cmd?.CanExecute(null) == true)
                {
                    _ = cmd.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form_Load error: {ex.Message}");
            }
        }

    }
}
