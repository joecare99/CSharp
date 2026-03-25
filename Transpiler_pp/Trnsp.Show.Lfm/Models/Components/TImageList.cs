using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TImageList component containing multiple bitmap images.
/// </summary>
public partial class TImageList : LfmComponentBase
{
    [ObservableProperty]
    private int _imageWidth = 16;

    [ObservableProperty]
    private int _imageHeight = 16;

    /// <summary>
    /// The combined bitmap data containing all images in the list.
    /// </summary>
    private TBitmap? _bitmap;
    public TBitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            if (SetProperty(ref _bitmap, value))
            {
                ExtractImagesFromBitmap();
            }
        }
    }

    static bool TryParseLazarusImageListHeader(byte[] data, out int width, out int height, out int payloadOffset)
    {
        width = height = 0;
        payloadOffset = -1;
        if (data.Length < 8) return false;
        // "Lz" signature
        if (data[0] != 0x4C || data[1] != 0x7A) return false;
        // Beispielformat: 4C 7A <ver?> <ver?> <width 4 bytes little-endian> <height 4 bytes little-endian>
        // In Ihren Beispielen folgen oft 3*4-Byte ints; wir lesen die nächsten zwei ints als width/height (sicherstellen)
        // Anpassung falls Format abweicht.
        // Hier: bytes 2..5 = int32 (evtl version), bytes 6..9 = width, bytes 10..13 = height
        // Wir versuchen mehrere Offsets robust:
        try
        {
            // Versuch: nach signature kommt 4 bytes (ver), dann width (4), height (4)
            int ver = BitConverter.ToInt32(data, 2);
            width = BitConverter.ToInt32(data, 6);
            height = BitConverter.ToInt32(data, 10);
            // sanity check
            if (width <= 0 || height <= 0 || width > 1024 || height > 1024)
            {
                // alternative: maybe width/height direkt nach signature
                width = BitConverter.ToInt32(data, 2);
                height = BitConverter.ToInt32(data, 6);
                payloadOffset = 10;
            }
            else
            {
                payloadOffset = 14;
            }
        }
        catch
        {
            return false;
        }
        // find start of zlib (0x78 0x9C or 0x78 0xDA)
        for (int i = payloadOffset; i < data.Length - 1; i++)
        {
            if (data[i] == 0x78 && (data[i + 1] == 0x9C || data[i + 1] == 0xDA)) { payloadOffset = i; return true; }
        }
        return false;
    }

    /// <summary>
    /// Collection of individual images extracted from the combined bitmap.
    /// </summary>
    public ObservableCollection<ImageSource?> Images { get; } = [];

    /// <summary>
    /// Gets the total number of images in this image list.
    /// </summary>
    public int Count => Images.Count;

    public TImageList()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "width":
                ImageWidth = ConvertToInt(value, 16);
                break;
            case "height":
                ImageHeight = ConvertToInt(value, 16);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }

    protected override void FinalizePendingHexData()
    {
        base.FinalizePendingHexData();
        
        // After base processing, try to extract images from the bitmap
        if (Glyph?.HasData == true)
        {
            Bitmap = Glyph;
        }
    }

    partial void OnImageWidthChanged(int value) => ExtractImagesFromBitmap();

    partial void OnImageHeightChanged(int value) => ExtractImagesFromBitmap();

    /// <summary>
    /// Extracts individual images from the combined bitmap strip.
    /// </summary>
    private void ExtractImagesFromBitmap()
    {
        Images.Clear();

        if (Bitmap?.ImageSource == null)
            return;

        var source = Bitmap.ImageSource as BitmapSource;
        if (source == null)
        {
            // Fallback: just add the whole image
            Images.Add(Bitmap.ImageSource);
            OnPropertyChanged(nameof(Count));
            return;
        }

        // Calculate number of images in the strip (horizontal layout)
        int bitmapWidth = source.PixelWidth;
        int bitmapHeight = source.PixelHeight;
        
        if (ImageWidth <= 0 || ImageHeight <= 0)
        {
            Images.Add(source);
            OnPropertyChanged(nameof(Count));
            return;
        }

        int cols = bitmapWidth / ImageWidth;
        int rows = bitmapHeight / ImageHeight;

        if (cols <= 0) cols = 1;
        if (rows <= 0) rows = 1;

        // Extract individual images
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                try
                {
                    var rect = new Int32Rect(col * ImageWidth, row * ImageHeight, ImageWidth, ImageHeight);
                    
                    // Ensure rect is within bounds
                    if (rect.X + rect.Width > bitmapWidth || rect.Y + rect.Height > bitmapHeight)
                        continue;

                    var croppedBitmap = new CroppedBitmap(source, rect);
                    Images.Add(croppedBitmap);
                }
                catch
                {
                    // If cropping fails, add null as placeholder
                    Images.Add(null);
                }
            }
        }

        OnPropertyChanged(nameof(Count));
    }

    /// <summary>
    /// Gets an image at the specified index.
    /// </summary>
    public ImageSource? GetImage(int index)
    {
        if (index < 0 || index >= Images.Count)
            return null;

        return Images[index];
    }
}
