using CommunityToolkit.Mvvm.Input;
using GenFree.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces
{
    /// <summary>
    /// Modern MVVM contract for Vorname (given name) ViewModel.
    /// Pure MVVM design: NO direct View reference. All UI state exposed via Observable Properties.
    /// This interface is WPF/MAUI ready (Form is generic for backward compatibility only).
    /// </summary>
    public interface IVornamViewModel : INotifyPropertyChanged
    {
      
        // ========================================================================
        // Observable Properties (MVVM Bindable via CommunityToolkit.Mvvm)
        // ========================================================================

        /// <summary>
        /// Search result items for autocomplete dropdown.
        /// Type: ObservableCollection&lt;IListItem&lt;int&gt;&gt;
        /// </summary>
        ObservableCollection<IListItem<int>> SearchResults { get; set; }

        /// <summary>
        /// Current loaded/edited names for the person.
        /// Type: ObservableCollection&lt;object&gt; (runtime: Gen_FreeWin.Models.VornamModel items)
        /// Note: Use as object collection to avoid cross-assembly dependency on VornamModel type.
        /// </summary>
        ObservableCollection<object> CurrentNames { get; set; }

        /// <summary>
        /// Search pattern from user input (bindable).
        /// </summary>
        string SearchPattern { get; set; }

        /// <summary>
        /// Indicates whether an async operation is in progress.
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Status/error message to display to user.
        /// </summary>
        string StatusMessage { get; set; }

        // ========================================================================
        // Form UI State Properties (NEW - for true MVVM)
        // These replace direct View access
        // ========================================================================

        /// <summary>
        /// Form heading text (replaces View.lblHeading.Text).
        /// </summary>
        string FormHeading { get; set; }

        /// <summary>
        /// Form background color as ARGB int (replaces View.BackColor).
        /// </summary>
        int FormBackColor { get; set; }

        /// <summary>
        /// Font size for form controls (replaces View.Font size).
        /// </summary>
        float FormFontSize { get; set; }

        /// <summary>
        /// Font family name for form controls (replaces View.Font family).
        /// </summary>
        string FormFontName { get; set; }

        /// <summary>
        /// Observable collection of name field view models (15 fields, lines 1-15).
        /// Replaces direct access to View.Text_Renamed[] array.
        /// Each item has PrimaryName, Synonym, IsModified, IsValid properties.
        /// Type: ObservableCollection&lt;object&gt; at interface level (runtime: NameFieldViewModel items)
        /// </summary>
        ObservableCollection<object> NameFields { get; set; }

        /// <summary>
        /// Signal for View to close (replaces View?.Close()).
        /// Set to true when form should be closed from ViewModel.
        /// </summary>
        bool RequestClose { get; set; }

        /// <summary>
        /// Current active name field index for search result selection.
        /// </summary>
        int CurrentFieldIndex { get; set; }

        // ========================================================================
        // Modern MVVM Relay Commands
        // Compatible with CommandBindingAttribute for declarative command binding
        // Usage: [CommandBinding(nameof(IVornamViewModel.LoadNamesCommand))]
        // ========================================================================

        /// <summary>
        /// Async command: Load names for current person/gender on form load.
        /// Usage: [CommandBinding(nameof(IVornamViewModel.LoadNamesCommand))]
        /// </summary>
        IAsyncRelayCommand LoadNamesCommand { get; }

        /// <summary>
        /// Async command: Search names matching pattern (invoked on TextChanged).
        /// Parameter: search text pattern (string)
        /// Usage: [CommandBinding(nameof(IVornamViewModel.SearchNamesCommand))]
        /// </summary>
        IAsyncRelayCommand<string> SearchNamesCommand { get; }

        /// <summary>
        /// Async command: Select a search result and populate name field.
        /// Parameter: IListItem&lt;int&gt; selected item from dropdown
        /// Usage: [CommandBinding(nameof(IVornamViewModel.SelectSearchResultCommand))]
        /// </summary>
        IAsyncRelayCommand<IListItem<int>> SelectSearchResultCommand { get; }

        /// <summary>
        /// Async command: Save all edited names to database.
        /// Usage: [CommandBinding(nameof(IVornamViewModel.SaveAllNamesCommand))]
        /// </summary>
        IAsyncRelayCommand SaveAllNamesCommand { get; }

        /// <summary>
        /// Sync command: Cancel edit and close form.
        /// Usage: [CommandBinding(nameof(IVornamViewModel.CancelEditCommand))]
        /// </summary>
        IRelayCommand CancelEditCommand { get; }

        /// <summary>
        /// Sync command: Done edit and close form.
        /// Usage: [CommandBinding(nameof(IVornamViewModel.DoneEditCommand))]
        /// </summary>
        IRelayCommand DoneEditCommand { get; }

        /// <summary>
        /// Async command: Delete all names of current gender.
        /// Usage: [CommandBinding(nameof(IVornamViewModel.DeleteAllNamesCommand))]
        /// </summary>
        IAsyncRelayCommand DeleteAllNamesCommand { get; }

        // ========================================================================
        // Legacy Event Handlers (Backward Compatibility)
        // Prefer RelayCommands + CommandBinding for new code
        // ========================================================================

        /// <summary>
        /// Handles form load initialization. Legacy event handler; use LoadNamesCommand instead.
        /// </summary>
        void Form_Load(object sender, EventArgs e);

    }
}
