using System.Drawing.Imaging;

/// <summary>
/// Einstiegspunkt der Anwendung zur Erstellung und Speicherung von farbcodierten Hilbert-Farbkarten.
/// </summary>
/// <remarks>
/// - Zielplattform: .NET 8, C# 12
/// - Erfordert das NuGet-Paket <c>System.Drawing.Common</c>, um <see cref="Bitmap"/> und verwandte Typen verwenden zu können.
/// - Bilder werden im Benutzer-Bilderordner unter <c>HilbertColorMaps</c> gespeichert.
/// - Für verschiedene Parameterkombinationen werden PNG-Dateien erzeugt.
/// </remarks>
public static class Program
{
    /// <summary>
    /// Hauptmethode der Anwendung. Generiert Hilbert-Farbkarten-Bitmaps mit unterschiedlichen Parametern
    /// und speichert diese als PNG-Dateien im Bilderordner des Benutzers.
    /// </summary>
    /// <param name="args">
    /// Optionale Kommandozeilenargumente (derzeit ungenutzt).
    /// </param>
    /// <remarks>
    /// Die Bildgröße wird als Potenz von 2 festgelegt (<c>2^exponent</c>).
    /// Die Schleifen <c>i</c> und <c>j</c> bestimmen verschiedene Konfigurationen für die Farbkartenerzeugung.
    /// </remarks>
    /// <exception cref="System.Exception">
    /// Ausnahmen, die beim Speichern der Bilddateien auftreten können, werden abgefangen und protokolliert.
    /// </exception>
    static void Main(string[] args)
    {
        // Kantenlänge des Bildes wählen (2 hoch 10 = 1024 Pixel)
        int exponent = 11;

        // Zielverzeichnis vorbereiten
        var PictureDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "HilbertColorMaps");
        Directory.CreateDirectory(PictureDir);

        // Parameterkombinationen durchlaufen und Bilder erzeugen
        for (int i = 1; i < 7; i++)
        {
            for (int j = 1; j < 7; j++)
            {
                Bitmap resultImage = HilpertColorMap.Map3DTo2DColor(exponent, i, j);
                int size = (int)Math.Pow(2, exponent);
                string filename = $"Hilbert_Color_Map_HSB_{size}_{j}{" abcdefg"[i]}.png";

                // Bild speichern
                try
                {
                    resultImage.Save(Path.Combine(PictureDir, filename), ImageFormat.Png);
                    Console.WriteLine($"Bild wurde als '{filename}' gespeichert.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Speichern des Bildes: {ex.Message}");
                }
            }
        }
    }
}