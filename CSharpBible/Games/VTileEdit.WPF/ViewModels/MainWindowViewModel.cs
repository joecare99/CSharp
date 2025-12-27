using CommonDialogs;
using CommonDialogs.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        CharacterPalette = new ObservableCollection<char>(Enumerable.Range(32, 96).Select(i => (char)i));

        SelectedTile = Tiles.FirstOrDefault();
        if (SelectedTile != null)
        {
            SelectGlyph(SelectedTile.Glyphs.FirstOrDefault());
        }

        ApplyForeground(Palette.FirstOrDefault());
        ApplyBackground(Palette.FirstOrDefault());

        UndoCommand = new RelayCommand(() => ShowPlaceholderMessage("Undo"));
        RedoCommand = new RelayCommand(() => ShowPlaceholderMessage("Redo"));
        SelectGlyphCommand = new RelayCommand<GlyphCellViewModel>(SelectGlyph, glyph => glyph != null);
        ApplyForegroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyForeground, swatch => swatch != null);
        ApplyBackgroundCommand = new RelayCommand<ColorSwatchViewModel>(ApplyBackground, swatch => swatch != null);
        ApplyCharacterCommand = new RelayCommand<char>(ApplyCharacter);
    }

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
            Filter = "Tile Files (*.tile)|*.tile|All Files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false,
        };
        if (ShowFileDlg?.Invoke(openDlg) == true)
        {
            _fileName = openDlg.FileName;
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (!string.IsNullOrEmpty(_fileName) && File.Exists(_fileName))
        {
            // Save to existing file
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
            Filter = "Tile Files (*.tile)|*.tile|All Files (*.*)|*.*",
            CheckFileExists = false,
        };
        if (ShowFileDlg?.Invoke(saveDlg) == true)
        {
            _fileName = saveDlg.FileName;
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

    private void ApplyCharacter(char character)
    {
        if (SelectedGlyph == null)
        {
            return;
        }

        SelectedGlyph.Character = character;
    }

    private static void ShowPlaceholderMessage(string context)
        => Debug.WriteLine($"Command '{context}' is not yet implemented.");
}
