using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Application.Tests;

[TestClass]
public sealed class AgentApprovalServiceTests
{
    [TestMethod]
    public async Task RequestApprovalAsync_WaitsForAndReturnsResolvedDecision()
    {
        AgentApprovalService service = new();
        AgentApprovalRequest request = new()
        {
            Id = "commit-1",
            Operation = "git commit",
            Preview = "Commit 1 changed file.",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        Task<bool> decision = service.RequestApprovalAsync(request);

        Assert.AreEqual(1, service.PendingRequests.Count);
        Assert.IsTrue(service.Resolve(request.Id, approved: true));

        Assert.IsTrue(await decision);
        Assert.AreEqual(0, service.PendingRequests.Count);
    }

    [TestMethod]
    public async Task RequestApprovalAsync_CancellationRejectsAndRemovesRequest()
    {
        AgentApprovalService service = new();
        AgentApprovalRequest request = new()
        {
            Id = "push-1",
            Operation = "git push",
            Preview = "Push branch main to origin.",
            CreatedAt = DateTimeOffset.UtcNow,
        };
        using var cancellationTokenSource = new System.Threading.CancellationTokenSource();

        Task<bool> decision = service.RequestApprovalAsync(request, cancellationTokenSource.Token);
        cancellationTokenSource.Cancel();

        Assert.IsFalse(await decision);
        Assert.AreEqual(0, service.PendingRequests.Count);
    }
}
