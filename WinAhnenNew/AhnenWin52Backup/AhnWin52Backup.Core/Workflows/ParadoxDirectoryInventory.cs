using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

public sealed record ParadoxTableInventory(string FileName, ParadoxTableSchema Schema,
    bool HasPrimaryIndex, bool HasMemoFile, IReadOnlyList<int> SecondaryIndexes);

/// <summary>Inventories table families without reading any record or memo bytes.</summary>
public sealed class ParadoxDirectoryInventory
{
    private readonly ParadoxTableReader _reader = new();

    public IReadOnlyList<ParadoxTableInventory> Read(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        string[] files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
        Dictionary<string, string> names = new(StringComparer.OrdinalIgnoreCase);
        foreach (string file in files)
        {
            if (!names.TryAdd(Path.GetFileName(file), file))
            {
                throw new InvalidDataException("Ambiguous case-insensitive filename in the Paradox directory.");
            }
        }

        List<ParadoxTableInventory> tables = new();
        foreach (string file in files.Where(path => Path.GetExtension(path).Equals(".DB", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            string stem = Path.GetFileNameWithoutExtension(file);
            if (!names.ContainsKey($"{stem}.PX"))
            {
                throw new InvalidDataException($"Missing primary index for {stem}.");
            }

            List<int> secondary = new();
            for (int number = 0; number < 16; number++)
            {
                bool x = names.ContainsKey($"{stem}.XG{number:X}");
                bool y = names.ContainsKey($"{stem}.YG{number:X}");
                if (x != y)
                {
                    throw new InvalidDataException($"Unpaired secondary index for {stem}, number {number:X}.");
                }

                if (x)
                {
                    secondary.Add(number);
                }
            }

            tables.Add(new ParadoxTableInventory(Path.GetFileName(file), _reader.ReadSchema(file),
                names.ContainsKey($"{stem}.PX"), names.ContainsKey($"{stem}.MB"), secondary));
        }

        HashSet<string> tableNames = tables.Select(table => Path.GetFileNameWithoutExtension(table.FileName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string file in files)
        {
            string extension = Path.GetExtension(file);
            bool sidecar = extension.Equals(".PX", StringComparison.OrdinalIgnoreCase) ||
                           extension.Equals(".MB", StringComparison.OrdinalIgnoreCase) ||
                           (extension.Length == 4 &&
                            (extension.StartsWith(".XG", StringComparison.OrdinalIgnoreCase) ||
                             extension.StartsWith(".YG", StringComparison.OrdinalIgnoreCase)) &&
                            Uri.IsHexDigit(extension[3]));
            if (sidecar && !tableNames.Contains(Path.GetFileNameWithoutExtension(file)))
            {
                throw new InvalidDataException($"Orphan Paradox sidecar: {Path.GetFileName(file)}.");
            }
        }

        return tables;
    }
}
