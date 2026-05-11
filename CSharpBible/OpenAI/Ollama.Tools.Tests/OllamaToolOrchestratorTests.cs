using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;
using Ollama.Tools.Tests.TestDoubles;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class OllamaToolOrchestratorTests
{
    [TestMethod]
    public async Task ExecuteAsync_ReturnsSuccessfulResultForRegisteredTool()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "ok",
            },
        ]);
        OllamaToolOrchestrator orchestrator = new(registry);

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "clock",
            Input = "now",
        });

        Assert.IsTrue(result.Success);
        Assert.AreEqual("ok:now", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsFailureForUnknownTool()
    {
        OllamaToolRegistry registry = new([]);
        OllamaToolOrchestrator orchestrator = new(registry);

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "missing",
            Input = "x",
        });

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error ?? string.Empty, "missing");
    }

    [TestMethod]
    public async Task ExecuteAsync_ReturnsFailureForInvalidArguments()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "ok",
                ValidationResult = OllamaToolValidationResult.Failure("Input must not be empty."),
            },
        ]);
        OllamaToolOrchestrator orchestrator = new(registry);

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "clock",
            Input = string.Empty,
        });

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Error ?? string.Empty, "Input must not be empty.");
        Assert.AreEqual(string.Empty, result.Output);
    }

    [TestMethod]
    public async Task RunAsync_UsesModelOutputToInvokeRegisteredTool()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "ok",
            },
        ]);
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) =>
            {
                Assert.AreEqual(2, options.Messages.Count);
                StringAssert.Contains(options.Messages[0].Content, "clock");
                Assert.AreEqual("user", options.Messages[1].Role);

                return Task.FromResult(new OllamaChatCompletion
                {
                    Content = "{\"toolName\":\"clock\",\"input\":\"now\"}",
                });
            },
        };
        OllamaToolLoopRunner runner = new(chatRunner, registry, new OllamaToolOrchestrator(registry));

        OllamaToolInvocationResult result = await runner.RunAsync("What time is it?");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("clock", result.ToolName);
        Assert.AreEqual("ok:now", result.Output);
    }

    [TestMethod]
    public async Task ExecuteAsync_UsesContentAnalysisAdapterForStructuredRequests()
    {
        TestContentAnalysisTool analysisTool = new()
        {
            Name = "analyze_text",
            Description = "Analyzes text content.",
            Result = new ContentAnalysisResult
            {
                Summary = "Looks good.",
                Score = 0.95,
            },
        };
        IOllamaTool adapter = new ContentAnalysisToolAdapter(analysisTool);
        OllamaToolRegistry registry = new([adapter]);
        OllamaToolOrchestrator orchestrator = new(registry);

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "analyze_text",
            Input = "{\"contentKind\":0,\"sourceKind\":0,\"mediaType\":\"text/plain\",\"content\":\"hello\"}",
        });

        Assert.IsTrue(result.Success);
        StringAssert.Contains(result.Output, "Looks good.");
        Assert.IsNotNull(analysisTool.LastAnalyzedRequest);
        Assert.AreEqual("hello", analysisTool.LastAnalyzedRequest.Content);
    }
}
