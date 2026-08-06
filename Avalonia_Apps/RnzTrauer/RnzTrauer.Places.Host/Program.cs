using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RnzTrauer.Places;

namespace RnzTrauer.Places.Host;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            Console.WriteLine("RNZ places host");
            Console.WriteLine("Usage: RnzTrauer.Places.Host --place <name> [--known <file>] [--output <file>]");
            return args.Length == 0 ? 2 : 0;
        }

        var place = GetOption(args, "--place");
        if (place is null)
        {
            Console.Error.WriteLine("Missing required option: --place");
            return 2;
        }

        var knownPath = GetOption(args, "--known");
        var known = knownPath is null
            ? Array.Empty<string>()
            : File.ReadAllLines(knownPath).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        var result = new PlaceNormalizer().Resolve(place, known);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        var output = GetOption(args, "--output");
        if (output is null)
            Console.WriteLine(json);
        else
            File.WriteAllText(output, json);
        return 0;
    }

    private static bool HasOption(string[] args, string name) =>
        args.Any(argument => string.Equals(argument, name, StringComparison.OrdinalIgnoreCase));

    private static string? GetOption(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        return null;
    }
}
