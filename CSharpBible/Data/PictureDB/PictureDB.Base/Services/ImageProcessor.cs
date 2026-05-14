using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class ImageProcessor : IImageProcessor
{
    private const int MaxDimension = 512; // max width or height in pixels
    private const long JpegQuality = 85;

    public string ConvertToBase64(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath)) throw new FileNotFoundException("Image file not found", filePath);

        using Image sourceImage = Image.FromFile(filePath);

        int width = sourceImage.Width;
        int height = sourceImage.Height;

        double scale = 1.0;
        if (width > MaxDimension || height > MaxDimension)
        {
            scale = Math.Min((double)MaxDimension / width, (double)MaxDimension / height);
        }

        int targetWidth = Math.Max(1, (int)Math.Round(width * scale));
        int targetHeight = Math.Max(1, (int)Math.Round(height * scale));

        using Bitmap bitmap = new(targetWidth, targetHeight, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(sourceImage, 0, 0, targetWidth, targetHeight);
        }

        using MemoryStream ms = new();
        ImageCodecInfo? encoder = ImageCodecInfo.GetImageEncoders()
            .FirstOrDefault(item => item.FormatID == ImageFormat.Jpeg.Guid);

        if (encoder is null)
        {
            throw new InvalidOperationException("JPEG encoder not found.");
        }

        using EncoderParameters encoderParameters = new(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, JpegQuality);
        bitmap.Save(ms, encoder, encoderParameters);
        return Convert.ToBase64String(ms.ToArray());
    }
}