using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools.Abstractions;
using BasicChatProgram = Ollama.Samples.BasicChat.Program;
using ChatCheckProgram = Ollama.Samples.ChatCheck.Program;
using EmbedCheckProgram = Ollama.Samples.EmbedCheck.Program;
using GenerateCheckProgram = Ollama.Samples.GenerateCheck.Program;
using Service1Program = Ollama_Service1.Program;
using Service2Program = Ollama_Service2.Program;
using TagsCheckProgram = Ollama.Samples.TagsCheck.Program;
using ToolUseProgram = Ollama.Samples.ToolUse.Program;

namespace PeripheralProduction.Tests;

[TestClass]
[DoNotParallelize]
public sealed class OllamaSampleCompositionTests
{
    [TestMethod]
    public async Task Service1_Main_StreamsThinkingAndResponse()
    {
        Service1Program.HttpClientFactory = () => CreateClient(
            "{\"thinking\":\"plan\",\"done\":false}\n\nnull\n{\"response\":\"answer\",\"done\":true}\n",
            request =>
            {
                Assert.AreEqual("/api/generate", request.RequestUri?.AbsolutePath);
                return Task.CompletedTask;
            });

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => Service1Program.Main(["custom", "prompt"]));

        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "custom prompt");
        StringAssert.Contains(output, "plan");
        StringAssert.Contains(output, "Antwort:");
        StringAssert.Contains(output, "answer");
    }

    [TestMethod]
    public async Task Service1_Main_ReportsEmptyAndFailedResponses()
    {
        Service1Program.HttpClientFactory = () => CreateClient("{\"done\":true}\n");

        (int emptyResult, string emptyOutput) = await ConsoleOutput.CaptureAsync(() => Service1Program.Main([]));
        Assert.AreEqual(0, emptyResult);
        StringAssert.Contains(emptyOutput, "<keine Thinking-Ausgabe>");
        StringAssert.Contains(emptyOutput, "Keine Antwort");

        Service1Program.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => Service1Program.Main([]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Aufruf an Ollama fehlgeschlagen.");
    }

    [TestMethod]
    public async Task Service2_Main_StreamsAndReportsFailures()
    {
        Service2Program.HttpClientFactory = () => CreateClient(
            "{\"thinking\":\"plan\",\"response\":\"answer\",\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => Service2Program.Main(["custom", "prompt"]));
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "custom prompt");
        StringAssert.Contains(output, "plan");
        StringAssert.Contains(output, "answer");

        Service2Program.HttpClientFactory = () => CreateClient("\nnull\n{}\n");
        (_, string emptyOutput) = await ConsoleOutput.CaptureAsync(() => Service2Program.Main([]));
        StringAssert.Contains(emptyOutput, "<keine Thinking-Ausgabe>");
        StringAssert.Contains(emptyOutput, "Keine Antwort");

        Service2Program.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.InternalServerError);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => Service2Program.Main([]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Aufruf an Ollama fehlgeschlagen.");
    }

    [TestMethod]
    public async Task BasicChat_Main_HandlesContentAndMissingOutput()
    {
        BasicChatProgram.HttpClientFactory = () => CreateClient(
            "{\"message\":{\"role\":\"assistant\",\"content\":\"answer\"},\"thinking\":\"plan\",\"done\":true}\n",
            request =>
            {
                Assert.AreEqual("/api/chat", request.RequestUri?.AbsolutePath);
                return Task.CompletedTask;
            });

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => BasicChatProgram.Main(["hello"]));
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "Requested model");
        StringAssert.Contains(output, "plan");
        StringAssert.Contains(output, "answer");

        BasicChatProgram.HttpClientFactory = () => CreateClient("{\"done\":true}\n");
        (_, string emptyOutput) = await ConsoleOutput.CaptureAsync(() => BasicChatProgram.Main([]));
        StringAssert.Contains(emptyOutput, "<no thinking output>");
        StringAssert.Contains(emptyOutput, "<no answer output>");
    }

    [TestMethod]
    public async Task BasicChat_Main_ReportsTransportFailure()
    {
        BasicChatProgram.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => BasicChatProgram.Main([]));

        Assert.AreEqual(1, result);
        StringAssert.Contains(output, "The Ollama sample failed.");
    }

    [TestMethod]
    public async Task ChatCheck_Main_HandlesContentAndFailure()
    {
        ChatCheckProgram.HttpClientFactory = () => CreateClient(
            "{\"message\":{\"role\":\"assistant\",\"content\":\"answer\"},\"thinking\":\"plan\",\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => ChatCheckProgram.Main(["hello"]));
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "plan");
        StringAssert.Contains(output, "answer");

        ChatCheckProgram.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => ChatCheckProgram.Main([]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Chat check failed.");
    }

    [TestMethod]
    public async Task ChatCheck_Main_ReportsMissingStreamingOutput()
    {
        ChatCheckProgram.HttpClientFactory = () => CreateClient("{\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => ChatCheckProgram.Main([]));

        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "<no thinking output>");
        StringAssert.Contains(output, "<no answer output>");
    }

    [TestMethod]
    public async Task GenerateCheck_Main_HandlesContentAndFailure()
    {
        GenerateCheckProgram.HttpClientFactory = () => CreateClient(
            "{\"response\":\"answer\",\"thinking\":\"plan\",\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => GenerateCheckProgram.Main(["hello"]));
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "plan");
        StringAssert.Contains(output, "answer");

        GenerateCheckProgram.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => GenerateCheckProgram.Main([]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Generate check failed.");
    }

    [TestMethod]
    public async Task GenerateCheck_Main_ReportsMissingStreamingOutput()
    {
        GenerateCheckProgram.HttpClientFactory = () => CreateClient("{\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => GenerateCheckProgram.Main([]));

        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "<no thinking output>");
        StringAssert.Contains(output, "<no answer output>");
    }

    [TestMethod]
    public async Task EmbedCheck_Main_HandlesVectorsEmptyVectorsAndFailures()
    {
        EmbedCheckProgram.HttpClientFactory = () => CreateClient("{\"embeddings\":[[0.25]]}");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => EmbedCheckProgram.Main(["hello"]));
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "Vector count: 1");
        StringAssert.Contains(output, "First value:");

        EmbedCheckProgram.HttpClientFactory = () => CreateClient("{\"embeddings\":[]}");
        (_, string emptyOutput) = await ConsoleOutput.CaptureAsync(() => EmbedCheckProgram.Main([]));
        StringAssert.Contains(emptyOutput, "Vector count: 0");
        StringAssert.Contains(emptyOutput, "First vector length: 0");

        EmbedCheckProgram.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(() => EmbedCheckProgram.Main([]));
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Embed check failed.");
    }

    [TestMethod]
    public async Task TagsCheck_Main_HandlesModelsEmptyCollectionsAndFailures()
    {
        TagsCheckProgram.HttpClientFactory = () => CreateClient("{\"models\":[{\"name\":\"named\",\"model\":\"model\"},{\"name\":null,\"model\":null}]}");

        (int result, string output) = await ConsoleOutput.CaptureAsync(TagsCheckProgram.Main);
        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "- named (model)");
        StringAssert.Contains(output, "<unknown>");

        TagsCheckProgram.HttpClientFactory = () => CreateClient("{\"models\":[]}");
        (_, string emptyOutput) = await ConsoleOutput.CaptureAsync(TagsCheckProgram.Main);
        StringAssert.Contains(emptyOutput, "No models were returned");

        TagsCheckProgram.HttpClientFactory = () => CreateClient("failure", statusCode: HttpStatusCode.BadGateway);
        (int failureResult, string failureOutput) = await ConsoleOutput.CaptureAsync(TagsCheckProgram.Main);
        Assert.AreEqual(1, failureResult);
        StringAssert.Contains(failureOutput, "Tags check failed.");
    }

    [TestMethod]
    public async Task ToolUse_Main_InvokesClockToolThroughDeterministicHttpResponse()
    {
        ToolUseProgram.HttpClientFactory = () => CreateClient(
            "{\"message\":{\"role\":\"assistant\",\"content\":\"{\\\"toolName\\\":\\\"clock\\\",\\\"input\\\":\\\"utc\\\"}\"},\"done\":true}\n");
        Ollama.Samples.ToolUse.ClockTool.CurrentTimeProvider = static () => new DateTimeOffset(2026, 8, 15, 8, 0, 0, TimeSpan.Zero);

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => ToolUseProgram.Main(["what", "time"]));

        Assert.AreEqual(0, result);
        StringAssert.Contains(output, "Registered tools:");
        StringAssert.Contains(output, "2026-08-15T08:00:00.0000000+00:00");
    }

    [TestMethod]
    public async Task ToolUse_Main_ReportsInvalidModelResponseAndClockToolExposesContract()
    {
        ToolUseProgram.HttpClientFactory = () => CreateClient("{\"message\":{\"role\":\"assistant\",\"content\":\"null\"},\"done\":true}\n");

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => ToolUseProgram.Main([]));

        Assert.AreEqual(1, result);
        StringAssert.Contains(output, "The model did not return a valid tool call JSON object.");

        IOllamaTool tool = new Ollama.Samples.ToolUse.ClockTool();
        Assert.AreEqual("clock", tool.Name);
        StringAssert.Contains(tool.Description, "host date");
        Assert.AreEqual(1, tool.Schema.Parameters.Count);
        Assert.IsTrue(tool.Validate(string.Empty).IsValid);
    }

    [TestMethod]
    public async Task ToolUse_Main_ReportsTransportExceptions()
    {
        ToolUseProgram.HttpClientFactory = () => new HttpClient(new DeterministicHttpMessageHandler((_, _) => throw new HttpRequestException("transport failed")));

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => ToolUseProgram.Main([]));

        Assert.AreEqual(1, result);
        StringAssert.Contains(output, "Tool sample failed.");
        StringAssert.Contains(output, "transport failed");
    }

    [TestMethod]
    public void DefaultHttpClientFactories_CreateConfiguredClientsWithoutSendingRequests()
    {
        using HttpClient service1 = Service1Program.CreateHttpClient();
        using HttpClient service2 = Service2Program.CreateHttpClient();
        using HttpClient basicChat = BasicChatProgram.CreateHttpClient();
        using HttpClient chatCheck = ChatCheckProgram.CreateHttpClient();
        using HttpClient embedCheck = EmbedCheckProgram.CreateHttpClient();
        using HttpClient generateCheck = GenerateCheckProgram.CreateHttpClient();
        using HttpClient tagsCheck = TagsCheckProgram.CreateHttpClient();
        using HttpClient toolUse = ToolUseProgram.CreateHttpClient();

        Assert.AreEqual(System.Threading.Timeout.InfiniteTimeSpan, basicChat.Timeout);
        Assert.AreEqual(System.Threading.Timeout.InfiniteTimeSpan, toolUse.Timeout);
        Assert.IsNotNull(service1);
        Assert.IsNotNull(service2);
        Assert.IsNotNull(chatCheck);
        Assert.IsNotNull(embedCheck);
        Assert.IsNotNull(generateCheck);
        Assert.IsNotNull(tagsCheck);
        Assert.IsNotNull(Ollama.Samples.ToolUse.ClockTool.GetCurrentTime());
    }

    private static HttpClient CreateClient(string responseContent, Func<HttpRequestMessage, Task>? inspectRequest = null, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        DeterministicHttpMessageHandler handler = new(async (request, cancellationToken) =>
        {
            if (inspectRequest is not null)
            {
                await inspectRequest(request);
            }

            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json"),
            };
        });

        return new HttpClient(handler);
    }
}
