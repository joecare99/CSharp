using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.Analysis;
using IDR.Infrastructure.Disassembly;
using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Cli;

public sealed class CommandLineHost
{
    private readonly AnalysisRunner _analysisRunner;
    private readonly PascalClassEmitter _classEmitter;
    private readonly DisassemblyEmitter _disassemblyEmitter;

    public CommandLineHost(
        AnalysisRunner analysisRunner,
        PascalClassEmitter? classEmitter = null,
        DisassemblyEmitter? disassemblyEmitter = null)
    {
        _analysisRunner = analysisRunner ?? throw new ArgumentNullException(nameof(analysisRunner));
        _classEmitter = classEmitter ?? new PascalClassEmitter();
        _disassemblyEmitter = disassemblyEmitter ?? new DisassemblyEmitter();
    }

    public RootCommand CreateRootCommand()
    {
        RootCommand rootCommand = new("Analyze Delphi PE files and emit reverse-engineering artifacts.");
        Command decompileCommand = new(
            "decompile",
            "Analyze one x86 Delphi EXE or DLL and emit JSON, Pascal class declarations, and disassembly.");
        Option<string> inputOption = new("--input")
        {
            Aliases = { "-i" },
            Description = "Path to the PE executable or library.",
            Required = true
        };
        Option<string> versionOption = new("--delphi-version")
        {
            Aliases = { "--version" },
            Description = "Delphi version (for example 7, 2007, 2010, XE2, or XE4).",
            Required = true
        };
        Option<string?> outputDirectoryOption = new("--output-directory")
        {
            Aliases = { "-o" },
            Description = "Directory for analysis.json, classes.pas, and disassembly.asm.",
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
        Option<bool> progressOption = new("--progress")
        {
            Description = "Write phase progress to standard error."
        };
        Option<bool> forceOption = new("--force")
        {
            Description = "Replace an existing output directory."
        };
        Option<bool> summaryOnlyOption = new("--summary-only")
        {
            Description = "Write counts and diagnostics in analysis.json without item and disassembly arrays."
        };

        decompileCommand.Options.Add(inputOption);
        decompileCommand.Options.Add(versionOption);
        decompileCommand.Options.Add(outputDirectoryOption);
        decompileCommand.Options.Add(knowledgeBaseOption);
        decompileCommand.Options.Add(knowledgeBaseDirectoryOption);
        decompileCommand.Options.Add(progressOption);
        decompileCommand.Options.Add(forceOption);
        decompileCommand.Options.Add(summaryOnlyOption);
        decompileCommand.SetAction((parseResult, cancellationToken) => DecompileAsync(
            parseResult.GetValue(inputOption)!,
            parseResult.GetValue(versionOption)!,
            parseResult.GetValue(outputDirectoryOption)!,
            parseResult.GetValue(knowledgeBaseOption),
            parseResult.GetValue(knowledgeBaseDirectoryOption),
            parseResult.GetValue(progressOption),
            parseResult.GetValue(forceOption),
            parseResult.GetValue(summaryOnlyOption),
            cancellationToken));
        rootCommand.Subcommands.Add(decompileCommand);
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

    private async Task<int> DecompileAsync(
        string inputPath,
        string versionText,
        string outputDirectory,
        string? knowledgeBasePath,
        string? knowledgeBaseDirectory,
        bool showProgress,
        bool force,
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

        if (knowledgeBasePath is not null && knowledgeBaseDirectory is not null)
        {
            Console.Error.WriteLine(
                "Specify either --knowledge-base or --knowledge-base-directory, not both.");
            return 2;
        }

        string targetDirectory;
        try
        {
            targetDirectory = Path.GetFullPath(outputDirectory);
            ValidateOutputDirectory(targetDirectory, force);
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            Console.Error.WriteLine(exception.Message);
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
            CommandLineAnalysisOutput report = CommandLineAnalysisReport.Create(
                execution.Session,
                execution.KnowledgeBase,
                execution.KnowledgeBasePath,
                execution.Detection,
                execution.Result,
                execution.Forms,
                execution.FormDiagnostics,
                summaryOnly);
            await WriteOutputDirectoryAsync(
                targetDirectory,
                force,
                report,
                _classEmitter.Emit(execution.Session),
                _disassemblyEmitter.Emit(execution.Session),
                cancellationToken).ConfigureAwait(false);
            Console.Out.WriteLine($"Analysis artifacts written to {targetDirectory}");
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

    private static void ValidateOutputDirectory(string path, bool force)
    {
        if (File.Exists(path))
        {
            throw new IOException($"Output path is a file: {path}");
        }

        if (Directory.Exists(path)
            && !force
            && Directory.EnumerateFileSystemEntries(path).Any())
        {
            throw new IOException(
                $"Output directory is not empty. Use --force to replace it: {path}");
        }
    }

    private static async Task WriteOutputDirectoryAsync(
        string targetDirectory,
        bool force,
        CommandLineAnalysisOutput report,
        string pascalClasses,
        string disassembly,
        CancellationToken cancellationToken)
    {
        string parentDirectory = Path.GetDirectoryName(targetDirectory)
            ?? throw new IOException("The output directory must have a parent directory.");
        Directory.CreateDirectory(parentDirectory);

        string stageDirectory = Path.Combine(
            parentDirectory,
            $".idr-stage-{Guid.NewGuid():N}");
        string backupDirectory = Path.Combine(
            parentDirectory,
            $".idr-backup-{Guid.NewGuid():N}");
        bool movedExistingDirectory = false;
        try
        {
            Directory.CreateDirectory(stageDirectory);
            await using (FileStream jsonStream = new(
                Path.Combine(stageDirectory, "analysis.json"),
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                81920,
                FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(
                    jsonStream,
                    report,
                    new JsonSerializerOptions { WriteIndented = true },
                    cancellationToken).ConfigureAwait(false);
            }

            await File.WriteAllTextAsync(
                Path.Combine(stageDirectory, "classes.pas"),
                pascalClasses,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                cancellationToken).ConfigureAwait(false);
            await File.WriteAllTextAsync(
                Path.Combine(stageDirectory, "disassembly.asm"),
                disassembly,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            if (Directory.Exists(targetDirectory))
            {
                if (!force && Directory.EnumerateFileSystemEntries(targetDirectory).Any())
                {
                    throw new IOException(
                        $"Output directory became non-empty during analysis: {targetDirectory}");
                }

                Directory.Move(targetDirectory, backupDirectory);
                movedExistingDirectory = true;
            }

            try
            {
                Directory.Move(stageDirectory, targetDirectory);
            }
            catch
            {
                if (movedExistingDirectory && !Directory.Exists(targetDirectory))
                {
                    Directory.Move(backupDirectory, targetDirectory);
                    movedExistingDirectory = false;
                }

                throw;
            }

            if (movedExistingDirectory)
            {
                Directory.Delete(backupDirectory, recursive: true);
                movedExistingDirectory = false;
            }
        }
        finally
        {
            if (Directory.Exists(stageDirectory))
            {
                Directory.Delete(stageDirectory, recursive: true);
            }
        }
    }

    private sealed class ConsoleAnalysisProgress : IProgress<AnalysisProgress>
    {
        public void Report(AnalysisProgress value) =>
            Console.Error.WriteLine($"{value.Phase}\t{value.Completed}\t{value.Total}");
    }
}
