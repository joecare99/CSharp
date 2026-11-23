using System;
using System.Buffers.Binary;
using System.IO;

namespace Avln_ImageEditor.Controls.Models;

/// <summary>Limits accepted encoded images before invoking the platform decoder.</summary>
public static class ImageDecodeLimits
{
    public const int MaxEncodedBytes = 32 * 1024 * 1024;
    public const int MaxDimension = 8192;
    public const int MaxPixels = 4 * 1024 * 1024;

    public static void ValidateSize(int width, int height)
    {
        if (width <= 0 || height <= 0 || width > MaxDimension || height > MaxDimension ||
            (long)width * height > MaxPixels)
            throw new InvalidDataException("Image dimensions exceed the supported limits.");
    }

    public static void ValidateEncoded(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length == 0 || bytes.Length > MaxEncodedBytes)
            throw new InvalidDataException("Encoded image is empty or exceeds 32 MiB.");

        ReadOnlySpan<byte> data = bytes;
        if (data.Length >= 24 && data.Slice(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) &&
            BinaryPrimitives.ReadUInt32BigEndian(data.Slice(8, 4)) == 13 && data.Slice(12, 4).SequenceEqual("IHDR"u8))
        {
            var width = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(16, 4));
            var height = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(20, 4));
            if (width > int.MaxValue || height > int.MaxValue) throw new InvalidDataException("Invalid PNG dimensions.");
            ValidateSize((int)width, (int)height);
            return;
        }

        if (data.Length >= 26 && data[0] == 'B' && data[1] == 'M')
        {
            uint header = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(14, 4));
            if (header == 12)
                ValidateSize(BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(18, 2)), BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(20, 2)));
            else if (header >= 40 && header <= data.Length - 14)
            {
                int width = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(18, 4));
                int height = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(22, 4));
                if (height == int.MinValue) throw new InvalidDataException("Invalid BMP height.");
                ValidateSize(width, Math.Abs(height));
            }
            else throw new InvalidDataException("Unsupported BMP header.");
            return;
        }

        if (data.Length >= 4 && data[0] == 0xff && data[1] == 0xd8)
        {
            int position = 2;
            while (position < data.Length)
            {
                if (data[position++] != 0xff) break;
                while (position < data.Length && data[position] == 0xff) position++;
                if (position >= data.Length) break;
                byte marker = data[position++];
                if (marker == 0xd9 || marker == 0xda) break;
                if (marker == 0x01 || (marker >= 0xd0 && marker <= 0xd7)) continue;
                if (position + 2 > data.Length) break;
                int length = BinaryPrimitives.ReadUInt16BigEndian(data.Slice(position, 2));
                if (length < 2 || length > data.Length - position) break;
                if ((marker >= 0xc0 && marker <= 0xcf) && marker != 0xc4 && marker != 0xc8 && marker != 0xcc)
                {
                    if (length < 8) break;
                    ValidateSize(BinaryPrimitives.ReadUInt16BigEndian(data.Slice(position + 5, 2)),
                        BinaryPrimitives.ReadUInt16BigEndian(data.Slice(position + 3, 2)));
                    return;
                }
                position += length;
            }
        }
        throw new InvalidDataException("Unsupported or malformed PNG, JPEG or BMP image.");
    }
}
