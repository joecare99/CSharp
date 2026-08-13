using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.Tools.Tests;

[TestClass]
public sealed class JsonSessionMemoryStoreTests
{
    [TestMethod]
    public async Task RecallAsync_ReturnsRelevantEntriesAndPersistsAcrossInstances()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"ollama-memory-{Guid.NewGuid():N}.json");
        try
        {
            JsonSessionMemoryStore firstStore = new(filePath, maximumEntriesPerSession: 3);
            await firstStore.RememberAsync("session-a", "The build uses net8.0.");
            await firstStore.RememberAsync("session-a", "The API endpoint is local.");
            await firstStore.RememberAsync("session-b", "The build uses net8.0.");

            JsonSessionMemoryStore secondStore = new(filePath, maximumEntriesPerSession: 3);
            var results = await secondStore.RecallAsync("session-a", "net8.0 build target");

            Assert.AreEqual(1, results.Count);
            StringAssert.Contains(results[0].Content, "net8.0");
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [TestMethod]
    public async Task RememberAsync_TrimsOldEntriesPerSession()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"ollama-memory-{Guid.NewGuid():N}.json");
        try
        {
            JsonSessionMemoryStore store = new(filePath, maximumEntriesPerSession: 2);
            await store.RememberAsync("session-a", "first item");
            await store.RememberAsync("session-a", "second item");
            await store.RememberAsync("session-a", "third item");

            var results = await store.RecallAsync("session-a", "item", maximumResults: 10);

            Assert.AreEqual(2, results.Count);
            Assert.IsFalse(results[0].Content.Contains("first", StringComparison.Ordinal));
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
