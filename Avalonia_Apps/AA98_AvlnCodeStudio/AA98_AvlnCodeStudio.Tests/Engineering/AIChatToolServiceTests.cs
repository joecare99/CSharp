using AA98_AvlnCodeStudio.Base.AI.Models;
using AA98_AvlnCodeStudio.Base.AI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AIChatToolServiceTests
{
    [TestMethod]
    public async Task SendAsync_ProvidesRegisteredToolAndAppendsCorrelatedResult()
    {
        using JsonDocument arguments = JsonDocument.Parse("""{"message":"hello"}""");
        FakeChatTool tool = new("chat.echo", isReadOnly: true);
        Queue<AICompletionResponse> responses = new(
        [
            new AICompletionResponse(
                new AIMessage(AIMessageRole.Assistant, string.Empty),
                [new AIChatToolCall("call-1", "chat.echo", arguments.RootElement)]),
            new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Completed.")),
        ]);
        FakeAIClient client = new(request =>
        {
            Assert.AreEqual(1, request.Tools.Count);
            Assert.AreEqual("chat.echo", request.Tools[0].Name);
            return Task.FromResult(responses.Dequeue());
        });

        AIChatTurnResult turn = await new AIChatService(client, new AIChatToolCatalog([tool]))
            .SendAsync([new AIMessage(AIMessageRole.User, "Run the echo tool.")]);

        Assert.AreEqual("Completed.", turn.AssistantMessage.Content);
        Assert.AreEqual(1, turn.ToolExecutions.Count);
        Assert.IsTrue(turn.ToolExecutions[0].WasExecuted);
        Assert.AreEqual("hello", tool.LastMessage);
        Assert.AreEqual(4, turn.Messages.Count);
        Assert.AreEqual(AIMessageRole.Assistant, turn.Messages[1].Role);
        Assert.AreEqual("call-1", turn.Messages[1].ToolCalls[0].CallId);
        Assert.AreEqual(AIMessageRole.Tool, turn.Messages[2].Role);
        Assert.AreEqual("call-1", turn.Messages[2].ToolCallId);
        Assert.AreEqual("chat.echo", turn.Messages[2].ToolName);
        Assert.AreEqual("hello", turn.Messages[2].Content);
        Assert.AreEqual("Completed.", turn.Messages[3].Content);
    }

    [TestMethod]
    public async Task SendAsync_RequiresApprovalBeforeRunningMutatingTools()
    {
        using JsonDocument arguments = JsonDocument.Parse("""{"message":"change"}""");
        FakeChatTool tool = new("chat.mutate", isReadOnly: false);
        Queue<AICompletionResponse> responses = new(
        [
            new AICompletionResponse(
                new AIMessage(AIMessageRole.Assistant, string.Empty),
                [new AIChatToolCall("call-2", "chat.mutate", arguments.RootElement)]),
            new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Not approved.")),
        ]);
        FakeAIClient client = new(_ => Task.FromResult(responses.Dequeue()));

        AIChatTurnResult turn = await new AIChatService(client, new AIChatToolCatalog([tool]))
            .SendAsync([new AIMessage(AIMessageRole.User, "Make a change.")]);

        Assert.IsFalse(turn.ToolExecutions[0].WasExecuted);
        Assert.AreEqual(0, tool.ExecutionCount);
        StringAssert.Contains(turn.ToolExecutions[0].Result, "not approved");
    }

    [TestMethod]
    public async Task SendAsync_ExecutesMutatingToolOnlyAfterApproval()
    {
        using JsonDocument arguments = JsonDocument.Parse("""{"message":"change"}""");
        FakeChatTool tool = new("chat.mutate", isReadOnly: false);
        Queue<AICompletionResponse> responses = new(
        [
            new AICompletionResponse(
                new AIMessage(AIMessageRole.Assistant, string.Empty),
                [new AIChatToolCall("call-3", "chat.mutate", arguments.RootElement)]),
            new AICompletionResponse(new AIMessage(AIMessageRole.Assistant, "Changed.")),
        ]);
        FakeAIClient client = new(_ => Task.FromResult(responses.Dequeue()));
        AIChatToolDescriptor? approvedDescriptor = null;

        AIChatTurnResult turn = await new AIChatService(client, new AIChatToolCatalog([tool]))
            .SendAsync(
                [new AIMessage(AIMessageRole.User, "Make a change.")],
                approveToolExecution: (descriptor, _, _) =>
                {
                    approvedDescriptor = descriptor;
                    return Task.FromResult(true);
                });

        Assert.IsTrue(turn.ToolExecutions[0].WasExecuted);
        Assert.AreEqual("chat.mutate", approvedDescriptor!.Name);
        Assert.AreEqual(1, tool.ExecutionCount);
    }

    [TestMethod]
    public async Task SendAsync_RejectsToolCallsNotInExplicitCatalog()
    {
        using JsonDocument arguments = JsonDocument.Parse("{}");
        FakeAIClient client = new(_ => Task.FromResult(
            new AICompletionResponse(
                new AIMessage(AIMessageRole.Assistant, string.Empty),
                [new AIChatToolCall("call-4", "not.registered", arguments.RootElement)])));

        await Assert.ThrowsExactlyAsync<System.Collections.Generic.KeyNotFoundException>(() =>
            new AIChatService(client, new AIChatToolCatalog([]))
                .SendAsync([new AIMessage(AIMessageRole.User, "Do something.")]));
    }

    private sealed class FakeAIClient(Func<AICompletionRequest, Task<AICompletionResponse>> complete) : IAIClient
    {
        public Task<AICompletionResponse> CompleteAsync(AICompletionRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return complete(request);
        }
    }

    private sealed class FakeChatTool(string name, bool isReadOnly) : IAIChatTool
    {
        public AIChatToolDescriptor Descriptor { get; } = new(
            name,
            "Test chat tool.",
            """{"type":"object","properties":{"message":{"type":"string"}},"required":["message"],"additionalProperties":false}""",
            requiresConfirmation: !isReadOnly);

        public string? LastMessage { get; private set; }

        public int ExecutionCount { get; private set; }

        public Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ExecutionCount++;
            LastMessage = arguments.GetProperty("message").GetString();
            return Task.FromResult(LastMessage ?? string.Empty);
        }
    }
}
