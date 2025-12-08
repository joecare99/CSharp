using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Treppen.Base;
using Treppen.Export.Models;
using Treppen.Export.Services.Interfaces;
using Treppen.Print.Services;
using Treppen.WPF.Printing;
using Treppen.WPF.Services.Interfaces;

namespace Treppen.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _size = 21;

    [ObservableProperty]
    private BitmapSource? _previewImage;

    [ObservableProperty]
    private IHeightLabyrinth _labyrinth;

    private readonly IDrawingService _drawingService;

    public MainWindowViewModel(IHeightLabyrinth labyrinth, IDrawingService drawingService)
    {
        _labyrinth = labyrinth;
        _drawingService = drawingService;
        Generate();
    }

    [RelayCommand]
    private void Generate()
    {
        var _l = Labyrinth;
        Labyrinth = null; // Clear reference to allow GC of previous labyrinth
        _l.Dimension = new System.Drawing.Rectangle(0, 0, Size, Size);
        _l.Generate();
        PreviewImage = _drawingService.CreateLabyrinthPreview(_l);
        Labyrinth=_l; // Notify that the labyrinth has changed
    }

    [RelayCommand]
    private void Draw()
    {
        var _l = Labyrinth;
        Labyrinth = null; // Clear reference to allow GC of previous labyrinth
        Labyrinth = _l; // Notify that the labyrinth has changed
    }

    [RelayCommand]
    private void Print()
    {
        IoC.GetKeyedRequiredService<IPrintRenderer>("prn").RenderAsync(Labyrinth,null,null).GetAwaiter().GetResult();
    }

    [RelayCommand]
    private void SaveAs()
    {
        var sfd = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Labyrinth exportieren",
            Filter = BuildFilter(),
            AddExtension = true,
            OverwritePrompt = true
        };
        if (sfd.ShowDialog() != true) return;

        var fileExt = System.IO.Path.GetExtension(sfd.FileName);
        // Registriere alle Exporter (Reihenfolge: WPF-3D-SVG zuletzt, damit es .svg überschreibt)
        PrintExporterRegistry.EnsureInitialized(
            typeof(Treppen.Print.Rendering.ObjPrintRenderer).Assembly,
            typeof(Svg3DPrintRenderer).Assembly);

        var type = PrintExporterRegistry.GetByExtension(fileExt);
        if (type == null)
        {
            MessageBox.Show($"Kein Exporter für '{fileExt}' gefunden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (Activator.CreateInstance(type) is not IPrintRenderer renderer)
        {
            MessageBox.Show($"Exporter '{type.Name}' konnte nicht erstellt werden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        using var fs = File.Create(sfd.FileName);
        var options = new PrintOptions
        {
            Title = "Treppen Labyrinth",
            CellSize = 40,
            MarginLeft = 10,
            MarginTop = 10,
            MarginRight = 10,
            MarginBottom = 10
        };
        renderer.RenderAsync(Labyrinth, options, fs).GetAwaiter().GetResult();
    }

    private static string BuildFilter()
    {
        // Registriere alle Exporter (WPF-3D-SVG zuletzt)
        PrintExporterRegistry.EnsureInitialized(
            typeof(Treppen.Print.Rendering.ObjPrintRenderer).Assembly,
            typeof(Svg3DPrintRenderer).Assembly);

        var svgType = PrintExporterRegistry.GetByName("SVG");
        var objType = PrintExporterRegistry.GetByName("Wavefront OBJ");
        string svgPart = svgType != null ? "SVG (*.svg)|*.svg" : string.Empty;
        string objPart = objType != null ? "Wavefront OBJ (*.obj)|*.obj" : string.Empty;
        string all = string.Join("|", new[] { svgPart, objPart }.Where(s => !string.IsNullOrEmpty(s)));
        return string.IsNullOrEmpty(all) ? "Alle Dateien (*.*)|*.*" : all;
    }
}
