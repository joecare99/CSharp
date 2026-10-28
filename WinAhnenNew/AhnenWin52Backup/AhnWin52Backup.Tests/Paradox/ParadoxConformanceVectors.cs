using System;
using System.IO;

namespace AhnWin52Backup.Tests.Paradox;

internal static class ParadoxConformanceVectors
{
    public static string Read(string name)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Conformance", "vectors.txt");
        foreach (string line in File.ReadLines(path))
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
            {
                continue;
            }

            int separator = trimmed.IndexOf('=');
            if (separator > 0 &&
                trimmed[..separator].Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return trimmed[(separator + 1)..];
            }
        }

        throw new InvalidDataException($"DBParadox vector '{name}' was not found in {path}.");
    }

    public static byte[] ReadHex(string name) => Convert.FromHexString(Read(name));
}
