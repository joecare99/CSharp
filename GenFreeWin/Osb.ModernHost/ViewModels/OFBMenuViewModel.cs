using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public sealed partial class OFBMenuViewModel : ObservableObject, IHostPageViewModel, IPageNavigationRequestSource
{
    public event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;

    public string Description => "Wählen Sie einen Arbeitsbereich für das Ortsfamilienbuch.";

    public ModernHostPage Page => ModernHostPage.Menu;

    public string Title => "Ortsfamilienbuch";

    [RelayCommand]
    private void OpenFamilyPublication()
    {
        RequestNavigation(ModernHostPage.FamilyPublication);
    }
   
    [RelayCommand]
    private void ContinueFamilyPublication()
    {
        RequestNavigation(ModernHostPage.FamilyPublication);
    }

    [RelayCommand]
    private void OpenPlaceSelection()
    {
        RequestNavigation(ModernHostPage.PlaceSelection);
    }

    [RelayCommand]
    private void OpenPublicationSettings()
    {
        RequestNavigation(ModernHostPage.PublicationSettings);
    }

    [RelayCommand]
    private void OpenStatistics()
    {
        RequestNavigation(ModernHostPage.Statistics);
    }

    private void RequestNavigation(ModernHostPage page)
    {
        NavigationRequested?.Invoke(this, new PageNavigationRequestedEventArgs(page));
    }
}
