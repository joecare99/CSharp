using Microsoft.Win32;
using SuperResolutionOnnxSample.ImageTiling;
using SuperResolutionOnnxSample.SuperResolution;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample;

public partial class MainWindow : Window
{
    private readonly OnnxYSuperResampler _resampler = new();
    private readonly SuperResolutionPipeline _pipeline;

    private BitmapImage? _input;
    private BitmapImage? _output;

    public MainWindow()
    {
        InitializeComponent();
        _pipeline = new SuperResolutionPipeline(_resampler);
        Closed += (_, _) => _resampler.Dispose();
    }

    private void OnEstimateTileClick(object sender, RoutedEventArgs e)
    {
        if (_input == null)
        {
            StatusText.Text = "Bitte zuerst ein Bild laden.";
            return;
        }

        int tile = TileSizeEstimator.EstimateSquareTileSize(_input.PixelWidth, _input.PixelHeight, maxTiles: 2000);
        if (tile > 0)
        {
            TileSizeBox.Text = tile.ToString();
            StatusText.Text = $"Geschaetzt: {tile} px";
        }
        else
        {
            StatusText.Text = "Keine sinnvolle Tilegroesse erkannt.";
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
            StatusText.Text = "Berechne...";

            if (TilesetMode.IsChecked == true)
            {
                if (!int.TryParse(TileSizeBox.Text, out int tile) || tile <= 0)
                    throw new InvalidOperationException("Ungueltige Tilegroesse.");

                _output = await _pipeline.UpscaleTilesetAsync(_input, tile);
            }
            else
            {
                _output = await _pipeline.UpscaleImageAsync(_input);
            }

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

    private void OnLoadClick(object sender, RoutedEventArgs e)
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
