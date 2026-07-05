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
using Gen_FreeWin.Main;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using Gen_FreeWin.UseCases;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Input;

namespace Gen_FreeWin.ViewModels
{
    /// <summary>
    /// Modernized thin MVVM ViewModel for Vorname (given name) management.
    /// Delegates business logic to VornamSearchUseCase and data persistence to IVornamDataService.
    /// Uses CommunityToolkit.Mvvm [RelayCommand] pattern for command binding.
    /// </summary>
    public partial class VornamViewModel : ObservableObject, IVornamViewModel
    {
        Form IVornamViewModel.View { get; set; }
        public Vornam View => (Vornam)((IVornamViewModel)this).View;

        // ============================================================================
        // MVVM Observable Properties (bindable via CommunityToolkit.Mvvm)
        // ============================================================================

        [ObservableProperty]
        private ObservableCollection<IListItem<int>> searchResults = new();

        [ObservableProperty]
        private ObservableCollection<VornamModel> currentNames = new();

        [ObservableProperty]
        private string searchPattern = "";

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string statusMessage = "";

        // ============================================================================
        // Private Fields & Dependencies
        // ============================================================================

        private readonly IVornamDataService _dataService;
        private readonly VornamSearchUseCase _searchUseCase;

        // Legacy Modul1 references
        private IModul1 Modul1 => _Modul1.Instance;
        private IInteraction Interaction => Menue.Default;
        private IVBConversions Conversion => Modul1.Conversions;
        private IProjectData ProjectData => Modul1.ProjectData;

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
        }

        // ============================================================================
        // MVVM Relay Commands
        // (Use [CommandBinding] attribute on WinForms controls for declarative binding)
        // ============================================================================

        /// <summary>
        /// Async relay command: Loads names for the current person/gender on form load.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.LoadNamesCommand))]
        /// </summary>
        [RelayCommand]
        public async Task LoadNames()
        {
            try
            {
                if (Modul1.FontSize > 0f)
                {
                    View.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                }

                View.lblHeading.Text = "Vorname                                                            Rufname-Marker                 Leitname";
                View.BackColor = Modul1.HintFarb;

                _currentPersonId = Personen.Default.PersonNr;
                _currentTextKennz = Personen.Default.edtSex.Text.Trim().ToUpper() == "F" ? ETextKennz.F_ : ETextKennz.V_;

                IsLoading = true;
                var names = await _searchUseCase.LoadNamesAsync(_currentPersonId, _currentTextKennz);

                // Update UI on main thread
                View?.Invoke((MethodInvoker)(() =>
                {
                    CurrentNames.Clear();
                    foreach (var name in names)
                    {
                        CurrentNames.Add(name);
                    }
                    RefreshUI();
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadNames error: {ex.Message}");
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
                if (textInfo.HasValue)
                {
                    // Update current name input fields
                    if (_currentNameIndex > 0)
                    {
                        View.Text_Renamed[_currentNameIndex].Text = textInfo.Value.Text;
                        View.Text_Renamed[_currentNameIndex + 50].Text = textInfo.Value.LeadName;
                    }

                    SearchResults.Clear();
                    SearchPattern = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SelectSearchResult error: {ex.Message}");
            }
        }

        /// <summary>
        /// Async relay command: Saves all edited names for the current person to the database.
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.SaveAllNamesCommand))]
        /// </summary>
        [RelayCommand]
        public async Task SaveAllNames()
        {
            try
            {
                IsLoading = true;

                // Collect names from form fields
                var names = new List<VornamModel>();
                for (int i = 1; i <= 15; i++)
                {
                    if (!string.IsNullOrWhiteSpace(View.Text_Renamed[i].Text))
                    {
                        var name = new VornamModel
                        {
                            PersonId = _currentPersonId,
                            PrimaryName = View.Text_Renamed[i].Text.Left(240).TrimEnd(),
                            Synonym = View.Text_Renamed[i + 50].Text.Left(240).TrimEnd(),
                            TextKennz = _currentTextKennz,
                            LineNumber = (short)i,
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
                    // Refresh and reload ancestor data
                    Modul1.Person_ReadNames(_currentPersonId, Modul1.Person);
                    var ancestorData = Modul1.Ancesters_GetPersonData(Modul1.Person.ID, out int iAhn, out string _);
                    Modul1.Kont[10] = ancestorData;
                    Modul1.Kont[97] = iAhn.AsString();
                    Personen.Default.edtGivennames.Text = Modul1.Person.Givennames;

                    // Show duplicates if configured
                    if (Conversion.Val(Modul1.Aus[24].AsInt().AsString()) == 1.0)
                    {
                        Personen.Default.frmDublicates.Width = Personen.Default.Width - 20;
                        Personen.Default.lstDuplicates.Width = Personen.Default.frmDublicates.Width - 40;
                        Personen.Default.frmDublicates.Visible = true;
                        Personen.Default.frmDublicates.Location = new Point(0, 166);
                        Personen.Default.Zeigfam(Personen.Default.edtSurnames.Text.Trim() + "," + Personen.Default.edtGivennames.Text.Trim());
                    }

                    View?.Close();
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
        /// Usage in view: [CommandBinding(nameof(IVornamViewModel.CancelEditCommand))]
        /// </summary>
        [RelayCommand]
        public void CancelEdit()
        {
            try
            {
                // Clear form fields
                for (int i = 1; i <= 65; i++)
                {
                    View.Text_Renamed[i].Text = "";
                }
                View.Close();
                Personen.Default.edtAlias.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CancelEdit error: {ex.Message}");
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
                        RefreshUI();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAllNames error: {ex.Message}");
            }
        }

        // ============================================================================
        // Interface Property Exposures (for IVornamViewModel contract)
        // Explicit interface implementation to satisfy IVornamViewModel with settable properties
        // CurrentNames is exposed as ObservableCollection<object> in the interface
        // but internally managed as ObservableCollection<VornamModel>
        // ============================================================================

        /// <summary>
        /// Explicit interface property for SearchResults (supports both get and set).
        /// </summary>
        ObservableCollection<IListItem<int>> IVornamViewModel.SearchResults
        {
            get => SearchResults;
            set => SearchResults = value;
        }

        /// <summary>
        /// Explicit interface property for CurrentNames (supports both get and set).
        /// Exposes as object collection to avoid cross-assembly model dependency;
        /// runtime items are Gen_FreeWin.Models.VornamModel.
        /// </summary>
        ObservableCollection<object> IVornamViewModel.CurrentNames
        {
            get => new ObservableCollection<object>(CurrentNames.Cast<object>());
            set
            {
                CurrentNames.Clear();
                foreach (var item in value)
                {
                    if (item is VornamModel model)
                        CurrentNames.Add(model);
                }
            }
        }

        /// <summary>
        /// Maps the LoadNames async relay command for command binding.
        /// </summary>
        IAsyncRelayCommand IVornamViewModel.LoadNamesCommand =>
            new AsyncRelayCommand(LoadNames);

        /// <summary>
        /// Maps the SearchNames async relay command for command binding.
        /// </summary>
        IAsyncRelayCommand<string> IVornamViewModel.SearchNamesCommand =>
            new AsyncRelayCommand<string>(SearchNames);

        /// <summary>
        /// Maps the SelectSearchResult async relay command for command binding.
        /// </summary>
        IAsyncRelayCommand<IListItem<int>> IVornamViewModel.SelectSearchResultCommand =>
            new AsyncRelayCommand<IListItem<int>>(SelectSearchResult);

        /// <summary>
        /// Maps the SaveAllNames async relay command for command binding.
        /// </summary>
        IAsyncRelayCommand IVornamViewModel.SaveAllNamesCommand =>
            new AsyncRelayCommand(SaveAllNames);

        /// <summary>
        /// Maps the CancelEdit relay command for command binding.
        /// </summary>
        IRelayCommand IVornamViewModel.CancelEditCommand =>
            new RelayCommand(CancelEdit);

        /// <summary>
        /// Maps the DeleteAllNames async relay command for command binding.
        /// </summary>
        IAsyncRelayCommand IVornamViewModel.DeleteAllNamesCommand =>
            new AsyncRelayCommand(DeleteAllNames);

        // ============================================================================
        // Helper Methods
        // ============================================================================

        /// <summary>
        /// Refreshes the UI form fields with current names from CurrentNames collection.
        /// </summary>
        private void RefreshUI()
        {
            try
            {
                // Clear all form fields
                for (int i = 1; i <= 65; i++)
                {
                    View.Text_Renamed[i].Text = "";
                }

                // Populate from CurrentNames collection
                for (int i = 0; i < CurrentNames.Count && i < 15; i++)
                {
                    View.Text_Renamed[i + 1].Text = CurrentNames[i].PrimaryName;
                    View.Text_Renamed[i + 1 + 50].Text = CurrentNames[i].Synonym;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshUI error: {ex.Message}");
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
            // Execute load names using the async command
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

        /// <summary>
        /// Legacy event handler: Command button click. Deprecated; use command bindings instead.
        /// </summary>
        public void Befehl_Click(object sender, EventArgs e) { }

        /// <summary>
        /// Legacy event handler: List double-click variant 1. Deprecated.
        /// </summary>
        public void List1_DoubleClick(object sender, EventArgs e) { }

        /// <summary>
        /// Legacy event handler: List double-click variant 2. Deprecated.
        /// </summary>
        public void Liste1_DoubleClick(object sender, EventArgs e) { }

        /// <summary>
        /// Legacy event handler: Text key press. Use SearchNamesCommand via [CommandBinding] instead.
        /// </summary>
        public void Text_Renamed_KeyPress(object sender, KeyPressEventArgs e) { }

        /// <summary>
        /// Legacy event handler: Text key up. Deprecated.
        /// </summary>
        public void Text_Renamed_KeyUp(object sender, KeyEventArgs e) { }

        /// <summary>
        /// Legacy event handler: Text changed. Use SearchNamesCommand via [CommandBinding] instead.
        /// </summary>
        public void Text_Renamed_TextChanged(object sender, EventArgs e) { }
    }
}
