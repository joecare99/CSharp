// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="FilterCommandHandler.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Handler for filter/checkbox state change commands (extracted from NamenSuchViewModel)</summary>
// ***********************************************************************

using System;

namespace Gen_FreeWin.ViewModels.Commands
{
    /// <summary>
    /// Encapsulates filter command logic extracted from NamenSuchViewModel.
    /// 
    /// Responsibilities:
    /// - Handle gender filter checkbox changes (Male, Female, Male2, Female2)
    /// - Handle family mode checkbox (FamOnly)
    /// - Handle spouse exclusion checkbox (OmitSpouse)
    /// - Manage filter UI state (visibility/enable flags)
    /// - Update status messages
    /// - Clear results when filters change
    /// 
    /// Benefits of extraction:
    /// - Filter logic is isolated and testable
    /// - Multiple filters don't clutter the ViewModel
    /// - Easier to add new filters without ViewModel growth
    /// - Clear separation: filters ≠ search ≠ navigation
    /// </summary>
    public class FilterCommandHandler
    {
        private readonly NamenSuchViewModel _viewModel;
        private readonly DataStoreAdapter _dataStoreAdapter;

        /// <summary>
        /// Initializes a new instance of the FilterCommandHandler.
        /// </summary>
        /// <param name="viewModel">The hosting ViewModel.</param>
        /// <param name="dataStoreAdapter">Adapter for data persistence.</param>
        public FilterCommandHandler(NamenSuchViewModel viewModel, DataStoreAdapter dataStoreAdapter)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _dataStoreAdapter = dataStoreAdapter ?? throw new ArgumentNullException(nameof(dataStoreAdapter));
        }

        /// <summary>
        /// Handles gender filter: Male checkbox toggled.
        /// </summary>
        public void HandleMaleChecked()
        {
            _viewModel.Females_Checked = false;
            _viewModel.SearchResultCount = 0;
            _viewModel.StatusMessage = "Filter: Nur Männer";
            ClearResults();
        }

        /// <summary>
        /// Handles gender filter: Female checkbox toggled.
        /// </summary>
        public void HandleFemaleChecked()
        {
            _viewModel.Male_Checked = false;
            _viewModel.SearchResultCount = 0;
            _viewModel.StatusMessage = "Filter: Nur Frauen";
            ClearResults();
        }

        /// <summary>
        /// Handles family filter: Male2 (family member - male) checkbox toggled.
        /// </summary>
        public void HandleFamilyMaleChecked()
        {
            _viewModel.Female2_Checked = false;
            _viewModel.SearchResultCount = 0;
            _viewModel.StatusMessage = "Filter: Männliche Familie";
            ClearResults();
        }

        /// <summary>
        /// Handles family filter: Female2 (family member - female) checkbox toggled.
        /// </summary>
        public void HandleFamilyFemaleChecked()
        {
            _viewModel.Male2_Checked = false;
            _viewModel.SearchResultCount = 0;
            _viewModel.StatusMessage = "Filter: Weibliche Familie";
            ClearResults();
        }

        /// <summary>
        /// Handles family mode: FamilyOnly checkbox toggled.
        /// Switches between normal and family-only filtering.
        /// </summary>
        public void HandleFamilyOnlyChecked()
        {
            _viewModel.Male2_Checked = false;
            _viewModel.Female2_Checked = false;
            _viewModel.Male_Checked = false;
            _viewModel.Females_Checked = false;

            if (_viewModel.FamOnly_Checked)
            {
                // In family-only mode, hide family member filters, show gender filters
                _viewModel.Male2_Visible = false;
                _viewModel.Female2_Visible = false;
                _viewModel.Male_Visible = true;
                _viewModel.Females_Visible = true;
                _viewModel.StatusMessage = "Modus: Nur Familie";
            }
            else
            {
                // Normal mode: show family filters, hide gender filters
                _viewModel.Male_Visible = false;
                _viewModel.Females_Visible = false;
                _viewModel.Male2_Visible = true;
                _viewModel.Female2_Visible = true;
                _viewModel.StatusMessage = "Modus: Normal";
            }

            ClearResults();
        }

        /// <summary>
        /// Handles spouse exclusion: OmitSpouse checkbox toggled.
        /// Adjusts UI labels and filter visibility accordingly.
        /// </summary>
        public void HandleOmitSpouseChecked()
        {
            _viewModel.Male_Checked = false;
            _viewModel.Females_Checked = false;
            _viewModel.FamOnly_Checked = false;

            if (_viewModel.OmitSpouse_Checked)
            {
                _viewModel.Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr.";
                _viewModel.Male_Visible = false;
                _viewModel.Females_Visible = false;
                _viewModel.FamOnly_Visible = false;
                _viewModel.StatusMessage = "Filter: Partner ignoriert";
            }
            else
            {
                _viewModel.Label3_Text = "Name,Vorname                        Datum JJJJ  Personennr. Heirat       Partner";
                _viewModel.Male2_Visible = true;
                _viewModel.Female2_Visible = true;
                _viewModel.FamOnly_Visible = true;
                _viewModel.StatusMessage = "Filter: Partner einbezogen";
            }

            ClearResults();
        }

        /// <summary>
        /// Clears result collections when filters change.
        /// </summary>
        private void ClearResults()
        {
            _viewModel.List1_Items.Clear();
            _viewModel.List2_Items.Clear();
            _viewModel.List3_Items.Clear();
            _viewModel.List4_Items.Clear();
            _viewModel.List5_Items.Clear();
            _viewModel.List7_Items.Clear();
            _viewModel.ListBox1_Items.Clear();
        }
    }
}
