using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.Tools;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class LocalWikiQualityTests
{
    [TestMethod]
    public async Task SearchAsync_RanksTitleAndTagMatchesBeforeSummaryOnlyMatches()
    {
        string filePath = CreateTempPath();
        try
        {
            LocalKnowledgeBaseStore store = new(filePath);
            await store.AddOrUpdateAsync(new LocalKnowledgeEntry
            {
                Id = "weak",
                Title = "General notes",
                Summary = "This mentions dependency injection as a secondary concept.",
            });
            await store.AddOrUpdateAsync(new LocalKnowledgeEntry
            {
                Id = "strong",
                Title = "Dependency Injection",
                Summary = "Use dependency injection for testable services.",
                Tags = ["architecture", "dependency-injection"],
            });

            var results = await store.SearchAsync("dependency injection");

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("strong", results[0].Id);
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    [TestMethod]
    public void Validate_RejectsUntrustedCitation()
    {
        LocalWikiWriteTool tool = new(new LocalKnowledgeBaseStore(CreateTempPath()));

        OllamaToolValidationResult validation = tool.Validate(
            """{"id":"entry","title":"Example","summary":"Curated text","citationUrl":"https://example.com/reference"}""");

        Assert.IsFalse(validation.IsValid);
        StringAssert.Contains(string.Join(" ", validation.Errors), "allow-listed");
    }

    [TestMethod]
    public void Validate_AcceptsLocalEntryWithoutCitation()
    {
        LocalWikiWriteTool tool = new(new LocalKnowledgeBaseStore(CreateTempPath()));

        OllamaToolValidationResult validation = tool.Validate(
            """{"id":"entry","title":"Example","summary":"Curated text","tags":["csharp"," csharp "]}""");

        Assert.IsTrue(validation.IsValid);
    }

    [TestMethod]
    public async Task ImportAsync_ImportsMarkdownVaultPagesForWikiSearch()
    {
        string vaultPath = Path.Combine(Path.GetTempPath(), $"ollama-vault-{Guid.NewGuid():N}");
        string databasePath = Path.Combine(Path.GetTempPath(), $"ollama-wiki-{Guid.NewGuid():N}.json");
        try
        {
            Directory.CreateDirectory(Path.Combine(vaultPath, "how-tos"));
            await File.WriteAllTextAsync(
                Path.Combine(vaultPath, "how-tos", "dependency-injection.md"),
                "---\ntitle: Use dependency injection\ntags: [how-to, csharp]\n---\n\n# Summary\n\nInject services through constructors.");
            LocalKnowledgeBaseStore store = new(databasePath);

            LocalWikiImportResult result = await new LocalWikiMarkdownImporter().ImportAsync(vaultPath, store);
            var matches = await store.SearchAsync("dependency injection");

            Assert.AreEqual(1, result.ImportedCount);
            Assert.AreEqual("how-tos/dependency-injection.md", matches[0].Id);
            Assert.AreEqual("how-tos", matches[0].Tags[0]);
            CollectionAssert.Contains(matches[0].Tags, "how-to");
        }
        finally
        {
            if (Directory.Exists(vaultPath))
            {
                Directory.Delete(vaultPath, recursive: true);
            }

            DeleteTempFile(databasePath);
        }
    }

    private static string CreateTempPath()
        => Path.Combine(Path.GetTempPath(), $"ollama-wiki-{Guid.NewGuid():N}.json");

    private static void DeleteTempFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
