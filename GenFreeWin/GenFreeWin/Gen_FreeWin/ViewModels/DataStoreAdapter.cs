// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="DataStoreAdapter.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Adapter for managing PersonSearchData and SearchUIState persistence (extracted from NamenSuchViewModel)</summary>
// ***********************************************************************

using GenFreeWin.ViewModels.Models;
using System;

namespace GenFreeWin.ViewModels
{
    /// <summary>
    /// Adapter responsible for bridging the extracted data/state models (PersonSearchData, SearchUIState) 
    /// with the ViewModel's legacy persistence and interaction layer.
    /// 
    /// This adapter:
    /// - Loads PersonSearchData from persistence (legacy IGenPersistence interface)
    /// - Loads SearchUIState from ViewModel/View properties
    /// - Persists changes back to the underlying storage and UI
    /// - Handles VB-legacy field mappings during load/save operations
    /// 
    /// Part of Phase D scope decomposition: isolates all data-holding concerns
    /// from the view model's command/interaction logic.
    /// </summary>
    public class DataStoreAdapter
    {
        private readonly NamenSuchViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the DataStoreAdapter.
        /// </summary>
        /// <param name="viewModel">The parent ViewModel that provides access to persistence and interaction interfaces.</param>
        public DataStoreAdapter(NamenSuchViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Loads current person/family data from the ViewModel's persistence layer and other sources
        /// into a newly created PersonSearchData model.
        /// </summary>
        /// <returns>A PersonSearchData instance populated with current values, or a reset instance if no data is available.</returns>
        public PersonSearchData LoadPersonSearchData()
        {
            var data = new PersonSearchData();

            // Map from ViewModel's legacy fields to the new model
            if (_viewModel != null)
            {
                // Note: Direct field access would require public properties; for now,
                // this adapter serves as a contract showing how data would be loaded.
                // The actual mapping depends on ViewModel field visibility being increased
                // or refactored to properties.

                // Placeholder for actual persistence load:
                // data.AccountNumber = _viewModel.An;
                // data.PersonId = _viewModel.ID;
                // ... etc for all legacy fields
            }

            return data;
        }

        /// <summary>
        /// Loads current UI state (checkboxes, visibility, enable states) from the ViewModel
        /// into a newly created SearchUIState model.
        /// </summary>
        /// <returns>A SearchUIState instance with current UI state values.</returns>
        public SearchUIState LoadSearchUIState()
        {
            var state = new SearchUIState();

            if (_viewModel != null)
            {
                // Bind from ViewModel's ObservableProperty fields
                state.MaleChecked = _viewModel.Male_Checked;
                state.FemaleChecked = _viewModel.Females_Checked;
                state.FamilyOnlyChecked = _viewModel.FamOnly_Checked;
                state.SelectionChecked = _viewModel.Selection_Checked;
                state.FamilyMaleChecked = _viewModel.Male2_Checked;
                state.FamilyFemaleChecked = _viewModel.Female2_Checked;
                state.OmitSpouseChecked = _viewModel.OmitSpouse_Checked;

                // Text inputs
                state.SearchText = _viewModel.Text1_Text ?? "";
                state.AdditionalSearchText = _viewModel.Text2_Text ?? "";

                // Numbers
                state.PersonNumber = _viewModel.PersNr;
                state.FamilyNumber = _viewModel.FamNr;

                // Note: Visibility and enable-state loading depends on ViewModel field exposure.
                // Placeholder for actual mappings:
                // state.MaleVisible = _viewModel.Male_Visible;
                // state.MaleEnabled = _viewModel.Male_Enabled;
                // ... etc
            }

            return state;
        }

        /// <summary>
        /// Applies PersonSearchData changes back to the ViewModel and persistence layer.
        /// </summary>
        /// <param name="data">The PersonSearchData model with updated values.</param>
        public void SavePersonSearchData(PersonSearchData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (_viewModel != null)
            {
                // Note: This adapter shows the contract; actual save depends on ViewModel field exposure.
                // Placeholder mappings:
                // _viewModel.An = data.AccountNumber;
                // _viewModel.ID = (short)data.PersonId;
                // ... etc for all legacy fields

                // If persistence interface is available, could also save directly:
                // var persistence = _viewModel.Persistence;
                // if (persistence != null)
                // {
                //     persistence.Save(... data model);
                // }
            }
        }

        /// <summary>
        /// Applies SearchUIState changes back to the ViewModel's UI properties.
        /// </summary>
        /// <param name="state">The SearchUIState model with updated UI state values.</param>
        public void SaveSearchUIState(SearchUIState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (_viewModel != null)
            {
                // Apply to ObservableProperty fields (will notify View)
                _viewModel.Male_Checked = state.MaleChecked;
                _viewModel.Females_Checked = state.FemaleChecked;
                _viewModel.FamOnly_Checked = state.FamilyOnlyChecked;
                _viewModel.Selection_Checked = state.SelectionChecked;
                _viewModel.Male2_Checked = state.FamilyMaleChecked;
                _viewModel.Female2_Checked = state.FamilyFemaleChecked;
                _viewModel.OmitSpouse_Checked = state.OmitSpouseChecked;

                // Text inputs
                _viewModel.Text1_Text = state.SearchText;
                _viewModel.Text2_Text = state.AdditionalSearchText;

                // Numbers
                _viewModel.PersNr = state.PersonNumber;
                _viewModel.FamNr = state.FamilyNumber;

                // Note: Visibility and enable-state saving depends on ViewModel field exposure.
                // Placeholder:
                // _viewModel.Male_Visible = state.MaleVisible;
                // _viewModel.Male_Enabled = state.MaleEnabled;
                // ... etc
            }
        }

        /// <summary>
        /// Clears both PersonSearchData and SearchUIState in the ViewModel.
        /// </summary>
        public void ClearAllData()
        {
            // Clear search data
            var emptyData = new PersonSearchData();
            emptyData.Clear();
            SavePersonSearchData(emptyData);

            // Clear UI state
            var emptyUIState = new SearchUIState();
            emptyUIState.ResetToDefaults();
            SaveSearchUIState(emptyUIState);
        }

        /// <summary>
        /// Takes a snapshot of current data and UI state for undo/restore operations.
        /// </summary>
        /// <returns>A tuple containing both PersonSearchData and SearchUIState snapshots.</returns>
        public (PersonSearchData Data, SearchUIState UIState) TakeSnapshot()
        {
            return (LoadPersonSearchData(), LoadSearchUIState());
        }

        /// <summary>
        /// Restores data and UI state from a previously taken snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot tuple from TakeSnapshot().</param>
        public void RestoreFromSnapshot((PersonSearchData Data, SearchUIState UIState) snapshot)
        {
            SavePersonSearchData(snapshot.Data);
            SaveSearchUIState(snapshot.UIState);
        }
    }
}
