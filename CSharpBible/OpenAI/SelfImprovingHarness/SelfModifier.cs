using System.Text.RegularExpressions;
namespace SelfImprovingHarness;
public interface ISelfModifier { Task<GenerationResult> CreateAsync(string sourceRoot, string generationId, string? repair, CancellationToken ct = default); }
public sealed class SelfModifier(IOllamaClient ollama, RunLogger log) : ISelfModifier
{
    public async Task<GenerationResult> CreateAsync(string sourceRoot, string generationId, string? repair, CancellationToken ct = default)
    {
        sourceRoot = Path.GetFullPath(sourceRoot);
        var generationsRoot = Path.Combine(sourceRoot, "generations");
        var destination = Path.GetFullPath(Path.Combine(generationsRoot, generationId));
        if (!destination.StartsWith(generationsRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal)) throw new UnauthorizedAccessException("Generation path is outside the project sandbox."); Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories).Where(f => !f.Contains(Path.DirectorySeparatorChar + "generations" + Path.DirectorySeparatorChar) && !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)))
        { var relative = Path.GetRelativePath(sourceRoot, file); var target = Path.Combine(destination, relative); Directory.CreateDirectory(Path.GetDirectoryName(target)!); var source = await File.ReadAllTextAsync(file, ct); var prompt = $"Verbessere diesen C#-Code für .NET 8: Performance, Lesbarkeit und Fehlerbehandlung. Gib ausschließlich vollständigen C#-Code zurück.\n{(repair is null ? "" : "Repariere diese Build-Fehler: " + repair)}\nDatei: {relative}\n```csharp\n{source}\n```"; string answer; try { answer = await ollama.GenerateAsync(prompt, ct); } catch { throw; } await File.WriteAllTextAsync(target, ExtractCode(answer), ct); }
        foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.csproj", SearchOption.TopDirectoryOnly)) File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
        await log.EventAsync("generation-created", new { generationId, destination }); return new(destination, generationId);
    }
    private static string ExtractCode(string text) { var match = Regex.Match(text, @"```(?:csharp|cs)?\s*(.*?)```", RegexOptions.Singleline | RegexOptions.IgnoreCase); return match.Success ? match.Groups[1].Value.Trim() + Environment.NewLine : text.Trim() + Environment.NewLine; }
}
