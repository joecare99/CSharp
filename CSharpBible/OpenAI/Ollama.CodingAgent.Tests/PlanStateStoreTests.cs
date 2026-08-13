using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class PlanStateStoreTests
{
    [TestMethod]
    public async Task SaveAsyncAndLoadAsync_PreserveGoalDependenciesAndStatuses()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"ollama-plan-{Guid.NewGuid():N}.json");
        try
        {
            PlanState plan = SubtaskPlanner.CreateInitialPlan("Run tests for the coding agent.");
            plan.Subtasks[0].Status = PlannedSubtaskStatus.Done;
            plan.Subtasks[1].Status = PlannedSubtaskStatus.InProgress;
            PlanStateStore store = new(filePath);

            await store.SaveAsync(plan);
            PlanState resumed = await store.LoadAsync();

            Assert.AreEqual(plan.GoalContract.Objective, resumed.GoalContract.Objective);
            Assert.AreEqual(PlannedSubtaskStatus.InProgress, resumed.Subtasks[1].Status);
            CollectionAssert.AreEqual(
                new[] { "collect-context" },
                (System.Collections.ICollection)resumed.Subtasks[2].Dependencies);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
