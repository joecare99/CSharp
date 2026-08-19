using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class PlanStateRendererTests
{
    [TestMethod]
    public void Render_IncludesGoalAndSubtaskLines()
    {
        PlanState plan = SubtaskPlanner.CreateInitialPlan("Build and test coding agent.");

        string text = PlanStateRenderer.Render(plan);

        StringAssert.Contains(text, "Goal contract:");
        StringAssert.Contains(text, "Subtasks:");
        StringAssert.Contains(text, "analyze-goal");
    }
}
