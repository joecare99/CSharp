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
    /// Defines observable properties, async/sync relay commands, and maintains backward compatibility with legacy form events.
    /// Note: CurrentNames is typed as ObservableCollection&lt;object&gt; to avoid cross-assembly model visibility.
    /// At runtime, each item is a Gen_FreeWin.Models.VornamModel; cast as needed when consuming.
    /// </summary>
    public interface IVornamViewModel : INotifyPropertyChanged
    {
        // ========================================================================
        // Form/View Reference
        // ========================================================================

        /// <summary>
        /// Gets or sets the associated WinForms View (Vornam form).
        /// </summary>
        Form View { get; set; }

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

        /// <summary>
        /// Handles command button click. Legacy event handler; use command bindings instead.
        /// </summary>
        void Befehl_Click(object sender, EventArgs e);

        /// <summary>
        /// Handles list double-click (variant 1). Legacy event handler.
        /// </summary>
        void List1_DoubleClick(object sender, EventArgs e);

        /// <summary>
        /// Handles list double-click (variant 2). Legacy event handler.
        /// </summary>
        void Liste1_DoubleClick(object sender, EventArgs e);

        /// <summary>
        /// Handles name field key press. Legacy event handler; use SearchNamesCommand instead.
        /// </summary>
        void Text_Renamed_KeyPress(object sender, KeyPressEventArgs e);

        /// <summary>
        /// Handles name field key up. Legacy event handler.
        /// </summary>
        void Text_Renamed_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e);

        /// <summary>
        /// Handles name field text changed. Legacy event handler; use SearchNamesCommand instead.
        /// </summary>
        void Text_Renamed_TextChanged(object sender, EventArgs e);
    }
}
