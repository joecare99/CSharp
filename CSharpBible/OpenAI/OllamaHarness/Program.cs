// Program.cs
using OllamaHarness.Bootstrap;
using OllamaHarness.Config;

// ─── Validation-only mode (wird vom Validator aufgerufen) ───
if (args.Contains("--validate-only") ||
    Environment.GetEnvironmentVariable("HARNESS_VALIDATE") == "1")
{
    Console.WriteLine("VALIDATION_OK");
    return 0;
}

// ─── Normaler Bootstrap-Modus ───
var config = new HarnessConfig
{
    OllamaBaseUrl = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://cachyos-x8664.fritz.box:11434",
    Model = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen3.8:27b",
    SourceDirectory = Path.Combine(AppContext.BaseDirectory, "Sources"),
    StagingDirectory = Path.Combine(AppContext.BaseDirectory, "staging"),
    OutputDirectory = Path.Combine(AppContext.BaseDirectory, "output"),
    MaxIterations = int.TryParse(
        Environment.GetEnvironmentVariable("MAX_ITER"), out var mi) ? mi : 10
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var optimizer = new SelfOptimizer(config);

try
{
    await optimizer.RunAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n  ⚠ Cancelled by user.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\n  FATAL: {ex}");
    return 1;
}

return 0;