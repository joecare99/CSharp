using System.Threading;
using System.Threading.Tasks;
using OFBCreator.Core.Models;
using OFBCreator.Projects.Models;

namespace OFBCreator.Avalonia.Services;

public interface IOFBWorkspaceService
{
    Task ExportAsync(OFBProject project, string projectPath, CancellationToken cancellationToken = default);

    Task<OFBFamilyGroupingResult> PreviewGroupingAsync(
        OFBProject project,
        string projectPath,
        CancellationToken cancellationToken = default);
}
