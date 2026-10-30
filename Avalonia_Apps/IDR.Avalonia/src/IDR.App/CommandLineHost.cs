using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Analysis;
using IDR.Infrastructure.Disassembly;
using IDR.Infrastructure.KnowledgeBase;
using IDR.Infrastructure.Pe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.CommandLine;

namespace IDR.App;

public sealed class CommandLineHost
{
    private readonly IPeImageLoader _imageLoader;
    private readonly IDelphiVersionDetector _versionDetector;
    private readonly IKnowledgeBaseProvider _knowledgeBaseProvider;
    private readonly IInstructionDecoder _instructionDecoder;
    private readonly AnalysisRunner _analysisRunner;

    public CommandLineHost(
        IPeImageLoader imageLoader,
        IDelphiVersionDetector versionDetector,
        IKnowledgeBaseProvider knowledgeBaseProvider,
        IInstructionDecoder instructionDecoder,
        IAnalysisService analysisService)
    {
        _imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
        _versionDetector = versionDetector ?? throw new ArgumentNullException(nameof(versionDetector));
        _knowledgeBaseProvider = knowledgeBaseProvider
            ?? throw new ArgumentNullException(nameof(knowledgeBaseProvider));
        _instructionDecoder = instructionDecoder
            ?? throw new ArgumentNullException(nameof(instructionDecoder));
        _analysisRunner = new AnalysisRunner(
            _imageLoader,
            _versionDetector,
            _knowledgeBaseProvider,
            analysisService ?? throw new ArgumentNullException(nameof(analysisService)));
    }

    public RootCommand CreateRootCommand()
    {
        RootCommand rootCommand = new("Analyze Delphi PE files in batch mode.");
        Command analyzeCommand = new("analyze", "Analyze one x86 Delphi EXE or DLL.");
        Option<string> inputOption = new("--input")
        {
            Aliases = { "-i" },
            Description = "Path to the PE executable or library.",
            Required = true
        };
        Option<string> versionOption = new("--delphi-version")
        {
            Aliases = { "--version" },
            Description = "Delphi version (for example 7, 8, 2007, 2009, 2010, XE1, XE2, XE3, or XE4).",
            Required = true
        };
        Option<string?> knowledgeBaseOption = new("--knowledge-base")
        {
            Description = "Path to a specific kb<version>.bin file."
        };
        Option<string?> knowledgeBaseDirectoryOption = new("--knowledge-base-directory")
        {
            Description = "Directory containing kb<version>.bin files; overrides IDR_KNOWLEDGE_BASE_DIRECTORY."
        };
        Option<string?> outputOption = new("--output")
        {
            Aliases = { "-o" },
            Description = "Write JSON to this file instead of standard output."
        };
        Option<bool> progressOption = new("--progress")
        {
            Description = "Write phase progress to standard error."
        };
        Option<bool> summaryOnlyOption = new("--summary-only")
        {
            Description = "Emit counts and diagnostics without item and disassembly arrays."
        };

        analyzeCommand.Options.Add(inputOption);
        analyzeCommand.Options.Add(versionOption);
        analyzeCommand.Options.Add(knowledgeBaseOption);
        analyzeCommand.Options.Add(knowledgeBaseDirectoryOption);
        analyzeCommand.Options.Add(outputOption);
        analyzeCommand.Options.Add(progressOption);
        analyzeCommand.Options.Add(summaryOnlyOption);
        analyzeCommand.SetAction((parseResult, cancellationToken) => AnalyzeAsync(
            parseResult.GetValue(inputOption)!,
            parseResult.GetValue(versionOption)!,
            parseResult.GetValue(knowledgeBaseOption),
            parseResult.GetValue(knowledgeBaseDirectoryOption),
            parseResult.GetValue(outputOption),
            parseResult.GetValue(progressOption),
            parseResult.GetValue(summaryOnlyOption),
            cancellationToken));
        rootCommand.Subcommands.Add(analyzeCommand);
        return rootCommand;
    }

    public Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);
        return CreateRootCommand().Parse(args).InvokeAsync(null, cancellationToken);
    }

    public static bool TryParseDelphiVersion(string value, out DelphiVersion version)
    {
        ArgumentNullException.ThrowIfNull(value);
        string normalized = value.Trim();
        if (normalized.StartsWith("Delphi", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized["Delphi".Length..];
        }

        if (normalized.StartsWith("XE", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(normalized["XE".Length..], out int xeVersion))
        {
            version = xeVersion switch
            {
                1 => DelphiVersion.DelphiXE1,
                2 => DelphiVersion.DelphiXE2,
                3 => DelphiVersion.DelphiXE3,
                4 => DelphiVersion.DelphiXE4,
                _ => DelphiVersion.Unknown
            };
            return version != DelphiVersion.Unknown;
        }

        if (int.TryParse(normalized, out int numericVersion)
            && Enum.IsDefined(typeof(DelphiVersion), numericVersion)
            && numericVersion != (int)DelphiVersion.Unknown)
        {
            version = (DelphiVersion)numericVersion;
            return true;
        }

        version = DelphiVersion.Unknown;
        return false;
    }

    private async Task<int> AnalyzeAsync(
        string inputPath,
        string versionText,
        string? knowledgeBasePath,
        string? knowledgeBaseDirectory,
        string? outputPath,
        bool showProgress,
        bool summaryOnly,
        CancellationToken cancellationToken)
    {
        if (!TryParseDelphiVersion(versionText, out DelphiVersion version))
        {
            Console.Error.WriteLine($"Unsupported Delphi version '{versionText}'.");
            return 2;
        }

        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine($"Input PE file does not exist: {inputPath}");
            return 1;
        }

        try
        {
            IProgress<AnalysisProgress>? progress = showProgress
                ? new ConsoleAnalysisProgress()
                : null;
            AnalysisExecution execution = await _analysisRunner.AnalyzeAsync(
                inputPath,
                version,
                knowledgeBasePath,
                knowledgeBaseDirectory,
                progress,
                cancellationToken).ConfigureAwait(false);
            await WriteOutputAsync(
                CommandLineAnalysisReport.Create(
                    execution.Session,
                    execution.KnowledgeBase,
                    execution.KnowledgeBasePath,
                    execution.Detection,
                    execution.Result,
                    execution.Forms,
                    execution.FormDiagnostics,
                    summaryOnly),
                outputPath,
                cancellationToken).ConfigureAwait(false);
            return 0;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Console.Error.WriteLine("Analysis canceled.");
            return 130;
        }
        catch (ArgumentException exception) when (
            exception.Message.StartsWith("Specify either a Knowledge Base", StringComparison.Ordinal))
        {
            Console.Error.WriteLine(exception.Message);
            return 2;
        }
        catch (Exception exception) when (
            exception is IOException
                or UnauthorizedAccessException
                or ArgumentException
                or InvalidDataException
                or InstructionDecodeException)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private sealed class ConsoleAnalysisProgress : IProgress<AnalysisProgress>
    {
        public void Report(AnalysisProgress value)
        {
            Console.Error.WriteLine($"{value.Phase}\t{value.Completed}\t{value.Total}");
        }
    }

    private static async Task WriteOutputAsync(
        CommandLineAnalysisOutput output,
        string? outputPath,
        CancellationToken cancellationToken)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };
        if (outputPath is null)
        {
            await JsonSerializer.SerializeAsync(
                Console.OpenStandardOutput(),
                output,
                options,
                cancellationToken).ConfigureAwait(false);
            await Console.Out.WriteLineAsync().ConfigureAwait(false);
            return;
        }

        await using FileStream stream = new(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);
        await JsonSerializer.SerializeAsync(stream, output, options, cancellationToken)
            .ConfigureAwait(false);
    }

}
