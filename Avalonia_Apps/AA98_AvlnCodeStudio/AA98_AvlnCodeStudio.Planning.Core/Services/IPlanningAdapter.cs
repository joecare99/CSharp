using AA98_AvlnCodeStudio.Planning.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.Core.Services;

/// <summary>
/// Defines a provider-neutral boundary for external planning adapters.
/// </summary>
public interface IPlanningAdapter
{
    /// <summary>
    /// Gets the adapter descriptor and its supported operations.
    /// </summary>
    PlanningAdapterDescriptor Descriptor { get; }

    /// <summary>
    /// Synchronizes planning items according to the requested direction.
    /// </summary>
    /// <param name="request">The provider-neutral synchronization request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The synchronization result.</returns>
    Task<PlanningSynchronizationResult> SynchronizeAsync(PlanningSynchronizationRequest request, CancellationToken cancellationToken = default);
}
