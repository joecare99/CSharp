using System;
using System.IO;
using System.Text.Json;
using RnzTrauer.Import;

namespace RnzTrauer.Import.Host;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            PrintHelp();
            return args.Length == 0 ? 2 : 0;
        }

        var htmlPath = GetRequiredOption(args, "--html");
        var schemaPath = GetRequiredOption(args, "--schema");
        var outputPath = GetOption(args, "--output");
        if (htmlPath is null || schemaPath is null)
            return 2;

        var pipeline = ImportPipelineFactory.CreateDefault();
        var report = pipeline.Import(File.ReadAllBytes(htmlPath), File.ReadAllLines(schemaPath));
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });

        if (outputPath is null)
            Console.WriteLine(json);
        else
            File.WriteAllText(outputPath, json);

        return 0;
    }

    private static bool HasOption(string[] args, string name)
    {
        foreach (var argument in args)
            if (string.Equals(argument, name, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    private static string? GetRequiredOption(string[] args, string name)
    {
        var value = GetOption(args, name);
        if (value is null)
            Console.Error.WriteLine($"Missing required option: {name}");
        return value;
    }

    private static string? GetOption(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
            if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                return args[index + 1];
        return null;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("RNZ import host");
        Console.WriteLine("Usage: RnzTrauer.Import.Host --html <file> --schema <file> [--output <file>]");
    }
}
