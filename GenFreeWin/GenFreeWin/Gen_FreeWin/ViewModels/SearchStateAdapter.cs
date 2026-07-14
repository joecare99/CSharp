// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchStateAdapter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Adapter to convert ViewModel state to/from service models</summary>
// ***********************************************************************

using GenFreeWin.ViewModels.Models;
using System;

namespace GenFreeWin.ViewModels
{
    /// <summary>
    /// Adapter that bridges NamenSuchViewModel observable properties 
    /// to SearchCriteria and FilterOptions models used by service layer.
    /// This enables incremental refactoring: ViewModel remains intact, 
    /// but new logic flows through typed models.
    /// </summary>
    public class SearchStateAdapter
    {
        private readonly NamenSuchViewModel _viewModel;

        /// <summary>
        /// Creates adapter instance with reference to ViewModel.
        /// </summary>
        public SearchStateAdapter(NamenSuchViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Creates SearchCriteria from current ViewModel state.
        /// Reads current checkbox/text properties and builds typed model.
        /// </summary>
        public SearchCriteria BuildSearchCriteria()
        {
            // Determine search mode based on active filter
            var mode = SearchMode.PersonBased;
            if (_viewModel.FamOnly_Checked)
                mode = SearchMode.FamilyBased;
            else if (_viewModel.OmitSpouse_Checked)
                mode = SearchMode.SpouseFiltered;

            return new SearchCriteria
            {
                SearchText = _viewModel.Text1_Text ?? "",
                IncludeMales = _viewModel.Male_Checked,
                IncludeFemales = _viewModel.Females_Checked,
                FamilyOnly = _viewModel.FamOnly_Checked,
                OmitSpouse = _viewModel.OmitSpouse_Checked,
                Selection = _viewModel.Selection_Checked,
                MaleFamily = _viewModel.Male2_Checked,
                FemaleFamily = _viewModel.Female2_Checked,
                AdditionalText = _viewModel.Text2_Text ?? "",
                PersonId = _viewModel.PersNr,
                FamilyId = _viewModel.FamNr,
                Mode = mode
            };
        }

        /// <summary>
        /// Creates FilterOptions snapshot of current ViewModel state.
        /// Captures all visib/enable/text state for serialization or undo.
        /// </summary>
        public FilterOptions CaptureFilterState()
        {
            return new FilterOptions
            {
                MaleChecked = _viewModel.Male_Checked,
                FemalesChecked = _viewModel.Females_Checked,
                FamOnlyChecked = _viewModel.FamOnly_Checked,
                SelectionChecked = _viewModel.Selection_Checked,
                Male2Checked = _viewModel.Male2_Checked,
                Female2Checked = _viewModel.Female2_Checked,
                OmitSpouseChecked = _viewModel.OmitSpouse_Checked,
                MaleVisible = _viewModel.Male_Visible,
                FemalesVisible = _viewModel.Females_Visible,
                FamOnlyVisible = _viewModel.FamOnly_Visible,
                Male2Visible = _viewModel.Male2_Visible,
                Female2Visible = _viewModel.Female2_Visible,
                OmitSpouseVisible = _viewModel.OmitSpouse_Visible,
                MaleEnabled = _viewModel.Male_Enabled,
                FemaleEnabled = _viewModel.Female_Enabled,
                FamOnlyEnabled = _viewModel.FamOnly_Enabled,
                OmitSpouseEnabled = _viewModel.OmitSpouse_Enabled,
                Text1 = _viewModel.Text1_Text ?? "",
                Text2 = _viewModel.Text2_Text ?? "",
                Text2Visible = _viewModel.Text2_Visible,
                // Note: PersonSheetVisible & FamilySheetVisible are private, skipped in snapshot
                StartSearchVisible = _viewModel.StartSearch_Visible,
                ReadyVisible = _viewModel.Ready_Visible,
                PersonId = _viewModel.PersNr,
                FamilyId = _viewModel.FamNr
            };
        }

        /// <summary>
        /// Applies FilterOptions state back to ViewModel properties.
        /// Used for restoring/applying filter snapshots.
        /// </summary>
        public void ApplyFilterState(FilterOptions options)
        {
            if (options == null)
                return;

            _viewModel.Male_Checked = options.MaleChecked;
            _viewModel.Females_Checked = options.FemalesChecked;
            _viewModel.FamOnly_Checked = options.FamOnlyChecked;
            _viewModel.Selection_Checked = options.SelectionChecked;
            _viewModel.Male2_Checked = options.Male2Checked;
            _viewModel.Female2_Checked = options.Female2Checked;
            _viewModel.OmitSpouse_Checked = options.OmitSpouseChecked;
            _viewModel.Text1_Text = options.Text1 ?? "";
            _viewModel.Text2_Text = options.Text2 ?? "";
            // Note: Visibility and Enable states are typically set by ViewModel logic,
            // so we don't restore them here (they're derived state)
        }

        /// <summary>
        /// Validates the current ViewModel search state.
        /// Returns true if search can be executed with current settings.
        /// </summary>
        public (bool IsValid, string ErrorMessage) ValidateCurrentState()
        {
            var criteria = BuildSearchCriteria();
            return criteria.IsValid()
                ? (true, "")
                : (false, "At least one gender filter must be selected or search text must be entered.");
        }
    }
}
