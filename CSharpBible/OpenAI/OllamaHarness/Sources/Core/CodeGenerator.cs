// Core/CodeGenerator.cs
using System.Text;
using OllamaHarness.Config;

namespace OllamaHarness.Core;

public sealed class CodeGenerator(OllamaClient ollama, HarnessConfig config)
{
    private const string SystemPrompt = """
        You are a senior C# architect specializing in .NET 10.
        You receive the FULL source code of a self-optimizing harness.
        Your task: produce an IMPROVED version of the requested file(s).
        
        Rules:
        - Output ONLY valid C# code, no markdown fences, no commentary.
        - Preserve the public API surface unless explicitly asked to change it.
        - Target C# 13 / .NET 10 features (primary constructors, collection expressions, etc.).
        - Improve: performance, clarity, error resilience, modularity.
        - Keep all namespace and using directives correct.
        - If you cannot improve a file meaningfully, return it unchanged.
        """;

    public async Task<string> ImproveFileAsync(
        string fileName,
        string currentSource,
        string[] goals,
        CancellationToken ct = default)
    {
        var prompt = BuildPrompt(fileName, currentSource, goals);

        var raw = await ollama.GenerateAsync(
            prompt,
            system: SystemPrompt,
            temperature: config.Temperature,
            ct: ct);

        return ExtractCode(raw);
    }

    public async Task<Dictionary<string, string>> ImproveProjectAsync(
        Dictionary<string, string> sources,
        CancellationToken ct = default)
    {
        var result = new Dictionary<string, string>();

        foreach (var (file, code) in sources)
        {
            Console.WriteLine($"  ▸ Optimizing {file}...");
            var improved = await ImproveFileAsync(file, code, config.OptimizationGoals, ct);
            result[file] = improved;
        }

        return result;
    }

    public async Task<string> GenerateNewModuleAsync(
        string description,
        string existingContext,
        CancellationToken ct = default)
    {
        var prompt = $"""
            Existing project context (namespaces, key types):
            {existingContext}

            Generate a NEW C# file that implements:
            {description}

            Requirements:
            - namespace OllamaHarness.Generated
            - .NET 10, C# 13 idioms
            - Include XML doc comments
            - Output ONLY the C# code
            """;

        var raw = await ollama.GenerateAsync(prompt, SystemPrompt, config.Temperature, ct);
        return ExtractCode(raw);
    }

    private static string BuildPrompt(string fileName, string source, string[] goals)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"File: {fileName}");
        sb.AppendLine($"Optimization goals: {string.Join(", ", goals)}");
        sb.AppendLine();
        sb.AppendLine("Current source:");
        sb.AppendLine("```");
        sb.AppendLine(source);
        sb.AppendLine("```");
        sb.AppendLine();
        sb.AppendLine("Produce the improved version of this file. Output ONLY C# code.");
        return sb.ToString();
    }

    private static string ExtractCode(string raw)
    {
        // Strip markdown fences if the model adds them despite instructions
        var trimmed = raw.Trim();
        if (trimmed.StartsWith("```csharp", StringComparison.OrdinalIgnoreCase))
            trimmed = trimmed["```csharp".Length..];
        else if (trimmed.StartsWith("```", StringComparison.Ordinal))
            trimmed = trimmed[3..];

        if (trimmed.EndsWith("```", StringComparison.Ordinal))
            trimmed = trimmed[..^3];

        return trimmed.Trim();
    }
}