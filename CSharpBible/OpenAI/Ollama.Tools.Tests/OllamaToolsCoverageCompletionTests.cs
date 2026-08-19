using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.Client.Models;
using Ollama.Tools.Abstractions;
using Ollama.Tools.ContentAnalysis;
using Ollama.Tools.Tests.TestDoubles;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class OllamaToolsCoverageCompletionTests
{
    [TestMethod]
    public void JsonSessionMemoryStore_RejectsNonPositiveEntryLimit()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new JsonSessionMemoryStore("memory.json", 0));
    }

    [TestMethod]
    public async Task JsonSessionMemoryStore_RejectsNonPositiveRecallLimit()
    {
        JsonSessionMemoryStore store = new("memory.json");

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => store.RecallAsync("session", "query", 0));
    }

    [TestMethod]
    public void OllamaToolLoopRunner_RejectsNullDependencies()
    {
        OllamaToolRegistry registry = new([]);
        OllamaToolOrchestrator orchestrator = new(registry);
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) => Task.FromResult(new OllamaChatCompletion()),
        };

        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaToolLoopRunner(null!, registry, orchestrator));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaToolLoopRunner(chatRunner, null!, orchestrator));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaToolLoopRunner(chatRunner, registry, null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new OllamaToolOrchestrator(null!));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public async Task RunToCompletionAsync_RejectsNonPositiveIterationLimit(int maximumIterations)
    {
        OllamaToolRegistry registry = new([]);
        OllamaToolLoopRunner runner = new(
            CreateChatRunner("No tool call."),
            registry,
            new OllamaToolOrchestrator(registry));

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => runner.RunToCompletionAsync("hello", maximumIterations));
    }

    [TestMethod]
    public async Task RunToCompletionAsync_ReturnsIterationLimitResult()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "12:00",
            },
        ]);
        OllamaToolLoopRunner runner = new(
            CreateChatRunner("{\"toolName\":\"clock\",\"input\":\"now\"}"),
            registry,
            new OllamaToolOrchestrator(registry));

        OllamaToolLoopResult result = await runner.RunToCompletionAsync("What time is it?", maximumIterations: 1);

        Assert.IsFalse(result.Completed);
        StringAssert.Contains(result.FinalResponse, "iteration limit");
        Assert.AreEqual(1, result.Invocations.Count);
    }

    [TestMethod]
    public async Task RunToCompletionAsync_ReinjectsFailedToolResultAndStopsForMissingToolName()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "12:00",
                ValidationResult = OllamaToolValidationResult.Failure("The clock is unavailable."),
            },
        ]);
        int callCount = 0;
        TestOllamaToolChatRunner chatRunner = new()
        {
            CompleteChatAsyncHandler = (options, cancellationToken) =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return Task.FromResult(new OllamaChatCompletion
                    {
                        Content = "{\"toolName\":\"clock\",\"input\":\"now\"}",
                    });
                }

                Assert.AreEqual("Tool execution failed: The clock is unavailable.", options.Messages[3].Content);
                return Task.FromResult(new OllamaChatCompletion
                {
                    Content = "{\"toolName\":\"\",\"input\":\"ignored\"}",
                });
            },
        };
        OllamaToolLoopRunner runner = new(chatRunner, registry, new OllamaToolOrchestrator(registry));

        OllamaToolLoopResult result = await runner.RunToCompletionAsync("What time is it?");

        Assert.IsTrue(result.Completed);
        Assert.AreEqual("{\"toolName\":\"\",\"input\":\"ignored\"}", result.FinalResponse);
        Assert.AreEqual(1, result.Invocations.Count);
    }

    [TestMethod]
    public async Task RunToCompletionAsync_StopsWhenTheModelReturnsJsonNull()
    {
        OllamaToolRegistry registry = new([]);
        OllamaToolLoopRunner runner = new(
            CreateChatRunner("null"),
            registry,
            new OllamaToolOrchestrator(registry));

        OllamaToolLoopResult result = await runner.RunToCompletionAsync("hello");

        Assert.IsTrue(result.Completed);
        Assert.AreEqual("null", result.FinalResponse);
        Assert.AreEqual(0, result.Invocations.Count);
    }

    [TestMethod]
    public async Task Orchestrator_UsesDefaultDenialReasonWhenPolicyOmitsOne()
    {
        IOllamaTool tool = CreateToolSubstitute("clock");
        OllamaToolRegistry registry = new([tool]);
        IOllamaToolExecutionPolicy policy = Substitute.For<IOllamaToolExecutionPolicy>();
        policy.Evaluate(tool).Returns(new OllamaToolPolicyDecision
        {
            IsAllowed = false,
        });
        OllamaToolOrchestrator orchestrator = new(registry, policy);

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "clock",
            Input = "now",
        });

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Tool execution was denied by policy.", result.Error);
    }

    [TestMethod]
    public async Task Orchestrator_PropagatesFailedToolOutput()
    {
        IOllamaTool tool = CreateToolSubstitute("clock");
        tool.ExecuteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new OllamaToolResult
        {
            Success = false,
            Output = "The clock failed.",
        }));
        OllamaToolRegistry registry = new([tool]);
        OllamaToolOrchestrator orchestrator = new(registry, new OllamaToolAllowlistPolicy(["clock"]));

        OllamaToolInvocationResult result = await orchestrator.ExecuteAsync(new OllamaToolCall
        {
            ToolName = "clock",
            Input = "now",
        });

        Assert.IsFalse(result.Success);
        Assert.AreEqual("The clock failed.", result.Error);
    }

    [TestMethod]
    public void PromptBuilder_DescribesOptionalParameters()
    {
        OllamaToolRegistry registry = new([
            new TestTool
            {
                Name = "clock",
                Description = "Returns the current time.",
                ResultText = "12:00",
                Schema = new OllamaToolSchema
                {
                    Parameters =
                    [
                        new OllamaToolParameter
                        {
                            Name = "timezone",
                            Description = "An optional timezone.",
                            Required = false,
                        },
                    ],
                },
            },
        ]);

        string instructions = OllamaToolPromptBuilder.BuildToolInstructions(registry);

        StringAssert.Contains(instructions, "timezone (string, optional)");
    }

    [TestMethod]
    public void RequestValidator_ReportsEmptyMediaTypeForInlineImage()
    {
        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(new ContentAnalysisRequest
        {
            ContentKind = OllamaContentKind.Image,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = string.Empty,
            Content = "image content",
        });

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "mediaType.required"));
        Assert.IsTrue(result.Issues.Any(static issue => issue.Code == "sourceKind.image.filePath.required"));
    }

    [TestMethod]
    public void RequestValidator_AcceptsUnknownSourceKindWithoutSourceSpecificValidation()
    {
        ContentAnalysisRequestValidationResult result = ContentAnalysisRequestValidator.Validate(new ContentAnalysisRequest
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = (OllamaContentSourceKind)999,
            MediaType = "text/plain",
        });

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task AnalyzePdfAsync_UsesFailureAndFallbackDisplayNamePaths()
    {
        IPdfTextExtractor failingExtractor = Substitute.For<IPdfTextExtractor>();
        failingExtractor.ExtractAsync(Arg.Any<PdfExtractionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new PdfExtractionResult
            {
                IsSuccessful = false,
            }));
        ContentAnalysisRouter failingRouter = new(new TextAnalysisTool(), new CSharpCodeAnalysisTool(), failingExtractor);

        InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => failingRouter.AnalyzePdfAsync("missing.pdf"));
        Assert.AreEqual("PDF extraction failed.", exception.Message);

        IPdfTextExtractor successfulExtractor = Substitute.For<IPdfTextExtractor>();
        successfulExtractor.ExtractAsync(Arg.Any<PdfExtractionRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(PdfExtractionResult.Success(
                "source.pdf",
                "This is a concise sample. It has two sentences and enough words for a basic analysis.")));
        ContentAnalysisRouter successfulRouter = new(new TextAnalysisTool(), new CSharpCodeAnalysisTool(), successfulExtractor);

        ContentAnalysisExecutionResult result = await successfulRouter.AnalyzePdfAsync("fallback-name.pdf");

        Assert.AreEqual(OllamaContentKind.Text, result.Decision.ContentKind);
    }

    [TestMethod]
    public async Task PdfTextExtractor_HandlesInputGuardsAndAbsentMetadata()
    {
        PdfPigTextExtractor extractor = new();

        PdfExtractionResult emptyPathResult = await extractor.ExtractAsync(new PdfExtractionRequest());
        PdfExtractionResult missingFileResult = await extractor.ExtractAsync(new PdfExtractionRequest
        {
            FilePath = "Ollama.Tools.Tests\\TestResults\\missing.pdf",
        });
        string filePath = CreateArtifactPath(".pdf");
        await File.WriteAllBytesAsync(filePath, [0x00, 0x01, 0x02]);

        try
        {
            PdfExtractionResult passwordResult = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = filePath,
                Password = "secret",
            });
            string requestJson = JsonSerializer.Serialize(new
            {
                filePath,
                fileMetadata = (object?)null,
            });
            ContentAnalysisRequest request = JsonSerializer.Deserialize<ContentAnalysisRequest>(requestJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            })
                ?? throw new AssertFailedException("The serialized request was not deserialized.");
            PdfExtractionResult invalidPdfResult = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = request.FilePath,
                FileMetadata = request.FileMetadata,
            });
            PdfExtractionResult suppliedMetadataResult = await extractor.ExtractAsync(new PdfExtractionRequest
            {
                FilePath = filePath,
                FileMetadata = new ContentAnalysisFileMetadata
                {
                    FileName = string.Empty,
                    Extension = ".custom",
                },
            });
            MethodInfo createFileMetadata = typeof(PdfPigTextExtractor).GetMethod(
                "CreateFileMetadata",
                BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new AssertFailedException("The file metadata factory was not found.");
            ContentAnalysisFileMetadata missingFileMetadata = (ContentAnalysisFileMetadata)(createFileMetadata.Invoke(
                null,
                ["Ollama.Tools.Tests\\TestResults\\missing.pdf", null])
                ?? throw new AssertFailedException("The file metadata factory returned null."));

            Assert.IsFalse(emptyPathResult.IsSuccessful);
            Assert.IsFalse(missingFileResult.IsSuccessful);
            Assert.IsFalse(passwordResult.IsSuccessful);
            Assert.IsFalse(invalidPdfResult.IsSuccessful);
            Assert.IsFalse(suppliedMetadataResult.IsSuccessful);
            Assert.IsNotNull(invalidPdfResult.FileMetadata);
            Assert.IsNull(missingFileMetadata.SizeBytes);
            Assert.IsNull(missingFileMetadata.LastWriteTimeUtc);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public async Task CSharpAnalysis_ReportsBalancedCodeAndMissingUsingDirectives()
    {
        CSharpCodeAnalysisTool tool = new();

        ContentAnalysisResult balancedResult = await tool.AnalyzeAsync(CreateCSharpRequest("namespace Demo;\npublic void Run() { }"));
        ContentAnalysisResult noUsingResult = await tool.AnalyzeAsync(CreateCSharpRequest(
            string.Join("\n", Enumerable.Repeat("public void Run() { }", 11))));

        Assert.IsTrue(balancedResult.Findings.Any(static finding => finding.Title == "Balanced C# source structure"));
        Assert.IsTrue(noUsingResult.Findings.Any(static finding => finding.Title == "No using directives detected"));
    }

    [TestMethod]
    public async Task TextAnalysis_RecognizesUnixParagraphSeparators()
    {
        TextAnalysisTool tool = new();
        string content = string.Join("\n\n", Enumerable.Repeat("word", 90));

        ContentAnalysisResult result = await tool.AnalyzeAsync(new ContentAnalysisRequest
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = content,
        });
        string windowsContent = string.Join(Environment.NewLine + Environment.NewLine, Enumerable.Repeat("word", 90));
        ContentAnalysisResult windowsResult = await tool.AnalyzeAsync(new ContentAnalysisRequest
        {
            ContentKind = OllamaContentKind.Text,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/plain",
            Content = windowsContent,
        });

        Assert.IsFalse(result.Findings.Any(static finding => finding.Title == "Missing paragraph structure"));
        Assert.IsFalse(windowsResult.Findings.Any(static finding => finding.Title == "Missing paragraph structure"));
    }

    private static TestOllamaToolChatRunner CreateChatRunner(string completionContent)
    {
        return new TestOllamaToolChatRunner
        {
            CompleteChatAsyncHandler = (options, cancellationToken) => Task.FromResult(new OllamaChatCompletion
            {
                Content = completionContent,
            }),
        };
    }

    private static IOllamaTool CreateToolSubstitute(string name)
    {
        IOllamaTool tool = Substitute.For<IOllamaTool>();
        tool.Name.Returns(name);
        tool.Description.Returns("A test tool.");
        tool.Schema.Returns(new OllamaToolSchema());
        tool.Validate(Arg.Any<string>()).Returns(OllamaToolValidationResult.Success());
        tool.ExecuteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new OllamaToolResult
        {
            Success = true,
            Output = "ok",
        }));
        return tool;
    }

    private static ContentAnalysisRequest CreateCSharpRequest(string content)
    {
        return new ContentAnalysisRequest
        {
            ContentKind = OllamaContentKind.SourceCode,
            SourceKind = OllamaContentSourceKind.Inline,
            MediaType = "text/x-csharp",
            Language = "csharp",
            Content = content,
        };
    }

    private static string CreateArtifactPath(string extension)
    {
        const string ArtifactDirectory = "Ollama.Tools.Tests\\TestResults\\CoverageArtifacts";
        Directory.CreateDirectory(ArtifactDirectory);
        return Path.Combine(ArtifactDirectory, Guid.NewGuid().ToString("N") + extension);
    }
}
