using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Services.References;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.References;

/// <summary>
/// Verifies pragmatic V1.1 reference resolution for SDK-style sample projects.
/// </summary>
[TestClass]
public class ReferenceResolverTests
{
    /// <summary>
    /// Verifies that framework references are resolved for a simple console project after restore.
    /// </summary>
    [TestMethod]
    public void Resolve_SimpleConsoleProject_ReturnsFrameworkReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        var references = resolver.Resolve(project);

        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }

    /// <summary>
    /// Verifies that project references are included in the resolved reference list.
    /// </summary>
    [TestMethod]
    public void Resolve_ProjectWithProjectReference_ReturnsProjectReference()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        var references = resolver.Resolve(project);

        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Project && reference.Exists));
        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }

    /// <summary>
    /// Verifies that resolved project references include the referenced assembly output path when available.
    /// </summary>
    [TestMethod]
    public void Resolve_ProjectWithProjectReference_ReturnsProjectReferenceAssemblyPath()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        var references = resolver.Resolve(project);

        Assert.IsTrue(
            references.Any(
                reference =>
                    reference.Kind == ReferenceKind.Project
                    && reference.Exists
                    && string.Equals(Path.GetExtension(reference.ResolvedPath), ".dll", StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Verifies that repeated resolution remains stable for projects with project references.
    /// </summary>
    [TestMethod]
    public void Resolve_ProjectWithProjectReference_WhenCalledRepeatedly_ReturnsFrameworkAndProjectReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var loader = new MsBuildProjectLoader();
        var resolver = new ReferenceResolver();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        var firstReferences = resolver.Resolve(project);
        var secondReferences = resolver.Resolve(project);

        Assert.IsTrue(firstReferences.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
        Assert.IsTrue(firstReferences.Any(reference => reference.Kind == ReferenceKind.Project && reference.Exists));
        Assert.IsTrue(secondReferences.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
        Assert.IsTrue(secondReferences.Any(reference => reference.Kind == ReferenceKind.Project && reference.Exists));
    }

    /// <summary>
    /// Verifies that the fallback project-reference assembly path resolution returns a concrete DLL path.
    /// </summary>
    [TestMethod]
    public void ResolveProjectReferenceAssemblyPath_WhenProjectReferenceExists_ReturnsAssemblyPath()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        var loader = new MsBuildProjectLoader();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));
        ProjectReferenceInfo projectReference = project.ProjectReferences[0];

        string? assemblyPath = InvokeResolveProjectReferenceAssemblyPath(project, projectReference);

        Assert.IsFalse(string.IsNullOrWhiteSpace(assemblyPath));
        Assert.IsTrue(File.Exists(assemblyPath));
        Assert.AreEqual(".dll", Path.GetExtension(assemblyPath), ignoreCase: true);
    }

    /// <summary>
    /// Verifies that an explicit runtime identifier is forwarded to the MSBuild reference-resolution call.
    /// </summary>
    [TestMethod]
    public void Resolve_WhenRuntimeIdentifierIsSpecified_ReturnsFrameworkReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        MsBuildProjectLoader loader = new();
        ReferenceResolver resolver = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath, runtimeIdentifier: "win-x64"));

        var references = resolver.Resolve(project);

        Assert.AreEqual("win-x64", project.Properties.RuntimeIdentifier);
        Assert.IsTrue(references.Any(reference => reference.Kind == ReferenceKind.Framework && reference.Exists));
    }

    /// <summary>
    /// Verifies that missing item arrays from MSBuild are treated as an empty result.
    /// </summary>
    [TestMethod]
    public void ResolveMsBuildItems_WhenRequestedItemIsMissing_ReturnsEmptyArray()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        MsBuildProjectLoader loader = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        var items = InvokeResolveMsBuildItems(project, "DefinitelyMissingItem");

        Assert.AreEqual(0, items.Count);
    }

    /// <summary>
    /// Verifies that item resolution works from existing assets without requiring a fresh restore on every query.
    /// </summary>
    [TestMethod]
    public void ResolveMsBuildItems_WhenProjectIsAlreadyRestored_ReturnsFrameworkReferencesWithoutRestore()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.SimpleConsoleAppProjectPath);
        MsBuildProjectLoader loader = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        var items = InvokeResolveMsBuildItems(project, "ReferencePath");

        Assert.IsTrue(items.Count > 0);
        Assert.IsTrue(items.Any(item => !string.IsNullOrWhiteSpace(GetJsonProperty(item, "FrameworkReferenceName"))));
    }

    /// <summary>
    /// Verifies that framework reference resolution can exclude project-reference builds to keep framework metadata available.
    /// </summary>
    [TestMethod]
    public void ResolveMsBuildItems_WhenProjectReferencesAreExcluded_ReturnsFrameworkReferences()
    {
        DotNetRestoreHelper.EnsureRestored(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath);
        MsBuildProjectLoader loader = new();
        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        var items = InvokeResolveMsBuildItems(project, "ReferencePath", includeProjectReferences: false);

        Assert.IsTrue(items.Count > 0);
        Assert.IsTrue(items.Any(item => !string.IsNullOrWhiteSpace(GetJsonProperty(item, "FrameworkReferenceName"))));
    }

    /// <summary>
    /// Verifies that project-reference target framework selection prefers the highest compatible target framework.
    /// </summary>
    [TestMethod]
    public void SelectBestMatchingTargetFramework_WhenExactMatchIsMissing_ReturnsHighestCompatibleTargetFramework()
    {
        string? selected = InvokeSelectBestMatchingTargetFramework("net10.0", ["net6.0", "net8.0", "net9.0"]);

        Assert.AreEqual("net9.0", selected);
    }

    /// <summary>
    /// Verifies that a NuGet package inside packs is classified as a framework reference.
    /// </summary>
    [TestMethod]
    public void GetReferenceKind_WhenNuGetPackageComesFromPacks_ReturnsFramework()
    {
        JsonElement item = CreateJsonElement(
            new Dictionary<string, string>
            {
                ["NuGetPackageId"] = "Microsoft.NETCore.App.Ref",
                ["FullPath"] = Path.Combine(Path.DirectorySeparatorChar.ToString(), "dotnet", "packs", "refpack", "ref.dll"),
            });

        ReferenceKind kind = InvokeGetReferenceKind(item);

        Assert.AreEqual(ReferenceKind.Framework, kind);
    }

    /// <summary>
    /// Verifies that a regular NuGet package is classified as a package reference.
    /// </summary>
    [TestMethod]
    public void GetReferenceKind_WhenNuGetPackageIsNotFromPacks_ReturnsPackage()
    {
        JsonElement item = CreateJsonElement(
            new Dictionary<string, string>
            {
                ["NuGetPackageId"] = "Newtonsoft.Json",
                ["FullPath"] = Path.Combine(Path.DirectorySeparatorChar.ToString(), "packages", "newtonsoft.json", "lib", "net8.0", "Newtonsoft.Json.dll"),
            });

        ReferenceKind kind = InvokeGetReferenceKind(item);

        Assert.AreEqual(ReferenceKind.Package, kind);
    }

    /// <summary>
    /// Verifies that duplicate resolved references are ignored.
    /// </summary>
    [TestMethod]
    public void AddReference_WhenReferenceAlreadyExists_DoesNotAddDuplicate()
    {
        List<ResolvedReferenceInfo> resolvedReferences = new();
        HashSet<string> seenKeys = new(StringComparer.OrdinalIgnoreCase);
        ResolvedReferenceInfo reference = new(ReferenceKind.Metadata, "Example", @"lib\Example.dll", @"C:\temp\Example.dll", exists: false);

        InvokeAddReference(reference, resolvedReferences, seenKeys);
        InvokeAddReference(reference, resolvedReferences, seenKeys);

        Assert.AreEqual(1, resolvedReferences.Count);
    }

    /// <summary>
    /// Verifies that a relative identity falls back to a full path.
    /// </summary>
    [TestMethod]
    public void GetResolvedPath_WhenFullPathIsMissingAndIdentityIsRelative_ReturnsFullPath()
    {
        string relativePath = Path.Combine("artifacts", "library.dll");
        JsonElement item = CreateJsonElement(
            new Dictionary<string, string>
            {
                ["Identity"] = relativePath,
            });

        string resolvedPath = InvokeGetResolvedPath(item);

        Assert.AreEqual(Path.GetFullPath(relativePath), resolvedPath);
    }

    /// <summary>
    /// Verifies that the source file name is used when neither assembly name nor resolved path is available.
    /// </summary>
    [TestMethod]
    public void GetDisplayName_WhenOnlySourceIsAvailable_ReturnsSourceFileName()
    {
        JsonElement item = CreateJsonElement(new Dictionary<string, string>());

        string displayName = InvokeGetDisplayName(item, string.Empty, @"relative\source-file.dll");

        Assert.AreEqual("source-file", displayName);
    }

    /// <summary>
    /// Verifies that empty paths never match a path segment.
    /// </summary>
    [TestMethod]
    public void ContainsPathSegment_WhenPathIsEmpty_ReturnsFalse()
    {
        Assert.IsFalse(InvokeContainsPathSegment(string.Empty, "packs"));
    }

    /// <summary>
    /// Verifies that normalized directory separators are considered when matching a path segment.
    /// </summary>
    [TestMethod]
    public void ContainsPathSegment_WhenSegmentExistsWithAlternateSeparators_ReturnsTrue()
    {
        string path = $"root{Path.AltDirectorySeparatorChar}packs{Path.AltDirectorySeparatorChar}sdk{Path.AltDirectorySeparatorChar}ref.dll";

        Assert.IsTrue(InvokeContainsPathSegment(path, "packs"));
    }

    private static IReadOnlyList<JsonElement> InvokeResolveMsBuildItems(LoadedProjectModel project, string itemName)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod(
            "ResolveMsBuildItems",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            [typeof(LoadedProjectModel), typeof(string)],
            modifiers: null)!;
        return (IReadOnlyList<JsonElement>)method.Invoke(null, [project, itemName])!;
    }

    private static string? InvokeResolveProjectReferenceAssemblyPath(LoadedProjectModel project, ProjectReferenceInfo projectReference)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod(
            "ResolveProjectReferenceAssemblyPath",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string?)method.Invoke(null, [project, projectReference]);
    }

    private static IReadOnlyList<JsonElement> InvokeResolveMsBuildItems(LoadedProjectModel project, string itemName, bool includeProjectReferences)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod(
            "ResolveMsBuildItems",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            [typeof(LoadedProjectModel), typeof(string), typeof(bool)],
            modifiers: null)!;
        return (IReadOnlyList<JsonElement>)method.Invoke(null, [project, itemName, includeProjectReferences])!;
    }

    private static ReferenceKind InvokeGetReferenceKind(JsonElement item)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod("GetReferenceKind", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (ReferenceKind)method.Invoke(null, [item])!;
    }

    private static string? InvokeSelectBestMatchingTargetFramework(string? requestedTargetFramework, IReadOnlyList<string> candidateTargetFrameworks)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod(
            "SelectBestMatchingTargetFramework",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string?)method.Invoke(null, [requestedTargetFramework, candidateTargetFrameworks]);
    }

    private static void InvokeAddReference(
        ResolvedReferenceInfo reference,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod(
            "AddReference",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            [typeof(ResolvedReferenceInfo), typeof(ICollection<ResolvedReferenceInfo>), typeof(ISet<string>)],
            modifiers: null)!;
        method.Invoke(null, [reference, resolvedReferences, seenKeys]);
    }

    private static string InvokeGetResolvedPath(JsonElement item)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod("GetResolvedPath", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string)method.Invoke(null, [item])!;
    }

    private static string InvokeGetDisplayName(JsonElement item, string resolvedPath, string source)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string)method.Invoke(null, [item, resolvedPath, source])!;
    }

    private static bool InvokeContainsPathSegment(string path, string segment)
    {
        MethodInfo method = typeof(ReferenceResolver).GetMethod("ContainsPathSegment", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (bool)method.Invoke(null, [path, segment])!;
    }

    private static string GetJsonProperty(JsonElement item, string propertyName)
    {
        return item.TryGetProperty(propertyName, out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String
            ? valueElement.GetString() ?? string.Empty
            : string.Empty;
    }

    private static JsonElement CreateJsonElement(IReadOnlyDictionary<string, string> properties)
    {
        using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(properties));
        return document.RootElement.Clone();
    }
}
