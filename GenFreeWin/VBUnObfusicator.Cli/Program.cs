using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models;
using BaseLib.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Text.Json;
#if NET5_0_OR_GREATER
using TranspilerLib.CSharp.StatEqualCheck;
#endif
using TranspilerLib.CSharp.VBLegacyReplace;
using TranspilerLib.Data;
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
#if NET5_0_OR_GREATER
            .AddSingleton<Func<ICodeBlock, ICodeBlock, System.Collections.Generic.IEnumerable<ICodeBlock>?, System.Collections.Generic.IEnumerable<ICodeBlock>?, StatEqualResult>>(_ => StatEqualCheck.Compare)
#endif
            .AddTransient<LegacyReplacementEngine>(_ => LegacyReplacementEngine.LoadDefaultRules())
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
#if NET5_0_OR_GREATER
        var compare = IoC.GetRequiredService<Func<ICodeBlock, ICodeBlock, System.Collections.Generic.IEnumerable<ICodeBlock>?, System.Collections.Generic.IEnumerable<ICodeBlock>?, StatEqualResult>>();
#endif
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

        var optimizedCode = engine.ToCode(parsed, options.Indent);
#if NET5_0_OR_GREATER
        StatEqualResult? equivalence = null;
        if (options.CheckEquivalence || !string.IsNullOrWhiteSpace(options.CompareAgainstPath))
        {
            var comparisonSource = string.IsNullOrWhiteSpace(options.CompareAgainstPath)
                ? optimizedCode
                : File.ReadAllText(options.CompareAgainstPath);
            var originalBlock = new CodeBlock { Name = "Input", Type = CodeBlockType.MainBlock, Code = codeSource };
            var candidateBlock = new CodeBlock { Name = "Comparison", Type = CodeBlockType.MainBlock, Code = comparisonSource };
            equivalence = compare(originalBlock, candidateBlock, null, null);
            WriteEquivalenceFindings(console, equivalence);
            if (!string.IsNullOrWhiteSpace(options.EquivalenceJsonPath))
            {
                var jsonPath = options.EquivalenceJsonPath!;
                var jsonDirectory = Path.GetDirectoryName(jsonPath);
                if (!string.IsNullOrEmpty(jsonDirectory))
                    Directory.CreateDirectory(jsonDirectory);
                var report = new
                {
                    status = equivalence.Status.ToString(),
                    isEquivalent = equivalence.IsEquivalent,
                    findings = equivalence.Findings.Select(finding => new
                    {
                        severity = finding.Severity.ToString(),
                        code = finding.Code,
                        message = finding.Message,
                        leftSpan = finding.LeftSpan,
                        rightSpan = finding.RightSpan
                    })
                };
                File.WriteAllText(jsonPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }), Encoding.UTF8);
            }
        }
#endif
        var result = optimizedCode;
        if (options.ReplaceVbLegacy)
        {
            var legacyEngine = string.IsNullOrWhiteSpace(options.LegacyRulesPath)
                ? IoC.GetRequiredService<LegacyReplacementEngine>()
                : LegacyReplacementEngine.LoadFile(options.LegacyRulesPath);
            var replacement = legacyEngine.Apply(result);
            result = AddRequiredUsings(replacement.Source, replacement.RequiredUsings);
            foreach (var diagnostic in replacement.Diagnostics.Where(diagnostic => diagnostic.Severity != TranspilerLib.CSharp.VBLegacyReplace.Models.RuleDiagnosticSeverity.Information))
                console.Error.WriteLine($"Legacy replacement [{diagnostic.Severity}] {diagnostic.Kind}: {diagnostic.Message}");
        }

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
#if NET5_0_OR_GREATER
        return options.FailOnMismatch && equivalence?.Status == StatEqualStatus.NotEquivalent ? 2 : 0;
#else
        return 0;
#endif
    }

#if NET5_0_OR_GREATER
    private static void WriteEquivalenceFindings(IConsole console, StatEqualResult result)
    {
        console.Error.WriteLine($"Equivalence check: {result.Status}");
        foreach (var finding in result.Findings)
        {
            var leftLocation = finding.LeftSpan is { } left ? $" original {left.Line}:{left.Column}" : string.Empty;
            var rightLocation = finding.RightSpan is { } right ? $" output {right.Line}:{right.Column}" : string.Empty;
            console.Error.WriteLine($"[{finding.Severity}] {finding.Code}:{leftLocation}{rightLocation} {finding.Message}");
        }
    }
#endif

    private static string AddRequiredUsings(string source, System.Collections.Generic.IReadOnlyList<string> requiredUsings)
    {
        if (requiredUsings.Count == 0)
            return source;

        var missingUsings = requiredUsings
            .Where(requiredUsing => !source.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Any(line => string.Equals(line.Trim(), $"using {requiredUsing};", StringComparison.Ordinal)))
            .Select(requiredUsing => $"using {requiredUsing};")
            .ToArray();
        if (missingUsings.Length == 0)
            return source;

        var (documentation, code) = SplitLeadingDocumentation(source);
        var prefix = string.Join(Environment.NewLine, missingUsings);
        return string.IsNullOrEmpty(documentation)
            ? $"{prefix}{Environment.NewLine}{code}"
            : $"{documentation}{Environment.NewLine}{prefix}{Environment.NewLine}{code}";
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

#if NET5_0_OR_GREATER
        var code = string.Join(Environment.NewLine, lines[documentationLines.Length..]).TrimStart();
#else
        var code = string.Join(Environment.NewLine, lines.GetSubArray(documentationLines.Length, lines.Length - documentationLines.Length)).TrimStart();
#endif
        return (string.Join(Environment.NewLine, documentationLines), code);
    }
}
