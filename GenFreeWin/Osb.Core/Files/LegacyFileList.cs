using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Osb.Core.Files;

public static class LegacyFileList
{
    public static IReadOnlyList<string> EnumerateNames(string directoryPath, string pattern)
    {
        if (directoryPath is null)
        {
            throw new ArgumentNullException(nameof(directoryPath));
        }

        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        if (!Directory.Exists(directoryPath))
        {
            return Array.Empty<string>();
        }

        return Directory
            .EnumerateFiles(directoryPath, pattern, SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(name => name is not null)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray()!;
    }
}
