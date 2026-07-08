using System;
using System.Windows.Forms;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services;
using Gen_FreeWin.Main;
using GenFree;
using GenFree.Helper;

namespace Gen_FreeWin.Views;

/// <summary>
/// Refaktorierte Codenlogik für Fehlerli-Dialog.
/// 
/// Trennung nach EVA-Prinzip und Verantwortlichkeiten:
/// - InputData: Dialog-Commands und Benutzerinteraktion
/// - Processing: Fehllisten-Generierung (delegiert an FehlerlistenService)
/// - OutputData: Listbox-Updates und UI-State
/// 
/// Diese Klasse enthält die modernisierte, zerteilte Business-Logik aus dem ursprünglichen
/// monolithischen Fehlerli_Click Handler.
/// </summary>
internal class FehlerliRefactored
{
    private readonly IFehlerlistenService _fehlerlistenService;
    private readonly Fehlerli _view;

    public FehlerliRefactored(Fehlerli view, IFehlerlistenService fehlerlistenService)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _fehlerlistenService = fehlerlistenService ?? throw new ArgumentNullException(nameof(fehlerlistenService));
    }

    /// <summary>
    /// Kommando: Lade Fehlliste für Personen ohne Eltern.
    /// Demonstriert EVA-Prinzip:
    /// - INPUT: User klickt Button
    /// - PROCESS: FehlerlistenService liefert Items via Callbacks
    /// - OUTPUT: ListBox wird aktualisiert
    /// </summary>
    public async void LoadPersonsWithoutParentsCommand()
    {
        try
        {
            _view.ProgressBar1.Minimum = 0;
            _view.ProgressBar1.Maximum = 100;

            var result = await _fehlerlistenService.GetPersonenOhneElternAsync(
                progressCallback: (current, max) =>
                {
                    if (max > 0)
                        _view.ProgressBar1.Value = (int)((double)current / max * 100);
                },
                itemAddCallback: (item) =>
                {
                    // AUSGABE: Listbox wird mit Items gefüllt (ohne pauschales Clear)
                    _view.List1.Items.Add(new ErrorListItemDisplay(item.DisplayText, item.Id));
                });

            if (!result.IsSuccess)
            {
                MessageBox.Show($"Fehler: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception beim Laden der Fehlliste: {ex.Message}");
        }
    }

    /// <summary>
    /// Kommando: Lade Fehlliste für Personen.
    /// Demonstriert Separation von Prädikat und Formatierung.
    /// </summary>
    public async void LoadPersonErrorsCommand()
    {
        try
        {
            _view.ProgressBar1.Minimum = 0;
            _view.ProgressBar1.Maximum = 100;

            var result = await _fehlerlistenService.GetPersonenErrorsAsync(
                progressCallback: UpdateProgress,
                itemAddCallback: (item) => _view.List1.Items.Add(new ErrorListItemDisplay(item.DisplayText, item.Id)));

            if (!result.IsSuccess)
                MessageBox.Show($"Fehler: {result.ErrorMessage}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Kommando: Lade Fehlliste für Familien.
    /// </summary>
    public async void LoadFamilyErrorsCommand()
    {
        try
        {
            _view.ProgressBar1.Minimum = 0;
            _view.ProgressBar1.Maximum = 100;

            var result = await _fehlerlistenService.GetFamilienErrorsAsync(
                progressCallback: UpdateProgress,
                itemAddCallback: (item) => _view.List1.Items.Add(new ErrorListItemDisplay(item.DisplayText, item.Id)));

            if (!result.IsSuccess)
                MessageBox.Show($"Fehler: {result.ErrorMessage}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Kommando: Lade Fehlliste für Orte.
    /// </summary>
    public async void LoadPlaceErrorsCommand()
    {
        try
        {
            _view.ProgressBar1.Minimum = 0;
            _view.ProgressBar1.Maximum = 100;

            var result = await _fehlerlistenService.GetOerterErrorsAsync(
                progressCallback: UpdateProgress,
                itemAddCallback: (item) => _view.List2.Items.Add(new ErrorListItemDisplay(item.DisplayText, item.Id)));

            if (!result.IsSuccess)
                MessageBox.Show($"Fehler: {result.ErrorMessage}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Hilfsmethode: Progress-Update für ProgressBar.
    /// Kleine, spezialisierte Methode statt inline in Lambda.
    /// </summary>
    private void UpdateProgress(int current, int max)
    {
        if (max > 0)
            _view.ProgressBar1.Value = Math.Min((int)((double)current / max * 100), 100);
    }

    /// <summary>
    /// Hilfsmethode: Sortierung umschalten basierend auf RadioButton-State.
    /// </summary>
    public void ToggleSorting(bool sorted)
    {
        _view.List1.Sorted = sorted;
    }

    /// <summary>
    /// Hilfsmethode: Listenansicht wechseln (List1 ↔ List2).
    /// </summary>
    public void SwitchListView(bool showList1)
    {
        _view.List1.Visible = showList1;
        _view.List2.Visible = !showList1;
    }

    /// <summary>
    /// Hilfsmethode: Liste löschen und State zurücksetzen.
    /// Modernes C# ohne VB-style Goto; klar strukturiert.
    /// </summary>
    public void ClearAllLists()
    {
        _view.List1.Items.Clear();
        _view.List2.Items.Clear();
        _view.ProgressBar1.Value = 0;
        _view.Label4.Text = "";
    }

    /// <summary>
    /// Hilfsmethode: Doppelklick-Navigation auf Listbox-Items.
    /// Delegiert Navigation als Event an den UI-Layer statt direkt aufzurufen.
    /// </summary>
    public event Action<ErrorListType, int> OnNavigationRequested;

    public void HandleListDoubleClick(object selectedItem, ErrorListType listType)
    {
        if (selectedItem is ErrorListItemDisplay item)
        {
            OnNavigationRequested?.Invoke(listType, item.Id);
        }
    }
}

/// <summary>
/// Hilfsklasse: ListItem für Listbox-Binding mit ID und Anzeigetext.
/// Alternative zum String-basierten Indexing im Legacy-Code.
/// HINWEIS: Um Konflikte zu vermeiden, verwenden wir diesen Namen nur in der Refactored-Klasse.
/// Im Legacy Fehlerli.cs bleibt es bei: new ListItem(displayText + "  " + id)
/// </summary>
public class ErrorListItemDisplay
{
    public string DisplayText { get; set; }
    public int Id { get; set; }

    public ErrorListItemDisplay(string displayText, int id)
    {
        DisplayText = displayText;
        Id = id;
    }

    public override string ToString() => DisplayText;
}
