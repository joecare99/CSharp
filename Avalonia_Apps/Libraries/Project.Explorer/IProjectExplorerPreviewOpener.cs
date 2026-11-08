using System.Threading;
using System.Threading.Tasks;

namespace Project.Explorer;

/// <summary>Optionally separates selection previews from explicit file activation.</summary>
public interface IProjectExplorerPreviewOpener : IProjectExplorerItemOpener
{
    Task PreviewAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default);

    Task ActivateAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default);
}
