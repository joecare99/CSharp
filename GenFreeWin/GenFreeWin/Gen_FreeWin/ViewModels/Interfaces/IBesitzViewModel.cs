using GenFreeWin.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GenFreeWin.ViewModels.Interfaces;

/// <summary>
/// Describes the presentation state and interaction flow of the ownership selection form.
/// </summary>
public interface IBesitzViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the action used to close the view.
    /// </summary>
    Action? RequestClose { get; set; }

    /// <summary>
    /// Gets or sets the action used to show an informational message to the user.
    /// </summary>
    Action<string>? ShowInformation { get; set; }

    /// <summary>
    /// Gets the selectable file list.
    /// </summary>
    ObservableCollection<BesitzAkteListItem> AkteItems { get; }

    /// <summary>
    /// Gets the selectable ownership entry list.
    /// </summary>
    ObservableCollection<BesitzEntryListItem> EntryItems { get; }

    string AkteText { get; }
    string VerwaltungsortText { get; }
    string BauernschaftText { get; }
    string HofklasseText { get; }
    string JahrText { get; }
    string ErbautText { get; }
    string AbgaengigText { get; }
    string NameText { get; }
    string GebaeudeartText { get; }
    string FlurText { get; }
    string ParzelleText { get; }
    string AkteHintText { get; }
    string EntryHintText { get; }

    bool AkteSelectionVisible { get; }
    bool EntrySelectionVisible { get; }
    bool AkteHintVisible { get; }
    bool EntryHintVisible { get; }
    bool DeleteVisible { get; }
    bool CancelVisible { get; }
    bool ConfirmVisible { get; }

    /// <summary>
    /// Loads the form state for the current person and optionally preselects an ownership entry.
    /// </summary>
    /// <param name="selectedEntryNumber">The currently linked ownership entry number, or <c>0</c> if none is linked.</param>
    /// <param name="personNumber">The current person number.</param>
    void Load(int selectedEntryNumber, int personNumber);

    /// <summary>
    /// Applies the selected file and loads its ownership history entries.
    /// </summary>
    /// <param name="item">The selected file item.</param>
    void SelectAkte(BesitzAkteListItem? item);

    /// <summary>
    /// Applies the selected ownership history entry.
    /// </summary>
    /// <param name="item">The selected ownership entry item.</param>
    void SelectEntry(BesitzEntryListItem? item);

    /// <summary>
    /// Persists the current property link.
    /// </summary>
    void ConfirmSelection();

    /// <summary>
    /// Removes the current property link.
    /// </summary>
    void RemoveSelection();
}
