using AA98_AvlnCodeStudio.Planning.AzureDevOps.DependencyInjection;
using AA98_AvlnCodeStudio.Planning.AzureDevOps.Services;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies the Azure DevOps planning adapter skeleton without network or SDK dependencies.
/// </summary>
[TestClass]
public class AzureDevOpsPlanningAdapterTests
{
    /// <summary>
    /// Verifies that the adapter declares import and export capabilities.
    /// </summary>
    [TestMethod]
    public void Descriptor_ReportsImportAndExportCapabilities()
    {
        AzureDevOpsPlanningAdapter adapter = new(Substitute.For<IPlanningCredentialService>());

        Assert.AreEqual("AzureDevOps", adapter.Descriptor.AdapterId);
        Assert.IsTrue(adapter.Descriptor.Capabilities.HasFlag(PlanningAdapterCapability.Import));
        Assert.IsTrue(adapter.Descriptor.Capabilities.HasFlag(PlanningAdapterCapability.Export));
    }

    /// <summary>
    /// Verifies that the skeleton resolves credentials through the neutral service and returns a diagnostic.
    /// </summary>
    [TestMethod]
    public async Task SynchronizeAsync_MatchingAdapter_UsesCredentialServiceAndReportsSkeletonState()
    {
        IPlanningCredentialService credentialService = Substitute.For<IPlanningCredentialService>();
        credentialService
            .GetCredentialAsync("AzureDevOps", "Project-42", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<string?>("credential"));
        AzureDevOpsPlanningAdapter adapter = new(credentialService);

        PlanningSynchronizationResult result = await adapter.SynchronizeAsync(new PlanningSynchronizationRequest
        {
            AdapterId = "AzureDevOps",
            ScopeId = "Project-42",
            Mode = PlanningSynchronizationMode.Import,
        });

        await credentialService.Received(1).GetCredentialAsync("AzureDevOps", "Project-42", Arg.Any<CancellationToken>());
        Assert.AreEqual(1, result.Diagnostics.Count);
        Assert.AreEqual("AZD002", result.Diagnostics[0].Code);
    }

    /// <summary>
    /// Verifies that the skeleton does not invoke credentials for a request addressed to another adapter.
    /// </summary>
    [TestMethod]
    public async Task SynchronizeAsync_DifferentAdapter_ReportsTargetMismatch()
    {
        IPlanningCredentialService credentialService = Substitute.For<IPlanningCredentialService>();
        AzureDevOpsPlanningAdapter adapter = new(credentialService);

        PlanningSynchronizationResult result = await adapter.SynchronizeAsync(new PlanningSynchronizationRequest
        {
            AdapterId = "Other",
        });

        await credentialService.DidNotReceive().GetCredentialAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
        Assert.AreEqual(1, result.Diagnostics.Count);
        Assert.AreEqual("AZD001", result.Diagnostics[0].Code);
    }

    /// <summary>
    /// Verifies that dependency injection exposes the adapter through the neutral contract.
    /// </summary>
    [TestMethod]
    public void AddAzureDevOpsPlanningAdapter_RegistersNeutralAdapter()
    {
        ServiceCollection services = new();
        services.AddSingleton(Substitute.For<IPlanningCredentialService>());
        services.AddAzureDevOpsPlanningAdapter();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IPlanningAdapter adapter = serviceProvider.GetRequiredService<IPlanningAdapter>();

        Assert.IsInstanceOfType<AzureDevOpsPlanningAdapter>(adapter);
    }
}
