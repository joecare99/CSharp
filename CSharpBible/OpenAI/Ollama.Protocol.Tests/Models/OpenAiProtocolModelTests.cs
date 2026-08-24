using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Protocol.Models.OpenAI;

namespace Ollama.Protocol.Tests.Models.OpenAI;

[TestClass]
public sealed class OpenAiProtocolModelTests
{
    [TestMethod]
    public void ChatCompletionRequest_SerializesSupportedFields()
    {
        OpenAiChatCompletionRequest request = new()
        {
            Model = "qwen3-vl:8b",
            Messages =
            [
                new OpenAiChatMessage
                {
                    Role = "user",
                    Content = new object[]
                    {
                        new OpenAiContentPart { Type = "text", Text = "What is shown?" },
                        new OpenAiContentPart
                        {
                            Type = "image_url",
                            ImageUrl = new OpenAiImageUrl { Url = "data:image/png;base64,AA==" },
                        },
                    },
                },
            ],
            MaxTokens = 300,
            Stream = true,
            StreamOptions = new OpenAiStreamOptions { IncludeUsage = true },
            Tools =
            [
                new OpenAiTool
                {
                    Function = new OpenAiFunctionDefinition { Name = "lookup" },
                },
            ],
        };

        string json = JsonSerializer.Serialize(request);

        StringAssert.Contains(json, "\"model\":\"qwen3-vl:8b\"");
        StringAssert.Contains(json, "\"image_url\"");
        StringAssert.Contains(json, "\"include_usage\":true");
        StringAssert.Contains(json, "\"tools\"");
    }

    [TestMethod]
    public void CompletionAndEmbeddingModels_DeserializeOpenAiJson()
    {
        OpenAiCompletionResponse completion = JsonSerializer.Deserialize<OpenAiCompletionResponse>(
            "{\"id\":\"cmpl-1\",\"choices\":[{\"text\":\"Hello\",\"index\":0}]}" )!;
        OpenAiEmbeddingResponse embedding = JsonSerializer.Deserialize<OpenAiEmbeddingResponse>(
            "{\"object\":\"list\",\"data\":[{\"object\":\"embedding\",\"embedding\":[0.1,0.2],\"index\":0}]}" )!;

        Assert.AreEqual("Hello", completion.Choices[0].Text);
        Assert.AreEqual(2, embedding.Data[0].Embedding.Count);
        Assert.AreEqual(0.1f, embedding.Data[0].Embedding[0]);
    }

    [TestMethod]
    public void ResponsesRequestAndEvent_SerializeSupportedFields()
    {
        OpenAiResponseRequest request = new()
        {
            Model = "qwen3:8b",
            Input = "Write a short poem",
            Instructions = "Be concise",
            MaxOutputTokens = 100,
            Stream = true,
            Tools = new List<OpenAiResponseTool>
            {
                new() { Name = "lookup" },
            },
        };
        OpenAiResponseStreamEvent streamEvent = new()
        {
            Type = "response.output_text.delta",
            Delta = "Blue",
        };

        string requestJson = JsonSerializer.Serialize(request);
        string eventJson = JsonSerializer.Serialize(streamEvent);

        StringAssert.Contains(requestJson, "\"max_output_tokens\":100");
        StringAssert.Contains(requestJson, "\"instructions\":\"Be concise\"");
        StringAssert.Contains(eventJson, "\"delta\":\"Blue\"");
    }
}
