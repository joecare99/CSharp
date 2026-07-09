using Gen_FreeWin.Data;
using Gen_FreeWin.Models;
using Gen_FreeWin.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using GenFree.Interfaces.Sys;
using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gen_FreeWin.ViewModels;

/// <summary>
/// Encapsulates the bindable state of the legacy picture management workflow.
/// </summary>
public partial class BilderViewModel : ObservableObject, IBilderViewModel
{
    private readonly IBilderRepository _repository;
    private readonly IModul1 _modul1;
    private BilderListItem? _selectedPictureItem;
    private string _pictureTitle = string.Empty;
    private string _pictureRemark = string.Empty;
    private bool _pictureListVisible;
    private bool _fileSelectionVisible;
    private bool _editPanelVisible;
    private bool _pictureVisible;
    private bool _personPictureButtonVisible;
    private bool _personPictureButtonEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="BilderViewModel"/> class.
    /// </summary>
    /// <param name="repository">The repository used to access persisted picture links.</param>
    /// <param name="modul1">The shared application context used by the legacy workflow.</param>
    public BilderViewModel(IBilderRepository repository, IModul1 modul1)
    {
        _repository = repository;
        _modul1 = modul1;
        BackCommand = new DelegateCommand(_ => CloseAndRefresh());
        AddPictureCommand = new DelegateCommand(_ => StartAddPicture());
        ShowPicturesCommand = new DelegateCommand(_ => ShowPictureList());
        SavePictureCommand = new DelegateCommand(_ => { }, _ => SaveEnabled);
        DeletePictureCommand = new DelegateCommand(_ => DeleteCurrentPicture(), _ => DeleteEnabled);
    }

    /// <inheritdoc />
    public Action? RequestClose { get; set; }

    /// <inheritdoc />
    public Action? RefreshOwner { get; set; }

    /// <inheritdoc />
    public ObservableCollection<BilderListItem> PictureItems { get; } = new();

    /// <inheritdoc />
    public BilderListItem? SelectedPictureItem
    {
        get => _selectedPictureItem;
        set
        {
            if (SetProperty(ref _selectedPictureItem, value))
            {
                ApplySelection(value);
            }
        }
    }

    /// <inheritdoc />
    public string PictureTitle
    {
        get => _pictureTitle;
        set
        {
            if (SetProperty(ref _pictureTitle, value))
            {
                NotifyCommandStates();
            }
        }
    }

    /// <inheritdoc />
    public string PictureRemark
    {
        get => _pictureRemark;
        set => SetProperty(ref _pictureRemark, value);
    }

    /// <inheritdoc />
    public string SelectedRecordNumberText => SelectedPictureItem?.RecordNumber.ToString() ?? string.Empty;

    /// <inheritdoc />
    public bool PictureListVisible
    {
        get => _pictureListVisible;
        private set => SetProperty(ref _pictureListVisible, value);
    }

    /// <inheritdoc />
    public bool FileSelectionVisible
    {
        get => _fileSelectionVisible;
        private set => SetProperty(ref _fileSelectionVisible, value);
    }

    /// <inheritdoc />
    public bool EditPanelVisible
    {
        get => _editPanelVisible;
        private set => SetProperty(ref _editPanelVisible, value);
    }

    /// <inheritdoc />
    public bool PictureVisible
    {
        get => _pictureVisible;
        private set => SetProperty(ref _pictureVisible, value);
    }

    /// <inheritdoc />
    public bool PersonPictureButtonVisible
    {
        get => _personPictureButtonVisible;
        private set => SetProperty(ref _personPictureButtonVisible, value);
    }

    /// <inheritdoc />
    public bool PersonPictureButtonEnabled
    {
        get => _personPictureButtonEnabled;
        private set => SetProperty(ref _personPictureButtonEnabled, value);
    }

    /// <inheritdoc />
    public bool SaveEnabled => !string.IsNullOrWhiteSpace(PictureTitle);

    /// <inheritdoc />
    public bool DeleteEnabled => SelectedPictureItem is not null;

    /// <inheritdoc />
    public ICommand BackCommand { get; }

    /// <inheritdoc />
    public ICommand AddPictureCommand { get; }

    /// <inheritdoc />
    public ICommand ShowPicturesCommand { get; }

    /// <inheritdoc />
    public ICommand SavePictureCommand { get; }

    /// <inheritdoc />
    public ICommand DeletePictureCommand { get; }

    /// <inheritdoc />
    public void Load()
    {
        PersonPictureButtonVisible = string.Equals(_modul1.sPKennz, "P", StringComparison.Ordinal);
        ShowPictureList();
    }

    /// <inheritdoc />
    public void ShowPictureList()
    {
        SynchronizeCollection(PictureItems, _repository.LoadPictures(_modul1.sPKennz, _modul1.Ubg));
        PictureListVisible = true;
        FileSelectionVisible = false;
        EditPanelVisible = false;
        PictureVisible = false;
        ClearSelection();
        PersonPictureButtonEnabled = !HasPersonPicture();
    }

    /// <inheritdoc />
    public void StartAddPicture()
    {
        SelectedPictureItem = null;
        PictureTitle = string.Empty;
        PictureRemark = string.Empty;
        FileSelectionVisible = true;
        PictureListVisible = false;
        EditPanelVisible = false;
        PictureVisible = false;
        PersonPictureButtonEnabled = PersonPictureButtonVisible && !HasPersonPicture();
    }

    /// <inheritdoc />
    public void SaveCurrentPicture(string storedPath, string fileName)
    {
        SaveInternal(storedPath, fileName, PictureTitle);
    }

    /// <inheritdoc />
    public void SaveAsPersonPicture(string storedPath, string fileName)
    {
        SaveInternal(storedPath, fileName, "Personenbild");
        PictureTitle = "Personenbild";
        PersonPictureButtonEnabled = false;
    }

    /// <inheritdoc />
    public void DeleteCurrentPicture()
    {
        if (SelectedPictureItem is null)
        {
            return;
        }

        _repository.DeletePicture(SelectedPictureItem.RecordNumber);
        ShowPictureList();
    }

    /// <inheritdoc />
    public void CloseAndRefresh()
    {
        RefreshOwner?.Invoke();
        RequestClose?.Invoke();
    }

    private void ApplySelection(BilderListItem? item)
    {
        if (item is null)
        {
            OnPropertyChanged(nameof(SelectedRecordNumberText));
            NotifyCommandStates();
            return;
        }

        BilderDetails details = _repository.LoadPictureDetails(item.RecordNumber);
        PictureTitle = details.Description;
        PictureRemark = details.Remark?.Trim() ?? string.Empty;
        PictureVisible = true;
        EditPanelVisible = true;
        FileSelectionVisible = false;
        PictureListVisible = true;
        PersonPictureButtonEnabled = PersonPictureButtonVisible
            && !string.Equals(details.Description, "Personenbild", StringComparison.Ordinal)
            && !HasPersonPicture(item.RecordNumber);
        OnPropertyChanged(nameof(SelectedRecordNumberText));
        NotifyCommandStates();
    }

    private void ClearSelection()
    {
        _selectedPictureItem = null;
        PictureTitle = string.Empty;
        PictureRemark = string.Empty;
        OnPropertyChanged(nameof(SelectedPictureItem));
        OnPropertyChanged(nameof(SelectedRecordNumberText));
        NotifyCommandStates();
    }

    private bool HasPersonPicture(int exceptRecordNumber = 0)
    {
        foreach (BilderListItem item in PictureItems)
        {
            if (item.RecordNumber != exceptRecordNumber
                && string.Equals(item.Description, "Personenbild", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void SaveInternal(string storedPath, string fileName, string description)
    {
        var request = new BilderSaveRequest
        {
            RecordNumber = SelectedPictureItem?.RecordNumber ?? 0,
            LinkedNumber = _modul1.Ubg,
            Marker = _modul1.sPKennz,
            Description = description,
            Remark = PictureRemark,
            StoredPath = storedPath,
            FileName = fileName,
        };

        int recordNumber = _repository.SavePicture(request);
        ShowPictureList();
        SelectedPictureItem = FindByRecordNumber(recordNumber);
    }

    private BilderListItem? FindByRecordNumber(int recordNumber)
    {
        foreach (BilderListItem item in PictureItems)
        {
            if (item.RecordNumber == recordNumber)
            {
                return item;
            }
        }

        return null;
    }

    private void NotifyCommandStates()
    {
        OnPropertyChanged(nameof(SaveEnabled));
        OnPropertyChanged(nameof(DeleteEnabled));
        if (SavePictureCommand is DelegateCommand saveCommand)
        {
            saveCommand.NotifyCanExecuteChanged();
        }

        if (DeletePictureCommand is DelegateCommand deleteCommand)
        {
            deleteCommand.NotifyCanExecuteChanged();
        }
    }

    private static void SynchronizeCollection<T>(ObservableCollection<T> target, IReadOnlyList<T> source)
    {
        if (source.Count == 0)
        {
            if (target.Count > 0)
            {
                target.Clear();
            }

            return;
        }

        for (int i = 0; i < source.Count; i++)
        {
            if (i < target.Count)
            {
                if (!EqualityComparer<T>.Default.Equals(target[i], source[i]))
                {
                    target[i] = source[i];
                }
            }
            else
            {
                target.Add(source[i]);
            }
        }

        while (target.Count > source.Count)
        {
            target.RemoveAt(target.Count - 1);
        }
    }
}
