using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client;
using Ollama.Client.Models;
using Ollama.Protocol.Models;
using Ollama.Tools.Tests.TestDoubles;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class OllamaToolCoverageTests
{
    [TestMethod]
    public void BuildToolInstructions_ReturnsSortedToolList()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "zeta",
                Description = "last",
                ResultText = "ok",
            },
            new TestTool
            {
                Name = "alpha",
                Description = "first",
                ResultText = "ok",
            },
        ]);

        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(registry);

        StringAssert.Contains(instructions, "Available tools:");
        Assert.IsTrue(instructions.IndexOf("alpha", StringComparison.Ordinal) < instructions.IndexOf("zeta", StringComparison.Ordinal));
        StringAssert.Contains(instructions, "Schema: Accepts a plain string input.");
        StringAssert.Contains(instructions, "input (string, required)");
        StringAssert.Contains(instructions, "toolName");
    }

    [TestMethod]
    public async Task ToolChatRunner_ForwardsCompletion()
    {
        ToolRunnerProtocolAdapter adapter = new();
        Ollama.Client.OllamaChatClient chatClient = new(adapter, "qwen3.5:4b");
        OllamaToolChatRunner runner = new(chatClient);

        OllamaChatCompletion completion = await runner.CompleteChatAsync(new Ollama.Client.ChatCompletionOptions
        {
            Messages =
            [
                new Ollama.Client.Models.OllamaClientChatMessage
                {
                    Role = "user",
                    Content = "hello",
                },
            ],
        });

        Assert.AreEqual("hello", completion.Content);
    }

    [TestMethod]
    public async Task RunAsync_ReturnsFailureForInvalidJson()
    {
        OllamaToolRegistry registry = new([]);
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) => Task.FromResult(new OllamaChatCompletion
            {
                Content = "not json",
            }),
        };
        OllamaToolLoopRunner runner = new(chatRunner, registry, new OllamaToolOrchestrator(registry));

        await Assert.ThrowsExactlyAsync<System.Text.Json.JsonException>(() => runner.RunAsync("hello"));
    }

    [TestMethod]
    public async Task RunAsync_ThrowsWhenToolNameIsMissing()
    {
        OllamaToolRegistry registry = new([]);
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) => Task.FromResult(new OllamaChatCompletion
            {
                Content = "null",
            }),
        };
        OllamaToolLoopRunner runner = new(chatRunner, registry, new OllamaToolOrchestrator(registry));

        OllamaToolInvocationResult result = await runner.RunAsync("hello");

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error ?? string.Empty, "valid tool call");
    }

    [TestMethod]
    public void ToolChatRunner_ThrowsForNullChatClient() => Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaToolChatRunner(null!));

    [TestMethod]
    public void BuildToolInstructions_ThrowsForNullRegistry() => Assert.ThrowsExactly<ArgumentNullException>(() => OllamaToolPromptBuilder.BuildToolInstructions(null!));

    private sealed class ToolRunnerProtocolAdapter : IOllamaProtocolAdapter
    {
        public Task<OllamaTagsResponse> GetTagsAsync(System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public IAsyncEnumerable<OllamaGenerateResponseChunk> GenerateStreamingAsync(OllamaGenerateRequest request, System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public async IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(OllamaChatRequest request, System.Threading.CancellationToken cancellationToken = default)
        {
            yield return new OllamaChatResponseChunk
            {
                Message = new OllamaChatMessage
                {
                    Role = "assistant",
                    Content = "hello",
                },
                Done = true,
            };

            await Task.Yield();
        }

        public Task<OllamaEmbedResponse> EmbedAsync(OllamaEmbedRequest request, System.Threading.CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
