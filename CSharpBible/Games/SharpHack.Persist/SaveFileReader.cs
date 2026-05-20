using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using SharpHack.Persist.Models;

namespace SharpHack.Persist;

/// <summary>
/// Reads compressed JSON save payloads from disk.
/// </summary>
public sealed class SaveFileReader
{
    /// <summary>
    /// Reads the specified compressed save file and returns the deserialized payload.
    /// </summary>
    /// <param name="filePath">The save file path.</param>
    /// <returns>The deserialized save payload.</returns>
    public SaveGameDto Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A save file path is required.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new SaveFileMissingException("The requested save file was not found.");
        }

        try
        {
            var serializer = new DataContractJsonSerializer(typeof(SaveGameDto));
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            {
                var saveGame = serializer.ReadObject(gzipStream) as SaveGameDto;
                if (saveGame == null)
                {
                    throw new SaveFileCorruptException("The save file did not contain a valid save payload.");
                }

                return saveGame;
            }
        }
        catch (SavePersistenceException)
        {
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new SaveFileAccessException("The save file could not be read because access was denied.", ex);
        }
        catch (IOException ex)
        {
            throw new SaveFileAccessException("The save file could not be read because the storage location was unavailable.", ex);
        }
        catch (InvalidDataException ex)
        {
            throw new SaveFileCorruptException("The save file contents are corrupt or incomplete.", ex);
        }
        catch (System.Runtime.Serialization.SerializationException ex)
        {
            throw new SaveFileCorruptException("The save file contents could not be deserialized.", ex);
        }
    }
}
