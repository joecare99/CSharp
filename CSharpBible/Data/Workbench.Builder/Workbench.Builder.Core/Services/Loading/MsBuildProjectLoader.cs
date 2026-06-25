using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Runtime.CompilerServices;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;

namespace Workbench.Builder.Core.Services.Loading;

/// <summary>
/// Loads SDK-style project files through MSBuild evaluation and maps the evaluated data into host-neutral builder models.
/// </summary>
public sealed class MsBuildProjectLoader : IProjectLoader
{
    private static readonly object SyncRoot = new();
    private static bool s_isLocatorRegistered;

    internal static Func<bool> IsMsBuildLocatorRegisteredOverride { get; set; } = static () => MSBuildLocator.IsRegistered;

    internal static Func<string?> FindDotNetSdkPathOverride { get; set; } = FindDotNetSdkPath;

    internal static Func<IEnumerable<VisualStudioInstance>> QueryVisualStudioInstancesOverride { get; set; } =
        static () => MSBuildLocator.QueryVisualStudioInstances();

    /// <inheritdoc/>
    public LoadedProjectModel Load(ProjectLoadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ProjectFilePath))
        {
            throw new ArgumentException("A project file path is required.", nameof(request));
        }

        EnsureMsBuildRegistered();

        string projectFilePath = Path.GetFullPath(request.ProjectFilePath);
        if (!File.Exists(projectFilePath))
        {
            throw new FileNotFoundException("The specified project file could not be found.", projectFilePath);
        }

        return LoadCore(request, projectFilePath);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static LoadedProjectModel LoadCore(ProjectLoadRequest request, string projectFilePath)
    {
        string projectDirectory = Path.GetDirectoryName(projectFilePath) ?? Directory.GetCurrentDirectory();

        Dictionary<string, string> globalProperties = CreateGlobalProperties(request);
        Project project = LoadProject(projectFilePath, globalProperties);

        ProjectPropertySet properties = new(
            assemblyName: GetRequiredPropertyValue(project, "AssemblyName", Path.GetFileNameWithoutExtension(projectFilePath)),
            rootNamespace: GetRequiredPropertyValue(project, "RootNamespace", Path.GetFileNameWithoutExtension(projectFilePath)),
            targetFramework: GetRequiredPropertyValue(project, "TargetFramework", string.Empty),
            outputType: GetOptionalPropertyValue(project, "OutputType"),
            langVersion: GetOptionalPropertyValue(project, "LangVersion"),
            nullable: GetOptionalPropertyValue(project, "Nullable"),
            defineConstants: GetOptionalPropertyValue(project, "DefineConstants"),
            implicitUsings: GetOptionalPropertyValue(project, "ImplicitUsings"),
            isPackable: GetOptionalPropertyValue(project, "IsPackable"),
            isTestProject: GetOptionalPropertyValue(project, nameof(ProjectPropertySet.IsTestProject)),
            configuration: GetOptionalPropertyValue(project, "Configuration"),
            runtimeIdentifier: GetOptionalPropertyValue(project, "RuntimeIdentifier"),
            outputPath: GetOptionalPropertyValue(project, "OutputPath"),
            intermediateOutputPath: GetOptionalPropertyValue(project, "IntermediateOutputPath"),
            projectAssetsFile: GetOptionalPropertyValue(project, "ProjectAssetsFile"));

        return new LoadedProjectModel(
            request,
            projectFilePath,
            projectDirectory,
            properties,
            GetCompileItems(project),
            GetProjectReferences(project),
            GetPackageReferences(project),
            IsSdkStyle(project),
            Array.Empty<BuildDiagnostic>());
    }

    private static void EnsureMsBuildRegistered()
    {
        if (s_isLocatorRegistered)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (s_isLocatorRegistered)
            {
                return;
            }

            if (!IsMsBuildLocatorRegisteredOverride())
            {
                string? sdkPath = FindDotNetSdkPathOverride();
                if (!string.IsNullOrWhiteSpace(sdkPath))
                {
                    MSBuildLocator.RegisterMSBuildPath(sdkPath);
                }
                else
                {
                    IEnumerable<VisualStudioInstance> instances = QueryVisualStudioInstancesOverride()
                        .OrderByDescending(static instance => instance.Version);
                    VisualStudioInstance? instance = instances.FirstOrDefault();
                    if (instance is null)
                    {
                        throw new InvalidOperationException("No MSBuild instance or .NET SDK path could be detected for registration.");
                    }

                    MSBuildLocator.RegisterInstance(instance);
                }
            }

            s_isLocatorRegistered = true;
        }
    }

    internal static void ResetForTests()
    {
        IsMsBuildLocatorRegisteredOverride = static () => MSBuildLocator.IsRegistered;
        FindDotNetSdkPathOverride = FindDotNetSdkPath;
        QueryVisualStudioInstancesOverride = static () => MSBuildLocator.QueryVisualStudioInstances();
        s_isLocatorRegistered = false;
    }

    private static string? FindDotNetSdkPath()
    {
        int currentRuntimeMajor = Environment.Version.Major;
        List<string> dotNetRoots = new();
        string? dotNetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrWhiteSpace(dotNetRoot))
        {
            dotNetRoots.Add(dotNetRoot);
        }

        string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        if (!string.IsNullOrWhiteSpace(programFiles))
        {
            dotNetRoots.Add(Path.Combine(programFiles, "dotnet"));
        }

        string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        if (!string.IsNullOrWhiteSpace(programFilesX86))
        {
            dotNetRoots.Add(Path.Combine(programFilesX86, "dotnet"));
        }

        foreach (string rootPath in dotNetRoots.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            string sdkRootPath = Path.Combine(rootPath, "sdk");
            if (!Directory.Exists(sdkRootPath))
            {
                continue;
            }

            DirectoryInfo? selectedDirectory = new DirectoryInfo(sdkRootPath)
                .EnumerateDirectories()
                .Select(static directory => new { Directory = directory, Version = TryParseSdkVersion(directory.Name) })
                .Where(static candidate => candidate.Version is not null && File.Exists(Path.Combine(candidate.Directory.FullName, "Microsoft.Build.dll")))
                .OrderByDescending(candidate => candidate.Version!.Major == currentRuntimeMajor)
                .ThenByDescending(static candidate => candidate.Version)
                .Select(static candidate => candidate.Directory)
                .FirstOrDefault();
            if (selectedDirectory is not null)
            {
                return selectedDirectory.FullName;
            }
        }

        return null;
    }

    private static Version? TryParseSdkVersion(string directoryName)
    {
        string[] parts = directoryName.Split('-', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return Version.TryParse(parts[0], out Version? version) ? version : null;
    }

    private static Dictionary<string, string> CreateGlobalProperties(ProjectLoadRequest request)
    {
        Dictionary<string, string> globalProperties = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Configuration"] = string.IsNullOrWhiteSpace(request.Configuration) ? "Debug" : request.Configuration,
        };

        if (!string.IsNullOrWhiteSpace(request.TargetFramework))
        {
            globalProperties[nameof(ProjectLoadRequest.TargetFramework)] = request.TargetFramework;
        }

        if (!string.IsNullOrWhiteSpace(request.RuntimeIdentifier))
        {
            globalProperties[nameof(ProjectLoadRequest.RuntimeIdentifier)] = request.RuntimeIdentifier;
        }

        return globalProperties;
    }

    private static Project LoadProject(string projectFilePath, Dictionary<string, string> globalProperties)
    {
        using ProjectCollection initialProjectCollection = new(globalProperties);
        Project project = initialProjectCollection.LoadProject(projectFilePath);
        if (!string.IsNullOrWhiteSpace(GetOptionalPropertyValue(project, nameof(ProjectLoadRequest.TargetFramework))))
        {
            return project;
        }

        string? targetFrameworks = GetOptionalPropertyValue(project, "TargetFrameworks");
        if (string.IsNullOrWhiteSpace(targetFrameworks))
        {
            return project;
        }

        string? selectedTargetFramework = SelectTargetFramework(targetFrameworks);
        if (string.IsNullOrWhiteSpace(selectedTargetFramework))
        {
            return project;
        }

        globalProperties[nameof(ProjectLoadRequest.TargetFramework)] = selectedTargetFramework;

        using ProjectCollection targetFrameworkProjectCollection = new(globalProperties);
        return targetFrameworkProjectCollection.LoadProject(projectFilePath);
    }

    private static string? SelectTargetFramework(string targetFrameworks)
    {
        string[] candidates = targetFrameworks
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(static targetFramework => !string.IsNullOrWhiteSpace(targetFramework))
            .ToArray();
        if (candidates.Length == 0)
        {
            return null;
        }

        string? builderTargetFramework = GetBuilderTargetFramework();
        if (!string.IsNullOrWhiteSpace(builderTargetFramework))
        {
            string? exactMatch = candidates.FirstOrDefault(
                candidate => string.Equals(candidate, builderTargetFramework, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(exactMatch))
            {
                return exactMatch;
            }
        }

        ParsedTargetFramework? builderFramework = ParseTargetFramework(builderTargetFramework);

        return candidates
            .Select(candidate => new { TargetFramework = candidate, Parsed = ParseTargetFramework(candidate) })
            .Where(static candidate => candidate.Parsed is not null)
            .Where(candidate => builderFramework is null || IsSupportedByBuilder(candidate.Parsed!, builderFramework))
            .OrderByDescending(candidate => GetTargetFrameworkRank(candidate.Parsed!))
            .Select(candidate => candidate.TargetFramework)
            .FirstOrDefault()
            ?? candidates[0];
    }

    private static string? GetBuilderTargetFramework()
    {
        object[] attributes = typeof(MsBuildProjectLoader).Assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), inherit: false);
        TargetFrameworkAttribute? attribute = attributes.OfType<TargetFrameworkAttribute>().FirstOrDefault();
        if (attribute is null || string.IsNullOrWhiteSpace(attribute.FrameworkName))
        {
            return null;
        }

        FrameworkName frameworkName;
        try
        {
            frameworkName = new FrameworkName(attribute.FrameworkName);
        }
        catch (ArgumentException)
        {
            return null;
        }

        return frameworkName.Identifier switch
        {
            ".NETCoreApp" when frameworkName.Version.Major >= 5 => $"net{frameworkName.Version.Major}.{frameworkName.Version.Minor}",
            ".NETStandard" => $"netstandard{frameworkName.Version.Major}.{frameworkName.Version.Minor}",
            ".NETFramework" => $"net{frameworkName.Version.Major}{frameworkName.Version.Minor}{frameworkName.Version.Build}",
            _ => null,
        };
    }

    private static bool IsSupportedByBuilder(ParsedTargetFramework candidate, ParsedTargetFramework builder)
    {
        if (candidate.Family == TargetFrameworkFamily.NetFramework)
        {
            return true;
        }

        if (candidate.Family == TargetFrameworkFamily.NetStandard)
        {
            return true;
        }

        return candidate.Family == builder.Family && candidate.Version <= builder.Version;
    }

    private static long GetTargetFrameworkRank(ParsedTargetFramework targetFramework)
    {
        int familyRank = targetFramework.Family switch
        {
            TargetFrameworkFamily.ModernNet => 3,
            TargetFrameworkFamily.NetFramework => 2,
            TargetFrameworkFamily.NetStandard => 1,
            _ => 0,
        };

        return ((long)familyRank * 1_000_000_000L)
            + ((long)targetFramework.Version.Major * 1_000_000L)
            + ((long)targetFramework.Version.Minor * 1_000L)
            + targetFramework.Version.Build;
    }

    private static ParsedTargetFramework? ParseTargetFramework(string? targetFramework)
    {
        if (string.IsNullOrWhiteSpace(targetFramework))
        {
            return null;
        }

        if (targetFramework.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
        {
            return TryCreateParsedTargetFramework(TargetFrameworkFamily.NetStandard, targetFramework[11..]);
        }

        if (!targetFramework.StartsWith("net", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string versionText = targetFramework[3..];
        if (versionText.Contains('.'))
        {
            return TryCreateParsedTargetFramework(TargetFrameworkFamily.ModernNet, versionText);
        }

        if (versionText.Length is < 2 or > 3 || !versionText.All(char.IsDigit))
        {
            return null;
        }

        int major = int.Parse(versionText[..1]);
        int minor = int.Parse(versionText[1..2]);
        int build = versionText.Length == 3 ? int.Parse(versionText[2..3]) : 0;
        return new ParsedTargetFramework(TargetFrameworkFamily.NetFramework, new Version(major, minor, build));
    }

    private static ParsedTargetFramework? TryCreateParsedTargetFramework(TargetFrameworkFamily family, string versionText)
    {
        return Version.TryParse(versionText, out Version? version)
            ? new ParsedTargetFramework(family, version)
            : null;
    }

    private enum TargetFrameworkFamily
    {
        ModernNet,
        NetFramework,
        NetStandard,
    }

    private sealed record ParsedTargetFramework(TargetFrameworkFamily Family, Version Version);

    private static IReadOnlyList<CompileItemInfo> GetCompileItems(Project project)
    {
        List<CompileItemInfo> compileItems = new();
        foreach (ProjectItem item in project.GetItems("Compile"))
        {
            string filePath = GetFullPath(item, item.EvaluatedInclude);
            compileItems.Add(new CompileItemInfo(item.EvaluatedInclude, filePath, File.Exists(filePath)));
        }

        return compileItems;
    }

    private static IReadOnlyList<ProjectReferenceInfo> GetProjectReferences(Project project)
    {
        List<ProjectReferenceInfo> projectReferences = new();
        foreach (ProjectItem item in project.GetItems("ProjectReference"))
        {
            string projectFilePath = GetFullPath(item, item.EvaluatedInclude);
            projectReferences.Add(new ProjectReferenceInfo(item.EvaluatedInclude, projectFilePath, File.Exists(projectFilePath)));
        }

        return projectReferences;
    }

    private static IReadOnlyList<PackageReferenceInfo> GetPackageReferences(Project project)
    {
        List<PackageReferenceInfo> packageReferences = new();
        foreach (ProjectItem item in project.GetItems("PackageReference"))
        {
            packageReferences.Add(
                new PackageReferenceInfo(
                    packageId: item.EvaluatedInclude,
                    version: item.GetMetadataValue("Version"),
                    privateAssets: item.GetMetadataValue("PrivateAssets")));
        }

        return packageReferences;
    }

    private static string GetFullPath(ProjectItem item, string fallbackPath)
    {
        string fullPath = item.GetMetadataValue("FullPath");
        if (!string.IsNullOrWhiteSpace(fullPath))
        {
            return fullPath;
        }

        string projectDirectory = Path.GetDirectoryName(item.Project.FullPath) ?? Directory.GetCurrentDirectory();
        return Path.GetFullPath(Path.Combine(projectDirectory, fallbackPath));
    }

    private static bool IsSdkStyle(Project project)
    {
        if (!string.IsNullOrWhiteSpace(project.Xml.Sdk))
        {
            return true;
        }

        return project.Xml.Imports.Any(static import => !string.IsNullOrWhiteSpace(import.Sdk));
    }

    private static string GetRequiredPropertyValue(Project project, string propertyName, string fallbackValue)
    {
        string? value = GetOptionalPropertyValue(project, propertyName);
        return string.IsNullOrWhiteSpace(value) ? fallbackValue : value;
    }

    private static string? GetOptionalPropertyValue(Project project, string propertyName)
    {
        string value = project.GetPropertyValue(propertyName);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
