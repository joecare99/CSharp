using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent;
using OpenAI.CodingAgent;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class LlmTrafficLoggingTests
{
    [TestMethod]
    public void FileLogger_RedactsCredentialsAndUsesSessionLogPath()
    {
        string workspace = CreateWorkspace();
        try
        {
            FileLlmTrafficLogger logger = new(
                new FileLlmTrafficLogOptions(
                    workspace,
                    "TestVendor",
                    "TestApplication",
                    new DateTimeOffset(2026, 8, 18, 2, 45, 58, TimeSpan.Zero)),
                "session-1");
            logger.LogRequest(
                "openai-compatible",
                new Uri("https://user:password@example.test/v1/chat"),
                "chat.completions",
                "{\"api_key\":\"secret-key\",\"prompt\":\"hello\"}",
                new Dictionary<string, string>
                {
                    ["Authorization"] = "Bearer secret-token",
                });

            string content = File.ReadAllText(logger.LogFilePath);
            StringAssert.Contains(content, "hello");
            StringAssert.Contains(content, "[REDACTED]");
            Assert.IsFalse(content.Contains("secret-key", StringComparison.Ordinal));
            Assert.IsFalse(content.Contains("secret-token", StringComparison.Ordinal));
            Assert.IsFalse(content.Contains("user:password", StringComparison.Ordinal));
            StringAssert.Contains(logger.LogFilePath, Path.Combine("TestVendor", "TestApplication", "Logs"));
            StringAssert.Contains(logger.LogFilePath, "20260818T024558Z-session-1.jsonl");
        }
        finally
        {
            Directory.Delete(workspace, recursive: true);
        }
    }

    [TestMethod]
    public async Task OpenAiClient_RecordsRequestResponseAndFailure()
    {
        ILlmTrafficLogger logger = Substitute.For<ILlmTrafficLogger>();
        RecordingHandler handler = new(HttpStatusCode.OK, "{\"choices\":[{\"message\":{\"content\":\"hello\"}}]}");
        using HttpClient httpClient = new(handler);
        OpenAICompatibleChatModelClient client = new(
            httpClient,
            new OpenAICompatibleClientOptions(new Uri("https://example.test/v1/"), "model", "secret"),
            logger);

        AgentCompletion completion = await client.CompleteDetailedAsync([new AgentMessage("user", "hello")]);

        Assert.AreEqual("hello", completion.Content);
        logger.Received(1).LogRequest(
            "openai-compatible",
            Arg.Any<Uri>(),
            "chat.completions",
            Arg.Is<string>(payload => payload.Contains("hello", StringComparison.Ordinal)),
            Arg.Is<IReadOnlyDictionary<string, string>>(headers => headers["Authorization"].Contains("secret", StringComparison.Ordinal)));
        logger.Received(1).LogResponse("openai-compatible", Arg.Any<Uri>(), "chat.completions", 200, Arg.Any<string>());

        handler.StatusCode = HttpStatusCode.BadRequest;
        await Assert.ThrowsExactlyAsync<HttpRequestException>(() => client.CompleteDetailedAsync([new AgentMessage("user", "fail")]))
            .ConfigureAwait(false);
        logger.Received(1).LogFailure(
            "openai-compatible",
            Arg.Any<Uri>(),
            "chat.completions",
            Arg.Any<HttpRequestException>(),
            Arg.Any<string>());
    }

    private static string CreateWorkspace()
    {
        string path = Path.Combine(Path.GetTempPath(), "coding-agent-log-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _response;

        public RecordingHandler(HttpStatusCode statusCode, string response)
        {
            StatusCode = statusCode;
            _response = response;
        }

        public HttpStatusCode StatusCode { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(StatusCode)
            {
                Content = new StringContent(_response),
            });
    }
}
