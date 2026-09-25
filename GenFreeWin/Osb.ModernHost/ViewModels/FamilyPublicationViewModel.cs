using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public sealed partial class FamilyPublicationViewModel : ObservableObject, IHostPageViewModel, IPageNavigationRequestSource
{
    public event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;
    public string Description => "Familienauswahl, Ausgabeoptionen und Vorschau des Ortsfamilienbuchs.";
    public ModernHostPage Page => ModernHostPage.FamilyPublication;
    public string Title => "Ortsfamilienbuch erstellen";
    [ObservableProperty] 
    private string _statusText = "Die Auswahltraversierung und Dokumenterzeugung benötigen weiterhin eine providerneutrale Quelle und anonymisierte Fixtures.";

    [RelayCommand]
    private void CreatePublication() => StatusText = "Die Publikation kann noch nicht erstellt werden, weil die Datenquelle nicht angebunden ist.";

    [RelayCommand]
    private void ReturnToMenu() => NavigationRequested?.Invoke(this, new PageNavigationRequestedEventArgs(ModernHostPage.Menu));
}
