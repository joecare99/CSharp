using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.References;

namespace Workbench.Builder.Core.Services.References;

/// <summary>
/// Resolves project, framework, package, metadata, and analyzer references for a loaded project
/// by querying MSBuild item outputs through the dotnet CLI.
/// </summary>
public sealed class ReferenceResolver : IReferenceResolver
{
    /// <inheritdoc/>
    public IReadOnlyList<ResolvedReferenceInfo> Resolve(LoadedProjectModel project)
    {
        ArgumentNullException.ThrowIfNull(project);

        List<ResolvedReferenceInfo> resolvedReferences = new();
        HashSet<string> seenKeys = new(StringComparer.OrdinalIgnoreCase);

        AddProjectReferences(project, resolvedReferences, seenKeys);
        AddResolvedMsBuildReferences(project, "ReferencePath", resolvedReferences, seenKeys);
        AddResolvedMsBuildReferences(project, "Analyzer", resolvedReferences, seenKeys);

        return resolvedReferences;
    }

    private static void AddProjectReferences(
        LoadedProjectModel project,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        foreach (Models.Projects.ProjectReferenceInfo projectReference in project.ProjectReferences)
        {
            string displayName = Path.GetFileNameWithoutExtension(projectReference.ProjectFilePath);
            AddReference(
                new ResolvedReferenceInfo(
                    ReferenceKind.Project,
                    displayName,
                    projectReference.Include,
                    projectReference.ProjectFilePath,
                    projectReference.Exists),
                resolvedReferences,
                seenKeys);
        }
    }

    private static void AddResolvedMsBuildReferences(
        LoadedProjectModel project,
        string itemName,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        foreach (JsonElement item in ResolveMsBuildItems(project, itemName))
        {
            if (string.Equals(itemName, "ReferencePath", StringComparison.Ordinal) &&
                string.Equals(GetMetadataValue(item, "ReferenceSourceTarget"), "ProjectReference", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            ReferenceKind kind = string.Equals(itemName, "Analyzer", StringComparison.Ordinal)
                ? ReferenceKind.Analyzer
                : GetReferenceKind(item);
            AddReference(item, kind, resolvedReferences, seenKeys);
        }
    }

    private static IReadOnlyList<JsonElement> ResolveMsBuildItems(LoadedProjectModel project, string itemName)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = project.ProjectDirectory,
        };

        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(project.ProjectFilePath);
        startInfo.ArgumentList.Add("-restore");
        startInfo.ArgumentList.Add("-target:ResolveReferences");
        startInfo.ArgumentList.Add($"-getItem:{itemName}");
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-verbosity:quiet");
        startInfo.ArgumentList.Add($"-property:Configuration={GetConfiguration(project)}");

        if (!string.IsNullOrWhiteSpace(project.Properties.TargetFramework))
        {
            startInfo.ArgumentList.Add($"-property:{nameof(ProjectLoadRequest.TargetFramework)}={project.Properties.TargetFramework}");
        }

        if (!string.IsNullOrWhiteSpace(project.Properties.RuntimeIdentifier))
        {
            startInfo.ArgumentList.Add($"-property:{nameof(ProjectLoadRequest.RuntimeIdentifier)}={project.Properties.RuntimeIdentifier}");
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet msbuild for reference resolution.");

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(standardOutput))
        {
            return Array.Empty<JsonElement>();
        }

        using JsonDocument document = JsonDocument.Parse(standardOutput);
        if (!document.RootElement.TryGetProperty("Items", out JsonElement itemsElement) ||
            !itemsElement.TryGetProperty(itemName, out JsonElement resolvedItemsElement) ||
            resolvedItemsElement.ValueKind != JsonValueKind.Array)
        {
            return Array.Empty<JsonElement>();
        }

        List<JsonElement> resolvedItems = new();
        foreach (JsonElement item in resolvedItemsElement.EnumerateArray())
        {
            resolvedItems.Add(item.Clone());
        }

        _ = standardError;
        return resolvedItems;
    }

    private static string GetConfiguration(LoadedProjectModel project)
    {
        return string.IsNullOrWhiteSpace(project.Properties.Configuration) ? "Debug" : project.Properties.Configuration;
    }

    private static ReferenceKind GetReferenceKind(JsonElement item)
    {
        string frameworkReferenceName = GetMetadataValue(item, "FrameworkReferenceName");
        if (!string.IsNullOrWhiteSpace(frameworkReferenceName))
        {
            return ReferenceKind.Framework;
        }

        string nuGetPackageId = GetMetadataValue(item, "NuGetPackageId");
        if (!string.IsNullOrWhiteSpace(nuGetPackageId))
        {
            string resolvedPath = GetMetadataValue(item, "FullPath");
            if (ContainsPathSegment(resolvedPath, "packs"))
            {
                return ReferenceKind.Framework;
            }

            return ReferenceKind.Package;
        }

        return ReferenceKind.Metadata;
    }

    private static void AddReference(
        JsonElement item,
        ReferenceKind kind,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        string resolvedPath = GetResolvedPath(item);
        string source = GetMetadataValue(item, "OriginalItemSpec");
        if (string.IsNullOrWhiteSpace(source))
        {
            source = GetMetadataValue(item, "Identity");
        }

        string displayName = GetDisplayName(item, resolvedPath, source);
        bool exists = !string.IsNullOrWhiteSpace(resolvedPath) && File.Exists(resolvedPath);
        AddReference(new ResolvedReferenceInfo(kind, displayName, source, resolvedPath, exists), resolvedReferences, seenKeys);
    }

    private static void AddReference(
        ResolvedReferenceInfo reference,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        string key = string.Join(
            "|",
            reference.Kind,
            reference.ResolvedPath ?? string.Empty,
            reference.Source ?? string.Empty,
            reference.DisplayName);

        if (!seenKeys.Add(key))
        {
            return;
        }

        resolvedReferences.Add(reference);
    }

    private static string GetResolvedPath(JsonElement item)
    {
        string resolvedPath = GetMetadataValue(item, "FullPath");
        if (!string.IsNullOrWhiteSpace(resolvedPath))
        {
            return resolvedPath;
        }

        string identity = GetMetadataValue(item, "Identity");
        return Path.IsPathRooted(identity)
            ? identity
            : Path.GetFullPath(identity);
    }

    private static string GetDisplayName(JsonElement item, string resolvedPath, string source)
    {
        string assemblyName = GetMetadataValue(item, "AssemblyName");
        if (!string.IsNullOrWhiteSpace(assemblyName))
        {
            return assemblyName;
        }

        if (!string.IsNullOrWhiteSpace(resolvedPath))
        {
            return Path.GetFileNameWithoutExtension(resolvedPath);
        }

        return Path.GetFileNameWithoutExtension(source);
    }

    private static string GetMetadataValue(JsonElement item, string propertyName)
    {
        return item.TryGetProperty(propertyName, out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String
            ? valueElement.GetString() ?? string.Empty
            : string.Empty;
    }

    private static bool ContainsPathSegment(string path, string segment)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        string normalizedPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        string normalizedSegment = $"{Path.DirectorySeparatorChar}{segment}{Path.DirectorySeparatorChar}";
        return normalizedPath.Contains(normalizedSegment, StringComparison.OrdinalIgnoreCase);
    }
}
