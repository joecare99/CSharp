using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RnzTrauer.Core.Domain;
using RnzTrauer.Core.Export;
using RnzTrauer.Core.Services;

namespace RnzTrauer.Export.Host;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            PrintHelp();
            return args.Length == 0 ? 2 : 0;
        }

        var input = GetOption(args, "--input");
        var output = GetOption(args, "--output");
        var format = GetOption(args, "--format");
        if (input is null || output is null || format is null)
        {
            Console.Error.WriteLine("--input, --output, and --format are required.");
            return 2;
        }

        if (!string.Equals(format, "tsv", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(format, "gedcom", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("--format must be tsv or gedcom.");
            return 2;
        }

        if (File.Exists(output) && !HasOption(args, "--overwrite"))
        {
            Console.Error.WriteLine("Output exists; use --overwrite to replace it.");
            return 2;
        }

        List<DeathNotice> notices;
        try
        {
            await using var stream = File.OpenRead(input);
            notices = await JsonSerializer.DeserializeAsync<List<DeathNotice>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Input JSON did not contain a notice array.");
        }
        catch (JsonException exception)
        {
            Console.Error.WriteLine($"Input JSON is invalid: {exception.Message}");
            return 2;
        }
        catch (InvalidDataException exception)
        {
            Console.Error.WriteLine($"Input JSON is invalid: {exception.Message}");
            return 2;
        }
        catch (IOException exception)
        {
            Console.Error.WriteLine($"Input file could not be read: {exception.Message}");
            return 2;
        }
        IExportService exporter = new NoticeExportService();
        if (string.Equals(format, "tsv", StringComparison.OrdinalIgnoreCase))
            await exporter.ExportCsvAsync(output, notices);
        else
            await exporter.ExportGedcomAsync(output, notices);

        Console.WriteLine(JsonSerializer.Serialize(new
        {
            Input = input,
            Output = output,
            Format = format.ToLowerInvariant(),
            NoticeCount = notices.Count,
        }));
        return 0;
    }

    private static bool HasOption(string[] args, string name)
    {
        foreach (var argument in args)
            if (string.Equals(argument, name, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
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
        Console.WriteLine("RNZ export host");
        Console.WriteLine("Usage: RnzTrauer.Export.Host --input <notices.json> --format <tsv|gedcom> --output <file> [--overwrite]");
    }
}
