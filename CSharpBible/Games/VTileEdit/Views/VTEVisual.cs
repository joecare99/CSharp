using System;
using System.ComponentModel;
using System.Drawing;
using VTileEdit.ViewModels;
using VTileEdit.Models;

namespace VTileEdit.Views;

public class VTEVisual : IVisual
{
    private IVTEViewModel _viewModel;

    public VTEVisual(IVTEViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.PropertyChanged += OnPropertyChanged;
        _viewModel.DoNewTileDialog = () =>
        {
            Console.Write("Tile-Breite: ");
            var wstr = Console.ReadLine();
            Console.Write("Tile-Höhe: ");
            var hstr = Console.ReadLine();
            if (int.TryParse(wstr, out int w) && int.TryParse(hstr, out int h) && w > 0 && h > 0)
            {
                return new Size(w, h);
            }
            return Size.Empty;
        };
    }

    public void HandleUserInput()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("VTileEdit - Menu");
            if (_viewModel.NewTilesCommand.CanExecute(this))
                Console.WriteLine("1) Neu");
            if (_viewModel.LoadTilesCommand.CanExecute(this))
                Console.WriteLine("2) Laden");
            if (_viewModel.SaveTilesCommand.CanExecute(this))
                Console.WriteLine("3) Speichern");
            if (_viewModel.SelectTileCommand.CanExecute(this))
                Console.WriteLine("4) Tile auswählen");
            if (_viewModel.EditTileCommand.CanExecute(this))
                Console.WriteLine("5) Tile editieren");
            Console.WriteLine("0) Beenden");
            Console.Write("Auswahl: ");
            var key = Console.ReadKey();
            Console.WriteLine();
            switch (key.KeyChar)
            {
                case '1': _viewModel.NewTilesCommand.Execute(this); break;
                case '2': _viewModel.LoadTilesCommand.Execute(this); break;
                case '3': _viewModel.SaveTilesCommand.Execute(this); break;
                case '4': _viewModel.SelectTileCommand.Execute(this); break;
                case '5': _viewModel.EditTileCommand.Execute(this); break;
                case '0': _viewModel.QuitCommand.Execute(this); break;
            }
        }
    }

    private void SavePath()
    {
        Console.Write("Datei speichern als (*.tdf;*.tdt;*.tdj;*.tdx;*.cs): ");
        var path = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(path))
        {
            try { _viewModel.SaveToPath(path); Console.WriteLine("Gespeichert."); }
            catch (Exception ex) { Console.WriteLine($"Fehler: {ex.Message}"); }
        }
    }

    private void SelectTile()
    {
        Console.Write("Enum-Wert des Tiles eingeben (Zahl): ");
        var s = Console.ReadLine();
        if (int.TryParse(s, out int val))
        {
            var enumType = _viewModel.Model.KeyType;
            var key = (Enum)Enum.ToObject(enumType, val);
            _viewModel.SelectTile(key);
        }
    }

    private void EditTile()
    {
        if (_viewModel.SelectedTile == null)
        {
            Console.WriteLine("Kein Tile ausgewählt.");
            return;
        }
        var size = _viewModel.Model.TileSize;
        if (size == Size.Empty)
        {
            Console.WriteLine("TileSize nicht gesetzt. Bitte zuerst laden oder Größe definieren.");
            return;
        }

        var lines = _viewModel.CurrentLines.Length == size.Height ? (string[])_viewModel.CurrentLines.Clone() : new string[size.Height];
        for (int y = 0; y < size.Height; y++)
        {
            if (lines[y] == null || lines[y].Length != size.Width) lines[y] = new string(' ', size.Width);
        }
        var colors = _viewModel.CurrentColors.Length == size.Width * size.Height ? (FullColor[])_viewModel.CurrentColors.Clone() : new FullColor[size.Width * size.Height];

        int cx = 0, cy = 0;
        ConsoleKey key;
        do
        {
            Console.Clear();
            RenderPreview(lines, colors, size, cx, cy);
            Console.WriteLine();
            Console.WriteLine("Pfeile bewegen, 'c' Zeichen setzen, 'f' Vordergrund, 'b' Hintergrund, 'q' zurück");
            key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.LeftArrow: cx = Math.Max(0, cx - 1); break;
                case ConsoleKey.RightArrow: cx = Math.Min(size.Width - 1, cx + 1); break;
                case ConsoleKey.UpArrow: cy = Math.Max(0, cy - 1); break;
                case ConsoleKey.DownArrow: cy = Math.Min(size.Height - 1, cy + 1); break;
                case ConsoleKey.C:
                    Console.Write("Zeichen eingeben: ");
                    var ch = Console.ReadKey(true).KeyChar;
                    var arr = lines[cy].ToCharArray();
                    arr[cx] = ch; lines[cy] = new string(arr);
                    break;
                case ConsoleKey.F:
                    colors[cy * size.Width + cx].fgr = PickColor("Vordergrundfarbe (0-15)");
                    break;
                case ConsoleKey.B:
                    colors[cy * size.Width + cx].bgr = PickColor("Hintergrundfarbe (0-15)");
                    break;
            }
        } while (key != ConsoleKey.Q);

        _viewModel.UpdateCurrentTile(lines, colors);
    }

    private ConsoleColor PickColor(string prompt)
    {
        Console.WriteLine(prompt);
        for (int i = 0; i < 16; i++) Console.WriteLine($"{i}: {(ConsoleColor)i}");
        var s = Console.ReadLine();
        if (int.TryParse(s, out int v) && v >= 0 && v < 16) return (ConsoleColor)v;
        return ConsoleColor.White;
    }

    private void RenderPreview(string[] lines, FullColor[] colors, Size size, int cx, int cy)
    {
        for (int y = 0; y < size.Height; y++)
        {
            for (int x = 0; x < size.Width; x++)
            {
                var idx = y * size.Width + x;
                var (fg, bg) = idx < colors.Length ? colors[idx] : new FullColor(ConsoleColor.White, ConsoleColor.Black);
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
                var ch = x < lines[y].Length ? lines[y][x] : ' ';
                if (x == cx && y == cy)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(ch);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public bool ShowFileDialog(IFileDialogData fileDialog)
    {
        // Not used
        return false;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Future: update UI on changes
    }

    private static string? ShowOpenFileDialog()
    {
        string? chosenPath = null;
        try
        {
            var t = new System.Threading.Thread(() =>
            {
                using var ofd = new System.Windows.Forms.OpenFileDialog
                {
                    Title = "Datei öffnen",
                    Filter = "Tile-Dateien (*.tdf;*.tdt;*.tdj;*.tdx)|*.tdf;*.tdt;*.tdj;*.tdx|Alle Dateien (*.*)|*.*",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = false,
                    RestoreDirectory = true,
                    InitialDirectory = Environment.CurrentDirectory,
                    DefaultExt = "tdf"
                };

                var result = ofd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    chosenPath = ofd.FileName;
                }
            });

            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
        }
        catch
        {
            // Ignorieren, null zurückgeben
        }

        return chosenPath;
    }
}
