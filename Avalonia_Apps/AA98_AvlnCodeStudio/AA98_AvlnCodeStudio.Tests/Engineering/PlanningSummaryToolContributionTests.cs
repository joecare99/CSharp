using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AppKomponentBaseLib.Commands;
using AppKomponentBaseLib.Components;
using AppKomponentBaseLib.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies the planning summary tool contribution.
/// </summary>
[TestClass]
public sealed class PlanningSummaryToolContributionTests
{
    /// <summary>
    /// Verifies that the descriptor exposes the expected tool metadata.
    /// </summary>
    [TestMethod]
    public void DescriptorProvidesToolMetadata()
    {
        var contribution = CreateContribution();

        Assert.AreEqual("Planning.Summarize", contribution.Descriptor.CommandId);
        Assert.AreEqual("Summarize planning", contribution.Descriptor.DisplayTitle);
        Assert.IsTrue(contribution.Descriptor.RequiresConsent);
        Assert.AreEqual(ToolCommandSafetyLevel.Low, contribution.Descriptor.SafetyLevel);
        Assert.AreEqual(2, contribution.Descriptor.Parameters.Count);
        Assert.AreEqual("repositoryRootPath", contribution.Descriptor.Parameters[0].ParameterName);
        Assert.AreEqual("DevOps", contribution.Descriptor.Parameters[1].DefaultValue);
        Assert.AreEqual(1, contribution.Descriptor.Results.Count);
        CollectionAssert.AreEqual(new[] { "Planning" }, (System.Collections.ICollection)contribution.Descriptor.RequiredContextKinds);
    }

    /// <summary>
    /// Verifies that execution is gated by planning context.
    /// </summary>
    [TestMethod]
    public void CanExecuteRequiresPlanningContext()
    {
        var contribution = CreateContribution();
        var context = new TestAppContext(new[] { new AppContextTarget("Editor") });

        Assert.IsFalse(contribution.CanExecute(context));
    }

    /// <summary>
    /// Verifies that execution aggregates planning items by kind.
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsyncBuildsSummaryFromPlanningItems()
    {
        var provider = Substitute.For<IPlanningProvider>();
        var planningResult = new PlanningReadResult
        {
            RepositoryRootPath = "C:/repo",
            PlanningRootPath = "DevOps"
        };

        planningResult.Items.Add(new PlanningItem { Kind = PlanningItemKind.Epic, Title = "Epic" });
        planningResult.Items.Add(new PlanningItem { Kind = PlanningItemKind.Epic, Title = "Another Epic" });
        planningResult.Items.Add(new PlanningItem { Kind = PlanningItemKind.Task, Title = "Task" });

        provider.ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(planningResult));

        var contribution = new PlanningSummaryToolContribution(provider);
        var context = new TestAppContext(new[] { new AppContextTarget("Planning") });
        object? result = await contribution.ExecuteAsync(
            context,
            new Dictionary<string, object?>
            {
                ["repositoryRootPath"] = "C:/repo",
                ["planningRootPath"] = "DevOps"
            },
            CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(string.Join(Environment.NewLine, new[] { "Epic: 2", "Task: 1" }), result);
    }

    private static PlanningSummaryToolContribution CreateContribution()
    {
        var provider = Substitute.For<IPlanningProvider>();
        return new PlanningSummaryToolContribution(provider);
    }

    private sealed class TestAppContext : IAppContext
    {
        public TestAppContext(IReadOnlyList<AppContextTarget> targets)
        {
            Targets = targets;
        }

        public string? ActiveComponentId => null;

        public AppComponentDescriptor? ActiveComponent => null;

        public IReadOnlyList<AppContextTarget> Targets { get; }

        public IServiceProvider? Services => null;
    }
}
