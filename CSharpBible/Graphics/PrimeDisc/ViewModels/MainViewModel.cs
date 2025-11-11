using PrimePlotter.Infrastructure;
using PrimePlotter.Services;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimePlotter.ViewModels;

/// <summary>
/// ViewModel für die Prime-Plot-Anwendung. Koordiniert die Berechnung von Primzahlen,
/// deren Visualisierung sowie Fortschritts- und Statusmeldungen für die UI.
/// </summary>
/// <remarks>
/// Ablauf (Start):
/// 1. Erzeugt eine <see cref="CancellationTokenSource"/> zur späteren Abbruchsteuerung.
/// 2. Berechnet die ersten <see cref="PrimeCount"/> Primzahlen über <see cref="PrimeService.FirstNPrimes"/>.
/// 3. Wandelt die berechneten Punkte in ein Pixelpuffer-Array mittels <see cref="PlotService.PlotPointsDownsampled"/>.
/// 4. Erzeugt einen <see cref="WriteableBitmap"/> auf dem UI-Thread, konvertiert ARGB nach BGRA und schreibt die Pixel.
/// 5. Speichert das Ergebnis automatisiert als PNG in den Bilder-Ordner des Benutzers.
/// 6. Aktualisiert Status und Fortschritt während der Phasen.
/// 
/// Threading:
/// - Rechenintensive Teile laufen in einem Hintergrund-Task (ThreadPool).
/// - UI-gebundene Änderungen (Properties, Bitmap-Erzeugung) erfolgen über den UI-<see cref="TaskScheduler"/>.
/// 
/// Abbruch:
/// - Ein laufender Vorgang kann über <see cref="Cancel"/> mit <see cref="_cts"/> abgebrochen werden.
/// - Bei Abbruch wird der Status auf "Abgebrochen." gesetzt.
/// 
/// Fehlerbehandlung:
/// - Exceptions werden gefangen und der <see cref="Status"/> entsprechend gesetzt.
/// 
/// Wiederverwendbarkeit:
/// - Die Klasse ist bewusst zustandsbehaftet (<see cref="_isRunning"/>) und nicht für gleichzeitige parallele Starts ausgelegt.
/// </remarks>
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

    /// <summary>
    /// Breite des zu erzeugenden Bildes in Pixeln.
    /// </summary>
    /// <remarks>Wird beim Start für die Erstellung des <see cref="WriteableBitmap"/> verwendet.</remarks>
    public int ImageWidth { get => _imageWidth; set => SetProperty(ref _imageWidth, value); }

    /// <summary>
    /// Höhe des zu erzeugenden Bildes in Pixeln.
    /// </summary>
    /// <remarks>Wird beim Plotten und bei der Bitmap-Erstellung verwendet.</remarks>
    public int ImageHeight { get => _imageHeight; set => SetProperty(ref _imageHeight, value); }

    /// <summary>
    /// Anzahl der zu berechnenden Primzahlen.
    /// </summary>
    /// <remarks>Größere Werte erhöhen die Rechenzeit und Speicherlast.</remarks>
    public int PrimeCount { get => _primeCount; set => SetProperty(ref _primeCount, value); }

    /// <summary>
    /// Fortschritt (0.0 bis 1.0) der aktuellen Operation (Berechnung / Plotten).
    /// </summary>
    /// <remarks>Wird von den Services inkrementell gemeldet.</remarks>
    public double Progress { get => _progress; private set => SetProperty(ref _progress, value); }

    /// <summary>
    /// Statusmeldung für die UI (z. B. "Berechne Primzahlen...", "Fertig.", Fehlermeldungen).
    /// </summary>
    public string Status { get => _status; private set => SetProperty(ref _status, value); }

    /// <summary>
    /// Fertig erzeugtes Bild der Primzahl-Punkte als <see cref="ImageSource"/>.
    /// </summary>
    /// <remarks>Nach erfolgreichem Abschluss gesetzt, ansonsten <c>null</c> bei Start/Abbruch.</remarks>
    public ImageSource? Bitmap { get => _bitmap; private set => SetProperty(ref _bitmap, value); }

    /// <summary>
    /// Command zum Starten des gesamten Berechnungs- und Plot-Vorgangs.
    /// </summary>
    /// <remarks>Nur ausführbar, wenn keine laufende Operation aktiv ist.</remarks>
    public RelayCommand StartCommand { get; }

    /// <summary>
    /// Command zum Abbrechen eines laufenden Vorgangs.
    /// </summary>
    /// <remarks>Nur aktiv, solange <see cref="_isRunning"/> true ist.</remarks>
    public RelayCommand CancelCommand { get; }

    /// <summary>
    /// Initialisiert eine neue Instanz des <see cref="MainViewModel"/>.
    /// </summary>
    public MainViewModel()
    {
        StartCommand = new RelayCommand(Start, () => !_isRunning);
        CancelCommand = new RelayCommand(Cancel, () => _isRunning);
    }

    /// <summary>
    /// Startet asynchron die Berechnung der Primzahlen und das anschließende Plotten sowie Speichern.
    /// </summary>
    /// <remarks>
    /// Schützt sich gegen parallelen Start mit <see cref="_isRunning"/>.
    /// Verwendet einen Hintergrund-Task und marshalt UI-Updates zurück auf den UI-Thread.
    /// </remarks>
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

    /// <summary>
    /// Bricht einen laufenden Vorgang ab, sofern vorhanden.
    /// </summary>
    /// <remarks>Setzt das <see cref="CancellationToken"/> für die Hintergrundarbeit.</remarks>
    private void Cancel() => _cts?.Cancel();

    /// <summary>
    /// Konvertiert ein ARGB-Integer-Array (je 32 Bit) in ein BGRA-Byte-Array (für WPF <see cref="PixelFormats.Bgra32"/>).
    /// </summary>
    /// <param name="argb">Quell-Array mit Farbwerten im Format 0xAARRGGBB.</param>
    /// <returns>Byte-Array mit Kanalreihenfolge B, G, R, A.</returns>
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

    /// <summary>
    /// Speichert eine Bitmap als PNG-Datei im Bilder-Ordner des aktuellen Benutzers.
    /// </summary>
    /// <param name="bmp">Zu speichernde Bitmap-Quelle.</param>
    /// <returns>Vollständiger Pfad der gespeicherten Datei.</returns>
    /// <exception cref="IOException">Bei Problemen mit Dateizugriff oder Schreibvorgang.</exception>
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
