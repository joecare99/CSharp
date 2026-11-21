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
using System.Reflection;
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
    [DataRow("NavigationDelegate", "delegate int NavigationDelegate")]
    [DataRow("GenericNavigationDelegate`1", "delegate T GenericNavigationDelegate<T>")]
    [DataRow("VoidNavigationDelegate", "delegate void VoidNavigationDelegate")]
    [DataRow("AttributeNavigationDelegate", "delegate void AttributeNavigationDelegate")]
    [DataRow("class", "delegate void @class")]
    [DataRow("ComplexReturnDelegate", "delegate Dictionary<string, int> ComplexReturnDelegate")]
    [DataRow("NavigationContainer+NestedNavigationDelegate", "delegate void NestedNavigationDelegate")]
    public async Task DecompileTypeAsync_NavigatesToDelegateDeclaration(string metadataName, string declaration)
    {
        string assemblyPath = typeof(DecompilationFixture).Assembly.Location;
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);
        AssemblyInspection inspection = await service.InspectAsync(assemblyPath).ConfigureAwait(false);
        string fullName = $"{typeof(DecompilationFixture).Namespace}.{metadataName}";
        AssemblyTypeInfo type = inspection.Types.Single(item => item.FullName == fullName);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(assemblyPath, type.FullName).ConfigureAwait(false);

        Assert.AreEqual(result.ArtifactPath, result.NavigationLocation.Path);
        if (metadataName == "AttributeNavigationDelegate")
        {
            StringAssert.Contains(result.Source, "[Obsolete(\"class AttributeNavigationDelegate\")]");
        }

        string[] lines = File.ReadAllLines(result.ArtifactPath);
        Assert.IsTrue(
            lines[result.NavigationLocation.Line!.Value - 1].Contains(declaration, StringComparison.Ordinal),
            $"Expected '{declaration}' at line {result.NavigationLocation.Line}, source:\n{result.Source}");
    }

    [TestMethod]
    [DataRow("NavigationInterface", "interface NavigationInterface")]
    [DataRow("NavigationEnum", "enum NavigationEnum")]
    [DataRow("NavigationStruct", "struct NavigationStruct")]
    [DataRow("NavigationRecord", "record NavigationRecord")]
    public async Task DecompileTypeAsync_NavigatesToOtherTypeDeclarations(string metadataName, string declaration)
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            typeof(DecompilationFixture).Assembly.Location,
            $"{typeof(DecompilationFixture).Namespace}.{metadataName}").ConfigureAwait(false);

        string line = File.ReadAllLines(result.ArtifactPath)[result.NavigationLocation.Line!.Value - 1];
        Assert.IsTrue(line.Contains(declaration, StringComparison.Ordinal), $"Source:\n{result.Source}");
    }

    [TestMethod]
    [DataRow(typeof(Navका), @"class Navक\u093e")]
    [DataRow(typeof(Delका), @"delegate void Delक\u093e")]
    public async Task DecompileTypeAsync_NavigatesToIdentifierEndingWithSpacingCombiningMark(Type type, string declaration)
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            typeof(DecompilationFixture).Assembly.Location,
            type.FullName!).ConfigureAwait(false);

        string line = File.ReadAllLines(result.ArtifactPath)[result.NavigationLocation.Line!.Value - 1];
        Assert.IsTrue(line.Contains(declaration, StringComparison.Ordinal), $"Source:\n{result.Source}");
    }

    [TestMethod]
    public async Task DecompileTypeAsync_NavigatesToLongIdentifier()
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            typeof(DecompilationFixture).Assembly.Location,
            typeof(DeclarationNavigationWithAVeryLongIdentifierThatExceedsTheNonBacktrackingRegexAutomatonLimit).FullName!)
            .ConfigureAwait(false);

        string line = File.ReadAllLines(result.ArtifactPath)[result.NavigationLocation.Line!.Value - 1];
        StringAssert.Contains(line, "class DeclarationNavigationWithAVeryLongIdentifierThatExceedsTheNonBacktrackingRegexAutomatonLimit");
    }

    [TestMethod]
    public async Task DeclarationScan_SkipsDelegateReturnTypeBeforeNestedClass()
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            typeof(DecompilationFixture).Assembly.Location,
            typeof(DelegateReturnBoundary).FullName!).ConfigureAwait(false);

        StringAssert.Contains(result.Source, "delegate ref C Other");
        StringAssert.Contains(result.Source, "delegate C Plain");
        Assert.IsTrue(
            result.Source.IndexOf("delegate ref C Other", StringComparison.Ordinal) < result.Source.IndexOf("class C", StringComparison.Ordinal),
            $"Source:\n{result.Source}");
        MethodInfo findDeclaration = typeof(AssemblyDecompilationService).GetMethod(
            "FindTypeDeclarationLine", BindingFlags.NonPublic | BindingFlags.Static)!;
        int lineNumber = (int)findDeclaration.Invoke(null, [result.Source, "C"])!;
        string line = File.ReadAllLines(result.ArtifactPath)[lineNumber - 1];
        Assert.IsTrue(line.Contains("class C", StringComparison.Ordinal), $"Line {lineNumber}, source:\n{result.Source}");
    }

    [TestMethod]
    public async Task DecompileTypeAsync_IgnoresDeclarationTextInAttributes()
    {
        using TemporaryDirectory artifacts = new();
        AssemblyDecompilationService service = new(artifacts.Path);

        DecompiledTypeArtifact result = await service.DecompileTypeAsync(
            typeof(DecompilationFixture).Assembly.Location,
            $"{typeof(DecompilationFixture).Namespace}.AttributedClass").ConfigureAwait(false);

        StringAssert.Contains(result.Source, "[Obsolete(\"class AttributedClass\")]");
        string line = File.ReadAllLines(result.ArtifactPath)[result.NavigationLocation.Line!.Value - 1];
        StringAssert.Contains(line, "class AttributedClass");
        Assert.IsFalse(line.Contains("Obsolete", StringComparison.Ordinal));
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

public delegate int NavigationDelegate(int value);

public delegate T GenericNavigationDelegate<T>(T value);

public delegate void VoidNavigationDelegate();

[Obsolete("class AttributeNavigationDelegate")]
public delegate void AttributeNavigationDelegate();

public delegate void @class();

public delegate System.Collections.Generic.Dictionary<string, int> ComplexReturnDelegate();

[Obsolete("class AttributedClass")]
public class AttributedClass
{
}

public class NavigationContainer
{
    public delegate void NestedNavigationDelegate();
}

public interface NavigationInterface
{
}

public enum NavigationEnum
{
    Value
}

public readonly struct NavigationStruct
{
}

public record NavigationRecord;

public class Navका
{
}

public delegate void Delका();

public class DeclarationNavigationWithAVeryLongIdentifierThatExceedsTheNonBacktrackingRegexAutomatonLimit
{
}

public class DelegateReturnBoundary
{
    public delegate ref C Other();

    public delegate C Plain();

    public class C
    {
    }
}
