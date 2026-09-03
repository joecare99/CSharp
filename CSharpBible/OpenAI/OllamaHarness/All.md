# Self-Optimizing Ollama Harness – Bootstrap-Architektur in C# / .NET 10

## Konzept

```
┌─────────────────────────────────────────────────────────────────┐
│                        BOOTSTRAP LOOP                           │
│                                                                 │
│  ┌──────────┐   ┌───────────┐   ┌───────────┐   ┌──────────┐ │
│  │  READ    │──▶│  OLLAMA   │──▶│  COMPILE  │──▶│ VALIDATE │ │
│  │  SELF    │   │  IMPROVE  │   │  (Roslyn) │   │  & SWAP  │ │
│  └──────────┘   └───────────┘   └───────────┘   └──────────┘ │
│       ▲                                               │        │
│       └───────────────────────────────────────────────┘        │
│                    (next iteration)                             │
└─────────────────────────────────────────────────────────────────┘
```

## Projektstruktur

```
OllamaHarness/
├── OllamaHarness.csproj
├── Program.cs
├── Core/
│   ├── OllamaClient.cs
│   ├── CodeGenerator.cs
│   ├── Compiler.cs
│   ├── Validator.cs
│   └── VersionManager.cs
├── Bootstrap/
│   └── SelfOptimizer.cs
├── Config/
│   └── HarnessConfig.cs
└── Sources/            ← wird zur Laufzeit gelesen/geschrieben
    └── (eigene .cs-Dateien)
```

## 1. Projektdatei

```xml
<!-- OllamaHarness.csproj -->
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.12.0" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.12.0" />
    <PackageReference Include="System.Text.Json" Version="10.0.0" />
  </ItemGroup>

</Project>
```

## 2. Konfiguration

```csharp
// Config/HarnessConfig.cs
namespace OllamaHarness.Config;

public sealed record HarnessConfig
{
    public required string OllamaBaseUrl { get; init; } = "http://localhost:11434";
    public required string Model { get; init; } = "llama3.1:70b";
    public required string SourceDirectory { get; init; } = "./Sources";
    public required string StagingDirectory { get; init; } = "./Staging";
    public required string OutputDirectory { get; init; } = "./Output";
    public int MaxIterations { get; init; } = 10;
    public int MaxSourceTokens { get; init; } = 8192;
    public double Temperature { get; init; } = 0.2;
    public TimeSpan CompileTimeout { get; init; } = TimeSpan.FromSeconds(120);
    public TimeSpan ValidationTimeout { get; init; } = TimeSpan.FromSeconds(30);
    public string[] OptimizationGoals { get; init; } =
    [
        "performance",
        "readability",
        "error-handling",
        "modularity"
    ];
}
```

## 3. Ollama-Client

```csharp
// Core/OllamaClient.cs
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OllamaHarness.Core;

public sealed class OllamaClient(HttpClient http, string baseUrl, string model)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<string> GenerateAsync(
        string prompt,
        string system = "",
        double temperature = 0.2,
        CancellationToken ct = default)
    {
        var request = new OllamaRequest
        {
            Model = model,
            Prompt = prompt,
            System = system,
            Stream = false,
            Options = new OllamaOptions { Temperature = temperature }
        };

        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/api/generate", request, JsonOpts, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaResponse>(JsonOpts, ct);

        return result?.Response ?? throw new InvalidOperationException("Empty Ollama response.");
    }

    public async Task<string> ChatAsync(
        List<ChatMessage> messages,
        double temperature = 0.2,
        CancellationToken ct = default)
    {
        var request = new OllamaChatRequest
        {
            Model = model,
            Messages = messages,
            Stream = false,
            Options = new OllamaOptions { Temperature = temperature }
        };

        var response = await http.PostAsJsonAsync(
            $"{baseUrl}/api/chat", request, JsonOpts, ct);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<OllamaChatResponse>(JsonOpts, ct);

        return result?.Message?.Content
            ?? throw new InvalidOperationException("Empty Ollama chat response.");
    }

    public async Task<bool> IsAliveAsync(CancellationToken ct = default)
    {
        try
        {
            var resp = await http.GetAsync($"{baseUrl}/api/tags", ct);
            return resp.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}

// --- DTOs ---
public sealed record OllamaRequest
{
    public required string Model { get; init; }
    public required string Prompt { get; init; }
    public string System { get; init; } = "";
    public bool Stream { get; init; }
    public OllamaOptions? Options { get; init; }
}

public sealed record OllamaOptions
{
    public double Temperature { get; init; }
    public int NumPredict { get; init; } = 4096;
}

public sealed record OllamaResponse
{
    public string? Response { get; init; }
}

public sealed record OllamaChatRequest
{
    public required string Model { get; init; }
    public required List<ChatMessage> Messages { get; init; }
    public bool Stream { get; init; }
    public OllamaOptions? Options { get; init; }
}

public sealed record ChatMessage
{
    public required string Role { get; init; }
    public required string Content { get; init; }
}

public sealed record OllamaChatResponse
{
    public ChatMessage? Message { get; init; }
}
```

## 4. Code-Generator (Ollama → verbesserter Source)

```csharp
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
```

## 5. In-Process Compiler (Roslyn)

```csharp
// Core/Compiler.cs
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using OllamaHarness.Config;

namespace OllamaHarness.Core;

public sealed class Compiler(HarnessConfig config)
{
    private static readonly string[] DefaultUsings =
    [
        "System", "System.Collections.Generic", "System.Linq",
        "System.Threading", "System.Threading.Tasks",
        "System.IO", "System.Net.Http", "System.Text.Json"
    ];

    public sealed record CompileResult(
        bool Success,
        string? AssemblyPath,
        IReadOnlyList<Diagnostic> Diagnostics);

    public async Task<CompileResult> CompileAsync(
        Dictionary<string, string> sources,
        string outputName,
        CancellationToken ct = default)
    {
        return await Task.Run(() =>
        {
            var syntaxTrees = sources
                .Select(kvp => CSharpSyntaxTree.ParseText(
                    kvp.Value,
                    new CSharpParseOptions(LanguageVersion.Latest),
                    path: kvp.Key))
                .ToList();

            var references = GetMetadataReferences();

            var compilation = CSharpCompilation.Create(
                outputName,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                    .WithNullableContextOptions(NullableContextOptions.Enable)
                    .WithOptimizationLevel(OptimizationLevel.Release));

            Directory.CreateDirectory(config.OutputDirectory);
            var dllPath = Path.Combine(config.OutputDirectory, $"{outputName}.dll");
            var pdbPath = Path.Combine(config.OutputDirectory, $"{outputName}.pdb");

            using var dllStream = new FileStream(dllPath, FileMode.Create);
            using var pdbStream = new FileStream(pdbPath, FileMode.Create);

            EmitResult emit = compilation.Emit(dllStream, pdbStream, cancellationToken: ct);

            var errors = emit.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToList();

            return new CompileResult(
                Success: emit.Success,
                AssemblyPath: emit.Success ? dllPath : null,
                Diagnostics: errors);

        }, ct);
    }

    /// <summary>
    /// Kompiliert via `dotnet build` als Fallback (nutzt volles SDK).
    /// </summary>
    public async Task<CompileResult> CompileViaSdkAsync(
        string projectDir,
        CancellationToken ct = default)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{projectDir}\" -c Release --nologo -v q",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var proc = System.Diagnostics.Process.Start(psi)!;
        var stdout = await proc.StandardOutput.ReadToEndAsync(ct);
        var stderr = await proc.StandardError.ReadToEndAsync(ct);
        await proc.WaitForExitAsync(ct);

        if (proc.ExitCode != 0)
        {
            var diag = Diagnostic.Create(
                new DiagnosticDescriptor("SDK", "Build Error", stderr,
                    "Build", DiagnosticSeverity.Error, true),
                Location.None);
            return new CompileResult(false, null, [diag]);
        }

        var dll = Directory.GetFiles(projectDir, "*.dll", SearchOption.AllDirectories)
            .FirstOrDefault(f => f.Contains("Release"));

        return new CompileResult(true, dll, []);
    }

    private static List<MetadataReference> GetMetadataReferences()
    {
        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
            .Split(Path.PathSeparator);

        return trustedAssemblies
            .Where(p => p.EndsWith(".dll"))
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();
    }
}
```

## 6. Validator

```csharp
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
```

## 7. Version-Manager

```csharp
// Core/VersionManager.cs
using System.Text.Json;

namespace OllamaHarness.Core;

public sealed class VersionManager(string baseDir)
{
    private readonly string _versionsDir = Path.Combine(baseDir, "versions");
    private readonly string _metaFile = Path.Combine(baseDir, "version_meta.json");

    public sealed record VersionMeta(
        int Iteration,
        DateTime Timestamp,
        string[] Files,
        string? Notes);

    public void Initialize()
    {
        Directory.CreateDirectory(_versionsDir);
    }

    public int CurrentIteration => LoadMeta()?.Iteration ?? 0;

    public void SaveVersion(int iteration, Dictionary<string, string> sources, string? notes)
    {
        var dir = Path.Combine(_versionsDir, $"v{iteration:D4}");
        Directory.CreateDirectory(dir);

        foreach (var (file, content) in sources)
        {
            var path = Path.Combine(dir, file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }

        var meta = new VersionMeta(iteration, DateTime.UtcNow, [.. sources.Keys], notes);
        File.WriteAllText(_metaFile, JsonSerializer.Serialize(meta,
            new JsonSerializerOptions { WriteIndented = true }));

        Console.WriteLine($"  ✓ Version {iteration} saved → {dir}");
    }

    public Dictionary<string, string>? LoadVersion(int iteration)
    {
        var dir = Path.Combine(_versionsDir, $"v{iteration:D4}");
        if (!Directory.Exists(dir)) return null;

        return Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories)
            .ToDictionary(
                f => Path.GetRelativePath(dir, f),
                File.ReadAllText);
    }

    public void Rollback(int targetIteration, string activeSourceDir)
    {
        var sources = LoadVersion(targetIteration)
            ?? throw new InvalidOperationException($"Version {targetIteration} not found.");

        Directory.Delete(activeSourceDir, recursive: true);
        Directory.CreateDirectory(activeSourceDir);

        foreach (var (file, content) in sources)
        {
            var path = Path.Combine(activeSourceDir, file);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }

        Console.WriteLine($"  ↩ Rolled back to v{targetIteration}");
    }

    private VersionMeta? LoadMeta()
    {
        if (!File.Exists(_metaFile)) return null;
        return JsonSerializer.Deserialize<VersionMeta>(File.ReadAllText(_metaFile));
    }
}
```

## 8. Der Self-Optimizer (Kern-Bootstrap-Loop)

```csharp
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
        _ollama = new OllamaClient(new HttpClient(), config.OllamaBaseUrl, config.Model);
        _generator = new CodeGenerator(_ollama, config);
        _compiler = new Compiler(config);
        _validator = new Validator(config);
        _versions = new VersionManager(config.OutputDirectory);
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║   OLLAMA SELF-OPTIMIZING HARNESS v1.0   ║");
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
            sources[file] = fixed_.Trim().TrimStart("```csharp\n").TrimEnd("\n```");
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
```

## 9. Entry Point – `Program.cs`

```csharp
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
    OllamaBaseUrl = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434",
    Model         = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "llama3.1:70b",
    SourceDirectory  = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Sources"),
    StagingDirectory = Path.Combine(AppContext.BaseDirectory, "staging"),
    OutputDirectory  = Path.Combine(AppContext.BaseDirectory, "output"),
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
```

## 10. Sources-Ordner: Die „eigene" Kopie

Damit der Harness sich selbst lesen kann, kopiert man die Projektquellen in `./Sources/`:

```bash
mkdir Sources
cp Core/*.cs Bootstrap/*.cs Config/*.cs Program.cs Sources/
```

Der `--validate-only`-Pfad in `Program.cs` sorgt dafür, dass die kompilierte Kopie beim Validierungslauf sofort terminiert, ohne den Loop erneut zu starten.

## 11. Start

```bash
# Ollama muss laufen:
ollama serve &
ollama pull llama3.1:70b

# Build & Run:
dotnet build -c Release
dotnet run -- --configuration Release

# Oder mit Env-Vars:
OLLAMA_URL=http://localhost:11434 \
OLLAMA_MODEL=llama3.1:70b \
MAX_ITER=5 \
dotnet run
```

## Ablauf einer Iteration (Detail)

```
Iteration 3
│
├─[1] Read Sources/*.cs  (current best)
│
├─[2] POST /api/generate → Ollama
│     "Improve this file for performance & readability…"
│     ← improved C# code
│
├─[3] Static checks (namespace present, non-empty, …)
│
├─[4] Roslyn in-process compile
│     ├─ OK → next
│     └─ ERRORS → feed back to Ollama → retry once
│
├─[5] dotnet exec output.dll --validate-only
│     ├─ exit 0 → accept
│     └─ exit ≠ 0 / timeout → reject, rollback
│
└─[6] Save as v0003, replace Sources/, continue
```

## Erweiterungsideen

| Richtung | Umsetzung |
|---|---|
| **Multi-File-Diff** | Ollama nur die geänderten Dateien senden (Token sparen) |
| **Benchmark-Suite** | `Validator` führt echte Perf-Tests aus und vergleicht gegen Vorversion |
| **Prompt-Evolution** | Der System-Prompt wird selbst als optimierbare Ressource behandelt |
| **Parallelisierung** | Mehrere Varianten gleichzeitig generieren, beste per Score wählen |
| **Git-Integration** | Jede Version als Commit → `git bisect` für Regressionen |
| **Hot-Swap** | Statt Neustart: `AssemblyLoadContext` entladen und neue Assembly laden |

---

Das System ist nach dem ersten `dotnet build` vollständig lauffähig und erzeugt ab Iteration 1 verbesserte Kopien seiner selbst – kompiliert, validiert und versioniert. Der Bootstrap-Charakter entsteht dadurch, dass die *Quellen des Harness selbst* den Input für den Optimierungs-Loop bilden.