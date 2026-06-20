using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        ProjectCollection projectCollection = new(globalProperties);
        Project project = LoadProject(projectCollection, projectFilePath, globalProperties);

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

    private static Project LoadProject(ProjectCollection projectCollection, string projectFilePath, Dictionary<string, string> globalProperties)
    {
        Project project = projectCollection.LoadProject(projectFilePath);
        if (!string.IsNullOrWhiteSpace(GetOptionalPropertyValue(project, nameof(ProjectLoadRequest.TargetFramework))))
        {
            return project;
        }

        string? targetFrameworks = GetOptionalPropertyValue(project, "TargetFrameworks");
        if (string.IsNullOrWhiteSpace(targetFrameworks))
        {
            return project;
        }

        string? firstTargetFramework = targetFrameworks
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstTargetFramework))
        {
            return project;
        }

        projectCollection.UnloadAllProjects();
        globalProperties[nameof(ProjectLoadRequest.TargetFramework)] = firstTargetFramework;
        return projectCollection.LoadProject(projectFilePath);
    }

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
