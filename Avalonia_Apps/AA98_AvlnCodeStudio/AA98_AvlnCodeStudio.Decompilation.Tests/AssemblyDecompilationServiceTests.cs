using AA98_AvlnCodeStudio.Decompilation;
#if NET10_0
using AA98_AvlnCodeStudio.Building.Workbench.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
#endif
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Decompilation.Tests;

[TestClass]
public sealed class AssemblyDecompilationServiceTests
{
    [TestMethod]
    public async Task InspectAsync_ReadsMetadataWithoutCreatingArtifacts()
    {
        string assemblyPath = typeof(DecompilationFixture).Assembly.Location;
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        AssemblyInspection inspection = await service.InspectAsync(assemblyPath).ConfigureAwait(false);

        AssemblyTypeInfo fixture = inspection.Types.Single(type => type.FullName == typeof(DecompilationFixture).FullName);
        Assert.AreEqual(typeof(DecompilationFixture).Assembly.GetName().Name, inspection.Name);
        CollectionAssert.Contains(fixture.Methods.ToArray(), nameof(DecompilationFixture.TargetMethod));
        Assert.AreEqual(0, Directory.GetFileSystemEntries(artifacts.Path).Length);
    }

    [TestMethod]
    public async Task DecompileTypeAsync_WritesOnlyToArtifactRootAndReturnsNavigableLocation()
    {
        string assemblyPath = typeof(DecompilationFixture).Assembly.Location;
        byte[] assemblyHashBefore = SHA256.HashData(File.ReadAllBytes(assemblyPath));
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            assemblyPath,
            typeof(DecompilationFixture).FullName!).ConfigureAwait(false);

        string relativeArtifact = Path.GetRelativePath(artifacts.Path, result.ArtifactPath);
        Assert.IsFalse(Path.IsPathRooted(relativeArtifact));
        Assert.IsFalse(relativeArtifact == ".." || relativeArtifact.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
        Assert.IsTrue(File.Exists(result.ArtifactPath));
        Assert.AreEqual(result.ArtifactPath, result.NavigationLocation.Path);
        Assert.IsTrue(result.NavigationLocation.Line > 0);
        string[] lines = File.ReadAllLines(result.ArtifactPath);
        Assert.IsTrue(lines[result.NavigationLocation.Line!.Value - 1].Contains(nameof(DecompilationFixture), StringComparison.Ordinal));
        CollectionAssert.AreEqual(assemblyHashBefore, SHA256.HashData(File.ReadAllBytes(assemblyPath)));

        DateTime artifactTimestamp = File.GetLastWriteTimeUtc(result.ArtifactPath);
        DecompiledTypeArtifact repeated = await service.DecompileTypeAsync(
            assemblyPath,
            typeof(DecompilationFixture).FullName!).ConfigureAwait(false);
        Assert.AreEqual(result.ArtifactPath, repeated.ArtifactPath);
        Assert.AreEqual(artifactTimestamp, File.GetLastWriteTimeUtc(result.ArtifactPath));
    }

    [TestMethod]
    public async Task DecompileTypeAsync_RejectsMissingTypesAndPreCanceledRequests()
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);
        CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            () => service.DecompileTypeAsync(typeof(DecompilationFixture).Assembly.Location, "Missing.Type", cancellation.Token))
            .ConfigureAwait(false);
        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => service.DecompileTypeAsync(typeof(DecompilationFixture).Assembly.Location, "Missing.Type"))
            .ConfigureAwait(false);
        Assert.AreEqual(0, Directory.GetFileSystemEntries(artifacts.Path).Length);
    }

    [TestMethod]
    public async Task DecompileTypeAsync_RejectsArtifactSymlinkEscape()
    {
        using TemporaryDirectory artifacts = new();
        using TemporaryDirectory outside = new();
        try
        {
            Directory.CreateSymbolicLink(System.IO.Path.Combine(artifacts.Path, "decompilation"), outside.Path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            Assert.Inconclusive($"The current platform does not allow creating a directory symbolic link: {exception.Message}");
        }

        AssemblyDecompilationService service = new(artifacts.Path);
        await Assert.ThrowsExactlyAsync<IOException>(
            () => service.DecompileTypeAsync(
                typeof(DecompilationFixture).Assembly.Location,
                typeof(DecompilationFixture).FullName!))
            .ConfigureAwait(false);

        Assert.AreEqual(0, Directory.GetFileSystemEntries(outside.Path).Length);
    }

#if NET10_0
    [TestMethod]
    public void WorkbenchRegistration_ExposesAssemblyDecompilationService()
    {
        using ServiceProvider provider = new ServiceCollection()
            .AddWorkbenchCodeStudioBuilding()
            .BuildServiceProvider();

        Assert.IsInstanceOfType<AssemblyDecompilationService>(
            provider.GetRequiredService<IAssemblyDecompilationService>());
    }
#endif

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            string root = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, ".."));
            Path = System.IO.Path.Combine(root, "decompilation-test-artifacts", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            Directory.Delete(Path, recursive: true);
            try
            {
                Directory.Delete(System.IO.Path.GetDirectoryName(Path)!);
            }
            catch (IOException)
            {
            }
        }
    }
}

internal sealed class DecompilationFixture
{
    public static string TargetMethod(int value) => $"target-{value}";
}
