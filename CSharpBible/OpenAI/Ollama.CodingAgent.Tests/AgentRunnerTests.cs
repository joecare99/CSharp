using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class AgentRunnerTests
{
    [TestMethod]
    public async Task RunAsync_ReturnsFirstNonEmptyResponse()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("First answer"));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());
        AgentRunResult result = await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Hello",
            SystemPrompt = "System",
        });

        Assert.AreEqual("First answer", result.FinalResponse);
        Assert.AreEqual(1, result.IterationsUsed);
        Assert.AreEqual(0, result.RetryAttemptsUsed);
        Assert.IsFalse(result.FinalizedWithMarker);
    }

    [TestMethod]
    public async Task RunAsync_PreservesThinkingFromDetailedModelClient()
    {
        IThinkingAgentModelClient modelClient = Substitute.For<IThinkingAgentModelClient>();
        modelClient
            .CompleteDetailedAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new AgentCompletion
            {
                Content = "Answer",
                Thinking = ["Plan", " -> execute"],
            }));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());
        AgentRunResult result = await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Thinking scenario",
            SystemPrompt = "System",
        });

        CollectionAssert.AreEqual(new[] { "Plan", " -> execute" }, (System.Collections.ICollection)result.Thinking);
    }

    [TestMethod]
    public async Task RunAsync_StripsFinalMarker()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("[[FINAL]]  Completed plan."));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());
        AgentRunResult result = await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Hello",
            SystemPrompt = "System",
        });

        Assert.AreEqual("Completed plan.", result.FinalResponse);
        Assert.IsTrue(result.FinalizedWithMarker);
    }

    [TestMethod]
    public async Task RunAsync_RetriesTransientFailures()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => Task.FromException<string>(new TimeoutException("first attempt timeout")),
                _ => Task.FromException<string>(new TimeoutException("second attempt timeout")),
                _ => Task.FromResult("Recovered"));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());
        AgentRunResult result = await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Retry scenario",
            SystemPrompt = "System",
        });

        Assert.AreEqual("Recovered", result.FinalResponse);
        Assert.AreEqual(2, result.RetryAttemptsUsed);
        await modelClient.Received(3).CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task RunAsync_ThrowsAfterRetryBudgetIsExhausted()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<string>(new TimeoutException("always failing")));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());

        TimeoutException exception = await Assert.ThrowsExactlyAsync<TimeoutException>(() => runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Failure scenario",
            SystemPrompt = "System",
        }));

        StringAssert.Contains(exception.Message, "always failing");
    }

    [TestMethod]
    public async Task RunAsync_ThrowsWhenNoResponseWithinIterationBudget()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(" "));

        AgentRunner runner = new(modelClient, new OllamaAgentRuntimeSettings(TimeSpan.FromMinutes(12), retryCount: 1, maxIterations: 2));

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => runner.RunAsync(new AgentRunRequest
        {
            Prompt = "No content scenario",
            SystemPrompt = "System",
        }));

        StringAssert.Contains(exception.Message, "did not produce a response");
    }

    [TestMethod]
    public async Task RunAsync_RecordsCorrelatedTurnDiagnostics()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("Answer"));
        InMemoryAgentDiagnosticsSink diagnostics = new();

        AgentRunner runner = new(modelClient, CreateBaselineSettings(), diagnostics);
        await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Diagnostics scenario",
            SystemPrompt = "System",
        });

        Assert.AreEqual(3, diagnostics.Events.Count);
        Assert.AreEqual("run.started", diagnostics.Events[0].EventName);
        Assert.AreEqual("completion.succeeded", diagnostics.Events[1].EventName);
        Assert.AreEqual("run.completed", diagnostics.Events[2].EventName);
        Assert.IsFalse(string.IsNullOrWhiteSpace(diagnostics.Events[0].CorrelationId));
        Assert.AreEqual(diagnostics.Events[0].CorrelationId, diagnostics.Events[1].CorrelationId);
        Assert.IsNotNull(diagnostics.Events[1].Duration);
    }

    [TestMethod]
    public async Task RunAsync_RecordsRetryFailuresWithErrorDetails()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(
                _ => Task.FromException<string>(new TimeoutException("first attempt timeout")),
                _ => Task.FromResult("Recovered"));
        InMemoryAgentDiagnosticsSink diagnostics = new();

        AgentRunner runner = new(modelClient, CreateBaselineSettings(), diagnostics);
        await runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Failure diagnostics scenario",
            SystemPrompt = "System",
        });

        Assert.IsTrue(diagnostics.Events.Any(static item => item.EventName == "completion.failed"));
        AgentDiagnosticEvent failure = diagnostics.Events[1];
        StringAssert.Contains(failure.Error ?? string.Empty, "TimeoutException");
        Assert.AreEqual(0, failure.Attempt);
    }

    [TestMethod]
    public async Task RunAsync_DoesNotRetryNonTransientExceptions()
    {
        IAgentModelClient modelClient = Substitute.For<IAgentModelClient>();
        modelClient
            .CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<string>(new ArgumentException("not transient")));

        AgentRunner runner = new(modelClient, CreateBaselineSettings());

        ArgumentException exception = await Assert.ThrowsExactlyAsync<ArgumentException>(() => runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Non transient failure",
            SystemPrompt = "System",
        }));

        StringAssert.Contains(exception.Message, "not transient");
        await modelClient.Received(1).CompleteAsync(Arg.Any<IReadOnlyList<AgentMessage>>(), Arg.Any<CancellationToken>());
    }

    private static OllamaAgentRuntimeSettings CreateBaselineSettings()
        => new(OllamaAgentRuntimeSettings.DefaultStepTimeout, OllamaAgentRuntimeSettings.DefaultRetryCount, OllamaAgentRuntimeSettings.DefaultMaxIterations, retryBackoff: TimeSpan.Zero);
}
