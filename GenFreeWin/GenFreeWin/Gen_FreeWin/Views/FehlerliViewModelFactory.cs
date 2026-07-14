using GenFreeWin.Services;
using GenFreeWin.ViewModels;
using System;
using System.Windows.Forms;

namespace GenFreeWin.Views;

/// <summary>
/// Factory für die Initialisierung und Bindung eines FehlerliViewModel an die Fehlerli-View.
/// 
/// Trennt die MVVM-Initialisierungslogik von der Legacy-View ab, um das Risiko zu minimieren.
/// Kann später erweitert werden, um echte DI-Container-Integration zu handhaben.
/// </summary>
internal static class FehlerliViewModelFactory
{
    /// <summary>
    /// Erstellt und konfiguriert ein FehlerliViewModel für eine gegebene Fehlerli-Form.
    /// 
    /// Rückgabewert: Konfiguriertes ViewModel, das an die View (Bindings) angeschlossen werden kann.
    /// 
    /// HINWEIS: Für echte Produktiv-Nutzung sollte hier ein echter DI-Container (Microsoft.Extensions.DependencyInjection) verwendet werden.
    /// </summary>
    internal static FehlerliViewModel CreateAndBindViewModel(Fehlerli view)
    {
        if (view == null)
            throw new ArgumentNullException(nameof(view));

        // EINGABE: Service-Instanz erzeugen (später aus DI)
        var service = new FehlerlistenService();

        // VERARBEITUNG: ViewModel mit Service initialisieren
        var viewModel = new FehlerliViewModel(service);

        // AUSGABE: Bindings aufsetzen
        SetupDataBindings(view, viewModel);

        return viewModel;
    }

    /// <summary>
    /// Hilfsmethode: Bindet ObservableCollections des ViewModels an WinForms ListBox-Controls.
    /// 
    /// Verwendet BindingSource als Adapter zwischen ObservableCollection und ListBox.
    /// Dies ermöglicht automatisches Tracking von Änderungen und UI-Updates.
    /// </summary>
    internal static void SetupDataBindings(Fehlerli view, FehlerliViewModel viewModel)
    {
        if (view == null || viewModel == null)
            return;

        try
        {
            // Adapter: BindingSource verbindet ObservableCollection mit WinForms
            // Dies ist notwendig, da ListBox.DataSource standardmäßig ObservableCollection nicht unterstützt

            var personenOhneElternBinding = new BindingSource
            {
                DataSource = viewModel.PersonenOhneElternList
            };

            var oerterErrorsBinding = new BindingSource
            {
                DataSource = viewModel.OerterErrorsList
            };

            // Binde ListBox-Controls an ihre entsprechenden Quellen
            // List1 zeigt Personen ohne Eltern
            if (view.List1 != null)
            {
                view.List1.DataSource = personenOhneElternBinding;
                view.List1.DisplayMember = "DisplayText"; // Zeige nur Text, nicht das ganze Objekt
                view.List1.ValueMember = "Id"; // Verwende Id als Selektions-Value
            }

            // List2 zeigt weitere Fehler (z.B. Orte)
            if (view.List2 != null)
            {
                view.List2.DataSource = oerterErrorsBinding;
                view.List2.DisplayMember = "DisplayText";
                view.List2.ValueMember = "Id";
            }

            // HINWEIS: Weitere Listen (PersonenErrorsList, FamilienErrorsList) 
            // können später an zusätzliche ListBox-Controls gebunden werden,
            // falls diese in der Form existieren.

            // Bei Bedarf können hier auch Commands an Buttons gebunden werden:
            // view.Button1.Click += (s, e) => viewModel.LoadPersonenOhneElternCommand.Execute(null);
        }
        catch (Exception ex)
        {
            // Binding-Fehler loggen, aber nicht crashen
            System.Diagnostics.Debug.WriteLine($"Fehler beim Einrichten der Bindings: {ex.Message}");
        }
    }
}
