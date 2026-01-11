using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample;

public partial class MainWindow : Window
{
    private readonly OnnxSuperResizer _resizer = new();
    private BitmapImage? _input;
    private BitmapImage? _output;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnLoadClick(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
        };
        if (dlg.ShowDialog() == true)
        {
            _input = LoadBitmap(dlg.FileName);
            InputImage.Source = _input;
            StatusText.Text = $"Geladen: {System.IO.Path.GetFileName(dlg.FileName)}";
        }
    }

    private async void OnSuperResClick(object sender, RoutedEventArgs e)
    {
        if (_input == null)
        {
            StatusText.Text = "Bitte zuerst ein Bild laden.";
            return;
        }

        try
        {
            StatusText.Text = "Lade Modell...";
            await _resizer.EnsureModelAsync();
            StatusText.Text = "Berechne...";
            _output = await _resizer.UpscaleAsync(_input);
            OutputImage.Source = _output;
            StatusText.Text = "Fertig.";
        }
        catch (Exception ex)
        {
            StatusText.Text = ex.Message;
        }
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (_output == null)
        {
            StatusText.Text = "Kein Ergebnis zum Speichern.";
            return;
        }
        var dlg = new SaveFileDialog
        {
            Filter = "PNG|*.png",
            FileName = "upscaled.png"
        };
        if (dlg.ShowDialog() == true)
        {
            using var fs = new FileStream(dlg.FileName, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_output));
            encoder.Save(fs);
            StatusText.Text = $"Gespeichert: {dlg.FileName}";
        }
    }

    private static BitmapImage LoadBitmap(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = fs;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }
}
