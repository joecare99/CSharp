using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Osb.ModernHost.Models;
using Osb.ModernHost.Services;

namespace Osb.ModernHost.ViewModels;

public sealed partial class StatisticsViewModel : ObservableObject, IHostPageViewModel, IPageNavigationRequestSource
{
    public StatisticsViewModel(StatisticsDisplayService displayService)
    {
        ArgumentNullException.ThrowIfNull(displayService);
        Buckets = displayService.CreateSampleBuckets();
    }

    public event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;

    public string Description => "Altersgruppen in der Statistikansicht.";

    public ModernHostPage Page => ModernHostPage.Statistics;

    public string Title => "Statistik";

    public string SampleNotice => "Beispieldarstellung aus dem deterministischen Core-Modell; keine Tenant- oder Datenbankstatistik.";

    public IReadOnlyList<StatisticsBucketDisplayItem> Buckets { get; }

    public int SampleEntryCount => 13;

    [ObservableProperty]
    private string _statusText = "Die Datenbankanbindung für echte Statistiken ist im modernen Host noch nicht verfügbar.";

    [RelayCommand]
    private void RefreshSample()
    {
        StatusText = "Die deterministische Beispieldarstellung wurde aktualisiert.";
    }

    [RelayCommand]
    private void ReturnToMenu() => NavigationRequested?.Invoke(
        this,
        new PageNavigationRequestedEventArgs(ModernHostPage.Menu));
}
