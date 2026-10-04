using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OFBCreator.Console.Services;
using OFBCreator.Core.Models;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Avalonia.Services;

/// <summary>Adapts the shared project contract to the same export pipeline used by the CLI.</summary>
public sealed class OFBWorkspaceService(ConsoleExportService exportService) : IOFBWorkspaceService
{
    private readonly ConsoleExportService _exportService =
        exportService ?? throw new ArgumentNullException(nameof(exportService));

    public Task ExportAsync(
        OFBProject project,
        string projectPath,
        CancellationToken cancellationToken = default) =>
        _exportService.ExportAsync(CreateOptions(project, projectPath), cancellationToken);

    public Task<OFBFamilyGroupingResult> PreviewGroupingAsync(
        OFBProject project,
        string projectPath,
        CancellationToken cancellationToken = default) =>
        _exportService.PreviewGroupingAsync(CreateOptions(project, projectPath), cancellationToken);

    private static OFBGenerateOptions CreateOptions(OFBProject project, string projectPath)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        if (!string.IsNullOrWhiteSpace(project.DataSource)
            && !string.Equals(project.DataSource, "gedcom", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException(
                $"The Avalonia workspace currently supports the 'gedcom' source, not '{project.DataSource}'.");

        var inputPath = OFBProjectStore.ResolveProjectPath(projectPath, project.InputPath);
        if (string.IsNullOrWhiteSpace(inputPath))
            throw new InvalidDataException($"Project '{project.Name}' does not specify an input file.");
        var outputPath = OFBProjectStore.ResolveProjectPath(projectPath, project.OutputPath)
            ?? Path.Combine(Path.GetDirectoryName(Path.GetFullPath(projectPath))!, project.Name + ".docx");

        return new OFBGenerateOptions
        {
            InputPath = inputPath,
            OutputPath = outputPath,
            Title = project.Title,
            PlaceId = project.PlaceId,
            IncludeDescendants = project.IncludeDescendants,
            Preface = project.Preface,
            Legend = project.Legend,
            Template = OFBProjectStore.ResolveEntryTemplate(projectPath, project.EntryTemplate),
            DataSource = project.DataSource ?? "gedcom",
            ExportRules = project.ExportRules,
            GroupingPolicy = project.GroupingPolicy,
            GroupingDecisions = project.GroupingDecisions
        };
    }
}
