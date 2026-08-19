using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
[DoNotParallelize]
public sealed class CliAndPlanningCoverageTests
{
    [TestMethod]
    public void Parse_HandlesAllCommandLineValuesAndMalformedArguments()
    {
        OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse(
        [
            "--help",
            "--endpoint", "https://example.test",
            "--model", "model",
            "--timeout-minutes", "1.5",
            "--retries", "0",
            "--max-iterations", "2",
            "--verbosity", "quiet",
            "--preflight",
            "--baseline-smoke",
            "--delegate",
            "--workspace-root", ".",
            "positional",
            "prompt",
        ]);

        Assert.IsTrue(options.ShowHelp);
        Assert.IsTrue(options.PreflightOnly);
        Assert.IsTrue(options.BaselineSmoke);
        Assert.IsTrue(options.DelegateMode);
        Assert.AreEqual("positional prompt", options.Prompt);
        Assert.AreEqual(AgentVerbosity.Quiet, options.RuntimeSettings.Verbosity);
        Assert.AreEqual(1.5d, options.RuntimeSettings.StepTimeout.TotalMinutes);
        Assert.ThrowsExactly<ArgumentException>(() => OllamaAgentCliOptions.Parse(["--unknown"]));
        Assert.ThrowsExactly<ArgumentException>(() => OllamaAgentCliOptions.Parse(["--model"]));
        Assert.ThrowsExactly<ArgumentException>(() => OllamaAgentCliOptions.Parse(["--retries", "nope"]));
        Assert.ThrowsExactly<ArgumentException>(() => OllamaAgentCliOptions.Parse(["--timeout-minutes", "nope"]));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => OllamaAgentCliOptions.Parse(["--max-iterations", "0"]));
        Assert.ThrowsExactly<ArgumentException>(() => OllamaAgentCliOptions.Parse(["--endpoint", " "]));
    }

    [TestMethod]
    public void Parse_ReadsEnvironmentFallbacks()
    {
        string[] variableNames =
        [
            "AGENT_TIMEOUT_MINUTES",
            "AGENT_RETRY_COUNT",
            "AGENT_MAX_ITERATIONS",
            "AGENT_VERBOSITY",
            "AGENT_SHOW_THINKING",
        ];
        Dictionary<string, string?> originalValues = new();
        foreach (string variableName in variableNames)
        {
            originalValues[variableName] = Environment.GetEnvironmentVariable(variableName);
        }

        try
        {
            Environment.SetEnvironmentVariable("AGENT_TIMEOUT_MINUTES", "2");
            Environment.SetEnvironmentVariable("AGENT_RETRY_COUNT", "1");
            Environment.SetEnvironmentVariable("AGENT_MAX_ITERATIONS", "3");
            Environment.SetEnvironmentVariable("AGENT_VERBOSITY", "verbose");
            Environment.SetEnvironmentVariable("AGENT_SHOW_THINKING", "true");

            OllamaAgentCliOptions options = OllamaAgentCliOptions.Parse([]);

            Assert.AreEqual(2d, options.RuntimeSettings.StepTimeout.TotalMinutes);
            Assert.AreEqual(1, options.RuntimeSettings.RetryCount);
            Assert.AreEqual(3, options.RuntimeSettings.MaxIterations);
            Assert.AreEqual(AgentVerbosity.Verbose, options.RuntimeSettings.Verbosity);
            Assert.IsTrue(options.RuntimeSettings.ShowThinking);
        }
        finally
        {
            foreach ((string variableName, string? value) in originalValues)
            {
                Environment.SetEnvironmentVariable(variableName, value);
            }
        }
    }

    [TestMethod]
    public async Task EvaluationPlanningAndDiagnostics_ExerciseFailureAndReadinessBranches()
    {
        AgentEvaluationRunner runner = new();
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => runner.RunAsync([], -0.1));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => runner.RunAsync([]));
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => runner.RunAsync(
        [
            new AgentEvaluationScenario("cancelled", "description", _ => Task.FromResult(new AgentEvaluationScenarioResult { Passed = true })),
        ], cancellationToken: cancellation.Token));

        AgentEvaluationReport report = await runner.RunAsync(
        [
            new AgentEvaluationScenario("pass", "description", _ => Task.FromResult(new AgentEvaluationScenarioResult { Passed = true, Details = "ok" })),
            new AgentEvaluationScenario("fail", "description", _ => Task.FromException<AgentEvaluationScenarioResult>(new InvalidOperationException("expected"))),
        ],
        minimumPassRate: 0.75);
        Assert.IsFalse(report.Ready);
        Assert.AreEqual(0.5d, report.PassRate);

        Assert.ThrowsExactly<ArgumentException>(() => new AgentEvaluationScenario(string.Empty, "description", _ => Task.FromResult(new AgentEvaluationScenarioResult())));
        Assert.ThrowsExactly<ArgumentException>(() => new AgentEvaluationScenario("id", string.Empty, _ => Task.FromResult(new AgentEvaluationScenarioResult())));
        PlanState ordinaryPlan = SubtaskPlanner.CreateInitialPlan("Update the source.");
        Assert.AreEqual("execute-change", ordinaryPlan.Subtasks[2].Id);
        ordinaryPlan.Subtasks[0].Status = PlannedSubtaskStatus.Done;
        ordinaryPlan.Subtasks[1].Status = PlannedSubtaskStatus.Done;
        Assert.AreEqual("execute-change", ordinaryPlan.GetReadySubtasks()[0].Id);
        Assert.IsFalse(GoalDriftAnalyzer.IsDriftDetected(ordinaryPlan.GoalContract, ordinaryPlan.Subtasks[2], "source updated successfully"));
    }
}
