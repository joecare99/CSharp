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
public sealed class OllamaChatStreamReaderTests
{
    [TestMethod]
    public async Task ReadChunksAsync_ReturnsChunksFromNdjsonStream()
    {
        string content = "{\"message\":{\"role\":\"assistant\",\"content\":\"Hello\"},\"thinking\":\"T1\",\"done\":false}\n"
            + "\n"
            + "{\"message\":{\"role\":\"assistant\",\"content\":\" World\"},\"thinking\":\"T2\",\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaChatStreamReader reader = new();
        List<OllamaChatResponseChunk> chunks = [];

        await foreach (OllamaChatResponseChunk chunk in reader.ReadChunksAsync(stream))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(2, chunks.Count);
        Assert.AreEqual("assistant", chunks[0].Message?.Role);
        Assert.AreEqual("Hello", chunks[0].Message?.Content);
        Assert.AreEqual(" World", chunks[1].Message?.Content);
        Assert.IsTrue(chunks[1].Done);
    }

    [TestMethod]
    public async Task ReadChunksAsync_SkipsBlankLinesAndNullPayloads()
    {
        string content = "\nnull\n{\"message\":{\"role\":\"assistant\",\"content\":\"Hello\"},\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaChatStreamReader reader = new();
        List<OllamaChatResponseChunk> chunks = [];

        await foreach (OllamaChatResponseChunk chunk in reader.ReadChunksAsync(stream))
        {
            chunks.Add(chunk);
        }

        Assert.AreEqual(1, chunks.Count);
        Assert.AreEqual("Hello", chunks[0].Message?.Content);
    }

    [TestMethod]
    public async Task ReadChunksAsync_ThrowsForCanceledToken()
    {
        string content = "{\"message\":{\"role\":\"assistant\",\"content\":\"Hello\"},\"done\":true}\n";
        await using MemoryStream stream = new(Encoding.UTF8.GetBytes(content));
        OllamaChatStreamReader reader = new();
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsExactlyAsync<global::System.OperationCanceledException>(async () =>
        {
            await foreach (OllamaChatResponseChunk _ in reader.ReadChunksAsync(stream, cancellationTokenSource.Token))
            {
            }
        });
    }

    [TestMethod]
    public async Task ReadChunksAsync_ThrowsForNullStream()
    {
        OllamaChatStreamReader reader = new();

        await Assert.ThrowsExactlyAsync<global::System.ArgumentNullException>(async () =>
        {
            await foreach (OllamaChatResponseChunk _ in reader.ReadChunksAsync(null!, CancellationToken.None))
            {
            }
        });
    }
}
