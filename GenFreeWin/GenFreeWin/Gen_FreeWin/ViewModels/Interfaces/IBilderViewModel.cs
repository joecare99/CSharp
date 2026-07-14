using GenFreeWin.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GenFreeWin.ViewModels.Interfaces;

/// <summary>
/// Describes the bindable state and commands of the picture management workflow.
/// </summary>
public interface IBilderViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the action used to close the form.
    /// </summary>
    Action? RequestClose { get; set; }

    /// <summary>
    /// Gets or sets the action used to refresh the owning editor after closing.
    /// </summary>
    Action? RefreshOwner { get; set; }

    /// <summary>
    /// Gets the persisted picture links of the current record.
    /// </summary>
    ObservableCollection<BilderListItem> PictureItems { get; }

    /// <summary>
    /// Gets or sets the currently selected persisted picture item.
    /// </summary>
    BilderListItem? SelectedPictureItem { get; set; }

    /// <summary>
    /// Gets or sets the editable picture title.
    /// </summary>
    string PictureTitle { get; set; }

    /// <summary>
    /// Gets or sets the editable picture remark.
    /// </summary>
    string PictureRemark { get; set; }

    /// <summary>
    /// Gets the record number text of the selected picture.
    /// </summary>
    string SelectedRecordNumberText { get; }

    bool PictureListVisible { get; }
    bool FileSelectionVisible { get; }
    bool EditPanelVisible { get; }
    bool PictureVisible { get; }
    bool PersonPictureButtonVisible { get; }
    bool PersonPictureButtonEnabled { get; }
    bool SaveEnabled { get; }
    bool DeleteEnabled { get; }

    /// <summary>
    /// Loads the picture links for the current owner context.
    /// </summary>
    void Load();

    /// <summary>
    /// Shows the current picture list.
    /// </summary>
    void ShowPictureList();

    /// <summary>
    /// Starts the file-selection workflow for adding a picture.
    /// </summary>
    void StartAddPicture();

    /// <summary>
    /// Applies the currently edited picture metadata and persists it.
    /// </summary>
    /// <param name="storedPath">The normalized path value to store.</param>
    /// <param name="fileName">The selected file name.</param>
    void SaveCurrentPicture(string storedPath, string fileName);

    /// <summary>
    /// Saves the current selection as the person's default picture.
    /// </summary>
    /// <param name="storedPath">The normalized path value to store.</param>
    /// <param name="fileName">The selected file name.</param>
    void SaveAsPersonPicture(string storedPath, string fileName);

    /// <summary>
    /// Deletes the currently selected picture link.
    /// </summary>
    void DeleteCurrentPicture();

    /// <summary>
    /// Finalizes the workflow and refreshes the owner if needed.
    /// </summary>
    void CloseAndRefresh();

    ICommand BackCommand { get; }
    ICommand AddPictureCommand { get; }
    ICommand ShowPicturesCommand { get; }
    ICommand SavePictureCommand { get; }
    ICommand DeletePictureCommand { get; }
}
