using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a Delphi TBitmap, holding raw bitmap data from LFM files
/// and providing conversion to WPF ImageSource.
/// </summary>
public class TBitmap
{
    private byte[]? _rawData;
    private ImageSource? _cachedImageSource;
    private bool _isDirty = true;
    private string _hexData = string.Empty;
    private int _width;
    private int _height;

    /// <summary>
    /// Gets or sets the raw bitmap data as hex string (from LFM format).
    /// </summary>
    public string HexData
    {
        get => _hexData;
        set
        {
            if (_hexData != value)
            {
                _hexData = value;
                _rawData = null;
                _cachedImageSource = null;
                _isDirty = true;
            }
        }
    }

    /// <summary>
    /// Gets the width of the bitmap (if available from header).
    /// </summary>
    public int Width
    {
        get => _width;
        private set => _width = value;
    }

    /// <summary>
    /// Gets the height of the bitmap (if available from header).
    /// </summary>
    public int Height
    {
        get => _height;
        private set => _height = value;
    }

    /// <summary>
    /// Gets whether the bitmap contains valid data.
    /// </summary>
    public bool HasData => !string.IsNullOrEmpty(HexData) || (_rawData != null && _rawData.Length > 0);

    /// <summary>
    /// Gets the raw byte data of the bitmap.
    /// </summary>
    public byte[] RawData
    {
        get
        {
            if (_rawData == null && !string.IsNullOrEmpty(HexData))
            {
                _rawData = ParseHexData(HexData);
            }
            return _rawData ?? [];
        }
    }

    /// <summary>
    /// Gets the bitmap as a WPF ImageSource.
    /// </summary>
    public ImageSource? ImageSource
    {
        get
        {
            if (_isDirty || _cachedImageSource == null)
            {
                _cachedImageSource = CreateImageSource();
                _isDirty = false;
            }
            return _cachedImageSource;
        }
    }

    /// <summary>
    /// Sets the raw bitmap data directly.
    /// </summary>
    public void SetRawData(byte[] data)
    {
        _rawData = data;
        _hexData = string.Empty;
        _cachedImageSource = null;
        _isDirty = true;
    }

    // existing methods like ParseHexData(...) remain unchanged below
    /// <summary>
    /// Parses hex string data from LFM format into byte array.
    /// </summary>
    private static byte[] ParseHexData(string hex)
    {
        var bytes = new List<byte>();
        string cleaned = new string(hex.Where(c => !char.IsWhiteSpace(c)).ToArray());
        for (int i = 0; i + 1 < cleaned.Length; i += 2)
        {
            if (byte.TryParse(cleaned.AsSpan(i, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b))
            {
                bytes.Add(b);
            }
        }
        return bytes.ToArray();
    }

    /// <summary>
    /// Creates an ImageSource from the stored raw data.
    /// </summary>
    private ImageSource? CreateImageSource()
    {
        var data = RawData;
        if (data.Length == 0)
            return null;

        // Try Lazarus ImageList format (Li / Lz header with raw/compressed RGBA strip)
        var imageListSource = TryCreateLazarusImageListSource(data);
        if (imageListSource != null)
            return imageListSource;

        try
        {     
            // Check for BMP header
            if (data.Length > 14 && data[0] == 0x42 && data[1] == 0x4D) // "BM"
            {
                return DecodeBmp(data);
            }

            // Check for PNG header
            if (data.Length > 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
            {
                return DecodePng(data);
            }

            // Try Delphi-specific bitmap format
            return DecodeDelphiBitmap(data);
        }
        catch
        {
            // If all decoding fails, return null
            return null;
        }
    }

    /// <summary>
    /// Decodes a standard BMP file.
    /// </summary>
    private static BitmapSource? DecodeBmp(byte[] data)
    {
        try
        {
            using var ms = new MemoryStream(data);
            var decoder = new BmpBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames.Count > 0 ? decoder.Frames[0] : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Decodes a standard PNG file.
    /// </summary>
    private static BitmapSource? DecodePng(byte[] data)
    {
        try
        {
            using var ms = new MemoryStream(data);
            var decoder = new PngBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            return decoder.Frames.Count > 0 ? decoder.Frames[0] : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Decodes Delphi's internal bitmap format.
    /// </summary>
    private BitmapSource? DecodeDelphiBitmap(byte[] data)
    {
        try
        {
            // Try to find BMP signature within the data
            int bmpOffset = FindBmpSignature(data);

            if (bmpOffset >= 0 && bmpOffset < data.Length - 14)
            {
                var bmpData = new byte[data.Length - bmpOffset];
                Array.Copy(data, bmpOffset, bmpData, 0, bmpData.Length);

                var result = DecodeBmp(bmpData);
                if (result != null)
                {
                    Width = result.PixelWidth;
                    Height = result.PixelHeight;
                    return result;
                }
            }

            // Try to decode as raw DIB
            return DecodeDib(data);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Finds the BMP signature "BM" in the data.
    /// </summary>
    private static int FindBmpSignature(byte[] data)
    {
        for (int i = 0; i < data.Length - 1; i++)
        {
            if (data[i] == 0x42 && data[i + 1] == 0x4D)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Decodes a DIB (Device Independent Bitmap) without file header.
    /// </summary>
    private BitmapSource? DecodeDib(byte[] data)
    {
        if (data.Length < 40)
            return null;

        try
        {
            int headerSize = BitConverter.ToInt32(data, 0);
            if (headerSize < 40)
                return null;

            int width = BitConverter.ToInt32(data, 4);
            int height = BitConverter.ToInt32(data, 8);
            short bitCount = BitConverter.ToInt16(data, 14);

            if (width <= 0 || width > 4096 || Math.Abs(height) > 4096)
                return null;

            Width = width;
            Height = Math.Abs(height);

            var bmpData = CreateBmpFromDib(data, width, height, bitCount);
            if (bmpData != null)
            {
                return DecodeBmp(bmpData);
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Creates a complete BMP file from DIB data.
    /// </summary>
    private static byte[] CreateBmpFromDib(byte[] dibData, int width, int height, int bitCount)
    {
        int rowSize = ((width * bitCount + 31) / 32) * 4;
        int pixelDataSize = rowSize * Math.Abs(height);
        int colorTableSize = bitCount <= 8 ? (1 << bitCount) * 4 : 0;
        int headerSize = 40;
        int fileHeaderSize = 14;
        int dataOffset = fileHeaderSize + headerSize + colorTableSize;
        int fileSize = dataOffset + pixelDataSize;

        if (dibData.Length < headerSize + colorTableSize)
            return dibData;

        var bmpFile = new byte[fileSize];

        bmpFile[0] = 0x42; // 'B'
        bmpFile[1] = 0x4D; // 'M'
        Array.Copy(BitConverter.GetBytes(fileSize), 0, bmpFile, 2, 4);
        Array.Copy(BitConverter.GetBytes(dataOffset), 0, bmpFile, 10, 4);

        int copyLength = Math.Min(dibData.Length, fileSize - fileHeaderSize);
        Array.Copy(dibData, 0, bmpFile, fileHeaderSize, copyLength);

        return bmpFile;
    }

    /// <summary>
    /// Creates a TBitmap from hex data string.
    /// </summary>
    public static TBitmap FromHexData(string hexData)
    {
        return new TBitmap { HexData = hexData };
    }

    /// <summary>
    /// Creates a TBitmap from raw byte data.
    /// </summary>
    public static TBitmap FromRawData(byte[] data)
    {
        var bitmap = new TBitmap();
        bitmap.SetRawData(data);
        return bitmap;
    }


    /// <summary>
    /// Tries to decode Lazarus TImageList bitmap blobs (Li/Lz header with 32-bit RGBA pixels).
    /// Returns null if the data does not match this format.
    /// </summary>
    private static ImageSource? TryCreateLazarusImageListSource(byte[] raw)
    {
        // Need at least magic + header (Li/Lz + 3 ints)
        if (raw.Length < 2 + 3 * sizeof(int))
            return null;

        using var ms = new MemoryStream(raw);
        using var br = new BinaryReader(ms);

        var b0 = br.ReadByte();
        var b1 = br.ReadByte();
        bool isLi = b0 == (byte)'L' && b1 == (byte)'i';
        bool isLz = b0 == (byte)'L' && b1 == (byte)'z';
        if (!isLi && !isLz)
            return null;

        try
        {
            // According to user format: magic, then count, width, height (all Int32, LE)
            int version = br.ReadInt32();
            int width = br.ReadInt32();
            int height = br.ReadInt32();

            if (version <=0 || width <= 0 || height <= 0)
                return null;

            byte[] pixelData;

            if (isLz)
            {
                // Remaining stream is zlib/deflate compressed RGBA data.
                // Lazarus usually writes a zlib stream (2-byte header). Try DeflateStream first,
                // fall back to skipping the first 2 bytes if necessary.
                long startPos = ms.Position+8;
                try
                {
                    using var deflate = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true);
                    using var outMs = new MemoryStream();
                    deflate.CopyTo(outMs);
                    pixelData = outMs.ToArray();
                }
                catch
                {
                    // Retry skipping potential zlib header
                    ms.Position = startPos + 2;
                    using var deflate = new DeflateStream(ms, CompressionMode.Decompress, leaveOpen: true);
                    using var outMs = new MemoryStream();
                    deflate.CopyTo(outMs);
                    pixelData = outMs.ToArray();
                }
            }
            else
            {
                // Li: uncompressed raw RGBA strip
                pixelData = br.ReadBytes((int)(ms.Length - ms.Position));
            }

            int bytesPerPixel = 4; // RGBA
            int pixelsPerImage = width * height;
            int count = pixelData.Length / pixelsPerImage/bytesPerPixel;
            int expectedBytes = count * pixelsPerImage * bytesPerPixel;
            if (pixelData.Length < expectedBytes || count <1)
                return null;

            int stripWidth = width ;
            int stripHeight = height * count;

            var wb = new WriteableBitmap(stripWidth, stripHeight, 96, 96, PixelFormats.Pbgra32, null);
            wb.WritePixels(new Int32Rect(0, 0, stripWidth, stripHeight), pixelData, stripWidth * bytesPerPixel, 0);

            return wb;
        }
        catch
        {
            return null;
        }
    }
}
