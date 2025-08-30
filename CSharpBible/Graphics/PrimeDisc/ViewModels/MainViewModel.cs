// ViewModels/MainViewModel.cs
using PrimePlotter.Infrastructure;
using PrimePlotter.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimePlotter.ViewModels;

public class MainViewModel : BindableBase
{
    private int _imageWidth = 2000;
    private int _imageHeight = 2000;
    private int _primeCount = 200_000;
    private double _progress;
    private string _status = "Bereit.";
    private ImageSource? _bitmap;
    private bool _isRunning;
    private CancellationTokenSource? _cts;

    public int ImageWidth { get => _imageWidth; set => SetProperty(ref _imageWidth, value); }
    public int ImageHeight { get => _imageHeight; set => SetProperty(ref _imageHeight, value); }
    public int PrimeCount { get => _primeCount; set => SetProperty(ref _primeCount, value); }

    public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    public ImageSource? Bitmap { get => _bitmap; private set => SetProperty(ref _bitmap, value); }

    public RelayCommand StartCommand { get; }
    public RelayCommand CancelCommand { get; }

    public MainViewModel()
    {
        StartCommand = new RelayCommand(Start, () => !_isRunning);
        CancelCommand = new RelayCommand(Cancel, () => _isRunning);
    }

    private void Start()
    {
        if (_isRunning) return;
        _isRunning = true;
        StartCommand.RaiseCanExecuteChanged();
        CancelCommand.RaiseCanExecuteChanged();

        _cts = new CancellationTokenSource();
        Progress = 0;
        Status = "Starte...";

        var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        var progress = new Progress<double>(p => Progress = p);

        Task.Run(async () =>
        {
            try
            {
                // 1) Primzahlen
                Status = "Berechne Primzahlen...";
                var primes = PrimeService.FirstNPrimes(PrimeCount, progress, _cts.Token);

                // 2) Plotten in Puffer
                Status = "Zeichne Punkte...";
                var pixels = PlotService.PlotPointsDownsampled(primes, ImageWidth, ImageHeight, 1, progress, _cts.Token);

                // 3) Bitmap auf UI erzeugen und füllen
                await Task.Factory.StartNew(() =>
                {
                    var wb = new WriteableBitmap(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null);
                    var rect = new System.Windows.Int32Rect(0, 0, ImageWidth, ImageHeight);
                    int stride = ImageWidth * 4;
                    // pixels ist ARGB, WPF erwartet BGRA — Kanäle tauschen:
                    var bgra = ArgbToBgra(pixels);
                    wb.WritePixels(rect, bgra, stride, 0);
                    Bitmap = wb;

                    // 4) Automatisch speichern
                    string path = SaveToPictures(wb);
                    Status = $"Fertig. Gespeichert unter: {path}";
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (OperationCanceledException)
            {
                await Task.Factory.StartNew(() =>
                {
                    Status = "Abgebrochen.";
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() =>
                {
                    Status = "Fehler: " + ex.Message;
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            finally
            {
                _isRunning = false;
                await Task.Factory.StartNew(() =>
                {
                    StartCommand.RaiseCanExecuteChanged();
                    CancelCommand.RaiseCanExecuteChanged();
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        });
    }

    private void Cancel() => _cts?.Cancel();

    private static byte[] ArgbToBgra(int[] argb)
    {
        // ARGB int -> BGRA bytes
        var bytes = new byte[argb.Length * 4];
        int bi = 0;
        for (int i = 0; i < argb.Length; i++)
        {
            int c = argb[i];
            byte a = (byte)((c >> 24) & 0xFF);
            byte r = (byte)((c >> 16) & 0xFF);
            byte g = (byte)((c >> 8) & 0xFF);
            byte b = (byte)(c & 0xFF);
            bytes[bi++] = b;
            bytes[bi++] = g;
            bytes[bi++] = r;
            bytes[bi++] = a;
        }
        return bytes;
    }

    private static string SaveToPictures(BitmapSource bmp)
    {
        string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string file = $"PrimesPlot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        string path = Path.Combine(dir, file);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        using var fs = File.Open(path, FileMode.Create, FileAccess.Write);
        encoder.Save(fs);
        return path;
    }
}
