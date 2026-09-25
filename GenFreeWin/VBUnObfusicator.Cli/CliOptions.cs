using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;

public sealed class CliOptions
{
    public string? InputPath { get; private set; }
    public string? OutputPath { get; private set; }
    public bool ReadFromStdin { get; private set; }
    public bool ReorderLabels { get; private set; }
    public bool RemoveSingleSourceLabels { get; private set; }
    public int Indent { get; private set; } = 4;
    public bool HasInputSource => ReadFromStdin || !string.IsNullOrWhiteSpace(InputPath);

    public bool TryValidate([NotNullWhen(false)] out string? errorMessage)
    {
        if (!HasInputSource)
        {
            errorMessage = "Specify --input <path> or --stdin.";
            return false;
        }

        if (Indent < 0)
        {
            errorMessage = "--indent requires a non-negative integer value.";
            return false;
        }

        var File = IoC.GetService<IFile>();
        if (File == null)
        {
            errorMessage = "File service is not available.";
            return false;
        }

        if (!ReadFromStdin && (string.IsNullOrWhiteSpace(InputPath) || !File.Exists(InputPath)))
        {
            errorMessage = !string.IsNullOrWhiteSpace(InputPath)
                ? $"Input file '{InputPath}' does not exist."
                : "Input source could not be resolved.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static RootCommand CreateCommand(Func<CliOptions, int> execute)
    {
        var inputOption = new Option<string?>("--input", ["-i"]) { Description = "Read C# source from a file" };
        var outputOption = new Option<string?>("--output", ["-o"]) { Description = "Write normalized output to a file instead of standard output" };
        var stdinOption = new Option<bool>("--stdin") { Description = "Read source from standard input instead of a file" };
        var reorderLabelsOption = new Option<bool>("--reorder-labels") { Description = "Reorder labels using the decompiler normalization pass" };
        var removeSingleSourceLabelsOption = new Option<bool>("--remove-single-source-labels") { Description = "Remove low-confidence single-source labels" };
        var indentOption = new Option<int>("--indent")
        {
            DefaultValueFactory = _ => 4,
            Description = "Indentation width for generated output"
        };
        var command = new RootCommand("Normalize decompiled code with the same CSCode parser used by the WPF tool");

        command.Options.Add(inputOption);
        command.Options.Add(outputOption);
        command.Options.Add(stdinOption);
        command.Options.Add(reorderLabelsOption);
        command.Options.Add(removeSingleSourceLabelsOption);
        command.Options.Add(indentOption);
        command.SetAction((ParseResult result) => execute(new CliOptions
        {
            InputPath = result.GetValue(inputOption),
            OutputPath = result.GetValue(outputOption),
            ReadFromStdin = result.GetValue(stdinOption),
            ReorderLabels = result.GetValue(reorderLabelsOption),
            RemoveSingleSourceLabels = result.GetValue(removeSingleSourceLabelsOption),
            Indent = result.GetValue(indentOption)
        }));

        return command;
    }
}
