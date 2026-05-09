using System.Collections.Generic;
using System.IO;
using System.Text;
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
}
