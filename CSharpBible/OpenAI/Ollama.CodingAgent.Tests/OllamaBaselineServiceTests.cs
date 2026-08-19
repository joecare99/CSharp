using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Client.Models;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class OllamaBaselineServiceTests
{
    [TestMethod]
    public async Task RunPreflightAsync_ReturnsSuccessWhenConfiguredModelIsAvailable()
    {
        IOllamaBaselineClient client = Substitute.For<IOllamaBaselineClient>();
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<string>>(["qwen2.5-coder:7b", "llama3"]));

        OllamaBaselineCheckResult result = await new OllamaBaselineService(client, "qwen2.5-coder:7b")
            .RunPreflightAsync();

        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.ModelAvailable);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task RunPreflightAsync_ReturnsFailureWhenConfiguredModelIsMissing()
    {
        IOllamaBaselineClient client = Substitute.For<IOllamaBaselineClient>();
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<string>>(["llama3"]));

        OllamaBaselineCheckResult result = await new OllamaBaselineService(client, "qwen2.5-coder:7b")
            .RunPreflightAsync();

        Assert.IsFalse(result.Success);
        Assert.IsFalse(result.ModelAvailable);
        StringAssert.Contains(result.Error, "not available");
    }

    [TestMethod]
    public async Task RunSmokeAsync_ReturnsFailureForEmptyResponse()
    {
        IOllamaBaselineClient client = Substitute.For<IOllamaBaselineClient>();
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<string>>(["qwen2.5-coder:7b"]));
        client.CompleteChatAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new OllamaChatCompletion { Content = " " }));

        OllamaBaselineCheckResult result = await new OllamaBaselineService(client, "qwen2.5-coder:7b")
            .RunSmokeAsync("Say hello.");

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error, "empty");
    }

    [TestMethod]
    public async Task RunPreflightAsync_PropagatesCancellation()
    {
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();
        IOllamaBaselineClient client = Substitute.For<IOllamaBaselineClient>();
        client.GetAvailableModelsAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromCanceled<IReadOnlyList<string>>(cancellationTokenSource.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => new OllamaBaselineService(client, "qwen2.5-coder:7b")
                .RunPreflightAsync(cancellationTokenSource.Token));
    }
}
