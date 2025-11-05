using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenX.Base;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace AA14_ScreenX.ViewModels;

public partial class RenderViewModel : ObservableObject
{
    private readonly IRendererService _renderer;

    // Image size and source rectangle
    [ObservableProperty] private int width = 256;
    [ObservableProperty] private int height = 256;
    [ObservableProperty] private ExRect source = new(-16, -16, 16, 16);

    [ObservableProperty] private double x1=-1;
    [ObservableProperty] private double y1=-1;
    [ObservableProperty] private double x2 = 1;
    [ObservableProperty] private double y2 = 1;

    partial void OnX1Changed(double value) => Source = new ExRect(value, Y1, X2, Y2);
    partial void OnY1Changed(double value) => Source = new ExRect(X1, value, X2, Y2);
    partial void OnX2Changed(double value) => Source = new ExRect(X1, Y1, value, Y2);
    partial void OnY2Changed(double value) => Source = new ExRect(X1, Y1, X2, value);
    partial void OnSourceChanged(ExRect value)
    {
        // Keep scalar properties in sync when Source changes programmatically
        if (X1 != value.X1) X1 = value.X1;
        if (Y1 != value.Y1) Y1 = value.Y1;
        if (X2 != value.X2) X2 = value.X2;
        if (Y2 != value.Y2) Y2 = value.Y2;
    }

    // UI parameters (from LFM)
    [ObservableProperty] private int patternIndex = 1; // rgpBasePattern.ItemIndex =1
    [ObservableProperty] private bool isInverse;
    [ObservableProperty] private double xWidth = 16;
    [ObservableProperty] private double xOffset = -8;
    [ObservableProperty] private double yOffset = -8;
    [ObservableProperty] private int mandelBrLoop = 64;

    // Available transforms (CheckGroup)
    public ObservableCollection<SelectableItem> AvailableTransforms { get; } = new(
    new[]
    {
        new SelectableItem("Null"),
        new SelectableItem("Wobble"),
        new SelectableItem("Strudel"),
        new SelectableItem("Ballon"),
        new SelectableItem("Sauger"),
        new SelectableItem("Schnecke"),
        new SelectableItem("Strflucht"),
        new SelectableItem("Kugel"),
        new SelectableItem("Tunnel"),
        new SelectableItem("MandelBr1"),
        new SelectableItem("MandelBr2")
    });

    // Selected list (bind to ListBox)
    public IEnumerable<string> SelectedTransforms => AvailableTransforms.Where(t => t.IsSelected).Select(t => t.Name);

    // Render outputs
    [ObservableProperty] private uint[]? pixels;
    [ObservableProperty] private Bitmap? image;

    public RenderViewModel(IRendererService renderer)
    {
        _renderer = renderer;
    }

    [RelayCommand]
    private async Task RenderAsync(CancellationToken ct)
    {
        var functions = BuildFunctions();
        var colorizer = BuildColorizer();
        var opts = new RenderOptions(Width, Height, Source, functions, colorizer);

        var res = await Task.Run(() => _renderer.Render(opts, ct), ct);
        Pixels = res.Pixels;
        Image = Imaging.BitmapAdapter.FromArgbPixels(res.Width, res.Height, res.Pixels);
        OnPropertyChanged(nameof(SelectedTransforms));
    }

    private List<DFunction> BuildFunctions()
    {
        var list = new List<DFunction>();
        // always start from identity
        list.Add((ExPoint p, ExPoint p0, ref bool brk) => p);

        foreach (var t in AvailableTransforms.Where(t => t.IsSelected))
        {
            switch (t.Name)
            {
                case "Null":
                    list.Add(Transformations.Null);
                    break;
                case "Wobble":
                    list.Add(Transformations.Wobble3);
                    break;
                case "Strudel":
                    list.Add(Transformations.Strudel2);
                    break;
                case "Ballon":
                    list.Add(Transformations.Ballon);
                    break;
                case "Sauger":
                    list.Add(Transformations.Sauger);
                    break;
                case "Schnecke":
                    list.Add(Transformations.Schnecke2);
                    break;
                case "Strflucht":
                    list.Add(Transformations.Strflucht);
                    break;
                case "Kugel":
                    list.Add(Transformations.Kugel);
                    break;
                case "Tunnel":
                    list.Add(Transformations.Tunnel);
                    break;
                case "MandelBr1":
                    list.Add((ExPoint p, ExPoint p0, ref bool brk) => Transformations.MandelbrStepN(p, p0, ref brk, MandelBrLoop));
                    break;
                case "MandelBr2":
                    list.Add((ExPoint p, ExPoint p0, ref bool brk) => Transformations.MandelbrFull(p, p0, ref brk, MandelBrLoop));
                    break;
                default:
                    list.Add(Transformations.Null);
                    break;
            }
        }
        return list;
    }

    private CFunction BuildColorizer()
    {
        return PatternIndex switch
        {  
            0 => // Mosaik
             (ExPoint p) => Color32.FromRgb((byte)(((int)(p.X * XWidth + XOffset)) % 256), (byte)(((int)(p.Y * XWidth + YOffset)) % 256), 128),
            1 => // Farbstreifen
             (ExPoint p) => Color32.FromRgb((byte)((int)(Math.Sin((p.X+p.Y*0.5) * Math.PI) * 127.5 + 127.5)&0xff),
                                            (byte)((int)(Math.Sin((p.X - p.Y * 0.5) * Math.PI) * 127.5 + 127.5) & 0xff),
                                            (byte)((int)(Math.Sin((p.Y+0.25) * Math.PI) * 127.5 + 127.5) & 0xFF)),
            2 => // Bild (Platzhalter)
             (ExPoint p) => Color32.FromRgb((byte)(p.X * 255), (byte)(p.Y * 255), 0),
            3 => // Winkelfarbe (approx)
             (ExPoint p) =>
             {
                 var ang = System.Math.Atan2(p.Y , p.X ); // -pi..pi
                 var hue = (ang + System.Math.PI) / (2 * System.Math.PI); //0..1
                 return HsvToColor(hue,1.0,1.0- Math.Sqrt(p.X*p.X+p.Y*p.Y)*0.25);
             }
            ,
            _ => (ExPoint p) => Color32.FromRgb((byte)(p.X * 255), (byte)(p.Y * 255), 0)
        };
    }

    private static uint HsvToColor(double h, double s, double v)
    {
        var i = (int)(h * 6);
        var f = h * 6 - i;
        var p = v * (1 - s);
        var q = v * (1 - f * s);
        var t = v * (1 - (1 - f) * s);
        (double r, double g, double b) = (i % 6) switch
        {
            0 => (v, t, p),
            1 => (q, v, p),
            2 => (p, v, t),
            3 => (p, q, v),
            4 => (t, p, v),
            _ => (v, p, q)
        };
        return Color32.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }
}

public partial class SelectableItem : ObservableObject
{
    [ObservableProperty] private bool isSelected;
    public string Name { get; }
    public SelectableItem(string name) => Name = name;
}
