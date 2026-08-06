using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RnzTrauer.Acquisition;

namespace RnzTrauer.Acquisition.Host;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            PrintHelp();
            return args.Length == 0 ? 2 : 0;
        }

        var sourceValue = GetRequiredOption(args, "--source");
        if (sourceValue is null)
            return 2;

        var source = ToSourceUri(sourceValue);
        var archivePath = GetOption(args, "--archive");
        var outputPath = GetOption(args, "--output");
        var maxBytes = ParseMaxBytes(GetOption(args, "--max-bytes"));
        using var httpClient = new HttpClient();
        var service = new HtmlAcquisitionService(httpClient);
        var result = await service.AcquireAsync(new HtmlAcquisitionRequest(
            source,
            archivePath,
            maxBytes));

        if (outputPath is not null)
            await File.WriteAllBytesAsync(outputPath, result.Content);

        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                result.Source,
                Length = result.Content.Length,
                result.MediaType,
                result.ArchivedPath,
                OutputPath = outputPath,
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static Uri ToSourceUri(string value)
    {
        if (File.Exists(value))
            return new Uri(Path.GetFullPath(value));
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return uri;
        throw new ArgumentException($"Source is not a file path or absolute URI: {value}");
    }

    private static long ParseMaxBytes(string? value)
    {
        return value is null
            ? 10 * 1024 * 1024
            : long.Parse(value);
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
        Console.WriteLine("RNZ acquisition host");
        Console.WriteLine("Usage: RnzTrauer.Acquisition.Host --source <file-or-uri> [--archive <file>] [--output <file>] [--max-bytes <n>]");
    }
}
