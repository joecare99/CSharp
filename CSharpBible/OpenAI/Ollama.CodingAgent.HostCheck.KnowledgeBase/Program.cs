using System;
using System.IO;
using System.Threading.Tasks;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.HostCheck.KnowledgeBase;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "ollama-coding-agent-kb",
            DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
        string dbPath = Path.Combine(root, "knowledge.json");
        Directory.CreateDirectory(root);

        LocalKnowledgeBaseStore store = new(dbPath);

        Console.WriteLine("== KnowledgeBase HostCheck ==");
        Console.WriteLine($"Database: {dbPath}");
        Console.WriteLine();

        if (args.Length > 0)
        {
            LocalWikiImportResult importResult = await new LocalWikiMarkdownImporter().ImportAsync(args[0], store);
            Console.WriteLine($"Vault: {Path.GetFullPath(args[0])}");
            Console.WriteLine($"Imported Markdown pages: {importResult.ImportedCount}");
            Console.WriteLine($"Skipped pages: {importResult.SkippedCount}");
            var vaultResults = await store.SearchAsync("dependency injection", maxResults: 5);
            Console.WriteLine($"Search hits for 'dependency injection': {vaultResults.Count}");
            foreach (LocalKnowledgeEntry result in vaultResults)
            {
                Console.WriteLine($"- {result.Id}: {result.Title}");
            }

            return 0;
        }

        LocalKnowledgeEntry entry = new()
        {
            Id = "mslearn-nullability",
            Title = "MS Learn - Nullable reference types",
            Summary = "Enable nullable and annotate reference flows clearly.",
            Source = "mslearn",
        };
        await store.AddOrUpdateAsync(entry);
        var results = await store.SearchAsync("nullable");
        Console.WriteLine($"Search hits for 'nullable': {results.Count}");
        Console.WriteLine();

        Console.WriteLine("Malformed input/output checks:");
        await TryMalformedCaseAsync(async () => await store.SearchAsync(string.Empty), "Empty query");
        await TryMalformedCaseAsync(async () =>
        {
            await store.AddOrUpdateAsync(new LocalKnowledgeEntry
            {
                Id = string.Empty,
                Title = "bad",
                Summary = "bad",
            });
        }, "Invalid entry");

        await File.WriteAllTextAsync(dbPath, "{ malformed json");
        await TryMalformedCaseAsync(async () => await store.LoadAllAsync(), "Malformed database file");

        return 0;
    }

    private static async Task TryMalformedCaseAsync(Func<Task> action, string label)
    {
        try
        {
            await action();
            Console.WriteLine($"Unexpected success: {label}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected failure ({label}): {ex.GetType().Name}");
        }
    }
}
