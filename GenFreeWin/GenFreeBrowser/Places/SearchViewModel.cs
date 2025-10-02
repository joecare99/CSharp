using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeBrowser.Places.Interface;

namespace GenFreeBrowser.Places;

/// <summary>
/// ViewModel for searching places via one or more authorities.
/// (UI-Technologie agnostisch – kann in WPF, WinForms, Console-TUI etc. benutzt werden)
/// </summary>
public sealed partial class SearchViewModel : ObservableObject
{
    private readonly ISearchHistoryService _history;
    private readonly IPlaceSearchService _placeSearchService;
    private readonly System.Collections.Generic.IEnumerable<IPlaceAuthority> _authorities;
    private CancellationTokenSource? _cts;

    public ObservableCollection<string> RecentQueries { get; } = new();
    public ObservableCollection<IPlaceAuthority> Authorities { get; } = new();
    public ObservableCollection<PlaceResult> Results { get; } = new();

    [ObservableProperty]
    private string _searchText = "<Text>";

    [ObservableProperty]
    private IPlaceAuthority? _selectedAuthority;

    [ObservableProperty]
    private PlaceResult? _selectedResult;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _status;

    public SearchViewModel(ISearchHistoryService history,
                           IPlaceSearchService placeSearchService,
                           System.Collections.Generic.IEnumerable<IPlaceAuthority> authorities)
    {
        _history = history;
        _placeSearchService = placeSearchService;
        _authorities = authorities;
        foreach (var a in authorities) Authorities.Add(a);
        _ = LoadHistoryAsync();
    }

    private async Task LoadHistoryAsync()
    {
        await _history.LoadAsync();
        RecentQueries.Clear();
        foreach (var q in _history.Items) RecentQueries.Add(q);
    }

    private bool CanSearch() => !IsBusy && !string.IsNullOrWhiteSpace(SearchText);

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task SearchAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var ct = _cts.Token;
        try
        {
            IsBusy = true;
            Status = "Suche läuft ...";
            Results.Clear();
            var query = new PlaceQuery(SearchText.Trim());

            System.Collections.Generic.IReadOnlyList<PlaceResult> all;
            if (SelectedAuthority is not null)
                all = await SelectedAuthority.SearchAsync(query, ct);
            else
                all = await _placeSearchService.SearchAllAsync(query, ct);

            foreach (var r in all.OrderBy(r => r.Name)) Results.Add(r);
            Status = $"{Results.Count} Treffer";
            _history.Register(SearchText.Trim());
            SyncHistory();
        }
        catch (OperationCanceledException)
        {
            Status = "Abgebrochen";
        }
        catch (Exception ex)
        {
            Status = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void UseHistoryItem(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        SearchText = text;
        SearchCommand.NotifyCanExecuteChanged();
    }

    private void SyncHistory()
    {
        RecentQueries.Clear();
        foreach (var q in _history.Items) RecentQueries.Add(q);
    }
}
