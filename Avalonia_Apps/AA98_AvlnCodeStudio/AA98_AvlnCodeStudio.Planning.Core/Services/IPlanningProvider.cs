using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Planning.Core.Models;

namespace AA98_AvlnCodeStudio.Planning.Core.Services;

/// <summary>
/// Defines a provider-neutral planning driver that can read, write, scaffold, and template planning data.
/// </summary>
public interface IPlanningProvider
{
    Task<PlanningReadResult> ReadAsync(PlanningReadRequest request, CancellationToken cancellationToken = default);

    Task<PlanningWriteResult> WriteAsync(PlanningWriteRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlanningDocumentTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
}
