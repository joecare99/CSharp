using System;
using System.IO;
using System.Security;

namespace Config.Service;

/// <summary>Describes whether the current process can currently write a layer store.</summary>
public sealed record ConfigLayerStoreAccessResult(bool CanWrite, string? ReadOnlyReason);

/// <summary>
/// Probes effective write access to an existing layer directory without creating directories
/// or asserting that a store is trusted as an enforced policy.
/// </summary>
public static class ConfigLayerStoreAccessEvaluator
{
    /// <summary>
    /// Checks the current process's ability to create a temporary sibling file.
    /// This is a point-in-time capability check; the actual store write remains authoritative.
    /// </summary>
    public static ConfigLayerStoreAccessResult Evaluate(IConfigLayerStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        if (store is not JsonConfigLayerStore jsonStore)
        {
            return new(false, "Write access cannot be verified for this configuration store.");
        }

        string path = jsonStore.FilePath;
        try
        {
            if (File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.ReparsePoint))
            {
                return new(false, "The configuration file is a symbolic link or reparse point.");
            }

            if (File.Exists(path)
                && OperatingSystem.IsWindows()
                && File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                return new(false, "The configuration file is marked read-only.");
            }

            string directory = Path.GetDirectoryName(path)
                ?? throw new InvalidOperationException("The configuration layer path has no parent directory.");
            if (!Directory.Exists(directory))
            {
                return new(false, "The configuration directory has not been provisioned; it will not be created automatically.");
            }

            if (ContainsReparsePoint(path))
            {
                return new(false, "The configuration path contains a symbolic link or reparse point.");
            }

            string probePath = Path.Combine(
                directory,
                $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.write-probe");
            using FileStream probe = new(
                probePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 1,
                FileOptions.DeleteOnClose);
            return new(true, null);
        }
        catch (UnauthorizedAccessException)
        {
            return new(false, "The current process does not have permission to write this configuration layer.");
        }
        catch (SecurityException)
        {
            return new(false, "The operating system denied access to this configuration layer.");
        }
        catch (IOException)
        {
            return new(false, "The configuration layer is not currently writable.");
        }
    }

    private static bool ContainsReparsePoint(string path)
    {
        string? current = Path.GetPathRoot(path);
        if (string.IsNullOrEmpty(current))
        {
            return true;
        }

        foreach (string segment in path[current.Length..]
                     .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries))
        {
            current = Path.Combine(current, segment);
            if (!File.Exists(current) && !Directory.Exists(current))
            {
                continue;
            }

            if (File.GetAttributes(current).HasFlag(FileAttributes.ReparsePoint))
            {
                return true;
            }
        }

        return false;
    }
}
