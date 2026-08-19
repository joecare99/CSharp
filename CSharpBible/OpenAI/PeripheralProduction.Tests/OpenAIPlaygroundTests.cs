using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenAI.Chat;
using OpenAIPlayground;
using OpenAIProgram = OpenAIPlayground.Program;

namespace PeripheralProduction.Tests;

[TestClass]
[DoNotParallelize]
public sealed class OpenAIPlaygroundTests
{
    [TestMethod]
    public async Task Main_ReportsMissingApiKeyWithoutCreatingClient()
    {
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);

        (int result, string output) = await ConsoleOutput.CaptureAsync(() => OpenAIProgram.Main([]));

        Assert.AreEqual(1, result);
        StringAssert.Contains(output, "OPENAI_API_KEY");
    }

    [TestMethod]
    public async Task Main_UsesInjectedClientAndWritesTextContent()
    {
        IOpenAIChatCompletionClient client = Substitute.For<IOpenAIChatCompletionClient>();
#pragma warning disable OPENAI001
        client.CompleteChatAsync(Arg.Any<string>())
            .Returns(Task.FromResult(OpenAIChatModelFactory.ChatCompletion(
                content: new ChatMessageContent(
                [
                    ChatMessageContentPart.CreateTextPart("answer"),
                    ChatMessageContentPart.CreateRefusalPart("ignored"),
                ]))));
#pragma warning restore OPENAI001
        OpenAIProgram.ChatClientFactory = _ => client;
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        try
        {
            (int result, string output) = await ConsoleOutput.CaptureAsync(() => OpenAIProgram.Main(["custom", "prompt"]));

            Assert.AreEqual(0, result);
            StringAssert.Contains(output, "custom prompt");
            StringAssert.Contains(output, "answer");
            await client.Received(1).CompleteChatAsync("custom prompt");
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        }
    }

    [TestMethod]
    public async Task Main_ReportsInjectedClientFailure()
    {
        OpenAIProgram.ChatClientFactory = _ => throw new InvalidOperationException("deterministic failure");
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-key");

        try
        {
            (int result, string output) = await ConsoleOutput.CaptureAsync(() => OpenAIProgram.Main([]));

            Assert.AreEqual(2, result);
            StringAssert.Contains(output, "The request failed.");
            StringAssert.Contains(output, "deterministic failure");
        }
        finally
        {
            Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        }
    }

    [TestMethod]
    public void CreateChatClient_ConstructsTheSdkAdapter()
    {
        IOpenAIChatCompletionClient client = OpenAIProgram.CreateChatClient("test-key");

        Assert.IsInstanceOfType<OpenAIChatCompletionClient>(client);
        Assert.ThrowsExactly<ArgumentNullException>(() => new OpenAIChatCompletionClient(null!));
    }

    [TestMethod]
    public async Task SdkAdapter_InvokesSubstitutedSdkClientWithoutNetwork()
    {
        ChatClient sdkClient = Substitute.For<ChatClient>("model", "test-key");
        OpenAIChatCompletionClient adapter = new(sdkClient);

        ChatCompletion completion = await adapter.CompleteChatAsync("prompt");

        Assert.IsNull(completion);
    }
}
