using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TileSetAnimator.Models;
using TileSetAnimator.Persistence;
using TileSetAnimator.Properties;
using TileSetAnimator.Services;

namespace TileSetAnimator.ViewModels;

/// <summary>
/// Provides the UI state for the tile set animator.
/// </summary>
 public partial class MainViewModel : ObservableObject
{
    private readonly IFileDialogService fileDialogService;
    private readonly ITileSetService tileSetService;
    private readonly IAnimationDetectionService animationDetectionService;
    private readonly IAnimationPreviewService animationPreviewService;
    private readonly ITileSetPersistence tileSetPersistence;
    private readonly ITileEnumSerializer tileEnumSerializer;

    private readonly ObservableCollection<MiniMapDefinitionViewModel> miniMaps = new();
    private static readonly ObservableCollection<MiniMapSlotViewModel> emptyMiniMapSlots = new();
    private const string DefaultMiniMapNameFormat = "Mini-Map {0}";

    private string? currentTileSheetPath;
    private TileSetClassMetadata? currentClassMetadata;
    private bool suppressMiniMapNotifications;
    private CancellationTokenSource? statePersistenceCancellation;

    public MainViewModel(
        IFileDialogService fileDialogService,
        ITileSetService tileSetService,
        IAnimationDetectionService animationDetectionService,
        IAnimationPreviewService animationPreviewService,
        ITileSetPersistence tileSetPersistence,
        ITileEnumSerializer tileEnumSerializer)
    {
        this.fileDialogService = fileDialogService;
        this.tileSetService = tileSetService;
        this.animationDetectionService = animationDetectionService;
        this.animationPreviewService = animationPreviewService;
        this.tileSetPersistence = tileSetPersistence;
        this.tileEnumSerializer = tileEnumSerializer;

        PendingFrames.CollectionChanged += OnPendingFramesChanged;

        LoadSheetCommand = new AsyncRelayCommand(LoadTileSheetAsync);
        DetectAnimationsCommand = new RelayCommand(DetectAnimations, HasTiles);
        SaveTileCommand = new AsyncRelayCommand(SaveTileAsync, () => SelectedTile != null && TileSheet != null);
        AddFrameCommand = new RelayCommand(AddFrame, () => SelectedTile != null);
        ClearFramesCommand = new RelayCommand(ClearFrames, () => PendingFrames.Count > 0);
        CommitAnimationCommand = new RelayCommand(CommitAnimation, () => PendingFrames.Count > 1);
        StartPreviewCommand = new RelayCommand(StartPreview, () => SelectedAnimation != null);
        StopPreviewCommand = new RelayCommand(animationPreviewService.Stop);
        PlaceTileInMiniMapCommand = new RelayCommand(PlaceSelectedTileInMiniMap, CanPlaceSelectedTileInMiniMap);
        UseSuggestedTileInMiniMapCommand = new RelayCommand(UseSuggestedTileInMiniMap, CanUseSuggestedTileInMiniMap);
        ClearMiniMapSlotCommand = new RelayCommand(ClearSelectedMiniMapSlot, CanClearSelectedMiniMapSlot);
        ClearMiniMapCommand = new RelayCommand(ClearMiniMap, HasMiniMapTiles);
        CreateMiniMapCommand = new RelayCommand(() => AddMiniMap());
        RemoveMiniMapCommand = new RelayCommand(RemoveSelectedMiniMap, CanRemoveSelectedMiniMap);
        ExportTileEnumCommand = new AsyncRelayCommand(ExportTileEnumAsync, () => Tiles.Count > 0);
        ImportTileEnumCommand = new AsyncRelayCommand(ImportTileEnumAsync);
        miniMaps.CollectionChanged += OnMiniMapsCollectionChanged;

        InitializeMiniMaps();
        SelectedMiniMap = MiniMaps.FirstOrDefault();
        SelectedMiniMapSlot = MiniMapSlots.FirstOrDefault();
        UpdateMiniMapSuggestionOptions();
        UpdateMiniMapCommands();

        StatusMessage = Resources.NoSheetLoadedMessage;
    }

    public ObservableCollection<TileDefinition> Tiles { get; } = new();

    public ObservableCollection<TileAnimation> Animations { get; } = new();

    public ObservableCollection<TileDefinition> PendingFrames { get; } = new();

    public IReadOnlyList<TileCategory> TileCategories { get; } = Enum.GetValues<TileCategory>();

    public ObservableCollection<string> SubCategoryOptions { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedTileImage))]
    [NotifyPropertyChangedFor(nameof(PreviewTileImage))]
    private BitmapSource? tileSheet;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedTileImage))]
    private TileDefinition? selectedTile;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTileImage))]
    private TileAnimation? selectedAnimation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreviewTileImage))]
    private TileDefinition? previewTile;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private int tileWidth = 16;

    [ObservableProperty]
    private int tileHeight = 16;

    [ObservableProperty]
    private int tileSpacing;

    [ObservableProperty]
    private int tileMargin;

    [ObservableProperty]
    private int minimumAnimationLength = 2;

    [ObservableProperty]
    private int frameDuration = 120;

    [ObservableProperty]
    private double zoomFactor = 4;

    [ObservableProperty]
    private string selectedTileName = string.Empty;

    [ObservableProperty]
    private string selectedTileNotes = string.Empty;

    [ObservableProperty]
    private TileCategory selectedTileCategory = TileCategory.Unknown;

    [ObservableProperty]
    private string selectedTileSubCategory = string.Empty;

    [ObservableProperty]
    private string tileSetClassKey = string.Empty;

    [ObservableProperty]
    private MiniMapDefinitionViewModel? selectedMiniMap;

    [ObservableProperty]
    private MiniMapSlotViewModel? selectedMiniMapSlot;

    public ObservableCollection<MiniMapDefinitionViewModel> MiniMaps => miniMaps;

    public ObservableCollection<MiniMapSlotViewModel> MiniMapSlots => SelectedMiniMap?.Slots ?? emptyMiniMapSlots;

    public ObservableCollection<string> MiniMapSuggestionOptions { get; } = new();

    public IAsyncRelayCommand LoadSheetCommand { get; }

    public IRelayCommand DetectAnimationsCommand { get; }

    public IAsyncRelayCommand SaveTileCommand { get; }

    public IRelayCommand AddFrameCommand { get; }

    public IRelayCommand ClearFramesCommand { get; }

    public IRelayCommand CommitAnimationCommand { get; }

    public IRelayCommand StartPreviewCommand { get; }

    public IRelayCommand StopPreviewCommand { get; }

    public IRelayCommand PlaceTileInMiniMapCommand { get; }

    public IRelayCommand UseSuggestedTileInMiniMapCommand { get; }

    public IRelayCommand ClearMiniMapSlotCommand { get; }

    public IRelayCommand ClearMiniMapCommand { get; }

    public IRelayCommand CreateMiniMapCommand { get; }

    public IRelayCommand RemoveMiniMapCommand { get; }

    public IAsyncRelayCommand ExportTileEnumCommand { get; }

    public IAsyncRelayCommand ImportTileEnumCommand { get; }

    public BitmapSource? SelectedTileImage => CreateTileImage(SelectedTile);

    public BitmapSource? PreviewTileImage => CreateTileImage(PreviewTile ?? SelectedAnimation?.FirstFrame);

    private bool HasTiles() => Tiles.Count > 0;

    private async Task LoadTileSheetAsync()
    {
        var file = fileDialogService.OpenTileSheet();
        if (string.IsNullOrWhiteSpace(file))
        {
            return;
        }

        currentTileSheetPath = file;
        var persistedState = await tileSetPersistence.LoadStateAsync(file).ConfigureAwait(true);
        TileSetClassKey = persistedState?.TileSetClassKey ?? ComputeDefaultClassKey(file);
        if (persistedState?.Grid.IsValid == true)
        {
            ApplyGridSettings(persistedState.Grid);
        }

        if (persistedState != null)
        {
            MinimumAnimationLength = persistedState.MinimumAnimationLength;
            FrameDuration = persistedState.FrameDuration;
            ZoomFactor = persistedState.ZoomFactor;
        }

        TileSheet = await tileSetService.LoadTileSheetAsync(file).ConfigureAwait(true);
        PendingFrames.Clear();
        RebuildTiles();

        var classMetadata = await tileSetPersistence.LoadClassAsync(TileSetClassKey).ConfigureAwait(true);
        currentClassMetadata = classMetadata;
        UpdateMiniMapSuggestionOptions();
        ApplyTileMetadataSnapshots(persistedState, classMetadata);
        RestoreAnimations(persistedState);
        RestoreMiniMaps(persistedState);
        UpdateMiniMapCommands();

        StatusMessage = string.Format(CultureInfo.CurrentCulture, Resources.TileCountFormat, Tiles.Count);
        DetectAnimationsCommand.NotifyCanExecuteChanged();
        await PersistStateAsync().ConfigureAwait(true);
    }

    private void RebuildTiles()
    {
        Tiles.Clear();
        if (TileSheet == null)
        {
            SelectedTile = null;
            StatusMessage = Resources.NoSheetLoadedMessage;
            UpdateCommandStates();
            return;
        }

        var settings = new TileGridSettings(TileWidth, TileHeight, TileSpacing, TileMargin);
        foreach (var tile in tileSetService.SliceTiles(TileSheet, settings))
        {
            Tiles.Add(tile);
        }

        SelectedTile = Tiles.FirstOrDefault();
        UpdateCommandStates();
        UpdateMiniMapSuggestionOptions();
        RebindMiniMapSlots();
        RefreshSubCategoryOptions(SelectedTileCategory);
    }

    private void DetectAnimations()
    {
        if (!HasTiles())
        {
            StatusMessage = Resources.NoSheetLoadedMessage;
            return;
        }

        var duration = TimeSpan.FromMilliseconds(Math.Max(30, FrameDuration));
        var animations = animationDetectionService.DetectAnimations(Tiles, MinimumAnimationLength, duration);
        Animations.Clear();
        foreach (var animation in animations)
        {
            Animations.Add(animation);
        }

        StatusMessage = animations.Count == 0
            ? Resources.SelectTileFirstMessage
            : string.Format(CultureInfo.CurrentCulture, Resources.AnimationCountFormat, animations.Count);
        UpdateCommandStates();
        RequestStatePersistence();
    }

    private async Task SaveTileAsync()
    {
        if (TileSheet == null || SelectedTile == null)
        {
            StatusMessage = Resources.SelectTileFirstMessage;
            return;
        }

        var path = fileDialogService.SaveTile(SelectedTile.Name.Replace(' ', '_'));
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        await tileSetService.SaveTileAsync(TileSheet, SelectedTile, path, ZoomFactor).ConfigureAwait(true);
        StatusMessage = path;
    }

    private void AddFrame()
    {
        if (SelectedTile == null)
        {
            return;
        }

        PendingFrames.Add(SelectedTile);
        UpdateCommandStates();
    }

    private void ClearFrames()
    {
        PendingFrames.Clear();
        UpdateCommandStates();
    }

    private void CommitAnimation()
    {
        if (PendingFrames.Count < 2)
        {
            return;
        }

        var duration = TimeSpan.FromMilliseconds(Math.Max(30, FrameDuration));
        var name = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.NewAnimationButtonText + " #{0}", Animations.Count + 1);
        Animations.Add(new TileAnimation(Guid.NewGuid(), name, PendingFrames.ToArray(), duration));
        PendingFrames.Clear();
        UpdateCommandStates();
        RequestStatePersistence();
    }

    private void StartPreview()
    {
        if (SelectedAnimation == null)
        {
            return;
        }

        animationPreviewService.Start(SelectedAnimation, tile => PreviewTile = tile);
    }

    private BitmapSource? CreateTileImage(TileDefinition? tile)
    {
        if (TileSheet == null || tile == null)
        {
            return null;
        }

        var cropped = new CroppedBitmap(TileSheet, tile.Bounds);
        cropped.Freeze();
        return cropped;
    }

    partial void OnSelectedTileChanged(TileDefinition? value)
    {
        SelectedTileName = value?.Name ?? string.Empty;
        SelectedTileNotes = value?.Notes ?? string.Empty;
        SelectedTileCategory = value?.Category ?? TileCategory.Unknown;
        RefreshSubCategoryOptions(SelectedTileCategory);
        SelectedTileSubCategory = value?.SubCategory ?? string.Empty;
        SaveTileCommand.NotifyCanExecuteChanged();
        AddFrameCommand.NotifyCanExecuteChanged();
        UpdateMiniMapCommands();
    }

    partial void OnSelectedAnimationChanged(TileAnimation? value)
    {
        PreviewTile = value?.FirstFrame;
        StartPreviewCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedTileNameChanged(string value) => ApplyTileMetadata(tile => tile with { Name = value });

    partial void OnSelectedTileNotesChanged(string value) => ApplyTileMetadata(tile => tile with { Notes = value });

    partial void OnSelectedTileCategoryChanged(TileCategory value)
    {
        RefreshSubCategoryOptions(value);
        if (SelectedTile == null)
        {
            return;
        }

        ApplyTileMetadata(tile => tile with { Category = value });
    }

    partial void OnSelectedTileSubCategoryChanged(string value)
    {
        if (SelectedTile == null)
        {
            return;
        }

        var normalized = value?.Trim() ?? string.Empty;
        ApplyTileMetadata(tile => tile with { SubCategory = normalized });
        RegisterSubCategory(normalized, SelectedTileCategory);
    }

    partial void OnTileSetClassKeyChanged(string value)
    {
        _ = LoadClassMetadataAsync(value);
        RequestStatePersistence();
    }

    partial void OnTileWidthChanged(int value)
    {
        RebuildTiles();
        RequestStatePersistence();
    }

    partial void OnTileHeightChanged(int value)
    {
        RebuildTiles();
        RequestStatePersistence();
    }

    partial void OnTileSpacingChanged(int value)
    {
        RebuildTiles();
        RequestStatePersistence();
    }

    partial void OnTileMarginChanged(int value)
    {
        RebuildTiles();
        RequestStatePersistence();
    }

    partial void OnMinimumAnimationLengthChanged(int value)
    {
        if (value < 2)
        {
            MinimumAnimationLength = 2;
        }

        RequestStatePersistence();
    }

    partial void OnZoomFactorChanged(double value)
    {
        if (value < 1)
        {
            ZoomFactor = 1;
        }

        RequestStatePersistence();
    }

    partial void OnFrameDurationChanged(int value)
    {
        if (value < 30)
        {
            FrameDuration = 30;
        }

        RequestStatePersistence();
    }

    partial void OnSelectedMiniMapSlotChanged(MiniMapSlotViewModel? value) => UpdateMiniMapCommands();

    partial void OnSelectedMiniMapChanged(MiniMapDefinitionViewModel? value)
    {
        SelectedMiniMapSlot = value?.Slots.FirstOrDefault();
        OnPropertyChanged(nameof(MiniMapSlots));
        UpdateMiniMapCommands();
        RemoveMiniMapCommand?.NotifyCanExecuteChanged();
        if (!suppressMiniMapNotifications)
        {
            RequestStatePersistence();
        }
    }

    private void ApplyTileMetadata(Func<TileDefinition, TileDefinition> updater)
    {
        if (SelectedTile == null)
        {
            return;
        }

        var index = Tiles.IndexOf(SelectedTile);
        if (index < 0)
        {
            return;
        }

        var updated = updater(SelectedTile);
        Tiles[index] = updated;
        SelectedTile = updated;
        UpdateClassMetadataSnapshot(updated);
        RequestStatePersistence();
    }

    private void UpdateCommandStates()
    {
        DetectAnimationsCommand.NotifyCanExecuteChanged();
        SaveTileCommand.NotifyCanExecuteChanged();
        AddFrameCommand.NotifyCanExecuteChanged();
        ClearFramesCommand.NotifyCanExecuteChanged();
        CommitAnimationCommand.NotifyCanExecuteChanged();
        StartPreviewCommand.NotifyCanExecuteChanged();
        ExportTileEnumCommand?.NotifyCanExecuteChanged();
    }

    private void OnPendingFramesChanged(object? sender, NotifyCollectionChangedEventArgs e) => UpdateCommandStates();

    private void InitializeMiniMaps()
    {
        ClearMiniMapDefinitions();
        var initialDefinition = CreateMiniMapDefinition();
        miniMaps.Add(initialDefinition);
    }

    private MiniMapDefinitionViewModel CreateMiniMapDefinition(string? name = null, Guid? id = null)
    {
        var slots = new ObservableCollection<MiniMapSlotViewModel>();
        for (var row = 0; row < 5; row++)
        {
            for (var column = 0; column < 5; column++)
            {
                var slot = new MiniMapSlotViewModel(row, column);
                slot.PropertyChanged += OnMiniMapSlotPropertyChanged;
                slots.Add(slot);
            }
        }

        var resolvedName = string.IsNullOrWhiteSpace(name) ? GenerateMiniMapName() : name;
        var definition = new MiniMapDefinitionViewModel(id ?? Guid.NewGuid(), resolvedName, slots);
        definition.PropertyChanged += OnMiniMapDefinitionPropertyChanged;
        return definition;
    }

    private void OnMiniMapSlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MiniMapSlotViewModel slot)
        {
            return;
        }

        var requiresPersistence = false;

        if (e.PropertyName == nameof(MiniMapSlotViewModel.SuggestionKey))
        {
            UpdateSlotSuggestion(slot);
            requiresPersistence = true;
        }
        else if (e.PropertyName == nameof(MiniMapSlotViewModel.Tile))
        {
            requiresPersistence = true;
        }

        if (!requiresPersistence && e.PropertyName != nameof(MiniMapSlotViewModel.SuggestedTile))
        {
            return;
        }

        if (requiresPersistence && !suppressMiniMapNotifications)
        {
            RequestStatePersistence();
        }

        UpdateMiniMapCommands();
    }

    private void OnMiniMapDefinitionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MiniMapDefinitionViewModel.Name))
        {
            RequestStatePersistence();
        }
    }

    private void UpdateSlotSuggestion(MiniMapSlotViewModel slot)
    {
        slot.SuggestedTile = FindTileByName(slot.SuggestionKey);
    }

    private void RebindMiniMapSlots()
    {
        suppressMiniMapNotifications = true;
        try
        {
            foreach (var miniMap in MiniMaps)
            {
                foreach (var slot in miniMap.Slots)
                {
                    if (slot.Tile != null)
                    {
                        slot.Tile = FindTileByName(slot.Tile.Name);
                    }

                    UpdateSlotSuggestion(slot);
                }
            }
        }
        finally
        {
            suppressMiniMapNotifications = false;
        }

        OnPropertyChanged(nameof(MiniMapSlots));
        UpdateMiniMapCommands();
    }

    private void UpdateMiniMapSuggestionOptions()
    {
        MiniMapSuggestionOptions.Clear();
        var names = currentClassMetadata?.Tiles
            .Select(t => t.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (names == null || names.Count == 0)
        {
            names = Tiles.Select(t => t.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();
        }

        var ordered = names
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase);

        foreach (var name in ordered)
        {
            MiniMapSuggestionOptions.Add(name);
        }
    }

    private void RefreshSubCategoryOptions(TileCategory category)
    {
        SubCategoryOptions.Clear();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (currentClassMetadata != null &&
            currentClassMetadata.CategoryDefinitions.TryGetValue(category, out var defined))
        {
            foreach (var entry in defined.OrderBy(entry => entry, StringComparer.CurrentCultureIgnoreCase))
            {
                if (seen.Add(entry))
                {
                    SubCategoryOptions.Add(entry);
                }
            }
        }

        foreach (var tile in Tiles.Where(t => t.Category == category))
        {
            if (string.IsNullOrWhiteSpace(tile.SubCategory) || !seen.Add(tile.SubCategory))
            {
                continue;
            }

            SubCategoryOptions.Add(tile.SubCategory);
        }
    }

    private void RegisterSubCategory(string? subCategory, TileCategory category)
    {
        if (string.IsNullOrWhiteSpace(subCategory) || string.IsNullOrWhiteSpace(TileSetClassKey))
        {
            return;
        }

        var metadata = EnsureClassMetadata();
        if (!metadata.CategoryDefinitions.TryGetValue(category, out var entries))
        {
            entries = new List<string>();
            metadata.CategoryDefinitions[category] = entries;
        }

        if (entries.Any(entry => string.Equals(entry, subCategory, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        entries.Add(subCategory);
        entries.Sort(StringComparer.CurrentCultureIgnoreCase);
        RefreshSubCategoryOptions(category);
        RequestClassMetadataPersistence();
    }

    private TileSetClassMetadata EnsureClassMetadata()
    {
        if (currentClassMetadata != null)
        {
            return currentClassMetadata;
        }

        var key = string.IsNullOrWhiteSpace(TileSetClassKey)
            ? ComputeDefaultClassKey(currentTileSheetPath ?? string.Empty)
            : TileSetClassKey;

        currentClassMetadata = new TileSetClassMetadata
        {
            Key = key,
            DisplayName = key
        };
        return currentClassMetadata;
    }

    private void UpdateClassMetadataSnapshot(TileDefinition tile)
    {
        if (string.IsNullOrWhiteSpace(TileSetClassKey))
        {
            return;
        }

        var metadata = EnsureClassMetadata();
        var snapshot = new TileMetadataSnapshot
        {
            Index = tile.Index,
            Name = tile.Name,
            Notes = string.IsNullOrWhiteSpace(tile.Notes) ? null : tile.Notes,
            Category = tile.Category,
            SubCategory = string.IsNullOrWhiteSpace(tile.SubCategory) ? null : tile.SubCategory
        };

        var existingIndex = metadata.Tiles.FindIndex(t => t.Index == tile.Index);
        if (existingIndex >= 0)
        {
            metadata.Tiles[existingIndex] = snapshot;
        }
        else
        {
            metadata.Tiles.Add(snapshot);
        }

        RequestClassMetadataPersistence();
    }

    private async Task PersistClassMetadataAsync()
    {
        if (currentClassMetadata == null || string.IsNullOrWhiteSpace(currentClassMetadata.Key))
        {
            return;
        }

        try
        {
            await tileSetPersistence.SaveClassAsync(currentClassMetadata).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to persist tile class '{currentClassMetadata.Key}': {ex}");
        }
    }

    private void RequestClassMetadataPersistence()
    {
        if (currentClassMetadata == null || string.IsNullOrWhiteSpace(TileSetClassKey))
        {
            return;
        }

        _ = PersistClassMetadataAsync();
    }

    private async Task LoadClassMetadataAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            currentClassMetadata = null;
            UpdateMiniMapSuggestionOptions();
            RebindMiniMapSlots();
            RefreshSubCategoryOptions(SelectedTileCategory);
            return;
        }

        try
        {
            currentClassMetadata = await tileSetPersistence.LoadClassAsync(key).ConfigureAwait(true)
                ?? new TileSetClassMetadata
                {
                    Key = key,
                    DisplayName = key
                };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load tile set class '{key}': {ex}");
            currentClassMetadata = new TileSetClassMetadata
            {
                Key = key,
                DisplayName = key
            };
        }

        currentClassMetadata.CategoryDefinitions ??= new Dictionary<TileCategory, List<string>>();

        UpdateMiniMapSuggestionOptions();
        RebindMiniMapSlots();
        RefreshSubCategoryOptions(SelectedTileCategory);
    }

    private async Task ExportTileEnumAsync()
    {
        if (Tiles.Count == 0)
        {
            StatusMessage = Resources.NoSheetLoadedMessage;
            return;
        }

        var suggestedName = string.IsNullOrWhiteSpace(TileSetClassKey) ? "Tiles" : TileSetClassKey;
        var targetPath = fileDialogService.SaveTileEnum(suggestedName);
        if (string.IsNullOrWhiteSpace(targetPath))
        {
            return;
        }

        var enumName = SanitizeIdentifier(currentClassMetadata?.DisplayName ?? "DisplayTile", "DisplayTile");
        var namespaceName = string.IsNullOrWhiteSpace(currentClassMetadata?.Key)
            ? null
            : $"{SanitizeIdentifier(currentClassMetadata.Key, "TileSet")}.ViewModel";

        try
        {
            await tileEnumSerializer.ExportAsync(Tiles, targetPath, enumName, namespaceName).ConfigureAwait(true);
            StatusMessage = targetPath;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task ImportTileEnumAsync()
    {
        var sourcePath = fileDialogService.OpenTileEnum();
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            return;
        }

        IReadOnlyList<TileMetadataSnapshot> definitions;
        try
        {
            definitions = await tileEnumSerializer.ImportAsync(sourcePath).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            return;
        }

        if (definitions.Count == 0)
        {
            StatusMessage = Resources.SelectTileFirstMessage;
            return;
        }

        var applied = 0;
        foreach (var snapshot in definitions)
        {
            var tile = Tiles.FirstOrDefault(t => t.Index == snapshot.Index);
            if (tile == null)
            {
                continue;
            }

            var updated = tile with
            {
                Name = snapshot.Name,
                Notes = snapshot.Notes ?? string.Empty
            };

            Tiles[Tiles.IndexOf(tile)] = updated;
            if (SelectedTile?.Index == updated.Index)
            {
                SelectedTile = updated;
            }

            UpdateClassMetadataSnapshot(updated);
            applied++;
        }

        if (applied > 0)
        {
            RequestStatePersistence();
            UpdateCommandStates();
            StatusMessage = string.Format(CultureInfo.CurrentCulture, Resources.TileCountFormat, Tiles.Count);
        }
    }

    private static string SanitizeIdentifier(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var builder = new StringBuilder();
        var capitalizeNext = true;
        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                if (builder.Length == 0 && char.IsDigit(ch))
                {
                    builder.Append('_');
                }

                builder.Append(capitalizeNext ? char.ToUpperInvariant(ch) : ch);
                capitalizeNext = false;
            }
            else if (ch is '_' or '-')
            {
                if (builder.Length == 0)
                {
                    builder.Append('_');
                }
                else if (builder[^1] != '_')
                {
                    builder.Append('_');
                }

                capitalizeNext = true;
            }
            else
            {
                capitalizeNext = true;
            }
        }

        return builder.Length == 0 ? fallback : builder.ToString();
    }

    private void RestoreMiniMaps(TileSetState? state)
    {
        suppressMiniMapNotifications = true;
        try
        {
            ClearMiniMapDefinitions();

            var snapshots = state?.MiniMaps;
            if (snapshots == null || snapshots.Count == 0)
            {
                var fallback = CreateMiniMapDefinition();
                ApplyLegacyMiniMap(state?.MiniMap, fallback);
                miniMaps.Add(fallback);
            }
            else
            {
                foreach (var definitionSnapshot in snapshots)
                {
                    var definition = CreateMiniMapDefinition(definitionSnapshot.Name, definitionSnapshot.Id);
                    foreach (var slotSnapshot in definitionSnapshot.Slots)
                    {
                        var slot = definition.Slots.FirstOrDefault(s => s.Row == slotSnapshot.Row && s.Column == slotSnapshot.Column);
                        if (slot == null)
                        {
                            continue;
                        }

                        slot.Tile = slotSnapshot.TileIndex.HasValue ? FindTileByIndex(slotSnapshot.TileIndex.Value) : null;
                        slot.SuggestionKey = slotSnapshot.SuggestionKey;
                        UpdateSlotSuggestion(slot);
                    }

                    miniMaps.Add(definition);
                }
            }

            var targetMiniMap = state?.SelectedMiniMapId != null
                ? MiniMaps.FirstOrDefault(m => m.Id == state.SelectedMiniMapId)
                : MiniMaps.FirstOrDefault();

            SelectedMiniMap = targetMiniMap ?? MiniMaps.FirstOrDefault();
            SelectedMiniMapSlot = SelectedMiniMap?.Slots.FirstOrDefault();
        }
        finally
        {
            suppressMiniMapNotifications = false;
        }

        UpdateMiniMapCommands();
    }

    private void ApplyLegacyMiniMap(IReadOnlyList<MiniMapSlotSnapshot>? slots, MiniMapDefinitionViewModel definition)
    {
        if (slots == null || slots.Count == 0)
        {
            return;
        }

        foreach (var slotSnapshot in slots)
        {
            var slot = definition.Slots.FirstOrDefault(s => s.Row == slotSnapshot.Row && s.Column == slotSnapshot.Column);
            if (slot == null)
            {
                continue;
            }

            slot.Tile = slotSnapshot.TileIndex.HasValue ? FindTileByIndex(slotSnapshot.TileIndex.Value) : null;
            slot.SuggestionKey = slotSnapshot.SuggestionKey;
            UpdateSlotSuggestion(slot);
        }
    }

    private void ClearMiniMapDefinitions()
    {
        foreach (var definition in miniMaps)
        {
            definition.PropertyChanged -= OnMiniMapDefinitionPropertyChanged;
            foreach (var slot in definition.Slots)
            {
                slot.PropertyChanged -= OnMiniMapSlotPropertyChanged;
            }
        }

        miniMaps.Clear();
        SelectedMiniMap = null;
        SelectedMiniMapSlot = null;
    }

    private string GenerateMiniMapName()
    {
        var index = 1;
        var existing = new HashSet<string>(MiniMaps.Select(map => map.Name), StringComparer.CurrentCultureIgnoreCase);
        string candidate;
        do
        {
            candidate = string.Format(CultureInfo.CurrentCulture, DefaultMiniMapNameFormat, index++);
        }
        while (existing.Contains(candidate));

        return candidate;
    }

    private void AddMiniMap()
    {
        var definition = CreateMiniMapDefinition();
        miniMaps.Add(definition);
        SelectedMiniMap = definition;
        SelectedMiniMapSlot = definition.Slots.FirstOrDefault();
        RequestStatePersistence();
    }

    private bool CanRemoveSelectedMiniMap() => SelectedMiniMap != null && MiniMaps.Count > 1;

    private void RemoveSelectedMiniMap()
    {
        if (!CanRemoveSelectedMiniMap() || SelectedMiniMap == null)
        {
            return;
        }

        var index = miniMaps.IndexOf(SelectedMiniMap);
        miniMaps.Remove(SelectedMiniMap);
        SelectedMiniMap = miniMaps.Count == 0
            ? null
            : miniMaps[Math.Clamp(index - 1, 0, miniMaps.Count - 1)];
        SelectedMiniMapSlot = SelectedMiniMap?.Slots.FirstOrDefault();
        RequestStatePersistence();
    }

    private void PlaceSelectedTileInMiniMap()
    {
        if (SelectedTile == null || SelectedMiniMapSlot == null)
        {
            return;
        }

        SelectedMiniMapSlot.Tile = SelectedTile;
        RequestStatePersistence();
        UpdateMiniMapCommands();
    }

    private bool CanPlaceSelectedTileInMiniMap() => SelectedTile != null && SelectedMiniMapSlot != null;

    private void UseSuggestedTileInMiniMap()
    {
        if (SelectedMiniMapSlot?.SuggestedTile == null)
        {
            return;
        }

        SelectedMiniMapSlot.Tile = SelectedMiniMapSlot.SuggestedTile;
        RequestStatePersistence();
        UpdateMiniMapCommands();
    }

    private bool CanUseSuggestedTileInMiniMap() => SelectedMiniMapSlot?.SuggestedTile != null;

    private void ClearSelectedMiniMapSlot()
    {
        if (SelectedMiniMapSlot == null)
        {
            return;
        }

        SelectedMiniMapSlot.Tile = null;
        RequestStatePersistence();
        UpdateMiniMapCommands();
    }

    private bool CanClearSelectedMiniMapSlot() => SelectedMiniMapSlot?.Tile != null;

    private void ClearMiniMap()
    {
        if (SelectedMiniMap == null)
        {
            return;
        }

        foreach (var slot in SelectedMiniMap.Slots)
        {
            slot.Tile = null;
        }

        RequestStatePersistence();
        UpdateMiniMapCommands();
    }

    private bool HasMiniMapTiles() => SelectedMiniMap?.Slots.Any(slot => slot.Tile != null) == true;

    private void OnMiniMapsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (MiniMapDefinitionViewModel definition in e.OldItems)
            {
                definition.PropertyChanged -= OnMiniMapDefinitionPropertyChanged;
                foreach (var slot in definition.Slots)
                {
                    slot.PropertyChanged -= OnMiniMapSlotPropertyChanged;
                }
            }
        }

        if (MiniMaps.Count == 0 && !suppressMiniMapNotifications)
        {
            var fallback = CreateMiniMapDefinition();
            miniMaps.Add(fallback);
            SelectedMiniMap = fallback;
        }

        RemoveMiniMapCommand?.NotifyCanExecuteChanged();
        UpdateMiniMapCommands();

        if (!suppressMiniMapNotifications)
        {
            RequestStatePersistence();
        }
    }

    private void UpdateMiniMapCommands()
    {
        PlaceTileInMiniMapCommand?.NotifyCanExecuteChanged();
        UseSuggestedTileInMiniMapCommand?.NotifyCanExecuteChanged();
        ClearMiniMapSlotCommand?.NotifyCanExecuteChanged();
        ClearMiniMapCommand?.NotifyCanExecuteChanged();
        RemoveMiniMapCommand?.NotifyCanExecuteChanged();
    }

    private TileDefinition? FindTileByName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return Tiles.FirstOrDefault(tile => string.Equals(tile.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private TileDefinition? FindTileByIndex(int index) => Tiles.FirstOrDefault(tile => tile.Index == index);

    private void ApplyTileMetadataSnapshots(TileSetState? state, TileSetClassMetadata? classMetadata)
    {
        if ((state?.Tiles.Count ?? 0) == 0 && (classMetadata?.Tiles.Count ?? 0) == 0)
        {
            return;
        }

        var lookup = new Dictionary<int, TileMetadataSnapshot>();
        if (classMetadata?.Tiles != null)
        {
            foreach (var snapshot in classMetadata.Tiles)
            {
                lookup[snapshot.Index] = snapshot;
            }
        }

        if (state?.Tiles != null)
        {
            foreach (var snapshot in state.Tiles)
            {
                lookup[snapshot.Index] = snapshot;
            }
        }

        for (var i = 0; i < Tiles.Count; i++)
        {
            var tile = Tiles[i];
            if (!lookup.TryGetValue(tile.Index, out var snapshot))
            {
                continue;
            }

            var updated = tile with
            {
                Name = snapshot.Name,
                Notes = snapshot.Notes ?? string.Empty,
                Category = snapshot.Category,
                SubCategory = snapshot.SubCategory ?? string.Empty
            };

            Tiles[i] = updated;
            if (SelectedTile?.Index == updated.Index)
            {
                SelectedTile = updated;
            }
        }
    }

    private void RestoreAnimations(TileSetState? state)
    {
        Animations.Clear();
        if (state?.Animations == null || state.Animations.Count == 0)
        {
            return;
        }

        foreach (var snapshot in state.Animations)
        {
            var frames = snapshot.FrameIndices
                .Select(FindTileByIndex)
                .Where(tile => tile != null)
                .Cast<TileDefinition>()
                .ToArray();

            if (frames.Length == 0)
            {
                continue;
            }

            var duration = TimeSpan.FromMilliseconds(Math.Max(30, snapshot.FrameDuration));
            var animation = new TileAnimation(
                snapshot.Id == Guid.Empty ? Guid.NewGuid() : snapshot.Id,
                string.IsNullOrWhiteSpace(snapshot.Name) ? Resources.NewAnimationButtonText : snapshot.Name,
                frames,
                duration);

            Animations.Add(animation);
        }
    }

    private void ApplyGridSettings(TileGridSettings grid)
    {
        if (!grid.IsValid)
        {
            return;
        }

        TileWidth = grid.TileWidth;
        TileHeight = grid.TileHeight;
        TileSpacing = grid.Spacing;
        TileMargin = grid.Margin;
    }

    private static string ComputeDefaultClassKey(string tileSheetPath)
    {
        if (string.IsNullOrWhiteSpace(tileSheetPath))
        {
            return "TileSet";
        }

        var fileName = Path.GetFileNameWithoutExtension(tileSheetPath);
        return SanitizeIdentifier(fileName, "TileSet");
    }

    private void RequestStatePersistence()
    {
        if (string.IsNullOrWhiteSpace(currentTileSheetPath))
        {
            return;
        }

        statePersistenceCancellation?.Cancel();
        statePersistenceCancellation?.Dispose();
        statePersistenceCancellation = new CancellationTokenSource();
        var token = statePersistenceCancellation.Token;

        _ = PersistStateAsync(token);
    }

    private async Task PersistStateAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currentTileSheetPath))
        {
            return;
        }

        var state = new TileSetState
        {
            TileSheetPath = currentTileSheetPath,
            TileSetClassKey = string.IsNullOrWhiteSpace(TileSetClassKey) ? null : TileSetClassKey,
            Grid = new TileGridSettings(TileWidth, TileHeight, TileSpacing, TileMargin),
            MinimumAnimationLength = MinimumAnimationLength,
            FrameDuration = FrameDuration,
            ZoomFactor = ZoomFactor,
            Tiles = Tiles.Select(tile => new TileMetadataSnapshot
            {
                Index = tile.Index,
                Name = tile.Name,
                Notes = string.IsNullOrWhiteSpace(tile.Notes) ? null : tile.Notes,
                Category = tile.Category,
                SubCategory = string.IsNullOrWhiteSpace(tile.SubCategory) ? null : tile.SubCategory
            }).ToList(),
            Animations = Animations.Select(animation => new TileAnimationSnapshot
            {
                Id = animation.Id,
                Name = animation.Name,
                FrameDuration = (int)Math.Max(30, animation.FrameDuration.TotalMilliseconds),
                FrameIndices = animation.Frames.Select(frame => frame.Index).ToList()
            }).ToList(),
            MiniMaps = MiniMaps.Select(map => new MiniMapDefinitionSnapshot
            {
                Id = map.Id,
                Name = map.Name,
                Slots = map.Slots.Select(slot => new MiniMapSlotSnapshot
                {
                    Row = slot.Row,
                    Column = slot.Column,
                    TileIndex = slot.Tile?.Index,
                    SuggestionKey = slot.SuggestionKey
                }).ToList()
            }).ToList(),
            SelectedMiniMapId = SelectedMiniMap?.Id
        };

        try
        {
            await tileSetPersistence.SaveStateAsync(state, cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Swallow cancellations triggered by coalescing requests.
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to persist tile set '{currentTileSheetPath}': {ex}");
        }
    }
}
