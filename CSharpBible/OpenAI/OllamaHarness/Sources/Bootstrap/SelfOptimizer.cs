// Bootstrap/SelfOptimizer.cs
using OllamaHarness.Config;
using OllamaHarness.Core;

namespace OllamaHarness.Bootstrap;

public sealed class SelfOptimizer
{
    private readonly HarnessConfig _config;
    private readonly OllamaClient _ollama;
    private readonly CodeGenerator _generator;
    private readonly Compiler _compiler;
    private readonly Validator _validator;
    private readonly VersionManager _versions;

    public SelfOptimizer(HarnessConfig config)
    {
        _config = config;
        _ollama = new OllamaClient(new HttpClient() { Timeout=new TimeSpan(0,10,0) }, config.OllamaBaseUrl, config.Model);
        _generator = new CodeGenerator(_ollama, config);
        _compiler = new Compiler(config);
        _validator = new Validator(config);
        _versions = new VersionManager(config.OutputDirectory);
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║   OLLAMA SELF-OPTIMIZING HARNESS v1.0    ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine();

        // Phase 0: Health check
        if (!await _ollama.IsAliveAsync(ct))
            throw new InvalidOperationException(
                $"Ollama not reachable at {_config.OllamaBaseUrl}");

        Console.WriteLine($"  ✓ Ollama connected ({_config.Model})");
        _versions.Initialize();

        // Phase 1: Read own sources
        var sources = ReadOwnSources();
        Console.WriteLine($"  ✓ Loaded {sources.Count} source files");

        // Phase 2: Save baseline (v0)
        _versions.SaveVersion(0, sources, "Initial baseline");

        // Phase 3: Iterative improvement loop
        for (int i = 1; i <= _config.MaxIterations; i++)
        {
            ct.ThrowIfCancellationRequested();
            Console.WriteLine($"\n{'═'} Iteration {i}/{_config.MaxIterations} {'═'}");

            try
            {
                var improved = await RunIterationAsync(i, sources, ct);

                if (improved is null)
                {
                    Console.WriteLine("  ⚠ No valid improvement produced. Stopping.");
                    break;
                }

                sources = improved;
                _versions.SaveVersion(i, sources, $"Iteration {i}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Iteration {i} failed: {ex.Message}");
                Console.WriteLine("    Rolling back to last good version...");
                _versions.Rollback(i - 1, _config.SourceDirectory);
                sources = _versions.LoadVersion(i - 1)!;
            }
        }

        Console.WriteLine("\n  ✓ Optimization complete.");
        Console.WriteLine($"    Final iteration: {_versions.CurrentIteration}");
    }

    private async Task<Dictionary<string, string>?> RunIterationAsync(
        int iteration,
        Dictionary<string, string> currentSources,
        CancellationToken ct)
    {
        // Step A: Ask Ollama to improve
        Console.WriteLine("  [1/4] Generating improved code...");
        var improved = await _generator.ImproveProjectAsync(currentSources, ct);

        // Step B: Static validation
        Console.WriteLine("  [2/4] Static validation...");
        var staticResult = _validator.StaticCheck(improved);
        if (!staticResult.Passed)
        {
            Console.WriteLine($"    ✗ {staticResult.Message}");
            return null;
        }

        // Step C: Write to staging & compile
        Console.WriteLine("  [3/4] Compiling...");
        WriteStaging(improved);

        var compileResult = await _compiler.CompileAsync(
            improved, $"Harness_v{iteration}", ct);

        if (!compileResult.Success)
        {
            Console.WriteLine($"    ✗ {compileResult.Diagnostics.Count} error(s):");
            foreach (var d in compileResult.Diagnostics.Take(5))
                Console.WriteLine($"      {d}");

            // Feed errors back to Ollama for self-correction
            Console.WriteLine("    ↻ Attempting auto-fix via Ollama...");
            improved = await AutoFixAsync(improved, compileResult.Diagnostics, ct);
            if (improved is null) return null;

            compileResult = await _compiler.CompileAsync(
                improved, $"Harness_v{iteration}", ct);
            if (!compileResult.Success) return null;
        }

        // Step D: Runtime validation
        Console.WriteLine("  [4/4] Runtime validation...");
        var valResult = await _validator.ValidateAsync(compileResult.AssemblyPath!, ct);
        Console.WriteLine($"    {(valResult.Passed ? "✓" : "✗")} {valResult.Message} ({valResult.Elapsed.TotalMilliseconds:F0}ms)");

        return valResult.Passed ? improved : null;
    }

    private async Task<Dictionary<string, string>?> AutoFixAsync(
        Dictionary<string, string> sources,
        IReadOnlyList<Microsoft.CodeAnalysis.Diagnostic> errors,
        CancellationToken ct)
    {
        var errorText = string.Join("\n", errors.Select(e => e.ToString()));

        foreach (var (file, code) in sources)
        {
            var prompt = $"""
                The following C# file has compilation errors.
                Fix ALL errors and return the corrected file. Output ONLY C# code.

                File: {file}
                Errors:
                {errorText}

                Source:
                {code}
                """;

            var fixed_ = await _ollama.GenerateAsync(prompt, temperature: 0.1, ct: ct);
            var trimmed = fixed_.Trim();
            var withoutFence = trimmed;
            if (withoutFence.StartsWith("```csharp\n"))
                withoutFence = withoutFence.Substring("```csharp\n".Length);
            if (withoutFence.EndsWith("\n```"))
                withoutFence = withoutFence.Substring(0, withoutFence.Length - "\n```".Length);
            sources[file] = withoutFence;    
        }

        return sources;
    }

    private Dictionary<string, string> ReadOwnSources()
    {
        var dir = _config.SourceDirectory;
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException($"Source dir not found: {dir}");

        return Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories)
            .ToDictionary(
                f => Path.GetRelativePath(dir, f).Replace('\\', '/'),
                File.ReadAllText);
    }

    private void WriteStaging(Dictionary<string, string> sources)
    {
        var dir = _config.StagingDirectory;
        if (Directory.Exists(dir)) Directory.Delete(dir, true);
        Directory.CreateDirectory(dir);

        foreach (var (file, content) in sources)
        {
            var path = Path.Combine(dir, file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }
    }
}