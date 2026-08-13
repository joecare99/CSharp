using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class AgentEvaluationRunnerTests
{
    [TestMethod]
    public async Task RunAsync_ReturnsScenarioOutcomesAndPassRate()
    {
        AgentEvaluationReport report = await new AgentEvaluationRunner().RunAsync(
        [
            new AgentEvaluationScenario(
                "baseline-chat",
                "Produces a non-empty response.",
                _ => Task.FromResult(new AgentEvaluationScenarioResult
                {
                    Passed = true,
                    Details = "response received",
                })),
            new AgentEvaluationScenario(
                "tool-denial",
                "Rejects a denied tool.",
                _ => Task.FromResult(new AgentEvaluationScenarioResult
                {
                    Passed = false,
                    Details = "denial behavior regressed",
                })),
        ], minimumPassRate: 0.5);

        Assert.AreEqual(2, report.Outcomes.Count);
        Assert.AreEqual(0.5, report.PassRate);
        Assert.IsTrue(report.Ready);
        Assert.IsTrue(report.Outcomes[0].Duration >= TimeSpan.Zero);
    }

    [TestMethod]
    public async Task RunAsync_ConvertsScenarioExceptionsIntoFailedOutcomes()
    {
        AgentEvaluationReport report = await new AgentEvaluationRunner().RunAsync(
        [
            new AgentEvaluationScenario(
                "failing-scenario",
                "Reports an infrastructure failure.",
                _ => throw new InvalidOperationException("fixture failed")),
        ]);

        Assert.IsFalse(report.Ready);
        Assert.IsFalse(report.Outcomes[0].Passed);
        StringAssert.Contains(report.Outcomes[0].Details, "fixture failed");
    }
}
