// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="HGAkteViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Refactored thin MVVM ViewModel for HGakte (Grundbuchakte) management</summary>
// ***********************************************************************

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using Gen_FreeWin.UseCases;
using Gen_FreeWin.ViewModels.Interfaces;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gen_FreeWin.ViewModels
{
    /// <summary>
    /// Thin MVVM ViewModel for Grundbuchakte (HGakte) management.
    /// Orchestrates UI state and delegates business logic to HGAkteSearchUseCase.
    /// </summary>
    public partial class HGAkteViewModel : BaseViewModelCT, IHGAkteViewModel
    {
        IContainerControl IHGAkteViewModel.View { get; set; }
        private HGakte View => (HGakte)((IHGAkteViewModel)this).View;

        public Action<FormWindowState, float> View_InitView { get; set; }
        public Action DoClose { get; set; }

        // Observable properties for UI binding
        [ObservableProperty]
        public partial bool Frame1_Visible { get; set; }

        [ObservableProperty]
        public partial bool Usage_Visible { get; set; }

        [ObservableProperty]
        public partial string Number_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Place_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Union_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Class_Text { get; set; } = "";

        [ObservableProperty]
        public partial string FireInsurance_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Additional_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Flurstueck_Text { get; set; } = "";

        [ObservableProperty]
        public partial string Parzelle_Text { get; set; } = "";

        [ObservableProperty]
        public partial ObservableCollection<IListItem<int>> AkteList_Items { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<IListItem<int>> GBE_Items { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<IListItem<int>> Usage_Items { get; set; }

        [ObservableProperty]
        public partial IListItem<int> AkteList_SelectedItem { get; set; }

        // Service and use case
        private readonly IHGAkteDataService _dataService;
        private readonly HGAkteSearchUseCase _searchUseCase;

        // Current state tracking
        private int _currentAkteId;

        // References
        private IModul1 Modul1 => _Modul1.Instance;
        private IInteraction Interaction => Menue.Default;
        private IStrings Strings => Modul1.Strings;
        private IProjectData ProjectData => Modul1.ProjectData;

        /// <summary>
        /// Creates a new HGAkteViewModel instance.
        /// </summary>
        public HGAkteViewModel(IHGAkteDataService dataService, HGAkteSearchUseCase searchUseCase)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _searchUseCase = searchUseCase ?? throw new ArgumentNullException(nameof(searchUseCase));

            // Initialize collections
            AkteList_Items = new ObservableCollection<IListItem<int>>();
            GBE_Items = new ObservableCollection<IListItem<int>>();
            Usage_Items = new ObservableCollection<IListItem<int>>();
        }

        /// <summary>
        /// Form Load event handler. Initializes the view.
        /// </summary>
        public void Form_Load(object sender, EventArgs e)
        {
            try
            {
                Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
                View_InitView(WiS, Modul1.FontSize);

                _ = Task.Run(() => LoadInitialDataAsync());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Form_Load error: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads initial data when form opens.
        /// </summary>
        private async Task LoadInitialDataAsync()
        {
            try
            {
                // Load all Akten
                var items = await _searchUseCase.LoadAktenAsync();
                AkteList_Items = items;

                if (items.Count > 0)
                {
                    AkteList_SelectedItem = items[0];
                    _currentAkteId = items[0].ItemData;
                    await LoadAkteDetailsAsync(_currentAkteId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadInitialDataAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads and displays Akte details for the specified ID.
        /// </summary>
        private async Task LoadAkteDetailsAsync(int akteId)
        {
            try
            {
                var (akte, gbes) = await _searchUseCase.LoadAkteDetailsAsync(akteId);
                if (akte == null)
                {
                    ClearFields();
                    return;
                }

                // Update observable properties
                Number_Text = akte.AkteNumber;
                Place_Text = akte.Kirchspiel;
                Union_Text = akte.Beschreibung;
                Class_Text = akte.Hof;
                FireInsurance_Text = akte.Brandkasse;
                Additional_Text = akte.Bemerkungen;
                Flurstueck_Text = akte.Flur;
                Parzelle_Text = akte.Parzelle;

                _currentAkteId = akteId;

                // Load associated GBEs
                GBE_Items = await _searchUseCase.LoadGBEsForAkteAsync(akte.AkteNumber);

                // Load property usages
                Usage_Items = await _searchUseCase.LoadPropertyUsagesAsync(akte.AkteNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadAkteDetailsAsync error: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all Akte fields.
        /// </summary>
        private void ClearFields()
        {
            Number_Text = "";
            Place_Text = "";
            Union_Text = "";
            Class_Text = "";
            FireInsurance_Text = "";
            Additional_Text = "";
            Flurstueck_Text = "";
            Parzelle_Text = "";
            GBE_Items.Clear();
            Usage_Items.Clear();
        }

        /// <summary>
        /// Navigates to previous Akte.
        /// </summary>
        [RelayCommand]
        private async Task PrevEntry()
        {
            try
            {
                if (AkteList_Items.Count == 0)
                    return;

                // Find current index and go to previous
                int currentIndex = -1;
                for (int i = 0; i < AkteList_Items.Count; i++)
                {
                    if (AkteList_Items[i].ItemData == _currentAkteId)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex > 0)
                {
                    AkteList_SelectedItem = AkteList_Items[currentIndex - 1];
                    await LoadAkteDetailsAsync(AkteList_Items[currentIndex - 1].ItemData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PrevEntry error: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigates to next Akte.
        /// </summary>
        [RelayCommand]
        private async Task NextEntry()
        {
            try
            {
                if (AkteList_Items.Count == 0)
                    return;

                // Find current index and go to next
                int currentIndex = -1;
                for (int i = 0; i < AkteList_Items.Count; i++)
                {
                    if (AkteList_Items[i].ItemData == _currentAkteId)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex >= 0 && currentIndex < AkteList_Items.Count - 1)
                {
                    AkteList_SelectedItem = AkteList_Items[currentIndex + 1];
                    await LoadAkteDetailsAsync(AkteList_Items[currentIndex + 1].ItemData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NextEntry error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new Akte entry.
        /// </summary>
        [RelayCommand]
        private async Task EnterNew2()
        {
            try
            {
                var nextId = await _searchUseCase.GetNextAkteIdAsync();

                var newAkte = new HGAkteModel
                {
                    Id = nextId,
                    AkteNumber = "",
                    Kirchspiel = "",
                    Beschreibung = "",
                    Hof = "",
                    Brandkasse = "",
                    Bemerkungen = "",
                    Flur = "",
                    Parzelle = ""
                };

                _currentAkteId = nextId;
                Number_Text = "";
                Place_Text = "";
                Union_Text = "";
                Class_Text = "";
                FireInsurance_Text = "";
                Additional_Text = "";
                Flurstueck_Text = "";
                Parzelle_Text = "";
                GBE_Items.Clear();
                Usage_Items.Clear();

                View.edtNumber?.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EnterNew2 error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes current Akte entry if no Akte number is set.
        /// </summary>
        [RelayCommand]
        private async Task ValidateAndClear()
        {
            try
            {
                // This method checks if Akte number is empty and deletes if needed
                if (string.IsNullOrWhiteSpace(Number_Text) && _currentAkteId > 0)
                {
                    await _searchUseCase.DeleteAkteAsync(_currentAkteId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ValidateAndClear error: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves current Akte entry.
        /// </summary>
        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Number_Text))
                {
                    var result = Interaction.MsgBox(
                        "Akte-Nummer darf nicht leer sein. Fenster schließen?",
                        title: "Warnung",
                        mb: MessageBoxButtons.YesNo,
                        icon: MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        DoClose?.Invoke();
                    }
                    return;
                }

                var akte = new HGAkteModel
                {
                    Id = _currentAkteId,
                    AkteNumber = Number_Text.Trim(),
                    Kirchspiel = Place_Text.Trim(),
                    Beschreibung = Union_Text.Trim(),
                    Hof = Class_Text.Trim(),
                    Brandkasse = FireInsurance_Text.Trim(),
                    Bemerkungen = Additional_Text.Trim(),
                    Flur = Flurstueck_Text.Trim(),
                    Parzelle = Parzelle_Text.Trim()
                };

                await _searchUseCase.UpdateAkteAsync(akte);
                DoClose?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            }
        }

        /// <summary>
        /// Shows property usage list.
        /// </summary>
        [RelayCommand]
        private async Task ShowUsage()
        {
            try
            {
                Usage_Visible = true;
                if (View.GroupBoxUsage != null)
                {
                    View.GroupBoxUsage.Top = 40;
                    View.GroupBoxUsage.Left = 12;
                    View.GroupBoxUsage.Width = View.Width - 420;
                    if (View.lstUsageList != null)
                    {
                        View.lstUsageList.Width = View.GroupBoxUsage.Width - 30;
                    }
                    View.GroupBoxUsage.Height = View.Height - 45;
                    View.GroupBoxUsage.Visible = true;
                }

                Usage_Items = await _searchUseCase.LoadPropertyUsagesAsync(Number_Text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowUsage error: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes usage display.
        /// </summary>
        [RelayCommand]
        private void CloseUsage()
        {
            Usage_Visible = false;
        }

        /// <summary>
        /// Closes the ViewModel and form.
        /// </summary>
        [RelayCommand]
        private void MainMenue()
        {
            Menue.Default.Show();
            DoClose?.Invoke();
        }

        /// <summary>
        /// Handles search functionality.
        /// </summary>
        [RelayCommand]
        private void Search()
        {
            _ = Interaction.MsgBox("Suchfunktion noch machen");
        }

        /// <summary>
        /// Handles back navigation.
        /// </summary>
        [RelayCommand]
        private void Back()
        {
            _ = Interaction.MsgBox("Noch machen");
        }

        /// <summary>
        /// Handles new entry within frame.
        /// </summary>
        [RelayCommand]
        private void NewEntry()
        {
            Frame1_Visible = false;
        }

        /// <summary>
        /// Handles cancel entry.
        /// </summary>
        [RelayCommand]
        private void CancelEntry()
        {
            Frame1_Visible = false;
        }

        /// <summary>
        /// Handles entry edit.
        /// </summary>
        [RelayCommand]
        private void EditEntry()
        {
            Frame1_Visible = false;
        }
    }
}
