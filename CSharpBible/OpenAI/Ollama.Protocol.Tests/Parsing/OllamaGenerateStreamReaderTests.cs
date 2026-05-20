using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Protocol.Models;
using Ollama.Protocol.Parsing;

namespace Ollama.Protocol.Tests.Parsing;

[TestClass]
public sealed class OllamaGenerateStreamReaderTests
{
    [TestMethod]
    public async Task ReadChunksAsync_ReturnsChunksFromNdjsonStream()
    {
        string content = "{\"response\":\"Hello\",\"thinking\":\"T1\",\"done\":false}\n"
            + "\n"
            + "{\"response\":\" World\",\"thinking\":\"T2\",\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaGenerateStreamReader reader = new();
        List<OllamaGenerateResponseChunk> chunks = [];

        await foreach (OllamaGenerateResponseChunk chunk in reader.ReadChunksAsync(stream))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(2, chunks.Count);
        Assert.AreEqual("Hello", chunks[0].Response);
        Assert.AreEqual("T1", chunks[0].Thinking);
        Assert.AreEqual(" World", chunks[1].Response);
        Assert.IsTrue(chunks[1].Done);
    }

    [TestMethod]
    public async Task ReadChunksAsync_SkipsBlankLinesAndNullPayloads()
    {
        string content = "\nnull\n{\"response\":\"Hello\",\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaGenerateStreamReader reader = new();
        List<OllamaGenerateResponseChunk> chunks = [];

        await foreach (OllamaGenerateResponseChunk chunk in reader.ReadChunksAsync(stream))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(1, chunks.Count);
        Assert.AreEqual("Hello", chunks[0].Response);
    }

    [TestMethod]
    public async Task ReadChunksAsync_ThrowsForCanceledToken()
    {
        string content = "{\"response\":\"Hello\",\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaGenerateStreamReader reader = new();
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<global::System.OperationCanceledException>(async () =>
        {
            await foreach (OllamaGenerateResponseChunk _ in reader.ReadChunksAsync(stream, cancellationTokenSource.Token))
            {
            }
        });
    }

    [TestMethod]
    public async Task ReadChunksAsync_ThrowsForNullStream()
    {
        OllamaGenerateStreamReader reader = new();

        await Assert.ThrowsExactlyAsync<global::System.ArgumentNullException>(async () =>
        {
            await foreach (OllamaGenerateResponseChunk _ in reader.ReadChunksAsync(null!, CancellationToken.None))
            {
            }
        });
    }
}
