using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Components;

internal static class ImageDocumentFileStorage
{
    internal const int MaxEncodedBytes = 32 * 1024 * 1024;

    public static async Task<byte[]> ReadAsync(string path, CancellationToken cancellationToken)
    {
        await using FileStream source = new(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
        if (source.Length > MaxEncodedBytes)
        {
            throw new InvalidDataException("The image exceeds the 32 MiB encoded-file limit.");
        }

        using MemoryStream result = new((int)source.Length);
        byte[] buffer = new byte[81920];
        int read;
        while ((read = await source.ReadAsync(buffer, cancellationToken)) != 0)
        {
            if (result.Length + read > MaxEncodedBytes)
            {
                throw new InvalidDataException("The image exceeds the 32 MiB encoded-file limit.");
            }

            result.Write(buffer, 0, read);
        }

        return result.ToArray();
    }

    public static async Task WriteAsync(
        string path,
        byte[] png,
        byte[]? expectedHash,
        CancellationToken cancellationToken)
    {
        if (png.Length > MaxEncodedBytes)
        {
            throw new InvalidDataException("The encoded PNG exceeds the 32 MiB file limit.");
        }

        string directory = Path.GetDirectoryName(path)
            ?? throw new IOException("The image destination has no parent directory.");
        string temporaryPath = Path.Combine(directory, $".avln-image-{Guid.NewGuid():N}.tmp");
        bool staged = false;
        try
        {
            await using (FileStream destination = new(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 81920, FileOptions.Asynchronous))
            {
                staged = true;
                await destination.WriteAsync(png, cancellationToken);
                await destination.FlushAsync(cancellationToken);
            }

            if (expectedHash is not null)
            {
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                {
                    throw new IOException("The image destination is a symbolic link or reparse point.");
                }

                byte[] current = await ReadAsync(path, cancellationToken);
                if (!CryptographicOperations.FixedTimeEquals(SHA256.HashData(current), expectedHash))
                {
                    throw new IOException("The image was changed externally. Save to a new PNG instead.");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (expectedHash is null)
            {
                File.Move(temporaryPath, path);
            }
            else
            {
                File.Replace(temporaryPath, path, null);
            }

            staged = false;
        }
        finally
        {
            if (staged)
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
