using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Interfaces;
using Ollama.Client.Services;
using Ollama.Client.Models;
using Ollama.Protocol.Models;
using Ollama.Tools.Tests.TestDoubles;
using System.Threading;

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
        OllamaChatClient chatClient = new(adapter, "qwen3.5:4b");
        OllamaToolChatRunner runner = new(chatClient);

        OllamaChatCompletion completion = await runner.CompleteChatAsync(new Ollama.Client.Models.ChatCompletionOptions
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
        int completionCallCount = 0;
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) =>
            {
                completionCallCount++;
                return Task.FromResult(new OllamaChatCompletion
                {
                    Content = "not json",
                });
            },
        };
        OllamaToolLoopRunner runner = new(chatRunner, registry, new OllamaToolOrchestrator(registry));

        OllamaToolInvocationResult result = await runner.RunAsync("hello");

        Assert.AreEqual(1, completionCallCount);
        Assert.IsFalse(result.Success);
        Assert.AreEqual(string.Empty, result.ToolName);
        Assert.AreEqual("hello", result.Input);
        Assert.AreEqual(string.Empty, result.Output);
        Assert.AreEqual("The model did not return a valid tool call JSON object.", result.Error);
    }

    [TestMethod]
    public async Task RunAsync_ReturnsFailureWhenToolNameIsMissing()
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

        public async IAsyncEnumerable<OllamaChatResponseChunk> ChatStreamingAsync(OllamaChatRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken = default)
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

        public Task<OllamaPsResponse> GetRunningModelsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new OllamaPsResponse
            {
                Models =
                [
                    new OllamaRunningModel
                    {
                        Name = "model-1",
                    },
                ],
            });
        }
    }
}
