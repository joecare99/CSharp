using System;
using System.IO;
using System.Text;

namespace AA98.DevOpsPlanning.Host.Services;

/// <summary>
/// Creates a local DevOps planning shared project and default planning folder structure.
/// </summary>
public sealed class DevOpsPlanningProjectScaffolder : IDevOpsPlanningProjectScaffolder
{
    /// <inheritdoc/>
    public DevOpsPlanningProjectScaffoldResult Create(string planningProjectRootPath)
    {
        if (string.IsNullOrWhiteSpace(planningProjectRootPath))
        {
            return new DevOpsPlanningProjectScaffoldResult(false, "Planning project path is required.");
        }

        string normalizedRootPath = Path.GetFullPath(planningProjectRootPath);
        Directory.CreateDirectory(normalizedRootPath);

        string projectName = ResolveProjectName(normalizedRootPath);
        string projectFilePath = Path.Combine(normalizedRootPath, $"{projectName}.shproj");
        string projectItemsPath = Path.Combine(normalizedRootPath, $"{projectName}.projitems");

        CreatePlanningFolders(normalizedRootPath);
        EnsureProjectItemsFile(projectItemsPath, projectName);
        EnsureSharedProjectFile(projectFilePath, projectName);

        return new DevOpsPlanningProjectScaffoldResult(true, $"Planning project '{projectName}' is ready at '{normalizedRootPath}'.", normalizedRootPath, projectFilePath, projectItemsPath);
    }

    private static void CreatePlanningFolders(string rootPath)
    {
        Directory.CreateDirectory(Path.Combine(rootPath, "Epics"));
        Directory.CreateDirectory(Path.Combine(rootPath, "Features"));
        Directory.CreateDirectory(Path.Combine(rootPath, "BacklogItems"));
        Directory.CreateDirectory(Path.Combine(rootPath, "Tasks"));
    }

    private static void EnsureSharedProjectFile(string projectFilePath, string projectName)
    {
        if (File.Exists(projectFilePath))
        {
            return;
        }

        string projectGuid = Guid.NewGuid().ToString().ToUpperInvariant();
        string content = $"<Project>{Environment.NewLine}" +
            $"  <PropertyGroup Label=\"Globals\">{Environment.NewLine}" +
            $"    <ProjectGuid>{projectGuid}</ProjectGuid>{Environment.NewLine}" +
            $"    <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>{Environment.NewLine}" +
            $"  </PropertyGroup>{Environment.NewLine}" +
            $"  <Import Project=\"$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')\" />{Environment.NewLine}" +
            $"  <Import Project=\"$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\CodeSharing\\Microsoft.CodeSharing.Common.Default.props\" />{Environment.NewLine}" +
            $"  <Import Project=\"$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\CodeSharing\\Microsoft.CodeSharing.Common.props\" />{Environment.NewLine}" +
            $"  <PropertyGroup />{Environment.NewLine}" +
            $"  <Import Project=\"{projectName}.projitems\" Label=\"Shared\" />{Environment.NewLine}" +
            $"  <Import Project=\"$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\CodeSharing\\Microsoft.CodeSharing.CSharp.targets\" />{Environment.NewLine}" +
            $"</Project>{Environment.NewLine}";

        File.WriteAllText(projectFilePath, content, Encoding.UTF8);
    }

    private static void EnsureProjectItemsFile(string projectItemsPath, string projectName)
    {
        if (File.Exists(projectItemsPath))
        {
            return;
        }

        string sharedGuid = Guid.NewGuid().ToString().ToUpperInvariant();
        string content = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}" +
            $"<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">{Environment.NewLine}" +
            $"  <PropertyGroup>{Environment.NewLine}" +
            $"    <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>{Environment.NewLine}" +
            $"    <HasSharedItems>true</HasSharedItems>{Environment.NewLine}" +
            $"    <SharedGUID>{sharedGuid}</SharedGUID>{Environment.NewLine}" +
            $"  </PropertyGroup>{Environment.NewLine}" +
            $"  <PropertyGroup Label=\"Configuration\">{Environment.NewLine}" +
            $"    <Import_RootNamespace>{projectName}</Import_RootNamespace>{Environment.NewLine}" +
            $"  </PropertyGroup>{Environment.NewLine}" +
            $"  <ItemGroup>{Environment.NewLine}" +
            $"    <None Include=\"$(MSBuildThisFileDirectory)**\\*.md\" Exclude=\"$(MSBuildThisFileDirectory)bin\\**;$(MSBuildThisFileDirectory)obj\\**;$(MSBuildThisFileDirectory).vs\\**\" />{Environment.NewLine}" +
            $"  </ItemGroup>{Environment.NewLine}" +
            $"</Project>{Environment.NewLine}";

        File.WriteAllText(projectItemsPath, content, Encoding.UTF8);
    }

    private static string ResolveProjectName(string rootPath)
    {
        string folderName = Path.GetFileName(Path.TrimEndingDirectorySeparator(rootPath));
        return string.IsNullOrWhiteSpace(folderName) ? "DevOps" : folderName;
    }
}
