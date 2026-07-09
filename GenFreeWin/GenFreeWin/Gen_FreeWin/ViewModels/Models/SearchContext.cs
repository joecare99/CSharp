namespace Gen_FreeWin.ViewModels.Models
{
    /// <summary>
    /// Encapsulates search context state and parameters used during search operations in Listfuell().
    /// Separates mutable state from the ViewModel, improving testability and maintainability.
    /// </summary>
    public class SearchContext
    {
        /// <summary>
        /// The selected search type from ComboBox2 (t308, t309, t311, t312, t313, t314, t315, t316, t317, t318, t319, t320, t321).
        /// </summary>
        public string SearchType { get; set; } = string.Empty;

        /// <summary>
        /// Primary search input text from Text1_Text.
        /// </summary>
        public string SearchText { get; set; } = string.Empty;

        /// <summary>
        /// Secondary search filter, if applicable (e.g., from Text2_Text in birth/death searches).
        /// </summary>
        public string FilterText { get; set; } = string.Empty;

        /// <summary>
        /// Error or info message to display (e.g., for descendant number format validation).
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Loop index for descendant/ancestor number parsing (for t308 descendant search).
        /// </summary>
        public int ParseIndex { get; set; } = 0;

        /// <summary>
        /// End index for descendant/ancestor number parsing.
        /// </summary>
        public int ParseEndIndex { get; set; } = 0;

        /// <summary>
        /// Counter for search result iteration.
        /// </summary>
        public int ResultCounter { get; set; } = 0;

        /// <summary>
        /// Flag: Include spouse records or not (from OmitSpouse_Checked).
        /// </summary>
        public bool IncludeSpouse { get; set; } = true;

        /// <summary>
        /// Current result buffer (item, item2, text, etc. combined).
        /// </summary>
        public string CurrentResult { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the search context is in error state.
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Resets the context for a new search operation.
        /// </summary>
        public void Reset()
        {
            SearchType = string.Empty;
            SearchText = string.Empty;
            FilterText = string.Empty;
            Message = string.Empty;
            ParseIndex = 0;
            ParseEndIndex = 0;
            ResultCounter = 0;
            CurrentResult = string.Empty;
            IsError = false;
        }

        /// <summary>
        /// Returns a string representation of the search context for debugging.
        /// </summary>
        public override string ToString()
        {
            return $"SearchContext(Type={SearchType}, SearchText={SearchText}, FilterText={FilterText}, " +
                   $"ParseIndex={ParseIndex}, ParseEndIndex={ParseEndIndex}, ResultCounter={ResultCounter}, " +
                   $"IncludeSpouse={IncludeSpouse}, IsError={IsError})";
        }
    }
}
