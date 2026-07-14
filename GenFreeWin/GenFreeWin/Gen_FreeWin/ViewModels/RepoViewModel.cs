// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="RepoViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Refactored ViewModel for Repository management with service-based architecture</summary>
// ***********************************************************************

using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using Gen_FreeWin.UseCases;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels;

/// <summary>
/// Refactored MVVM ViewModel for Repository (Archiv) data management.
/// Delegates data access to IRepoDataService and business logic to RepoSearchUseCase.
/// This is a "thin controller" that focuses on UI state and command orchestration.
/// </summary>
public partial class RepoViewModel : BaseViewModelCT, IRepoViewModel
{
    // ============================================================================
    // OBSERVABLE PROPERTIES (UI State)
    // ============================================================================

    [ObservableProperty]
    public partial int SourceCount { get; set; }

    [ObservableProperty]
    public partial string RepoName_Text { get; set; }

    [ObservableProperty]
    public partial string RepoStreet_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPlace_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPLZ_Text { get; set; }

    [ObservableProperty]
    public partial string RepoPhone_Text { get; set; }

    [ObservableProperty]
    public partial string RepoMail_Text { get; set; }

    [ObservableProperty]
    public partial bool BtnDeleteVisible { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(Save2Command))]
    bool _IsNotReadonly;

    [ObservableProperty]
    public partial ObservableCollection<IListItem<int>> Repolist_Items { get; set; } = new();

    [ObservableProperty]
    public partial ObservableCollection<IListItem<int>> Sources_Items { get; set; } = new();

    [ObservableProperty]
    public partial IListItem<int> Repolist_SelectedItem { get; set; }

    [ObservableProperty]
    public partial IListItem<int> Sources_SelectedItem { get; set; }

    [ObservableProperty]
    public partial string RichTextBox2_Text { get; set; }

    [ObservableProperty]
    string _RichTextBox1_Text;

    // ============================================================================
    // PUBLIC PROPERTIES
    // ============================================================================

    public float FontSize { get; set; }
    public object HintFarb { get; set; }
    public bool Button5_Visible { get; private set; }
    public Action DoClose { get; set; }
    public Action<string> DoStart { get; set; }

    public IRelayCommand LinkClickCommand => throw new NotImplementedException();
    public IRelayCommand Sources_DblCommand => throw new NotImplementedException();

    // ============================================================================
    // PRIVATE FIELDS
    // ============================================================================

    private IModul1 Modul1 { get; }
    private readonly IRepoDataService _dataService;
    private readonly RepoSearchUseCase _searchUseCase;

    IInteraction Interaction => Menue.Default;

    // ============================================================================
    // CONSTRUCTOR
    // ============================================================================

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoViewModel"/> class.
    /// </summary>
    /// <param name="modul1">The module instance for genealogy data access.</param>
    /// <param name="dataService">The repository data service.</param>
    public RepoViewModel(IModul1 modul1, IRepoDataService dataService)
    {
        Modul1 = modul1 ?? throw new ArgumentNullException(nameof(modul1));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _searchUseCase = new RepoSearchUseCase(dataService);
    }

    // ============================================================================
    // RELAY COMMANDS
    // ============================================================================

    /// <summary>
    /// Loads repositories and sources on form initialization.
    /// </summary>
    [RelayCommand]
    private async void FormLoad()
    {
        try
        {
            if (Modul1.FontSize > 0f)
            {
                FontSize = Modul1.FontSize;
            }
            HintFarb = Modul1.HintFarb;

            Sources_Items.Clear();
            Clear();

            // Load all repositories via service
            var repoItems = await _searchUseCase.LoadRepositoriesAsync();
            Repolist_Items = repoItems;

            IsNotReadonly = true;
            if (Modul1.Typ == DriveType.CDRom)
            {
                IsNotReadonly = false;
            }

            if (Repolist_Items.Count == 0 || Modul1.KontM[1].AsDouble() == 1.0)
            {
                return;
            }

            // Select and load first repository
            if (Repolist_Items.Count > 0)
            {
                MyListItem myListItem = (MyListItem)Repolist_Items[0];
                SourceCount = 1;
                int iRepoNr = myListItem.ItemData;
                await LoadRepositoryDetailsAsync(iRepoNr);
                BtnDeleteVisible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FormLoad error: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves a new repository and associates it with a source.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotReadonly))]
    private async void Save()
    {
        try
        {
            if (RepoName_Text.Trim() == "")
            {
                var Mldg = "Feld Name darf nicht leer sein, Maske schlie­ßen?";
                if (Interaction.MsgBox(Mldg, title: "Warnung", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning) != DialogResult.Cancel)
                {
                    Close();
                }
                return;
            }

            var repo = new RepoModel
            {
                Name = RepoName_Text,
                Street = RepoStreet_Text,
                Place = RepoPlace_Text,
                PostalCode = RepoPLZ_Text,
                Phone = RepoPhone_Text,
                Email = RepoMail_Text,
                Website = RichTextBox2_Text,
                Remarks = RichTextBox1_Text
            };

            int iQuellNr = MainProject.Forms.Quellverw._Label1_13.Tag.AsInt();
            int iNr = await _searchUseCase.SaveRepositoryWithSourceAsync(repo, iQuellNr);

            if (iNr > 0)
            {
                Close();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves or updates an existing repository record.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotReadonly))]
    private async void Save2()
    {
        try
        {
            if (RepoName_Text.Trim() == "")
            {
                return;
            }

            int iRepoNr = SourceCount;
            if (iRepoNr == 0)
            {
                iRepoNr = 1;
            }

            var repo = new RepoModel
            {
                Id = iRepoNr,
                Name = RepoName_Text,
                Street = RepoStreet_Text,
                Place = RepoPlace_Text,
                PostalCode = RepoPLZ_Text,
                Phone = RepoPhone_Text,
                Email = RepoMail_Text,
                Website = RichTextBox2_Text,
                Remarks = RichTextBox1_Text
            };

            int result = await _searchUseCase.SaveRepositoryWithSourceAsync(repo, 0);
            if (result > 0)
            {
                Clear();
                SourceCount = 0;
                var repoItems = await _searchUseCase.LoadRepositoriesAsync();
                Repolist_Items = repoItems;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save2 error: {ex.Message}");
        }
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    [RelayCommand]
    private void Close()
    {
        DoClose?.Invoke();
    }

    /// <summary>
    /// Creates a new repository entry and clears the form.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotReadonly))]
    private async void NewEntry()
    {
        try
        {
            SourceCount = await _searchUseCase.GetNextRepositoryIdAsync();
            Clear();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"NewEntry error: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads details for the selected repository in the list.
    /// </summary>
    [RelayCommand]
    private async void List1Dbl()
    {
        try
        {
            if (Repolist_SelectedItem == null)
            {
                return;
            }

            MyListItem myListItem = (MyListItem)Repolist_SelectedItem;
            SourceCount = myListItem.ItemData;
            await LoadRepositoryDetailsAsync(SourceCount);
            Button5_Visible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"List1Dbl error: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles double-click on source list item (opens source editor).
    /// </summary>
    [RelayCommand]
    private void List2Dbl()
    {
        try
        {
            if (Sources_SelectedItem == null)
            {
                return;
            }

            MyListItem myListItem = (MyListItem)Sources_SelectedItem;
            DoClose?.Invoke();
            MainProject.Forms.Quellverw.btnHometown.Text = Modul1.IText[EUserText.tNMBack];
            Quellverw quellverw = MainProject.Forms.Quellverw;
            long Satznr = myListItem.ItemData;
            quellverw.ViewModel.Les1(Satznr, Rich: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"List2Dbl error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes the current repository record.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsNotReadonly))]
    private async void Delete()
    {
        try
        {
            if (SourceCount <= 0)
            {
                return;
            }

            bool success = await _searchUseCase.DeleteRepositoryAsync(SourceCount);
            if (success)
            {
                Clear();
                string searchText = RepoStreet_Text;
                var repoItems = await _searchUseCase.SearchRepositoriesAsync(searchText);
                Repolist_Items = repoItems;
                BtnDeleteVisible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
        }
    }

    // ============================================================================
    // HELPER METHODS
    // ============================================================================

    /// <summary>
    /// Loads and displays repository details and associated sources.
    /// </summary>
    /// <summary>
    /// Loads and displays repository details and associated sources.
    /// </summary>
    private async Task LoadRepositoryDetailsAsync(int repoId)
    {
        try
        {
            var (repo, sources) = await _searchUseCase.LoadRepositoryDetailsAsync(repoId);
            if (repo == null)
            {
                return;
            }

            RepoName_Text = repo.Name;
            RepoStreet_Text = repo.Street;
            RepoPlace_Text = repo.Place;
            RepoPLZ_Text = repo.PostalCode;
            RepoPhone_Text = repo.Phone;
            RepoMail_Text = repo.Email;
            RichTextBox2_Text = repo.Website;
            RichTextBox1_Text = repo.Remarks;

            Modul1.Nr = repoId;
            Button5_Visible = sources.Count == 0;

            // Load sources for this repository
            var sourceItems = await _searchUseCase.LoadSourcesForRepositoryAsync(repoId);
            Sources_Items = sourceItems;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadRepositoryDetailsAsync error: {ex.Message}");
        }
    }

    /// <summary>
    /// Clears all repository fields to prepare for a new entry.
    /// </summary>
    private void Clear()
    {
        RepoName_Text = "";
        RepoStreet_Text = "";
        RepoPlace_Text = "";
        RepoPLZ_Text = "";
        RepoPhone_Text = "";
        RepoMail_Text = "";
        RichTextBox2_Text = "";
        RichTextBox1_Text = "";
        Repolist_Items.Clear();
    }
}
