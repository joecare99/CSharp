using System.Text;
using TranspilerLib.Models.Scanner;

var options = CliOptions.Parse(args);

if (options.ShowHelp)
{
    Console.WriteLine(CliOptions.UsageText);
    return 0;
}

if (!options.HasInputSource)
{
    Console.Error.WriteLine("No input source specified. Use --input <file> or --stdin.");
    Console.Error.WriteLine(CliOptions.UsageText);
    return 1;
}

string source;
if (options.ReadFromStdin)
{
    source = Console.In.ReadToEnd();
}
else if (!string.IsNullOrWhiteSpace(options.InputPath))
{
    source = File.ReadAllText(options.InputPath);
}
else
{
    Console.Error.WriteLine("Input source could not be resolved.");
    return 1;
}

var engine = new CSCode { OriginalCode = source };
var parsed = engine.Parse();

if (options.ReorderLabels)
    engine.ReorderLabels(parsed);

if (options.RemoveSingleSourceLabels)
    engine.RemoveSingleSourceLabels1(parsed);

var result = engine.ToCode(parsed, options.Indent);

if (!string.IsNullOrWhiteSpace(options.OutputPath))
{
    var dir = Path.GetDirectoryName(options.OutputPath);
    if (!string.IsNullOrEmpty(dir))
        Directory.CreateDirectory(dir);
    File.WriteAllText(options.OutputPath, result, Encoding.UTF8);
}
else
{
    Console.Write(result);
}

return 0;

internal sealed class CliOptions
{
    public string? InputPath { get; private set; }
    public string? OutputPath { get; private set; }
    public bool ShowHelp { get; private set; }
    public bool ReadFromStdin { get; private set; }
    public bool ReorderLabels { get; private set; }
    public bool RemoveSingleSourceLabels { get; private set; }
    public int Indent { get; private set; } = 4;
    public bool HasInputSource => ReadFromStdin || !string.IsNullOrWhiteSpace(InputPath);

    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg is "-h" or "--help")
            {
                options.ShowHelp = true;
                continue;
            }

            if (arg is "--stdin")
            {
                options.ReadFromStdin = true;
                continue;
            }

            if (arg is "--reorder-labels")
            {
                options.ReorderLabels = true;
                continue;
            }

            if (arg is "--remove-single-source-labels")
            {
                options.RemoveSingleSourceLabels = true;
                continue;
            }

            if (arg is "--indent")
            {
                var value = GetNextValue(args, ref i, "--indent");
                if (!int.TryParse(value, out var indent) || indent < 0)
                    throw new ArgumentException("--indent requires a non-negative integer value.");
                options.Indent = indent;
                continue;
            }

            if (arg is "-i" or "--input")
            {
                options.InputPath = GetNextValue(args, ref i, "--input");
                continue;
            }

            if (arg is "-o" or "--output")
            {
                options.OutputPath = GetNextValue(args, ref i, "--output");
                continue;
            }

            if (arg.Contains('='))
            {
                var split = arg.Split('=', 2);
                var key = split[0];
                var value = split[1];
                switch (key)
                {
                    case "--input":
                        options.InputPath = value;
                        break;
                    case "--output":
                        options.OutputPath = value;
                        break;
                    case "--indent":
                        if (!int.TryParse(value, out var indent) || indent < 0)
                            throw new ArgumentException("--indent requires a non-negative integer value.");
                        options.Indent = indent;
                        break;
                    case "--reorder-labels":
                        options.ReorderLabels = bool.TryParse(value, out var reorder) && reorder;
                        break;
                    case "--remove-single-source-labels":
                        options.RemoveSingleSourceLabels = bool.TryParse(value, out var remove) && remove;
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
                continue;
            }

            throw new ArgumentException($"Unknown argument: {arg}");
        }

        return options;
    }

    private static string GetNextValue(string[] args, ref int index, string flagName)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"Missing value for {flagName}.");

        index++;
        return args[index];
    }

    public const string UsageText = "VBUnObfusicator.Cli - normalize decompiled code with the same CSCode parser used by the WPF tool\n\n" +
        "Usage: VBUnObfusicator.Cli --input <path> [--output <path>] [--reorder-labels] [--remove-single-source-labels] [--indent N]\n\n" +
        "Options:\n" +
        "  -i, --input <path>              Read C# source from a file\n" +
        "  -o, --output <path>             Write normalized output to a file. If omitted, writes to stdout\n" +
        "  --stdin                        Read source from stdin instead of a file\n" +
        "  --reorder-labels               Reorder labels using the decompiler normalization pass\n" +
        "  --remove-single-source-labels  Remove low-confidence single-source labels\n" +
        "  --indent N                     Indentation width for generated output (default: 4)\n" +
        "  -h, --help                     Show this message\n";
}
