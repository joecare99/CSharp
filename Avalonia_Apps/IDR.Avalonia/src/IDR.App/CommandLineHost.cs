using IDR.Core.Models;
using IDR.Core.Services;
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
    private readonly IAnalysisService _analysisService;

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
        _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
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
            Description = "Delphi version (for example 7, 2007, 2009, 2010, XE1, XE2, XE3, or XE4).",
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

        if (knowledgeBasePath is not null && knowledgeBaseDirectory is not null)
        {
            Console.Error.WriteLine(
                "Specify either --knowledge-base or --knowledge-base-directory, not both.");
            return 2;
        }

        string resolvedKnowledgeBasePath = ResolveKnowledgeBasePath(
            version,
            knowledgeBasePath,
            knowledgeBaseDirectory);
        try
        {
            PeImage image = await _imageLoader.LoadAsync(inputPath, cancellationToken)
                .ConfigureAwait(false);
            DelphiVersionDetection detection = _versionDetector.Detect(image);
            IKnowledgeBase knowledgeBase = await _knowledgeBaseProvider
                .OpenAsync(resolvedKnowledgeBasePath, cancellationToken)
                .ConfigureAwait(false);
            AnalysisSession session = new(
                Path.GetFullPath(inputPath),
                image.Image,
                image.Sections,
                image.ImageBase,
                version)
            {
                EntryPointRva = image.EntryPointRva
            };
            session.LoadPeDirectories(image.Imports, image.Exports, image.ResourceStrings);
            session.LoadKnowledgeBase(knowledgeBase);

            IProgress<AnalysisProgress>? progress = showProgress
                ? new ConsoleAnalysisProgress()
                : null;
            AnalysisResult result = await _analysisService
                .AnalyzeAsync(session, progress, cancellationToken)
                .ConfigureAwait(false);
            await WriteOutputAsync(
                CreateOutput(
                    session,
                    knowledgeBase,
                    resolvedKnowledgeBasePath,
                    detection,
                    result,
                    image.Forms,
                    image.FormDiagnostics,
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

    private static string ResolveKnowledgeBasePath(
        DelphiVersion version,
        string? knowledgeBasePath,
        string? knowledgeBaseDirectory)
    {
        if (knowledgeBasePath is not null)
        {
            return Path.GetFullPath(knowledgeBasePath);
        }

        string directory = knowledgeBaseDirectory
            ?? Environment.GetEnvironmentVariable("IDR_KNOWLEDGE_BASE_DIRECTORY")
            ?? AppContext.BaseDirectory;
        return new KnowledgeBasePathResolver(directory).ResolvePath(version);
    }

    private static CommandLineAnalysisOutput CreateOutput(
        AnalysisSession session,
        IKnowledgeBase knowledgeBase,
        string knowledgeBasePath,
        DelphiVersionDetection detection,
        AnalysisResult result,
        IReadOnlyList<DelphiForm> forms,
        IReadOnlyList<string> formDiagnostics,
        bool summaryOnly)
    {
        AnalysisItem[] items = session.Items.Values.OrderBy(item => item.Address).ToArray();
        return new CommandLineAnalysisOutput(
            1,
            session.SourcePath,
            session.ImageBase,
            session.EntryPointRva,
            session.SelectedDelphiVersion.ToString(),
            detection.Candidates.Select(candidate => candidate.ToString()).ToArray(),
            knowledgeBasePath,
            knowledgeBase.Modules.Count,
            knowledgeBase.Procedures.Count,
            session.SystemModuleId,
            items.Length,
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.Vmt)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.Rtti)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.String)),
            items.Count(item => item.Flags.HasFlag(AnalysisFlags.ProcedureStart)),
            session.DisassemblyLines.Count,
            forms.Count,
            summaryOnly ? [] : forms.ToArray(),
            summaryOnly ? [] : items.Select(item => new CommandLineAnalysisItem(
                item.Address,
                $"0x{item.Address:X8}",
                $"0x{session.ImageBase + item.Address:X8}",
                item.Flags.ToString(),
                item.Name,
                item.TypeKind?.ToString(),
                item.RecordSizeBytes,
                item.ClassInstanceSizeBytes,
                item.ParentAddress,
                item.ExceptionRegionHandlerAddress,
                item.FinallyRegionEndAddress,
                item.Int64ComparisonEndAddress,
                item.ProcedureSizeBytes,
                item.ReturnStackBytes,
                item.ReturnTypeCandidate,
                item.DataTypeCandidate,
                item.ResourceStringCandidate,
                item.ThreadVariableCandidate,
                item.UsesFramePointer,
                item.StackPointerDeltaBytes,
                item.StackArguments.ToArray(),
                item.StackLocalVariables.ToArray(),
                item.RegisterArgumentCandidates.ToArray(),
                item.CrossReferences.Select(reference => new CommandLineCrossReference(
                    reference.TargetAddress,
                    $"0x{reference.TargetAddress:X8}",
                    reference.Kind.ToString(),
                    reference.TargetName)).ToArray(),
                session.GetIncomingCrossReferences(item.Address).Select(reference =>
                    new CommandLineCrossReference(
                        reference.SourceAddress,
                        $"0x{reference.SourceAddress:X8}",
                        reference.Kind.ToString(),
                        reference.TargetName)).ToArray(),
                item.ExceptionHandlers.ToArray(),
                item.Methods.ToArray(),
                item.Fields.ToArray(),
                item.RecordFields.ToArray(),
                item.MemberAccessCandidates.ToArray(),
                item.Interfaces.ToArray(),
                item.VirtualMethods.ToArray())).ToArray(),
            summaryOnly ? [] : session.DisassemblyLines.Select(line => new CommandLineDisassemblyLine(
                line.Address,
                $"0x{line.Address:X8}",
                line.Bytes,
                line.Mnemonic,
                line.FormattedText,
                line.BranchTarget,
                line.FlowControl.ToString())).ToArray(),
            formDiagnostics.Concat(result.Diagnostics).ToArray());
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

    private sealed record CommandLineAnalysisOutput(
        int SchemaVersion,
        string Input,
        ulong ImageBase,
        uint EntryPointRva,
        string DelphiVersion,
        string[] DetectedVersionCandidates,
        string KnowledgeBase,
        int KnowledgeBaseModuleCount,
        int KnowledgeBaseProcedureCount,
        ushort? SystemModuleId,
        int ItemCount,
        int VmtCount,
        int RttiCount,
        int StringCount,
        int ProcedureCount,
        int DisassemblyLineCount,
        int FormCount,
        DelphiForm[] Forms,
        CommandLineAnalysisItem[] Items,
        CommandLineDisassemblyLine[] Disassembly,
        string[] Diagnostics);

    private sealed record CommandLineAnalysisItem(
        uint AddressRva,
        string Rva,
        string Va,
        string Flags,
        string? Name,
        string? TypeKind,
        uint? RecordSizeBytes,
        uint? ClassInstanceSizeBytes,
        uint? ParentRva,
        uint? ExceptionRegionHandlerRva,
        uint? FinallyRegionEndRva,
        uint? Int64ComparisonEndRva,
        uint? ProcedureSizeBytes,
        ushort? ReturnStackBytes,
        string? ReturnTypeCandidate,
        string? DataTypeCandidate,
        string? ResourceStringCandidate,
        string? ThreadVariableCandidate,
        bool UsesFramePointer,
        long? StackPointerDeltaBytes,
        StackArgument[] StackArguments,
        StackLocalVariable[] StackLocalVariables,
        string[] RegisterArgumentCandidates,
        CommandLineCrossReference[] OutgoingCrossReferences,
        CommandLineCrossReference[] IncomingCrossReferences,
        DelphiExceptionHandler[] ExceptionHandlers,
        DelphiVmtMethod[] Methods,
        DelphiVmtField[] Fields,
        DelphiRttiRecordField[] RecordFields,
        MemberAccessCandidate[] MemberAccessCandidates,
        DelphiVmtInterface[] Interfaces,
        DelphiVmtVirtualMethod[] VirtualMethods);

    private sealed record CommandLineCrossReference(
        uint AddressRva,
        string Rva,
        string Kind,
        string? Name);

    private sealed record CommandLineDisassemblyLine(
        uint AddressRva,
        string Rva,
        string Bytes,
        string Mnemonic,
        string FormattedText,
        uint? BranchTargetRva,
        string FlowControl);
}
