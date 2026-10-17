using System.Threading;
using System.Threading.Tasks;
using OFBCreator.Core.Models;
using OFBCreator.Projects.Models;
using Genealogy.Statistics;

namespace OFBCreator.Avalonia.Services;

public interface IOFBWorkspaceService
{
    Task ExportAsync(OFBProject project, string projectPath, CancellationToken cancellationToken = default);

    Task<OFBFamilyGroupingResult> PreviewGroupingAsync(
        OFBProject project,
        string projectPath,
        CancellationToken cancellationToken = default);

    Task<OFBWorkspacePreview> PreviewWorkspaceAsync(
        OFBProject project,
        string projectPath,
        CancellationToken cancellationToken = default);

    Task<GenealogyStatisticsResult> PreviewStatisticsAsync(
        OFBProject project,
        string projectPath,
        int? firstYear,
        int? lastYear,
        CancellationToken cancellationToken = default);
}
