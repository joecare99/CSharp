using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimePlotter.Services.Interfaces;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrimePlotter.ViewModels;

/// <summary>
/// ViewModel zur steuernden Koordination der Primzahlberechnung und anschließenden
/// Visualisierung als Punktraster / Bitmap innerhalb einer WPF-Oberfläche.
/// </summary>
/// <remarks>
/// Kernaufgaben:
/// <list type="bullet">
/// <item><description>Ermittlung der ersten <see cref="PrimeCount"/> Primzahlen über <see cref="IPrimeService"/> (CPU-lastig).</description></item>
/// <item><description>Umrechnung der Primzahlen in Bildkoordinaten über <see cref="IPlotService"/>.</description></item>
/// <item><description>Erzeugung eines <see cref="WriteableBitmap"/> zur Anzeige sowie Persistierung als PNG-Datei.</description></item>
/// <item><description>Bereitstellung von Fortschritt, Statusmeldungen und Abbruchfunktion für die UI.</description></item>
/// </list>
/// Threading-Modell:
/// <para>Die rechenintensiven Teile laufen auf einem Hintergrundthread (Task.Run), UI-gebundene Updates (Properties, Bitmap-Erstellung)
/// werden über den UI-<see cref="TaskScheduler"/> marshalled.</para>
/// Abbruch:
/// <para>Ein laufender Vorgang kann über <see cref="CancelCommand"/> mittels <see cref="CancellationToken"/> abgebrochen werden.</para>
/// Performance:
/// <para>Zur Minimierung von UI-Blockaden werden nur finale Schritte (Bitmap-Erstellung / Property-Updates) im UI-Thread ausgeführt.
/// Arrays (ARGB -> BGRA) werden einmalig konvertiert.</para>
/// </remarks>
public partial class MainViewModel : ObservableObject
{
    private readonly IPrimeService _primeService;
    private readonly IPlotService _plotService;

    /// <summary>
    /// Zielbreite des zu erzeugenden Bitmaps in Pixeln.
    /// Muss &gt; 0 sein. Hohe Werte erhöhen Speicherbedarf und Rechenzeit beim Plotten.
    /// </summary>
    [ObservableProperty]
    private int imageWidth = 2000;

    /// <summary>
    /// Zielhöhe des zu erzeugenden Bitmaps in Pixeln.
    /// Muss &gt; 0 sein. Hohe Werte erhöhen Speicherbedarf und Rechenzeit beim Plotten.
    /// </summary>
    [ObservableProperty]
    private int imageHeight = 2000;

    /// <summary>
    /// Anzahl der zu berechnenden Primzahlen (Startend bei 2).
    /// Größere Werte erhöhen exponentiell die Gesamtdauer bei nicht optimalen Sieb-Implementierungen.
    /// </summary>
    [ObservableProperty]
    private int primeCount = 200_000;

    /// <summary>
    /// Fortschritt des aktuellen Arbeitsablaufs (0.0–100.0).
    /// Wird über einen <see cref="IProgress{T}"/>-Callback aktualisiert.
    /// </summary>
    [ObservableProperty]
    private double progress;

    /// <summary>
    /// Menschlich lesbare Statusanzeige für die UI (z. B. "Berechne Primzahlen...", "Fertig.", "Abgebrochen.").
    /// </summary>
    [ObservableProperty]
    private string status = "Bereit.";

    /// <summary>
    /// Ergebnis-Bitmap der geplotteten Primzahlen (BGRA-Format) zur Bindung im UI.
    /// Kann null sein, solange kein Plot abgeschlossen wurde.
    /// </summary>
    [ObservableProperty]
    private ImageSource? bitmap;

    /// <summary>
    /// Interner Ausführungszustand zur Steuerung der Command-Aktivierbarkeit.
    /// True während einer laufenden Berechnung / Plot-Operation.
    /// </summary>
    private bool _isRunning;

    /// <summary>
    /// Aktiver <see cref="CancellationTokenSource"/> für den kooperativen Abbruch langlaufender Operationen.
    /// Wird bei Start neu erzeugt und nach Abschluss verworfen.
    /// </summary>
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Erstellt eine neue Instanz des ViewModels mit abhängigen Diensten zur Primzahlberechnung und Punktprojektion.
    /// </summary>
    /// <param name="primeService">Dienst zur effizienten Ermittlung der ersten N Primzahlen.</param>
    /// <param name="plotService">Dienst zur Umrechnung / Projektion der Primzahlen in 2D-Koordinaten.</param>
    public MainViewModel(IPrimeService primeService, IPlotService plotService)
    {
        _primeService = primeService;
        _plotService = plotService;
    }

    /// <summary>
    /// Startet den end-to-end Prozess: Primzahlen berechnen, Punkte plotten, Bitmap erzeugen und abspeichern.
    /// </summary>
    /// <remarks>
    /// Ablauf:
    /// <list type="number">
    /// <item><description>Validierung des Ausführungszustands (kein paralleler Start).</description></item>
    /// <item><description>Initialisierung von Fortschritt und Status.</description></item>
    /// <item><description>Berechnung der Primzahlen über <see cref="IPrimeService.FirstNPrimes"/>.</description></item>
    /// <item><description>Plotten / Projektion der Werte über <see cref="IPlotService.PlotPointsDownsampled"/>.</description></item>
    /// <item><description>Erzeugung eines WriteableBitmap und Speicherung als PNG.</description></item>
    /// </list>
    /// Fehlerbehandlung:
    /// <para><see cref="OperationCanceledException"/> führt zu Status "Abgebrochen.".</para>
    /// <para>Allgemeine Ausnahmen werden mit Meldung "Fehler: ..." angezeigt.</para>
    /// UI-Thread:
    /// <para>Bitmap-Zuweisung sowie Status- und Command-Aktualisierung erfolgen über den UI-Scheduler.</para>
    /// </remarks>
    [RelayCommand(CanExecute = nameof(CanStart))]
    private void Start()
    {
        if (_isRunning) return;
        _isRunning = true;
        OnPropertyChanged(nameof(StartCommand));
        OnPropertyChanged(nameof(CancelCommand));
        _cts = new CancellationTokenSource();
        Progress = 0;
        Status = "Starte...";
        var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        var progressReporter = new Progress<double>(p => Progress = p);
        Task.Run(async () =>
        {
            try
            {
                Status = "Berechne Primzahlen...";
                var primes = _primeService.FirstNPrimes(PrimeCount, progressReporter, _cts.Token);
                Status = "Zeichne Punkte...";
                var pixels = _plotService.PlotPointsDownsampled(primes, ImageWidth, ImageHeight, 1, progressReporter, _cts.Token);
                await Task.Factory.StartNew(() =>
                {
                    var wb = new WriteableBitmap(ImageWidth, ImageHeight, 96, 96, PixelFormats.Bgra32, null);
                    var rect = new System.Windows.Int32Rect(0, 0, ImageWidth, ImageHeight);
                    int stride = ImageWidth * 4;
                    var bgra = ArgbToBgra(pixels);
                    wb.WritePixels(rect, bgra, stride, 0);
                    Bitmap = wb;
                    string path = SaveToPictures(wb);
                    Status = $"Fertig. Gespeichert unter: {path}";
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (OperationCanceledException)
            {
                await Task.Factory.StartNew(() => Status = "Abgebrochen.", CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            catch (Exception ex)
            {
                await Task.Factory.StartNew(() => Status = "Fehler: " + ex.Message, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
            finally
            {
                _isRunning = false;
                await Task.Factory.StartNew(() =>
                {
                    StartCommand.NotifyCanExecuteChanged();
                    CancelCommand.NotifyCanExecuteChanged();
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            }
        });
    }

    /// <summary>
    /// Prüft, ob der Startvorgang aktuell ausgeführt werden darf (kein laufender Prozess).
    /// </summary>
    /// <returns><c>true</c>, wenn keine Berechnung aktiv ist; sonst <c>false</c>.</returns>
    private bool CanStart() => !_isRunning;

    /// <summary>
    /// Löst einen kooperativen Abbruch des aktuellen Prozesses aus (falls vorhanden).
    /// </summary>
    /// <remarks>
    /// Der Abbruch wird erst wirksam, wenn die Dienste regelmäßig den <see cref="CancellationToken"/> prüfen
    /// und eine <see cref="OperationCanceledException"/> auslösen.
    /// </remarks>
    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _cts?.Cancel();

    /// <summary>
    /// Prüft, ob ein laufender Prozess abgebrochen werden kann.
    /// </summary>
    /// <returns><c>true</c>, wenn gerade eine Berechnung läuft; sonst <c>false</c>.</returns>
    private bool CanCancel() => _isRunning;

    /// <summary>
    /// Konvertiert ein int-Array im ARGB-Format (0xAARRGGBB) in ein Byte-Array im BGRA-Format
    /// passend für <see cref="PixelFormats.Bgra32"/>.
    /// </summary>
    /// <param name="argb">Quellfarben (pro Eintrag ein Pixel).</param>
    /// <returns>Byte-Array mit 4 Einträgen pro Pixel (B, G, R, A).</returns>
    /// <remarks>
    /// Performance: Linearer Durchlauf O(n). Keine zusätzlichen Allokationen außer Zielarray.
    /// </remarks>
    private static byte[] ArgbToBgra(int[] argb)
    {
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
    /// Speichert ein <see cref="BitmapSource"/> als PNG-Datei im Bilder-Ordner des aktuellen Benutzers.
    /// </summary>
    /// <param name="bmp">Das zu speichernde Bitmap.</param>
    /// <returns>Vollständiger Dateipfad der gespeicherten PNG-Datei.</returns>
    /// <remarks>
    /// Dateiname enthält Zeitstempel zur Kollisionsvermeidung.
    /// Wirft bei IO-Fehlern entsprechende Ausnahmen (<see cref="IOException"/>, <see cref="UnauthorizedAccessException"/>).
    /// </remarks>
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
