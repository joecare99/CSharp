using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent.Models;

namespace Ollama.CodingAgent.HostCheck.Planning.Tests;

[TestClass]
public sealed class PlanningHostCheckTests
{
    [TestMethod]
    public void Main_HandlesDefaultCustomAndInvalidPrompts()
    {
        Assert.AreEqual(0, InvokeMain([]));
        Assert.AreEqual(0, InvokeMain(["Build the project."]));
        Assert.ThrowsExactly<TargetInvocationException>(() => InvokeMain([""]));
    }

    [TestMethod]
    public void Main_CoversUnexpectedSuccessValidationBranchesThroughSeams()
    {
        Type programType = Assembly.Load("Ollama.CodingAgent.HostCheck.Planning")
            .GetType("Ollama.CodingAgent.HostCheck.Planning.Program", throwOnError: true)!;
        FieldInfo planFactory = programType.GetField("CreatePlan", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo goalFactory = programType.GetField("CreateGoal", BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate originalPlanFactory = (Delegate)planFactory.GetValue(null)!;
        Delegate originalGoalFactory = (Delegate)goalFactory.GetValue(null)!;
        PlanState validPlan = SubtaskPlanner.CreateInitialPlan("valid build");
        try
        {
            planFactory.SetValue(null, (Func<string, PlanState>)(_ => validPlan));
            Assert.AreEqual(2, InvokeMain(["valid"]));

            planFactory.SetValue(null, originalPlanFactory);
            goalFactory.SetValue(null, (Func<string, IReadOnlyList<string>, GoalContract>)((objective, criteria) =>
                new GoalContract("valid objective", criteria)));
            Assert.AreEqual(3, InvokeMain(["valid"]));
        }
        finally
        {
            planFactory.SetValue(null, originalPlanFactory);
            goalFactory.SetValue(null, originalGoalFactory);
        }
    }

    private static int InvokeMain(string[] arguments)
    {
        Type programType = Assembly.Load("Ollama.CodingAgent.HostCheck.Planning")
            .GetType("Ollama.CodingAgent.HostCheck.Planning.Program", throwOnError: true)!;
        MethodInfo main = programType.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (int)main.Invoke(null, [arguments])!;
    }
}
