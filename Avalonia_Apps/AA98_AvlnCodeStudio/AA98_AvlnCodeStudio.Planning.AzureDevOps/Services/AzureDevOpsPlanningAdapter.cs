using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.AzureDevOps.Services;

/// <summary>
/// Provides the Azure DevOps planning adapter boundary without Azure DevOps SDK dependencies.
/// </summary>
public sealed class AzureDevOpsPlanningAdapter : IPlanningAdapter
{
    private const string AdapterId = "AzureDevOps";

    private readonly IPlanningCredentialService _credentialService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureDevOpsPlanningAdapter"/> class.
    /// </summary>
    /// <param name="credentialService">The abstract credential resolver.</param>
    public AzureDevOpsPlanningAdapter(IPlanningCredentialService credentialService)
    {
        _credentialService = credentialService ?? throw new ArgumentNullException(nameof(credentialService));
    }

    /// <inheritdoc />
    public PlanningAdapterDescriptor Descriptor { get; } = new()
    {
        AdapterId = AdapterId,
        DisplayName = "Azure DevOps",
        Capabilities = PlanningAdapterCapability.Import | PlanningAdapterCapability.Export,
    };

    /// <inheritdoc />
    public async Task<PlanningSynchronizationResult> SynchronizeAsync(PlanningSynchronizationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        PlanningSynchronizationResult result = new();
        if (!string.Equals(request.AdapterId, AdapterId, StringComparison.Ordinal))
        {
            result.Diagnostics.Add(new Diagnostic
            {
                Code = "AZD001",
                Message = $"The request targets adapter '{request.AdapterId}', not '{AdapterId}'.",
                Severity = DiagnosticSeverity.Error,
            });
            return result;
        }

        _ = await _credentialService.GetCredentialAsync(AdapterId, request.ScopeId, cancellationToken).ConfigureAwait(false);
        result.Diagnostics.Add(new Diagnostic
        {
            Code = "AZD002",
            Message = "Azure DevOps synchronization is not implemented by the adapter skeleton.",
            Severity = DiagnosticSeverity.Warning,
        });
        return result;
    }
}
