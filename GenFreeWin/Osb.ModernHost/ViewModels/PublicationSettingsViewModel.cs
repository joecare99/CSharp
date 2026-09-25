using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Osb.Core.Profiles;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public sealed partial class PublicationSettingsViewModel : ObservableObject, IHostPageViewModel, IPageNavigationRequestSource
{
    public event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;

    public string Description => "Visuelle Publikationseinstellungen mit einem typisierten, noch nicht persistierten Profil.";

    public ModernHostPage Page => ModernHostPage.PublicationSettings;

    public string Title => "Einstellungen";

    [ObservableProperty]
    private bool _includeSelectedPlaces = true;

    [ObservableProperty]
    private string _footerTemplate = "{footer}";

    [ObservableProperty]
    private string _profileName = "Standardprofil";

    [ObservableProperty]
    private string _statusText = "Die Legacy-Profilpersistenz ist im modernen Host noch nicht angebunden.";

    [ObservableProperty]
    private string _titleTemplate = "{name}";

    [RelayCommand]
    private void ValidateProfile()
    {
        try
        {
            var profile = new PublicationProfile(
                1,
                ProfileName,
                IncludeSelectedPlaces ? PlaceSelectionMode.IncludeOnly : PlaceSelectionMode.ExcludeSelected,
                Array.Empty<string>(),
                TitleTemplate,
                FooterTemplate);
            StatusText = $"Profil '{profile.Name}' ist gültig; Speichern wird später angebunden.";
        }
        catch (ArgumentException exception)
        {
            StatusText = exception.Message;
        }
    }

    [RelayCommand]
    private void ReturnToMenu()
    {
        NavigationRequested?.Invoke(this, new PageNavigationRequestedEventArgs(ModernHostPage.Menu));
    }
}
