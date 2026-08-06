using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Avalonia.ViewModels;

/// <summary>MVVM replacement for the form, data source, filter frame, and detail frame.</summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly INoticeRepository _repository;
    private readonly INoticeTextParser _parser;
    private readonly IExportService _exporter;
    private IReadOnlyCollection<string> _places = Array.Empty<string>();

    public MainWindowViewModel(INoticeRepository repository, INoticeTextParser parser, IExportService exporter)
    {
        _repository = repository; _parser = parser; _exporter = exporter;
        _ = LoadAsync();
    }

    /// <summary>Main editable record list rendered in the DB tab grid.</summary>
    public ObservableCollection<DeathNotice> Notices { get; } = [];

    /// <summary>Queue list equivalent to the legacy filter frame options.</summary>
    public ObservableCollection<NoticeFilterKind> ReviewQueues { get; } = new((NoticeFilterKind[])Enum.GetValues(typeof(NoticeFilterKind)));

    /// <summary>Category choices mirroring the historical rubrik radio-group values.</summary>
    public ObservableCollection<AdvertisementCategory> Categories { get; } = new((AdvertisementCategory[])Enum.GetValues(typeof(AdvertisementCategory)));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(ParseSelectedCommand))]
    [NotifyPropertyChangedFor(nameof(SelectedNoticeDescription))]
    [NotifyPropertyChangedFor(nameof(SelectedNoticePath))]
    private DeathNotice? _selectedNotice;

    [ObservableProperty] private string _orderNumberPrefix = string.Empty;
    [ObservableProperty] private string _keywordContains = string.Empty;
    [ObservableProperty] private NoticeFilterKind _selectedQueue;
    [ObservableProperty] private string _status = "Loading notices…";
    [ObservableProperty] private string _webSourcePath = string.Empty;
    [ObservableProperty] private string _schemaPath = "Huebner_Schema.txt";
    [ObservableProperty] private bool _autoContinue;
    [ObservableProperty] private bool _appendToDatabase = true;
    [ObservableProperty] private bool _verbose;
    [ObservableProperty] private bool _verboseSchema = true;
    [ObservableProperty] private string _dbServer = Environment.GetEnvironmentVariable("RNZ_DB_SERVER") ?? "localhost";
    [ObservableProperty] private string _dbPort = Environment.GetEnvironmentVariable("RNZ_DB_PORT") ?? "3306";
    [ObservableProperty] private string _dbUser = Environment.GetEnvironmentVariable("RNZ_DB_USER") ?? "root";
    [ObservableProperty] private string _dbName = Environment.GetEnvironmentVariable("RNZ_DB_NAME") ?? "RNZ";

    /// <summary>Legacy-style linked-record display text shown in the detail header.</summary>
    public string SelectedNoticeDescription => SelectedNotice?.Description ?? "<no selection>";

    /// <summary>Current notice source path shown in the footer-like detail line.</summary>
    public string SelectedNoticePath => SelectedNotice?.Path ?? "<no path>";

    [RelayCommand]
    private async Task LoadAsync()
    {
        try { Status = "Loading…"; _places = await _repository.GetPlaceNamesAsync(); var results = await _repository.FindAsync(new NoticeFilter(OrderNumberPrefix, KeywordContains, SelectedQueue)); Notices.Clear(); foreach (var notice in results) Notices.Add(notice); Status = $"{Notices.Count} records"; }
        catch (Exception ex) { Status = $"Database unavailable: {ex.Message}"; }
    }

    /// <summary>Applies a queue by symbolic name to support compact legacy-style queue buttons.</summary>
    /// <param name="queueName">Name of <see cref="NoticeFilterKind"/> enum value.</param>
    [RelayCommand]
    private async Task LoadQueueAsync(string? queueName)
    {
        if (string.IsNullOrWhiteSpace(queueName) || !Enum.TryParse<NoticeFilterKind>(queueName, out var queue))
            return;
        SelectedQueue = queue;
        await LoadAsync();
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task SaveAsync() { if (SelectedNotice is null) return; await _repository.SaveAsync(SelectedNotice); Status = "Record saved."; }
    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void ParseSelected() { if (SelectedNotice is null) return; var facts = _parser.Parse(SelectedNotice, SelectedNotice.Text ?? string.Empty, _places); SelectedNotice.BirthDate ??= facts.BirthDate; SelectedNotice.DeathDate ??= facts.DeathDate; SelectedNotice.BurialDate ??= facts.BurialDate; SelectedNotice.MaidenName ??= facts.MaidenName; SelectedNotice.Place ??= facts.Place; if (facts.AdjustedCategory is not null) SelectedNotice.Category = facts.AdjustedCategory.Value; Status = "Text parsed; extracted values are ready for review and Save."; }
    [RelayCommand] private Task ExportCsvAsync() => _exporter.ExportCsvAsync("RNZ-Anzeigen.csv", Notices);
    [RelayCommand] private Task ExportGedcomAsync() => _exporter.ExportGedcomAsync("RNZ-Anzeigen.ged", Notices);
    private bool HasSelection() => SelectedNotice is not null;
}
