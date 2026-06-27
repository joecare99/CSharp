using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Defines a host-neutral command contribution that can participate in workbench menus and related command surfaces.
/// </summary>
public interface IWorkbenchCommandContribution
{
    /// <summary>
    /// Gets the command metadata that the host uses for presentation.
    /// </summary>
    WorkbenchCommandDescriptor Descriptor { get; }

    /// <summary>
    /// Determines whether the command can execute for the current workbench context.
    /// </summary>
    /// <param name="context">The current workbench context.</param>
    /// <returns><see langword="true"/> when the command is available; otherwise <see langword="false"/>.</returns>
    bool CanExecute(IWorkbenchCommandContext context);

    /// <summary>
    /// Executes the command for the current workbench context.
    /// </summary>
    /// <param name="context">The current workbench context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when command execution has finished.</returns>
    Task ExecuteAsync(IWorkbenchCommandContext context, CancellationToken cancellationToken = default);
}
