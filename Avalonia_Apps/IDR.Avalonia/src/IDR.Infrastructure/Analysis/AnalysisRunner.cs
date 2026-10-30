using IDR.Core.Models;
using IDR.Core.Services;
using IDR.Infrastructure.KnowledgeBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.Analysis;

public sealed class AnalysisRunner
{
    private readonly IPeImageLoader _imageLoader;
    private readonly IDelphiVersionDetector _versionDetector;
    private readonly IKnowledgeBaseProvider _knowledgeBaseProvider;
    private readonly IAnalysisService _analysisService;

    public AnalysisRunner(
        IPeImageLoader imageLoader,
        IDelphiVersionDetector versionDetector,
        IKnowledgeBaseProvider knowledgeBaseProvider,
        IAnalysisService analysisService)
    {
        _imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
        _versionDetector = versionDetector ?? throw new ArgumentNullException(nameof(versionDetector));
        _knowledgeBaseProvider = knowledgeBaseProvider
            ?? throw new ArgumentNullException(nameof(knowledgeBaseProvider));
        _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
    }

    public async Task<AnalysisExecution> AnalyzeAsync(
        string inputPath,
        DelphiVersion version,
        string? knowledgeBasePath,
        string? knowledgeBaseDirectory,
        IProgress<AnalysisProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (knowledgeBasePath is not null && knowledgeBaseDirectory is not null)
        {
            throw new ArgumentException(
                "Specify either a Knowledge Base file or directory, not both.");
        }

        string resolvedKnowledgeBasePath = ResolveKnowledgeBasePath(
            version,
            knowledgeBasePath,
            knowledgeBaseDirectory);
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

        AnalysisResult result = await _analysisService
            .AnalyzeAsync(session, progress, cancellationToken)
            .ConfigureAwait(false);
        return new AnalysisExecution(
            session,
            knowledgeBase,
            resolvedKnowledgeBasePath,
            detection,
            result,
            image.Forms,
            image.FormDiagnostics);
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
}

public sealed record AnalysisExecution(
    AnalysisSession Session,
    IKnowledgeBase KnowledgeBase,
    string KnowledgeBasePath,
    DelphiVersionDetection Detection,
    AnalysisResult Result,
    IReadOnlyList<DelphiForm> Forms,
    IReadOnlyList<string> FormDiagnostics);
