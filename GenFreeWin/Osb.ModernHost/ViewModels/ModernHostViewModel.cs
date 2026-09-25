using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;

namespace Osb.ModernHost.ViewModels;

public sealed partial class ModernHostViewModel : ObservableObject, IDisposable
{
    private readonly IHostNavigationService _navigationService;

    public ModernHostViewModel(
        ModernHostStartupService startupService,
        IHostNavigationService navigationService)
    {
        State = startupService.CreateState();
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        ActivePageViewModel = _navigationService.ActivePageViewModel;
        _navigationService.ActivePageChanged += NavigationService_ActivePageChanged;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActivePageTitle))]
    private IHostPageViewModel? _activePageViewModel;

    public ModernHostState State { get; }

    public string ActivePageTitle => ActivePageViewModel?.Title ?? string.Empty;

    public bool IsReady => State.IsReady;

    public string StatusText => IsReady
        ? string.Format(
            "Tenant bereit:\r\n{0}\r\n\r\nDie moderne Host-Komposition ist aktiv. Fachworkflows werden schrittweise über Osb.Core angebunden.",
            State.Tenant!.TenantRootPath)
        : string.Format(
            "Der Host konnte nicht gestartet werden:\r\n{0}\r\n\r\nAufruf: Osb.ModernHost.exe --tenant <absoluter-Pfad>",
            State.ErrorMessage ?? "Unbekannter Startfehler.");

    public void Dispose()
    {
        _navigationService.ActivePageChanged -= NavigationService_ActivePageChanged;
    }

    private void NavigationService_ActivePageChanged(object? sender, PageChangedEventArgs eventArgs)
    {
        ActivePageViewModel = eventArgs.ActivePageViewModel;
    }
}
