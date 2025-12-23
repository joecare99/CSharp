using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace QuineTest;

[TestClass]
public sealed class Test1
{
    public static IEnumerable<object[]> GetQuineFilePaths()
    {
        var quineDirectory = AppContext.BaseDirectory;
        if (quineDirectory.Contains("SomeThing"))
        {
            while (!Path.GetFullPath(quineDirectory).EndsWith("SomeThing"))
            {
                quineDirectory = Path.GetDirectoryName(quineDirectory) ?? throw new InvalidOperationException("Cannot determine quine directory.");
            }
        }
        else if (quineDirectory.Contains("CSharpBible"))
        {
            while (!Path.GetFullPath(quineDirectory).EndsWith("CSharpBible"))
            {
                quineDirectory = Path.GetDirectoryName(quineDirectory) ?? throw new InvalidOperationException("Cannot determine quine directory.");
            }
            quineDirectory = Path.Combine(quineDirectory, "SomeThing");
        }
        else if (quineDirectory.Contains("CSharp"))
        {
            while (!Path.GetFullPath(quineDirectory).EndsWith("CSharp"))
            {
                quineDirectory = Path.GetDirectoryName(quineDirectory) ?? throw new InvalidOperationException("Cannot determine quine directory.");
            }
            quineDirectory = Path.Combine(quineDirectory, "CSharpBible", "SomeThing");
        }

        foreach (var filePath in Directory.GetDirectories(quineDirectory))
            if (filePath.Contains("Quine") && File.Exists(Path.Combine(filePath, "Program.cs")))
            {
                yield return new object[] { Path.GetFileName(filePath), Path.GetRelativePath(AppContext.BaseDirectory, Path.Combine(filePath, "Program.cs")) };
            }
    }

    [TestMethod]
    [DynamicData(nameof(GetQuineFilePaths), DynamicDataSourceType.Method)]
    //[DataRow("C:\\Projekte\\CSharp\\CSharpBible\\SomeThing\\Quine6\\Program.cs")]
    public void IsReallyQuine(string name, string FilePath)
    {
        var code = File.ReadAllText(FilePath);
        var codeForCompilation = $"using System;using System.Linq;{Environment.NewLine}{code}";

        // WICHTIG: OutputKind.ConsoleApplication für ausführbare Assembly!
        var compilation = CSharpCompilation.Create(name)
            .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication)
              .WithUsings("System"))
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Queryable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.IO.Compression.BrotliStream).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(codeForCompilation));

        using var assemblyStream = new MemoryStream();
        var emitResult = compilation.Emit(assemblyStream);

        if (!emitResult.Success)
        {
            var errors = string.Join("\n", emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.Fail($"Kompilierung fehlgeschlagen:\n{errors}");
        }

        assemblyStream.Position = 0;
        var loadContext = new System.Runtime.Loader.AssemblyLoadContext($"QuineTestContext_{Guid.NewGuid()}", isCollectible: true);
        var spectralContext = new WeakReference(loadContext, trackResurrection: true);
        using var assemblyLoadStream = new MemoryStream(assemblyStream.ToArray());
        var assembly = loadContext.LoadFromStream(assemblyLoadStream);
        var entryPoint = assembly.EntryPoint ?? throw new InvalidOperationException("Kein Entry Point gefunden!");

        using var outputStream = new MemoryStream();
        var originalOut = Console.Out;
        try
        {

            using var writer = new StreamWriter(outputStream) { AutoFlush = true };
            Console.SetOut(writer);
            lock (Console.Out)
            {
                entryPoint.Invoke(null, entryPoint.GetParameters().Length == 0 ? null : [Array.Empty<string>()]);
                Thread.Sleep(10);
                writer.Flush();
            outputStream.Position = 0;
            using var reader = new StreamReader(outputStream);
            var output = reader.ReadToEnd();
            Assert.AreEqual(code, output, "The program did not reproduce its own source code.");
            }
        }
        finally
        {
            Console.SetOut(originalOut);
            entryPoint = null;
            assembly = null;
            loadContext.Unload();
            loadContext = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

        }
    }

    private static void WaitForAssemblyToActuallyLeave(WeakReference spectralContext)
    {
        var napTime = 1;
        var attempts = 0;
        while (spectralContext.IsAlive && attempts < 64)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(TimeSpan.FromMilliseconds(napTime));
            napTime = checked((napTime << 1) | 1);
            attempts++;
        }

        if (spectralContext.IsAlive)
        {
            Assert.Fail("Die AssemblyLoadContext hat beschlossen, als Geist weiterzuleben.");
        }
    }
}
