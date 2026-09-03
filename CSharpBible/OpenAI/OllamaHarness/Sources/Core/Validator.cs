// Core/Validator.cs
using System.Diagnostics;
using OllamaHarness.Config;

namespace OllamaHarness.Core;

public sealed class Validator(HarnessConfig config)
{
    public sealed record ValidationResult(bool Passed, string Message, TimeSpan Elapsed);

    /// <summary>
    /// Führt die kompilierte Assembly aus und prüft, ob sie innerhalb
    /// des Timeouts erfolgreich terminiert (Exit-Code 0).
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(
        string assemblyPath,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{assemblyPath}\" --validate-only",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                Environment = { ["HARNESS_VALIDATE"] = "1" }
            };

            using var proc = Process.Start(psi)!;

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(config.ValidationTimeout);

            var stdout = await proc.StandardOutput.ReadToEndAsync(cts.Token);
            var stderr = await proc.StandardError.ReadToEndAsync(cts.Token);
            await proc.WaitForExitAsync(cts.Token);

            sw.Stop();

            if (proc.ExitCode == 0)
                return new ValidationResult(true, stdout.Trim(), sw.Elapsed);

            return new ValidationResult(false,
                $"Exit {proc.ExitCode}: {stderr.Trim()}", sw.Elapsed);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            return new ValidationResult(false, "Timeout exceeded.", sw.Elapsed);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new ValidationResult(false, ex.Message, sw.Elapsed);
        }
    }

    /// <summary>
    /// Statische Analyse: Prüft ob generierter Code fundamentale Muster enthält.
    /// </summary>
    public ValidationResult StaticCheck(Dictionary<string, string> sources)
    {
        foreach (var (file, code) in sources)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new ValidationResult(false, $"{file} is empty.", TimeSpan.Zero);

            if (!code.Contains("namespace"))
                return new ValidationResult(false, $"{file} missing namespace.", TimeSpan.Zero);
        }

        return new ValidationResult(true, "Static checks passed.", TimeSpan.Zero);
    }
}