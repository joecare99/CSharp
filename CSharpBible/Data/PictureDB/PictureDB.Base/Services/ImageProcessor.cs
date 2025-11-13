using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class ImageProcessor : IImageProcessor
{
    private const int MaxDimension = 1024; // max width or height in pixels
    private const int JpegQuality = 85;

    public string ConvertToBase64(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath)) throw new FileNotFoundException("Image file not found", filePath);

        using var image = Image.Load(filePath);

        int width = image.Width;
        int height = image.Height;

        // Determine scale factor to fit into MaxDimension
        double scale = 1.0;
        if (width > MaxDimension || height > MaxDimension)
        {
            scale = Math.Min((double)MaxDimension / width, (double)MaxDimension / height);
        }

        if (scale < 1.0)
        {
            int newWidth = Math.Max(1, (int)Math.Round(width * scale));
            int newHeight = Math.Max(1, (int)Math.Round(height * scale));
            image.Mutate(x => x.Resize(newWidth, newHeight));
        }

        using var ms = new MemoryStream();
        var encoder = new JpegEncoder { Quality = JpegQuality };
        image.SaveAsJpeg(ms, encoder);
        ms.Seek(0, SeekOrigin.Begin);
        var bytes = ms.ToArray();
        return Convert.ToBase64String(bytes);
    }
}