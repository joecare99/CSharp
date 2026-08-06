using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Media;

namespace RnzTrauer.Media.Host;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || HasOption(args, "--help"))
        {
            PrintHelp();
            return args.Length == 0 ? 2 : 0;
        }

        var pdfPath = GetRequiredOption(args, "--pdf");
        var toolPath = GetRequiredOption(args, "--tool");
        var xmlPath = GetRequiredOption(args, "--xml");
        if (pdfPath is null || toolPath is null || xmlPath is null)
            return 2;

        var result = await new PdfXmlExtractionService(
            new SystemExternalProcessRunner(),
            new PdfXmlDocumentParser()).ExtractAsync(
                new PdfXmlExtractionRequest(
                    pdfPath,
                    toolPath,
                    xmlPath,
                    TimeSpan.FromSeconds(ParseIntOption(args, "--timeout-seconds", 120))),
                CancellationToken.None);

        Console.WriteLine(JsonSerializer.Serialize(
            new
            {
                result.PdfPath,
                result.XmlPath,
                result.Text,
                ImageCandidates = result.ImageCandidates.Count,
                result.StandardError,
            },
            new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static int ParseIntOption(string[] args, string name, int defaultValue)
    {
        var value = GetOption(args, name);
        return value is null ? defaultValue : int.Parse(value);
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
        Console.WriteLine("RNZ media host");
        Console.WriteLine("Usage: RnzTrauer.Media.Host --pdf <file> --tool <executable> --xml <file> [--timeout-seconds <n>]");
    }
}
