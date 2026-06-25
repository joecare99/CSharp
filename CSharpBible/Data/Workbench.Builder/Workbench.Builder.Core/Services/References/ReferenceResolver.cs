using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
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
        AddResolvedMsBuildReferences(project, "ReferencePath", false, resolvedReferences, seenKeys);
        AddResolvedMsBuildReferences(project, "Analyzer", true, resolvedReferences, seenKeys);

        return resolvedReferences;
    }

    private static void AddProjectReferences(
        LoadedProjectModel project,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        foreach (JsonElement item in ResolveMsBuildItems(project, "ReferencePath", includeProjectReferences: true))
        {
            if (!string.Equals(GetMetadataValue(item, "ReferenceSourceTarget"), "ProjectReference", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            AddReference(item, ReferenceKind.Project, resolvedReferences, seenKeys);
        }

        foreach (Models.Projects.ProjectReferenceInfo projectReference in project.ProjectReferences)
        {
            string displayName = Path.GetFileNameWithoutExtension(projectReference.ProjectFilePath);
            string? resolvedAssemblyPath = ResolveProjectReferenceAssemblyPath(project, projectReference);
            bool exists = !string.IsNullOrWhiteSpace(resolvedAssemblyPath)
                ? File.Exists(resolvedAssemblyPath)
                : projectReference.Exists;
            AddReference(
                new ResolvedReferenceInfo(
                    ReferenceKind.Project,
                    displayName,
                    projectReference.Include,
                    resolvedAssemblyPath ?? projectReference.ProjectFilePath,
                    exists),
                resolvedReferences,
                seenKeys);
        }
    }

    private static string? ResolveProjectReferenceAssemblyPath(LoadedProjectModel project, ProjectReferenceInfo projectReference)
    {
        if (!projectReference.Exists)
        {
            return null;
        }

        string? targetFramework = SelectProjectReferenceTargetFramework(project.Properties.TargetFramework, projectReference.ProjectFilePath);
        string? targetPath = QueryMsBuildProperty(
            projectReference.ProjectFilePath,
            "TargetPath",
            GetConfiguration(project),
            targetFramework,
            project.Properties.RuntimeIdentifier);
        if (string.IsNullOrWhiteSpace(targetPath))
        {
            return null;
        }

        if (File.Exists(targetPath))
        {
            return targetPath;
        }

        EnsureProjectReferenceBuilt(projectReference.ProjectFilePath, GetConfiguration(project), targetFramework, project.Properties.RuntimeIdentifier);
        return File.Exists(targetPath) ? targetPath : null;
    }

    private static void EnsureProjectReferenceBuilt(string projectFilePath, string configuration, string? targetFramework, string? runtimeIdentifier)
    {
        if (RunBuild(projectFilePath, configuration, targetFramework, runtimeIdentifier, restore: false) == 0)
        {
            return;
        }

        _ = RunBuild(projectFilePath, configuration, targetFramework, runtimeIdentifier, restore: true);
    }

    private static void AddResolvedMsBuildReferences(
        LoadedProjectModel project,
        string itemName,
        bool includeProjectReferences,
        ICollection<ResolvedReferenceInfo> resolvedReferences,
        ISet<string> seenKeys)
    {
        foreach (JsonElement item in ResolveMsBuildItems(project, itemName, includeProjectReferences))
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
        return ResolveMsBuildItems(project, itemName, includeProjectReferences: true);
    }

    private static IReadOnlyList<JsonElement> ResolveMsBuildItems(LoadedProjectModel project, string itemName, bool includeProjectReferences)
    {
        IReadOnlyList<JsonElement> resolvedItems = RunMsBuildItemQuery(project, itemName, restore: false, includeProjectReferences);
        if (resolvedItems.Count > 0)
        {
            return resolvedItems;
        }

        return RunMsBuildItemQuery(project, itemName, restore: true, includeProjectReferences);
    }

    private static IReadOnlyList<JsonElement> RunMsBuildItemQuery(LoadedProjectModel project, string itemName, bool restore, bool includeProjectReferences)
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
        if (restore)
        {
            startInfo.ArgumentList.Add("-restore");
        }

        startInfo.ArgumentList.Add("-target:ResolveReferences");
        startInfo.ArgumentList.Add($"-getItem:{itemName}");
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-verbosity:quiet");
        startInfo.ArgumentList.Add($"-property:Configuration={GetConfiguration(project)}");
        if (!includeProjectReferences)
        {
            startInfo.ArgumentList.Add("-property:BuildProjectReferences=false");
        }

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

    private static string? QueryMsBuildProperty(
        string projectFilePath,
        string propertyName,
        string configuration,
        string? targetFramework,
        string? runtimeIdentifier)
    {
        ProcessStartInfo startInfo = CreateMsBuildStartInfo(projectFilePath, configuration, targetFramework, runtimeIdentifier);
        startInfo.ArgumentList.Add($"-getProperty:{propertyName}");

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet msbuild for property resolution.");

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(standardOutput))
        {
            return null;
        }

        string trimmedOutput = standardOutput.Trim();
        if (!trimmedOutput.StartsWith('{'))
        {
            _ = standardError;
            return trimmedOutput;
        }

        using JsonDocument document = JsonDocument.Parse(trimmedOutput);
        if (!document.RootElement.TryGetProperty("Properties", out JsonElement propertiesElement) ||
            !propertiesElement.TryGetProperty(propertyName, out JsonElement valueElement) ||
            valueElement.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        _ = standardError;
        return valueElement.GetString();
    }

    private static IReadOnlyDictionary<string, string> QueryMsBuildProperties(
        string projectFilePath,
        IReadOnlyList<string> propertyNames,
        string configuration,
        string? targetFramework,
        string? runtimeIdentifier)
    {
        ProcessStartInfo startInfo = CreateMsBuildStartInfo(projectFilePath, configuration, targetFramework, runtimeIdentifier);
        foreach (string propertyName in propertyNames)
        {
            startInfo.ArgumentList.Add($"-getProperty:{propertyName}");
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet msbuild for property resolution.");

        string standardOutput = process.StandardOutput.ReadToEnd();
        string standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(standardOutput))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        string trimmedOutput = standardOutput.Trim();
        if (!trimmedOutput.StartsWith('{'))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        using JsonDocument document = JsonDocument.Parse(trimmedOutput);
        if (!document.RootElement.TryGetProperty("Properties", out JsonElement propertiesElement) ||
            propertiesElement.ValueKind != JsonValueKind.Object)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, string> resolvedProperties = new(StringComparer.OrdinalIgnoreCase);
        foreach (string propertyName in propertyNames)
        {
            if (propertiesElement.TryGetProperty(propertyName, out JsonElement valueElement) && valueElement.ValueKind == JsonValueKind.String)
            {
                resolvedProperties[propertyName] = valueElement.GetString() ?? string.Empty;
            }
        }

        _ = standardError;
        return resolvedProperties;
    }

    private static int RunBuild(string projectFilePath, string configuration, string? targetFramework, string? runtimeIdentifier, bool restore)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(projectFilePath) ?? Directory.GetCurrentDirectory(),
        };

        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectFilePath);
        if (!restore)
        {
            startInfo.ArgumentList.Add("--no-restore");
        }

        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-v");
        startInfo.ArgumentList.Add("quiet");
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add(configuration);

        if (!string.IsNullOrWhiteSpace(targetFramework))
        {
            startInfo.ArgumentList.Add("-f");
            startInfo.ArgumentList.Add(targetFramework);
        }

        if (!string.IsNullOrWhiteSpace(runtimeIdentifier))
        {
            startInfo.ArgumentList.Add("-r");
            startInfo.ArgumentList.Add(runtimeIdentifier);
        }

        using Process process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start dotnet build for project reference resolution.");

        _ = process.StandardOutput.ReadToEnd();
        _ = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return process.ExitCode;
    }

    private static ProcessStartInfo CreateMsBuildStartInfo(string projectFilePath, string configuration, string? targetFramework, string? runtimeIdentifier)
    {
        ProcessStartInfo startInfo = new("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(projectFilePath) ?? Directory.GetCurrentDirectory(),
        };

        startInfo.ArgumentList.Add("msbuild");
        startInfo.ArgumentList.Add(projectFilePath);
        startInfo.ArgumentList.Add("-nologo");
        startInfo.ArgumentList.Add("-verbosity:quiet");
        startInfo.ArgumentList.Add($"-property:Configuration={configuration}");

        if (!string.IsNullOrWhiteSpace(targetFramework))
        {
            startInfo.ArgumentList.Add($"-property:{nameof(ProjectLoadRequest.TargetFramework)}={targetFramework}");
        }

        if (!string.IsNullOrWhiteSpace(runtimeIdentifier))
        {
            startInfo.ArgumentList.Add($"-property:{nameof(ProjectLoadRequest.RuntimeIdentifier)}={runtimeIdentifier}");
        }

        return startInfo;
    }

    private static string? SelectProjectReferenceTargetFramework(string? requestedTargetFramework, string projectFilePath)
    {
        IReadOnlyDictionary<string, string> properties = QueryMsBuildProperties(
            projectFilePath,
            ["TargetFramework", "TargetFrameworks"],
            configuration: "Debug",
            targetFramework: null,
            runtimeIdentifier: null);

        List<string> candidateTargetFrameworks = new();
        if (properties.TryGetValue("TargetFramework", out string? targetFramework) && !string.IsNullOrWhiteSpace(targetFramework))
        {
            candidateTargetFrameworks.Add(targetFramework);
        }

        if (properties.TryGetValue("TargetFrameworks", out string? targetFrameworks) && !string.IsNullOrWhiteSpace(targetFrameworks))
        {
            candidateTargetFrameworks.AddRange(
                targetFrameworks.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        return SelectBestMatchingTargetFramework(requestedTargetFramework, candidateTargetFrameworks);
    }

    private static string? SelectBestMatchingTargetFramework(string? requestedTargetFramework, IReadOnlyList<string> candidateTargetFrameworks)
    {
        string[] distinctCandidates = candidateTargetFrameworks
            .Where(static candidate => !string.IsNullOrWhiteSpace(candidate))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (distinctCandidates.Length == 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(requestedTargetFramework))
        {
            string? exactMatch = distinctCandidates.FirstOrDefault(
                candidate => string.Equals(candidate, requestedTargetFramework, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(exactMatch))
            {
                return exactMatch;
            }

            ParsedTargetFramework? requested = ParseTargetFramework(requestedTargetFramework);
            if (requested is not null)
            {
                string? compatibleMatch = distinctCandidates
                    .Select(candidate => new { Candidate = candidate, Parsed = ParseTargetFramework(candidate) })
                    .Where(x => x.Parsed is not null && x.Parsed.Value.Family == requested.Value.Family && x.Parsed.Value.Rank <= requested.Value.Rank)
                    .OrderByDescending(x => x.Parsed!.Value.Rank)
                    .Select(x => x.Candidate)
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(compatibleMatch))
                {
                    return compatibleMatch;
                }
            }
        }

        return distinctCandidates
            .Select(candidate => new { Candidate = candidate, Parsed = ParseTargetFramework(candidate) })
            .OrderByDescending(x => x.Parsed?.Rank ?? int.MinValue)
            .Select(x => x.Candidate)
            .First();
    }

    private static ParsedTargetFramework? ParseTargetFramework(string? targetFramework)
    {
        if (string.IsNullOrWhiteSpace(targetFramework))
        {
            return null;
        }

        if (targetFramework.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseVersionFamily("netstandard", targetFramework[11..]);
        }

        if (!targetFramework.StartsWith("net", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string suffix = targetFramework[3..];
        if (suffix.Contains('.', StringComparison.Ordinal))
        {
            return TryParseVersionFamily("net", suffix);
        }

        return int.TryParse(suffix, out int rank)
            ? new ParsedTargetFramework("netfx", rank)
            : null;
    }

    private static ParsedTargetFramework? TryParseVersionFamily(string family, string versionText)
    {
        return Version.TryParse(versionText, out Version? version)
            ? new ParsedTargetFramework(family, (version.Major * 100) + version.Minor)
            : null;
    }

    private readonly record struct ParsedTargetFramework(string Family, int Rank);

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
