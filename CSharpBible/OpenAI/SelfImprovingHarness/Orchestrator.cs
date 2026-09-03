using System.Text.Json;
using Microsoft.Extensions.Options;
namespace SelfImprovingHarness;
public sealed class Orchestrator(ICompilerService compiler, ISelfModifier modifier, IFitnessEvaluator fitness, RunLogger log, IOptions<HarnessOptions> opts)
{
    public async Task RunAsync(string root, CancellationToken ct = default)
    { var baselineBuild = await compiler.BuildAsync(root, ct); var baseline = await fitness.EvaluateAsync(root, baselineBuild, ct); await log.EventAsync("baseline", baseline); var best = baseline.Score; string? repair = null;
      for (var g = 1; g <= opts.Value.MaxGenerations; g++) { var id = $"gen{g:000}"; GenerationResult candidate; try { candidate = await modifier.CreateAsync(root, id, repair, ct); } catch (Exception ex) { await log.EventAsync("ollama-error", new { id, error = ex.Message }); Console.Error.WriteLine($"Generation {id} übersprungen: {ex.Message}"); break; }
        BuildResult br = new(false, "", [], TimeSpan.Zero); FitnessResult fr = new(0, false, false, null, "not evaluated"); for (var attempt = 0; attempt <= opts.Value.MaxRepairAttempts; attempt++) { br = await compiler.BuildAsync(candidate.Path, ct); fr = await fitness.EvaluateAsync(candidate.Path, br, ct); if (br.Success) break; repair = string.Join("\n", br.Errors); if (attempt < opts.Value.MaxRepairAttempts) { candidate = await modifier.CreateAsync(root, id + $"-repair{attempt + 1}", repair, ct); } } await log.EventAsync("candidate", new { id, fr.Score, accepted = fr.Score > best + opts.Value.MinImprovement });
        if (fr.Score > best + opts.Value.MinImprovement) { best = fr.Score; await BackupAsync(root, ct); await File.WriteAllTextAsync(Path.Combine(root, "state.json"), JsonSerializer.Serialize(new { current = candidate.Path, score = best, updated = DateTimeOffset.UtcNow }, new JsonSerializerOptions { WriteIndented = true }), ct); Console.WriteLine($"Akzeptiert: {candidate.Path} (Fitness {best:F3})"); } else Console.WriteLine($"Verworfen: {candidate.Path} (Fitness {fr.Score:F3})"); repair = null; }
    }
    private static async Task BackupAsync(string root, CancellationToken ct) { var backup = Path.Combine(root, "backups", DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss")); Directory.CreateDirectory(backup); foreach (var f in Directory.EnumerateFiles(root, "*.cs", SearchOption.TopDirectoryOnly)) File.Copy(f, Path.Combine(backup, Path.GetFileName(f)), true); await File.WriteAllTextAsync(Path.Combine(backup, "backup-marker.txt"), "Backup before swap\n", ct); }
}
