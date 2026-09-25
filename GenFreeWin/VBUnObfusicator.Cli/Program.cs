using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.Linq;
using System.Text;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

internal class Program
{
    public static Func<CliOptions, int> DoRun { get; set; } = Run;

    public static int Main(string[] args)
    {
        var parseResult = Init(args);
        return parseResult.Invoke();
    }

    public static ParseResult Init(string[] args)
    {
        var sc = new ServiceCollection()
            .AddSingleton<IConsole, ConsoleProxy>()
            .AddSingleton<IFile, FileProxy>()
            .AddSingleton<IDirectory, DirectoryProxy>()
            .AddTransient<IPath, PathProxy>()
            .AddTransient<ICodeOptimizer, CodeOptimizer>()
            .AddTransient<ITokenHandler>(sp => new CSTokenHandler()
            {
                stringEndChars = CSCode.stringEndChars,
                reservedWords = CSCode.ReservedWords
            })
            .AddTransient<ICodeBuilder, CSCodeBuilder>()
            .AddSingleton<ICSCode, CSCode>();

        var serviceProvider = sc.BuildServiceProvider();
        IoC.Configure(serviceProvider);

        return CliOptions.CreateCommand(DoRun).Parse(args);
    }

    public static int Run(CliOptions options)
    {
        IConsole console = IoC.GetRequiredService<IConsole>();
        IFile File = IoC.GetRequiredService<IFile>();
        IDirectory Directory = IoC.GetRequiredService<IDirectory>();
        IPath Path = IoC.GetRequiredService<IPath>();
        var engine = IoC.GetRequiredService<ICSCode>();

        if (!options.TryValidate(out var validationError))
        {
            console.Error.WriteLine(validationError);
            return 1;
        }

        var source = options.ReadFromStdin
            ? console.In.ReadToEnd()
            : File.ReadAllText(options.InputPath!);
        var (leadingDocumentation, codeSource) = SplitLeadingDocumentation(source);

        engine.OriginalCode = codeSource;
        var parsed = engine.Parse();

        if (options.ReorderLabels)
            engine.ReorderLabels(parsed);

        if (options.RemoveSingleSourceLabels)
            engine.RemoveSingleSourceLabels1(parsed);

        var result = engine.ToCode(parsed, options.Indent);
        if (!string.IsNullOrEmpty(leadingDocumentation))
            result = $"{leadingDocumentation}{Environment.NewLine}{result}";

        if (!string.IsNullOrWhiteSpace(options.OutputPath))
        {
            var dir = Path.GetDirectoryName(options.OutputPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(options.OutputPath, result, Encoding.UTF8);
        }
        else
        {
            console.Write(result);
        }
        return 0;
    }

    private static (string Documentation, string Code) SplitLeadingDocumentation(string source)
    {
        var normalized = source.TrimStart('\uFEFF').Replace("\r\n", "\n");
        var lines = normalized.Split('\n');
        var documentationLines = lines
            .TakeWhile(line => line.TrimStart().StartsWith("///", StringComparison.Ordinal))
            .ToArray();

        if (documentationLines.Length == 0)
            return (string.Empty, source);

        var code = string.Join(Environment.NewLine, lines[documentationLines.Length..]).TrimStart();
        return (string.Join(Environment.NewLine, documentationLines), code);
    }
}