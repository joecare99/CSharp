using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace VTileEdit.WPF.ViewModels;

/// <summary>
/// Central view-model driving the WPF editor shell.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private const int DefaultTileWidth = 8;
    private const int DefaultTileHeight = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        Tiles = new ObservableCollection<TileViewModel>(TileViewModel.CreateSampleTiles());
        Palette = new ObservableCollection<ColorSwatchViewModel>(Enum.GetValues<ConsoleColor>().Select(color => new ColorSwatchViewModel(color)));

        SelectedTile = Tiles.FirstOrDefault();
        if (SelectedTile != null)
        {
            SelectGlyph(SelectedTile.Glyphs.FirstOrDefault());
        }

        ApplyForeground(Palette.FirstOrDefault());
        ApplyBackground(Palette.FirstOrDefault());

        NewCommand = new RelayCommand(CreateNewTile);
        OpenCommand = new RelayCommand(() => ShowPlaceholderMessage("Open"));
        SaveCommand = new RelayCommand(() => ShowPlaceholderMessage("Save"));
        SaveAsCommand = new RelayCommand(() => ShowPlaceholderMessage("SaveAs"));
        ExitCommand = new RelayCommand(() => Application.Current?.Shutdown());
        UndoCommand = new RelayCommand(() => ShowPlaceholderMessage("Undo"));
        RedoCommand = new RelayCommand(() => ShowPlaceholderMessage("Redo"));
        SelectGlyphCommand = new RelayCommand<GlyphCellViewModel>(SelectGlyph, glyph => glyph != null);
        ApplyForegroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyForeground, swatch => swatch != null);
        ApplyBackgroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyBackground, swatch => swatch != null);
    }

    /// <summary>
    /// Gets the tiles displayed in the list.
    /// </summary>
    public ObservableCollection<TileViewModel> Tiles { get; }

    /// <summary>
    /// Gets the palette used for both foreground and background colors.
    /// </summary>
    public ObservableCollection<ColorSwatchViewModel> Palette { get; }

    [ObservableProperty]
    private TileViewModel? selectedTile;

    [ObservableProperty]
    private GlyphCellViewModel? selectedGlyph;

    [ObservableProperty]
    private ColorSwatchViewModel? selectedForegroundSwatch;

    [ObservableProperty]
    private ColorSwatchViewModel? selectedBackgroundSwatch;

    /// <summary>
    /// Gets the command adding a new tile to the list.
    /// </summary>
    public IRelayCommand NewCommand { get; }

    /// <summary>
    /// Gets the command opening an existing tile set.
    /// </summary>
    public IRelayCommand OpenCommand { get; }

    /// <summary>
    /// Gets the command saving the tile set.
    /// </summary>
    public IRelayCommand SaveCommand { get; }

    /// <summary>
    /// Gets the command saving to a new file.
    /// </summary>
    public IRelayCommand SaveAsCommand { get; }

    /// <summary>
    /// Gets the command closing the application.
    /// </summary>
    public IRelayCommand ExitCommand { get; }

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
        var tile = CreateEmptyTile($"Tile {Tiles.Count + 1}", DefaultTileWidth, DefaultTileHeight);
        Tiles.Add(tile);
        SelectedTile = tile;
    }

    private static TileViewModel CreateEmptyTile(string name, int width, int height)
    {
        var glyphs = new GlyphCellViewModel[width * height];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                glyphs[row * width + column] = new GlyphCellViewModel(row, column, ' ', ConsoleColor.Gray, ConsoleColor.Black);
            }
        }

        return new TileViewModel(name, width, height, glyphs);
    }

    private void SelectGlyph(GlyphCellViewModel? glyph)
    {
        if (SelectedGlyph != null)
        {
            SelectedGlyph.SetSelection(false);
        }

        SelectedGlyph = glyph;
        SelectedGlyph?.SetSelection(true);
    }

    private void ApplyForeground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        if (SelectedForegroundSwatch != null)
        {
            SelectedForegroundSwatch.IsForegroundSelection = false;
        }

        SelectedForegroundSwatch = swatch;
        SelectedForegroundSwatch.IsForegroundSelection = true;
        SelectedGlyph?.SetForeground(swatch.Color);
    }

    private void ApplyBackground(ColorSwatchViewModel? swatch)
    {
        if (swatch == null)
        {
            return;
        }

        if (SelectedBackgroundSwatch != null)
        {
            SelectedBackgroundSwatch.IsBackgroundSelection = false;
        }

        SelectedBackgroundSwatch = swatch;
        SelectedBackgroundSwatch.IsBackgroundSelection = true;
        SelectedGlyph?.SetBackground(swatch.Color);
    }

    private static void ShowPlaceholderMessage(string context)
        => Debug.WriteLine($"Command '{context}' is not yet implemented.");
}
