using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies provider-neutral planning adapter contracts.
/// </summary>
[TestClass]
public class PlanningAdapterContractTests
{
    /// <summary>
    /// Verifies that adapter capabilities support independent import and export operations.
    /// </summary>
    [TestMethod]
    public void PlanningAdapterCapability_CombinesImportAndExport()
    {
        PlanningAdapterCapability capabilities = PlanningAdapterCapability.Import | PlanningAdapterCapability.Export;

        Assert.IsTrue(capabilities.HasFlag(PlanningAdapterCapability.Import));
        Assert.IsTrue(capabilities.HasFlag(PlanningAdapterCapability.Export));
    }

    /// <summary>
    /// Verifies that a local planning identity remains separate from its provider identity.
    /// </summary>
    [TestMethod]
    public void PlanningItemMapping_KeepsLocalAndProviderIdentitySeparate()
    {
        PlanningItemMapping mapping = new()
        {
            LocalItemId = "AA98-T065",
        };
        mapping.ProviderItem.AdapterId = "ExternalPlanning";
        mapping.ProviderItem.ScopeId = "Project-42";
        mapping.ProviderItem.ProviderItemId = "1234";

        Assert.AreEqual("AA98-T065", mapping.LocalItemId);
        Assert.AreEqual("ExternalPlanning", mapping.ProviderItem.AdapterId);
        Assert.AreEqual("Project-42", mapping.ProviderItem.ScopeId);
        Assert.AreEqual("1234", mapping.ProviderItem.ProviderItemId);
        Assert.AreNotEqual(mapping.LocalItemId, mapping.ProviderItem.ProviderItemId);
    }

    /// <summary>
    /// Verifies that a neutral adapter can consume synchronization data without provider SDK types.
    /// </summary>
    [TestMethod]
    public async Task IPlanningAdapter_SynchronizesProviderNeutralRequest()
    {
        IPlanningAdapter adapter = new FakePlanningAdapter();
        PlanningSynchronizationRequest request = new()
        {
            AdapterId = "ExternalPlanning",
            ScopeId = "Project-42",
            Mode = PlanningSynchronizationMode.Import,
        };

        PlanningSynchronizationResult result = await adapter.SynchronizeAsync(request).ConfigureAwait(false);

        Assert.AreEqual("ExternalPlanning", adapter.Descriptor.AdapterId);
        Assert.IsTrue(adapter.Descriptor.Capabilities.HasFlag(PlanningAdapterCapability.Import));
        Assert.AreEqual(1, result.ImportedItems.Count);
        Assert.AreEqual("AA98-T065", result.ImportedItems[0].Id);
    }

    /// <summary>
    /// Verifies that credentials are obtained only through the abstract credential service.
    /// </summary>
    [TestMethod]
    public async Task IPlanningCredentialService_ResolvesCredentialByAdapterAndScope()
    {
        IPlanningCredentialService credentialService = new FakePlanningCredentialService();

        string? credential = await credentialService.GetCredentialAsync("ExternalPlanning", "Project-42").ConfigureAwait(false);

        Assert.AreEqual("credential", credential);
    }

    private sealed class FakePlanningAdapter : IPlanningAdapter
    {
        public PlanningAdapterDescriptor Descriptor { get; } = new()
        {
            AdapterId = "ExternalPlanning",
            DisplayName = "External Planning",
            Capabilities = PlanningAdapterCapability.Import,
        };

        public Task<PlanningSynchronizationResult> SynchronizeAsync(PlanningSynchronizationRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PlanningSynchronizationResult result = new();
            result.ImportedItems.Add(new PlanningItem
            {
                Id = "AA98-T065",
                Title = request.ScopeId,
                Kind = PlanningItemKind.Task,
            });
            return Task.FromResult(result);
        }
    }

    private sealed class FakePlanningCredentialService : IPlanningCredentialService
    {
        public Task<string?> GetCredentialAsync(string adapterId, string? scopeId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<string?>(adapterId == "ExternalPlanning" && scopeId == "Project-42" ? "credential" : null);
        }
    }
}
