using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class GoalDriftAnalyzerTests
{
    [TestMethod]
    public void IsDriftDetected_ReturnsTrueForGenericFailureWithoutGoalSignal()
    {
        GoalContract goal = new("Implement build pipeline diagnostics in coding agent", ["result"]);
        PlannedSubtask subtask = new("s1", "Run build", "Validate diagnostics.");

        bool drift = GoalDriftAnalyzer.IsDriftDetected(goal, subtask, "operation was canceled");

        Assert.IsTrue(drift);
    }

    [TestMethod]
    public void IsDriftDetected_ReturnsFalseWhenGoalKeywordIsPresent()
    {
        GoalContract goal = new("Implement build pipeline diagnostics in coding agent", ["result"]);
        PlannedSubtask subtask = new("s1", "Run build", "Validate diagnostics.");

        bool drift = GoalDriftAnalyzer.IsDriftDetected(goal, subtask, "build failed while updating diagnostics.");

        Assert.IsFalse(drift);
    }
}
