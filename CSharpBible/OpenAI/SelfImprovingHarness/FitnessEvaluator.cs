using System.Diagnostics;
using Microsoft.Extensions.Options;
namespace SelfImprovingHarness;

public interface IFitnessEvaluator { Task<FitnessResult> EvaluateAsync(string projectDirectory, BuildResult build, CancellationToken ct = default); }
public sealed class FitnessEvaluator(IOptions<HarnessOptions> opts, RunLogger log) : IFitnessEvaluator
{
    public async Task<FitnessResult> EvaluateAsync(string dir, BuildResult build, CancellationToken ct = default)
    {
        if (!build.Success) return new(0, false, false, null, string.Join(" | ", build.Errors)); var smoke = await RunSmokeAsync(dir, ct); double? ms = null; if (smoke) { var sw = Stopwatch.StartNew(); for (var i = 0; i < opts.Value.BenchmarkIterations; i++) _ = Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories).Count(); ms = sw.Elapsed.TotalMilliseconds / Math.Max(1, opts.Value.BenchmarkIterations); }
        var score = smoke ? 100.0 + (ms is > 0 ? 1.0 / ms.Value : 0) : 50;
        var r = new FitnessResult(score, true, smoke, ms, smoke ? "Build und Smoke-Test erfolgreich" : "Smoke-Test fehlgeschlagen"); await log.EventAsync("fitness", new { dir, r.Score, r.BuildPassed, r.SmokePassed, r.BenchmarkMs }); 
        return r;
    }
    private static async Task<bool> RunSmokeAsync(string dir, CancellationToken ct) { var dll = Path.Combine(dir, "bin", "Debug", "net8.0", "SelfImprovingHarness.dll"); if (!File.Exists(dll)) return false; var psi = new ProcessStartInfo("dotnet", $"\"{dll}\" --smoke-test") { WorkingDirectory = dir, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false }; using var p = Process.Start(psi)!; await p.WaitForExitAsync(ct); return p.ExitCode == 0; }
}
