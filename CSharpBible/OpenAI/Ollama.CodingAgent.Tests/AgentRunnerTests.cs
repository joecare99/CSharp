using System;
using System.Collections.Generic;
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

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => runner.RunAsync(new AgentRunRequest
        {
            Prompt = "Failure scenario",
            SystemPrompt = "System",
        }));

        StringAssert.Contains(exception.Message, "failed after");
        Assert.IsNotNull(exception.InnerException);
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

    private static OllamaAgentRuntimeSettings CreateBaselineSettings()
        => new(OllamaAgentRuntimeSettings.DefaultStepTimeout, OllamaAgentRuntimeSettings.DefaultRetryCount, OllamaAgentRuntimeSettings.DefaultMaxIterations);
}
