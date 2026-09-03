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