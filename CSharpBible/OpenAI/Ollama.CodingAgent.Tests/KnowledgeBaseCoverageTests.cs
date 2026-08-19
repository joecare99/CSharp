using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class KnowledgeBaseCoverageTests
{
    [TestMethod]
    public async Task StoreAndTools_HandleValidInvalidAndPersistedEntries()
    {
        using TestWorkspace workspace = new();
        string databasePath = workspace.GetPath("knowledge.json");
        LocalKnowledgeBaseStore store = new(databasePath);

        Assert.AreEqual(databasePath, store.DatabaseFilePath);
        Assert.AreEqual(0, (await store.LoadAllAsync()).Count);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => store.SearchAsync(string.Empty));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => store.SearchAsync("term", 0));
        await store.AddOrUpdateAsync(new LocalKnowledgeEntry
        {
            Id = "entry",
            Title = "Dependency injection",
            Summary = "Inject the dependency into a constructor.",
            Tags = ["architecture"],
        });
        await store.AddOrUpdateAsync(new LocalKnowledgeEntry
        {
            Id = "ENTRY",
            Title = "Dependency injection revised",
            Summary = "A revised dependency injection guide.",
            Tags = ["architecture", "testing"],
        });
        Assert.AreEqual(1, (await store.SearchAsync("dependency injection")).Count);
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => store.AddOrUpdateAsync(new LocalKnowledgeEntry
        {
            Id = "bad",
            Title = string.Empty,
            Summary = "summary",
        }));

        LocalWikiSearchTool searchTool = new(store);
        Assert.IsFalse(searchTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(searchTool.Validate("""{"query":"term","maxResults":21}""").IsValid);
        Assert.IsTrue(searchTool.Validate("""{"query":"dependency","maxResults":1}""").IsValid);
        OllamaToolResult search = await searchTool.ExecuteAsync("""{"query":"dependency","maxResults":1}""");
        Assert.IsTrue(search.Success);
        StringAssert.Contains(search.Output, "revised");
        Assert.AreEqual("local_wiki_search", searchTool.Name);
        Assert.IsTrue(searchTool.Schema.Parameters.Count > 0);

        LocalWikiWriteTool writeTool = new(store);
        Assert.IsFalse(writeTool.Validate(string.Empty).IsValid);
        Assert.IsFalse(writeTool.Validate("""{"id":"entry","title":"title","summary":"text","citationUrl":"not a uri"}""").IsValid);
        Assert.IsFalse(writeTool.Validate($$"""{"id":"entry","title":"title","summary":"{{new string('x', 8001)}}"}""").IsValid);
        Assert.IsTrue(writeTool.Validate("""{"id":"local","title":"Local","summary":"Text"}""").IsValid);
        OllamaToolResult write = await writeTool.ExecuteAsync("""{"id":"written","title":"Written","summary":"A local entry.","tags":[" one ","one",""]}""");
        Assert.IsTrue(write.Success);
        Assert.AreEqual("local_wiki_write", writeTool.Name);
        Assert.IsTrue(writeTool.Schema.Parameters.Count > 0);
        Assert.AreEqual(2, (await store.LoadAllAsync()).Count);

        await File.WriteAllTextAsync(databasePath, string.Empty);
        Assert.AreEqual(0, (await store.LoadAllAsync()).Count);
        await File.WriteAllTextAsync(databasePath, "[null]");
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => store.LoadAllAsync());
    }

    [TestMethod]
    public async Task Importer_UsesFallbackTitlesAndSkipsEmptyPages()
    {
        using TestWorkspace workspace = new();
        string vault = workspace.GetPath("vault");
        Directory.CreateDirectory(Path.Combine(vault, "visible"));
        await File.WriteAllTextAsync(Path.Combine(vault, "visible", "heading.md"), "# Heading title\n\nbody");
        await File.WriteAllTextAsync(Path.Combine(vault, "visible", "fallback.md"), "body without a heading");
        await File.WriteAllTextAsync(Path.Combine(vault, "empty.md"), string.Empty);
        LocalKnowledgeBaseStore store = new(workspace.GetPath("database.json"));

        await Assert.ThrowsExactlyAsync<DirectoryNotFoundException>(() => new LocalWikiMarkdownImporter().ImportAsync(workspace.GetPath("missing"), store));
        LocalWikiImportResult result = await new LocalWikiMarkdownImporter().ImportAsync(vault, store);
        IReadOnlyList<LocalKnowledgeEntry> entries = await store.LoadAllAsync();

        Assert.AreEqual(2, result.ImportedCount);
        Assert.AreEqual(1, result.SkippedCount);
        Assert.IsTrue(entries.Any(entry => entry.Title == "Heading title"));
        Assert.IsTrue(entries.Any(entry => entry.Title == "fallback"));
    }

    [TestMethod]
    public void WritePolicy_CoversAllCitationOutcomes()
    {
        LocalWikiWritePolicy policy = new();
        Assert.IsFalse(policy.TryValidate(new LocalKnowledgeEntry { Id = string.Empty, Title = "title", Summary = "text" }, out string requiredError));
        StringAssert.Contains(requiredError, "required");
        Assert.IsFalse(policy.TryValidate(new LocalKnowledgeEntry { Id = "id", Title = "title", Summary = new string('x', 8001) }, out _));
        Assert.IsFalse(policy.TryValidate(new LocalKnowledgeEntry { Id = "id", Title = "title", Summary = "text", CitationUrl = "https://example.test" }, out _));
        Assert.IsTrue(policy.TryValidate(new LocalKnowledgeEntry { Id = "id", Title = "title", Summary = "text", CitationUrl = "https://learn.microsoft.com/en-us/dotnet" }, out string noError));
        Assert.AreEqual(string.Empty, noError);
    }
}
