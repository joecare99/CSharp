using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
namespace SelfImprovingHarness;
public interface ICompilerService { Task<BuildResult> BuildAsync(string projectDirectory, CancellationToken ct = default); }
public sealed class CompilerService(IOptions<HarnessOptions> opts, RunLogger log) : ICompilerService
{
    public async Task<BuildResult> BuildAsync(string projectDirectory, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew(); var psi = new ProcessStartInfo("dotnet", "build --nologo") { WorkingDirectory = projectDirectory, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        using var p = new Process { StartInfo = psi }; p.Start(); var outputTask = p.StandardOutput.ReadToEndAsync(ct); var errorTask = p.StandardError.ReadToEndAsync(ct);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct); timeout.CancelAfter(TimeSpan.FromSeconds(opts.Value.BuildTimeoutSeconds));
        try { await p.WaitForExitAsync(timeout.Token); } catch (OperationCanceledException) { try { p.Kill(true); } catch { } return new(false, "Build timeout", ["Build timed out"], sw.Elapsed); }
        var output = (await outputTask) + "\n" + (await errorTask); var errors = Regex.Matches(output, @"(?im)^.*(?:error CS\d+|error NU\d+|Build FAILED).*$").Select(m => m.Value.Trim()).Distinct().ToArray();
        var result = new BuildResult(p.ExitCode == 0, output, errors, sw.Elapsed); await log.EventAsync("build", new { projectDirectory, result.Success, result.Errors, result.Duration }); return result;
    }
}
