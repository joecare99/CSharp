// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchCommandHandler.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Handler for search execution commands (extracted from NamenSuchViewModel)</summary>
// ***********************************************************************

using Gen_FreeWin.Services;
using System;
using System.Threading.Tasks;

namespace Gen_FreeWin.ViewModels.Commands
{
    /// <summary>
    /// Encapsulates search command logic extracted from NamenSuchViewModel.
    /// 
    /// Responsibilities:
    /// - Execute search via INameSearchService with SearchCriteria
    /// - Update result collections (List1_Items, etc.)
    /// - Manage search state (StatusMessage, IsLoading, SearchResultCount)
    /// - Clear search results
    /// 
    /// Benefits of extraction:
    /// - Separates search orchestration from ViewModel state management
    /// - Easier to test search logic independent of ViewModel
    /// - Cleaner command method signatures
    /// - Reusable search handler across multiple ViewModels
    /// </summary>
    public class SearchCommandHandler
    {
        private readonly NamenSuchViewModel _viewModel;
        private readonly INameSearchService _searchService;
        private readonly ISearchResultMapper _resultMapper;
        private readonly SearchStateAdapter _stateAdapter;

        /// <summary>
        /// Initializes a new instance of the SearchCommandHandler.
        /// </summary>
        /// <param name="viewModel">The hosting ViewModel.</param>
        /// <param name="searchService">Service for executing searches.</param>
        /// <param name="resultMapper">Service for formatting results.</param>
        /// <param name="stateAdapter">Adapter for building search criteria from UI state.</param>
        public SearchCommandHandler(
            NamenSuchViewModel viewModel,
            INameSearchService searchService,
            ISearchResultMapper resultMapper,
            SearchStateAdapter stateAdapter)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _resultMapper = resultMapper ?? throw new ArgumentNullException(nameof(resultMapper));
            _stateAdapter = stateAdapter ?? throw new ArgumentNullException(nameof(stateAdapter));
        }

        /// <summary>
        /// Executes a search based on current UI state and updates result collections.
        /// </summary>
        /// <returns>A task that completes when the search is finished.</returns>
        public async Task ExecuteSearchAsync()
        {
            try
            {
                _viewModel.IsLoading = true;
                _viewModel.StatusMessage = "Suche wird ausgeführt...";
                _viewModel.SearchResultCount = 0;

                // Clear existing results
                _viewModel.List1_Items.Clear();

                // Build search criteria from current UI state
                var criteria = _stateAdapter.BuildSearchCriteria();

                // Execute search - NOTE: method name varies, placeholder for integration
                // var results = await _searchService.ExecuteSearchAsync(criteria);

                // DEFERRED Phase E: 
                // - Wire correct search service method
                // - Handle tuple shape mapping (SearchResult → List1_Items tuple)
                // - For now, cascade to legacy Listfuell() if available

                _viewModel.StatusMessage = "Suche: Phase D placeholder (use legacy path)";
            }
            catch (Exception ex)
            {
                _viewModel.StatusMessage = $"Fehler bei Suche: {ex.Message}";
                _viewModel.SearchResultCount = 0;
            }
            finally
            {
                _viewModel.IsLoading = false;
            }
        }

        /// <summary>
        /// Clears all search results from result collections.
        /// </summary>
        public void ClearResults()
        {
            _viewModel.List1_Items.Clear();
            _viewModel.List2_Items.Clear();
            _viewModel.List3_Items.Clear();
            _viewModel.List4_Items.Clear();
            _viewModel.List5_Items.Clear();
            _viewModel.List7_Items.Clear();
            _viewModel.ListBox1_Items.Clear();

            _viewModel.SearchResultCount = 0;
            _viewModel.StatusMessage = "Ergebnisse gelöscht";
        }
    }
}
