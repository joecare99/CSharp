using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Testing.Services;

internal static class VSTestAdapterResolver
{
    private const string ManifestFileName = "vstest-adapters.json";
    private const int MaximumManifestBytes = 64 * 1024;
    private const int MaximumConfiguredAdapters = 32;

    public static async Task<string[]> ResolveAsync(
        string assemblyPath,
        string projectDirectory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        assemblyPath = Path.GetFullPath(assemblyPath);
        projectDirectory = Path.GetFullPath(projectDirectory);
        if (!Directory.Exists(projectDirectory))
        {
            throw new DirectoryNotFoundException(projectDirectory);
        }

        cancellationToken.ThrowIfCancellationRequested();
        string outputDirectory = Path.GetDirectoryName(assemblyPath)
            ?? throw new InvalidOperationException("The test assembly has no output directory.");
        List<string> adapters = Directory.Exists(outputDirectory)
            ? Directory.GetFiles(outputDirectory, "*.dll")
                .Where(static path => path.EndsWith("TestAdapter.dll", StringComparison.OrdinalIgnoreCase))
                .ToList()
            : [];
        cancellationToken.ThrowIfCancellationRequested();

        string manifestPath = Path.Combine(projectDirectory, ManifestFileName);
        if (!File.Exists(manifestPath))
        {
            return adapters.ToArray();
        }

        EnsureNoReparsePoints(projectDirectory, manifestPath);
        byte[] manifestBytes;
        await using (FileStream stream = new(
            manifestPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
        {
            if (stream.Length > MaximumManifestBytes)
            {
                throw new InvalidDataException($"The {ManifestFileName} manifest exceeds {MaximumManifestBytes} bytes.");
            }

            manifestBytes = new byte[stream.Length];
            int offset = 0;
            while (offset < manifestBytes.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int read = await stream.ReadAsync(manifestBytes.AsMemory(offset), cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    break;
                }
                offset += read;
            }

            if (offset != manifestBytes.Length)
            {
                Array.Resize(ref manifestBytes, offset);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(manifestBytes);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"The {ManifestFileName} manifest is not valid JSON.", exception);
        }

        using (document)
        {
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || root.EnumerateObject().Count() != 1
                || !root.TryGetProperty("adapterPaths", out JsonElement paths)
                || paths.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidDataException(
                    $"The {ManifestFileName} manifest must contain only an 'adapterPaths' array.");
            }

            if (paths.GetArrayLength() > MaximumConfiguredAdapters)
            {
                throw new InvalidDataException(
                    $"The {ManifestFileName} manifest may configure at most {MaximumConfiguredAdapters} adapters.");
            }

            HashSet<string> knownPaths = new(adapters, StringComparer.OrdinalIgnoreCase);
            foreach (JsonElement entry in paths.EnumerateArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (entry.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidDataException("Every configured VSTest adapter path must be a string.");
                }

                string configuredPath = entry.GetString()!;
                string resolvedPath = ResolveConfiguredPath(projectDirectory, configuredPath);
                EnsureNoReparsePoints(projectDirectory, resolvedPath);
                if (!File.Exists(resolvedPath))
                {
                    throw new FileNotFoundException("A configured VSTest adapter does not exist.", resolvedPath);
                }
                if (!resolvedPath.EndsWith("TestAdapter.dll", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException(
                        $"A configured VSTest adapter must have a 'TestAdapter.dll' file name: {configuredPath}");
                }

                if (knownPaths.Add(resolvedPath))
                {
                    adapters.Add(resolvedPath);
                }
            }
        }

        return adapters.ToArray();
    }

    private static string ResolveConfiguredPath(string projectDirectory, string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath)
            || Path.IsPathRooted(configuredPath)
            || configuredPath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
                .Any(static segment => segment is "" or "." or ".."))
        {
            throw new InvalidDataException(
                "Configured VSTest adapter paths must be non-empty project-relative paths without '.' or '..' segments.");
        }

        string resolvedPath;
        try
        {
            resolvedPath = Path.GetFullPath(Path.Combine(projectDirectory, configuredPath));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new InvalidDataException($"The configured VSTest adapter path '{configuredPath}' is invalid.", exception);
        }

        string relativePath = Path.GetRelativePath(projectDirectory, resolvedPath);
        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relativePath.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidDataException("A configured VSTest adapter path resolves outside the project directory.");
        }

        return resolvedPath;
    }

    private static void EnsureNoReparsePoints(string projectDirectory, string targetPath)
    {
        string relativePath = Path.GetRelativePath(projectDirectory, targetPath);
        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relativePath.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new InvalidDataException("A configured VSTest adapter path resolves outside the project directory.");
        }

        string currentPath = projectDirectory;
        foreach (string segment in relativePath.Split(
            [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar],
            StringSplitOptions.RemoveEmptyEntries))
        {
            currentPath = Path.Combine(currentPath, segment);
            if (!File.Exists(currentPath) && !Directory.Exists(currentPath))
            {
                continue;
            }

            if ((File.GetAttributes(currentPath) & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidDataException("Configured VSTest adapter paths may not traverse symbolic links or junctions.");
            }
        }
    }
}
