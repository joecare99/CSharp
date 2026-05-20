using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using SharpHack.Persist.Models;

namespace SharpHack.Persist;

/// <summary>
/// Writes save payloads as compressed JSON files.
/// </summary>
public sealed class SaveFileWriter
{
    /// <summary>
    /// Writes the specified save payload to disk using a compressed JSON format.
    /// </summary>
    /// <param name="filePath">The target save file path.</param>
    /// <param name="saveGame">The save payload.</param>
    public void Write(string filePath, SaveGameDto saveGame)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A save file path is required.", nameof(filePath));
        }

        if (saveGame == null)
        {
            throw new ArgumentNullException(nameof(saveGame));
        }

        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new DataContractJsonSerializer(typeof(SaveGameDto));
            var tempFilePath = filePath + ".tmp";

            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
            {
                serializer.WriteObject(gzipStream, saveGame);
            }

            ReplaceFile(tempFilePath, filePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new SaveFileAccessException("The save file could not be written because access was denied.", ex);
        }
        catch (IOException ex)
        {
            throw new SaveFileAccessException("The save file could not be written because the storage location was unavailable.", ex);
        }
    }

    private static void ReplaceFile(string tempFilePath, string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        File.Move(tempFilePath, filePath);
    }
}
