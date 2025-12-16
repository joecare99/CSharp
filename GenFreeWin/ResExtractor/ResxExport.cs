using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;
using System.ComponentModel.Design;
using System.Windows.Forms;
static class ResxExport
{
    public static void ExportImages(string resxPath, string outputDir)
    {
        Directory.CreateDirectory(outputDir);

        using var reader = new ResXResourceReader(resxPath)
        {
            UseResXDataNodes = true // wichtig für Zugriff auf RawData
        };

        foreach (DictionaryEntry de in reader)
        try{
            var node = (ResXDataNode)de.Value;
            object? value = node.GetValue((ITypeResolutionService?)null);
            if (value is Bitmap bmp)
            {
                // Originalformat bestimmen
                var fmt = bmp.RawFormat;
                string ext = ImageCodecInfo.GetImageEncoders()
                               .FirstOrDefault(c => c.FormatID == fmt.Guid)?
                               .FilenameExtension?.Split(';')[0].Trim('*').ToLower()
                             ?? "png";

                string file = Path.Combine(outputDir, de.Key + "." + ext);

                // Verlustfrei speichern (falls kein Encoder gefunden, PNG als Fallback)
                if (ext == "png")
                    bmp.Save(file, ImageFormat.Png);
                else
                    bmp.Save(file, fmt);
            }
        }
            catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Exportieren von Ressource '{de.Key}': {ex.Message}");
            }
    }
}