using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GenFreeBrowser.Places;

namespace PlaceAuthorityConsoleDemo;

public sealed partial class ConsoleAppViewModel : ObservableObject
{
    public SearchViewModel Search { get; }

    [ObservableProperty]
    private string _resultsText = string.Empty;

    [ObservableProperty]
    private string _status = "Enter=Search  ESC=Exit  Pfeil?/?=History";

    [ObservableProperty]
    private string _historyBanner = "History: <leer>";

    [ObservableProperty]
    private int _historyIndex = -1;

    public ConsoleAppViewModel(SearchViewModel searchVm)
    {
        Search = searchVm;
        Search.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SearchViewModel.Status))
            {
                Status = Search.Status ?? string.Empty;
            }
        };
    }

    private bool CanSearch() => !string.IsNullOrWhiteSpace(Search.SearchText) && !Search.IsBusy;

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task DoSearchAsync()
    {
        if (Search.SearchCommand.CanExecute(null))
            await (Search.SearchCommand as IAsyncRelayCommand)!.ExecuteAsync(null);
        BuildResultsText();
        UpdateHistoryBanner();
    }

    [RelayCommand]
    private void HistoryPrev()
    {
        if (Search.RecentQueries.Count == 0) return;
        HistoryIndex = (HistoryIndex + 1) % Search.RecentQueries.Count;
        Search.SearchText = Search.RecentQueries[HistoryIndex];
        DoSearchCommand.NotifyCanExecuteChanged();
        UpdateHistoryBanner();
    }

    [RelayCommand]
    private void HistoryNext()
    {
        if (Search.RecentQueries.Count == 0) return;
        HistoryIndex--;
        if (HistoryIndex < 0) HistoryIndex = Search.RecentQueries.Count - 1;
        Search.SearchText = Search.RecentQueries[HistoryIndex];
        DoSearchCommand.NotifyCanExecuteChanged();
        UpdateHistoryBanner();
    }

    public void AppendChar(char ch)
    {
        Search.SearchText += ch;
        HistoryIndex = -1;
        DoSearchCommand.NotifyCanExecuteChanged();
    }

    public void Backspace()
    {
        if (Search.SearchText.Length == 0) return;
        Search.SearchText = Search.SearchText[..^1];
        HistoryIndex = -1;
        DoSearchCommand.NotifyCanExecuteChanged();
    }

    public void UpdateHistoryBanner()
    {
        var arr = Search.RecentQueries.Take(8).ToArray();
        HistoryBanner = arr.Length == 0 ? "History: <leer>" : "History: " + string.Join(" | ", arr);
    }

    private void BuildResultsText()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"-- Results ({Search.Results.Count}) --");
        int i = 1;
        foreach (var p in Search.Results)
        {
            var hier = (p.Hierarchy?.Count ?? 0) > 0 ? " | " + string.Join(" > ", p.Hierarchy.Take(4)) : string.Empty;
            sb.AppendLine($"{i++,3}. {p.Name} [{p.Source}] {p.Location.Lon:F4},{p.Location.Lat:F4}{hier}");
        }
        if (Search.Results.Count == 0) sb.AppendLine("(keine Treffer)");
        ResultsText = sb.ToString();
    }
}
