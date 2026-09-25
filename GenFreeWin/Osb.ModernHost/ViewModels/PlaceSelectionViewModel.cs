using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Osb.Core.Selection;
using Osb.ModernHost.Models;

namespace Osb.ModernHost.ViewModels;

public sealed partial class PlaceSelectionViewModel : ObservableObject, IHostPageViewModel, IPageNavigationRequestSource
{
    public PlaceSelectionViewModel()
    {
        SelectedPlaces.CollectionChanged += SelectedPlaces_CollectionChanged;
    }

    public event EventHandler<PageNavigationRequestedEventArgs>? NavigationRequested;

    public string Description => "Orte suchen, auswählen und die Auswahl als .OSP-kompatiblen Text prüfen.";

    public bool IsSearchAvailable => false;

    public ModernHostPage Page => ModernHostPage.PlaceSelection;

    public ObservableCollection<string> SearchResults { get; } = new();

    public string SerializedSelectionText => PlaceSelectionTextCodec.Serialize(SelectedPlaces);

    public ObservableCollection<string> SelectedPlaces { get; } = new();

    public string Title => "Ortsauswahl";

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string? _selectedPlace;

    [ObservableProperty]
    private string? _selectedSearchResult;

    [ObservableProperty]
    private string _selectionText = string.Empty;

    [ObservableProperty]
    private string _statusText = "Die Ortsdatenbank ist im modernen Host noch nicht angebunden.";

    [RelayCommand]
    private void ApplySelectionText()
    {
        try
        {
            var entries = PlaceSelectionTextCodec.Deserialize(SelectionText);
            SelectedPlaces.Clear();

            foreach (var entry in entries)
            {
                SelectedPlaces.Add(entry);
            }

            StatusText = entries.Count == 0
                ? "Die Ortsauswahl wurde geleert."
                : $"{entries.Count} Ort(e) wurden aus dem .OSP-kompatiblen Text übernommen.";
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

    private void SelectedPlaces_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        OnPropertyChanged(nameof(SerializedSelectionText));
    }
}
