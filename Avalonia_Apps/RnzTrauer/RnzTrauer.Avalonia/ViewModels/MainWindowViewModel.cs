using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;
using RnzTrauer.Places;

namespace RnzTrauer.Avalonia.ViewModels;

/// <summary>MVVM replacement for the form, data source, filter frame, and detail frame.</summary>
public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly INoticeRepository _repository;
    private readonly INoticeTextParser _parser;
    private readonly IExportService _exporter;
    private readonly ICoordinateSchemaProbe _coordinateSchemaProbe;
    private readonly IPlaceCoordinateStore _coordinateStore;
    private IReadOnlyCollection<string> _places = Array.Empty<string>();

    public MainWindowViewModel(
        INoticeRepository repository,
        INoticeTextParser parser,
        IExportService exporter,
        ICoordinateSchemaProbe coordinateSchemaProbe,
        IPlaceCoordinateStore coordinateStore)
    {
        _repository = repository;
        _parser = parser;
        _exporter = exporter;
        _coordinateSchemaProbe = coordinateSchemaProbe;
        _coordinateStore = coordinateStore;
        _ = LoadAsync();
        _ = ProbeCoordinateSchemaAsync();
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
    [ObservableProperty] private string _coordinateSchemaStatus = "Not checked";
    [ObservableProperty] private string _coordinateSchemaDiagnostic = "Schema capability has not been checked.";
    [ObservableProperty] private string _coordinateSchemaDiagnosticCode = "schema.unverified";
    [ObservableProperty] private bool _coordinatePersistenceAvailable;
    [ObservableProperty] private string _coordinatePlace = string.Empty;
    [ObservableProperty] private string _coordinateLatitude = string.Empty;
    [ObservableProperty] private string _coordinateLongitude = string.Empty;
    [ObservableProperty] private string _coordinateSource = string.Empty;
    [ObservableProperty] private bool _coordinateIsApproximate;

    /// <summary>Legacy-style linked-record display text shown in the detail header.</summary>
    public string SelectedNoticeDescription => SelectedNotice?.Description ?? "<no selection>";

    /// <summary>Current notice source path shown in the footer-like detail line.</summary>
    public string SelectedNoticePath => SelectedNotice?.Path ?? "<no path>";

    partial void OnSelectedNoticeChanged(DeathNotice? value)
    {
        CoordinatePlace = value?.Place ?? string.Empty;
        CoordinateLatitude = string.Empty;
        CoordinateLongitude = string.Empty;
        CoordinateSource = string.Empty;
        CoordinateIsApproximate = false;
        CoordinateSchemaDiagnostic = value?.Place is null
            ? "Select a notice with a place to edit coordinates."
            : "Place selected; load stored coordinates when needed.";
        CoordinateSchemaDiagnosticCode = value?.Place is null
            ? "coordinate.place_unavailable"
            : "coordinate.place_selected";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        try { Status = "Loading…"; _places = await _repository.GetPlaceNamesAsync(); var results = await _repository.FindAsync(new NoticeFilter(OrderNumberPrefix, KeywordContains, SelectedQueue)); Notices.Clear(); foreach (var notice in results) Notices.Add(notice); Status = $"{Notices.Count} records"; }
        catch (Exception ex) { Status = $"Database unavailable: {ex.Message}"; }
    }

    [RelayCommand]
    private async Task ProbeCoordinateSchemaAsync()
    {
        CoordinateSchemaStatus = "Checking…";
        CoordinatePersistenceAvailable = false;
        try
        {
            var report = await _coordinateSchemaProbe.ProbeAsync();
            CoordinateSchemaStatus = report.Status.ToString();
            CoordinateSchemaDiagnostic = report.Diagnostic;
            CoordinateSchemaDiagnosticCode = report.DiagnosticCode;
            CoordinatePersistenceAvailable = report.CanPersist;
        }
        catch (Exception exception)
        {
            CoordinateSchemaStatus = "Unverified";
            CoordinateSchemaDiagnostic = $"Schema probe failed: {exception.Message}";
            CoordinateSchemaDiagnosticCode = "schema.probe_exception";
        }
    }

    [RelayCommand]
    private async Task LoadCoordinateAsync()
    {
        if (string.IsNullOrWhiteSpace(CoordinatePlace))
        {
            CoordinateSchemaDiagnostic = "Enter a place before loading coordinates.";
            CoordinateSchemaDiagnosticCode = "coordinate.place_required";
            return;
        }

        try
        {
            var coordinate = await _coordinateStore.GetAsync(CoordinatePlace);
            if (coordinate is null)
            {
                CoordinateSchemaDiagnostic = "No stored coordinates were found for this place.";
                CoordinateSchemaDiagnosticCode = "coordinate.not_found";
                CoordinateLatitude = string.Empty;
                CoordinateLongitude = string.Empty;
                CoordinateSource = string.Empty;
                CoordinateIsApproximate = false;
                return;
            }

            CoordinatePlace = coordinate.Place;
            CoordinateLatitude = coordinate.Latitude.ToString(CultureInfo.InvariantCulture);
            CoordinateLongitude = coordinate.Longitude.ToString(CultureInfo.InvariantCulture);
            CoordinateSource = coordinate.Source ?? string.Empty;
            CoordinateIsApproximate = coordinate.IsApproximate;
            CoordinateSchemaDiagnostic = "Stored coordinates loaded.";
            CoordinateSchemaDiagnosticCode = "coordinate.loaded";
        }
        catch (Exception exception)
        {
            CoordinateSchemaDiagnostic = $"Coordinate load failed: {exception.Message}";
            CoordinateSchemaDiagnosticCode = "coordinate.load_failed";
        }
    }

    [RelayCommand]
    private async Task SaveCoordinateAsync()
    {
        if (!CoordinatePersistenceAvailable)
        {
            CoordinateSchemaDiagnostic = "Coordinate persistence is unavailable until the schema is confirmed.";
            CoordinateSchemaDiagnosticCode = "coordinate.persistence_unavailable";
            return;
        }

        if (string.IsNullOrWhiteSpace(CoordinatePlace)
            || !double.TryParse(CoordinateLatitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude)
            || !double.TryParse(CoordinateLongitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
        {
            CoordinateSchemaDiagnostic = "Place, latitude, and longitude must be valid invariant values.";
            CoordinateSchemaDiagnosticCode = "coordinate.invalid_input";
            return;
        }

        try
        {
            await _coordinateStore.SaveAsync(
                new PlaceCoordinate(
                    CoordinatePlace,
                    latitude,
                    longitude,
                    string.IsNullOrWhiteSpace(CoordinateSource) ? "avalonia" : CoordinateSource,
                    CoordinateIsApproximate));
            CoordinateSchemaDiagnostic =
                "Coordinates saved. Source and approximation metadata are not persisted by the current schema.";
            CoordinateSchemaDiagnosticCode = "coordinate.saved_partial_metadata";
        }
        catch (Exception exception)
        {
            CoordinateSchemaDiagnostic = $"Coordinate save failed: {exception.Message}";
            CoordinateSchemaDiagnosticCode = "coordinate.save_failed";
        }
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
