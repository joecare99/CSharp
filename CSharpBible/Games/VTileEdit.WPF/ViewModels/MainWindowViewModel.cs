using CommonDialogs;
using CommonDialogs.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VTileEdit.Models;
using CoreViewModels = VTileEdit;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Central view-model driving the WPF editor shell.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private const int DefaultTileWidth = 8;
    private const int DefaultTileHeight = 8;
    private static readonly string[] PreferredConsoleFonts = new[] { "Consolas", "Cascadia Mono", "Cascadia Code", "Lucida Console", "Courier New" };

    private readonly CoreViewModels.TileDocument _document;
    private readonly Dictionary<TileViewModel, CoreViewModels.TileDefinition> _tileLookup = new();
    private readonly VTEModel _persistenceModel = new();

    static MainWindowViewModel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        _document = CoreViewModels.TileDocument.CreateSample(DefaultTileWidth, DefaultTileHeight);

        Tiles = new ObservableCollection<TileViewModel>();
        Palette = new ObservableCollection<ColorSwatchViewModel>(Enum.GetValues<ConsoleColor>().Select(color => new ColorSwatchViewModel(color)));
        CharacterPalette = new ObservableCollection<char>(_document.CharacterPalette);
        AvailableFontFamilies = new ObservableCollection<FontFamily>(GetConsoleFontFamilies());
        SelectedFontFamily = AvailableFontFamilies.FirstOrDefault() ?? SystemFonts.MessageFontFamily;
        TilesView = CollectionViewSource.GetDefaultView(Tiles);
        TilesView.Filter = FilterTile;

        SyncTilesFromDocument();

        TileSetName = _document.Name;
        TileSetTileWidth = _document.TileWidth;
        TileSetTileHeight = _document.TileHeight;
        TileSetNameInput = TileSetName;
        TileSetTileWidthInput = TileSetTileWidth;
        TileSetTileHeightInput = TileSetTileHeight;
        EvaluateTileSetInputs();

        ApplyForeground(Palette.FirstOrDefault());
        ApplyBackground(Palette.FirstOrDefault());

        UndoCommand = new RelayCommand(() => ShowPlaceholderMessage("Undo"));
        RedoCommand = new RelayCommand(() => ShowPlaceholderMessage("Redo"));
        SelectGlyphCommand = new RelayCommand<GlyphCellViewModel>(SelectGlyph, glyph => glyph != null);
        ApplyForegroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyForeground, swatch => swatch != null);
        ApplyBackgroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyBackground, swatch => swatch != null);
        ApplyCharacterCommand = new RelayCommand<char>(ApplyCharacter);
        ApplyTileSetChangesCommand = new RelayCommand(ApplyTileSetChanges, () => TileSetHasPendingChanges);
        DiscardTileSetChangesCommand = new RelayCommand(DiscardTileSetChanges, () => TileSetHasPendingChanges);
    }

    /// <summary>
    /// Gets the list of console-friendly font families available to the application.
    /// </summary>
    public ObservableCollection<FontFamily> AvailableFontFamilies { get; }

    [ObservableProperty]
    private FontFamily selectedFontFamily = SystemFonts.MessageFontFamily;

    [ObservableProperty]
    private string tileFilterText = string.Empty;

    [ObservableProperty]
    private string tileSetName = "Tile Set";

    [ObservableProperty]
    private int tileSetTileWidth = DefaultTileWidth;

    [ObservableProperty]
    private int tileSetTileHeight = DefaultTileHeight;

    [ObservableProperty]
    private string tileSetNameInput = "Tile Set";

    [ObservableProperty]
    private int tileSetTileWidthInput = DefaultTileWidth;

    [ObservableProperty]
    private int tileSetTileHeightInput = DefaultTileHeight;

    [ObservableProperty]
    private bool tileSetHasPendingChanges;

    [ObservableProperty]
    private bool tileSetShowsShrinkWarning;

    /// <summary>
    /// Represents a delegate that displays a file dialog and returns a value indicating whether the user confirmed the
    /// dialog.
    /// </summary>
    /// <remarks>Assign a method to this delegate to customize how file dialogs are shown within the
    /// application. The delegate should return <see langword="true"/> if the user confirms the dialog (such as by
    /// clicking OK), or <see langword="false"/> if the dialog is canceled.</remarks>
    public Func<IFileDialog, bool>? ShowFileDlg { get; set; }

    /// <summary>
    /// Gets the tiles displayed in the list.
    /// </summary>
    public ObservableCollection<TileViewModel> Tiles { get; }

    /// <summary>
    /// Gets a filtered view of <see cref="Tiles"/> honoring <see cref="TileFilterText"/>.
    /// </summary>
    public ICollectionView TilesView { get; }

    /// <summary>
    /// Gets the palette used for both foreground and background colors.
    /// </summary>
    public ObservableCollection<ColorSwatchViewModel> Palette { get; }

    /// <summary>
    /// Gets the character palette for the charmap view.
    /// </summary>
    public ObservableCollection<char> CharacterPalette { get; }

    [ObservableProperty]
    private TileViewModel? selectedTile;

    [ObservableProperty]
    private GlyphCellViewModel? selectedGlyph;

    [ObservableProperty]
    private ColorSwatchViewModel? selectedForegroundSwatch;

    [ObservableProperty]
    private ColorSwatchViewModel? selectedBackgroundSwatch;
    private string? _fileName;

    private ColorSwatchViewModel? _activeForegroundSwatch;
    private ColorSwatchViewModel? _activeBackgroundSwatch;
    private bool _isUpdatingForegroundSwatch;
    private bool _isUpdatingBackgroundSwatch;

    /// <summary>
    /// Gets the undo command placeholder.
    /// </summary>
    public IRelayCommand UndoCommand { get; }

    /// <summary>
    /// Gets the redo command placeholder.
    /// </summary>
    public IRelayCommand RedoCommand { get; }

    /// <summary>
    /// Gets the command selecting a glyph from the preview grid.
    /// </summary>
    public IRelayCommand SelectGlyphCommand { get; }

    /// <summary>
    /// Gets the command applying a foreground swatch.
    /// </summary>
    public IRelayCommand ApplyForegroundCommand { get; }

    /// <summary>
    /// Gets the command applying a background swatch.
    /// </summary>
    public IRelayCommand ApplyBackgroundCommand { get; }

    /// <summary>
    /// Gets the command applying a character from the charmap.
    /// </summary>
    public IRelayCommand ApplyCharacterCommand { get; }

    /// <summary>
    /// Gets the command applying tile-set metadata updates.
    /// </summary>
    public IRelayCommand ApplyTileSetChangesCommand { get; }

    /// <summary>
    /// Gets the command discarding tile-set metadata changes.
    /// </summary>
    public IRelayCommand DiscardTileSetChangesCommand { get; }

    [RelayCommand]
    private void New()
    {
        CreateNewTile();
    }

    [RelayCommand]
    private void Open()
    {
        var openDlg = new OpenFileDialogProxy()
        {
            Title = "Open Tile File",
            Filter = "Tile Files (*.tdf;*.tdt;*.tdj;*.tdx)|*.tdf;*.tdt;*.tdj;*.tdx|All Files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false,
        };
        if (ShowFileDlg?.Invoke(openDlg) == true)
        {
            _fileName = openDlg.FileName;
            LoadFromFile(_fileName);
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (!string.IsNullOrEmpty(_fileName) && File.Exists(_fileName))
        {
            // Save to existing file
            SaveToFile(_fileName);
            return;
        }
        SaveAs();
    }

    [RelayCommand]
    private void SaveAs()
    {
        var saveDlg = new SaveFileDialogProxy()
        {
            Title = "Save Tile File",
            Filter = "Tile Files (*.tdf;*.tdt;*.tdj;*.tdx;*.cs)|*.tdf;*.tdt;*.tdj;*.tdx;*.cs|All Files (*.*)|*.*",
            CheckFileExists = false,
        };
        if (ShowFileDlg?.Invoke(saveDlg) == true)
        {
            _fileName = saveDlg.FileName;
            SaveToFile(_fileName);
        }
    }


    [RelayCommand]
    private void Exit()
    {
        Application.Current?.Shutdown();
    }


    partial void OnSelectedTileChanged(TileViewModel? value)
    {
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetSelection(false);
        }

        SelectGlyph(value?.Glyphs.FirstOrDefault());
    }

    private void CreateNewTile()
    {
        var documentTile = _document.CreateTile($"Tile {Tiles.Count + 1}");
        var tile = CreateTileViewModel(documentTile);
        Tiles.Add(tile);
        _tileLookup[tile] = documentTile;
        SelectedTile = tile;
    }

    private static TileViewModel CreateTileViewModel(CoreViewModels.TileDefinition definition)
    {
        var glyphs = definition.Glyphs
            .Select(g => new GlyphCellViewModel(g.Row, g.Column, g.Character, g.Foreground, g.Background));
        return new TileViewModel(definition.DisplayName, definition.Width, definition.Height, glyphs);
    }

    private void SyncTilesFromDocument()
    {
        Tiles.Clear();
        _tileLookup.Clear();

        foreach (var definition in _document.Tiles)
        {
            var tile = CreateTileViewModel(definition);
            Tiles.Add(tile);
            _tileLookup[tile] = definition;
        }

        SelectedTile = Tiles.FirstOrDefault();
        if (SelectedTile != null)
        {
            SelectGlyph(SelectedTile.Glyphs.FirstOrDefault());
        }
    }

    private bool TryGetSelectedDefinition(out CoreViewModels.TileDefinition definition)
    {
        definition = null!;
        if (SelectedTile != null && _tileLookup.TryGetValue(SelectedTile, out var match))
        {
            definition = match;
            return true;
        }

        return false;
    }

    private void SelectGlyph(GlyphCellViewModel? glyph)
    {
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetSelection(false);
        }

        SelectedGlyph = glyph;
        SelectedGlyph?.SetSelection(true);

        if (glyph != null)
        {
            SyncSwatchesWithGlyph(glyph);
        }
    }

    private void ApplyForeground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        SetForegroundSelection(swatch, updateBinding: true);
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetForeground(swatch.Color);
            UpdateDocumentForeground(SelectedGlyph);
        }
    }

    private void ApplyBackground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        SetBackgroundSelection(swatch, updateBinding: true);
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetBackground(swatch.Color);
            UpdateDocumentBackground(SelectedGlyph);
        }
    }

    private void ApplyCharacter(char character)
    {
        if (SelectedGlyph == null)
        {
            return;
        }
 
        SelectedGlyph.Character = character;
        UpdateDocumentCharacter(SelectedGlyph);
    }
 
    private void ApplyTileSetChanges()
    {
        var normalizedWidth = NormalizeTileDimension(TileSetTileWidthInput);
        var normalizedHeight = NormalizeTileDimension(TileSetTileHeightInput);
        var normalizedName = string.IsNullOrWhiteSpace(TileSetNameInput) ? "Tile Set" : TileSetNameInput.Trim();
 
        TileSetNameInput = normalizedName;
        TileSetTileWidthInput = normalizedWidth;
        TileSetTileHeightInput = normalizedHeight;

        _document.Rename(normalizedName);
        _document.ApplyDimensions(normalizedWidth, normalizedHeight);

        TileSetName = _document.Name;
        TileSetTileWidth = _document.TileWidth;
        TileSetTileHeight = _document.TileHeight;

        SyncTilesFromDocument();
 
        EvaluateTileSetInputs();
    }
 
    private static int NormalizeTileDimension(int value)
        => value <= 0 ? 1 : value;
 
    private void DiscardTileSetChanges()
    {
        RestoreTileSetInputs();
    }
 
    private static void ShowPlaceholderMessage(string context)
        => Debug.WriteLine($"Command '{context}' is not yet implemented.");

    private void SyncSwatchesWithGlyph(GlyphCellViewModel glyph)
    {
        var foregroundSwatch = Palette.FirstOrDefault(p => p.Color == glyph.Foreground);
        if (foregroundSwatch != null)
        {
            SetForegroundSelection(foregroundSwatch, updateBinding: true);
        }

        var backgroundSwatch = Palette.FirstOrDefault(p => p.Color == glyph.Background);
        if (backgroundSwatch != null)
        {
            SetBackgroundSelection(backgroundSwatch, updateBinding: true);
        }
    }

    private void SetForegroundSelection(ColorSwatchViewModel swatch, bool updateBinding)
    {
        if (updateBinding)
        {
            try
            {
                _isUpdatingForegroundSwatch = true;
                SelectedForegroundSwatch = swatch;
            }
            finally
            {
                _isUpdatingForegroundSwatch = false;
            }
        }

        if (_activeForegroundSwatch != null && _activeForegroundSwatch != swatch)
        {
            _activeForegroundSwatch.IsForegroundSelection = false;
        }

        swatch.IsForegroundSelection = true;
        _activeForegroundSwatch = swatch;
    }

    private void SetBackgroundSelection(ColorSwatchViewModel swatch, bool updateBinding)
    {
        if (updateBinding)
        {
            try
            {
                _isUpdatingBackgroundSwatch = true;
                SelectedBackgroundSwatch = swatch;
            }
            finally
            {
                _isUpdatingBackgroundSwatch = false;
            }
        }

        if (_activeBackgroundSwatch != null && _activeBackgroundSwatch != swatch)
        {
            _activeBackgroundSwatch.IsBackgroundSelection = false;
        }

        swatch.IsBackgroundSelection = true;
        _activeBackgroundSwatch = swatch;
    }

    private void UpdateDocumentForeground(GlyphCellViewModel glyph)
    {
        if (TryGetSelectedDefinition(out var definition))
        {
            _document.SetForeground(definition, glyph.Row, glyph.Column, glyph.Foreground);
        }
    }

    private void UpdateDocumentBackground(GlyphCellViewModel glyph)
    {
        if (TryGetSelectedDefinition(out var definition))
        {
            _document.SetBackground(definition, glyph.Row, glyph.Column, glyph.Background);
        }
    }

    private void UpdateDocumentCharacter(GlyphCellViewModel glyph)
    {
        if (TryGetSelectedDefinition(out var definition))
        {
            _document.SetGlyphCharacter(definition, glyph.Row, glyph.Column, glyph.Character);
        }
    }

    partial void OnSelectedForegroundSwatchChanged(ColorSwatchViewModel? value)
    {
        if (value == null || _isUpdatingForegroundSwatch)
        {
            return;
        }

        SetForegroundSelection(value, updateBinding: false);
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetForeground(value.Color);
            UpdateDocumentForeground(SelectedGlyph);
        }
    }

    partial void OnSelectedBackgroundSwatchChanged(ColorSwatchViewModel? value)
    {
        if (value == null || _isUpdatingBackgroundSwatch)
        {
            return;
        }
 
        SetBackgroundSelection(value, updateBinding: false);
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetBackground(value.Color);
            UpdateDocumentBackground(SelectedGlyph);
        }
    }

    partial void OnTileSetNameInputChanged(string value)
        => EvaluateTileSetInputs();

    partial void OnTileSetTileWidthInputChanged(int value)
        => EvaluateTileSetInputs();

    partial void OnTileSetTileHeightInputChanged(int value)
        => EvaluateTileSetInputs();

    partial void OnTileSetHasPendingChangesChanged(bool value)
    {
        (ApplyTileSetChangesCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DiscardTileSetChangesCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    private static IEnumerable<FontFamily> GetConsoleFontFamilies()
    {
        var installed = Fonts.SystemFontFamilies.ToList();
        var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var preferred in PreferredConsoleFonts)
        {
            var match = installed.FirstOrDefault(f => string.Equals(f.Source, preferred, StringComparison.OrdinalIgnoreCase) ||
                                                      f.FamilyNames.Values.Any(name => string.Equals(name, preferred, StringComparison.OrdinalIgnoreCase)));
            if (match != null && added.Add(match.Source))
            {
                yield return match;
            }
        }

        foreach (var candidate in installed.Where(IsConsoleCandidate))
        {
            if (added.Add(candidate.Source))
            {
                yield return candidate;
            }
        }

        if (added.Add(SystemFonts.MessageFontFamily.Source))
        {
            yield return SystemFonts.MessageFontFamily;
        }
    }

    private static bool IsConsoleCandidate(FontFamily family)
    {
        var source = family.Source;
        return source.Contains("Mono", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Console", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Courier", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Code", StringComparison.OrdinalIgnoreCase);
    }

    private bool FilterTile(object obj)
    {
        if (string.IsNullOrWhiteSpace(TileFilterText))
        {
            return true;
        }
 
        if (obj is not TileViewModel tile)
        {
            return false;
        }
 
        return tile.DisplayName.Contains(TileFilterText, StringComparison.OrdinalIgnoreCase);
    }

    partial void OnTileFilterTextChanged(string value)
    {
        TilesView.Refresh();
    }

    private void EvaluateTileSetInputs()
    {
        var currentName = (_document.Name ?? string.Empty).Trim();
        var pendingName = (TileSetNameInput ?? string.Empty).Trim();
        TileSetShowsShrinkWarning = TileSetTileWidthInput < TileSetTileWidth
            || TileSetTileHeightInput < TileSetTileHeight;
        var hasChanges = !string.Equals(currentName, pendingName, StringComparison.Ordinal)
            || TileSetTileWidthInput != TileSetTileWidth
            || TileSetTileHeightInput != TileSetTileHeight;
        TileSetHasPendingChanges = hasChanges;
    }
 
    private void RestoreTileSetInputs()
    {
        TileSetNameInput = _document.Name;
        TileSetTileWidthInput = _document.TileWidth;
        TileSetTileHeightInput = _document.TileHeight;
        EvaluateTileSetInputs();
    }

    private static EStreamType ResolveStreamType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jdt" => EStreamType.Text,
            ".txt" => EStreamType.Text,
            ".bin" => EStreamType.Binary,
            ".tdb" => EStreamType.Binary,
            ".tdf" => EStreamType.Binary,
            ".tdj" => EStreamType.Json,
            ".tdx" => EStreamType.Xml,
            ".cs" => EStreamType.Code,
            _ => throw new NotSupportedException($"Unsupported file extension '{ext}'")
        };
    }

    private void SaveToFile(string path)
    {
        var streamType = ResolveStreamType(path);

        _persistenceModel.Clear();
        _persistenceModel.SetTileSize(new System.Drawing.Size(TileSetTileWidth, TileSetTileHeight));

        var index = 0;
        foreach (var tile in Tiles)
        {
            var (lines, colors) = ExtractTile(tile);
            _persistenceModel.SetTileDef(index++, lines, colors);
        }

        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        _persistenceModel.SaveToStream(fs, streamType);
    }

    private void LoadFromFile(string path)
    {
        var streamType = ResolveStreamType(path);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        _persistenceModel.Clear();
        _persistenceModel.LoadFromStream(fs, streamType);

        var size = _persistenceModel.TileSize;
        TileSetTileWidth = size.Width;
        TileSetTileHeight = size.Height;
        TileSetName = Path.GetFileNameWithoutExtension(path);
        TileSetNameInput = TileSetName;
        TileSetTileWidthInput = TileSetTileWidth;
        TileSetTileHeightInput = TileSetTileHeight;

        Tiles.Clear();
        foreach (var key in _persistenceModel.TileKeys)
        {
            var def = _persistenceModel.GetTileDef(key);
            var tileVm = BuildTileFrom(def.lines, def.colors, TileSetTileWidth, TileSetTileHeight, key.ToString());
            Tiles.Add(tileVm);
        }

        SyncDocumentFromTiles();

        EvaluateTileSetInputs();
    }

    private (string[] lines, FullColor[] colors) ExtractTile(TileViewModel tile)
    {
        var lines = new string[tile.TileHeight];
        var colors = new FullColor[tile.TileWidth * tile.TileHeight];

        for (var row = 0; row < tile.TileHeight; row++)
        {
            var sb = new StringBuilder(tile.TileWidth);
            for (var col = 0; col < tile.TileWidth; col++)
            {
                var glyph = tile.Glyphs[row * tile.TileWidth + col];
                sb.Append(glyph.Character);
                colors[row * tile.TileWidth + col] = new FullColor(glyph.Foreground, glyph.Background);
            }
            lines[row] = sb.ToString();
        }

        return (lines, colors);
    }

    private static TileViewModel BuildTileFrom(string[] lines, FullColor[] colors, int width, int height, string name)
    {
        var glyphs = new List<GlyphCellViewModel>(width * height);
        for (var row = 0; row < height; row++)
        {
            for (var col = 0; col < width; col++)
            {
                var ch = row < lines.Length && col < lines[row].Length ? lines[row][col] : ' ';
                var color = colors.Length > row * width + col ? colors[row * width + col] : new FullColor(ConsoleColor.Gray, ConsoleColor.Black);
                glyphs.Add(new GlyphCellViewModel(row, col, ch, color.fgr, color.bgr));
            }
        }

        return new TileViewModel(name, width, height, glyphs);
    }

    private void SyncDocumentFromTiles()
    {
        _document.Rename(TileSetName);
        _document.ApplyDimensions(TileSetTileWidth, TileSetTileHeight);
        _document.Tiles.Clear();
        _tileLookup.Clear();

        foreach (var tile in Tiles)
        {
            var glyphs = tile.Glyphs.Select(g => new VTileEdit.GlyphData(g.Row, g.Column, g.Character, g.Foreground, g.Background)).ToList();
            var def = new VTileEdit.TileDefinition(tile.DisplayName, tile.TileWidth, tile.TileHeight, glyphs);
            _document.Tiles.Add(def);
            _tileLookup[tile] = def;
        }
    }
 
    private enum TileKey { }
 }
