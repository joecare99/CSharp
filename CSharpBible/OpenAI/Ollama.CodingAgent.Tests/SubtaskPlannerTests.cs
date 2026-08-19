using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class SubtaskPlannerTests
{
    [TestMethod]
    public void CreateInitialPlan_BuildPrompt_IncludesBuildExecutionSubtask()
    {
        PlanState plan = SubtaskPlanner.CreateInitialPlan("Build the coding agent project and summarize issues.");

        Assert.IsTrue(plan.Subtasks.Count >= 3);
        Assert.AreEqual("execute-build", plan.Subtasks[2].Id);
    }

    [TestMethod]
    public void CreateInitialPlan_TestPrompt_IncludesTestExecutionSubtask()
    {
        PlanState plan = SubtaskPlanner.CreateInitialPlan("Run tests and propose one fix.");

        Assert.AreEqual("execute-tests", plan.Subtasks[2].Id);
    }

    [TestMethod]
    public void CreateInitialPlan_ExposesOrderedDependenciesAndReadyWork()
    {
        PlanState plan = SubtaskPlanner.CreateInitialPlan("Build the coding agent project.");

        Assert.AreEqual("analyze-goal", plan.GetReadySubtasks()[0].Id);
        plan.Subtasks[0].Status = PlannedSubtaskStatus.Done;
        Assert.AreEqual("collect-context", plan.GetReadySubtasks()[0].Id);
        Assert.AreEqual("analyze-goal", plan.Subtasks[1].Dependencies[0]);
    }
}
