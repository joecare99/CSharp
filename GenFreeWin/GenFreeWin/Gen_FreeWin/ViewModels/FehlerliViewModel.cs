using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services;
using System;
using System.Collections.ObjectModel;

namespace Gen_FreeWin.ViewModels;

/// <summary>
/// ViewModel für die Fehlerli-Dialog.
/// Kapselt Business-Logik und UI-State nach MVVM-Pattern.
/// 
/// Nutzt CommunityToolkit.Mvvm [ObservableProperty] für automatische PropertyChanged-Events.
/// </summary>
public partial class FehlerliViewModel : ObservableObject
{
    private readonly IFehlerlistenService _fehlerlistenService;

    /// <summary>
    /// Fehlliste für Personen ohne Eltern.
    /// Wird automatisch an ListBox in der View gebunden.
    /// [ObservableProperty] erzeugt PropertyChanged-Events bei Änderungen.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ErrorListItem> personenOhneElternList = new();

    /// <summary>
    /// Fehlliste für Personen mit fehlenden Daten.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ErrorListItem> personenErrorsList = new();

    /// <summary>
    /// Fehlliste für Familien mit Fehlern.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ErrorListItem> familienErrorsList = new();

    /// <summary>
    /// Fehlliste für Orte mit Fehlern.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ErrorListItem> oerterErrorsList = new();

    /// <summary>
    /// Aktueller Sortierfeld-Name (z.B. "Name", "Id", "ErrorType").
    /// </summary>
    [ObservableProperty]
    private string sortFieldName = "DisplayText";

    /// <summary>
    /// Sortierrichtung: true = aufsteigend, false = absteigend.
    /// </summary>
    [ObservableProperty]
    private bool sortAscending = true;

    /// <summary>
    /// Progress-Prozentanteil (0-100) für die ProgressBar.
    /// </summary>
    [ObservableProperty]
    private int progressValue = 0;

    /// <summary>
    /// Maximaler Progress-Wert.
    /// </summary>
    [ObservableProperty]
    private int progressMaximum = 100;

    /// <summary>
    /// Status-Text über den aktuellen Vorganz.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = "Bereit";

    /// <summary>
    /// Gibt an, ob gerade geladen wird (für UI-Disable während des Ladens).
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    /// <summary>
    /// Aktuell ausgewähltes Element in der aktiven Liste.
    /// </summary>
    [ObservableProperty]
    private ErrorListItem selectedItem;

    public FehlerliViewModel(IFehlerlistenService fehlerlistenService)
    {
        _fehlerlistenService = fehlerlistenService ?? throw new ArgumentNullException(nameof(fehlerlistenService));
    }

    /// <summary>
    /// Command für: Personen ohne Eltern laden.
    /// </summary>
    [RelayCommand]
    private async void LoadPersonenOhneEltern()
    {
        await LoadErrorListAsync(
            "Personen ohne Eltern",
            PersonenOhneElternList,
            () => _fehlerlistenService.GetPersonenOhneElternAsync(
                UpdateProgress,
                item => PersonenOhneElternList.Add(item)));
    }

    /// <summary>
    /// Command für: Personen mit Fehlern laden.
    /// </summary>
    [RelayCommand]
    private async void LoadPersonenErrors()
    {
        await LoadErrorListAsync(
            "Personen-Fehler",
            PersonenErrorsList,
            () => _fehlerlistenService.GetPersonenErrorsAsync(
                UpdateProgress,
                item => PersonenErrorsList.Add(item)));
    }

    /// <summary>
    /// Command für: Familien mit Fehlern laden.
    /// </summary>
    [RelayCommand]
    private async void LoadFamilienErrors()
    {
        await LoadErrorListAsync(
            "Familien-Fehler",
            FamilienErrorsList,
            () => _fehlerlistenService.GetFamilienErrorsAsync(
                UpdateProgress,
                item => FamilienErrorsList.Add(item)));
    }

    /// <summary>
    /// Command für: Orte mit Fehlern laden.
    /// </summary>
    [RelayCommand]
    private async void LoadOerterErrors()
    {
        await LoadErrorListAsync(
            "Orte-Fehler",
            OerterErrorsList,
            () => _fehlerlistenService.GetOerterErrorsAsync(
                UpdateProgress,
                item => OerterErrorsList.Add(item)));
    }

    /// <summary>
    /// Command für: Sortierrichtung umschalten.
    /// </summary>
    [RelayCommand]
    private void ToggleSorting()
    {
        SortAscending = !SortAscending;
    }

    /// <summary>
    /// Command für: Alle Listen löschen.
    /// </summary>
    [RelayCommand]
    private void ClearAllLists()
    {
        PersonenOhneElternList.Clear();
        PersonenErrorsList.Clear();
        FamilienErrorsList.Clear();
        OerterErrorsList.Clear();
        ProgressValue = 0;
        StatusMessage = "Listen geleert";
    }

    /// <summary>
    /// Zentrale Hilfsmethode zum Laden einer Fehlliste.
    /// Implementiert die EVA-Logik:
    /// - EINGABE: Liste wird geleert, Service wird aufgerufen
    /// - VERARBEITUNG: Items werden inkrementell hinzugefügt (via itemCallback)
    /// - AUSGABE: Progress-Feedback, Status-Message, UI wird updated
    /// </summary>
    private async System.Threading.Tasks.Task LoadErrorListAsync(
        string listName,
        ObservableCollection<ErrorListItem> targetList,
        Func<System.Threading.Tasks.Task<ErrorListResult>> loadOperation)
    {
        IsLoading = true;
        StatusMessage = $"{listName} wird geladen...";
        ProgressValue = 0;
        targetList.Clear();

        try
        {
            var result = await loadOperation();

            if (result.IsSuccess)
            {
                StatusMessage = $"{listName}: {targetList.Count} Einträge geladen";
                ProgressValue = 100;
            }
            else
            {
                StatusMessage = $"Fehler beim Laden: {result.ErrorMessage}";
                ProgressValue = 0;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ausnahme: {ex.Message}";
            ProgressValue = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Callback für Service-Progress-Updates.
    /// Wird von Service aufgerufen, um Progress-Bar zu aktualisieren.
    /// </summary>
    private void UpdateProgress(int current, int maximum)
    {
        if (maximum > 0)
        {
            ProgressMaximum = maximum;
            ProgressValue = (int)((current / (double)maximum) * 100);
        }
    }
}
